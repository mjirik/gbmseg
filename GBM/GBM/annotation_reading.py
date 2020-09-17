import pandas as pd
from pathlib import Path
import json
import matplotlib.pyplot as plt
import numpy as np
from matplotlib.path import Path as mplPath
import skimage.io


def load_annotated_dataset(csv_file_path, images_directory_path):
    csv_path = Path(csv_file_path)
    df = pd.read_csv(csv_path)

    originals = []
    masks = []
    i = 0

    for fn in df["filename"].unique():
        i += 1

        img_file_path = f"{images_directory_path}/{fn}"
        img = skimage.io.imread(img_file_path, as_gray=True)

        img_mask = np.zeros([img.shape[1], img.shape[0]])
        dirty = False

        for region in df[df["filename"] == fn].region_shape_attributes:
            region_shape_attributes = json.loads(region)

            # I found out, that CSV contains some strange areas
            if "all_points_x" not in region_shape_attributes or "all_points_y" not in region_shape_attributes:
                continue

            plt.imshow(img, cmap="gray")
            polygon_x = region_shape_attributes["all_points_x"]
            polygon_y = region_shape_attributes["all_points_y"]

            polygon = list(zip(polygon_y, polygon_x))
            poly_path = mplPath(polygon)
            x, y = np.mgrid[
                   : img.shape[0], : img.shape[1]
                   ]
            coors = np.hstack(
                (x.reshape(-1, 1), y.reshape(-1, 1))
            )

            mask = poly_path.contains_points(coors)
            mask = mask.reshape([img.shape[0], img.shape[1]])
            dirty = True

            img_mask = np.logical_xor(img_mask, mask)

        if dirty:
            originals.append(img)
            plt.imshow(img, cmap="gray")
            plt.show()

            masks.append(img_mask)
            plt.imshow(img_mask, cmap="gray")
            plt.show()

    return originals, masks


def cut_images(images, width, height, xstep, ystep):
    cut_array = []

    for img in images:
        for x in range(0, img.shape[1]-width, xstep):
            for y in range(0, img.shape[0]-height, ystep):
                cut = img[y: y + height, x: x + width]
                cut_array.append(cut)

    return cut_array


def load_image(filepath):
    img = skimage.io.imread(filepath, as_gray=True)
    return img

