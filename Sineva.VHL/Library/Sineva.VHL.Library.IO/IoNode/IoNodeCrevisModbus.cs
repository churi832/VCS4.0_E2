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
	public abstract class IoNodeCrevisModbus : _IoNode
	{
		#region Fields
		protected ushort m_ProductNo = 0x0000;
		protected ushort m_MaxSlotNo = 0;
		protected string m_Description = string.Empty;

		protected ushort m_PortNo = 502;
		protected string m_IpAddress = "___.___.___.___";
		protected uint m_TimeOut = 10000;
		#endregion

		#region Properties
		[Category("MODUBUS TCP")]
		public ushort PortNo
		{
			get { return m_PortNo; }
		}
		[Category("MODUBUS TCP")]
		public string IpAddress
		{
			get { return m_IpAddress; }
			set { m_IpAddress = value; }
		}
		[Category("MODUBUS TCP")]
		public uint TimeOut
		{
			get { return m_TimeOut; }
			set { m_TimeOut = value; }
		}

		public ushort MaxSlaveNo { get { return m_MaxSlotNo; } }
		public string Description { get { return m_Description; } }
		[TypeConverter(typeof(UInt16HexTypeConverter))]
		public ushort ProductNo { get { return m_ProductNo; } }
				
		[Browsable(false)]
		public ushort ExpansionSlotWordSize { get { return (ushort)(m_MaxSlotNo + 2); } }
		#endregion

		#region Constructor
		public IoNodeCrevisModbus()
		{
			m_Name = "";
			m_IoNodeBusType = IoBusType.CrevisModbus;
		}
		#endregion

		
	}
}
