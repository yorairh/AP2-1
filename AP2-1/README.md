# Advanced Programming #2 - First Assignment
## Yorai Roth Hazan, Ori Dabush
***
## First part - Documentation of the features of the Project
* When you run the project a window with an explaination opens. After you press OK you will be taken to the application main window. You will need to enter the files (.xml configuration file + 2 .csv files), to open FlightGear, and to enter the following settings:
> --generic=socket,in,10,127.0.0.1,5400,tcp,playback_small
--fdm=null
* Now, When you click on the upload button a few things will happen:
    * The simulator will start running in the FlightGear window.
    * A control pannel to the video will open with buttons to control the speed of the video and to stop it.
    * Information pannel that shows the joystick of the pilot, the rudder and the throttle. In addition to that you can see the height of the plane, the speed, the orientation and the RPY (roll, pitch, yaw) under the joystick in clocks and numbers formats.
    * A list of all the data type that you can track, when you choose one of them a graph of this category as a function of time will apear, another graph will apear which shows the correlated category to the chosen one as a function of time. 
    * The big graph in the middle of the screen will be the correlated category as a function of the chosen category.
    * The table near the big graph will detect anomalies using a given .dll file that detects anomalies.
***
## Second Part - Documentation of the files of the project
### A link to [the structure of our project](https://github.com/yorairh/AP2-1/blob/master/AP21.pdf)
Our project is built using MVVM Architecture: (view, viewmodel, model)
* We have one view (main window) and for each part of the project a viewmodel and a model interfaces.
* We have them all extending another interface that called INotifyPropertyChanged - for data binding and event handling.
* For each part of the project (graphs, videocontrol, data, menu...) we created A viewmodel and model implementions that manage the changes in those subjects - The NotifyPropertyChanged event in  the INotifyProperyChanged interface help with the binding between the three parts (view, viewmodel, model).
* In the NotifyPropertyChanged we send a PropertyChangedEventArgs which is a class that we implemented into classes for all the different subjects in the project. Each PropertyChangedEventArgs contains info about what changed and that is how the whole project notify itself about changes.
* We created a different thread that reads the .csv file lines and by that we can change the different varriables according to the file info.
* We Used TCP Socket to send the lines of the file to the FlightGear application.
* We used a library that we wrote in c++ to link the given .dll anomaly detection libraries with our project.
***
## Third Part - Installations required
* The path to the csv files must conatin only ASCII characters. (Or it will not work)
* Download [Visual Studio](https://visualstudio.microsoft.com/) (Latest release) and [FlightGear](https://www.flightgear.org/)
* NuGet Packages: OxyPlot.Wpf, OxyPlot.Core
* Framework: .NET framework 4.7.2
***
## Forth Part - Running on new computer 
1. Download [Visual Studio](https://visualstudio.microsoft.com/) (Latest release) and [FlightGear](https://www.flightgear.org/).
2. Watch our [explaination video](https://youtu.be/NE104wh8ttw) and learn how to run the application.
3. Start FlightGear and click on setting. In the bottom paste
> --generic=socket,in,10,127.0.0.1,5400,tcp,playback_small
--fdm=null
3. Open visual studio and click on "Open a project or a solution", then choose the given folder and click on the sln file and it will open the project.
4. Click on Start (with a green arrow) and start running the project!
***
## Fifth Part
link to our [GitHub repository](https://github.com/yorairh/AP2-1.git)
link to our [UML diagram](https://github.com/yorairh/AP2-1/blob/master/AP21.pdf)
***
## Sixth Part - Video
link to our [explaination video](https://youtu.be/NE104wh8ttw)
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

