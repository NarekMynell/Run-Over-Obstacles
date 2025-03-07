## Project Details
* **Unity Version:** 2022.3.4f1
* **Render Pipeline:** Universal Render Pipeline
* **Mode:** 3D

## Description
The game features a road with certain obstacles placed on it. The ```Player``` runs along this road and jumps over the obstacles in such a way that, at the moment of being above the obstacle, the legs always maintain the same spread.  
The game includes a ```slider``` that allows adjusting the time scale to make the jump more visible.  
Obstacles can be moved along the road during runtime (in Unity's Editor), and this does not break the game mechanics—the player will always detect their position and perform an accurate jump. However, the obstacles should not be placed so close to each other that jumping over them becomes impossible.

## Architecture
The road is represented as a ```Mesh```.  
The player's running path is determined by another ```Mesh```, which consists of a list of sequential points and is passed as a parameter to the ```Road``` (in ```RoadData```). Then, in the ```Road``` script, an array of vectors is created from the path’s world coordinates and passed as a parameter to ```Road```’s ```SharedData```, which also stores the obstacle data.
~~~C#
RoadSharedData
    Vector3[] Path;
    ObstacleSharedData[] Obstacles;
~~~
Communication between the road and the player is managed through the ```RunManager```. With ```Road``` as a parameter, it sends an event about the road’s update, including the current ```Road```’s ```SharedData```. This event is received by the ```PlayerController```, which stores the ```Road```’s ```SharedData``` and starts the run by creating and transitioning to the ```PlayerRun``` state.

#### Run
Movement occurs along the path’s lines, segment by segment. The code performs precise calculations without approximations (the only approximation being the precision of ```float```), assuming the player passes over all lines and vertices without deviation or speed changes.  
In ```PlayerRun```, the distance to the nearest obstacle along the movement path is calculated at every moment.  
The player’s ```Animator``` has states for running and jumping. The running ```AnimationClip``` includes two events—for the left and right legs—at moments when they are ready to jump. When these events are triggered, the code checks if this is the last such position before the upcoming obstacle. If so, a jump is performed (from either the right or left leg), and the animation speed is adjusted simultaneously to ensure the legs have a fixed spread at the moment of being above the obstacle. After the jump, the animation returns to its normal speed.

## Getting Started
To use the project, follow one of these methods:  
* Clone it and open the folder as a new project via Unity Hub.  
* Download the ```Jumper.unitypackage``` package from the **Releases** section and import it into an existing project.  
The **Releases** section also includes an APK file.