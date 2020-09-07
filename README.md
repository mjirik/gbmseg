  
[![Build Status](https://travis-ci.org/mjirik/gbmseg.svg?branch=master)](https://travis-ci.org/mjirik/gbmseg)
[![Coverage Status](https://coveralls.io/repos/github/mjirik/gbmseg/badge.svg?branch=master)](https://coveralls.io/github/mjirik/gbmseg?branch=master)
[![PyPI version](https://badge.fury.io/py/gbmseg.svg)](http://badge.fury.io/py/gbmseg)


gbmseg

GBM segmentation


# Install

Tested on Ubuntu 20.4 (LTS)

## Prepare user account

```bash
APPUSERNAME=gbmseg
APPDIR="/home/$APPUSERNAME_dotnet"
sudo useradd --system --gid webapps --home /webapps/gbmseg_dotnet --shell /bin/bash gbmseg
sudo mkdir /webapps/gbmseg_dotnet
sudo chown -R gbmseg:users /webapps/gbmseg_dotnet/
sudo chmod -R g+w /webapps/gbmseg_dotnet/
```
 
## Install prerequisities

* imagemagick is required by conda maybe it is not necessary

[Install with snap](https://docs.microsoft.com/cs-cz/dotnet/core/install/linux-ubuntu#apt-troubleshooting)

```bash
sudo apt-get install snapd graphicsmagick-imagemagick-compat
sudo snap install dotnet-sdk --classic --channel=3.1
sudo snap alias dotnet-sdk.dotnet dotnet
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

## Get the project dir and other specific things

```bash
sudo su $APPUSERNAME
```

```bash
cd ~
mkdir logs
https://github.com/mjirik/gbmseg.git
pip install gdown
gdown https://drive.google.com/uc?id=1yjXluRB8Y8N1e5wG6h2WyhVXHnZvKxBU
unzip deploy.zip
conda env create -n gbmseg -f gbmseg/environment.yml
exit
```

```bash
sudo cp gbmseg/deploy_confs/supervisor/*.conf /etc/supervisor/conf.d/
```

Update `supervisor`:

```bash
sudo supervisorctl reread
sudo supervisorctl update
```

