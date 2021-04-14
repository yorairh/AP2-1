using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface ITimeManagerModel : INotifyPropertyChanged
    {
        void SetPause(bool pause);
        void SetTime(int time);
        void Jump(int val);
        void SetSpeed(double speed);
    }
}
