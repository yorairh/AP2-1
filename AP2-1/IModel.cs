using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IModel : INotifyPropertyChanged
    {

        void SetPause(bool pause);

        void UploadFile(string pathCSVAnomalies, string pathXML);
        void Jump(int val);
        void SetTime(int time);
        void SetSpeed(double speed);
        void SetCurrentCategory(string category);
        List<float> GetRelevantData();
        string GetCurrentCategory();
        float GetCategoryMinimum(string category);
        float GetCategoryMaximum(string category);
        void Exit();
    }
}
