using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace AP2_1
{
    interface IGraphViewModel : INotifyPropertyChanged
    {
        void UpdateGraph();
    }
}
