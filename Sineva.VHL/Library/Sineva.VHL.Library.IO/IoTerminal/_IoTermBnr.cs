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
    [Serializable]
	public abstract class _IoTermBnr : _IoTerminal
	{
		#region Fields
		protected int m_BusNo;
		#endregion

		#region Properties
		[Category("!Address")]
		public int BusNo
        {
			get { return m_BusNo; }
            set { m_BusNo = value; }
        }
		#endregion

		public _IoTermBnr()
		{
			m_IoTermBusType = IoBusType.BnR;
		}

		#region Methods
		public string GetBusNo()
		{
			return string.Format("{0}", m_BusNo);
		}
		#endregion

		#region [Override]
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.m_Name))
			{
				return base.ToString() + "." + this.IoTypes;
			}
			else
				return BusNo + "." + this.m_Name + "." + this.Id;
		}
		#endregion
	}
}
