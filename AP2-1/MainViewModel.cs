using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class MainViewModel
    {
        // GraphViewModel
        private IMainModel model;
        Dictionary<string, List<float>> learnedData;

        public event propertyChanged notifyPropertyChanged;

        public MainViewModel(IMainModel model)
        {
            this.model = model;
            learnedData = new Dictionary<string, List<float>>();
        }

        public void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn)
        {
            model.UploadFile(pathCSVAnomalies, pathXML, pathCSVLearn);
        }

        public void Exit()
        {
            model.Exit();
        }

        public void SetCurrentCategory(string category)
        {
            model.SetCurrentCategory(category);
            if (VM_CurrCategoryPM.Axes.Count > 1)
            {
                var yAxis = VM_CurrCategoryPM.Axes.ElementAt(1);
                yAxis.Minimum = model.GetCategoryMinimum(category) - 5;
                yAxis.Maximum = model.GetCategoryMaximum(category) + 5;
                yAxis.Title = category;
            }
            if (VM_CurrCorrelatedCategoryPM.Axes.Count > 1)
            {
                var yAxis = VM_CurrCorrelatedCategoryPM.Axes.ElementAt(1);
                string corrFeat = GetCorrelatedFeature(category);
                yAxis.Minimum = model.GetCategoryMinimum(corrFeat) - 5;
                yAxis.Maximum = model.GetCategoryMaximum(corrFeat) + 5;
                yAxis.Title = corrFeat;
            }
            if (VM_CorrelatedAsFuncOfCurrent.Axes.Count > 1)
            {
                var xAxis = VM_CorrelatedAsFuncOfCurrent.Axes.ElementAt(0);
                xAxis.Minimum = model.GetCategoryMinimum(category) - 5;
                xAxis.Maximum = model.GetCategoryMaximum(category) + 5;
                xAxis.Title = category;

                var yAxis = VM_CorrelatedAsFuncOfCurrent.Axes.ElementAt(1);
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

    }
}
