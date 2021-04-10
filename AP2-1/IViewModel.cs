using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IViewModel : INotifyPropertyChanged
    {
        void UploadFile(string pathCSV, string pathXML);
        void SetPause(bool pause);
        void Jump(int len);
        void SetTime(int time);
        void SetSpeed(double speed);
        void Exit();
    }
}
