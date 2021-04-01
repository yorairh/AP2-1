using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class ViewModel : IViewModel
    {
        private IModel model;
        
        public event propertyChanged notifyPropertyChanged;

        public ViewModel(IModel model)
        {
            this.model = model;

            model.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as FileUploadEventArgs != null)
                {
                    FileUploadEventArgs args = e as FileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                // more....
            };
        }

        public void UploadFile(string path)
        {
            model.UploadFile(path);
        }

    }
}
