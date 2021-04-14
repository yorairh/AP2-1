using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AP2_1
{
    class FlightSimulatorViewModel : IViewModel
    {
        private IModel model;
        private string learnFile;
        Dictionary<string, List<float>> learnedData;
        private FunctionSeries regLine;
        private int fileLen;

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

        private PlotModel currCorrelatedCategoryPM;
        public PlotModel VM_CurrCorrelatedCategoryPM
        {
            get
            {
                return currCorrelatedCategoryPM;
            }
            set
            {
                currCorrelatedCategoryPM = value;
            }
        }

        private PlotModel correlatedAsFuncOfCurrent;
        public PlotModel VM_CorrelatedAsFuncOfCurrent
        {
            get
            {
                return correlatedAsFuncOfCurrent;
            }
            set
            {
                correlatedAsFuncOfCurrent = value;
            }
        }

        public event propertyChanged notifyPropertyChanged;

        public FlightSimulatorViewModel(IModel model)
        {
            VM_AnomaliesData = new List<Anomaly>();
            learnedData = new Dictionary<string, List<float>>();
            results = IntPtr.Zero;
            this.model = model;
            this.currCategoryPM = new PlotModel();
            currCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "Time",
                Minimum = -5,
                Maximum = 35,
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });
            currCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });

            this.VM_CurrCorrelatedCategoryPM = new PlotModel();
            VM_CurrCorrelatedCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "Time",
                Minimum = -5,
                Maximum = 35,
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });
            VM_CurrCorrelatedCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });

            this.VM_CorrelatedAsFuncOfCurrent = new PlotModel();
            VM_CorrelatedAsFuncOfCurrent.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });
            VM_CorrelatedAsFuncOfCurrent.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });

            model.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as CSVAnomaliesFileUploadEventArgs != null)
                {
                    CSVAnomaliesFileUploadEventArgs args = e as CSVAnomaliesFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                        fileLen = args.Length;
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
        private const string LINEAR_LIBRARY_PATH = "LinearRegression.dll";
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getSlope(IntPtr r, IntPtr feature);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern float getYIntercept(IntPtr r, IntPtr feature);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getAnomalyDescription(IntPtr r, int index);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getAnomalyTimeStep(IntPtr r, int index);
        [DllImport(LINEAR_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int anomaliesLen(IntPtr r);
        private const string STRING_LIBRARY_PATH = "StringWrapper.dll";
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreatestringWrapper();
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int len(IntPtr s);
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern char getCharByIndex(IntPtr s, int x);
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern void addChar(IntPtr s, char c);
        [DllImport(STRING_LIBRARY_PATH, CallingConvention = CallingConvention.Cdecl)]
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

        private void detectAnomaliesAndSetResults(string pathCSVAnomalies)
        {
            IntPtr train = CreateStringWrapperFromString(learnFile), test = CreateStringWrapperFromString(pathCSVAnomalies);
            results = detectAnomalis(train, test);
            SetAnomalies(results);
            removeStr(train);
            removeStr(test);
        }

        private string GetCorrelatedFeature(string feature)
        {
            if (results == IntPtr.Zero) return null;
            IntPtr f = CreateStringWrapperFromString(feature);
            IntPtr corr = getCorrelateFeatureByFeatureName(results, f);
            string corrStr = GetStr(corr);
            removeStr(corr);
            removeStr(f);
            return corrStr;
        }

        
        public List<Anomaly> VM_AnomaliesData
        {
            get;
            set;
        }
        private List<Anomaly> anomalies;

        private void SetAnomalies(IntPtr results)
        {
            VM_AnomaliesData.Clear();
            anomalies = new List<Anomaly>();
            int len = anomaliesLen(results);
            for (int i = 0; i < len; ++i)
            {
                IntPtr desc = getAnomalyDescription(results, i);
                int timeStep = getAnomalyTimeStep(results, i);
                anomalies.Add(new Anomaly(timeStep, GetStr(desc)));
                // VM_AnomaliesData.Add(new Anomaly(timeStep, GetStr(desc)));
                removeStr(desc);
            }
        }

        /*
         end of this
         */

        public void UpdateGraph()
        {
            var data = model.GetRelevantDataByFeature(model.GetCurrentCategory());
            if (data == null) return;

            VM_CurrCategoryPM?.Series?.Clear();
            List<ScatterPoint> points = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                points.Add(new ScatterPoint(i, data.ElementAt(i)));
            }
            VM_CurrCategoryPM?.Series?.Add(new ScatterSeries
            {
                ItemsSource = points,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });

            var corrData = model.GetRelevantDataByFeature(GetCorrelatedFeature(model.GetCurrentCategory()));
            if (corrData == null) return;
            VM_CurrCorrelatedCategoryPM?.Series?.Clear();
            List<ScatterPoint> corrPoints = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                corrPoints.Add(new ScatterPoint(i, corrData.ElementAt(i)));
            }
            VM_CurrCorrelatedCategoryPM?.Series?.Add(new ScatterSeries
            {
                ItemsSource = corrPoints,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });

            VM_CorrelatedAsFuncOfCurrent?.Series?.Clear();
            List<ScatterPoint> scatterPoints = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                scatterPoints.Add(new ScatterPoint(data[i], corrData[i]));
            }
            VM_CorrelatedAsFuncOfCurrent?.Series?.Add(regLine);
            VM_CorrelatedAsFuncOfCurrent?.Series.Add(new ScatterSeries
            {
                ItemsSource = scatterPoints,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });
        }

        public void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn)
        {
            if (results != IntPtr.Zero)
            {
                deleteResults(results);
                results = IntPtr.Zero;
            }
            // learn and detect anomalies
            learnFile = pathCSVLearn;
            detectAnomaliesAndSetResults(pathCSVAnomalies);
            model.UploadFile(pathCSVAnomalies, pathXML);
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
            if (currCorrelatedCategoryPM.Axes.Count > 1)
            {
                var yAxis = currCorrelatedCategoryPM.Axes.ElementAt(1);
                string corrFeat = GetCorrelatedFeature(category);
                yAxis.Minimum = model.GetCategoryMinimum(corrFeat) - 5;
                yAxis.Maximum = model.GetCategoryMaximum(corrFeat) + 5;
                yAxis.Title = corrFeat;
            }
            if (correlatedAsFuncOfCurrent.Axes.Count > 1)
            {
                var xAxis = correlatedAsFuncOfCurrent.Axes.ElementAt(0);
                xAxis.Minimum = model.GetCategoryMinimum(category) - 5;
                xAxis.Maximum = model.GetCategoryMaximum(category) + 5;
                xAxis.Title = category;

                var yAxis = correlatedAsFuncOfCurrent.Axes.ElementAt(1);
                string corrFeat = GetCorrelatedFeature(category);
                yAxis.Minimum = model.GetCategoryMinimum(corrFeat) - 5;
                yAxis.Maximum = model.GetCategoryMaximum(corrFeat) + 5;
                yAxis.Title = corrFeat;
                float a, b;
                var feature = CreateStringWrapperFromString(category);
                a = getSlope(results, feature);
                b = getYIntercept(results, feature);
                removeStr(feature);
                regLine = new FunctionSeries((double x) => a * x + b, xAxis.Minimum, xAxis.Maximum, 0.1);
                // Line reg = LinearRegressionCalculator.CalculateLineRegression(learnedData[category], learnedData[GetCorrelatedFeature(category)]);
                // regLine = new FunctionSeries((double x) => reg.Slope * x + reg.YIntercept, xAxis.Minimum, xAxis.Maximum, 0.0001);
            }

            VM_AnomaliesData.Clear();
            foreach (Anomaly a in anomalies)
            {
                if (a.GetFeature1() == category)
                {
                    VM_AnomaliesData.Add(a);
                }
            }
        }

        public void SetLibrary(string path)
        {
            // libraryPath = path;
            File.Copy(path, LIBRARY_PATH);
        }

        public string GetLibrary()
        {
            return LIBRARY_PATH;
        }

        public void Exit()
        {
            model.Exit();
        }

        
    }
}
