using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace AP2_1
{
    class FlightSimulatorModel : IModel
    {
        private Thread sendFileThread;
        private string[] fileData;
        private List<string> categories;
        public List<int> minValues;
        public List<int> maxValues;
        private double sendingSpeed;
        private volatile int index;
        private volatile bool pause;
        private object indexLock;
        private string currentCategory;

        public event propertyChanged notifyPropertyChanged;

        public FlightSimulatorModel()
        {
            indexLock = new object();
            
        }

        public void SendFile(object parameter)
        {
            FlightSimulatorModel arg = parameter as FlightSimulatorModel;
            if (arg == null)
            {
                return;
            }
            int currIndex;
            lock (arg.indexLock)
            {
                currIndex = arg.index;
            }
            while (currIndex < arg.fileData.Length)
            {
                if (!arg.pause)
                {
                    // send fileData[index]

                    lock (arg.indexLock)
                    {
                        ++arg.index;
                        string[] currData = fileData[currIndex].Split(',');
                        float aileron = float.Parse(currData[categories.IndexOf("aileron")], CultureInfo.InvariantCulture.NumberFormat);
                        float elevator = float.Parse(currData[categories.IndexOf("elevator")], CultureInfo.InvariantCulture.NumberFormat);
                        float rudder = float.Parse(currData[categories.IndexOf("rudder")], CultureInfo.InvariantCulture.NumberFormat);
                        float throttle = float.Parse(currData[categories.IndexOf("throttle")], CultureInfo.InvariantCulture.NumberFormat);
                        float altimeter = float.Parse(currData[categories.IndexOf("altitude-ft")], CultureInfo.InvariantCulture.NumberFormat);
                        float airSpeed = float.Parse(currData[categories.IndexOf("airspeed-kt")], CultureInfo.InvariantCulture.NumberFormat);
                        float orientation = float.Parse(currData[categories.IndexOf("heading-deg")], CultureInfo.InvariantCulture.NumberFormat);
                        float roll = float.Parse(currData[categories.IndexOf("roll-deg")], CultureInfo.InvariantCulture.NumberFormat);
                        float pitch = float.Parse(currData[categories.IndexOf("pitch-deg")], CultureInfo.InvariantCulture.NumberFormat);
                        float yaw = float.Parse(currData[categories.IndexOf("side-slip-deg")], CultureInfo.InvariantCulture.NumberFormat);
                        float[] info = { aileron, elevator, rudder, throttle, altimeter, airSpeed, orientation, roll, pitch, yaw};
                        arg.notifyPropertyChanged(arg, new InformationChangedEventArgs(PropertyChangedEventArgs.InfoVal.InfoChanged, info));
                        string newTime = TimeFormat(arg.index / 10);
                        arg.notifyPropertyChanged(arg, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, arg.index));
                    }
                    Thread.Sleep((int)(100 / arg.sendingSpeed));
                }
                lock (arg.indexLock)
                {
                    currIndex = arg.index;
                }
            }
        }

        public void UploadFile(string pathCSV, string pathXML)
        {
            if (sendFileThread != null)
            {
                sendFileThread.Abort();
                sendFileThread = null;
            }

            // upload the CSV file
            fileData = File.ReadAllLines(pathCSV);
            //upload the XML file
            categories = new List<string>();
            
            XmlDocument doc = new XmlDocument();
            doc.Load(pathXML);
            XmlNode input = doc.GetElementsByTagName("input").Item(0);
            foreach (XmlNode child in input.ChildNodes)
            {
                foreach (XmlNode item in child.ChildNodes)
                {
                    if (item.Name == "name")
                        categories.Add(item.InnerText);
                }
            }
            minValues = new List<int>(categories.Count);
            maxValues = new List<int>(categories.Count);
            for (int i = 0; i < fileData.Length; ++i)
            {
                List<int> max = new List<int>(categories.Count), min = new List<int>(categories.Count);
                string[] curr = fileData[i].Split(',');
                for (int j = 0; j < curr.Length; ++j)
                {
                    float val = float.Parse(curr[j], CultureInfo.InvariantCulture.NumberFormat);
                    if (i == 0 || val < minValues.ElementAt(j)) minValues.Insert(j, (int) val);
                    if (i == 0 || val > maxValues.ElementAt(j)) maxValues.Insert(j, (int) val);
                }
            }

            // notify uploaded
            notifyPropertyChanged(this, new CSVFileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, fileData.Length));
            notifyPropertyChanged(this, new XMLFileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, categories));

            // create the thread uploading the file lines
            index = 0;
            pause = false;
            sendingSpeed = 1;

            sendFileThread = new Thread(SendFile);
            sendFileThread.Start(this);
        }

        public List<float> GetRelevantData()
        {
            if (currentCategory == null)
            {
                return null;
            }
            int currIndex;
            lock (indexLock)
            {
                currIndex = index;
            }
            int firstIndex = (currIndex - 29 < 0 ? 0 : currIndex - 29);
            List<float> relData = new List<float>();
            for (int i = firstIndex; i <= currIndex; ++i)
            {
                relData.Add(float.Parse(fileData[i].Split(',')[categories.IndexOf(currentCategory)], CultureInfo.InvariantCulture.NumberFormat));
            }
            return relData;
        }

        public string GetCurrentCategory()
        {
            return currentCategory;
        }
        public float GetCurrentCategoryMinimum()
        {
            return minValues.ElementAt(categories.IndexOf(currentCategory));
        }
        public float GetCurrentCategoryMaximum() {
            return maxValues.ElementAt(categories.IndexOf(currentCategory));
        }

        public void SetPause(bool pause)
        {
            this.pause = pause;
        }

        private static string TimeFormat(int seconds)
        {
            int h = seconds / 3600, m = (seconds - 3600 * h) / 60, s = seconds - 3600 * h - m * 60;
            DateTime dt = new DateTime(1, 1, 1, h, m, s); // the date doesn't matter
            return dt.ToString("HH:mm:ss");
        }

        public void Jump(int val)
        {
            lock (indexLock)
            {
                index += val;
                string newTime = TimeFormat(index / 10);
                notifyPropertyChanged(this, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, index));
            }
        }

        public void SetTime(int time)
        {
            lock (indexLock)
            {
                index = time;
                string newTime = TimeFormat(index / 10);
                notifyPropertyChanged(this, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, index));
            }
        }

        public void SetSpeed(double speed)
        {
            sendingSpeed = speed;
        }

        public void SetCurrentCategory(string category)
        {
            this.currentCategory = category;
        }

        public void Exit()
        {
            if (sendFileThread != null && sendFileThread.IsAlive)
            {
                sendFileThread.Abort();
            }
        }
    }
}
