using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.Servo
{
	public enum enACSChannelType
	{
		Serial,
		Network,
		PCI,
		Simulator,
	}

	public enum enACSProtocolType
	{
		PointToPoint,
		Network,
	}

	[Serializable]
	public class AxisBlockACS : _AxisBlock
	{
		#region Field
		// ACS Channel
		private enACSChannelType m_ChannelType = enACSChannelType.Network;
		// Serial Setting
		private PortNo m_PortNo = PortNo.COM1;
		private int m_BaudRate = 19200;
		// Networdk Setting
		private string m_RemoteIpString = "192.168.0.1";
		private enACSProtocolType m_ProtocolType = enACSProtocolType.PointToPoint;
		//PCI Setting
		private int m_PciSlotNo = 1;

		private bool m_Connected = false;
        #endregion

        #region Properties
        public enACSChannelType ChannelType
		{
			get { return m_ChannelType; }
			set { m_ChannelType = value; }
		}
		public PortNo PortNo
		{
			get { return m_PortNo; }
			set { m_PortNo = value; }
		}
		public int BaudRate
		{
			get { return m_BaudRate; }
			set { m_BaudRate = value; }
		}

		public string RemoteIpString
		{
			get { return m_RemoteIpString; }
			set { m_RemoteIpString = value; }
		}
		public enACSProtocolType ProtocolType
		{
			get { return m_ProtocolType; }
			set { m_ProtocolType = value; }
		}
		public int PciSlotNo
		{
			get { return m_PciSlotNo; }
			set { m_PciSlotNo = value; }
		}

        [Browsable(false), XmlIgnore()]
		public bool Connected
		{
			get { return m_Connected; }
			set { m_Connected = value; }
		}
		#endregion

		#region Constructor
		public AxisBlockACS()
		{
			this.ControlFamily = ServoControlFamily.ACS230;
		}

		public void Initialize(ServoControlFamily control)
		{
            this.ControlFamily = control;
		}
		#endregion

		#region Methods
		public int GetAxisCount()
		{
			return m_Axes.Count;
		}

		public ushort GetMotionCommand(int axisId)
		{
            return (m_Axes[axisId - StartAxisId] as MpAxis).Command;
		}

		public double  GetTargetPosition(int axisId)
		{
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetPos;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
		}

		public double GetTargetSpeed(int axisId)
		{
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetSpeed;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
		}

		public double GetJogSpeed(int axisId)
		{
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpJogSpeed;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
		}

		public double GetTargetAcc(int axisId)
		{
            return (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetAcc;
		}

		public void SetMotionState(int axisId, enAxisInFlag state)
		{
            (m_Axes[axisId - StartAxisId] as MpAxis).AxisStatus = state;
		}

		public void SetCurPosition(int axisId, double pos)
		{
            double curPos = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(pos);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurPos = curPos;
		}

		public void SetCurTorque(int axisId, double torque)
		{
            int curTorque = (int)(m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(torque);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurTorque = curTorque;
		}

		public void SetCurSpeed(int axisId, double speed)
		{
            int curSpeed = (int)(m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(speed);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurSpeed = curSpeed;
		}
		#endregion
	}
}
