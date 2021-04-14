using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class FlightDataViewModel : IFlightDataViewModel
    {
        private IFlightDataModel model;


        public event propertyChanged notifyPropertyChanged;

        public FlightDataViewModel(IFlightDataModel model)
        {
            this.model = model;
            model.notifyPropertyChanged += (object sender, EventArgs e) =>
            {
                if (e as InformationChangedEventArgs != null)
                {
                    InformationChangedEventArgs args = e as InformationChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.InfoChanged)
                    {
                        // Roll = args.Roll;
                        // Pitch = args.Pitch;
                        // Yaw = args.Yaw;

                        // tbHeight.Text = args.Altimeter.ToString();
                        // tbAirSpeed.Text = args.AirSpeed.ToString();
                        // angleOfRoll.Angle = args.Roll;
                        // angleOfPitch.Angle = args.Pitch;
                        // angleOfYaw.Angle = args.Yaw;

                        notifyPropertyChanged(this, args);
                    }
                }
            };
        }
    }
}
