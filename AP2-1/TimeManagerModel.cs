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

        public static string TimeFormat(int seconds)
        {
            int h = seconds / 3600, m = (seconds - 3600 * h) / 60, s = seconds - 3600 * h - m * 60;
            DateTime dt = new DateTime(1, 1, 1, h, m, s); // the date doesn't matter
            return dt.ToString("HH:mm:ss");
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
            string newTime = TimeFormat(time / 10);
            notifyPropertyChanged(this, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, time));
        }

        public void SetSpeed(double speed)
        {
            mainModel.SendingSpeed = speed;
        }

        public void UpdateTime()
        {
            string newTime = TimeFormat(mainModel.Index / 10);
            notifyPropertyChanged(this, new TimeChangedEventArgs(PropertyChangedEventArgs.InfoVal.TimeChanged, newTime, mainModel.Index));
        }
    }
}
