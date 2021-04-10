using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class XMLFileUploadEventArgs : PropertyChangedEventArgs
    {
        List<string> categories;
        public XMLFileUploadEventArgs(InfoVal info, List<string> categories) : base(info)
        {
            this.categories = categories;
        }
        public List<string> Categories
        {
            set
            {
                this.categories = value;
            }
            get
            {
                return this.categories;
            }
        }
    }
}
