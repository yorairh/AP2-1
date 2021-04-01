using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    interface IModel : INotifyPropertyChanged
    {

        void UploadFile(string PathToFile);
    }
}
