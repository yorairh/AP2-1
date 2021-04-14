using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IMainModel : INotifyPropertyChanged
    {
        string LearnFile { get; set; }
        bool Pause { get; set; }
        int Index { get; set; }
        double SendingSpeed { get; set; }
        void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn);
        void SetCurrentCategory(string category);
        string GetCurrentCategory();
        float? GetValueByCategory(string category);
        List<float> GetRelevantDataByFeature(string feature);
        float GetCategoryMinimum(string category);
        float GetCategoryMaximum(string category);
        void Exit();
    }
}