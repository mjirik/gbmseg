import skimage.transform
import numpy as np


def augment_images(images):
    augmented = []

    for img in images:
        augmented.append(img)
        augmented.append(skimage.transform.rotate(img, 90))
        augmented.append(skimage.transform.rotate(img, 180))
        augmented.append(skimage.transform.rotate(img, 270))
        augmented.append(img[:, ::-1])
        augmented.append(img[::-1, :])

    return augmented


def add_padding(images, padding):
    result = []

    for img in images:
        img_pad = np.zeros([img.shape[0] + 2 * padding, img.shape[1] + 2 * padding])
        img_pad[padding:img.shape[0]+padding, padding:img.shape[1]+padding] = img[::, ::]
        resized = skimage.transform.resize(img_pad, (img.shape[0], img.shape[1]))
        result.append(resized)

    return result
