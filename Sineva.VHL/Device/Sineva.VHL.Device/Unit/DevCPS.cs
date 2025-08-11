using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using static Sineva.VHL.Device.Assem._DevBCR;

namespace Sineva.VHL.Device
{
    #region Enums
    public enum Section : int
    {
        s = 0,          // Straight
        c,              // Curve
        b,              // Branch
    }
    public enum Travel : int
    {
        s = 0,          // Stop
        r,              // Run
    }
    #endregion
    [Serializable]
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    public class DevCPS : _Device
    {
        private const string DevName = "DevCPS";

        #region Fields
        private XComm m_Comm = new XComm();
        //private byte[] m_ReadBuffer = new byte[1024];
        private List<byte> m_RecvList = new List<byte>();
        private int m_ReadIntervalTime = 1000; // 1000 msec
        private SeqMonitor m_SeqMonitor;
        private bool m_CpsMonitoring = false;

        private AlarmData m_ALM_NotDefine;
        private AlarmData m_ALM_ConnectFailed;
        private AlarmData m_ALM_ReadTagTimeout;
        private Dictionary<string, string> m_CPSValues = new Dictionary<string, string>();
        private int m_BoostVoltage = 0;
        private int m_OutputVoltage = 0;
        private double m_BoostCurrent = 0;
        private double m_OutputCurrent = 0;
        private int m_HeatSinkTemp = 0;
        private int m_InternalNTCTemp = 0;
        private int m_PickupNTCTemp = 0;
        #endregion

        #region Properties
        [Category("Configuration"), Description("Comm"), DeviceSetting(true)]
        public XComm Comm
        {
            get { return m_Comm; }
            set { m_Comm = value; }
        }
        [Category("Configuration"), Description("Read Interval Time (msec)"), DeviceSetting(true)]
        public int ReadIntervalTime
        {
            get { return m_ReadIntervalTime; }
            set { m_ReadIntervalTime = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int BoostVoltage
        { 
            get { return m_BoostVoltage; } 
            set { m_BoostVoltage = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int OutputVoltage
        {
            get { return m_OutputVoltage; }
            set { m_OutputVoltage = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_NotDefine
        {
            get { return m_ALM_NotDefine; }
            set { m_ALM_NotDefine = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ConnectFailed
        {
            get { return m_ALM_ConnectFailed; }
            set { m_ALM_ConnectFailed = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public AlarmData ALM_ReadTagTimeout
        {
            get { return m_ALM_ReadTagTimeout; }
            set { m_ALM_ReadTagTimeout = value; }
        }
        public double BoostCurrent
        {
            get { return m_BoostCurrent; }
            set { m_BoostCurrent = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public int HeatSinkTemp
        {
            get { return m_HeatSinkTemp; }
            set { m_HeatSinkTemp = value; }
        }
        #endregion

        #region Constructor
        public DevCPS()
        {
            if (!Initialized)
            {
                this.MyName = DevName;
            }
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
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            ALM_NotDefine = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, MyName, ParentName, "Not Define Alarm");
            ALM_ConnectFailed = AlarmListProvider.Instance.NewAlarm(AlarmCode.DataIntegrity, false, MyName, ParentName, "Connect Failed");
            ALM_ReadTagTimeout = AlarmListProvider.Instance.NewAlarm(AlarmCode.DataIntegrity, false, MyName, ParentName, "Read Value Timeout");
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            //ok &= Connect();
            m_Comm.Initialize();
            m_Comm.ReceivedData += new XComm.ReceivedDataEventHandler(OnReceivedData);

            GV.LifeTimeItems.Add(new GeneralObject($"{ParentName}.{MyName}", "Create Time", this, "GetLifeTime", 1000));
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            if (ok)
            {
                m_SeqMonitor = new SeqMonitor(this);
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 6. Initialize 마무으리
            m_CPSValues.Add("Status", "");
            m_CPSValues.Add("BoostVoltage", "");
            m_CPSValues.Add("OutputVoltage", "");
            m_CPSValues.Add("BoostCurrent", "");
            m_CPSValues.Add("OutputCurrent", "");
            m_CPSValues.Add("HeatSinkTemp", "");
            m_CPSValues.Add("InternalNTCTemp", "");
            m_CPSValues.Add("PickupNTCTemp", "");
            m_CPSValues.Add("ErrorCode", "");
            m_CPSValues.Add("AcqTime", "");

            Initialized = true;
            Initialized &= ok;
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            return Initialized;
        }

        #endregion

        #region Methods
        public bool IsMonitoring()
        {
            return m_CpsMonitoring;
        }
        public void OpenPort()
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return;
            if (AppConfig.Instance.Simulation.SERIAL) return;
            if (m_Comm._CommPortNo == PortNo._Emulate) return;

            m_CpsMonitoring = false;
            m_Comm.Open(); //CPS : COM1
            m_Comm.FlushInBuffer();
            m_Comm.FlushOutBuffer();
        }
        public void ClosePort()
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return;
            if (!Initialized) return;

            m_CpsMonitoring = false;
            m_Comm.Close();
        }
        public bool IsOpen()
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;

            bool rv = m_Comm.IsOpen();
            if (!rv) m_CpsMonitoring = false;
            return rv;
        }
        private void OnReceivedData(object sender)
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return;
            if (!Initialized) return;

            try
            {
                if (m_Comm.SerialPort.IsOpen)
                {
                    if (m_Comm.SerialPort.BytesToRead > 0)
                    {
                        byte[] m_ReadBuffer = new byte[m_Comm.SerialPort.BytesToRead];
                        m_Comm.SerialPort.Read(m_ReadBuffer, 0, m_ReadBuffer.Length);
                        //byte[] bytRcv = new byte[count];
                        //Array.Copy(m_ReadBuffer, bytRcv, count);
                        MakeData(m_ReadBuffer);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void MakeData(byte[] param)
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return;
            if (!Initialized) return;

            try
            {
                if (m_RecvList.Count == 0)
                {
                    if (param[0] == 0x02)
                    {
                        m_RecvList.AddRange(param);
                    }
                }
                else
                {
                    m_RecvList.AddRange(param);
                }
                if (m_RecvList.Count >= 40)
                {
                    if (m_RecvList.Count == 40 && m_RecvList[m_RecvList.Count - 1] == 0x03)
                    {
                        ParsingReceivedData(m_RecvList.ToArray());
                    }
                    m_RecvList.Clear();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void ParsingReceivedData(byte[] data)
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return;
            if (!Initialized) return;

            try
            {
                byte checkSum = data[38];
                byte[] rcvValData = new byte[37];
                Array.Copy(data, 1, rcvValData, 0, 37);
                if (CheckSum(rcvValData) != checkSum)
                {
                    //Check Error
                    return;
                }
                string strRcv = Encoding.ASCII.GetString(rcvValData);
                string status = strRcv.Substring(0, 3);
                string boostVoltage = strRcv.Substring(3, 4);   
                string outputVoltage = strRcv.Substring(7, 4);
                string boostCurrent = strRcv.Substring(11, 4);
                string outputCurrent = strRcv.Substring(15, 4);
                string heatSinkTemp = strRcv.Substring(19, 4);
                string internalNTCTemp = strRcv.Substring(23, 4);
                string pickupNTCTemp = strRcv.Substring(27, 4);
                string errorCode = strRcv.Substring(35, 2);

                m_CPSValues["Status"] = status;
                m_CPSValues["BoostVoltage"] = boostVoltage;
                m_CPSValues["OutputVoltage"] = outputVoltage;
                m_CPSValues["BoostCurrent"] = string.Format("{0}", Convert.ToInt32(boostCurrent) / 10.0);
                m_CPSValues["OutputCurrent"] = string.Format("{0}", Convert.ToInt32(outputCurrent) / 10.0);
                m_CPSValues["HeatSinkTemp"] = string.Format("{0}", Convert.ToInt32(heatSinkTemp) / 10);
                m_CPSValues["InternalNTCTemp"] = string.Format("{0}", Convert.ToInt32(internalNTCTemp) / 10);
                m_CPSValues["PickupNTCTemp"] = string.Format("{0}", Convert.ToInt32(pickupNTCTemp) / 10);
                m_CPSValues["ErrorCode"] = errorCode;
                m_CPSValues["AcqTime"] = DateTime.Now.ToString("yyyyMMdd_HH:mm:ss");

                int temp0 = 0;
                m_BoostVoltage = int.TryParse(boostVoltage, out temp0) ? temp0 : 0;
                m_OutputVoltage = int.TryParse(outputVoltage, out temp0) ? temp0 : 0;
                m_HeatSinkTemp = int.TryParse(heatSinkTemp, out temp0) ? temp0 : 0;
                m_InternalNTCTemp = int.TryParse(internalNTCTemp, out temp0) ? temp0 : 0;
                m_PickupNTCTemp = int.TryParse(pickupNTCTemp, out temp0) ? temp0 : 0;
                double temp1 = 0.0f;
                m_BoostCurrent = double.TryParse(boostCurrent, out temp1) ? temp1 : 0.0f;
                m_OutputCurrent = double.TryParse(outputCurrent, out temp1) ? temp1 : 0.0f;

                m_CpsMonitoring = true;
                string msg = $"{status},{boostVoltage},{outputVoltage},{boostCurrent},{outputCurrent},{heatSinkTemp},{internalNTCTemp},{pickupNTCTemp},{errorCode}";
                CpsLog.WriteLog(string.Format("RECV : {0}", msg));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public bool SendCommand()
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;

            string DateTimeData = DateTime.Now.ToString("yyMMddHHmmss");
            string SectionTravelData = GetSectionTravelData();
            string CommandData = string.Format("R{0}{1}", DateTimeData, SectionTravelData);
            SendCmd(CommandData);
            CpsLog.WriteLog(string.Format("SEND : {0}", CommandData));
            return true;
        }
        private bool SendCmd(string command)
        {
            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.NoUse) return false;
            if (!Initialized) return false;

            try
            {
                byte[] byt = Encoding.ASCII.GetBytes(command);
                byte checksum = CheckSum(byt);
                byte[] sendMsg = new byte[18];
                sendMsg[0] = 0x02;
                Array.Copy(byt, 0, sendMsg, 1, 15);
                sendMsg[16] = checksum;
                sendMsg[17] = 0x03;
                m_Comm.Write(sendMsg, 0, sendMsg.Length);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
                return false;
            }
            return true;
        }
        public byte CheckSum(byte[] inputData)
        {
            ushort USValue = 0;
            for (int i = 0; i < inputData.Length; i++)
            {
                USValue += inputData[i];
            }
            byte checkSum = (byte)(USValue & 0xff);
            return checkSum;
        }
        private string GetSectionTravelData()
        {
            string SectionTravelData = string.Empty;
            Section sectionData = Section.s;
            Travel travelData = Travel.s;
            try
            {
                LinkType curLinkType = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type;
                travelData = GV.WheelBusy == false ? Travel.s : Travel.r;
                switch (curLinkType)
                {
                    case LinkType.LeftCurve:
                    case LinkType.RightCurve:
                        sectionData = Section.c;
                        break;
                    case LinkType.LeftBranch:
                    case LinkType.RightBranch:
                    case LinkType.LeftSBranch:
                    case LinkType.RightSBranch:
                    case LinkType.LeftJunction:
                    case LinkType.RightJunction:
                    case LinkType.LeftSJunction:
                    case LinkType.RightSJunction:
                    case LinkType.LeftCompositedSCurveBranch:
                    case LinkType.RightCompositedSCurveBranch:
                    case LinkType.LeftCompositedSCurveJunction:
                    case LinkType.RightCompositedSCurveJunction:
                        sectionData = Section.b;
                        break;
                    case LinkType.Straight:
                    case LinkType.LeftBranchStraight:
                    case LinkType.RightBranchStraight:
                    case LinkType.LeftJunctionStraight:
                    case LinkType.RightJunctionStraight:
                    case LinkType.Ascent:
                    case LinkType.Descent:
                    case LinkType.JunctionStraight:
                    case LinkType.SideStraight:
                    case LinkType.SideLeftJunctionStraight:
                    case LinkType.SideRightJunctionStraight:
                    default:
                        sectionData = Section.s;
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            finally
            {
                SectionTravelData = sectionData.ToString() + travelData.ToString();
            }
            return SectionTravelData;
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            public static readonly string FuncName = "[SeqMonitor]";
            private DevCPS m_Device = null;
            #endregion

            #region Constructor
            public SeqMonitor(DevCPS device)
            {
                this.SeqName = $"SeqMonitor{device.MyName}";
                m_Device = device;
                TaskCpsSerialControl.Instance.RegSeq(this);
            }
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                this.InitSeq();
            }
            #endregion

            #region override
            public override int Do()
            {
                if (!m_Device.Initialized) return -1;

                int rv = -1;
                int seqNo = this.SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (SetupManager.Instance.SetupSafty.CpsVoltageCheckUse == Use.Use)
                            {
                                if (m_Device.IsOpen())
                                {
                                    seqNo = 100;
                                }
                                else
                                {
                                    seqNo = 10;
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            m_Device.OpenPort();
                            SequenceDeviceLog.WriteLog(FuncName, "CPS Comm Begin Connect", seqNo);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        break;
                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 5 * 1000) break;
                            if (m_Device.IsOpen())
                            {
                                SequenceDeviceLog.WriteLog(FuncName, "CPS Comm Connected", seqNo);
                                seqNo = 0;
                                rv = 0;
                            }
                            else
                            {
                                SequenceDeviceLog.WriteLog(FuncName, "CPS Comm Disconnected", seqNo);
                                seqNo = 10;
                            }
                        }
                        break;
                    case 100:
                        {
                            m_Device.SendCommand();
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        break;
                    case 110:
                        {
                            if (XFunc.GetTickCount() - StartTicks > m_Device.ReadIntervalTime)
                            {
                                seqNo = 100;
                            }
                            else if (m_Device.IsOpen() == false)
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
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

                var helperXml = new XmlHelper<DevCPS>();
                DevCPS dev = helperXml.Read(fileName);
                if (dev != null)
                {
                    this.IsValid = dev.IsValid;
                    this.DeviceId = dev.DeviceId;
                    this.DeviceStartTime = dev.DeviceStartTime;
                    if (this.ParentName == "") this.ParentName = dev.ParentName;
                    this.MyName = dev.MyName;
                    this.Comm = dev.Comm;
                    this.ReadIntervalTime = dev.ReadIntervalTime;
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
                var helperXml = new XmlHelper<DevCPS>();
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
