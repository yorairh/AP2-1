# Advanced Programming #2 - First Assignment
## Yorai Roth Hazan, Ori Dabush
***
## First part - Documentation of the features of the Project
* When you run the project a window with a button will open, the button will take you to the file explorer and ou will need to click on a xml file.
The same thing will happen with a csv file afterwards.
* After you enter the two files an upload button will apear. when you click on the upload button a few things will happen:
    * The simulator will start running in a different window
    * A control pannel to the video will open with buttons to control the speed of the video and to stop it.
    * Information pannel (top right) that shows the joystick of the pilot, the rudder and the throttle. In addition to that you can see the height of the plane, the speed, the orientation and the RPY (roll, pitch, yaw) under the joystick in clocks and numbers formats.
    * A list of all the data types (top middle) that you can track, when you choose one of them a graph with information about this type will apear, another graph will apear which contains a corelated type. 
    * The big graph in the middle of the screen will be the both values of the graphs when the X is tracked by the first graph and the Y by the second.
    * The big graph will also track exceptions with different color using a library that detect exceptions.
***
## Second Part - Documentation of the files of the project
### A link to the structure of our project -
Our project is built like a MVVM structure: (view, viewmodel, model)
* We have one view (main window) and for each part of the project a viewmodel and a model interfaces.
* We have them all extending another interfaces that we named INotifyPropertyChanged - for data binding.
* For each part of the project (graphs, videocontrol, info, joystick...) we created A viewmodel and model implementions that manage the changes in those subjects - The NotifyPropertyChanged event in  the INotifyProperyChanged interface help with the binding between the three classes (view, viewmodel, model).
* In the NotifyPropertyChanged we Send An EventArgs which is an interface that we implemented into classes for all the different subjects in the project. Each eventArgs contains info about what changed and that is how the whole project notify itself about changes.
* We created a different thread that read the csv file lines and by that we can change the different varriables according to the file info.
* We Used tcp socket to send the lines of the file to the flight simulator.
***
## Third Part - Installations required
* The csv file must be in a repository that contains only english letters. (Or it will not work)
* Download [Visual Studio](https://visualstudio.microsoft.com/) (Latest release) and 
[FlightGear](https://www.flightgear.org/)
* NuGet Packages: OxyPlot.Wpf, OxyPlot.Core
* Framework: .NET framework 4.7.2
***
## Forth Part - Running on new computer 
1. Download [Visual Studio](https://visualstudio.microsoft.com/) (Latest release) and [FlightGear](https://www.flightgear.org/)
2. Start fightGear and click on setting. In the bottom paste
> --generic=socket,in,10,127.0.0.1,5400,tcp,playback_small
--fdm=null
3. Open visual studio and click on "Open a project or a solution", then choose the given folder and click on the sln file and it will open the project.
4. Click on Start (with a green arrow) and start running the project!
***
## Fifth Part
link to our GitHub code - https://github.com/yorairh/AP2-1.git
***
## Sixth Part - Video
***
## DLL explanation
In order to create a new anomaly detector algorythm you will need to create a new DLL.
Your DLL need to implement the following .h code:
```cpp
Results* detectAnomalis(stringWrapper* trainFile, stringWrapper* testFile);
stringWrapper* getCorrelateFeatureByFeatureName(Results* r, stringWrapper* 
int isAnomaly(Results* r, stringWrapper* feature, int timeStep);
void deleteResults(Results* r);
```
### Pay attention! you will need these two classes:
```cpp
#include <string>
#include <vector>
#include <map>
#include "AnomalyDetector.h"
#include "SimpleAnomalyDetector.h"

class stringWrapper {
public:
    string str;

    stringWrapper(); // Constructor

    stringWrapper(string str); // Constructor

    int len(); // get the length of the string

    char getCharByIndex(int x); // get the char in index x

    void addChar(char c); // add char at the end of the string
};

class Results {
public:
    vector<AnomalyReport> anomalies;
    vector<correlatedFeatures> features;
    map<string, string> corrFeatures;

    Results(vector<AnomalyReport> anomalies, vector<correlatedFeatures>,features, map<string, string> corrFeat); // Constructor
};
```
In order to add a new DLL you need to open the settings and choose your DLL in the menu.

