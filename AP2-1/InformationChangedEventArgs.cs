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
        
        private float altimeter;

        public float Altimeter
        {
            get
            {
                return this.altimeter;
            }
            set
            {
                altimeter = value;
            }
        }

        private float airSpeed;

        public float AirSpeed
        {
            get
            {
                return this.airSpeed;
            }
            set
            {
                airSpeed = value;
            }
        }

        private float orientation;

        public float Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                orientation = value;
            }
        }

        private float roll;

        public float Roll
        {
            get
            {
                return this.roll;
            }
            set
            {
                roll = value;
            }
        }

        private float pitch;

        public float Pitch
        {
            get
            {
                return this.pitch;
            }
            set
            {
                pitch = value;
            }
        }

        private float yaw;

        public float Yaw
        {
            get
            {
                return this.yaw;
            }
            set
            {
                yaw = value;
            }
        }

        public InformationChangedEventArgs(InfoVal info, float[] stats) : base(info)
        {
            this.aileron = stats[0];
            this.elevator = stats[1];
            this.rudder = stats[2];
            this.throttle = stats[3];
            this.altimeter = stats[4];
            this.airSpeed = stats[5];
            this.orientation = stats[6];
            this.roll = stats[7];
            this.pitch = stats[8];
            this.yaw = stats[9];
        }
    }
}
