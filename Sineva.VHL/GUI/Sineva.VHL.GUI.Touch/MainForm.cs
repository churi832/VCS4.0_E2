using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using Sineva.VHL.Library.MXP;
using Sineva.VHL.Library.Servo;
using static Sineva.VHL.Library.MXP.MXP;
using Sineva.VHL.GUI.Touch.Properties;
using ModBus;

namespace Sineva.VHL.GUI.Touch
{
    public partial class MainForm : Form
    {
        #region Fields
        private ushort[] m_Status;
        private ushort[] m_Alarms;
        private const int MAX_STATUS_COUNT = 50;
        private const int MAX_ALARM_COUNT = 9000;
        private System.Windows.Forms.Timer m_TimerUpdate = null;

        private SeqTouchUpdate m_SeqTouchUpdate = new SeqTouchUpdate();
        private SeqHeartBit m_SeqHeartBit = null;

        private static XLogGarbageCollector _XLogGarbageCollector = null;

        /// <summary>
        /// override interlock 구현
        private int m_Level1VelLimit = 4000; //8000 ~ 10000
        private int m_Level2VelLimit = 4000; //6000 ~ 8000
        private int m_Level3VelLimit = 4000; //5000 ~ 6000
        private int m_Level4VelLimit = 3400; //4000 ~ 5000
        private int m_Level5VelLimit = 2500; //3000 ~ 4000
        private int m_Level6VelLimit = 1500; //2000 ~ 3000
        private int m_Level7VelLimit = 1000; //1000 ~ 2000
        private int m_Level8VelLimit = 500; //0 ~ 1000
        private int m_Level1Distance = 8000; //8000 ~ 10000
        private int m_Level2Distance = 6000; //6000 ~ 8000
        private int m_Level3Distance = 5000; //5000 ~ 6000
        private int m_Level4Distance = 4000; //4000 ~ 5000
        private int m_Level5Distance = 3000; //3000 ~ 4000
        private int m_Level6Distance = 2000; //2000 ~ 3000
        private int m_Level7Distance = 1000; //1000 ~ 2000
        private int m_Level8Distance = 500; //0 ~ 1000
        private enFrontDetectState m_UpperFrontState = enFrontDetectState.enNone;
        private double m_MasterVelocity = 0.0f;
        // XyPosition : X=Velocity, Y=Distance
        private Dictionary<enFrontDetectState, XyPosition> m_VelocityLimitTable = new Dictionary<enFrontDetectState, XyPosition>();
        private Queue<int> m_QueueOBS1 = new Queue<int>();
        private Queue<int> m_QueueOBS2 = new Queue<int>();
        private Queue<int> m_QueueOBS3 = new Queue<int>();
        private Queue<int> m_QueueOBS4 = new Queue<int>();
        private bool m_StopOverride = false;
        private uint m_StopSetTime = 0;
        /// </summary>
        /// 
        #endregion

        public MainForm()
        {
            InitializeComponent();
            InitializeMainFormComponent();
        }

        private void InitializeMainFormComponent()
        {
            m_SeqHeartBit = new SeqHeartBit(this);

            m_Status = new ushort[MAX_STATUS_COUNT];
            m_Alarms = new ushort[MAX_ALARM_COUNT];
            m_TimerUpdate = new Timer();
            m_TimerUpdate.Tick += new EventHandler(Instance_TimerUpdateTick);
            m_TimerUpdate.Interval = 20;
            m_TimerUpdate.Start();

            AppConfig.Instance.Initialize();
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation1, new XyPosition(m_Level1VelLimit, m_Level1Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation2, new XyPosition(m_Level2VelLimit, m_Level2Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation3, new XyPosition(m_Level3VelLimit, m_Level3Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation4, new XyPosition(m_Level4VelLimit, m_Level4Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation5, new XyPosition(m_Level5VelLimit, m_Level5Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation6, new XyPosition(m_Level6VelLimit, m_Level6Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation7, new XyPosition(m_Level7VelLimit, m_Level7Distance));
            m_VelocityLimitTable.Add(enFrontDetectState.enDeccelation8, new XyPosition(m_Level8VelLimit, m_Level8Distance));

            bool thread_create = false;
            ServoManager.Instance.Initialize(thread_create);
            MxpManager.Instance.Initialize(thread_create);
            TouchManager.Instance.Initialize();
            RemoteManager.TouchInstance.Initialize(ConnectionMode.Server, CHANNEL_TYPE.IPC, "127.0.0.1");
            TaskHandler.Instance.InitTaskHandler();

            ///Log 관련 Thread를 여기서 돌리자
            if (_XLogGarbageCollector == null)
            {
                _XLogGarbageCollector = XLogGarbageCollector.Instance;
                _XLogGarbageCollector.Initialize();
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            using (SettingForm form = new SettingForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    TouchManager.Instance.Save();
                }
            }
        }
        private void Instance_TimerUpdateTick(object sender, EventArgs e)
        {
            #region VHL Program HeartBit Check
            try
            {
                m_SeqHeartBit.Do();
                m_UpperFrontState = ReadOverride();
                m_MasterVelocity = GetMasterVelocity();

                label16.Text = string.Format("UpperFrontState={0}, MasterVelocity={1}", m_UpperFrontState, m_MasterVelocity);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            #endregion
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateObject();
                if (RemoteManager.TouchInstance.Conneted)
                {
                    UpdateState();
                    UpdateAlarm();
                }
                if (TouchManager.Instance.IsConnected())
                {
                    TouchManager.Instance.UpdateStatus(m_Status, m_Alarms);
                }
                m_SeqTouchUpdate.Do();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void UpdateObject()
        {
            if (TouchManager.Instance.IsConnected() && lbConnectStatus.BackColor != Color.LightGreen)
            {
                lbConnectStatus.BackColor = Color.LightGreen;
                lbConnectStatus.Text = "Connected";
            }
            else if (!TouchManager.Instance.IsConnected() && lbConnectStatus.BackColor != Color.LightGray)
            {
                lbConnectStatus.BackColor = Color.LightGray;
                lbConnectStatus.Text = "Disconnected";
            }
        }
        private void UpdateState()
        {
            try
            {
                ushort start_address = TouchManager.Instance.StatusStartAddress;
                Array.Clear(m_Status, 0, MAX_STATUS_COUNT);

                #region System Time
                DateTime dateTime = DateTime.Now;
                ushort[] regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dateTime.Year.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Year - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dateTime.Month.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Month - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dateTime.Day.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Day - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dateTime.Hour.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Hour - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dateTime.Minute.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Minute - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dateTime.Second.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Second - start_address, regVal.Length);
                #endregion

                #region System Status
                int mode = RemoteManager.TouchInstance.Remoting.TouchGUI.OpMode == OperateMode.Auto ? 1 : 0;
                int eqp_state = RemoteManager.TouchInstance.Remoting.TouchGUI.EqState == EqpState.Down ? 2 :
                    RemoteManager.TouchInstance.Remoting.TouchGUI.EqState == EqpState.Run ? 1 : 0;
                double velocity = RemoteManager.TouchInstance.Remoting.TouchGUI.Velocity;
                int up_area = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUpArea;
                int up1 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp1 ? 1 : 0;
                int up2 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp2 ? 1 : 0;
                int up3 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp3 ? 1 : 0;
                int up4 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsUp4 ? 1 : 0;
                int dn_area = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDownArea;
                int dn1 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown1 ? 1 : 0;
                int dn2 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown2 ? 1 : 0;
                int dn3 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown3 ? 1 : 0;
                int dn4 = RemoteManager.TouchInstance.Remoting.TouchGUI.ObsDown4 ? 1 : 0;
                string destination = RemoteManager.TouchInstance.Remoting.TouchGUI.Destination;
                destination = destination == "" ? "NULL" : destination;
                string carrierID = RemoteManager.TouchInstance.Remoting.TouchGUI.CarrierID;
                carrierID = carrierID == "" ? "NULL" : carrierID;
                int heart_bit_count = RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitCount;
                this.tbOpMode.Text = RemoteManager.TouchInstance.Remoting.TouchGUI.OpMode.ToString();
                this.tbEqState.Text = RemoteManager.TouchInstance.Remoting.TouchGUI.EqState.ToString();
                this.tbVelocity.Text = velocity.ToString("F2");
                this.tbUpObs.Text = up_area.ToString();
                this.tbUp1.Text = up1.ToString();
                this.tbUp2.Text = up2.ToString();
                this.tbUp3.Text = up3.ToString();
                this.tbUp4.Text = up4.ToString();
                this.tbDownObs.Text = dn_area.ToString();
                this.tbDown1.Text = dn1.ToString();
                this.tbDown2.Text = dn2.ToString();
                this.tbDown3.Text = dn3.ToString();
                this.tbDown4.Text = dn4.ToString();

                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, mode.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Mode - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, eqp_state.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Run - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.Double, velocity.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Velocity - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dn_area.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsDownArea - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dn1.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsDownOut1 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dn2.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsDownOut2 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dn3.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsDownOut3 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, dn4.ToString());
                //Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsDownOut4 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, up_area.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsUpArea - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, up1.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsUpOut1 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, up2.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsUpOut2 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, up3.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsUpOut3 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UShort, up4.ToString());
                //Array.Copy(regVal, 0, m_Status, (int)StatusAddress.ObsDownOut4 - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.String, destination);
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.Destination - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.String, carrierID);
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.CarrierID - start_address, regVal.Length);
                regVal = TouchManager.Instance.GetRegVal(AddressType.HoldRegister, DataType.UInt, heart_bit_count.ToString());
                Array.Copy(regVal, 0, m_Status, (int)StatusAddress.HeartBitCount - start_address, regVal.Length);
                #endregion
            }
            catch
            {

            }
        }

        private void UpdateAlarm()
        {
            try
            {
                #region Alarm
                Array.Clear(m_Alarms, 0, MAX_ALARM_COUNT);
                bool alarm = RemoteManager.TouchInstance.Remoting.TouchGUI.IsAlarm;
                string alarmIds = RemoteManager.TouchInstance.Remoting.TouchGUI.AlarmIds;
                if (alarmIds != string.Empty)
                {
                    m_Alarms[0] = 1; //触发Touch报警
                    string[] ids = alarmIds.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int index = 0;
                        if (int.TryParse(ids[i], out index))
                        {
                            if (index > 0 && index < MAX_ALARM_COUNT) m_Alarms[1] = (ushort)index;  //1001为Alarm编号，Touch中建立了字符串表，会根据ID显示相应的Alarm
                        }
                    }
                }
                else
                {
                    m_Alarms[0] = 0;
                }

                if (alarm) this.tbAlarm.Text = "ALARM";
                else this.tbAlarm.Text = "NONE";
                this.tbAlarmIds.Text = alarmIds;
                #endregion
            }
            catch
            {

            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnServoOff_Click(object sender, EventArgs e)
        {
            MxpManager.Instance.AllServoStop();
        }
        public void LogMessage(string str, bool log = true)
        {
            this.lbMessage.Text = str;
            if (log) TouchGUILog.WriteLog(str);
        }
        public void LogServoOff(string str)
        {
            this.lbServoOff.Text = str;
            TouchGUILog.WriteLog(str);
        }
        private class SeqTouchUpdate : XSeqFunc
        {
            #region Field
            public static readonly string FuncName = "[SeqTouchUpdate]";
            #endregion

            public SeqTouchUpdate()
            {
                this.SeqName = $"SeqTouchUpdate";
            }
            public override void SeqAbort()
            {
                this.InitSeq();
            }
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (TouchManager.Instance.IsConnected())
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks < TouchManager.Instance.ConnectRetryInterval * 1000.0f) break;
                            TouchManager.Instance.Connect();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            if (!TouchManager.Instance.IsConnected())
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (XFunc.GetTickCount() - StartTicks < TouchManager.Instance.SendInterval * 1000.0f) break;
                            if (TouchManager.Instance.SendQueueCount > 0)
                            {
                                TouchManager.Instance.SendMsg();
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 110;
                            }
                        }
                        break;

                    case 110:
                        {
                            StartTicks = XFunc.GetTickCount();
                            if (TouchManager.Instance.IsConnected())
                                seqNo = 100;
                            else seqNo = 0;
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
        }

        private class SeqHeartBit : XSeqFunc
        {
            #region Field
            public static readonly string FuncName = "[SeqHeartBit]";
            private int m_HeartBitCount = 0;
            private int m_RetryLimit = 5;
            private int m_RetryCount = 0;
            private bool m_HeartBitNgCheck = false;

            private MainForm m_MainForm = null;
            #endregion

            public SeqHeartBit(MainForm form)
            {
                this.SeqName = $"SeqHeartBit";
                m_MainForm = form;
            }
            public override void SeqAbort()
            {
                this.InitSeq();
            }
            public override int Do()
            {
                int seqNo = SeqNo;
                int rv = -1;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (RemoteManager.TouchInstance.Conneted)
                            {
                                m_MainForm.LogMessage(string.Format("Remote Connected"));
                                m_RetryCount = 0;
                                seqNo = 10;
                            }
                            else
                            {
                                m_MainForm.LogMessage(string.Format("Remote Disconnected"));

                                m_HeartBitNgCheck = false;
                                m_RetryCount = 0;
                                m_HeartBitCount = 0;

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                        }
                        break;

                    case 10:
                        {
                            int count = RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitCount;
                            if (count != m_HeartBitCount)
                            {
                                m_MainForm.LogMessage(string.Format("Heart Bit Count Changed [{0}=>{1}]", m_HeartBitCount, count), false);
                                m_HeartBitNgCheck = false;
                                m_RetryCount = 0;
                                m_HeartBitCount = count;
                                m_RetryLimit = TouchManager.Instance.HeartBitCheckTime / 200;
                            }
                            else if (RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitRun)
                            {
                                m_MainForm.LogMessage(string.Format("Heart Bit Count, Not Changed [{0}=>{1}]", m_HeartBitCount, count));
                                m_HeartBitNgCheck = true;                                
                            }
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 200.0f) break;
                            if (m_HeartBitNgCheck) m_RetryCount++;
                            if (m_RetryCount > m_RetryLimit) // 2sec over
                            {
                                // Servo Off
                                RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitAlarm = true;
                                MxpManager.Instance.AllServoStop();
                                m_MainForm.LogServoOff(string.Format("Axis All Servo OFF"));
                                seqNo = 30;
                            }
                            else
                            {
                                if (m_HeartBitNgCheck)
                                    m_MainForm.LogServoOff(string.Format("Heart Bit Count, Not Changed Retry[{0}]", m_RetryCount));
                                seqNo = 10;
                            }
                        }
                        break;

                    case 30:
                        {
                            bool run = RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitRun;
                            run &= RemoteManager.TouchInstance.Remoting.TouchGUI.HeartBitCount != m_HeartBitCount;
                            if (run)
                            {
                                m_MainForm.LogServoOff(string.Format("Heart Bit Run"));
                                seqNo = 10;
                            }
                        }
                        break;

                    case 100:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 1000.0f)
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return rv;
            }
        }

        private enFrontDetectState ReadOverride()
        {
            // SlaveNo : 5
            // ioType : MXP_IO_TYPE.IO_IN
            // Offset : 0
            // bit0 : RESET_SW
            // bit1 : UP1
            // bit2 : UP2
            // bit3 : UP3
            // bit4 : UP4
            // bit5 : DN1
            // bit6 : DN2
            // bit7 : DN3

            enFrontDetectState rv = m_UpperFrontState;
            try
            {
                UInt32 slaveNo = TouchManager.Instance.SlaveNo;
                MXP_IO_TYPE ioType = MXP_IO_TYPE.IO_IN;
                UInt16 offset = TouchManager.Instance.Offset;
                byte data = 0;
                MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.IO_ReadByte(slaveNo, ioType, offset, ref data);
                if (result == MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    ushort startAddress = TouchManager.Instance.StartAddress;
                    m_QueueOBS1.Enqueue(((data >> (startAddress + 0)) & 0x01) != 0x01 ? 1 : 0); // B 접점처리
                    if (m_QueueOBS1.Count > 3) m_QueueOBS1.Dequeue();
                    m_QueueOBS2.Enqueue(((data >> (startAddress + 1)) & 0x01) != 0x01 ? 1 : 0);
                    if (m_QueueOBS2.Count > 3) m_QueueOBS2.Dequeue();
                    m_QueueOBS3.Enqueue(((data >> (startAddress + 2)) & 0x01) != 0x01 ? 1 : 0);
                    if (m_QueueOBS3.Count > 3) m_QueueOBS3.Dequeue();
                    m_QueueOBS4.Enqueue(((data >> (startAddress + 3)) & 0x01) != 0x01 ? 1 : 0);
                    if (m_QueueOBS4.Count > 3) m_QueueOBS4.Dequeue();
                }

                double obs1 = m_QueueOBS1.Count > 0 ? m_QueueOBS1.Average() : 0.5;
                double obs2 = m_QueueOBS2.Count > 0 ? m_QueueOBS2.Average() : 0.5;
                double obs3 = m_QueueOBS3.Count > 0 ? m_QueueOBS3.Average() : 0.5;
                double obs4 = m_QueueOBS4.Count > 0 ? m_QueueOBS4.Average() : 0.5;

                bool ng = false;
                ng |= obs1 > 0 && obs1 < 1;
                ng |= obs2 > 0 && obs2 < 1;
                ng |= obs3 > 0 && obs3 < 1;
                ng |= obs4 > 0 && obs4 < 1;

                if (!ng)
                {
                    int front_up = 0;
                    front_up |= (int)obs1 == 1 ? 0x01 << 0 : 0x00;
                    front_up |= (int)obs2 == 1 ? 0x01 << 1 : 0x00;
                    front_up |= (int)obs3 == 1 ? 0x01 << 2 : 0x00;
                    front_up |= (int)obs4 == 1 ? 0x01 << 3 : 0x00;

                    bool ok = front_up >= 0 && front_up <= 8;
                    if (m_UpperFrontState == enFrontDetectState.enDeccelation8 && m_MasterVelocity > 10.0f)
                        ok &= (enFrontDetectState)front_up != enFrontDetectState.enNone;
                    if (ok)
                        rv = (enFrontDetectState)front_up;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return rv;
        }

        private double GetMasterVelocity()
        {
            double rv = 0.0f;
            try
            {
                // Master NodeId = 1
                uint nodeID = 1;
                float cmdVelocity = 0.0f;
                float setOverride = 0.1f;
                MXP.MXP_FUNCTION_STATUS_RESULT result = MXP.AX_ReadCommandVelocity(nodeID, ref cmdVelocity);
                if (result == MXP.MXP_FUNCTION_STATUS_RESULT.RT_NO_ERROR)
                {
                    rv = cmdVelocity;
                    float curOverride = 0;
                    result = MXP.AX_ReadVelocityOverrideFactor(nodeID, ref curOverride);

                    if (m_UpperFrontState >= enFrontDetectState.enDeccelation8)
                    {
                        if (m_StopOverride)
                        {
                            if (cmdVelocity < 10.0f && XFunc.GetTickCount() - m_StopSetTime > 1000)
                            {
                                m_StopOverride = false;
                                RemoteManager.TouchInstance.Remoting.TouchGUI.SetOverrideZero = true;
                            }
                        }
                        else if (cmdVelocity > 10.0f)
                        {
                            float setTime = 100.0f;
                            setOverride = 0.0f;
                            result = MXP.AX_SetOverride_Ex(nodeID, setOverride, setTime);
                            TouchGUILog.WriteLog(string.Format("TOUCH GUI, Abnormal Override SET #1 : setOverride={0}, setTime={1}, FrontState={2}, cmdVelocity={3}, curOverride={4}, Bit={5},{6},{7},{8}", setOverride, setTime, m_UpperFrontState, cmdVelocity, curOverride, m_QueueOBS1.Average(), m_QueueOBS2.Average(), m_QueueOBS3.Average(), m_QueueOBS4.Average()));

                            m_StopOverride = true;
                            m_StopSetTime = XFunc.GetTickCount();
                        }
                    }
                    else if (m_VelocityLimitTable.ContainsKey(m_UpperFrontState))
                    {
                        double limit_vel = m_VelocityLimitTable[m_UpperFrontState].X;
                        double limit_distance = m_VelocityLimitTable[m_UpperFrontState].Y;
                        if (cmdVelocity > limit_vel)
                        {
                            // override -> 0.1로 때리자 ~~~
                            if (curOverride > 0.9f)
                            {
                                float setTime = 500.0f;
                                double dt = Math.Sqrt((2 * (limit_distance - 300.0f)) / cmdVelocity);
                                //double dt = (limit_distance - 400.0f) / cmdVelocity;
                                if (dt < 0.1) dt = 0.1;
                                else if (dt > 2.0) dt = 2.0;
                                setTime = (float)(1000.0f * dt);
                                result = MXP.AX_SetOverride_Ex(nodeID, setOverride, setTime);
                                TouchGUILog.WriteLog(string.Format("TOUCH GUI, Abnormal Override SET #2 : setOverride={0}, setTime={1}, FrontState={2}, cmdVelocity={3}, curOverride={4}, Bit={5},{6},{7},{8}", setOverride, setTime, m_UpperFrontState, cmdVelocity, curOverride, m_QueueOBS1.Average(), m_QueueOBS2.Average(), m_QueueOBS3.Average(), m_QueueOBS4.Average()));
                            }
                        }
                    }
                    else if (XFunc.GetTickCount() - m_StopSetTime > 1000)
                    {
                        if (m_UpperFrontState < enFrontDetectState.enDeccelation8) m_StopOverride = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
    }
}
