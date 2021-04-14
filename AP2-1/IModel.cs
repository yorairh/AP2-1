using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IModel : INotifyPropertyChanged
    {


        void UploadFile(string pathCSVAnomalies, string pathXML);
        void SetCurrentCategory(string category);
        string GetCurrentCategory();
        List<float> GetRelevantDataByFeature(string feature);
        void Exit();
        float GetCategoryMinimum(string category);
        float GetCategoryMaximum(string category);
        int GetCurrentTimeStep();


        void SetPause(bool pause);
        void SetTime(int time);
        void Jump(int val);
        void SetSpeed(double speed);
    }
}
