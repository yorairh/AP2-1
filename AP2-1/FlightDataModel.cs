using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class FlightDataModel : IFlightDataModel
    {

        private IMainModel mainModel;

        public event propertyChanged notifyPropertyChanged;

        public FlightDataModel(IMainModel mainModel)
        {
            this.mainModel = mainModel;
        }
        public void UpdateData()
        {
            string[] props = { "aileron", "elevator", "rudder", "throttle", "altitude-ft", "airspeed-kt", "heading-deg", "roll-deg", "pitch-deg", "side-slip-deg" };
            float[] info = new float[props.Length];
            for (int i = 0; i < info.Length; ++i)
            {
                var temp = mainModel.GetValueByCategory(props[i]);
                if (temp == null) return;
                info[i] = temp.Value;
            }
            notifyPropertyChanged(this, new InformationChangedEventArgs(PropertyChangedEventArgs.InfoVal.InfoChanged, info));
        }
    }
}
