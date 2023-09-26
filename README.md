# 2023SHRProject
2023 Independent study researching reinforcement learning in Unity

## Table of Contents

- [2023SHRProject](#2023shrproject)
  - [Table of Contents](#table-of-contents)
  - [Unity Version](#unity-version)
  - [Authors](#authors)
  - [ML Agents Setup and Installation](#ml-agents-setup-and-installation)
    - [Install Unity Version 2022.3.10f1](#install-unity-version-2022310f1)
    - [Install Python 3.8.8](#install-python-388)
    - [Clone the Repo](#clone-the-repo)
    - [Install the com.unity.ml-agents Unity Package](#install-the-comunityml-agents-unity-package)
    - [Create a Python Virtual Environment](#create-a-python-virtual-environment)
    - [Open the Python Virtual Environment](#open-the-python-virtual-environment)
    - [Install the mlagents Python Package](#install-the-mlagents-python-package)
      - [Install PyTorch](#install-pytorch)
      - [Install mlagents](#install-mlagents)
      - [Verify Installation](#verify-installation)
      - [Troubleshooting ML-Agents Installation](#troubleshooting-ml-agents-installation)

## Unity Version
2022.3.10f1

## Authors
- [Justin Stevens](https://github.com/JSteve0)
- [Mitchell Harris0n]()
- [Jonathan Riverra]()

## [ML Agents](https://unity.com/products/machine-learning-agents) Setup and Installation

### Install Unity Version 2022.3.10f1
[Download](https://unity.com/download) and install Unity Hub. Once Installed, open Unity Hub and install Unity Version 2022.3.10f1.

> Note: The dependencies should work with Unity Version >= 2022. However, it has been tested and confirmed to work with Unity Version 2022.3.10f1.

### Install Python 3.8.8
1. [Download](https://www.python.org/downloads/release/python-388/) and install Python 3.8.8. 
2. Add python to your path for ease of use. Follow this [guide](https://realpython.com/add-python-to-path/) for more information.
3. Verify you have the right version of Python installed by running the following command in a shell:
```
python --version
```

> Note: Other version of Python 3.8 may work. However, it has been tested and confirmed to work on 3.8.8. 
> Do not use version 3.9 and above as that is know not to work with the correct version of PyTorch.

### Clone the Repo
Clone the repo with the following command
```
git clone https://github.com/mucsci-students/2023SHRProject.git
```

### Install the com.unity.ml-agents Unity Package
1. Open the repo in Unity
2. In the navbar, open Window -> Package Manager
3. Find ML Agents and install version 2.0.1 if it is not already installed

### Create a Python Virtual Environment
1. Install virtualenv. To do so, open a shell and run the following code:
```
pip install virtualenv
```
2. Create a folder for the virtual environment. To do so, run the following command:
```
python -m venv <virtual-environment-name>
```
I named mine venv, so I did:
```
python -m venv <virtual-environment-name>
```
3. Verify that a <virtual-environment-name> folder was created.

### Open the Python Virtual Environment
1. Open your venv environment. To do so, use the following command:
```
cd <virtual-environment-name>
```
If you named yours venv like I did then do:
```
cd venv
```
2. Run one of the following commands based on your shell:
    - Powershell:
      - ```./Scripts/Activate.ps1```
    - Windows Command Prompt (Still need to test)
      - ```./Scripts/activate.bat```
    - Mac Terminal (Still need to test)
      - ```./Scripts/activate```

### Install the mlagents Python Package
Make sure you are in your virtual environment and run:
```
python --version
```
Confirm that you have version 3.8.8. If not you will need to delete your venv and restart all the python related steps. If your python version is correct and you are inside the venv, continue along with the rest of the steps below.

#### Install PyTorch
Run the following command in your venv:
```
pip install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html
```
> Note: If this fails, you most likely have an incompatible version of python installed

#### Install mlagents
Run the following command in your venv:
```
python -m pip install mlagents==0.29.0
```

#### Verify Installation
1. Run the following command:
```
mlagents-learn --help
```
Verify that there are no errors

2. Next run the following command:
```
mlagents-learn
```
Verify that there are no errors and that it begins listening on port 5004.

> Note: The port may be different, but as long as it is listening on a port without errors, your installation should be working.

#### Troubleshooting ML-Agents Installation
- Problem: An error related to `protobuf` in the venv
  - Solution: Run the following command in the venv:
    - ```pip install protobuf==3.20.*```
- Problem: An error related to `six` in the venv
  - Solution: Run the following command in the venv:
    - ```pip install six```