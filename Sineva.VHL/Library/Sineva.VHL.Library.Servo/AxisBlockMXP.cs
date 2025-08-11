using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
	public class AxisBlockMXP : _AxisBlock
	{
		#region Field
		private bool m_HeartBitUsing = true;
		private int m_HeartBitTimeout = 1000; // ms 단위
		private enMotionBufferMode m_BufferMode = enMotionBufferMode.BlendingNext;
		private bool m_Connected = false;
		private bool m_IsHeartBitOk = false;
		private bool m_IsHeartBitError = false;

        // MXP PLC
        private bool m_CornerControlUsing = false;
        private uint m_CornerControlStartAddress = 5400;
        private uint m_CornerStateStartAddress = 5401;
        private uint m_CornerFunctionStartAddress = 5404;
        private MxpPlcCornerControl m_CornerControl;
        private MxpPlcCornerState m_CornerState;
        private MxpPlcCornerFunction m_CornerFunction;

        // MXP PLC 초기화 Parameter...torque 보상용
        private uint m_MasterNodeId = 1;
        private uint m_MasterMXConfigratorSlaveNo = 2;
        private uint m_SlaveNodeId = 0;
        private uint m_SlaveMXConfigratorSlaveNo = 1;
        private uint m_LeftBcrSlaveNo = 0; // default 8
        private uint m_RightBcrSlaveNo = 0; // default 7
        #endregion

        #region Properties
        public enMotionBufferMode BufferMode
        {
			get { return m_BufferMode; }
			set { m_BufferMode = value; }
		}
		public bool HeartBitUsing
		{
			get { return m_HeartBitUsing; }
			set { m_HeartBitUsing = value; }
		}
		public int HeartBitTimeout
		{
			get { return m_HeartBitTimeout; }
			set { m_HeartBitTimeout = value; }
		}
        [Browsable(false), XmlIgnore()]
		public bool Connected
		{
			get { return m_Connected; }
			set { m_Connected = value; }
		}
        [Browsable(false), XmlIgnore()]
        public bool HeartBitOk
        {
            get { return m_IsHeartBitOk; }
            set { m_IsHeartBitOk = value; }
        }
        [Browsable(false), XmlIgnore()]
        public bool IsHeartBitError
        {
            get { return m_IsHeartBitError; }
            set { m_IsHeartBitError = value; }
        }
        
        public bool CornerControlUsing
        {
            get { return m_CornerControlUsing; }
            set { m_CornerControlUsing = value; }
        }
        public uint CornerFunctionStartAddress 
        {
            get { return m_CornerFunctionStartAddress; }
            set { m_CornerFunctionStartAddress = value; }
        }
        public uint CornerControlStartAddress 
        {
            get { return m_CornerControlStartAddress; }
            set { m_CornerControlStartAddress = value; }
        }
        public uint CornerStateStartAddress 
        {
            get { return m_CornerStateStartAddress; }
            set { m_CornerStateStartAddress = value; }
        }
        public uint MasterNodeId
        {
            get { return m_MasterNodeId; }
            set { m_MasterNodeId = value; }
        }
        public uint MasterMXConfigratorSlaveNo
        {
            get { return m_MasterMXConfigratorSlaveNo; }
            set { m_MasterMXConfigratorSlaveNo = value; }
        }
        public uint SlaveNodeId
        {
            get { return m_SlaveNodeId; }
            set { m_SlaveNodeId = value; }
        }
        public uint SlaveMXConfigratorSlaveNo
        {
            get { return m_SlaveMXConfigratorSlaveNo; }
            set { m_SlaveMXConfigratorSlaveNo = value; }
        }
        public uint LeftBcrSlaveNo
        {
            get { return m_LeftBcrSlaveNo; }
            set { m_LeftBcrSlaveNo = value; }
        }
        public uint RightBcrSlaveNo
        {
            get { return m_RightBcrSlaveNo; }
            set { m_RightBcrSlaveNo = value; }
        }
        #endregion

        #region Constructor
        public AxisBlockMXP()
		{
			this.ControlFamily = ServoControlFamily.MXP;
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
		public SequenceCommand GetSequenceCommand(int axisId)
		{
			return m_Axes[axisId - StartAxisId].SequenceCommand;
        }
		public double  GetTargetPosition(int axisId)
		{
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetPos;
            return (m_Axes[axisId - StartAxisId] as MpAxis).Pulse2Len(target);
		}
		public double GetTargetDistance(int axisId)
		{
            double target = (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetDistance;
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
        public double GetTargetDec(int axisId)
        {
            return (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetDec;
        }
		public double GetTargetJerk(int axisId)
		{
			return (m_Axes[axisId - StartAxisId] as MpAxis).MpTargetJerk;
        }
        public double GetJogAcc(int axisId)
        {
            return (m_Axes[axisId - StartAxisId] as MpAxis).MpJogAcc;
        }
        public double GetJogDec(int axisId)
        {
            return (m_Axes[axisId - StartAxisId] as MpAxis).MpJogDec;
        }
        public double GetJogJerk(int axisId)
        {
            return (m_Axes[axisId - StartAxisId] as MpAxis).MpJogJerk;
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
            double curTorque = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(torque);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurTorque = curTorque / 100.0f;
		}

		public void SetCurSpeed(int axisId, double speed)
		{
            double curSpeed = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(speed);
            (m_Axes[axisId - StartAxisId] as MpAxis).CurSpeed = curSpeed;
		}
        public void SetCommandPosition(int axisId, double pos)
        {
            double cmdPos = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(pos);
            (m_Axes[axisId - StartAxisId] as MpAxis).CommandPos = cmdPos;
        }
        public void SetCommandSpeed(int axisId, double speed)
        {
            double cmdSpeed = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(speed);
            (m_Axes[axisId - StartAxisId] as MpAxis).CommandSpeed = cmdSpeed;
        }
        public void SetCommandSpeedPdo(int axisId, double speed)
        {
            double cmdSpeed = (m_Axes[axisId - StartAxisId] as MpAxis).Len2Pulse(speed);
            (m_Axes[axisId - StartAxisId] as MpAxis).CommandSpeedPdo = cmdSpeed;
        }
        public void SetFollowingError(int axisId, double followingError)
        {
            (m_Axes[axisId - StartAxisId] as MpAxis).FollowingError = followingError;
        }
        #endregion

        #region PLC Read/Write
        // PLC <- OHT
        public byte[] GetMxpPlcFunction()
        {
            return XFunc.StructureToByte(m_CornerFunction);
        }
        public void SetMxpPlcFunction(MxpPlcCornerFunction func)
        {
            m_CornerFunction = func;
        }
        // PLC <- OHT
        public byte[] GetMxpPlcControl()
        {
            return XFunc.StructureToByte(m_CornerControl);
        }
        public void SetMxpPlcControl(MxpPlcCornerControl con)
        {
            m_CornerControl = con;
        }
        // PLC -> OHT
        public MxpPlcCornerState GetMxpPlcState()
        {
            return m_CornerState;
        }
        public void SetMxpPlcState(MxpPlcCornerState state)
        {
            m_CornerState = state;
        }
        #endregion
    }
}
