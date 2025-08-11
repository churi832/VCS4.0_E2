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
using System.ComponentModel;

namespace Sineva.VHL.Library.IO
{
	[Serializable()]
	public class IoNodeMxpEC : _IoNode
	{
		#region Fields
        #endregion

        #region Properties
		#endregion

		#region Constructor
		public IoNodeMxpEC()
		{
			m_Name = "MXP";
			m_IoNodeBusType = IoBusType.Mxp;
		}
		#endregion
	}
}
