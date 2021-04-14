using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface ITimeManagerViewModel: INotifyPropertyChanged
    {
        void Jump(int len);
        void SetTime(int time);
        void SetSpeed(double speed);
        void SetPause(bool pause);
    }
}
