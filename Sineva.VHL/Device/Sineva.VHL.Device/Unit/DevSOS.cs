using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Net;
using System.Windows.Forms;
using System.IO;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using Sineva.VHL.Data.Alarm;
using static System.Collections.Specialized.BitVector32;
using System.Drawing.Design;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using System.Reflection;
using Sineva.VHL.Data;

namespace Sineva.VHL.Device
{
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class SosTable
    {
        #region Fields
        #endregion
        #region Properties
        public int Level0 { get; set; } // > 99999
        public int Level1 { get; set; } // 8000 ~ 10000
        public int Level2 { get; set; } // 6000 ~ 8000
        public int Level3 { get; set; } // 5000 ~ 6000
        public int Level4 { get; set; } // 4000 ~ 5000
        public int Level5 { get; set; } // 3000 ~ 4000
        public int Level6 { get; set; } // 2000 ~ 3000
        public int Level7 { get; set; } // 1000 ~ 2000
        public int Level8 { get; set; } // 500 ~ 1000
        public int Level9 { get; set; } // 0 ~ 500
        #endregion

        #region Constructor
        public SosTable()
        {
        }
        #endregion
    }
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevSOS : _Device
    {
        private const string DevName = "DevSOS";

        #region Fields
        private _DevSOS m_OBS = new _DevSOS();

        /////////////////////////////////////////
        private AlarmData m_ALM_Command_Set = null;
        private int m_SetDelayTime = 100; // Level Change Delay Time
        private List<SosTable> m_DistanceTable = new List<SosTable>();
        private enFrontDetectState m_OverrideReleaseState = enFrontDetectState.enDeccelation5;

        private SeqAction m_SeqAction = null;
        private SeqMonitor m_SeqMonitor = null;

        private enFrontDetectState m_FrontDetectState = enFrontDetectState.enNone;
        private uint m_SetOBS = 0;
        private uint m_SetChangeOBS = 0;
        private bool m_AreaSetting = false;
        #endregion

        #region Property - Device
        [Category("!Setting Device (I/O Setting)"), Description("OBS I/O"), DeviceSetting(true)]
        public _DevSOS OBS 
        { 
            get { return m_OBS; } 
            set { m_OBS = value; } 
        }
        #endregion
        #region Property - Timer Setting
        [Category("!Setting Device"), DisplayName("Level Change Delay Time(msec)"), Description("Level Change Delay Time(msec)"), DeviceSetting(true)]
        public int SetDelayTime 
        { 
            get { return m_SetDelayTime; } 
            set { m_SetDelayTime = value; } 
        }
        [Category("!Setting Device"), DisplayName("Level Set Distance(mm)"), Description("Level Set Distance(mm)"), DeviceSetting(true)]
        public List<SosTable> DistanceTable
        {
            get { return m_DistanceTable; }
            set { m_DistanceTable = value; }
        }
        [Category("!Setting Device"), DisplayName("Override Release Level"), Description("Override Release Level"), DeviceSetting(true)]
        public enFrontDetectState OverrideReleaseState 
        {
            get { return m_OverrideReleaseState; }
            set { m_OverrideReleaseState = value; }
        }
        #endregion

        #region AlarmData
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_Command_Set
        {
            get { return m_ALM_Command_Set; }
            set { m_ALM_Command_Set = value; }
        }
        #endregion

        #region Constructor
        public DevSOS()
        {
            this.MyName = "DevSOS";
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
            #region 2. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;
            if (m_OBS.IsValid) ok &= m_OBS.Initialize(this.ToString(), false, false);

            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            m_ALM_Command_Set = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, MyName, "Upper.SOS", "Command Setting Alarm");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            SetOBS(0);
            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            if (ok)
            {
                m_SeqAction = new SeqAction(this);
                m_SeqMonitor = new SeqMonitor(this);
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

        #region Methods - public
        public int SetOBS(uint type) 
        {
            int rv = 0;
            if (m_SetOBS != m_SetChangeOBS) { m_AreaSetting = false; rv = -1; } //ING
            else if (m_SetOBS == m_SetChangeOBS && m_AreaSetting == false) { m_AreaSetting = true; rv = 1; }
            m_SetChangeOBS = type;
            return rv;
        }
        public uint GetOBS() { return m_SetOBS; }
        public enFrontDetectState GetFrontDetectState()
        {
            enFrontDetectState rv = m_FrontDetectState;
            return rv; 
        }
        public double GetTableDistanceOfDetectState(enFrontDetectState beforeState)
        {
            double rv = double.MaxValue;
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    // near Case
                    if (beforeState < m_FrontDetectState)
                    {
                        if (m_FrontDetectState == enFrontDetectState.enDeccelation1) rv = DistanceTable[row].Level0; // 10000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation2) rv = DistanceTable[row].Level1; // 8000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation3) rv = DistanceTable[row].Level2; // 6000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation4) rv = DistanceTable[row].Level3; // 5000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation5) rv = DistanceTable[row].Level4; // 4000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation6) rv = DistanceTable[row].Level5; // 3000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation7) rv = DistanceTable[row].Level6; // 2000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation8) rv = DistanceTable[row].Level7; // 1000
                        else if (m_FrontDetectState == enFrontDetectState.enStop) rv = DistanceTable[row].Level8; // 0
                    }
                    // far away Case
                    else
                    {
                        if (m_FrontDetectState == enFrontDetectState.enStop) rv = DistanceTable[row].Level9; // 500
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation8) rv = DistanceTable[row].Level8; // 0
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation7) rv = DistanceTable[row].Level7; // 1000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation6) rv = DistanceTable[row].Level6; // 2000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation5) rv = DistanceTable[row].Level5; // 3000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation4) rv = DistanceTable[row].Level4; // 4000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation3) rv = DistanceTable[row].Level3; // 5000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation2) rv = DistanceTable[row].Level2; // 6000
                        else if (m_FrontDetectState == enFrontDetectState.enDeccelation1) rv = DistanceTable[row].Level1; // 8000
                        else if (m_FrontDetectState == enFrontDetectState.enNone) rv = DistanceTable[row].Level0; // > 10000
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public double GetTableAreaNoneDistance()
        {
            double rv = 99999.0f; //straight type 최대거리 10m
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    rv = DistanceTable[row].Level0;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public double GetTableAreaMaxDistance()
        {
            double rv = 10000.0f; //straight type 최대거리 10m
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    rv = DistanceTable[row].Level1;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public double GetTableAreaMinDistance()
        {
            double rv = 0.0f; //최소거리 0m
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    rv = DistanceTable[row].Level7;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public double GetEnableAccelerationMinDistance()
        {
            double rv = 3000.0f; //default 3m 이상이면 가속하도록 하자
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    rv = DistanceTable[row].Level5;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public double GetSensorLevelMinDistance()
        {
            double rv = 99999.0f; //straight type 최대거리 10m
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    if (m_FrontDetectState == enFrontDetectState.enNone) rv = DistanceTable[row].Level0; // > 10000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation1) rv = DistanceTable[row].Level1; // 8000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation2) rv = DistanceTable[row].Level2; // 6000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation3) rv = DistanceTable[row].Level3; // 5000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation4) rv = DistanceTable[row].Level4; // 4000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation5) rv = DistanceTable[row].Level5; // 3000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation6) rv = DistanceTable[row].Level6; // 2000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation7) rv = DistanceTable[row].Level7; // 1000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation8) rv = DistanceTable[row].Level8; // 0
                    else rv = DistanceTable[row].Level9; // 500
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public double GetSensorLevelMaxDistance()
        {
            double rv = 99999.0f; //straight type 최대거리 10m
            try
            {
                int row = (int)m_SetOBS;
                if (row < DistanceTable.Count)
                {
                    if (m_FrontDetectState == enFrontDetectState.enNone) rv = DistanceTable[row].Level0; // 10000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation1) rv = DistanceTable[row].Level0; // 10000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation2) rv = DistanceTable[row].Level1; // 8000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation3) rv = DistanceTable[row].Level2; // 6000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation4) rv = DistanceTable[row].Level3; // 5000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation5) rv = DistanceTable[row].Level4; // 4000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation6) rv = DistanceTable[row].Level5; // 3000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation7) rv = DistanceTable[row].Level6; // 2000
                    else if (m_FrontDetectState == enFrontDetectState.enDeccelation8) rv = DistanceTable[row].Level7; // 1000
                    else if (m_FrontDetectState == enFrontDetectState.enStop) rv = DistanceTable[row].Level8; // 0
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        #endregion

        #region Sequence
        private class SeqAction : XSeqFunc
        {
            #region Field
            private DevSOS m_Device = null;
            #endregion

            #region Constructor
            public SeqAction(DevSOS device)
            {
                this.SeqName = $"SeqAction{device.MyName}";
                m_Device = device;
                StartTicks = XFunc.GetTickCount();

                TaskDeviceControlHighSpeed.Instance.RegSeq(this);
            }
            #endregion

            #region Override
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.OBS.IsValid)
                            {
                                uint current_path_obs = (uint)ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperAreaValue;
                                if (m_Device.m_SetChangeOBS != m_Device.OBS.GetOBSType())
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("{0} OBS Change [Before={1}, After={2}]", m_Device.MyName, m_Device.OBS.GetOBSType(), m_Device.m_SetChangeOBS));
                                    m_Device.OBS.SetOBSType(m_Device.m_SetChangeOBS);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else if (current_path_obs != m_Device.OBS.GetOBSType())
                                {
                                    SequenceDeviceLog.WriteLog(string.Format("{0} Link OBS Change [Before={1}, After={2}]", m_Device.MyName, m_Device.OBS.GetOBSType(), current_path_obs));
                                    m_Device.OBS.SetOBSType(current_path_obs);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 20;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            // 전방감지 설정할 시간을 주자
                            if (GetElapsedTicks() > m_Device.SetDelayTime)
                            {
                                m_Device.m_SetOBS = m_Device.m_SetChangeOBS;
                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS Change Delay [{1}(ms)]", m_Device.MyName, m_Device.SetDelayTime));
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            // 전방감지 설정할 시간을 주자
                            if (GetElapsedTicks() > m_Device.SetDelayTime)
                            {
                                m_Device.m_SetOBS = (uint)ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperAreaValue;
                                SequenceDeviceLog.WriteLog(string.Format("{0} Link OBS Change Delay [{1}(ms)]", m_Device.MyName, m_Device.SetDelayTime));
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            private DevSOS m_Device = null;
            private uint m_DelayTime = 60;
            private bool m_FirstTimeUpdate = true;
            #endregion

            #region Constructor
            public SeqMonitor(DevSOS device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                StartTicks = XFunc.GetTickCount();

                TaskDeviceControlHighSpeed.Instance.RegSeq(this);
            }
            #endregion

            #region Override
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_Device.OBS.IsValid)
                            {
                                // Real Time이 않되는 경우 Count를 조절하자 ~~
                                int id = TaskDeviceControlHighSpeed.Instance.ID;
                                float scan_time = XSequence.StatusList[id].LastScanMilliSec;
                                if (scan_time > 50.0f && m_Device.OBS.ScanAverageCount != 1) m_Device.OBS.SetScanCount(1);
                                else if (scan_time > 40.0f && m_Device.OBS.ScanAverageCount != 2) m_Device.OBS.SetScanCount(2);
                                else if (scan_time > 30.0f && m_Device.OBS.ScanAverageCount != 3) m_Device.OBS.SetScanCount(3);
                                else if (scan_time > 20.0f && m_Device.OBS.ScanAverageCount != 4) m_Device.OBS.SetScanCount(4);
                                else if (m_Device.OBS.ScanAverageCount != 5) m_Device.OBS.SetScanCount(5);

                                // Real Time Monitoring
                                double obs1 = m_Device.OBS.GetUSB1();
                                double obs2 = m_Device.OBS.GetUSB2();
                                double obs3 = m_Device.OBS.GetUSB3();
                                double obs4 = m_Device.OBS.GetUSB4();

                                bool ng = m_Device.m_SetOBS != m_Device.m_SetChangeOBS; // Area 변경 되고 있는 중임. 이때는 Level 불확실함.
                                ng |= obs1 > 0.0f && obs1 < 1.0f ? true : false;
                                ng |= obs2 > 0.0f && obs2 < 1.0f ? true : false;
                                ng |= obs3 > 0.0f && obs3 < 1.0f ? true : false;
                                ng |= obs4 > 0.0f && obs4 < 1.0f ? true : false;

                                int rv = 0;
                                if (!ng)
                                {
                                    enFrontDetectState detect_state = enFrontDetectState.enNone;
                                    rv |= obs1 == 1.0f ? 0x01 << 0 : 0x00;
                                    rv |= obs2 == 1.0f ? 0x01 << 1 : 0x00;
                                    rv |= obs3 == 1.0f ? 0x01 << 2 : 0x00;
                                    rv |= obs4 == 1.0f ? 0x01 << 3 : 0x00;
                                    if (SetupManager.Instance.SetupSafty.OBSUpperSensorUse == Use.NoUse) rv = 0;
                                    if (rv > 8) detect_state = enFrontDetectState.enStop;
                                    else detect_state = (enFrontDetectState)rv;

                                    if (detect_state == enFrontDetectState.enNone)
                                    {
                                        // 비정상적으로 바뀌면서 충돌 사고를 발생시킨다.
                                        // 정상 1~6단계에서 NONE으로 변경되는 경우
                                        if (m_Device.m_FrontDetectState != detect_state || m_FirstTimeUpdate)
                                        {
                                            m_FirstTimeUpdate = false;
                                            if (m_Device.m_FrontDetectState == enFrontDetectState.enDeccelation1)
                                            {
                                                //단계적 변화일 경우는 바로 update
                                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Change #1 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                                m_Device.m_FrontDetectState = detect_state;
                                            }
                                            else if (m_Device.m_FrontDetectState < enFrontDetectState.enDeccelation7)
                                            {
                                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Abnormal Change #1 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));

                                                m_DelayTime = 2;
                                                StartTicks = XFunc.GetTickCount();
                                                seqNo = 20;
                                            }
                                            else
                                            {
                                                SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Abnormal Change #1 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));

                                                // 1분 기다려도 계속 NONE이면 6->5->4->3->2->1->NONE 순서로 바꾸어 주자~~~
                                                m_DelayTime = 5; // 60;
                                                StartTicks = XFunc.GetTickCount();
                                                seqNo = 20;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int changed_level = Math.Abs(detect_state - m_Device.m_FrontDetectState);
                                        if (changed_level == 1 || m_FirstTimeUpdate)
                                        {
                                            m_FirstTimeUpdate = false;
                                            //단계적 변화일 경우는 바로 update
                                            SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Change #1 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                            m_Device.m_FrontDetectState = detect_state;
                                        }
                                        else if (changed_level > 1)
                                        {
                                            // 이상 변화 일 경우는 잠시(50msec) 대기해 보자
                                            SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Abnormal Change #1 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                            StartTicks = XFunc.GetTickCount();
                                            seqNo = 10;
                                        }
                                    }
                                }

                                // 속도 / 감속도 = dt0, 2 * 거리 / 속도 = dt1 , dt0 < dt1 인 경우는 시간이 좀 남아 있음. 거리는 1m 전에 멈추는 걸로 계산 
                                //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                //System.Diagnostics.Debug.WriteLine($"{time} SeqNo({seqNo}) Sensor Status {rv} =[{obs1}_{obs2}_{obs3}_{obs4}]");
                            }
                        }
                        break;

                    case 10:
                        {
                            // Real Time Monitoring
                            double obs1 = m_Device.OBS.GetUSB1();
                            double obs2 = m_Device.OBS.GetUSB2();
                            double obs3 = m_Device.OBS.GetUSB3();
                            double obs4 = m_Device.OBS.GetUSB4();

                            bool ng = m_Device.m_SetOBS != m_Device.m_SetChangeOBS;
                            ng |= obs1 > 0.0f && obs1 < 1.0f ? true : false;
                            ng |= obs2 > 0.0f && obs2 < 1.0f ? true : false;
                            ng |= obs3 > 0.0f && obs3 < 1.0f ? true : false;
                            ng |= obs4 > 0.0f && obs4 < 1.0f ? true : false;

                            if (!ng)
                            {
                                enFrontDetectState detect_state = enFrontDetectState.enNone;
                                int rv = 0;
                                rv |= obs1 == 1.0f ? 0x01 << 0 : 0x00;
                                rv |= obs2 == 1.0f ? 0x01 << 1 : 0x00;
                                rv |= obs3 == 1.0f ? 0x01 << 2 : 0x00;
                                rv |= obs4 == 1.0f ? 0x01 << 3 : 0x00;
                                if (rv > 8) detect_state = enFrontDetectState.enStop;
                                else detect_state = (enFrontDetectState)rv;

                                int changed_level = Math.Abs(detect_state - m_Device.m_FrontDetectState);
                                if (changed_level <= 1)
                                {
                                    // normal case
                                    SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Change #2 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                    m_Device.m_FrontDetectState = detect_state;
                                    seqNo = 0;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > 50)
                                {
                                    // 50msec가 지나도록 정상으로 돌아오지 않은 경우
                                    SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Abnormal Change #2 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                    m_Device.m_FrontDetectState = detect_state;
                                    seqNo = 0;
                                }
                            }
                        }
                        break;

                    case 20:
                        {
                            // Real Time Monitoring
                            double obs1 = m_Device.OBS.GetUSB1();
                            double obs2 = m_Device.OBS.GetUSB2();
                            double obs3 = m_Device.OBS.GetUSB3();
                            double obs4 = m_Device.OBS.GetUSB4();

                            bool ng = m_Device.m_SetOBS != m_Device.m_SetChangeOBS;
                            ng |= obs1 > 0.0f && obs1 < 1.0f ? true : false;
                            ng |= obs2 > 0.0f && obs2 < 1.0f ? true : false;
                            ng |= obs3 > 0.0f && obs3 < 1.0f ? true : false;
                            ng |= obs4 > 0.0f && obs4 < 1.0f ? true : false;

                            if (!ng)
                            {
                                enFrontDetectState detect_state = enFrontDetectState.enNone;
                                int rv = 0;
                                rv |= obs1 == 1.0f ? 0x01 << 0 : 0x00;
                                rv |= obs2 == 1.0f ? 0x01 << 1 : 0x00;
                                rv |= obs3 == 1.0f ? 0x01 << 2 : 0x00;
                                rv |= obs4 == 1.0f ? 0x01 << 3 : 0x00;
                                if (rv > 8) detect_state = enFrontDetectState.enStop;
                                else detect_state = (enFrontDetectState)rv;

                                if (detect_state != enFrontDetectState.enNone)
                                {
                                    // normal case
                                    SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Change #3 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, detect_state));
                                    m_Device.m_FrontDetectState = detect_state;
                                    seqNo = 0;
                                }
                                else if (XFunc.GetTickCount() - StartTicks > m_DelayTime * 1000) //delay time 기다려 보자~~
                                {
                                    enFrontDetectState set_front = m_Device.m_FrontDetectState - 1;
                                    if (set_front < enFrontDetectState.enNone) set_front = enFrontDetectState.enNone;
                                    // 1분이 지났는데도 계속 NONE이군. 단계적으로 해제하자~~
                                    SequenceDeviceLog.WriteLog(string.Format("{0} OBS State Abnormal Release #3 [Before={1}, After={2}]", m_Device.MyName, m_Device.m_FrontDetectState, set_front));

                                    if (set_front == enFrontDetectState.enNone)
                                    {
                                        m_Device.m_FrontDetectState = set_front;
                                        seqNo = 0;
                                    }
                                    else
                                    {
                                        m_Device.m_FrontDetectState = set_front;
                                        m_DelayTime = (uint)SetupManager.Instance.SetupSafty.OBSAbnormalReleaseWaitTime; // 2sec 간격으로 해제하자 ~~ //고객 요청으로 해제시간 1초로 변경.
                                        StartTicks = XFunc.GetTickCount();
                                    }
                                }
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
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

                var helperXml = new XmlHelper<DevSOS>();
                DevSOS dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;
                    this.SetDelayTime = dev.SetDelayTime;

                    this.OBS = dev.OBS;
                    this.DistanceTable = dev.DistanceTable;
                    this.OverrideReleaseState = dev.OverrideReleaseState;
                }
            }
            catch (Exception err)
            {
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
                var helperXml = new XmlHelper<DevSOS>();
                helperXml.Save(fileName, this);
            }
            catch (Exception err)
            {
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
