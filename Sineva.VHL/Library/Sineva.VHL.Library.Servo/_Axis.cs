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

namespace Sineva.VHL.Library.Servo
{
    public enum enHomeMethod
    {
        NONE,
        NOT,                // NOT 탐색 후, HomeOffset 이동
        POT,                // POT 탐색 후, HomeOffset 이동
        NOT_HOME,           // NOT 탐색 후, HOME 센서 탐색, HomeOffset 이동
        NOT_CPHASE,         // NOT 탐색 후, C상 탐색, HomeOffset 이동
        POT_HOME,           // POT 탐색 후, HOME 센서 탐색, HomeOffset 이동
        POT_CPHASE,         // POT 탐색 후, C상 탐색, HomeOffset 이동
        ABS_ZERO,           // 절대치 모터 0 위치 이동
        ZERO_MOVE_HOME,        // Slave 축에 대해서만 적용, 현재의 Master/Slave 축 위치 상태를 유지하도록 함
        PLC,                // PLC에서 Home을 실행하도록 함.
    }

    public enum enAxisDir
    {
        Positive,
        Negative
    }

    public enum enAxisCoord
    {
        X,
        Y,
        Z,
        T
    }
    public enum enMotionMoveMethod
    {
        Absolute,
        Relative,
    }
    public enum enMotionBufferMode
    {
        Aborting = 0,
        Buffered = 1,
        BlendingLow = 2,
        BlendingPrevious = 3,
        BlendingNext = 4,
        BlendingHigh = 5,
        SingleBlock = 6
    }
    [Serializable]
    public abstract class _Axis : IAxisFactory
    {
        #region Fields
        private int m_AxisId;
        private int m_NodeId = 1; // RS485 or ACS 같은 경우...
        private int m_SlaveNodeId = -1; //use when Gantry Mode, 미설정 -1
        private int m_MasterNodeId = -1; //원점 이동 할때 완료 확인을 위한 NodeId, 미설정 -1
        private int m_LeftBcrNodeId = -1;
        private int m_RightBcrNodeId = -1;
        private int m_MXConfigratorSlaveNo = -1;
        private string m_AxisName = "";
        private string m_AxisDescription = "";

        private bool m_IsValid = false; //Motor 사용 유무
		private int m_HomeOrder = 0;
        private int m_MoveOrder = 0;
        private bool m_CommandSkip = false;
        private bool m_GantryType = false; // Master & Slave link
        private bool m_InPosCheckSkip = false;

        private enMxpBcrControl m_BcrControl = enMxpBcrControl.MXP;
        private enMotorType m_MotorType = enMotorType.Normal;
        private enAxisCoord m_AxisCoord = enAxisCoord.X;
        private ushort m_DecimalPoint = 4; // 제어기와 주고 받는 배수값....제어기에서 10곱하여 줄 경우.... 10을 나누어서 계산
        private Scale m_ScaleInfo = new Scale();
        private MotionSensor m_MotionSensorPara = new MotionSensor();
        private string m_AxisStateMsg = string.Empty; // mxp 축 상태를 표시
        private string m_AixsAlarmMsg = string.Empty; // Alarm 상태 Display용

        private bool m_TorqueCheckUse = true;
        private bool m_NegLimitUse = true;
        private bool m_PosLimitUse = true;
        private float m_NegLimitPos = -1;
        private float m_PosLimitPos = 1;
        private float m_SpeedLimit = 100;
        private float m_AccLimit = 100;
        private float m_DecLimit = 100;
        private float m_JerkLimit = 100;
        private float m_AccDefault = 2800;
        private float m_DecDefault = 2800;
        private float m_JerkDefault = 2200;

        private double m_InRangeValue = 0.001f;
        private Int32 m_InposWaitTime = 5000; // msec
        private Int32 m_SetTimeoutMargin = 10000; // msec
        private int m_AlarmStartId = 0;
        private double m_LeftBCROffset = 0.0f;
        private double m_RightBCROffset = 0.0f;
        private double m_LeftBCRCmdOffset = 0.0f;
        private double m_RightBCRCmdOffset = 0.0f;

        private enHomeMethod m_HomeMethod = enHomeMethod.NOT_CPHASE;
        private uint m_HomeSearchSpeed = 100;
        private uint m_HomeCreepSpeed = 10;
        private double m_HomeOffset = 0.0f;

        protected ServoCmdProxy m_CmdPxy = new ServoCmdProxy();
        protected SegmentCommand m_SegmentCommand = new SegmentCommand();
        protected SequenceCommand m_SequenceCommand = new SequenceCommand();
        protected SequenceState m_SequenceState = new SequenceState();
        protected bool m_Repeat = false;

        protected enAxisResult m_CmdResult = new enAxisResult();
        protected AxisCheckItem m_CheckedItem = new AxisCheckItem();
        protected enAxisInFlag m_AxisCurStatus;
        protected string m_LastErrorMsg = "";
        protected double m_CurPos; // Int32 => double
        protected double m_LeftBcrPos;
        protected double m_RightBcrPos;
        protected double m_CurSpeed;
        protected double m_CurTorque;

        private double m_TargetPos = 0.0f;

        private List<ushort> m_MXPErrorTest = new List<ushort>();
        private List<ushort> m_DriveErrorTest = new List<ushort>();
        #endregion

        #region Properties
        [Category("Axis Setting")]
        public bool IsValid
		{
			get { return m_IsValid; }
			set { m_IsValid = value; }
        }
        [Category("Motor Setting")]
        public int MoveOrder
        {
            get { return m_MoveOrder; }
            set { m_MoveOrder = value; }
        }
        [Category("Motor Setting")]
        public int HomeOrder
		{
			get { return m_HomeOrder; }
			set { m_HomeOrder = value; }
		}
        [Category("Motor Setting")]
        public bool CommandSkip
        {
            get { return m_CommandSkip; }
            set { m_CommandSkip = value; }
        }
        [Category("Motor Setting")]
        public bool InPosCheckSkip
        {
            get { return m_InPosCheckSkip; }
            set { m_InPosCheckSkip = value; }
        }
        [Category("Motor Setting")]
        public float NegLimitPos
        {
            get { return m_NegLimitPos; }
            set { m_NegLimitPos = value; }
        }
        [Category("Motor Setting")]
        public float PosLimitPos
        {
            get { return m_PosLimitPos; }
            set { m_PosLimitPos = value; }
        }
        [Category("Motor Setting")]
        public float SpeedLimit
        {
            get { return m_SpeedLimit; }
            set { m_SpeedLimit = value; }
        }
        [Category("Motor Setting")]
        public float AccLimit
        {
            get { return m_AccLimit; }
            set { m_AccLimit = value; }
        }
        [Category("Motor Setting")]
        public float DecLimit
        {
            get { return m_DecLimit; }
            set { m_DecLimit = value; }
        }
        [Category("Motor Setting")]
        public float JerkLimit
        {
            get { return m_JerkLimit; }
            set { m_JerkLimit = value; }
        }
        [Category("Motor Setting")]
        public float AccDefault
        {
            get { return m_AccDefault; }
            set { m_AccDefault = value; }
        }
        [Category("Motor Setting")]
        public float DecDefault
        {
            get { return m_DecDefault; }
            set { m_DecDefault = value; }
        }
        [Category("Motor Setting")]
        public float JerkDefault
        {
            get { return m_JerkDefault; }
            set { m_JerkDefault = value; }
        }
        [Category("Motor Setting")]
        public double InRangeValue
        {
            get { return m_InRangeValue; }
            set { m_InRangeValue = value; }
        }
        [Category("Motor Setting")]
        public Int32 InposWaitTime
        {
            get { return m_InposWaitTime; }
            set { m_InposWaitTime = value; }
        }
        [Category("Motor Setting")]
        public Int32 SetTimeoutMargin
        {
            get { return m_SetTimeoutMargin; }
            set { m_SetTimeoutMargin = value; }
        }
        [Category("Axis Setting"), ReadOnly(true)]
        public int AxisId
		{
			get { return m_AxisId; }
			set { m_AxisId = value; }
		}
        [Category("Axis Setting")]
        public string AxisName
		{
			get { return m_AxisName; }
			set { m_AxisName = value; }
		}
        [Category("Axis Setting")]
        public string AxisDescription
		{
			get { return m_AxisDescription; }
			set { m_AxisDescription = value; }
		}
        [Category("Axis Setting")]
        public bool GantryType
        {
            get { return m_GantryType; }
            set { m_GantryType = value; }
        }
        [Category("Axis Setting")]
        public int NodeId
        {
            get { return m_NodeId; }
            set { m_NodeId = value; }
        }
        [Category("Axis Setting")]
        public int SlaveNodeId
        {
            get { return m_SlaveNodeId; }
            set { m_SlaveNodeId = value; }
        }
        [Category("Axis Setting")]
        public int MasterNodeId
        {
            get { return m_MasterNodeId; }
            set { m_MasterNodeId = value; }
        }
        [Category("Axis Setting")]
        public int LeftBcrNodeId
        {
            get { return m_LeftBcrNodeId; }
            set { m_LeftBcrNodeId = value; }
        }
        [Category("Axis Setting")]
        public int RightBcrNodeId
        {
            get { return m_RightBcrNodeId; }
            set { m_RightBcrNodeId = value; }
        }
        [Category("Axis Setting")]
        public int MXConfigratorSlaveNo
        {
            get { return m_MXConfigratorSlaveNo; }
            set { m_MXConfigratorSlaveNo = value; }
        }
        
        [Category("Axis Setting"), ReadOnly(true)]
        public int AlarmStartId
        {
            get { return m_AlarmStartId; }
            set { m_AlarmStartId = value; }
        }

        [Category("Axis Setting")]
        public double LeftBCROffset
        {
            get { return m_LeftBCROffset; }
            set { m_LeftBCROffset = value; }
        }
        [Category("Axis Setting")]
        public double RightBCROffset
        {
            get { return m_RightBCROffset; }
            set { m_RightBCROffset = value; }
        }
        [Category("Axis Setting")]
        public double LeftBCRCmdOffset
        {
            get { return m_LeftBCRCmdOffset; }
            set { m_LeftBCRCmdOffset = value; }
        }
        [Category("Axis Setting")]
        public double RightBCRCmdOffset
        {
            get { return m_RightBCRCmdOffset; }
            set { m_RightBCRCmdOffset = value; }
        }
        [Category("Axis Setting")]
        public double HomeOffset 
        {
            get { return m_HomeOffset; }
            set { m_HomeOffset = value; }
        }
        [Category("Axis Setting")]
        public enMxpBcrControl BcrControl
        {
            get { return m_BcrControl; }
            set { m_BcrControl = value; }
        }
        [Category("Axis Setting")]
        public enMotorType MotorType
        {
            get { return m_MotorType; }
            set { m_MotorType = value; }
        }
        [Category("Axis Setting")]
        public enAxisCoord AxisCoord 
        {
            get { return m_AxisCoord; }
            set { m_AxisCoord = value; } 
        }
        [Category("Motor Origin Move")]
        public enHomeMethod HomeMethod
        {
            get { return m_HomeMethod; }
            set { m_HomeMethod = value; }
        }
        [Category("Motor Origin Move")]
        public uint HomeSearchSpeed
        {
            get { return m_HomeSearchSpeed; }
            set { m_HomeSearchSpeed = value; }
        }
        [Category("Motor Origin Move")]
        public uint HomeCreepSpeed
        {
            get { return m_HomeCreepSpeed; }
            set { m_HomeCreepSpeed = value; }
        }
        
        [Category("Axis Setting")]
        public bool TorqueCheckUse
        {
            get { return m_TorqueCheckUse; }
            set { m_TorqueCheckUse = value; }
        }
        [Category("Axis Setting")]
        public bool NegLimitUse
        {
            get { return m_NegLimitUse; }
            set { m_NegLimitUse = value; }
        }
        [Category("Axis Setting")]
        public bool PosLimitUse
        {
            get { return m_PosLimitUse; }
            set { m_PosLimitUse = value; }
        }
        [Category("Axis Setting")]
        public Scale ScaleInfo
        {
            get { return m_ScaleInfo; }
            set { m_ScaleInfo = value; }
        }
        [Category("Axis Setting")]
        public ushort DecimalPoint
        {
            get { return m_DecimalPoint; }
            set { m_DecimalPoint = value; }
        }
        [Category("Axis Setting")]
        public MotionSensor MotionSensorPara
        {
            get { return m_MotionSensorPara; }
            set { m_MotionSensorPara = value; }
        }        
        #endregion

        #region Commad Set
        [Browsable(false), XmlIgnore()]
        public ServoCmdProxy CmdPxy
        {
            get { return m_CmdPxy; }
            set { CmdPxy = value; }
        }
        [Browsable(false), XmlIgnore()]
        public bool Repeat
        {
            get { return m_Repeat; }
            set { m_Repeat = value; }
        }
        public double BufferPos { get; set; }
        public double TargetPos 
        {
            get
            {
                return m_TargetPos;
            }
            set 
            {
                m_TargetPos = value;
                EventHandlerManager.Instance.InvokeUpdateTargetPosition(AxisId); 
            }
        }
        public double TargetSpeed { get; set; }
        public double TargetAcc { get; set; }
        public double TargetDec { get; set; }
        public double TargetJerk { get; set; }
        public double TargetDistance { get; set; }
        public double JogSpeed { get; set; }
        public double JogAcc { get; set; }
        public double JogDec { get; set; }
        public double JogJerk { get; set; }
        public double JogDistance { get; set; }

        [XmlIgnore(), Browsable(false)]
        public SegmentCommand SegmentCommand
        {
            get { return m_SegmentCommand; }
            set { m_SegmentCommand = value; }
        }
        [XmlIgnore()]
        public SequenceCommand SequenceCommand
        {
            get { return m_SequenceCommand; }
            set { m_SequenceCommand = value; }
        }
        [XmlIgnore()]
        public SequenceState SequenceState
        {
            get { return m_SequenceState; }
            set { m_SequenceState = value; }
        }        
        #endregion

        #region Axis Status
        [XmlIgnore()]
        public double CurPos // int32 => double
        {
            get { return m_CurPos; }
            set { m_CurPos = value; }
        }
        [XmlIgnore()]
        public double LeftBcrPos // int32 => double
        {
            get { return m_LeftBcrPos; }
            set { m_LeftBcrPos = value; }
        }
        [XmlIgnore()]
        public double RightBcrPos // int32 => double
        {
            get { return m_RightBcrPos; }
            set { m_RightBcrPos = value; }
        }

        [XmlIgnore()]
        public double CurTorque
        {
            get { return m_CurTorque; }
            set { m_CurTorque = value; }
        }

        [XmlIgnore()]
        public double CurSpeed
        {
            get { return m_CurSpeed; }
            set { m_CurSpeed = value; }
        }

        [Browsable(false), XmlIgnore()]
        public string LastErrorMsg
        {
            get { return m_LastErrorMsg; }
            set { m_LastErrorMsg = value; }
        }
        [Browsable(false), XmlIgnore()]
        public enAxisResult CmdResult
        {
            get { return m_CmdResult; }
            set { m_CmdResult = value; }
        }
        [Browsable(false)]
        [Category("Axis Status Check")]
        public AxisCheckItem CheckedItem
        {
            get { return m_CheckedItem; }
            set { m_CheckedItem = value; }
        }
        [XmlIgnore()]
        public enAxisInFlag AxisStatus
        {
            get { return m_AxisCurStatus; }
            set { m_AxisCurStatus = value; }
        }
        [XmlIgnore]
        public string AxisStateMsg
        {
            get { return m_AxisStateMsg; }
            set { m_AxisStateMsg = value;}
        }
        [XmlIgnore]
        public string AixsAlarmMsg
        {
            get { return m_AixsAlarmMsg; }
            set { m_AixsAlarmMsg = value; }
        }

        [Browsable(false)]
        public List<ushort> MXPErrorTest
        {
            get { return m_MXPErrorTest; }
            set { m_MXPErrorTest = value; }
        }
        [Browsable(false)]
        public List<ushort> DriveErrorTest
        {
            get { return m_DriveErrorTest; }
            set { m_DriveErrorTest = value; }
        }
        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(m_AxisName))
                return base.ToString();
            else
                return string.Format("{0}-{1}", m_AxisName, m_AxisDescription);
        }

		#region Method
		public _Axis CreateObject()
		{
			_Axis axis = Activator.CreateInstance(this.GetType()) as _Axis;
			return axis;
		}
		#endregion
	}
}
