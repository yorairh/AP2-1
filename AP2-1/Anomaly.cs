using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class Anomaly
    {
        private string feature1;
        private string feature2;
        public int TimeStep
        {
            get; set;
        }

        public string GetFeature1()
        {
            return feature1;
        }
        public Anomaly(int timeStep, string description)
        {
            this.TimeStep = timeStep;
            var f = description.Split(',');
            if (f.Length == 2)
            {
                this.feature1 = f[0];
                this.feature2 = f[1];
            }
        }
    }
}
