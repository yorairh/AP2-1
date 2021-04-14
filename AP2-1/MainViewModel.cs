using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AP2_1
{
    class MainViewModel : IMainViewModel
    {
        // GraphViewModel
        private IMainModel model;
        Dictionary<string, List<float>> learnedData;

        public int AnomaliesFileLength { get; set; }
        public List<MenuItem> CategoriesMenu { get; set; }

        public event propertyChanged notifyPropertyChanged;

        public MainViewModel(IMainModel model)
        {
            this.model = model;

            model.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as CSVAnomaliesFileUploadEventArgs != null)
                {
                    CSVAnomaliesFileUploadEventArgs args = e as CSVAnomaliesFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                if (e as XMLFileUploadEventArgs != null)
                {
                    XMLFileUploadEventArgs args = e as XMLFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                // more....
            };

            learnedData = new Dictionary<string, List<float>>();
            CategoriesMenu = new List<MenuItem>();
        }

        public void UploadFile(string pathCSVAnomalies, string pathXML, string pathCSVLearn)
        {
            model.UploadFile(pathCSVAnomalies, pathXML, pathCSVLearn);
        }

        public void Exit()
        {
            model.Exit();
        }

        public void SetCurrentCategory(string category)
        {
            model.SetCurrentCategory(category);
        }
    }
}
