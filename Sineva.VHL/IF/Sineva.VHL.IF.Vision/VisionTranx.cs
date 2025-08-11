using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.Vision
{

    public class VisionCommand
	{
		#region Fields
		public enVisionCommandCode m_PrimaryOutCode;
        public enVisionCommandCode m_SecondaryInCode;
        public enVisionCommandCode m_PrimaryInCode;
        public enVisionCommandCode m_SecondaryOutCode;
        public string m_PrimaryOutString = "";
        public string m_SecondaryInString = "";
        public string m_PrimaryInString = "";
        public string m_SecondaryOutString = "";
        
		public bool m_WaitFlag = true;
		public bool m_OnlyHeader = false;
		public string m_SndData;
		public string m_RcvData;

        public bool m_PrimaryOut = false;
        public bool m_SecondaryIn = false;
        public bool m_PrimaryIn = false;
        public bool m_SecondaryOut = false;
        
		public VisionClient m_VisionClient = VisionClient.Instance;
		#endregion

		#region Properties
		public string SndData
		{
			get { return m_SndData; }
			set { m_SndData = value; }
		}

		public string RcvData
		{
			get { return m_RcvData; }
			set { m_RcvData = value; }
		}
		#endregion

		#region Constructor
		public VisionCommand()
		{
		}
		// Primary와 Secondary Code가 동일 함.
		public VisionCommand(enVisionCommandCode pCode, bool onlyHeader, bool wait)
		{
			m_PrimaryOutCode = pCode;
			m_PrimaryOutString = string.Format("{0}", m_PrimaryOutCode);
			m_SecondaryInCode = pCode;
			m_SecondaryInString = string.Format("{0}", m_SecondaryInCode);
            if (wait)
            {
                m_PrimaryInCode = pCode;
                m_PrimaryInString = string.Format("{0}", (int)m_PrimaryInCode);
                m_SecondaryOutCode = pCode;
                m_SecondaryOutString = string.Format("{0}", (int)m_SecondaryOutCode);
            }

            m_OnlyHeader = onlyHeader;
			m_WaitFlag = wait;

			if (m_VisionClient.VisionCommandLists.Contains(this) == false)
				m_VisionClient.VisionCommandLists.Add(this);
        }
		#endregion

		#region Methods Virtual
		public virtual bool SendPrimary()
		{
			return true;
		}
		public virtual bool SendPrimary(string data)
		{
			throw new NotImplementedException();
		}
		public virtual bool SendPrimary(enVisionDevice dev)
		{
			throw new NotImplementedException();
		}
		public virtual bool SendPrimary(enVisionDevice dev, string data)
		{
			throw new NotImplementedException();
		}
		public virtual bool SendPrimary(enVisionDevice dev, string data1, string data2)
		{
			throw new NotImplementedException();
		}
		public virtual void SendSecondary()
		{
			throw new NotImplementedException();
		}
		public virtual void SendSecondary(string ack)
		{
			throw new NotImplementedException();
		}

		public virtual enVisionResult IsPrimaryRcvd()
		{
			throw new NotImplementedException();
		}

		public virtual enVisionResult IsPrimaryRcvd(ref string data)
		{
			throw new NotImplementedException();
		}
		public virtual enVisionResult IsSecondaryRcvd(ref XyPosition outP)
		{
			throw new NotImplementedException();
		}
		public virtual enVisionResult IsSecondaryRcvd(ref XyPosition outP, ref XyPosition avgCd, ref double height)
		{
			throw new NotImplementedException();
		}
		public virtual enVisionResult IsSecondaryRcvd(ref XyPosition Camera1_Left, ref XyPosition Camera1_Right, ref XyPosition Camera2_Left_Down, ref XyPosition Camera2_Left_Up, ref XyPosition Camera2_Right_Down, ref XyPosition Camera2_Right_Up)
		{
			throw new NotImplementedException();
		}
		public virtual enVisionResult IsSecondaryRcvd()
		{
			throw new NotImplementedException();
		}
		public virtual enVisionResult IsSecondaryRcvd(ref string outData)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Methods Protected
		protected bool _SendPrimary(string data)
		{
			if (m_VisionClient.IsConnected())
			{
				if (m_OnlyHeader)
					m_SndData = string.Format("{0}\r", m_PrimaryOutString);
				else
					m_SndData = string.Format("{0},{1}\r", m_PrimaryOutString, data);

				m_SecondaryIn = false;
				m_PrimaryIn = false;
                bool rv = m_VisionClient.SendData(this);
				if (rv)
				{
					m_PrimaryOut = true;
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		protected void _SendSecondary(string data)
		{
			if (m_VisionClient.IsConnected())
			{
				m_SndData = string.Format("{0},{1}\r", m_SecondaryOutString, data);

                m_PrimaryIn = false;
                m_SecondaryIn = false;
                bool rv = m_VisionClient.SendData(this);
				if (rv)
				{
					m_SecondaryOut = true;
				}
            }
        }
		protected bool _IsPrimaryRcvd(ref string data)
		{
			if (m_PrimaryIn == true)
			{
				data = m_RcvData;
			}
			return m_PrimaryIn;
		}
		protected enVisionResult _IsSecondaryRcvd(ref string data)
		{
			enVisionResult rv = enVisionResult.WAIT;

			if (m_SecondaryIn)
			{
				rv = enVisionResult.OK;
				data = m_RcvData;
			}
			return rv;
		}
		#endregion
	}
}
