using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class TimeChangedEventArgs : PropertyChangedEventArgs
    {
        public string NewTime { get; set; }
        public int Seconds { get; set; }

        public TimeChangedEventArgs(InfoVal info, string newTime, int seconds) : base(info)
        {
            this.NewTime = newTime;
            this.Seconds = seconds;
        }
    }
}
