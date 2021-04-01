using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class PropertyChangedEventArgs : EventArgs
    {
        public enum InfoVal {
            FileUpdated = 0,
        };

        private InfoVal info;
        public PropertyChangedEventArgs(InfoVal info)
        {
            this.info = info;
        }

        public InfoVal Info
        {
            set
            {
                this.info = value;
            }
            get
            {
                return this.info;
            }
        }
    }
}
