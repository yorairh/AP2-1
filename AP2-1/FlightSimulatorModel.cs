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

namespace AP2_1
{
    class FlightSimulatorModel : IModel
    {
        private Thread sendFile;
        private string[] fileData;
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
                        arg.notifyPropertyChanged(arg, new InformationChangedEventArgs(PropertyChangedEventArgs.InfoVal.InfoChanged, aileron, elevator, rudder, throttle));
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

        public void UploadFile(string path)
        {
            if (sendFile != null)
            {
                sendFile.Abort();
                sendFile = null;
            }

            // upload the file
            fileData = File.ReadAllLines(path);
            // notify uploaded
            notifyPropertyChanged(this, new FileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, fileData.Length));

            // create the thread uploading the file lines
            index = 0;
            pause = false;
            sendingSpeed = 1;

            sendFile = new Thread(SendFile);
            sendFile.Start(this);
        }

        public void SetPause(bool pause)
        {
            this.pause = pause;
        }

        private static string TimeFormat(int seconds)
        {
            int h, m, s;
            h = seconds / 3600;
            m = (seconds - (3600 * h)) / 60;
            s = seconds - 3600 * h - m * 60;
            string sh = h.ToString();
            string sm = m.ToString();
            string ss = s.ToString();
            if (h < 10)
            {
                sh = "0" + sh;
            }
            if (m < 10)
            {
                sm = "0" + sm;
            }
            if (s < 10)
            {
                ss = "0" + ss;
            }
            return sh + ":" + sm + ":" + ss;
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
            sendFile.Abort();
        }
    }
}
