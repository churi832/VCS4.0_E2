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
	public abstract class _IoTermCrevis : _IoTerminal
	{
		#region Fields
		protected int m_NodeNo;
		protected IoType m_TermIoType = IoType.DI;
		protected ushort m_ChannelBitSize = 0x01;
		protected ushort m_ProductNo = 0x0000;
		protected string m_Description = string.Empty;

		private ushort? m_ImageWordStart;
		private ushort? m_ImageWordBitOffset;
		private ushort? m_ImageWordBitSize;
		#endregion

		#region Properties
		[Category("!Address")]
		public int NodeNo
		{
			get { return m_NodeNo; }
			set { m_NodeNo = value; }
		}
		[ReadOnly(true)]
		public IoType TermIoType
		{
			get { return m_TermIoType; }
			set { m_TermIoType = value; }
		}
		[System.Xml.Serialization.XmlIgnore()]
		public ushort ChannelBitSize { get { return m_ChannelBitSize; } }
		[System.Xml.Serialization.XmlIgnore()]
		public string Description { get { return m_Description; } }
		[TypeConverter(typeof(UInt16HexTypeConverter))]
		public ushort ProductNo { get { return m_ProductNo; } }

		[XmlIgnore(), Browsable(false)]
		public ushort? ImageWordStart
		{
			get { return m_ImageWordStart; }
			set { m_ImageWordStart = value; }
		}
		[XmlIgnore(), Browsable(false)]
		public ushort? ImageWordBitOffset
		{
			get { return m_ImageWordBitOffset; }
			set { m_ImageWordBitOffset = value; }
		}
		[XmlIgnore(), Browsable(false)]
		public ushort? ImageWordBitSize
		{
			get { return m_ImageWordBitSize; }
			set { m_ImageWordBitSize = value; }
		}
		#endregion

		public _IoTermCrevis()
		{
			m_IoTermBusType = IoBusType.CrevisModbus;
		}

		#region Methods
		public string GetBusNo()
		{
			return string.Format("{0}", m_NodeNo);
		}
		#endregion

		#region [Override]
		public override string ToString()
		{
			if(string.IsNullOrEmpty(this.m_Name))
			{
				return base.ToString() + "." + this.IoTypes;
			}
			else
				return NodeNo + "." + this.m_Name + "." + this.Id;
		}
		#endregion
	}
}
