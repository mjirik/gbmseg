import sys

from annotation_reading import load_image
from settings import load_settings

settings = load_settings(sys.argv[1])

img = load_image(sys.argv[2])

measurement_rate = float(settings["measurement_rate"])
threshold = float(settings["measurement_threshold"])
min_size = int(settings["min_membrane_size"])
max_size = int(settings["max_membrane_size"])

isMembrane = False
membraneSize = 0

sizes = []

for y in range(128, 4095, 256):
    for x in range(0, 4095):
        if img[y, x] >= threshold:
            if isMembrane:
                membraneSize += 1
            else:
                isMembrane = True
        else:
            if isMembrane:
                if min_size <= membraneSize <= max_size:
                    sizes.append(membraneSize)
                isMembrane = False
                membraneSize = 0

isMembrane = False
membraneSize = 0

for x in range(128, 4095, 256):
    for y in range(0, 4095):
        if img[y, x] >= threshold:
            if isMembrane:
                membraneSize += 1
            else:
                isMembrane = True
        else:
            if isMembrane:
                if min_size <= membraneSize <= max_size:
                    sizes.append(membraneSize)
                isMembrane = False
                membraneSize = 0

result = 0
for size in sizes:
    result += size
result /= len(sizes)
result *= measurement_rate

file = open(sys.argv[3], "w+")
file.write(str(result))
file.close()
