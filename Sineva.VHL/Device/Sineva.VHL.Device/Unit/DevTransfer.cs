using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Data;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Library.MXP;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.IF.OCS;
using System.Xml.Linq;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevTransfer : _Device
    {
        private const string DevName = "DevTransfer";

        #region Fields
        private DevAxisTag m_AxisMaster = new DevAxisTag();
        private DevAxisTag m_AxisSlave = new DevAxisTag();

        private _DevInput m_MoveInterlockSensors = new _DevInput();

        private _DevOutput m_DoWheelBrake = new _DevOutput();
        private _DevInput m_DiWheelBrake = new _DevInput();

        private AlarmData m_ALM_NotDefine = null;
        private AlarmData m_ALM_SettingError = null;
        private AlarmData m_ALM_MoveInterlockBummperError = null;
        private AlarmData m_ALM_MoveInterlockSteerNotChangeError = null;
        private AlarmData m_ALM_MoveInterlockHoistPositionError = null;
        private AlarmData m_ALM_MoveInterlockSlidePositionError = null;
        private AlarmData m_ALM_MoveInterlockWheelBrakeError = null;
        private AlarmData m_ALM_MoveInterlockForwardDetectError = null;
        private AlarmData m_ALM_MoveInterlockFoupExistButGripOpenError = null;
        private AlarmData m_ALM_PathCalculateBcrAbnormal = null;
        private AlarmData m_ALM_SequenceMoveInRangeError = null;
        private AlarmData m_ALM_ActivationInitFail = null;
        private AlarmData m_ALM_MoveInterlockHoistHomeSensorNotDetectError = null;
        private AlarmData m_ALM_MoveInterlockSlideHomeSensorNotDetectError = null;
        private AlarmData m_ALM_MoveInterlockRotateHomeSensorNotDetectError = null;

        private VelocityData m_TeachingVelocitySearch = null;

        private SeqTransfer m_SeqTransfer = null;
        private MotionSensor m_MotionSensor = new MotionSensor();
        private List<MotionProfile> m_MotionProfiles = new List<MotionProfile>();
        private double m_MotionStartPosition = 0.0f;
        private double m_MotionRunPosition = 0.0f;
        private int m_MotionStep = 0;
        private bool m_SetMotionCommand = false; // 새로운 명령이 만들어졌다고 하고 지령을 내렸으면 FALSE 시킨다.
        private bool m_PlusMoved = false;
        private double m_WheelMasterWorkingDistance = 0.0d;
        private double m_WheelSlaveWorkingDistance = 0.0d;
        #endregion

        #region Properties
        [Category("!Setting Device (Axis)"), Description("Vehicle Master Motor"), DeviceSetting(true)]
        public DevAxisTag AxisMaster { get { return m_AxisMaster; } set { m_AxisMaster = value; } }
        [Category("!Setting Device (Axis)"), Description("Vehicle Slave Motor"), DeviceSetting(true)]
        public DevAxisTag AxisSlave { get { return m_AxisSlave; } set { m_AxisSlave = value; } }
        [Category("!Setting Device (Interlock)"), Description("Master Axis Move Check Sensor"), DeviceSetting(true)]
        public _DevInput MoveInterlockSensors
        {
            get { return m_MoveInterlockSensors; }
            set { m_MoveInterlockSensors = value; }
        }
        [Category("I/O Setting (Digital Output)"), Description("Wheel Motor Brake"), DeviceSetting(true)]
        public _DevOutput DoWheelBrake { get { return m_DoWheelBrake; } set { m_DoWheelBrake = value; } }
        [Category("I/O Setting (Digital Input)"), Description("Wheel Motor Brake Release"), DeviceSetting(true)]
        public _DevInput DiWheelBrake { get { return m_DiWheelBrake; } set { m_DiWheelBrake = value; } }

        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SettingError
        {
            get { return m_ALM_SettingError; }
            set { m_ALM_SettingError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockBummperError
        {
            get { return m_ALM_MoveInterlockBummperError; }
            set { m_ALM_MoveInterlockBummperError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockSteerNotChangeError
        {
            get { return m_ALM_MoveInterlockSteerNotChangeError; }
            set { m_ALM_MoveInterlockSteerNotChangeError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockHoistPositionError
        {
            get { return m_ALM_MoveInterlockHoistPositionError; }
            set { m_ALM_MoveInterlockHoistPositionError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockSlidePositionError
        {
            get { return m_ALM_MoveInterlockSlidePositionError; }
            set { m_ALM_MoveInterlockSlidePositionError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockWheelBrakeError
        {
            get { return m_ALM_MoveInterlockWheelBrakeError; }
            set { m_ALM_MoveInterlockWheelBrakeError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockForwardDetectError
        {
            get { return m_ALM_MoveInterlockForwardDetectError; }
            set { m_ALM_MoveInterlockForwardDetectError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockFoupExistButGripOpenError
        {
            get { return m_ALM_MoveInterlockFoupExistButGripOpenError; }
            set { m_ALM_MoveInterlockFoupExistButGripOpenError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_PathCalculateBcrAbnormal
        {
            get { return m_ALM_PathCalculateBcrAbnormal; }
            set { m_ALM_PathCalculateBcrAbnormal = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_SequenceMoveInRangeError
        {
            get { return m_ALM_SequenceMoveInRangeError; }
            set { m_ALM_SequenceMoveInRangeError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ActivationInitFail
        {
            get { return m_ALM_ActivationInitFail; }
            set { m_ALM_ActivationInitFail = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockHoistHomeSensorNotDetectError
        {
            get { return m_ALM_MoveInterlockHoistHomeSensorNotDetectError; }
            set { m_ALM_MoveInterlockHoistHomeSensorNotDetectError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockSlideHomeSensorNotDetectError
        {
            get { return m_ALM_MoveInterlockSlideHomeSensorNotDetectError; }
            set { m_ALM_MoveInterlockSlideHomeSensorNotDetectError = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_MoveInterlockRotateHomeSensorNotDetectError
        {
            get { return m_ALM_MoveInterlockRotateHomeSensorNotDetectError; }
            set { m_ALM_MoveInterlockRotateHomeSensorNotDetectError = value; }
        }
        [Category("Velocity"), Description("Sensor Search Prop"), DeviceSetting(true)]
        public VelocityData TeachingVelocitySearch
        {
            get { return m_TeachingVelocitySearch; }
            set { m_TeachingVelocitySearch = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MotionStartPosition
        {
            get { return m_MotionStartPosition; }
            set { m_MotionStartPosition = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public double MotionRunPosition
        {
            get { return m_MotionRunPosition; }
            set { m_MotionRunPosition = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int MotionStep
        {
            get { return m_MotionStep; }
            set { m_MotionStep = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("WheelMaster Working Distance"), Description("Device Life WheelMaster Working Distance"), DeviceSetting(false, true)]
        public double WheelMasterWorkingDistance
        {
            get { return m_WheelMasterWorkingDistance; }
            set { SaveCurState = m_WheelMasterWorkingDistance != value; m_WheelMasterWorkingDistance = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("WheelSlave Working Distance"), Description("Device Life WheelSlave Working Distance"), DeviceSetting(false, true)]
        public double WheelSlaveWorkingDistance
        {
            get { return m_WheelSlaveWorkingDistance; }
            set { SaveCurState = m_WheelSlaveWorkingDistance != value; m_WheelSlaveWorkingDistance = value; }
        }
        #endregion

        #region Constructor
        public DevTransfer()
        {
            if (!Initialized)
                this.MyName = DevName;
        }
        #endregion

        #region Override
        public override bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            // 신규 Device 생성 시, _Device.Initialize() 내용 복사 후 붙여넣어서 사용하시오
            this.ParentName = name;
            if (read_xml) ReadXml();
            if (this.IsValid == false) return true;

            //////////////////////////////////////////////////////////////////////////////
            #region 1. 이미 초기화 완료된 상태인지 확인
            if (Initialized)
            {
                if (false)
                {
                    // 초기화된 상태에서도 변경이 가능한 항목을 추가
                }
                return true;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 2. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            m_ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Device Not Define Alarm");
            m_ALM_SettingError = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Parameter Setting Alarm");
            m_ALM_MoveInterlockBummperError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Bummper Collision Alarm");
            m_ALM_MoveInterlockSteerNotChangeError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Steer Not Change Alarm");
            m_ALM_MoveInterlockHoistPositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Hoist Position Alarm");
            m_ALM_MoveInterlockSlidePositionError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Slide Position Alarm");
            m_ALM_MoveInterlockWheelBrakeError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Wheel Brake Alarm");
            m_ALM_MoveInterlockForwardDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Forward Stop Alarm");
            m_ALM_MoveInterlockFoupExistButGripOpenError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Foup Exist But Grip Open Alarm");
            m_ALM_PathCalculateBcrAbnormal = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Path Calculate BCR Abnormal Interlock Alarm");
            m_ALM_SequenceMoveInRangeError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Sequence Move Inrange Alarm");
            m_ALM_ActivationInitFail = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Transfer Activation Initialize Fail Alarm");
            m_ALM_MoveInterlockHoistHomeSensorNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Hoist Home Sensor Not Detected Alarm");
            m_ALM_MoveInterlockSlideHomeSensorNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Slide Home Sensor Not Detected Alarm");
            m_ALM_MoveInterlockRotateHomeSensorNotDetectError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, ParentName, "Move Interlock Because Rotate Home Sensor Not Detected Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 3. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;
            ok &= IsValidCheckTag(m_AxisMaster) ? m_AxisMaster.GetDevAxis() != null : true;
            ok &= IsValidCheckTag(m_AxisSlave) ? m_AxisSlave.GetDevAxis() != null : true;
            if (m_MoveInterlockSensors.IsValid) ok &= m_MoveInterlockSensors.Initialize(this.ToString(), false, true);
            if (m_DoWheelBrake.IsValid) ok &= m_DoWheelBrake.Initialize(this.ToString(), false, false);
            if (m_DiWheelBrake.IsValid) ok &= m_DiWheelBrake.Initialize(this.ToString(), false, false);
            if (m_AxisMaster.IsValid) ok &= m_TeachingVelocitySearch != null;

            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "WheelMaster Working Distance", this, "GetWorkingDistance", 1000, 0));
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "WheelSlave Working Distance", this, "GetWorkingDistance", 1000, 1));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                SeqMonitor monitor = new SeqMonitor(this);
                m_SeqTransfer = new SeqTransfer(this);
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 6. Initialize 마무으리
            Initialized = true;
            Initialized &= ok;
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            return Initialized;
        }
        #endregion

        #region Method
        public override void SeqAbort()
        {
            if (!Initialized) return;

            m_SetMotionCommand = false; //이중으로 명령이 내려가는걸 막자 ~~~
            m_SeqTransfer.SeqAbort();
            m_AxisMaster.GetDevAxis().SeqAbort();
            m_AxisSlave.GetDevAxis().SeqAbort();
        }
        #endregion

        #region Move Command
        public int Home()
        {
            if (!Initialized) return m_ALM_NotDefine.ID;
            int rv = -1;
            rv = m_SeqTransfer.Home();
            return rv;
        }
        public int Move(ushort point, ushort prop)
        {
            if (!Initialized) return m_ALM_NotDefine.ID;
            int rv = -1;
            rv = m_SeqTransfer.Move(point, prop);
            return rv;
        }
        public int Move(double pos, VelSet set, bool safty_use)
        {
            if (!Initialized) return m_ALM_NotDefine.ID;
            int rv = -1;
            rv = m_SeqTransfer.Move(pos, set, safty_use);
            return rv;
        }
        /// <summary>
        /// useBcrScan, 최종 목표 위치에 도착할때만 사용으로 하자
        ///  JCS, MTL, SPL, AutoDoor 등 멈추는 위치는 정확하지 않아도 된다.
        /// 목표위치에 근접했을 경우 AbsMove를 날려 정확도를 높이면 된다.
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="useBcrScan"></param>
        /// <returns></returns>
        public int SetCommand(List<Data.Process.Path> run_paths)
        {
            int rv = 0;
            try
            {
                if (run_paths.Count == 0)
                {
                    SequenceLog.WriteLog("[DevTransfer]", string.Format("run_paths Count is 0"));
                    return m_ALM_PathCalculateBcrAbnormal.ID;
                }
                string msg = "SetCommand \r\n";
                List<Data.Process.Path> paths = ObjectCopier.Clone(run_paths); // paths 값이 바뀔수 있어 복사해서 사용하자 ~

                m_SeqTransfer.SeqAbort();
                _Axis axis = m_AxisMaster.GetDevAxis().GetAxis();
                m_MotionSensor = ObjectCopier.Clone(axis.MotionSensorPara); // default 값을 가져온다.
                double current_velocity = m_AxisMaster.GetDevAxis().GetCurVelocity();
                double current_position = m_AxisMaster.GetDevAxis().GetCurPosition();

                Sineva.VHL.Data.Process.Path lastPathNext = ProcessDataHandler.Instance.CurTransferCommand.GetToPath(paths.Last());
                DataItem_Node toNode = paths.Last().IsToNode();
                bool bcrScanUse = paths.Last().BcrScanUse;

                // Moving Profile Setting
                bool isBranchJunction = false;
                if (paths.Count > 0) isBranchJunction = paths[0].IsBranchJunctionStraight();// 분기 합류 출발 일때
                if (paths.Count > 1) isBranchJunction = paths[1].IsBranchJunctionStraight();// 직선 다음 분기 합류일때

                int index = 0;
                m_MotionProfiles.Clear();
                List<MotionProfile> tempProfiles = new List<MotionProfile>();
                bool SameBCRSelectBeforeLastPath = false;
                foreach (Data.Process.Path path in paths)
                {
                    double runDistance = path.RunDistance;
                    double runVelocity = path.Velocity;
                    double runAcc = path.Acceleration;
                    double runDec = path.Deceleration;
                    double runJerk = path.Jerk;
                    // 첫번째 구간이 짧은 경우 Short Distance 발생 . 100%
                    // 첫번째 구간이 짧은 경우는 Next link에서 붙여서 명령을 만들자! 
                    // path의 tonode가 jcs이고 permit을 받은 경우
                    Sineva.VHL.Data.Process.Path beforePath = index > 0 ? paths[index - 1] : null;
                    Sineva.VHL.Data.Process.Path nextPath = index < paths.Count - 1 ? paths[index + 1] : null;

                    // 첫번째 구간의 Distance 조정이 필요함.
                    if (index == 0)
                    {
                        bool loop_case = ProcessDataHandler.Instance.CurTransferCommand.IsPathLoop;
                        double curLeftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double curRightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                        DataItem_Node curNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsNearNode(curLeftBCR, curRightBCR); // 무조건 ToNode를 선택할 수 없다.
                        /// SPL도착 했는데 Finished 않돼었네... Node 위치 도착이라 그럴수 있음.
                        double margin = 0.0f;//사람이 대충 밀고 시작 할수 있는데 Node 언저리라 마진이 좀 필요할 것 같은뎀..
                        if (path.FromNodeID == curNode.NodeID && (curNode.Type == NodeType.Lifter || curNode.Type == NodeType.MTL)) margin = 25.0f;

                        // Loop Case에는 Target 위치가 Current Position보다 적은 위치에 있을 수 있다.
                        double targetLeftBCR = path.LeftBCREnd;
                        double targetRightBCR = path.RightBCREnd;
                        if (paths.Count == 1) // loop case는 무조건 End Position으로 계산 하자~~~
                        {
                            targetLeftBCR = path.LeftBCRTarget;
                            targetRightBCR = path.RightBCRTarget;
                        }
                        double left_distance = Math.Round(targetLeftBCR - curLeftBCR, 1);
                        double right_distance = Math.Round(targetRightBCR - curRightBCR, 1);
                        double distance = path.BcrDirection == enBcrCheckDirection.Right ? right_distance : left_distance;
                        if (distance > -1.0f && (left_distance <= path.Distance + margin || right_distance < path.Distance + margin)) runDistance = distance;
                        else
                        {
                            msg += string.Format("Abnormal : path={0}, curLeftBCR={1}, curRightBCR={2}, targetLeftBCR = {3}, targetRightBCR={4}, left_distance={5}, right_distance={6} \r\n",
                                path.ToString(), curLeftBCR, curRightBCR, targetLeftBCR, targetRightBCR, left_distance, right_distance);
                            rv = m_ALM_PathCalculateBcrAbnormal.ID;
                            SequenceDeviceLog.WriteLog(DevName, msg);
                            return rv; //여기서 Return을 하지많으면 알람 발생한 상태로 움직이더라..
                        }

                        // 다음 구간이 있는 경우에는 다음 구간의 속도를 기본으로 사용하자!
                        // 어차피 다음 구간 속도로 떨어지는 시점에 SetCommand를 했을 거다.
                        // 다음 구간이 없는 경우는 자기 구간속도로 사용하자
                        if (nextPath != null)
                        {
                            if (path.Type == LinkType.Straight) runVelocity = nextPath.Velocity;
                            else runVelocity = path.Velocity;

                            if (nextPath.BcrScanUse)
                            {
                                //속도가 너무 늦다....
                                double limit_vel = nextPath.RunDistance;
                                if (limit_vel < 100.0f) limit_vel = 100.0f;
                                if (current_velocity < 10.0f) runVelocity = Math.Min(path.Velocity, limit_vel);
                                else runVelocity = Math.Min(current_velocity / 2.0f, limit_vel); // 이동 중 속도 계산
                                if (runDistance > SetupManager.Instance.SetupWheel.BCRScanPathDistance)
                                {
                                    double vel = runDistance / 3;
                                    if (vel > path.Velocity) vel = path.Velocity;
                                    runVelocity = vel;
                                }
                            }
                            else
                            {
                                double set_vel = runDistance / 2;
                                if (set_vel > path.Velocity) set_vel = path.Velocity;
                                else if (set_vel < runVelocity) set_vel = runVelocity;

                                if (Math.Abs(set_vel - current_velocity) > runDistance) set_vel = runDistance; // 속도 변화가 거리값보다 큰 경우
                                if (current_velocity < 10.0f && set_vel > runDistance) set_vel = 0.9f * runDistance; // 멈추었다 가는 경우 거리값보다 작게 설정
                                runVelocity = set_vel;
                            }
                            if (runVelocity > path.Velocity) runVelocity = path.Velocity; //limit 제한
                        }
                    }
                    // 마지막 구간은 Velocity < Distance 조건으로 만들자...
                    //if (index == paths.Count - 1 && runVelocity > runDistance)
					if (index == paths.Count - 1)
                    {
                        if (beforePath != null && beforePath.BcrDirection == path.BcrDirection && !beforePath.IsCorner()) SameBCRSelectBeforeLastPath = true;

                        if (runVelocity > runDistance)
                        {
                            if (paths.Count > 1)
                            {
                                if (runVelocity > paths[index - 1].Velocity) runVelocity = paths[index - 1].Velocity;
                                else runVelocity = runDistance;
                            }
                            else runVelocity = runDistance;
                        }
                    }
                    // 분기/합류 출발인 경우. 가속하면서 저전압 알람이 발생한다. 
                    if (isBranchJunction && index > 0)
                    {
                        if (path.IsBranchJunctionStraight())
                        {
                            //if (current_velocity < runVelocity) // 증속일 경우
                            bool accCase = current_velocity < runVelocity;
                            accCase &= Math.Abs(current_velocity - runVelocity) > runVelocity * 0.05f;
                            if (accCase)
                            {
                                if (current_velocity > 1000.0f) runVelocity = current_velocity; // 현재속도 유지
                                else if (runVelocity > 1000.0f) runVelocity = 1000.0f; // 설정속도 제한. 현재속도가 1000 이하로 떨어진 경우
                                if (tempProfiles.Count > index - 1 && path.Velocity != tempProfiles[index - 1].Velocity) //첫번째 구간 속도를 이어서 가기 위한 조건. 
                                {
                                    if (runVelocity > path.Distance) runVelocity = path.Distance; //거리 제한
                                }
                                if (runAcc > runVelocity) runAcc = runVelocity;
                                if (runDec > runVelocity) runDec = runVelocity;
                                if (runJerk > 2 * runVelocity) runJerk = 2 * runVelocity;
                            }
                        }
                        //index>1일때 정션브런치 아닌경우isBranchJunction = false 만들자!
                        else if (index > 1)
                        {
                            isBranchJunction = false;
                        }
                    }
                    //////////////////////////////////////////////////////////////////////

                    //////////////////////////////////////////////////////////////////////
                    /// FromExtensionDistance, ToExtensionDistance 계산
                    /// 자신 기준으로 +면 길어지고 -면 줄어들고
                    if (path.ToExtensionDistance != 0 && nextPath != null)
                    {
                        if (nextPath.RunDistance > 3 * Math.Abs(path.ToExtensionDistance))
                        {
                            runDistance += path.ToExtensionDistance;
                            nextPath.RunDistance -= path.ToExtensionDistance;
                        }
                    }
                    if (path.FromExtensionDistance != 0 && beforePath != null)
                    {
                        if (beforePath.RunDistance > 3 * Math.Abs(path.FromExtensionDistance))
                        {
                            runDistance += path.FromExtensionDistance;
                            beforePath.RunDistance -= path.FromExtensionDistance;
                        }
                    }
                    //////////////////////////////////////////////////////////////////////

                    MotionProfile profile = new MotionProfile();
                    profile.Distance = runDistance;
                    profile.Velocity = runVelocity;
                    profile.Acceleration = runAcc;
                    profile.Deceleration = runDec;
                    profile.Jerk = runJerk;
                    profile.VelocityLimitFlag = path.IsCorner() ? (byte)1 : (byte)0;
                    tempProfiles.Add(profile);
                    index++;
                }
                // 2개구간으로 나누자 !
                bool last_two_step = false;
                if (tempProfiles.Count > 0)
                {
                    if (tempProfiles.Count == 1)
                    {
                        double set_vel = tempProfiles[0].Velocity;
                        double limit_vel = tempProfiles[0].Distance / 3.0f;
                        if (limit_vel < SetupManager.Instance.SetupWheel.BCRScanPathVelocity)
                            limit_vel = SetupManager.Instance.SetupWheel.BCRScanPathVelocity;
                        if (set_vel > limit_vel) tempProfiles[0].Velocity = limit_vel;
                    }

                    double scan_distance = SetupManager.Instance.SetupWheel.BCRScanPathDistance;
                    double scan_velocity = SetupManager.Instance.SetupWheel.BCRScanPathVelocity;
                    if (bcrScanUse == false && lastPathNext != null)
                    {
                        if (lastPathNext.IsCorner() && toNode.JCSCheck > 0)
                        {
                            scan_distance = lastPathNext.Velocity;
                            scan_velocity = lastPathNext.Velocity;
                        }
                    }

                    bool two_step = SetupManager.Instance.SetupOperation.BcrScanTwoStepUse == Use.Use;
                    two_step |= bcrScanUse == false; // BCR Scan이 아닌 경우에는 속도를 줄여서 멈춘후 이동하자....
                    two_step |= bcrScanUse && tempProfiles.Last().Distance < scan_distance;
                    if (scan_distance > 0 && two_step)
                    {
                        // 마지막 구간을 2개로 나누자 ! 만일 scan 설정 거리의 2배보다 적은 경우는 마지막 path를 Scan 영역으로 활용 
                        double d1 = tempProfiles.Last().Distance;
                        if (d1 > 2.0f * scan_distance)
                        {
                            double scan_margin = scan_distance + 100.0f;
                            // 첫번째 구간
                            tempProfiles.Last().Distance = d1 - scan_margin;
                            // 두번째 구간(BCR Scan)
                            MotionProfile scan_profile = new MotionProfile();
                            scan_profile.Distance = scan_margin;
                            // 속도
                            scan_velocity = Math.Min(tempProfiles.Last().Velocity, scan_velocity);
                            scan_profile.Velocity = scan_velocity;
                            // 가감속
                            if (bcrScanUse)
                            {
                                scan_profile.Acceleration = m_MotionSensor.SensorScanAcceleration;
                                scan_profile.Deceleration = m_MotionSensor.SensorScanDeceleration;
                                scan_profile.Jerk = m_MotionSensor.SensorScanJerk;
                            }
                            else
                            {
                                scan_profile.Acceleration = tempProfiles.Last().Acceleration;
                                scan_profile.Deceleration = tempProfiles.Last().Deceleration;
                                scan_profile.Jerk = tempProfiles.Last().Jerk;
                            }
                            scan_profile.VelocityLimitFlag = 0;
                            tempProfiles.Add(scan_profile);
                            last_two_step = true;
                        }
                        else
                        {
                            // 둘로 나누지 못하는 경우 속도 제한
                            double limit_vel = d1;
                            scan_velocity = Math.Min(scan_velocity, limit_vel);
                            tempProfiles.Last().Velocity = scan_velocity;
                            //가감속 제한..
                            double limit_acc = tempProfiles.Last().Acceleration;
                            if (scan_velocity * 2 < limit_acc)
                            {
                                tempProfiles.Last().Acceleration = scan_velocity * 2; //최대 가속도를 속도 두배로..
                                tempProfiles.Last().Deceleration = scan_velocity * 2; //최대 가속도를 속도 두배로..
                                tempProfiles.Last().Jerk = scan_velocity * 4; //최대 가속도를 속도 두배로..
                            }
                        }
                    }

                    {
                        // Velocity/ACC/DEC/JERK가 같은 경우 Profile을 Merging하자~~ Short Distance 발생 확율을 줄이자~~
                        double beforeVelocity = 0.0f;
                        double beforeAcceleration = 0.0f;
                        double beforeDeceleration = 0.0f;
                        double beforeJerk = 0.0f;
                        bool LastPathShortBCR = false;

                        msg += string.Format("[Motion Profiles]\r\n");
                        int tempProfileLastIndex = tempProfiles.Count != 0 ? tempProfiles.Count - 1 : 0;
                        for (int i = 0; i < tempProfiles.Count; i++)
                        {
                            bool before_same = false;

                            if (i > 0)
                            {
                                if(Math.Abs(tempProfiles[i].Velocity - beforeVelocity) < 1.0f)
                                {
                                    before_same = true;
                                    /*
                                    before_same = Math.Abs(tempProfiles[i].Velocity - beforeVelocity) < 1.0f;
                                    //before_same &= Math.Abs(tempProfiles[i].Acceleration - beforeAcceleration) < 1.0f;
                                    //before_same &= Math.Abs(tempProfiles[i].Deceleration - beforeDeceleration) < 1.0f;
                                    //before_same &= Math.Abs(tempProfiles[i].Jerk - beforeJerk) < 1.0f;
                                    */
                                }
                                else
                                {
                                    if(i == tempProfileLastIndex && paths.Count > 1)
                                    {
                                        int LastpathsIndex = paths.Count - 1;
                                        LastPathShortBCR = SameBCRSelectBeforeLastPath;
                                        LastPathShortBCR &= paths.Last().BcrDirection == enBcrCheckDirection.Right ?
                                                            Math.Abs(paths[LastpathsIndex - 1].RightBCREnd - paths.Last().RightBCRStart) < 10.0f : Math.Abs(paths[LastpathsIndex - 1].LeftBCREnd - paths.Last().LeftBCRStart) < 10.0f;
                                        LastPathShortBCR &= tempProfiles[i].Distance < 200.0f;
                                    }
                                }
                            }
                            if (before_same)
                            {
                                m_MotionProfiles.Last().Distance += tempProfiles[i].Distance;
                                m_MotionProfiles.Last().VelocityLimitFlag = Math.Max(tempProfiles[i].VelocityLimitFlag, m_MotionProfiles.Last().VelocityLimitFlag);
                                m_MotionProfiles.Last().Acceleration = Math.Min(tempProfiles[i].Acceleration, m_MotionProfiles.Last().Acceleration);
                                m_MotionProfiles.Last().Deceleration = Math.Min(tempProfiles[i].Deceleration, m_MotionProfiles.Last().Deceleration);
                                m_MotionProfiles.Last().Jerk = Math.Min(tempProfiles[i].Acceleration, m_MotionProfiles.Last().Jerk);
                            }
                            else if(LastPathShortBCR)
                            {
                                m_MotionProfiles.Last().Distance += tempProfiles[i].Distance;
                            }
                            else
                            {
                                MotionProfile profile = new MotionProfile
                                {
                                    Distance = tempProfiles[i].Distance,
                                    Velocity = tempProfiles[i].Velocity,
                                    Acceleration = tempProfiles[i].Acceleration,
                                    Deceleration = tempProfiles[i].Deceleration,
                                    Jerk = tempProfiles[i].Jerk,
                                    VelocityLimitFlag = tempProfiles[i].VelocityLimitFlag
                                };
                                m_MotionProfiles.Add(profile);
                            }
                            beforeVelocity = m_MotionProfiles.Last().Velocity;
                            beforeAcceleration = m_MotionProfiles.Last().Acceleration;
                            beforeDeceleration = m_MotionProfiles.Last().Deceleration;
                            beforeJerk = m_MotionProfiles.Last().Jerk;

                            msg += $"   {i}, {tempProfiles[i]} \r\n";
                        }
                    }

                    #region ShortBlock 삭제
                    // 가감속은 속도의 2배까지, Jert는 속도의 4배까지 제한하자 !
                    // Velocity Short Flag Check
                    //for (int i = 0; i < m_MotionProfiles.Count; i++)
                    //{
                    //    Byte shortFlag = 0;
                    //    float s = (float)m_MotionProfiles[i].Distance;
                    //    float v = (float)m_MotionProfiles[i].Velocity;
                    //    float a = (float)m_MotionProfiles[i].Acceleration;
                    //    //if (a > 2 * v) { a = 2 * v; m_MotionProfiles[i].Acceleration = a; }
                    //    float d = (float)m_MotionProfiles[i].Deceleration;
                    //    //if (d > 2 * v) { d = 2 * v; m_MotionProfiles[i].Deceleration = d; }
                    //    float j = (float)m_MotionProfiles[i].Jerk;
                    //    //if (j > 4 * v) { j = 4 * v; m_MotionProfiles[i].Jerk = j; }

                    //    float v0 = v;
                    //    if (i == 0) v0 = (float)current_velocity;
                    //    else if (i > 0) v0 = (float)m_MotionProfiles[i - 1].Velocity; // Low_Blend Using
                    //    if (v0 < 0.0f) v0 = 0.0f; // 음수는 없어야 한다.

                    //    float v1 = i < m_MotionProfiles.Count - 1 ? (float)m_MotionProfiles[i + 1].Velocity : v;
                    //    bool dont_change_vel = Math.Abs(v0 - v) < 0.3f * s;
                    //    dont_change_vel |= Math.Abs(v1 - v) < 0.3f * s;
                    //    dont_change_vel |= (v0 == 0.0f || v1 == 0.0f); // 음수는 없어야 한다.
                    //    if (v > 2 * s && !dont_change_vel)
                    //    {
                    //        if (m_MotionProfiles[i].VelocityLimitFlag == 1) // 곡선인 경우 CurveSpeed를 설정
                    //            v = (float)SetupManager.Instance.SetupWheel.CurveSpeed; //0.5f * s; // 거리가 짧고 속도가 큰 경우
                    //        else v = (float)(v0 + v1) / 2; //중간속도를 선택하자~~
                    //    }
                    //    if (v < 10.0f) v = 10.0f; // 최소 속도
                    //    if (v > v1)
                    //    {
                    //        MXP.MXP_FUNCTION_STATUS_RESULT result = MxpManager.Instance.ReadVelocityProfile((uint)axis.NodeId, s, v0, v, v1, a, d, j, ref shortFlag);
                    //        if (result == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR && shortFlag > 0)
                    //        {
                    //            string short_msg = string.Format("Short Distance #1 : {0}:{1},{2},{3},{4},{5},{6},{7}", i, s, v0, v, v1, a, d, j);
                    //            SequenceDeviceLog.WriteLog(DevName, short_msg);

                    //            // 최대값으로 운영해 보자
                    //            if (v > s) v = 0.9f * s; //속도는 거리보다 작게 설정하자~~
                    //            if (v > v0 && v > v1) //이때는 속도를 줄이자...급하게 올라갔다 급하게 내려오니까 저전압 알람이 발생하네...
                    //            {
                    //                v -= 500;
                    //                if (v < v0) v = v0;
                    //                if (v < v1) v = v1;
                    //            }
                    //            else
                    //            {
                    //                a = 2500.0f;
                    //                d = 3000.0f;
                    //                j = 4000.0f;
                    //            }
                    //            MXP.MXP_FUNCTION_STATUS_RESULT result2 = MxpManager.Instance.ReadVelocityProfile((uint)axis.NodeId, s, v0, v, v1, a, d, j, ref shortFlag);
                    //            if (result2 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR && shortFlag > 0)
                    //            {
                    //                short_msg = string.Format("Short Distance #2 : {0}:{1},{2},{3},{4},{5},{6},{7}", i, s, v0, v, v1, a, d, j);
                    //                SequenceDeviceLog.WriteLog(DevName, short_msg);

                    //                double curveVel = SetupManager.Instance.SetupWheel.CurveSpeed;
                    //                if (s > curveVel)
                    //                {
                    //                    float min_vel = Math.Min(v0, v);
                    //                    min_vel = Math.Min(min_vel, v1);
                    //                    if (v > 1000.0f)
                    //                    {
                    //                        float vel = (float)v;
                    //                        for (int n = 0; n < 10; n++)
                    //                        {
                    //                            vel -= 500.0f;
                    //                            MXP.MXP_FUNCTION_STATUS_RESULT result1 = MxpManager.Instance.ReadVelocityProfile((uint)axis.NodeId, s, v0, vel, v1, a, d, j, ref shortFlag);
                    //                            if (result1 == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                    //                            {
                    //                                if (shortFlag == 0) break;
                    //                            }
                    //                            if (vel < min_vel) break;
                    //                        }
                    //                        if (vel < min_vel) v = min_vel;
                    //                        else v = vel;
                    //                    }
                    //                    //else
                    //                    //    v = min_vel;
                    //                }
                    //                else if (s > 50.0f) v = 0.9f * s;
                    //                else v = 50.0f; // 최소속도
                    //            }
                    //        }
                    //    }
                    //    m_MotionProfiles[i].Velocity = v;
                    //    m_MotionProfiles[i].Acceleration = a;
                    //    m_MotionProfiles[i].Deceleration = d;
                    //    m_MotionProfiles[i].Jerk = j;
                    //}
                    #endregion

                    // Moving Sensor Setting
                    bool right_bcr_using = paths.Last().BcrDirection == enBcrCheckDirection.Right;
                    double last_path_bcr_start = right_bcr_using ? paths.Last().RightBCRStart : paths.Last().LeftBCRStart;
                    double last_path_run_distance = paths.Last().RunDistance;
                    m_MotionSensor.SlaveNo = right_bcr_using ? (uint)axis.RightBcrNodeId : (uint)axis.LeftBcrNodeId;
                    m_MotionSensor.SensorUse = bcrScanUse ? (uint)1 : (uint)0;

                    if (m_MotionSensor.SensorUse == 1)
                    {
                        m_MotionSensor.SensorPositionSetRange = (float)SetupManager.Instance.SetupWheel.InRange;
                        if (toNode != null)
                            if (toNode.Type == NodeType.MTL || toNode.Type == NodeType.Lifter) m_MotionSensor.SensorPositionSetRange = 5.0f;
                    }

                    // 마지막 구간을 둘로 나눌수 있다. Relative+BCR혼용, BCR혼용 거리, 최대 2 * BCRScanPathDistance(setup 설정)
                    //double bcr_offset = right_bcr_using ? axis.RightBCROffset : axis.LeftBCROffset;
                    double target_bcr = last_path_bcr_start + last_path_run_distance;// + bcr_offset;
                    m_MotionSensor.SensorTargetValue = (float)(target_bcr);
                    if (last_two_step)
                    {
                        m_MotionSensor.SensorScanDistance = (float)(1.0f * scan_distance);
                    }
                    else
                    {
                        if (SetupManager.Instance.SetupOperation.BcrScanTwoStepUse == Use.Use)
                        {
                            m_MotionSensor.SensorScanDistance = (float)(0.9f * m_MotionProfiles.Last().Distance);
                        }
                        else
                        {
                            double vel = m_MotionProfiles.Last().Velocity;
                            double dec = m_MotionProfiles.Last().Deceleration;
                            double dt0 = vel / dec;
                            double s0 = dec * dt0 * dt0; //감속거리 * 2배
                            double s1 = m_MotionProfiles.Last().Distance;
                            if (s0 < s1)
                            {
                                if (s0 > scan_distance) m_MotionSensor.SensorScanDistance = (float)s0;
                                else m_MotionSensor.SensorScanDistance = (float)s1;
                            }
                            else
                                m_MotionSensor.SensorScanDistance = (float)s1;
                            
                            //m_MotionSensor.SensorScanDistance = s1 < scan_distance ? (float)(s1) : (float)Math.Min(s0, s1);
                        }
                    }
                    m_MotionSensor.SensorScanVelocity = (float)m_MotionProfiles.Last().Velocity; // BCR Scan Area에 들어 왔을때 현 속도로 재 계산 필요함.
                                                                                                 // acc, dec, jerk는 axis sensor setting값을 유지 하자!
                    ProcessDataHandler.Instance.CurTransferCommand.IsSetExternalEncorder = m_MotionSensor.SensorUse == 1 ? true : false;
                    ProcessDataHandler.Instance.CurTransferCommand.BcrScanMotion = ObjectCopier.Clone(m_MotionProfiles.Last()); // 마지막 동작
                    ProcessDataHandler.Instance.CurTransferCommand.TargetLeftBcrPosition = paths.Last().LeftBCRStart + last_path_run_distance;// + axis.LeftBCROffset;
                    ProcessDataHandler.Instance.CurTransferCommand.TargetRightBcrPosition = paths.Last().RightBCRStart + last_path_run_distance;// + axis.RightBCROffset;
                    ProcessDataHandler.Instance.CurTransferCommand.TargetBcrDistance = paths.Last().TotalDistance;
                    ProcessDataHandler.Instance.CurTransferCommand.TargetMotorPosition = current_position + paths.Last().TotalDistance;
                    ProcessDataHandler.Instance.CurTransferCommand.StopCheckDistance = 1.5 * m_MotionSensor.SensorScanDistance; // virtual bcr로 비교하니까 link가 짧아도 비교 가능.
                    ProcessDataHandler.Instance.CurTransferCommand.IsStopCheckVelocity = true;
                    ProcessDataHandler.Instance.CurTransferCommand.StopCheckVelocity = m_MotionSensor.SensorScanVelocity;
                    if (lastPathNext != null)
                    {
                        if (toNode.JCSCheck > 0 && lastPathNext.IsCorner() == false && !bcrScanUse)
                        {
                            double vel = paths.Last().Velocity;
                            double dec = paths.Last().Deceleration;
                            double dt0 = vel / dec;
                            double s0 = dec * dt0 * dt0 + 2 * m_MotionProfiles.Last().Distance; //감속거리 * 2배
                            double s1 = toNode.JCSCheck;
                            double s2 = paths.Last().RunDistance;
                            double max = Math.Max(s0, s1);
                            double min = Math.Min(max, s2);
                            ProcessDataHandler.Instance.CurTransferCommand.StopCheckDistance = min; //좀더 멀리에서 Permit 확인을 하자!

                            bool branch_straight = lastPathNext.Type == LinkType.LeftBranchStraight;
                            branch_straight |= lastPathNext.Type == LinkType.RightBranchStraight;
                            branch_straight |= lastPathNext.Type == LinkType.LeftJunctionStraight;
                            branch_straight |= lastPathNext.Type == LinkType.RightJunctionStraight;
                            if (toNode != null && toNode.JCSCheck > 0 && branch_straight)
                            {
                                // 속도가 이어져 가는 경우는 속도는 확인 하지 말자 ~~~ check 거리는 크게하자
                                bool noCheckVelocity = lastPathNext.Velocity > paths.Last().Velocity - 100.0f; //다음이 큰 경우는 속도 확인하지 말고 진행 가능
                                if (noCheckVelocity)
                                {
                                    ProcessDataHandler.Instance.CurTransferCommand.IsStopCheckVelocity = false; // 속도 범위 확인은 하지 말자 !
                                }
                            }
                        }
                        ProcessDataHandler.Instance.CurTransferCommand.StopCheckVelocity = lastPathNext.Velocity;
                    }

                    UpdateTargetPosition(ProcessDataHandler.Instance.CurTransferCommand.TargetMotorPosition); // TargetPos를 먼저 Update 해야 SCAN 조건을 막을 수 있다.
                    m_MotionStep = 0;
                    m_MotionStartPosition = current_position;
                    m_AxisMaster.GetDevAxis().SetSequenceMoveCommand(true); // Sequence Move Command 설정이 되었다... ABS Move 할 필요없다.
                    m_SetMotionCommand = true;

                    msg += string.Format("[BCR Scan Information]\r\nBcrDirection={0}\r\n", paths.Last().BcrDirection);
                    msg += string.Format("SlaveNo={0}\r\n", m_MotionSensor.SlaveNo);
                    msg += string.Format("SensorUse={0}\r\n", m_MotionSensor.SensorUse);
                    msg += string.Format("SensorTargetValue={0:F2}\r\n", m_MotionSensor.SensorTargetValue);
                    msg += string.Format("SensorScanDistance={0:F2}\r\n", m_MotionSensor.SensorScanDistance);
                    msg += string.Format("SensorScanVelocity={0:F2}\r\n", m_MotionSensor.SensorScanVelocity);
                    msg += string.Format("TargetLeftBcrPosition={0:F2}\r\n", ProcessDataHandler.Instance.CurTransferCommand.TargetLeftBcrPosition);
                    msg += string.Format("TargetRightBcrPosition={0:F2}\r\n", ProcessDataHandler.Instance.CurTransferCommand.TargetRightBcrPosition);
                    msg += string.Format("TargetBcrDistance={0:F2}\r\n", ProcessDataHandler.Instance.CurTransferCommand.TargetBcrDistance);
                    msg += string.Format("TargetMotorPosition={0:F2}\r\n", ProcessDataHandler.Instance.CurTransferCommand.TargetMotorPosition);
                    msg += string.Format("StopCheckDistance={0:F2}\r\n", ProcessDataHandler.Instance.CurTransferCommand.StopCheckDistance);
                    msg += string.Format("StopCheckVelocity={0:F2}\r\n", ProcessDataHandler.Instance.CurTransferCommand.StopCheckVelocity);
                    SequenceDeviceLog.WriteLog(DevName, msg);
                }
                else
                {
                    rv = m_ALM_PathCalculateBcrAbnormal.ID;
                    msg = string.Format("NG, m_MotionProfiles Count = 0 !");
                    SequenceDeviceLog.WriteLog(DevName, msg);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public int SequenceMove()
        {
            int rv = -1;
            rv = m_SeqTransfer.Do();
            return rv;
        }
        public void UpdateTargetPosition(double targetPos)
        {
            m_AxisMaster.GetDevAxis().GetAxis().TargetPos = targetPos;
        }
        public double GetInRange()
        {
            double rv = 0.0f;
            if (ProcessDataHandler.Instance.CurTransferCommand.IsSetExternalEncorder)
            {
                if (m_MotionSensor.SensorPositionSetRange == 0.0f)
                    rv = SetupManager.Instance.SetupWheel.InRange;
                else rv = m_MotionSensor.SensorPositionSetRange;
            }
            else
            {
                rv = 50.0f;
            }
            return rv;
        }
        public double GetSensorScanVelocity()
        {
            return m_MotionSensor.SensorScanVelocity;
        }
        public bool IsLastMovePlusDirection()
        {
            return m_PlusMoved;
        }
        /// <summary>
        /// Get WheelMaster or WheelSlave working distance
        /// </summary>
        /// <param name="axis">0:Master, 1:Slave</param>
        /// <returns></returns>
        public double GetWorkingDistance(int axis)
        {
            double rv = 0.0f;
            if (axis == 0) rv = m_WheelMasterWorkingDistance;
            else if (axis == 1) rv = m_WheelSlaveWorkingDistance;
            return rv;
        }
        #endregion

        #region valid check
        public bool IsValidCheckTag(DevAxisTag axis)
        {
            bool rv = true;
            if (rv) rv &= axis != null ? true : false;
            if (rv) rv &= axis.IsValid ? true : false;
            if (rv) rv &= axis.GetDevAxis() != null ? true : false;
            return rv;
        }
        #endregion
        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevTransfer m_Device = null;
            private _DevAxis m_Master = null;
            private _DevAxis m_Slave = null;
            #endregion

            public SeqMonitor(DevTransfer device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;

                if (m_Device.IsValidCheckTag(m_Device.AxisMaster)) m_Master = m_Device.AxisMaster.GetDevAxis();
                if (m_Device.IsValidCheckTag(m_Device.AxisSlave)) m_Slave = m_Device.AxisSlave.GetDevAxis();

                TaskDeviceControlHighSpeed.Instance.RegSeq(this);
            }

            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Master != null && m_Slave != null)
                            {
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            // Busy Check
                            enAxisInFlag master_status = m_Master.GetAxis().AxisStatus;
                            enAxisInFlag slave_status = m_Slave.GetAxis().AxisStatus;
                            bool master_busy = true;
                            master_busy &= (master_status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                            master_busy &= (master_status & enAxisInFlag.HEnd) == enAxisInFlag.HEnd ? true : false;
                            master_busy &= (master_status & enAxisInFlag.InPos) != enAxisInFlag.InPos ? true : false;

                            //bool slave_busy = true;
                            //slave_busy &= (slave_status & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? true : false;
                            //slave_busy &= (slave_status & enAxisInFlag.InPos) != enAxisInFlag.InPos ? true : false;
                            if (master_busy /*|| slave_busy*/) GV.WheelBusy = true;
                            else GV.WheelBusy = false;

                            ProcessDataHandler.Instance.CurVehicleStatus.MasterWheelVelocity = m_Master.GetCurVelocity();
                            ProcessDataHandler.Instance.CurVehicleStatus.MasterWheelCMDVelocity = m_Master.GetCommandVelocity();

                            // Normalize Current Position
                            m_Device.MotionRunPosition = m_Master.GetAxis().CurPos - m_Device.MotionStartPosition;

                            double sum = 0;
                            int step = 0;
                            foreach (MotionProfile motion in m_Device.m_MotionProfiles)
                            {
                                sum += motion.Distance;
                                if (m_Device.MotionRunPosition > sum) step++;
                            }
                            m_Device.MotionStep = step;
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
        }

        private class SeqTransfer : XSeqFunc
        {
            #region Fields
            private const int m_MAX_Axis = 2;
            private DevTransfer m_Device = null;
            private List<_DevAxis> m_DevAxes = new List<_DevAxis>();

            private _DevAxis m_Master = null;
            private _DevAxis m_Slave = null;

            private List<_DevAxis> m_TargetDevAxes = new List<_DevAxis>();
            private List<_DevAxis> m_UsedDevAxes = new List<_DevAxis>();

            private List<ushort> m_TargetPoint = null;
            private List<ushort> m_TargetProp = null;
            private List<double> m_TargetPos = null;
            private List<VelSet> m_TargetVelSet = null;

            private bool[] m_CommandComp = null;
            private int m_HomeSeqNo = 0;
            private int m_MoveSeqNo = 0;

            private int m_RetryCount = 0;
            private int m_CmdErrorRetryCount = 0;
            private double m_TargetPosition = 0.0f;
            private VelSet m_SearchingVelSet = new VelSet();
            private bool m_InrangeCheckOK = true;
            private bool m_BusyTimeOver = false; // Break가 잡히는 순간 명령이 내려갔는데 실행을 못하네...retry 하자!
            #endregion

            #region Constructor
            public SeqTransfer(DevTransfer device)
            {
                this.SeqName = $"SeqTransfer{device.MyName}";
                m_Device = device;
                if (m_Device.AxisMaster.IsValid) m_Master = m_Device.AxisMaster.GetDevAxis();
                if (m_Device.AxisSlave.IsValid) m_Slave = m_Device.AxisSlave.GetDevAxis();

                //Find Used Axis
                m_DevAxes.Clear();
                if (m_Device.AxisMaster.IsValid) m_DevAxes.Add(m_Master);
                if (m_Device.AxisSlave.IsValid) m_DevAxes.Add(m_Slave);

                m_CommandComp = new bool[m_DevAxes.Count];
                m_TargetPoint = new List<ushort>();
                m_TargetProp = new List<ushort>();
                m_TargetPos = new List<double>();
                m_TargetVelSet = new List<VelSet>();
            }

            public override void SeqAbort()
            {
                m_HomeSeqNo = 0;
                m_MoveSeqNo = 0;
                this.InitSeq();
            }
            #endregion

            #region Methods - override
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;
                int rv1 = -1;

                if (!m_Device.Initialized) return rv;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (GV.TransferMoveEnable && m_Device.m_SetMotionCommand)
                            {
                                m_Device.m_SetMotionCommand = false; //이중으로 명령이 내려가는걸 막자 ~~~

                                // 잘못된 명령이 날아가는 경우 Hold Bit로 막고 있다...
                                // 그래서 출발할때는 Reset을 하고 시작해야 겠다...
                                m_Master.SetHolding(false);

                                // Sequence 명령을 만들어 날리자 !
                                // Target Position Check
                                // Safty Check
                                SequenceCommand command = m_Master.GetAxis().SequenceCommand;
                                command.MoveMethod = enMotionMoveMethod.Relative;
                                command.BufferMode = enMotionBufferMode.BlendingLow;
                                command.MotionProfiles = ObjectCopier.Clone(m_Device.m_MotionProfiles);
                                command.PositionSensorInfo = ObjectCopier.Clone(m_Device.m_MotionSensor);

                                enAxisResult rv2 = m_Master.GetServoUnit().SequenceMoveAxisStart(m_Master.GetAxis(), command);
                                SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Sequence Move Start, Result : {2}", m_Master.GetServoUnit().ServoName, m_Master.GetName(), rv2));
                                SequenceDeviceLog.WriteLog(string.Format("Set Command :\r\n{0}", command.GetCommandLog()));
                                StartTicks = XFunc.GetTickCount();

                                m_RetryCount = 0;
                                m_CmdErrorRetryCount = 0;
                                m_InrangeCheckOK = false;
                                // 멈춘 상태에서 출발하는 경우
                                m_BusyTimeOver = GV.WheelBusy ? false : true;
                                seqNo = 10;
                            }
                            else if(!GV.TransferMoveEnable) // 이게 없으니까 무언정지 생기네.. TransferMoveEnable 풀릴때까지 기다리기 때문..
                            {
                                if (GV.TransferMoveEnableCode == 1) rv = m_Device.ALM_MoveInterlockBummperError.ID;
                                else if (GV.TransferMoveEnableCode == 2) rv = m_Device.ALM_MoveInterlockSteerNotChangeError.ID;
                                else if (GV.TransferMoveEnableCode == 3) rv = m_Device.ALM_MoveInterlockHoistPositionError.ID;
                                else if (GV.TransferMoveEnableCode == 4) rv = m_Device.ALM_MoveInterlockSlidePositionError.ID;
                                else if (GV.TransferMoveEnableCode == 5) rv = m_Device.ALM_MoveInterlockWheelBrakeError.ID;
                                //else if (GV.TransferMoveEnableCode == 6) rv = m_Device.ALM_MoveInterlockForwardDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 7) rv = m_Device.ALM_MoveInterlockFoupExistButGripOpenError.ID;
                                else if (GV.TransferMoveEnableCode == 8) rv = m_Device.ALM_MoveInterlockHoistHomeSensorNotDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 9) rv = m_Device.ALM_MoveInterlockSlideHomeSensorNotDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 10) rv = m_Device.ALM_MoveInterlockRotateHomeSensorNotDetectError.ID;
                                
                                if(rv > 0)
                                    SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Interlock Alarm"));

                                seqNo = 0;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_BusyTimeOver && GV.WheelBusy)
                            {
                                StartTicks = XFunc.GetTickCount(); //움직이고 있는데 Busy가 꺼졌다.. 그런데 StartTick이 SequenceMove 하기전에 시작해서 그냥 바로 들어온다..
                                m_BusyTimeOver = false;
                            }
                            bool timer_over = XFunc.GetTickCount() - StartTicks > 200;

                            if (timer_over)
                            {
                                enAxisInFlag state = m_Master.GetAxis().AxisStatus; // (m_Master.GetAxis() as IAxisCommand).GetAxisCurStatus();

                                //Axis Done
                                bool range_ok = true;
                                if (ProcessDataHandler.Instance.CurTransferCommand != null &&
                                    ProcessDataHandler.Instance.CurTransferCommand.RunPathMaps != null &&
                                    ProcessDataHandler.Instance.CurTransferCommand.RunPathMaps.Count > 1)
                                {
                                    double runDistance = ProcessDataHandler.Instance.CurTransferCommand.RunPathMaps.Last().RunDistance; // 마지막 구간에 진입했으면 멈추는 동작을 할거야...
                                    double targetDistance = 0.1f * ProcessDataHandler.Instance.CurTransferCommand.RunPathMaps.Last().TotalDistance;
                                    double remainDistance = ProcessDataHandler.Instance.CurTransferCommand.RemainMotorDistance;
                                    range_ok &= (remainDistance < targetDistance || remainDistance < runDistance);
                                    // target - remain compare
                                    //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                    //System.Diagnostics.Debug.WriteLine($"{time} targetDistance={targetDistance}, remainDistance={remainDistance}, diff={range_ok}");
                                }

                                enAxisResult move_end = m_Master.GetServoUnit().MotionDoneAxis(m_Master.GetAxis());
                                if (move_end == enAxisResult.Success && !GV.WheelBusy)
                                {
                                    if (range_ok)
                                    {
                                        SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Move OK", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName));
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 20;
                                    }
                                    else if (XFunc.GetTickCount() - StartTicks > 10 * 1000) //움직이고 있는데 Busy가 꺼졌다.. 그런데 StartTick이 SequenceMove 하기전에 시작해서 그냥 바로 들어온다..
                                    {
                                        // 왜 멈추고 있지 ? MXP가 Command를 실행하지 않는 경우가 발생하네....10초 지나면 알람을 띄우자...
                                        string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                        List<int> con_list = (m_Master.GetAxis() as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                        List<int> dri_list = (m_Master.GetAxis() as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                        if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                        string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Master.GetAxis() as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                        SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Move Timeout Error #1 [code={2}][{3}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, code_name, axis_alarm_code));
                                        rv = m_Master.GetAlarmID(enAxisResult.Timeover);
                                        seqNo = 0;
                                    }
                                }
                                else if (move_end == enAxisResult.CmdError && m_CmdErrorRetryCount < 5)
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("{0}.{1} SequenceMove Command Set Abnormal. Retry !", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else if (m_BusyTimeOver && XFunc.GetTickCount() - StartTicks > 3000)
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("{0}.{1} SequenceMove Command Set Abnormal. Not Run !", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else if (move_end > enAxisResult.Success)
                                {
                                    string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                    List<int> con_list = (m_Master.GetAxis() as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                    List<int> dri_list = (m_Master.GetAxis() as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                    if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                    string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Master.GetAxis() as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                    SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Move Error #1 [code={2}][{3}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, code_name, axis_alarm_code));

                                    if (move_end == enAxisResult.InrangeError)
                                    {
                                        m_Master.GetServoUnit().AlarmClear(); // 일단 지우자~~
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 200;
                                    }
                                    else
                                    {
                                        rv = m_Master.GetAlarmID(move_end);
                                        seqNo = 0;
                                    }
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 1000) // 명령이 내려지길 기다렸다가 보자 ~~
                                {
                                    bool alarm = false;
                                    alarm |= state.HasFlag(enAxisInFlag.Alarm);
                                    //alarm |= state.HasFlag(enAxisInFlag.OverrideAbnormalStop);
                                    alarm |= state.HasFlag(enAxisInFlag.InRange_Error);
                                    if (alarm)
                                    {
                                        if (state.HasFlag(enAxisInFlag.Alarm)) move_end = enAxisResult.Alarm;
                                        //else if (state.HasFlag(enAxisInFlag.OverrideAbnormalStop)) move_end = enAxisResult.OverrideAbnormalStop;
                                        else if (state.HasFlag(enAxisInFlag.InRange_Error)) move_end = enAxisResult.InrangeError;

                                        string code_name = Enum.GetName(typeof(enAxisResult), move_end);

                                        List<int> con_list = (m_Master.GetAxis() as MpAxis).ControllerAlarmIdList.FindAll(n => n != 0);
                                        List<int> dri_list = (m_Master.GetAxis() as MpAxis).DriverAlarmIdList.FindAll(n => n != 0);
                                        if (con_list.Count == 0) con_list.Add(0); if (dri_list.Count == 0) dri_list.Add(0);

                                        string axis_alarm_code = string.Format("CMD={0},Control={1},Driver={2}", (m_Master.GetAxis() as MpAxis).CommandAlarmId, string.Join("-", con_list), string.Join("-", dri_list));
                                        SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Move Error #2[code={2}][{3}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, code_name, axis_alarm_code));
                                        if (move_end == enAxisResult.InrangeError)
                                        {
                                            m_Master.GetServoUnit().AlarmClear(); // 일단 지우자~~
                                            StartTicks = XFunc.GetTickCount();
                                            seqNo = 200;
                                        }
                                        else
                                        {
                                            rv = m_Master.GetAlarmID(move_end);
                                            seqNo = 0;
                                        }
                                    }
                                }

                                bool mp_control = m_Master.GetAxis().SequenceCommand.PositionSensorInfo.ControlMp;
                                if (!mp_control)
                                {
                                    //혹시 MP에서 BCR 정지가 잘 되지 않는 경우 여기서 ABS Move 관찰하다가 External Encorder Run 시키자 ~~~
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            enAxisInFlag state = m_Master.GetAxis().AxisStatus;// (m_Master.GetAxis() as IAxisCommand).GetAxisCurStatus();
                            bool inRangeError = (state & enAxisInFlag.InRange_Error) == enAxisInFlag.InRange_Error ? true : false;
                            bool inRangeChecking = (state & enAxisInFlag.InRange_Checking) == enAxisInFlag.InRange_Checking ? true : false;

                            bool in_range = true;
                            double diff = 0.0f;
                            if (m_Device.m_MotionSensor.SensorUse == 1)
                            {
                                //diff = ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance;
                                double leftBcrDiff = m_Device.m_MotionSensor.SensorTargetValue - ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                                double rightBcrDiff = m_Device.m_MotionSensor.SensorTargetValue - ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                                diff = Math.Min(Math.Abs(leftBcrDiff), Math.Abs(rightBcrDiff));
                                in_range &= Math.Abs(diff) < m_Device.GetInRange();
                                in_range &= inRangeChecking == false;
                            }
                            else
                            {
                                diff = ProcessDataHandler.Instance.CurTransferCommand.RemainMotorDistance;
                                in_range &= Math.Abs(diff) < m_Device.GetInRange();
                            }
                            if (in_range)
                            {
                                if (m_Device.m_MotionSensor.SensorUse == 1)
                                {
                                    if (m_RetryCount > 3 || m_InrangeCheckOK)
                                    {
                                        SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Check[Target:{2},Run:{3},Current:{4}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_Master.GetTargetPos(), m_Device.MotionRunPosition, m_Master.GetCurPosition()));
                                        if (inRangeError)
                                        {
                                            m_Master.GetServoUnit().AlarmClear();
                                            StartTicks = XFunc.GetTickCount();
                                            seqNo = 200;
                                        }
                                        else
                                        {
                                            rv = 0;
                                            seqNo = 0;
                                        }
                                    }
                                    else
                                    {
                                        m_InrangeCheckOK = true;
                                        SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Check Retry[Target:{2},Run:{3},Current:{4},Retry:{5}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_Master.GetTargetPos(), m_Device.MotionRunPosition, m_Master.GetCurPosition(), m_RetryCount));
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 40;
                                    }
                                }
                                else
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Check[Target:{2},Run:{3},Current:{4}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_Master.GetTargetPos(), m_Device.MotionRunPosition, m_Master.GetCurPosition()));
                                    if (inRangeError)
                                    {
                                        m_Master.GetServoUnit().AlarmClear();
                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 200;
                                    }
                                    else
                                    {
                                        rv = 0;
                                        seqNo = 0;
                                    }
                                }
                            }
                            else if (inRangeError)
                            {
                                // Alarm
                                SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Check Mxp.InRange Alarm [Target:{2},Cur:{3}, Diff:{4}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_Master.GetTargetPos(), m_Device.MotionRunPosition, diff));
                                if (inRangeError)
                                {
                                    m_Master.GetServoUnit().AlarmClear();
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 200;
                                }
                                else
                                {
                                    rv = m_Device.ALM_SequenceMoveInRangeError.ID;
                                    seqNo = 0;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10000)
                            {
                                // 여기서 이동할 거리는 BCR 남은 거리를 적용하자
                                diff = ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance;
                                // MXP가 RemainDistance가 많이 남았는데 멈추어 버렸네...BCR Scan을 못하는거잖아...
                                int lastLinkId = ProcessDataHandler.Instance.CurTransferCommand.RunPathMaps.Last().LinkID;
                                int curLinkId = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID;
                                bool more_move = lastLinkId == curLinkId;
                                more_move = diff > 0 && diff < ProcessDataHandler.Instance.CurTransferCommand.RunPathMaps.Last().RunDistance;
                                more_move = diff > m_Device.GetInRange();
                                if (more_move)
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Check NG [Target:{2},Cur:{3}, Diff:{4}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_Master.GetTargetPos(), m_Device.MotionRunPosition, diff));
                                    // abs move
                                    m_TargetPosition = m_Master.GetCurPosition() + diff;
                                    ushort propId = m_Device.TeachingVelocitySearch.PropId;
                                    m_SearchingVelSet = m_Master.GetTeachingVel(propId);
                                    seqNo = 30;

                                }
                                else
                                {
                                    // Alarm
                                    SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Check TimeOver.InRange Alarm [Target:{2},Cur:{3}, Diff:{4}]", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_Master.GetTargetPos(), m_Device.MotionRunPosition, diff));
                                    rv = m_Device.ALM_SequenceMoveInRangeError.ID;
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 30:
                        {
                            rv1 = m_Device.Move(m_TargetPosition, m_SearchingVelSet, true);
                            if (rv1 == 0)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position More Move OK - {0}", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_RetryCount));
                                seqNo = 20;
                            }
                            else if (rv1 > 0)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position More Move NG", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName));
                                rv = rv1;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 40:
                        {
                            double in_range = m_Device.GetInRange();
                            double leftBcrDiff = m_Device.m_MotionSensor.SensorTargetValue - ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBcrDiff = m_Device.m_MotionSensor.SensorTargetValue - ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            double diff = Math.Min(Math.Abs(leftBcrDiff), Math.Abs(rightBcrDiff));
                            m_InrangeCheckOK &= diff < in_range;

                            if (XFunc.GetTickCount() - StartTicks > 300)
                            {
                                SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Position Move Retry - {2}:leftBcrDiff={3},rightBcrDiff={4},in_range={5}", m_Master.GetServoUnit().ServoName, m_Master.GetAxis().AxisName, m_RetryCount, leftBcrDiff, rightBcrDiff, in_range));
                                m_RetryCount++;
                                seqNo = 20;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000)
                            {
                                if (m_BusyTimeOver) m_BusyTimeOver = false;
                                else m_CmdErrorRetryCount++;

                                // 잘못된 명령이 날아가는 경우 Hold Bit로 막고 있다...
                                // 그래서 출발할때는 Reset을 하고 시작해야 겠다...
                                m_Master.SetHolding(false);

                                // Sequence 명령을 만들어 날리자 !
                                // Target Position Check
                                // Safty Check
                                SequenceCommand command = m_Master.GetAxis().SequenceCommand;
                                command.MoveMethod = enMotionMoveMethod.Relative;
                                command.BufferMode = enMotionBufferMode.BlendingLow;
                                command.MotionProfiles = ObjectCopier.Clone(m_Device.m_MotionProfiles);
                                command.PositionSensorInfo = ObjectCopier.Clone(m_Device.m_MotionSensor);

                                enAxisResult rv2 = m_Master.GetServoUnit().SequenceMoveAxisStart(m_Master.GetAxis(), command);
                                SequenceDeviceLog.WriteLog(string.Format("{0}.{1} Sequence Move Retry Start, Result : {2}", m_Master.GetServoUnit().ServoName, m_Master.GetName(), rv2));
                                SequenceDeviceLog.WriteLog(string.Format("Set Command :\r\n{0}", command.GetCommandLog()));
                                StartTicks = XFunc.GetTickCount();

                                m_InrangeCheckOK = false;
                                seqNo = 10;
                            }
                        }
                        break;

                    case 200:
                        {
                            // Alarm Clear 하고 있는데 다음 명령이 내려오는 경우가 있음.
                            enAxisResult move_end = m_Master.GetServoUnit().MotionDoneAxis(m_Master.GetAxis());
                            if (move_end == enAxisResult.Success)
                            {
                                rv = m_Device.ALM_SequenceMoveInRangeError.ID;
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5000)
                            {
                                rv = m_Device.ALM_SequenceMoveInRangeError.ID;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
            #endregion
            #region Methods
            public int Home()
            {
                int rv = -1;
                int seqNo = m_HomeSeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Home Start"));

                            m_TargetDevAxes.Clear();
                            foreach (_DevAxis axis in m_DevAxes) m_TargetDevAxes.Add(axis);
                            for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                            seqNo = 100;
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])
                                {
                                    int home_status = -1;
                                    home_status = axis.Home();
                                    if (home_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Home OK [{0}]", axis.GetName()));
                                    }
                                    else if (home_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Home NG [{0}][alarm={1}]", axis.GetName(), home_status));
                                        almId = home_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Home Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Home Alarm"));
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                m_HomeSeqNo = seqNo;
                return rv;
            }
            public int Move(ushort point, ushort prop)
            {
                int rv = -1;
                int seqNo = m_MoveSeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (GV.TransferMoveEnable)
                            {
                                string logMsg = "";
                                // Move Order 계산.
                                m_TargetDevAxes.Clear();
                                m_TargetPoint.Clear();
                                m_TargetProp.Clear();
                                foreach (_DevAxis axis in m_DevAxes)
                                {
                                    m_TargetDevAxes.Add(axis);
                                    m_TargetPoint.Add(point);
                                    m_TargetProp.Add(prop);
                                    logMsg += string.Format("[{0}={1}],", axis.GetName(), point);
                                }

                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Start [{0}]", logMsg));
                                seqNo = 100;
                            }
                            else
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Interlock Alarm"));

                                if (GV.TransferMoveEnableCode == 1) rv = m_Device.ALM_MoveInterlockBummperError.ID;
                                else if (GV.TransferMoveEnableCode == 2) rv = m_Device.ALM_MoveInterlockSteerNotChangeError.ID;
                                else if (GV.TransferMoveEnableCode == 3) rv = m_Device.ALM_MoveInterlockHoistPositionError.ID;
                                else if (GV.TransferMoveEnableCode == 4) rv = m_Device.ALM_MoveInterlockSlidePositionError.ID;
                                else if (GV.TransferMoveEnableCode == 5) rv = m_Device.ALM_MoveInterlockWheelBrakeError.ID;
                                else if (GV.TransferMoveEnableCode == 6) rv = m_Device.ALM_MoveInterlockForwardDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 7) rv = m_Device.ALM_MoveInterlockFoupExistButGripOpenError.ID;
                                else if (GV.TransferMoveEnableCode == 8) rv = m_Device.ALM_MoveInterlockHoistHomeSensorNotDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 9) rv = m_Device.ALM_MoveInterlockSlideHomeSensorNotDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 10) rv = m_Device.ALM_MoveInterlockRotateHomeSensorNotDetectError.ID;
                                else rv = m_Device.ALM_MoveInterlockFoupExistButGripOpenError.ID;

                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])
                                {
                                    int move_status = -1;
                                    move_status = axis.Move(m_TargetPoint[index], m_TargetProp[index], true);
                                    if (move_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Alarm"));
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                m_MoveSeqNo = seqNo;
                return rv;
            }
            public int Move(double pos, VelSet set, bool safty_use)
            {
                int rv = -1;
                int seqNo = m_MoveSeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            bool interlock_ok = true;
                            if (safty_use) interlock_ok = GV.TransferMoveEnable;
                            if (interlock_ok)
                            {
                                double diff_master = Math.Abs(pos - m_Master.GetCurPosition());
                                double diff_slave = Math.Abs(pos - m_Slave.GetCurPosition());
                                m_Device.WheelMasterWorkingDistance += diff_master;
                                m_Device.WheelSlaveWorkingDistance += diff_slave;
                                m_Device.m_PlusMoved = pos - m_Master.GetCurPosition() > 0.0f;
                                string logMsg = "";
                                // Move Order 계산.
                                m_TargetDevAxes.Clear();
                                m_TargetPos.Clear();
                                m_TargetVelSet.Clear();
                                foreach (_DevAxis axis in m_DevAxes)
                                {
                                    m_TargetDevAxes.Add(axis);
                                    m_TargetPos.Add(pos);
                                    m_TargetVelSet.Add(set);
                                    logMsg += string.Format("[{0}={1}],", axis.GetName(), pos);
                                }

                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Start [{0}]", logMsg));
                                seqNo = 100;
                            }
                            else
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Interlock Alarm"));
                                if (GV.TransferMoveEnableCode == 1) rv = m_Device.ALM_MoveInterlockBummperError.ID;
                                else if (GV.TransferMoveEnableCode == 2) rv = m_Device.ALM_MoveInterlockSteerNotChangeError.ID;
                                else if (GV.TransferMoveEnableCode == 3) rv = m_Device.ALM_MoveInterlockHoistPositionError.ID;
                                else if (GV.TransferMoveEnableCode == 4) rv = m_Device.ALM_MoveInterlockSlidePositionError.ID;
                                else if (GV.TransferMoveEnableCode == 5) rv = m_Device.ALM_MoveInterlockWheelBrakeError.ID;
                                else if (GV.TransferMoveEnableCode == 6) rv = m_Device.ALM_MoveInterlockForwardDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 7) rv = m_Device.ALM_MoveInterlockFoupExistButGripOpenError.ID;
                                else if (GV.TransferMoveEnableCode == 8) rv = m_Device.ALM_MoveInterlockHoistHomeSensorNotDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 9) rv = m_Device.ALM_MoveInterlockSlideHomeSensorNotDetectError.ID;
                                else if (GV.TransferMoveEnableCode == 10) rv = m_Device.ALM_MoveInterlockRotateHomeSensorNotDetectError.ID;
                                else rv = m_Device.ALM_MoveInterlockFoupExistButGripOpenError.ID;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            bool alarm = false;
                            int almId = 0;
                            bool complete = true;
                            int index = 0;
                            foreach (_DevAxis axis in m_TargetDevAxes)
                            {
                                if (!m_CommandComp[index])								
                                {
                                    int move_status = -1;
                                    move_status = axis.Move(m_TargetPos[index], m_TargetVelSet[index], safty_use);
                                    if (move_status == 0)
                                    {
                                        m_CommandComp[index] = true;
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move OK [{0}]", axis.GetName()));
                                    }
                                    else if (move_status > 0)
                                    {
                                        SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move NG [{0}][alarm={1}]", axis.GetName(), move_status));
                                        almId = move_status;
                                        alarm = true;
                                    }
                                }

                                complete &= m_CommandComp[index];
                                index++;
                            }

                            if (complete)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Complete"));
                                rv = 0;
                                seqNo = 0;
                            }
                            else if (alarm)
                            {
                                SequenceDeviceLog.WriteLog(m_Device.MyName, string.Format("Transfer Move Alarm"));
                                foreach (_DevAxis axis in m_TargetDevAxes) axis.SeqAbort();
                                for (int i = 0; i < m_CommandComp.Length; i++) m_CommandComp[i] = false;
                                rv = almId;
                                seqNo = 0;
                            }
                        }
                        break;
                }

                m_MoveSeqNo = seqNo;
                return rv;
            }
            #endregion
        }
        #endregion
        #region [Xml Read/Write]
        public override bool ReadXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return false;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists == false)
                {
                    WriteXml();
                }

                var helperXml = new XmlHelper<DevTransfer>();
                DevTransfer dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;

                    this.AxisMaster = dev.AxisMaster;
                    this.AxisSlave = dev.AxisSlave;

                    this.MoveInterlockSensors = dev.MoveInterlockSensors;
                    this.DoWheelBrake = dev.DoWheelBrake;
                    this.DiWheelBrake = dev.DiWheelBrake;
                    this.TeachingVelocitySearch = dev.TeachingVelocitySearch;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }

        public override void WriteXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return;

            try
            {
                var helperXml = new XmlHelper<DevTransfer>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        public bool CheckPath(ref string fileName)
        {
            bool ok = false;
            string filePath = AppConfig.Instance.XmlDevicesPath;

            if (Directory.Exists(filePath) == false)
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "Configuration folder select";
                dlg.SelectedPath = AppConfig.GetSolutionPath();
                dlg.ShowNewFolderButton = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filePath = dlg.SelectedPath;
                    if (MessageBox.Show("do you want to save seleted folder !", "SAVE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        AppConfig.Instance.ConfigPath.SelectedFolder = filePath;
                        AppConfig.Instance.WriteXml();
                    }
                    fileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }
            else
            {
                fileName = string.Format("{0}\\{1}.xml", filePath, GetDefaultFileName());
                ok = true;
            }
            return ok;
        }

        public string GetDefaultFileName()
        {
            if (this.MyName == "") this.MyName = DevName;
            return this.ToString();
        }
        #endregion

    }
}
