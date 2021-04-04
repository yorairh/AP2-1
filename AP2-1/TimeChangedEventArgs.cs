using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class TimeChangedEventArgs : PropertyChangedEventArgs
    {
        private string newTime;
        public string NewTime
        {
            get
            {
                return this.newTime;
            }
            set
            {
                newTime = value;
            }
        }

        private int seconds;
        public int Seconds
        {
            get
            {
                return this.seconds;
            }
            set
            {
                seconds = value;
            }
        }

        public TimeChangedEventArgs(InfoVal info, string newTime, int seconds) : base(info)
        {
            this.newTime = newTime;
            this.seconds = seconds;
        }
    }
}
