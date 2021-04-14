using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IMainViewModel : INotifyPropertyChanged
    {
        void SetCurrentCategory(string category);
        void Exit();
        void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn);
    }
}
