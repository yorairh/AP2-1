using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace AP2_1
{
    class GraphModel : IGraphModel
    {
        private FunctionSeries regLine;
        public IntPtr Results { get; set; }
        public PlotModel CurrCategoryPM { get; set; }
        public PlotModel CurrCorrelatedCategoryPM { get; set; }
        public PlotModel CorrelatedAsFuncOfCurrent { get; set; }
        public List<Anomaly> AnomaliesData { get; set; }

        private IMainModel mainModel;
        
        public GraphModel(IMainModel mainModel)
        {
            this.mainModel = mainModel;
            this.CurrCategoryPM = new PlotModel();
            CurrCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "Time",
                Minimum = -5,
                Maximum = 35,
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });
            CurrCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });

            this.CurrCorrelatedCategoryPM = new PlotModel();
            CurrCorrelatedCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "Time",
                Minimum = -5,
                Maximum = 35,
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });
            CurrCorrelatedCategoryPM.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });

            this.CorrelatedAsFuncOfCurrent = new PlotModel();
            CorrelatedAsFuncOfCurrent.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });
            CorrelatedAsFuncOfCurrent.Axes.Add(new LinearAxis
            {
                Title = "",
                Minimum = 0,
                Maximum = 0,
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });
        }

        private List<Anomaly> anomalies;

        private void SetAnomalies(IntPtr results)
        {
            AnomaliesData.Clear();
            anomalies = new List<Anomaly>();
            int len = LibraryManger.anomaliesLen(results);
            for (int i = 0; i < len; ++i)
            {
                IntPtr desc = LibraryManger.getAnomalyDescription(results, i);
                int timeStep = LibraryManger.getAnomalyTimeStep(results, i);
                anomalies.Add(new Anomaly(timeStep, StringWrapper.GetStr(desc)));
                // VM_AnomaliesData.Add(new Anomaly(timeStep, GetStr(desc)));
                StringWrapper.removeStr(desc);
            }
        }

        private void DetectAnomaliesAndSetResults(string pathCSVAnomalies)
        {
            if (Results != IntPtr.Zero)
            {
                LibraryManger.deleteResults(Results);
                Results = IntPtr.Zero;
            }
            IntPtr train = StringWrapper.CreateStringWrapperFromString(mainModel.LearnFile), test = StringWrapper.CreateStringWrapperFromString(pathCSVAnomalies);
            Results = LibraryManger.detectAnomalis(train, test);
            SetAnomalies(Results);
            StringWrapper.removeStr(train);
            StringWrapper.removeStr(test);
        }

        private string GetCorrelatedFeature(string feature)
        {
            if (Results == IntPtr.Zero) return null;
            IntPtr f = StringWrapper.CreateStringWrapperFromString(feature);
            IntPtr corr = LibraryManger.getCorrelateFeatureByFeatureName(Results, f);
            string corrStr = StringWrapper.GetStr(corr);
            StringWrapper.removeStr(corr);
            StringWrapper.removeStr(f);
            return corrStr;
        }

        public void UpdateGraph()
        {
            var data = mainModel.GetRelevantDataByFeature(mainModel.GetCurrentCategory());
            if (data == null) return;

            CurrCategoryPM?.Series?.Clear();
            List<ScatterPoint> points = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                points.Add(new ScatterPoint(i, data.ElementAt(i)));
            }
            CurrCategoryPM?.Series?.Add(new ScatterSeries
            {
                ItemsSource = points,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });

            var corrData = mainModel.GetRelevantDataByFeature(GetCorrelatedFeature(mainModel.GetCurrentCategory()));
            if (corrData == null) return;
            CurrCorrelatedCategoryPM?.Series?.Clear();
            List<ScatterPoint> corrPoints = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                corrPoints.Add(new ScatterPoint(i, corrData.ElementAt(i)));
            }
            CurrCorrelatedCategoryPM?.Series?.Add(new ScatterSeries
            {
                ItemsSource = corrPoints,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });

            CorrelatedAsFuncOfCurrent?.Series?.Clear();
            List<ScatterPoint> scatterPoints = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                scatterPoints.Add(new ScatterPoint(data[i], corrData[i]));
            }
            CorrelatedAsFuncOfCurrent?.Series?.Add(regLine);
            CorrelatedAsFuncOfCurrent?.Series.Add(new ScatterSeries
            {
                ItemsSource = scatterPoints,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.FromRgb(0, 0, 55)
            });
        }
    }
}
