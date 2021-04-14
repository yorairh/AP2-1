using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class TimeManagerViewModel : ITimeManagerViewModel
    {
        private ITimeManagerModel model;

        public event propertyChanged notifyPropertyChanged;

        public TimeManagerViewModel(ITimeManagerModel model)
        {
            this.model = model;
        }

        public void SetPause(bool pause)
        {
            model.SetPause(pause);
        }

        public void Jump(int val)
        {
            model.Jump(val);
        }

        public void SetTime(int time)
        {
            model.SetTime(time);
        }

        public void SetSpeed(double speed)
        {
            model.SetSpeed(speed);
        }
    }
}
