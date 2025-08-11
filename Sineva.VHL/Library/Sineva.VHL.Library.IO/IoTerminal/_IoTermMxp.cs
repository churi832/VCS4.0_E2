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
using System.Xml.Serialization;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
	public abstract class _IoTermMxp : _IoTerminal
	{
        #region Fields
        private int m_SlaveNo = 0;
        protected int m_InChannelCnt = 0;
        protected int m_OutChannelCnt = 0;
        #endregion

        #region Properties
        [Category("!Terminal Info")]
        public int SlaveNo
        {
            get { return m_SlaveNo; }
            set { m_SlaveNo = value; }
        }
        [Category("!Terminal Info"), ReadOnly(true)]
        public int InChannelCnt
        {
            get { return m_InChannelCnt; }
        }

        [Category("!Terminal Info"), ReadOnly(true)]
        public int OutChannelCnt
        {
            get { return m_OutChannelCnt; }
        }
        #endregion

        public _IoTermMxp()
		{
			m_IoTermBusType = IoBusType.Mxp;
		}

		#region Methods
		public string GetSlaveNo()
		{
			return string.Format("{0}", m_SlaveNo);
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
				return m_SlaveNo + "." + this.m_Name + "." + this.Id;
		}
		#endregion
	}
}
