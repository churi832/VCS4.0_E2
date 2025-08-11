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
    public class ServoCmdProxy
	{
		public bool CmdTrigger { get; set; }
		public enAxisOutFlag Cmdtype { get; set; }

		public ServoCmdProxy()
		{
		}

		public bool SetCmd(enAxisOutFlag cmd)
		{
			if( cmd != enAxisOutFlag.MotionStop &&
				cmd != enAxisOutFlag.ServoOff )
			{
				if (CmdTrigger == true)
					return false;
			}

			CmdTrigger = true;
			Cmdtype = cmd;

			return true;
		}

		public enAxisOutFlag GetCurCmd()
		{
			return Cmdtype;
		}
	}

}
