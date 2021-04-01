using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    delegate void propertyChanged(object sender, EventArgs e);
    interface INotifyPropertyChanged
    {
        event propertyChanged notifyPropertyChanged;
    }
}
