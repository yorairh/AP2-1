using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AP2_1
{
    class Model : IModel
    {

        private string[] fileData;

        public event propertyChanged notifyPropertyChanged;

        public void UploadFile(string path)
        {
            // upload the file
            fileData = File.ReadAllLines(path);
            // notify uploaded
            notifyPropertyChanged(this, new FileUploadEventArgs(PropertyChangedEventArgs.InfoVal.FileUpdated, fileData.Length));
            // foreach(string str in fileData) {
            //       Console.WriteLine(str);
            // }
        }
    }
}
