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
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
    public class MpAxis : _Axis, IAxisCommand
    {
		#region Fields
		private ushort m_Command;
		private float m_LoadCellZero;
		private float m_Diameter = 0.0f;
		private float m_StartAngle = 0.0f;

        private int m_CommandAlarmId = 0;
        //private int m_ControllerAlarmId = 0;
        //private int m_DriverAlarmId = 0;
        private int m_BCRReader1AlarmId = 0;
        private int m_BCRReader2AlarmId = 0;
        private List<int> m_ControllerAlarmIdList = new List<int>();
        private List<int> m_DriverAlarmIdList = new List<int>();
        private int m_ControllerAlarmCount = 0;
        private int m_DriverAlarmCount = 0;
        private double m_CommandPos; //Motor Controller 지령 위치
        private double m_CommandSpeed; //Motor Controller 지령 속도
        private double m_CommandSpeedPdo; //Controller -> Drive 지령 속도
        private double m_FollowingError;
        private bool m_HoldingBit = false;
        private bool m_PauseBit = false; //전방 Stop Sensor 감지 상태
        private enFrontDetectState m_OverrideSensorState = enFrontDetectState.enNone;
        private bool m_SetOverrideZeroTouchGUI = false;
        private bool m_BcrScanBit = false;
        private bool m_SequenceMoveCommandSet = false; // SequenceMove 명령을 만들고 MXP 지령이 내려가기전 ABS Move 조건이 만족되어 지령이 내려지는걸 막자

        private bool m_TorqueLimitUse = false;
        private bool m_TorqueLimitBit = false;
        private bool m_FollowingVelocityLimitUse = false; // Following Error 제한
        private double m_FollowingVelocityLimitValue = 500.0f;
        private int m_TorqueLimitUseSetAddress = 539;
        private int m_TorqueLimitAddress = 540;
        private int m_TorqueLimitOffstAddress = 541;
        private int m_TorqueFollowingGainK3Address = 544;
        private double m_TorqueLimitRate = 1100.0f;
        private double m_TorqueRunRate = 3000.0f;

        private double m_TargetOverrideRate = 1.0f;
        private double m_CurrentOverrideRate = 1.0f; //MXP Override
        private double m_OverrideStopDistance = 500.0f;
        private double m_OverrideLimitDistance = 300.0f;
        private double m_OverrideAcceleration = 3000.0f;
        private double m_OverrideDeceleration = 3000.0f;
        private double m_OverrideCollisionDistance = 10000.0f;
        private double m_OverrideMaxDistance = 1000.0f;
        private double m_OverrideMinDistance = 1000.0f;
        private double m_OverrideMaxVelocity = 500.0f;
        private double m_OverrideIncreaseTimeRatio = 2.0f;
        private double m_OverrideConstantVelocityRatio = 0.3f;
        private double m_SensorRemainDistance = 10000.0f;

        private double m_LenPerPulse = 1.0f;
        private double m_FilterGain = 0.5f;
        private Queue<float> m_Median = new Queue<float>();
        private static readonly object m_LockKey = new object();

        private double m_MpTargetPos = 0.0f;
        private double m_MpTargetSpeed = 0.0f;
        private double m_MpTargetAcc = 0.0f;
        private double m_MpTargetDec = 0.0f;
        private double m_MpTargetJerk = 0.0f;
        private double m_MpJogSpeed = 0.0f;
        private double m_MpJogAcc = 0.0f;
        private double m_MpJogDec = 0.0f;
        private double m_MpJogJerk = 0.0f;
        private double m_MpJogDistance = 0.0f;
        private double m_MpTargetDistance = 0.0f;

        private double m_TrajectoryTargetTime = 0.0f;
        private double m_TrajectoryCurrentTime = 0.0f;
        private double m_TrajectoryTargetVelocity = 0.0f;
        private int m_TrajectoryCurrentStep = 0;
        private int m_TrajectoryState = 0;

        private double m_OriginOnDetectError = 0.0f;
        #endregion

        #region Properties
        [Category("MP Motor")]
        public double LenPerPulse
        {
            get { return m_LenPerPulse; }
            set { m_LenPerPulse = value; }
        }
        [Category("MP Motor")]
        public double FilterGain
        {
            get { return m_FilterGain; }
            set { m_FilterGain = value; }
        }
        [Category("MP Motor")]
		public float Diameter
		{
			get { return m_Diameter; }
			set { m_Diameter = value; }
		}
		[Category("MP Motor")]
		public float StartAngle
		{
			get { return m_StartAngle; }
			set { m_StartAngle = value; }
		}
		[Category("MP Motor")]
		public float LoadCellZero
		{
			get { return m_LoadCellZero; }
			set { m_LoadCellZero = value; }
		}
        
		[XmlIgnore(), Browsable(false)]
		public ushort Command
		{
			get { return m_Command; }
			set { m_Command = value; }
		}


        [XmlIgnore(), Browsable(false)]
        public int CommandAlarmId
        {
            get { return m_CommandAlarmId; }
            set { m_CommandAlarmId = value; }
        }
        
        //[XmlIgnore(), Browsable(false)]
        //public int ControllerAlarmId
        //{
        //    get { return m_ControllerAlarmId; }
        //    set { m_ControllerAlarmId = value; }
        //}

        //[XmlIgnore(), Browsable(false)]
        //public int DriverAlarmId
        //{
        //    get { return m_DriverAlarmId; }
        //    set { m_DriverAlarmId =  value; }
        //}
        [XmlIgnore(), Browsable(false)]
        public List<int> ControllerAlarmIdList
        {
            get { return m_ControllerAlarmIdList; }
            set { m_ControllerAlarmIdList = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public List<int> DriverAlarmIdList
        {
            get { return m_DriverAlarmIdList; }
            set { m_DriverAlarmIdList = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int ControllerAlarmCount
        {
            get { return m_ControllerAlarmCount; }
            set { m_ControllerAlarmCount = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int DriverAlarmCount
        {
            get { return m_DriverAlarmCount; }
            set { m_DriverAlarmCount = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int BCRReader1AlarmId
        {
            get { return m_BCRReader1AlarmId; }
            set { m_BCRReader1AlarmId = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int BCRReader2AlarmId
        {
            get { return m_BCRReader2AlarmId; }
            set { m_BCRReader2AlarmId = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double CommandPos
        {
            get { return m_CommandPos; }
            set { m_CommandPos = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double CommandSpeed
        {
            get { return m_CommandSpeed; }
            set { m_CommandSpeed =  value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double CommandSpeedPdo
        {
            get { return m_CommandSpeedPdo; }
            set { m_CommandSpeedPdo = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double FollowingError
        {
            get { return m_FollowingError; }
            set { m_FollowingError = value; }
        }        
        [XmlIgnore(), Browsable(false)]
        public bool HoldingBit
        {
            get { return m_HoldingBit; }
            set { m_HoldingBit = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool PauseBit
        {
            get { return m_PauseBit; }
            set { m_PauseBit = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public enFrontDetectState OverrideSensorState
        {
            get { return m_OverrideSensorState; }
            set { m_OverrideSensorState = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool SetOverrideZeroTouchGUI
        {
            get { return m_SetOverrideZeroTouchGUI; }
            set { m_SetOverrideZeroTouchGUI = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool BcrScanBit
        {
            get { return m_BcrScanBit; }
            set { m_BcrScanBit = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool SequenceMoveCommandSet
        {
            get { return m_SequenceMoveCommandSet; }
            set { m_SequenceMoveCommandSet = value; }
        }
        
        [Category("MP Motor")]
        public bool TorqueLimitUse
        {
            get { return m_TorqueLimitUse; }
            set { m_TorqueLimitUse = value; }
        }        
        [Category("MP Motor")]
        public int TorqueLimitUseSetAddress
        {
            get { return m_TorqueLimitUseSetAddress; }
            set { m_TorqueLimitUseSetAddress = value; }
        }
        [Category("MP Motor")]
        public int TorqueLimitAddress
        {
            get { return m_TorqueLimitAddress; }
            set { m_TorqueLimitAddress = value; }
        }
        [Category("MP Motor")]
        public int TorqueLimitOffstAddress
        {
            get { return m_TorqueLimitOffstAddress; }
            set { m_TorqueLimitOffstAddress = value; }
        }
        [Category("MP Motor")]
        public int TorqueFollowingGainK3Address
        {
            get { return m_TorqueFollowingGainK3Address; }
            set { m_TorqueFollowingGainK3Address = value; }
        }
        [Category("MP Motor")]
        public double TorqueLimitRate
        {
            get { return m_TorqueLimitRate; }
            set { m_TorqueLimitRate = value; }
        }
        [Category("MP Motor")]
        public double TorqueRunRate
        {
            get { return m_TorqueRunRate; }
            set { m_TorqueRunRate = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool TorqueLimitBit
        {
            get { return m_TorqueLimitBit; }
            set { m_TorqueLimitBit = value; }
        }
        [Category("MP Motor")]
        public bool FollowingVelocityLimitUse
        {
            get { return m_FollowingVelocityLimitUse; }
            set { m_FollowingVelocityLimitUse = value; }
        }
        [Category("MP Motor")]
        public double FollowingVelocityLimitValue
        {
            get { return m_FollowingVelocityLimitValue; }
            set { m_FollowingVelocityLimitValue = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double TargetOverrideRate
        {
            get { return m_TargetOverrideRate; }
            set { m_TargetOverrideRate = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double CurrentOverrideRate
        {
            get { return m_CurrentOverrideRate; }
            set { m_CurrentOverrideRate = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideStopDistance
        {
            get { return m_OverrideStopDistance; }
            set { m_OverrideStopDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideLimitDistance
        {
            get { return m_OverrideLimitDistance; }
            set { m_OverrideLimitDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideAcceleration
        {
            get { return m_OverrideAcceleration; }
            set { m_OverrideAcceleration = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideDeceleration
        {
            get { return m_OverrideDeceleration; }
            set { m_OverrideDeceleration = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideCollisionDistance
        {
            get { return m_OverrideCollisionDistance; }
            set { m_OverrideCollisionDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideMaxDistance
        {
            get { return m_OverrideMaxDistance; }
            set { m_OverrideMaxDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideMinDistance
        {
            get { return m_OverrideMinDistance; }
            set { m_OverrideMinDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideMaxVelocity
        {
            get { return m_OverrideMaxVelocity; }
            set { m_OverrideMaxVelocity = value; }
        }        
        [XmlIgnore(), Browsable(false)]
        public double OverrideIncreaseTimeRatio
        {
            get { return m_OverrideIncreaseTimeRatio; }
            set { m_OverrideIncreaseTimeRatio = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OverrideConstantVelocityRatio
        {
            get { return m_OverrideConstantVelocityRatio; }
            set { m_OverrideConstantVelocityRatio = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double SensorRemainDistance
        {
            get { return m_SensorRemainDistance; }
            set { m_SensorRemainDistance = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public double MpTargetPos
        {
            get { return m_MpTargetPos; }
            set { m_MpTargetPos = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpTargetSpeed
        {
            get { return m_MpTargetSpeed; }
            set { m_MpTargetSpeed = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpTargetAcc
        {
            get { return m_MpTargetAcc; }
            set { m_MpTargetAcc = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpTargetDec
        {
            get { return m_MpTargetDec; }
            set { m_MpTargetDec = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpTargetJerk
        {
            get { return m_MpTargetJerk; }
            set { m_MpTargetJerk = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpTargetDistance
        {
            get { return m_MpTargetDistance; }
            set { m_MpTargetDistance = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpJogSpeed
        {
            get { return m_MpJogSpeed; }
            set { m_MpJogSpeed = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpJogAcc
        {
            get { return m_MpJogAcc; }
            set { m_MpJogAcc = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpJogDec
        {
            get { return m_MpJogDec; }
            set { m_MpJogDec = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpJogJerk
        {
            get { return m_MpJogJerk; }
            set { m_MpJogJerk = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MpJogDistance
        {
            get { return m_MpJogDistance; }
            set { m_MpJogDistance = value; }
        }

        [XmlIgnore(), Browsable(false)]
        public double TrajectoryTargetTime
        {
            get { return m_TrajectoryTargetTime; }
            set { m_TrajectoryTargetTime = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double TrajectoryCurrentTime
        {
            get { return m_TrajectoryCurrentTime; }
            set { m_TrajectoryCurrentTime = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double TrajectoryTargetVelocity
        {
            get { return m_TrajectoryTargetVelocity; }
            set { m_TrajectoryTargetVelocity = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int TrajectoryCurrentStep
        {
            get { return m_TrajectoryCurrentStep; }
            set { m_TrajectoryCurrentStep = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int TrajectoryState
        {
            get { return m_TrajectoryState; }
            set { m_TrajectoryState = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double OriginOnDetectError
        {
            get { return m_OriginOnDetectError; }
            set { m_OriginOnDetectError = value; }
        }        
        #endregion

        #region Constructor
        public MpAxis()
		{
		}
		#endregion

		#region Methods
		public double Pulse2Len(double val) // int32 => double
		{
			if (DecimalPoint < 0) return 0.0f;
			double len = (val / Math.Pow(10, DecimalPoint));
            double rv = len;
			return rv;
		}

		public double Len2Pulse(double val)
		{
			double len = Math.Round(val, 6);
			double rv = (len * Math.Pow(10, DecimalPoint));
			return rv;
		}
		#endregion

		#region IAxisCommand 멤버

		public void SetPosAsync(double pos)
		{
			lock (m_LockKey)
			{
				double p = 0.0f;
                double newVal = pos;
				if (ScaleInfo.Usage == Use.Use) p = ScaleInfo.GetX(newVal);
				else p = newVal;

                m_MpTargetPos = Len2Pulse(p);
			}
		}
        public void SetSpeedAsync(VelSet set)
		{
            m_MpTargetSpeed = Len2Pulse(set.Vel);
            m_MpTargetAcc = set.Acc;
            m_MpTargetDec = set.Dec;
            m_MpTargetJerk = set.Jerk;
		}

        public void SetJogSpeedAsync(VelSet set)
		{
            m_MpJogSpeed = Len2Pulse(set.Vel);
            m_MpJogAcc = set.Acc;
            m_MpJogDec = set.Dec;
            m_MpJogJerk = set.Jerk;
		}
        public void SetTargetDistance(double distance)
        {
            m_MpTargetDistance = distance;
        }
        public void SetCommandAsync(enAxisOutFlag command)
		{
			m_Command = (ushort)command;
		}
        public void SetSpeedOverrideRate(double rate)
        {
            m_TargetOverrideRate = rate;
        }
        public void SetHolding(bool hold)
        {
            m_HoldingBit = hold;
        }
        public void SetPause(bool pause)
        {
            m_PauseBit = pause;
        }
        public void SetOverrideSensorState(enFrontDetectState sensor_state)
        {
            m_OverrideSensorState = sensor_state;
        }
        public void SetSequenceMoveCommand(bool set)
        {
            m_SequenceMoveCommandSet = set;
        }
        public enAxisInFlag GetAxisCurStatus()
		{
			return (enAxisInFlag)m_AxisCurStatus;
		}
        public double GetAxisCurPos()
		{
			double newVal = 0.0f;
            lock (m_LockKey)
            {
                double pos = Pulse2Len(m_CurPos);

                double p = 0.0f;
                if (ScaleInfo.Usage == Use.Use) p = ScaleInfo.GetY(pos);
                else p = pos;

                newVal = Math.Round(p, 6);
            }
            return newVal;
		}

        public double GetAxisCurSpeed()
        {
            return Pulse2Len(m_CurSpeed);
        }
        
		public double GetAxisCurTorque()
		{
			double torque = Math.Round(m_CurTorque, 6);
			return torque;
		}
        public double GetAxisCurLeftBarcode()
        {
            double bcr = Math.Round(LeftBcrPos, 1);
            return bcr;
        }
        public double GetAxisCurRightBarcode()
        {
            double bcr = Math.Round(RightBcrPos, 1);
            return bcr;
        }
        #endregion

        public void SetHomeOffsetAsync(double offset)
        {
            HomeOffset += offset;
        }
        public void SetCalibration(Scale scale)
        {
            ScaleInfo = scale;
        }
        public enAxisOutFlag GetAxisCurCommand()
        {
            return (enAxisOutFlag)m_Command;
        }

        public bool GetTorqueType()
        {
            return (MotorType == enMotorType.Torque) ? true : false;
        }
        public bool GetAcsType()
        {
            return (MotorType == enMotorType.Acs) ? true : false;
        }
        public double GetAxisCurHomeOffset()
        {
            return Pulse2Len(HomeOffset);
        }
        public double GetAxisTargetSpeed()
        {
            return Pulse2Len(TargetSpeed);
        }
        public double GetSpeedOverrideRate()
        {
            return m_CurrentOverrideRate;
        }
    }
}
