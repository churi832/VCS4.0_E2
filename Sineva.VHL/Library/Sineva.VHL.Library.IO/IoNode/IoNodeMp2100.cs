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
	public enum enMp2100CpuNo : ushort
	{
		No1 = 1,
		No2 = 2,
		No3 = 3,
		No4 = 4,
	}

	[Serializable()]
	public class IoNodeMp2100 : _IoNode
	{
		#region Fields
		private enMp2100CpuNo m_CpuNo = enMp2100CpuNo.No1;
		private ushort m_PortNo = 1;
		private ushort m_NetworkNo = 0;
		private ushort m_StationNo = 0;
		private ushort m_UnitNo = 0;
		private string m_IpAddress = "";
		private uint m_TimeOut = 10000;
		#endregion

		#region Properties
		[Category("CMotionAPI COM_DEVICE")]
		public enMp2100CpuNo CpuNo
		{
			get { return m_CpuNo; }
			set { m_CpuNo = value; }
		}

		[Category("CMotionAPI COM_DEVICE")]
		public ushort PortNo
		{
			get { return m_PortNo; }
			set { m_PortNo = value; }
		}

		[Category("CMotionAPI COM_DEVICE")]
		public ushort NetworkNo
		{
			get { return m_NetworkNo; }
			set { m_NetworkNo = value; }
		}

		[Category("CMotionAPI COM_DEVICE")]
		public ushort StationNo
		{
			get { return m_StationNo; }
			set { m_StationNo = value; }
		}

		[Category("CMotionAPI COM_DEVICE")]
		public ushort UnitNo
		{
			get { return m_UnitNo; }
			set { m_UnitNo = value; }
		}

		[Category("CMotionAPI COM_DEVICE")]
		public string IpAddress
		{
			get { return m_IpAddress; }
			set { m_IpAddress = value; }
		}

		[Category("CMotionAPI COM_DEVICE")]
		public uint TimeOut
		{
			get { return m_TimeOut; }
			set { m_TimeOut = value; }
		}
		#endregion

		#region Constructor
		public IoNodeMp2100()
		{
			m_Name = "Mp2100";
            m_IoNodeBusType = IoBusType.MP2100;
		}
		#endregion
	}
}
