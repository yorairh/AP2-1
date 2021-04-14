using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace AP2_1
{
    interface IGraphModel
    {
        IntPtr Results { get; set; }
        PlotModel CurrCategoryPM{get;set;}

        PlotModel CurrCorrelatedCategoryPM{get;set;}

        PlotModel CorrelatedAsFuncOfCurrent{get;set;}
        List<Anomaly> AnomaliesData{get;set;}
        void DetectAnomaliesAndSetResults(string pathCSVAnomalies);
        void UpdateAxes();

        void UpdateGraph();
    }
}
