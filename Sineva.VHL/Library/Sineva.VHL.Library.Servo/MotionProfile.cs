using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Servo
{
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    [Serializable]
    public class MotionProfile
    {
        public double Distance { get; set; }
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        public double Deceleration { get; set; }
        public double Jerk { get; set; }
        public byte VelocityLimitFlag { get; set; }
        public MotionProfile()
        {
        }
        public void Clear()
        {
            Distance = 0;
            Velocity = 0;
            Acceleration = 0;
            Deceleration = 0;
            Jerk = 0;
            VelocityLimitFlag = 0;
        }

        public override string ToString()
        {
            return string.Format("{0:F2},{1:F2},{2:F2},{3:F2},{4:F2}", Distance, Velocity, Acceleration, Deceleration, Jerk);
        }
    }
}
