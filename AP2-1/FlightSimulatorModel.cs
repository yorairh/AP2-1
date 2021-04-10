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
        private double sendingSpeed;
        private volatile int index;
        private volatile bool pause;
        private object indexLock;

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
                        float aileron = float.Parse(currData[0], CultureInfo.InvariantCulture.NumberFormat);
                        float elevator = float.Parse(currData[1], CultureInfo.InvariantCulture.NumberFormat);
                        float rudder = float.Parse(currData[2], CultureInfo.InvariantCulture.NumberFormat);
                        float throttle = float.Parse(currData[6], CultureInfo.InvariantCulture.NumberFormat);
                        float altimeter = float.Parse(currData[25], CultureInfo.InvariantCulture.NumberFormat);
                        float airSpeed = float.Parse(currData[21], CultureInfo.InvariantCulture.NumberFormat);
                        float orientation = float.Parse(currData[19], CultureInfo.InvariantCulture.NumberFormat);
                        float roll = float.Parse(currData[17], CultureInfo.InvariantCulture.NumberFormat);
                        float pitch = float.Parse(currData[18], CultureInfo.InvariantCulture.NumberFormat);
                        float yaw = float.Parse(currData[20], CultureInfo.InvariantCulture.NumberFormat);
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
            // notify uploaded
            notifyPropertyChanged(this, new FileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, fileData.Length));

            // create the thread uploading the file lines
            index = 0;
            pause = false;
            sendingSpeed = 1;

            sendFileThread = new Thread(SendFile);
            sendFileThread.Start(this);
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

        public void Exit()
        {
            if (sendFileThread != null && sendFileThread.IsAlive)
            {
                sendFileThread.Abort();
            }
        }
    }
}
