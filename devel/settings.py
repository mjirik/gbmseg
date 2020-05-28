import json


def load_settings(filepath):
    f = open(filepath)
    settings = json.load(f)
    f.close()
    return settings
