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
	public abstract class _IoTermMp : _IoTerminal
	{
		public enum enMpChannelType
		{
			bitCh,
			wordCh,
		};

		public enum enMpDeviceName
		{
			IL,
			OL,
			IA,
			OA,
            MW,
		};

		//public enum enMpInOutType
		//{
		//    In,
		//    Out,
		//    InOut,
		//}

		#region Fields
		//protected int m_InStartAddress = 0;
		//protected int m_OutStartAddress = 0;
		protected int m_InChannelCnt = 0;
		protected int m_OutChannelCnt = 0;
		protected UInt32 m_InRegHandle;
		protected UInt32 m_OutRegHandle;

		protected enMpDeviceName m_InRegPrefix;
		protected enMpDeviceName m_OutRegPrefix;
		protected enMpChannelType m_ChType;
		//private enMpInOutType m_InOutType;
		protected uint m_InRegisterCount;
		protected uint m_OutRegisterCount;
		#endregion

		#region Properties
		//[Category("!Address")]
		//public int InStartAddress
		//{
		//    get { return m_InStartAddress; }
		//    set { m_InStartAddress = value; }
		//}

		//[Category("!Address")]
		//public int OutStartAddress
		//{
		//    get { return m_OutStartAddress; }
		//    set { m_OutStartAddress = value; }
		//}

		[Category("!Terminal Info"), ReadOnly(true)]
		public uint OutRegisterCount
		{
			get { return m_OutRegisterCount; }
		}

		[Category("!Terminal Info"), ReadOnly(true)]
		public uint InRegisterCount
		{
			get { return m_InRegisterCount; }
		}

		[Category("!Terminal Info"), ReadOnly(true)]
		public enMpChannelType ChType
		{
			get { return m_ChType; }
			set { m_ChType = value; }
		}

		[Category("!Address"), ReadOnly(true)]
		public enMpDeviceName InRegPrefix
		{
			get { return m_InRegPrefix; }
		}

		[Category("!Address"), ReadOnly(true)]
		public enMpDeviceName OutRegPrefix
		{
			get { return m_OutRegPrefix; }
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

		public _IoTermMp()
		{
			m_IoTermBusType = IoBusType.MP2100;
		}

		#region Methods
		public string GetHexStartAddress()
		{
			return string.Format("{0:4X}H", m_InStartAddress);
		}
		#endregion
	}
}
