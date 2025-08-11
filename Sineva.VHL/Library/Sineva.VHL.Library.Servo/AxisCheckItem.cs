/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
    public class AxisCheckItem
    {
        public bool Checked { get; set; }
        public string AxisName { get; set; }
        public string ServoOnOff { get; set; }
        public string NegLimitCheck { get; set; }
        public string PosLimitCheck { get; set; }
        public string NegSoftwareLimit { get; set; }
        public string PosSoftwareLimit { get; set; }
        public string Home { get; set; }
        public string Jog { get; set; }
        public string Move { get; set; }

        public AxisCheckItem()
        {

        }

        public void Init(_Axis axis)
        {
            AxisName = axis.AxisName;
            if (ServoOnOff == null) ServoOnOff = "NA";
            if (NegLimitCheck == null) NegLimitCheck = "NA";
            if (PosLimitCheck == null) PosLimitCheck = "NA";
            if (NegSoftwareLimit == null) NegSoftwareLimit = string.Format("{0}", axis.NegLimitPos);
            if (PosSoftwareLimit == null) PosSoftwareLimit = string.Format("{0}", axis.PosLimitPos);
            if (Home == null) Home = "NA";
            if (Jog == null) Jog = "NA";
            if (Move == null) Move = "NA";
        }
    }
}
