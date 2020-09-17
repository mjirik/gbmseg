import os
import sys
import numpy as np
import skimage.io

from annotation_reading import load_image
from settings import load_settings
from unet import deserialize_unet

os.environ['CUDA_VISIBLE_DEVICES'] = '-1'

# Load settings from JSON file
settings = load_settings(sys.argv[1])
model = deserialize_unet(settings["model_path"], settings["weights_path"])

img = load_image(sys.argv[2])

# Cut image
cut_array = []
for x in range(0, img.shape[1] - 255, 256):
    for y in range(0, img.shape[0] - 255, 256):
        cut = img[y: y + 256, x: x + 256]
        cut_array.append(cut)

# Process image

x_r = np.array(cut_array)
x_r = x_r.reshape([x_r.shape[0], 256, 256, 1])
res = model.predict(x_r)

out = np.zeros([4096, 4096])

# Assemble image
for i in range(0, res.shape[0]):
    top = int(i % 16) * 256
    left = int(i / 16) * 256

    for x in range(0, 256):
        for y in range(0, 256):
            out[top + y, left + x] = res[i, y, x]

skimage.io.imsave(sys.argv[3], out)
