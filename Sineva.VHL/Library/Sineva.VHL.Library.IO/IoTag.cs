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
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
    [Editor(typeof(UIEditorIoTag), typeof(UITypeEditor))]
	public class IoTag
	{
		#region Fields
		private IoChannel m_Channel = null;
		private bool m_ChannelFindDone = false;
		private bool m_IsInverted = false;
        private bool m_ChannelNotFound = false;
        #endregion

        #region Properties
        [DeviceSetting(true)]
		public string Name { get; set; }
        [DeviceSetting(true)]
        public bool IsInverted
		{
			get { return m_IsInverted; }
			set { m_IsInverted = value; }
		}
        //public int Id { get; set; }
        [DeviceSetting(true)]
        public IoType IoType { get; set; }
		[XmlIgnore]
		public bool IsDetected
		{
			get
			{
				bool detect = false;
				if (m_Channel != null && !m_ChannelNotFound) 
					detect = GetChannel().GetDi();
				return detect;	
			}
		}
		#endregion

		#region Constructor
		public IoTag()
		{
		}
		#endregion

		#region Methods
		public IoChannel GetChannel()
		{
            if(m_ChannelNotFound) return null;

			if (m_ChannelFindDone == false)
			{
				m_Channel = IoManager.Instance.GetChannelByName(IoType, Name);
				m_ChannelFindDone = true;
				if (m_Channel == null && !m_ChannelNotFound)
				{
                    m_ChannelNotFound = true;
                    //MessageBox.Show(string.Format("There is no '{0}' in the I/O define", Name), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    EventHandlerManager.Instance.InvokeConfigErrorHappened(string.Format("There is no '{0}' in the I/O define", Name));
				}
			}
			return m_Channel;
		}
        public void SetDo(bool val)
		{
			if (m_Channel != null && !m_ChannelNotFound) m_Channel.SetDo(val);
		}
        public bool GetDi()
		{
			bool val = false;
			if (m_Channel != null && !m_ChannelNotFound) val = m_Channel.GetDi();
			return val;
		}
        public void SetAo(double val)
		{
            if (m_Channel != null && !m_ChannelNotFound) m_Channel.SetAo(val);
        }
        public double GetAi()
		{
            double val = 0.0f;
            if (m_Channel != null && !m_ChannelNotFound) val = m_Channel.GetAi();
            return val;

        }
        public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
			{
				return base.ToString();
			}
			else
			{
				if (m_IsInverted == false)
					return Name;
				else
					return string.Format("!{0}", Name);
			}
		}
		#endregion
	}
}
