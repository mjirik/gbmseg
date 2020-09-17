import numpy as np
import sys
import h5py
import os
from annotation_reading import load_annotated_dataset, cut_images
from augmentation import augment_images, add_padding
from unet import train_unet, create_unet, serialize_unet, deserialize_unet
from settings import load_settings
from matplotlib import pyplot as plt

# This is here because I fucked up my GPU and I need TF to use CPU then
os.environ['CUDA_VISIBLE_DEVICES'] = '-1'

# Load settings from JSON file
settings = load_settings(sys.argv[1])

# Create or load UNET model
if os.path.exists(settings["model_path"]) and os.path.exists(settings["weights_path"]):
    model = deserialize_unet(settings["model_path"], settings["weights_path"])
else:
    model = create_unet(settings["img_width"], settings["img_height"])

# Parse of load dataset
if os.path.exists(settings["dataset_path"]):
    h5f = h5py.File(settings["dataset_path"], 'r')
    x = h5f['x'][:]
    y = h5f['y'][:]
else:
    # Load annotated images
    x, y = load_annotated_dataset(settings["csv_file_path"], settings["images_dir_path"])
    # Save to HDF5
    h5f = h5py.File(settings["dataset_path"], 'w')
    h5f.create_dataset('x', data=x)
    h5f.create_dataset('y', data=y)
    h5f.close()

initial_epoch = 0
initial_image = 0

# Load current progress from file
if os.path.exists(settings["current_state_path"]):
    f = open(settings["current_state_path"], "r")
    initial_epoch = int(f.readline())
    initial_image = int(f.readline())
    f.close()

# Test model
# Cut images
dataset = x
for j in range(0, dataset.shape[0] - 1):
    xx = dataset[j]
    cut_array = []
    for x in range(0, xx.shape[1] - 255, 256):
        for y in range(0, xx.shape[0] - 255, 256):
            cut = xx[y: y + 256, x: x + 256]
            cut_array.append(cut)

    x_r = np.array(cut_array)
    x_r = x_r.reshape([x_r.shape[0], 256, 256, 1])
    res = model.predict(x_r)

    out = np.zeros([4096, 4096])

    for i in range(0, res.shape[0]):
        top = int(i % 16) * 256
        left = int(i / 16) * 256

        for x in range(0, 256):
            for y in range(0, 256):
                out[top + y, left + x] = res[i, y, x]

    plt.imshow(out)
    #plt.show()
    plt.savefig(f'data/{j}.png')

images_done = 0
epochs_done = 0

# Because dataset is split into sets by original image, we cannot use TF built in epochs mechanism
for epoch in range(initial_epoch, settings["epochs_count"]):
    for i in range(initial_image, len(x)):
        xx = [x[i]]
        yy = [y[i]]

        # Cut images
        xx = cut_images(xx, settings["img_width"], settings["img_height"], settings["img_xstep"], settings["img_ystep"])
        yy = cut_images(yy, settings["img_width"], settings["img_height"], settings["img_xstep"], settings["img_ystep"])

        # Augment dataset
        xx = augment_images(xx)
        yy = augment_images(yy)

        xx = add_padding(xx, settings["img_padding"])
        yy = add_padding(yy, settings["img_padding"])

        # Train NN
        train_unet(model, xx, yy, settings["img_width"], settings["img_height"])
        # Save model and weights
        serialize_unet(model, settings["model_path"], settings["weights_path"])

        # Save current progress
        f = open(settings["current_state_path"], "w")
        f.write(f"{epoch}\n")
        f.write(f"{i}\n")
        f.close()

        images_done += 1

        if 0 < settings["max_job_images"] <= images_done:
            break

    epochs_done += 1

    if 0 < settings["max_job_epochs"] <= epochs_done:
        break
