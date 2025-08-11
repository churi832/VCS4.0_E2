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
	public class IoNodeBnrX20CP0482 : _IoNode
	{
		#region Fields
		private ushort m_PortNo = 1;
		private string m_IpAddress = "";
		private uint m_TimeOut = 10000;
        #endregion

        #region Properties
        [Category("BNR MODUBUS")]
        public ushort PortNo
        {
            get { return m_PortNo; }
            set { m_PortNo = value; }
        }
        [Category("BNR MODUBUS")]
		public string IpAddress
		{
			get { return m_IpAddress; }
			set { m_IpAddress = value; }
		}

		[Category("BNR MODUBUS")]
		public uint TimeOut
		{
			get { return m_TimeOut; }
			set { m_TimeOut = value; }
		}
		#endregion

		#region Constructor
		public IoNodeBnrX20CP0482()
		{
			m_Name = "BNR";
			m_IoNodeBusType = IoBusType.BnR;
		}
		#endregion
	}
}
