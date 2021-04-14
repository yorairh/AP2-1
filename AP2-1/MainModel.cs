using System;
using System.Windows;
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
    class MainModel : IMainModel
    {
        private TcpClient FGClient;
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
        private IGraphModel graphModel;
        private IFlightDataModel flightDataModel;
        private ITimeManagerModel timeManagerModel;

        public void SetGraphModel(IGraphModel model)
        {
            this.graphModel = model;
        }

        public void SetFlightDataModel(IFlightDataModel model)
        {
            this.flightDataModel = model;
        }
        public void SetTimeManagerModel(ITimeManagerModel model)
        {
            this.timeManagerModel = model;
        }

        public string LearnFile { get; set; }
        public bool Pause {
            get
            {
                return pause;
            }
            set
            {
                if (!value && sendFileThread != null && !sendFileThread.IsAlive)
                {
                    sendFileThread.Abort();
                    StartThread();
                } 
                else
                {
                    pause = value;
                }
            }
        }

        public int Index
        {
            get
            {
                lock (indexLock)
                {
                    return index;
                }
            }
            set
            {
                lock (indexLock)
                {
                    index = value;
                }
            }
        }

        public double SendingSpeed
        {
            get
            {
                return sendingSpeed;
            }
            set
            {
                sendingSpeed = value;
            }
        }

        public event propertyChanged notifyPropertyChanged;

        public MainModel()
        {
            indexLock = new object();
        }

        public MainModel(IGraphModel graphModel, IFlightDataModel flightDataModel, ITimeManagerModel timeManagerModel)
        {
            this.graphModel = graphModel;
            this.flightDataModel = flightDataModel;
            this.timeManagerModel = timeManagerModel;
            indexLock = new object();
        }

        public void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn)
        {
            if (sendFileThread != null)
            {
                sendFileThread.Abort();
                sendFileThread = null;
            }
            LearnFile = pathCSVLearn;
            graphModel.DetectAnomaliesAndSetResults(pathCSVAnomalies);

            // upload the CSV file
            fileData = File.ReadAllLines(pathCSVAnomalies).Skip(1).ToArray(); // skip the headlines
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

            SetMinimumAndMaximum();

            // notify uploaded
            notifyPropertyChanged(this, new CSVAnomaliesFileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, fileData.Length));
            notifyPropertyChanged(this, new XMLFileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, categories));

            

            Index = 0;
            sendingSpeed = 1;
            StartThread();
        }

        private void StartThread()
        {
            try
            {
                if (FGClient == null || !FGClient.Connected)
                {
                    FGClient = new TcpClient("localhost", 5400);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Connection to Flight Gear Failed. Try To Upload Again.");
                return;
            }
            // create the thread uploading the file lines
            pause = false;
            
            sendFileThread = new Thread(SendFile);
            sendFileThread.Start(this);
        }

        public void SendFile(object parameter)
        {
            MainModel arg = parameter as MainModel;
            if (arg == null)
            {
                return;
            }
            while (Index < arg.fileData.Length)
            {
                if (!arg.pause)
                {
                    // send fileData[index]
                    var bytes = Encoding.ASCII.GetBytes(fileData[Index] + Environment.NewLine);
                    FGClient.GetStream().Write(bytes, 0, bytes.Length);

                    ++Index;
                    flightDataModel.UpdateData();
                    timeManagerModel.UpdateTime();
                    Thread.Sleep((int)(100 / arg.SendingSpeed));
                }
            }
            FGClient.Close();
        }

        public float? GetValueByCategory(string category)
        {
            if (Index < fileData.Length)
            {
                string[] currData = fileData[Index].Split(',');
                return float.Parse(currData[categories.IndexOf(category)], CultureInfo.InvariantCulture.NumberFormat);
            }
            return null;
        }

        public void SetCurrentCategory(string category)
        {
            this.currentCategory = (category == "Choose property" ? null : category);
            graphModel.UpdateAxes();
        }

        public List<float> GetRelevantDataByFeature(string category)
        {
            if (categories == null || !categories.Contains(category)) return null;
            int currIndex;
            lock (indexLock)
            {
                currIndex = index;
            }
            List<float> relData = new List<float>();
            if (currIndex >= fileData.Length)
            {
                currIndex = fileData.Length - 1;
            }
            int firstIndex = (currIndex - 29 < 0 ? 0 : currIndex - 29);
            for (int i = firstIndex; i <= currIndex; ++i)
            {
                relData.Add(float.Parse(fileData[i].Split(',')[categories.IndexOf(category)], CultureInfo.InvariantCulture.NumberFormat));
            }
            return relData;
        }

        public void Exit()
        {
            if (sendFileThread != null && sendFileThread.IsAlive)
            {
                sendFileThread.Abort();
            }
            if (FGClient != null && !FGClient.Connected)
            {
                FGClient.Close();
            }
        }

        private void SetMinimumAndMaximum()
        {
            minValues = new List<int>();
            maxValues = new List<int>();
            string[] curr = fileData[0].Split(',');
            for (int j = 0; j < categories.Count; ++j)
            {
                float val = float.Parse(curr[j], CultureInfo.InvariantCulture.NumberFormat);
                minValues.Add((int)val);
                maxValues.Add((int)val);
            }

            for (int i = 1; i < fileData.Length; ++i)
            {
                curr = fileData[i].Split(',');
                for (int j = 0; j < categories.Count; ++j)
                {
                    float val = float.Parse(curr[j], CultureInfo.InvariantCulture.NumberFormat);
                    if ((int)val < minValues.ElementAt(j))
                        minValues[j] = (int)val;
                    // minValues.Insert(j, (int)val);
                    if ((int)val > maxValues.ElementAt(j))
                        maxValues[j] = (int)val;
                    // maxValues.Insert(j, (int)val);
                }
            }
        }

        public string GetCurrentCategory()
        {
            return currentCategory;
        }

        public float GetCategoryMinimum(string category)
        {
            return minValues.ElementAt(categories.IndexOf(category));
        }

        public float GetCategoryMaximum(string category)
        {
            return maxValues.ElementAt(categories.IndexOf(category));
        }

        public int GetCurrentTimeStep()
        {
            int res;
            lock (indexLock)
            {
                res = index;
            }
            return res;
        }
    }
}
