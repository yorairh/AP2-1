using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class TimeManagerModel : ITimeManagerModel
    {
        private IMainModel mainModel;

        public event propertyChanged notifyPropertyChanged;

        public TimeManagerModel(IMainModel mainModel)
        {
            this.mainModel = mainModel;
        }

        public void SetPause(bool pause)
        {
            mainModel.Pause = pause;
        }

        public void Jump(int val)
        {
            mainModel.Index += val;
               string newTime = TimeFormat(mainModel.Index / 10);
               notifyPropertyChanged(this, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, mainModel.Index));
        }

        public void SetTime(int time)
        {
            mainModel.Index = time;
            string newTime = TimeFormat(mainModel.Index / 10);
            notifyPropertyChanged(this, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, mainModel.Index));
        }

        public void SetSpeed(double speed)
        {
            mainModel.SendingSpeed = speed;
        }
    }
}
