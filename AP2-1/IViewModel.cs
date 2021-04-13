using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace AP2_1
{
    interface IViewModel : INotifyPropertyChanged
    {

        PlotModel VM_CurrCategoryPM
        {
            get;
            set;
        }

        PlotModel VM_CurrCorrelatedCategoryPM
        {
            get;
            set;
        }

        PlotModel VM_CorrelatedAsFuncOfCurrent
        {
            get;
            set;
        }

        void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn);
        void SetPause(bool pause);
        void Jump(int len);
        void SetTime(int time);
        void SetSpeed(double speed);
        void SetCurrentCategory(string category);
        void SetLibrary(string path);
        string GetLibrary();
        void UpdateGraph();
        void Exit();
    }
}
