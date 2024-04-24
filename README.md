# 2023SHRProject
2023 Independent study researching reinforcement learning in Unity

## Table of Contents

- [2023SHRProject](#2023shrproject)
  - [Table of Contents](#table-of-contents)
  - [Unity Version](#unity-version)
  - [Authors](#Authors)
  - [Introduction](#Introduction)
  - [Acknowledgements](#Acknowledgements)
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
- [Mitchell Harris0n](https://github.com/mharrison7787)
- [Jonathan Riverra](https://github.com/jjriver1)

## Introduction

### Opening remarks/What the project is
This is the work of us (the three authors above) over the course of two semesters at  Millersville University. What this project is all about is we trainned AI Agents to Play a Tower Defense Game Using Reinforcement Learning. With that in mind the core of the project is two fold. One is the game itself. We based our game off the game series bloons tower defense. We made the game ourselves from scratch and on its own is a fully working game. Second is the AI component that plays our game. Using the resources below the AI can learn over time to play the game with the parameters you set. For further a explanation here are our published poster and papers on our research.
### Resources
[MIM Poster](/MUPapers/Made-In-Millersville-Poster.pdf):
This is our poster submitted for the made in millersville conference that goes over a brief snippet of each section of our paper. 
		
[PACISE Paper](/MUPapers/PACISE-PAPER.pdf): This is our full indepth detailed research paper on what we did for this project. It goes over the game, research process, developing the game, developing the AI, results and future enhancements we would make in the future. 

[MIM Paper](/MUPapers/Harrison-Final.pdf): This is a summarized more simplified version of what goes on in our project on a surface level. 

### Acknowledgements
We  would like to personally thank Dr. Chadd Hogg for getting us this far. Meeting with us weekly and being always avaible to guide us to the right path. If it weren't for him we would of not been able to gotten so far. So again thank you for all you did for us Dr. Hogg!!

## Author Notes && future featutes
This section goes into a little more detail on the end result of this project such as where the AI is currently at, additonal features we would of implemented, and certian bugs found in the project we would fix if we had more time. 

- **Add camo balloons and detection to towers:** Currently in our game we only have basic bloons types (red, blue, yellow, etc) and adding camo bloons would make the game and AI more interesting.
- **Fix Projectile collision/bug with certain tower types:** Durring gameplay, projectiles may stay longer than expected and glitch out when trying to pop a bloon that another tower is targeting or popped already. This issue can affect the overall performance. We have a temporary fix in place, where we delete the projectile after a few seconds, however a more permanent solution is probably needed. This bug can be found with the boomerang monkey, bomb shooter, and sometimes the sniper monkey.
- **Fix/verify object links in Scenes:** In the main menu scene and the some of the other debug scenes there may be missing objects links that can crash the game. Make sure to verify that all object links are in the right place.
- **Async Loading:** When building the game you may notice that a quick loading screen will pop up and go away after hitting 100%. This is due to an asynchronous loading script that allows the game to load resources in the background without freezing the game. Currently this script works as intended when switching from the main menu to the game scene but does not seem to work when restarting the game or going back to the main menu screen, it loads but does not hit 100% (it stays at 0%). This is most likely a visual bug but is still annoying to look at.
- **Sizing issues on larger resolutions:** When switching the games resolution you may notice some UI components do not scale properly. This can be fixed by adjusting the rect transform, specifically the anchor and pivot points. Most components are fixed but there may be some minor elements that still need adjustment (upgrade menu text, upgrade menu buttons, settings menu, main menu text..etc)
- **Upgrade system for the AI**: Currently for our AI we do not have a system set up in place for allowing the AI to choose its own upgrades based on gameplay. This could be a valuable addition to improve the AI's decision-making process and overall performance in the game.
- **More tower types for the AI:** Currently for our AI we are only giving it access to 2 tower types (dart monkey and sniper monkey). We ideally would like to give it more options to choose from but we need to ensure that the additional tower types are balanced and complement each other well. This would provide the AI with more strategic options and enhance its ability to adapt to different gameplay scenarios.
- **Balance tower types:** Most of the towers in the game are unbalanced and either cost to little or provide too much power for their cost. Adjusting the stats and costs of each tower would ensure a more fair and challenging gameplay experience for both players and the AI.
- **Clean up/delete unnecessary game files:** There are a number of files and game assets that can be deleted.
- **Upgrade menu functionality:** When pulling up the upgrade menu you may notice that you have to close it by clicking on the same monkey you opened it for, this can be a hassle for new users. Maybe adding a close button could improve the user experience. Or you could add a way for the user to click on the game scene to close the menu (kinda similar to the mobile game)
- **The AI Status:** The AI currently at a state of placing two towers: The Dart Monkey and The Sniper Monkey in realtively good spots. The AI if towers are placed well can get to the high 30s of the waves and usually fails by end. The average wave it would get to was around 20 with average placement of sniper and dart monkeys. 



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
  - ```pip install six``` 

## [Tensorflow](https://www.tensorflow.org/learn) Setup and Installation

### Install Tensorflow
Once ML agents and the game is working, its time for tensor flow to see the ongoing results. First install tensorflow in your enviorment using this command:
```
pip install tensorflow
```

### Yaml Configure
Once installed you will then go to your config yaml file to update the hyper paramters to your liking. Try testing around different configuration to get the AI to your liking to learn more such as the beta and epslon. For more information on the different parameters read the documentation. 

### Running the new Hyper Params
Once the hyper parameters are to your liking run the command
```
mlagents-learn .\configuration.yaml --run id <a name for the brain you want>
```
> Note: You may need to add at the end of the command --force as sometimes it does not run without it

### Run Tensorflow
Once this is running you can run this command
```
tensorboard --logdir .\results\
```
This will open a web port that will display the ongoing results of how your AI is doing.
