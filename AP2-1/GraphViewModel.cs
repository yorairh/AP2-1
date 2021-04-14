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
    class GraphViewModel : IGraphViewModel
    {
        private IGraphModel model;

        public event propertyChanged notifyPropertyChanged;

        public GraphViewModel(IGraphModel model)
        {
            this.model = model;
        }

        public void UpdateGraph()
        {
            model.UpdateGraph();
        }
    }
}