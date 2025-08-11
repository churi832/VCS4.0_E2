using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
    [Flags]
    public enum enBnrCmdFlag : ushort
    {
        cmdInit = 1 << 0,
        cmdOpen = 1 << 1,
        cmdClose = 1 << 2,
        cmdStop = 1 << 3,
        cmdReset = 1 << 4,
        cmdForce = 1 << 5,
    }
    [Serializable]
    [Flags]
    public enum enBnrState : ushort
    {
        stSTOP = 1 << 0,
        stOPEN = 1 << 1,
        stCLOSE = 1 << 2,
        stMOVING = 1 << 3,
        stERROR = 1 << 4,
    }

    [Serializable()]
	public class BnrAxisChannel : IoChannel
	{
        #region Fields
        private bool m_IsValid = true;

        protected enBnrCmdFlag m_Command = 0;
        protected float m_ParaMoveTorque = 50;	// 50% .. 구동 torque (50)
        protected float m_ParaPeakTorque = 70;    // 70% .. peak torque (80)
        protected float m_ParaMoveTime = 10;		// 10sec .. move time (50)
        protected float m_ParaHoldTorque = 20;	// 20% .. 유지 torque (15)
        protected float m_ParaTolerance = 10.0f;	// 0.1mm ~ 1.0mm (10)
        protected float m_ParaMaxCurrent = 0.01f;	// Motor Gain Value (0.005)
        protected int m_ParaOpenDirection = 0;	// 기본(0: +)방향, (1: -)일 경우 반대로 움직임.
        protected float m_ParaForceSet = 0;		// 강제 출력
        protected float m_ParaResolution = 0.08527f;	// resolution
        protected short m_MoveTimeout = 50;

        protected enBnrState m_Status = 0;
        protected ushort m_LastAlarmCode = 0;
        protected float m_CurOpenPosition = 0.0f;
        protected float m_CurDistance = 0.0f;
        protected float m_CurHoldPosition = 0.0f;
        protected float m_CurPosition = 0.0f;
        protected float m_CurCurrent = 0.0f;
        #endregion

        #region Properties - BNR Control Handshake
        [Category("AXIS Info"), Description("Channel Use/Nouse")]
        public bool IsValid { get { return m_IsValid; } set { m_IsValid = value; } }
        [Browsable(false), XmlIgnore()]
        public enBnrCmdFlag Command { get { return m_Command; } set { m_Command = value; if (CommandChangedEvent != null) CommandChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Start Moving Torque (5 ~ 50 %)")]
        public float ParaMoveTorque { get { return m_ParaMoveTorque; } set { m_ParaMoveTorque = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Complete Torque (MoveTorque ~ 80 %)")]
        public float ParaPeakTorque { get { return m_ParaPeakTorque; } set { m_ParaPeakTorque = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Moving Time (5 ~ 10 sec)")]
        public float ParaMoveTime { get { return m_ParaMoveTime; } set { m_ParaMoveTime = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Close Hold Torque (~20% < MoveTorque)")]
        public float ParaHoldTorque { get { return m_ParaHoldTorque; } set { m_ParaHoldTorque = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("0.1 ~ 1 mm")]
        public float ParaTolerance { get { return m_ParaTolerance; } set { m_ParaTolerance = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Motor Spec. Max Current")]
        public float ParaMaxCurrent { get { return m_ParaMaxCurrent; } set { m_ParaMaxCurrent = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Open Direction(default = 1)")]
        public int ParaOpenDirection { get { return m_ParaOpenDirection; } set { m_ParaOpenDirection = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Force Set Torque")]
        public float ParaForceSet { get { return m_ParaForceSet; } set { m_ParaForceSet = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Encorder Resolution")]
        public float ParaResolution { get { return m_ParaResolution; } set { m_ParaResolution = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }
        [Category("AXIS Info"), Description("Move Timeout  (sec)")]
        public short MoveTimeout { get { return m_MoveTimeout; } set { m_MoveTimeout = value; if (ParameterChangedEvent != null) ParameterChangedEvent(this, EventArgs.Empty); } }


        [Browsable(false), XmlIgnore()]
        public enBnrState Status { get { return m_Status; } set { m_Status = value; } }
        [Browsable(false), XmlIgnore()]
        public ushort LastAlarmCode { get { return m_LastAlarmCode; } set { m_LastAlarmCode = value; } }
        [Browsable(false), XmlIgnore()]
        public float CurOpenPosition { get { return m_CurOpenPosition; } set { m_CurOpenPosition = value; } }
        [Browsable(false), XmlIgnore()]
        public float CurDistance { get { return m_CurDistance; } set { m_CurDistance = value; } }
        [Browsable(false), XmlIgnore()]
        public float CurHoldPosition { get { return m_CurHoldPosition; } set { m_CurHoldPosition = value; } }
        [Browsable(false), XmlIgnore()]
        public float CurPosition { get { return m_CurPosition; } set { m_CurPosition = value; } }
        [Browsable(false), XmlIgnore()]
        public float CurCurrent { get { return m_CurCurrent; } set { m_CurCurrent = value; } }
        #endregion

        #region Properties - OP Control Handshake
        [Browsable(false), XmlIgnore()]
        public bool cmdInit
        {
            set
            {
                if (value == true) Command |= enBnrCmdFlag.cmdInit;
                else if (Command.HasFlag(enBnrCmdFlag.cmdInit)) Command ^= enBnrCmdFlag.cmdInit;
            }
        }
        [Browsable(false), XmlIgnore()]
        public bool cmdOpen
        {
            set
            {
                if (value == true) Command |= enBnrCmdFlag.cmdOpen;
                else if (Command.HasFlag(enBnrCmdFlag.cmdOpen)) Command ^= enBnrCmdFlag.cmdOpen;
            }
        }
        [Browsable(false), XmlIgnore()]
        public bool cmdClose
        {
            set
            {
                if (value == true) Command |= enBnrCmdFlag.cmdClose;
                else if (Command.HasFlag(enBnrCmdFlag.cmdClose)) Command ^= enBnrCmdFlag.cmdClose;
            }
        }
        [Browsable(false), XmlIgnore()]
        public bool cmdStop
        {
            set
            {
                if (value == true) Command |= enBnrCmdFlag.cmdStop;
                else Command ^= enBnrCmdFlag.cmdStop;
            }
        }
        [Browsable(false), XmlIgnore()]
        public bool cmdReset
        {
            set
            {
                if (value == true) Command |= enBnrCmdFlag.cmdReset;
                else if (Command.HasFlag(enBnrCmdFlag.cmdReset)) Command ^= enBnrCmdFlag.cmdReset;
            }
        }
        [Browsable(false), XmlIgnore()]
        public bool cmdForce
        {
            set
            {
                if (value == true) Command |= enBnrCmdFlag.cmdForce;
                else if (Command.HasFlag(enBnrCmdFlag.cmdForce)) Command ^= enBnrCmdFlag.cmdForce;
            }
        }

        [Browsable(false), XmlIgnore()]
        public bool IsStop { get { return (Status & enBnrState.stSTOP) == enBnrState.stSTOP ? true : false; } }
        [Browsable(false), XmlIgnore()]
        public bool IsClampOpen { get { return (Status & enBnrState.stOPEN) == enBnrState.stOPEN ? true : false; } }
        [Browsable(false), XmlIgnore()]
        public bool IsClampClose { get { return (Status & enBnrState.stCLOSE) == enBnrState.stCLOSE ? true : false; } }
        [Browsable(false), XmlIgnore()]
        public bool IsBUSY { get { return (Status & enBnrState.stMOVING) == enBnrState.stMOVING ? true : false; } }
        [Browsable(false), XmlIgnore()]
        public bool IsAlarm { get { return (Status & enBnrState.stERROR) == enBnrState.stERROR ? true : false; } }
        #endregion

        #region Event
        public event EventHandler CommandChangedEvent;
        public event EventHandler ParameterChangedEvent;
        #endregion

        #region Constructor
        public BnrAxisChannel()
        {
		}
        public BnrAxisChannel(IoType type)
        {
            m_IoType = type;
        }
		public BnrAxisChannel(IoType type, int id)
        {
            m_IoType = type;
            m_Id = id;
		}
        #endregion
		
		#region Methods - ToString
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
	}
}
