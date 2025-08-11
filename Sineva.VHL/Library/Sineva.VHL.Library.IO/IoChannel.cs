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
using System.Drawing.Design;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
	public enum IoType
	{
		DI = 0,
		DO = 1,
		AI = 2,
		AO = 3,
        AX = 4, //Axis type
	}

	[Serializable]
	public class IoChannel : IChannelCommand
	{
		#region Fields
		protected int m_Id = -1;
		protected int m_Node = 0;
		protected int m_Terminal = 0;
		protected int m_ChannelNo = 0;
		protected string m_WiringNo = "";
		protected string m_Name = "";
		protected string m_Description = "";
		protected string m_State = "";
		protected IoType m_IoType = IoType.DI;
		private bool m_IsBContact = false;
        private bool m_DoManualNoUse;
        private List<IoTag> m_DiSafetyIntr = new List<IoTag>();
        private List<IoTag> m_DoSafetyIntr = new List<IoTag>();
		private bool m_SafetyIntrInverse = false;

		private AnalogScale m_Scale = new AnalogScale();
		#endregion

		#region Properties
		[Category("Address")]
		[ReadOnly(true)]
		public int Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}
		[Category("Address")]
		[ReadOnly(true)]
		public int Node
		{
			get { return m_Node; }
			set { m_Node = value; }
		}
		[Category("Address")]
		[ReadOnly(true)]
		public int Terminal
		{
			get { return m_Terminal; }
			set { m_Terminal = value; }
		}
		[Category("Address")]
		[ReadOnly(true)]
		public int ChannelNo
		{
			get { return m_ChannelNo; }
			set { m_ChannelNo = value; }
		}
		[Category("Address")]
		//[ReadOnly(true)]
		public string WiringNo
		{
			get { return m_WiringNo; }
			set { m_WiringNo = value; }
		}
		[Category("I/O Info")]
		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}
		[Category("I/O Info")]
		public string Description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}
		[Category("I/O Info")]
		[ReadOnly(true)]
		public IoType IoType
		{
			get { return m_IoType; }
			set { m_IoType = value; }
		}
		[Category("I/O Info")]
		[ReadOnly(true), XmlIgnore()]
		public string State
		{
			get { return m_State; }
			set { m_State = value; }
		}

		[Category("I/O Info")]
		public bool IsBContact
		{
			get { return m_IsBContact; }
			set { m_IsBContact = value; }
		}
        [Category("I/O Info")]
        public bool DoManualNoUse
        {
            get { return m_DoManualNoUse; }
            set { m_DoManualNoUse = value; }
        }
        [Category("I/O Info")]
		[Editor(typeof(UIEditorIoTags), typeof(UITypeEditor))]
        public List<IoTag> DiSafetyIntr
        {
            get { return m_DiSafetyIntr; }
            set { m_DiSafetyIntr = value; }
        }
        [Category("I/O Info")]
        public List<IoTag> DoSafetyIntr
        {
            get { return m_DoSafetyIntr; }
            set { m_DoSafetyIntr = value; }
        }
		[Category("I/O Info")]
		public bool SafetyIntrInverse
		{
			get { return m_SafetyIntrInverse; }
			set { m_SafetyIntrInverse = value; }
		}
		[Browsable(false), XmlIgnore()]
        public AnalogScale Scale
		{
			get { return m_Scale; }
			set { m_Scale = value; }
		}
		[Category("I/O Info"), ReadOnly(true), XmlIgnore()]
		public string StateAdcCal { get { return m_Scale.CalValue.ToString(); } }
        #endregion

        #region Constructor
        public IoChannel()
        {
		}
        public IoChannel(IoType type)
        {
            m_IoType = type;
        }
		public IoChannel(IoType type, int id)
        {
            m_IoType = type;
            m_Id = id;
		}
        #endregion
		
		#region Methods
		public virtual IoChannel Clone()
		{
			IoChannel dev = new IoChannel();
			dev.Id = m_Id;
			dev.Node = m_Node;
			dev.Terminal = m_Terminal;
			dev.ChannelNo = m_ChannelNo;
			dev.Description = m_Description;
			dev.IoType = m_IoType;
			dev.Name = m_Name;
			dev.WiringNo = m_WiringNo;

			return dev;
		}

		public override string ToString()
		{
			if (this.Name.Length == 0)
			{
				return this.GetType().Name;
			}
			else
			{
				return string.Format("{0}.{1}",this.Name, Id);
			}
		}
		#endregion

		#region IChannelCommand 멤버

		public void SetDo(bool val)
		{
			if (this.IoType == IO.IoType.DO)
			{
				bool intr = false;
				if (DiSafetyIntr.Count > 0)
				{
					for (int i = 0; i < DiSafetyIntr.Count; i++)
					{
						if (SafetyIntrInverse)
							intr |= DiSafetyIntr[i].GetChannel().GetDi() ? false : true;
						else intr |= DiSafetyIntr[i].GetChannel().GetDi() ? true : false;
					}
				}
				if (intr == false)
					this.State = val ? "ON" : "OFF";
			}
			else if (this.IoType == IO.IoType.DI)
            {
				this.State = val ? "ON" : "OFF";
			}
		}

		public bool GetDi()
		{
			bool rv = false;
			if (this.IoType == IO.IoType.DI || this.IoType == IO.IoType.DO)
			{
				rv = this.State == "ON" ? true : false;
			}
			return rv;
		}

		public void SetAo(double val)
		{
			if (this.IoType == IO.IoType.AI || this.IoType == IO.IoType.AO)
			{
				this.State = val.ToString();
			}
		}

		public double GetAi()
		{
			double rv = 0.0f;
			if (this.IoType == IO.IoType.AI || this.IoType == IO.IoType.AO)
			{
				if(m_Scale.UseScale)
				{
					int adc;
					if(int.TryParse(this.State, out adc))
					{
						m_Scale.CurAdc = adc;
						return m_Scale.GetCalValue();
					}
				}
				else
				{
					double temp = 0.0f;
					rv = double.TryParse(this.State, out temp) ? temp : 0.0f;
				}
			}
			return rv;
		}
		#endregion
	}
}
