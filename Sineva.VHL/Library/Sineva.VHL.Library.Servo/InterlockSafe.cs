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
	public class InterlockSafe
	{
		#region Fields
		private _Axis m_Axis = null;
		private int m_AxisId = -1;
		private float m_Low = 0;
		private float m_High = 0;
		#endregion

		#region Properties
		#endregion

		#region Constructor
		public InterlockSafe(int axisId, float low, float high)
		{
			m_AxisId = axisId;
			m_Low = low;
			m_High = high;
		}

		public bool IsSafe()
		{
			float cur = (float)(ServoManager.Instance.GetAxisById(m_AxisId) as IAxisCommand).GetAxisCurPos();
			if (m_Low <= cur && cur <= m_High)
			{
				return true;
			}
			else
				return false;
				
		}
		#endregion
	}

	public class InterlockDangerSensor
	{

	}
}
