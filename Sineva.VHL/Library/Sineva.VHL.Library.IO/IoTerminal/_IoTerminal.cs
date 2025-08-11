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
    public abstract class _IoTerminal : ITerminalFactory
	{
		#region Fields
		protected int m_Id;
		protected int m_IdByIoType;
		protected List<IoType> m_IoTypes = new List<IoType>();
		protected int m_ChannelCount;
		protected List<IoChannel> m_Channels = new List<IoChannel>();
		protected string m_Name = "";
		protected int m_InStartAddress = 0;
		protected int m_OutStartAddress = 0;
        //private string m_Description = ""; 
        protected IoBusType m_IoTermBusType = IoBusType.NotDefine;

        protected UInt32 m_InValue = 0;
        protected UInt32 m_OutValue = 0;
        #endregion

        #region Properties
        [Category("!Address"), ReadOnly(true)]
		public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        [Category("!Address"), ReadOnly(true)]
        public int IdByIoType
        {
            get { return m_IdByIoType; }
            set { m_IdByIoType = value; }
        }
        [XmlIgnore()]
		[Category("!Terminal Info"), ReadOnly(true)]
		public List<IoType> IoTypes
        {
            get { return m_IoTypes; }
            set { m_IoTypes = value; }
        }
        //[Category("Terminal Info")]
        //[ReadOnly(true)]
        //public TerminalType TerminalType
        //{
        //    get { return m_TerminalType; }
        //    set { m_TerminalType = value; }        
        //}
        [Category("!Terminal Info"), ReadOnly(true)]
        public int ChannelCount
        {
            get { return m_ChannelCount; }
            set { m_ChannelCount = value; }
        }
		[Category("!Terminal Info"), ReadOnly(true)]
		public List<IoChannel> Channels
        {
            get { return m_Channels; }
            set { m_Channels = value; }
        }
        //[Category("Terminal Info")]
        //[ReadOnly(true)]
        //public string Description
        //{
        //    get { return m_Description; }
        //} 
		[Category("!Address"), TypeConverter(typeof(Int32HexTypeConverter))]
		public int InStartAddress
		{
			get { return m_InStartAddress; }
			set { m_InStartAddress = value; }
		}

		[Category("!Address"), TypeConverter(typeof(Int32HexTypeConverter))]
		public int OutStartAddress
		{
			get { return m_OutStartAddress; }
			set { m_OutStartAddress = value; }
		}
        [Category("!Terminal Info"), ReadOnly(true)]
        public IoBusType IoTermBusType
        {
            get { return m_IoTermBusType; }
        }

        [Category("!Terminal Info"), XmlIgnore()]
        public UInt32 InValue
        {
            get { return m_InValue; }
            set { m_InValue = value; }
        }
        [Category("!Terminal Info"), XmlIgnore()]
        public UInt32 OutValue
        {
            get { return m_OutValue; }
            set { m_OutValue = value; }
        }
        #endregion

        #region Constructor
        public _IoTerminal()
        {

        }
        #endregion

        #region [Virtual Methods]
        public virtual void CreateChannels()
        {
            if (this.Channels.Count != 0) return;

            for (int i = 0; i < m_ChannelCount; i++)
            {
                IoChannel item = new IoChannel();
				foreach(IoType type in IoTypes)
				{
					switch (type)
					{
						case IoType.DI:
							//item = new IoItemDI();
							item.Name = "di__";
							break;
						case IoType.DO:
							item.Name = "do__";
							break;
						case IoType.AI:
							item.Name = "ai__";
							break;
						case IoType.AO:
							item.Name = "ao__";
							break;
					}
					item.ChannelNo = i;
					item.IoType = type;
					this.Channels.Add(item);
				}
            }
        }

		public virtual void BuildChannels()
		{
			int inAdd = this.InStartAddress;
			int outAdd = this.OutStartAddress;

			foreach (IoChannel ch in Channels)
			{
				if (ch.IoType == IoType.DI)
				{
					ch.WiringNo = string.Format("{0:X4}H", inAdd++);
				}
				else if (ch.IoType == IoType.DO)
				{
					ch.WiringNo = string.Format("{0:X4}H", outAdd++);
				}
			}
		}
        public virtual void BuildChannels(int nodeNo, ref int startDI, ref int startDO, ref int startAI, ref int startAO)
        {            
            foreach(IoChannel ch in Channels)
            {
                if(ch.IoType == IoType.DI) ch.WiringNo = string.Format("{0}X{1:X4}", nodeNo == 0 ? "" : nodeNo.ToString(), startDI++);
                else if(ch.IoType == IoType.DO) ch.WiringNo = string.Format("{0}Y{1:X4}", nodeNo == 0 ? "" : nodeNo.ToString(), startDO++);
                else if(ch.IoType == IoType.AI) ch.WiringNo = string.Format("{0}AI{1:X4}", nodeNo == 0 ? "" : nodeNo.ToString(), startAI++);
                else if(ch.IoType == IoType.AO) ch.WiringNo = string.Format("{0}AO{1:X4}", nodeNo == 0 ? "" : nodeNo.ToString(), startAO++);
            }
        }
        #endregion

        #region [Methods]
		public List<IoChannel> GetChannels()
		{
			return this.Channels;
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
				return this.m_Name;
        } 
        #endregion

        #region [ITerminalFactory 멤버]
        public _IoTerminal CreateObject()
        {
            return Activator.CreateInstance(this.GetType()) as _IoTerminal;
        }
        #endregion
	}
}
