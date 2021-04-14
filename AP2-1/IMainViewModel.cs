using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AP2_1
{
    interface IMainViewModel : INotifyPropertyChanged
    {
        int AnomaliesFileLength { get; set; }
        List<MenuItem> CategoriesMenu { get; set; }
        void SetCurrentCategory(string category);
        void Exit();
        void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn);
    }
}
