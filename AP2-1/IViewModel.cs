using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IViewModel : INotifyPropertyChanged
    {

        void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn);
        void SetPause(bool pause);
        void Jump(int len);
        void SetTime(int time);
        void SetSpeed(double speed);
        void SetCurrentCategory(string category);
        void SetLibrary(string path);
        void UpdateGraph();
        void Exit();
    }
}
