  
[![Build Status](https://travis-ci.org/mjirik/gbmseg.svg?branch=master)](https://travis-ci.org/mjirik/gbmseg)
[![Coverage Status](https://coveralls.io/repos/github/mjirik/gbmseg/badge.svg?branch=master)](https://coveralls.io/github/mjirik/gbmseg?branch=master)
[![PyPI version](https://badge.fury.io/py/gbmseg.svg)](http://badge.fury.io/py/gbmseg)


gbmseg

GBM segmentation


# Install

Tested on Ubuntu 20.4 (LTS)

## Prepare user account

```bash
$APPUSERNAME=gbmseg
sudo useradd --system --gid webapps --home /webapps/gbmseg_dotnet gbmseg
sudo mkdir /webapps/gbmseg_dotnet
sudo usermod --shell /bin/bash gbmseg
sudo chown -R gbmseg:users /webapps/gbmseg_dotnet/
sudo chmod -R g+w /webapps/gbmseg_dotnet/
```
 
## Install prerequisities

* imagemagick is required by conda maybe it is not necessary


```bash
sudo apt-get install snapd graphicsmagick-imagemagick-compat
sudo snap install dotnet-runtime-31
```

```bash
sudo su $APPUSERNAME
```

```bash
cd ~
wget https://raw.githubusercontent.com/mjirik/discon/master/tools/install_conda.sh && source install_conda.sh
miniconda3/bin/conda init
exit
```

## Get the project dir

```bash
sudo su $APPUSERNAME
```

```bash
cd ~
https://github.com/mjirik/gbmseg.git
pip install gdown
```

