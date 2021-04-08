using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class InformationChangedEventArgs : PropertyChangedEventArgs
    {
        private float aileron;
        public float Aileron
        {
            get
            {
                return this.aileron;
            }
            set
            {
                aileron = value;
            }
        }

        private float elevator;
        public float Elevator
        {
            get
            {
                return this.elevator;
            }
            set
            {
                elevator = value;
            }
        }

        private float rudder;
        public float Rudder
        {
            get
            {
                return this.rudder;
            }
            set
            {
                rudder = value;
            }
        }

        private float throttle;
        public float Throttle
        {
            get
            {
                return this.throttle;
            }
            set
            {
                throttle = value;
            }
        }

        public InformationChangedEventArgs(InfoVal info, float aileron, float elevator, float rudder, float throttle) : base(info)
        {
            this.aileron = aileron;
            this.elevator = elevator;
            this.rudder = rudder;
            this.throttle = throttle;
        }
    }
}
