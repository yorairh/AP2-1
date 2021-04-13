﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class FlightSimulatorViewModel : IViewModel
    {
        private IModel model;
        private string learnFile;

        private PlotModel currCategoryPM;
        public PlotModel VM_CurrCategoryPM
        {
            get
            {
                return currCategoryPM;
            }
            set
            {
                currCategoryPM = value;
            }
        }

        public event propertyChanged notifyPropertyChanged;

        public FlightSimulatorViewModel(IModel model)
        {
            this.model = model;
            this.currCategoryPM = new PlotModel();
            currCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "Time",
                Minimum = -5,
                Maximum = 35,
                Position = AxisPosition.Bottom
            });
            currCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left
            });


            model.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as CSVAnomaliesFileUploadEventArgs != null)
                {
                    CSVAnomaliesFileUploadEventArgs args = e as CSVAnomaliesFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                if (e as TimeChangedEventArgs != null)
                {
                    TimeChangedEventArgs args = e as TimeChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.TimeChanged)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                if (e as InformationChangedEventArgs != null)
                {
                    InformationChangedEventArgs args = e as InformationChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.InfoChanged)
                    {
                        notifyPropertyChanged(this, args);
                        // UpdateGraph();
                    }
                }
                if (e as XMLFileUploadEventArgs != null)
                {
                    XMLFileUploadEventArgs args = e as XMLFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                // more....
            };
        }

        public void UpdateGraph()
        {
            var data = model.GetRelevantData();
            if (data == null) return;

            currCategoryPM?.Series?.Clear();
            List<ScatterPoint> points = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                points.Add(new ScatterPoint(i, data.ElementAt(i)));
            }
            currCategoryPM?.Series?.Add(new ScatterSeries
            {
                ItemsSource = points,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });
        }

        public void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn)
        {
            // learn and detect anomalies
            detectAnomaliesAndSetResults(pathCSVAnomalies);
            

            model.UploadFile(pathCSVAnomalies, pathXML);
            learnFile = pathCSVLearn;
        }

        public void SetPause(bool pause)
        {
            model.SetPause(pause);
        }

        public void Jump(int val)
        {
            model.Jump(val);
        }

        public void SetTime(int time)
        {
            model.SetTime(time);
        }

        public void SetSpeed(double speed)
        {
            model.SetSpeed(speed);
        }

        public void SetCurrentCategory(string category)
        {
            model.SetCurrentCategory(category);
            if (currCategoryPM.Axes.Count > 1)
            {
                var yAxis = currCategoryPM.Axes.ElementAt(1);
                yAxis.Minimum = model.GetCategoryMinimum(category) - 5;
                yAxis.Maximum = model.GetCategoryMaximum(category) + 5;
                yAxis.Title = category;
            }
        }

        public void SetLibrary(string path)
        {
            // libraryPath = path;
            File.Copy(path, LIBRARY_PATH);
        }

        public void Exit()
        {
            model.Exit();
        }

        /*
         Wrapper and DLL shit from here
         */
        private const string LIBRARY_PATH = "AnomaliesLibrary.dll";
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr detectAnomalis(IntPtr trainFile, IntPtr testFile);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getCorrelateFeatureByFeatureName(IntPtr r, IntPtr name);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int isAnomaly(IntPtr r, IntPtr feature, int timeStep);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void deleteResults(IntPtr r);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreatestringWrapper();
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int len(IntPtr s);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern char getCharByIndex(IntPtr s, int x);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addChar(IntPtr s, char c);
        [DllImport(LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void removeStr(IntPtr s);
        public static string GetStr(IntPtr s)
        {
            int l = len(s);
            string str = "";
            for (int i = 0; i < l; ++i)
            {
                str += getCharByIndex(s, i).ToString();
            }
            return str;
        }
        public static IntPtr CreateStringWrapperFromString(string str)
        {
            IntPtr s = CreatestringWrapper();
            foreach (char c in str)
            {
                addChar(s, c);
            }
            return s;
        }

        private IntPtr results;
        private Dictionary<string, string> correlatedProperties;
        
        private void detectAnomaliesAndSetResults(string pathCSVAnomalies)
        {
            IntPtr train = CreateStringWrapperFromString(learnFile), test = CreateStringWrapperFromString(pathCSVAnomalies);
            results = detectAnomalis(train, test);
            removeStr(train);
            removeStr(test);
        }
    }
}
