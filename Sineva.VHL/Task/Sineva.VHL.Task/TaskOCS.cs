using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Data.Alarm;
using System.Security.Claims;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Device;
using Sineva.VHL.Data.Process;
using System.Xml;
using System.Xml.Linq;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Data.PathFinder;
using System.Security.Cryptography;
using Sineva.VHL.Data.LogIn;
using System.Diagnostics;
using Sineva.VHL.Library.Servo;
using static Sineva.VHL.Task.TaskMonitor;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.Library.SimpleWifi;

namespace Sineva.VHL.Task
{
    public class TaskOCS : XSequence
    {
        #region Field
        public static readonly TaskOCS Instance = new TaskOCS();
        #endregion

        #region Constructor
        public TaskOCS()
        {
            this.RegSeq(new SeqLogin());
            this.RegSeq(new SeqStatusReport());
            this.RegSeq(new SeqAlarmReport());
            this.RegSeq(new SeqEventReport());
            this.RegSeq(new SeqOcsCommonProcess());
            this.RegSeq(new SeqOcsCommandProcess());
            this.RegSeq(new SeqOcsMapDataProcess());
        }
        #endregion

        #region Methods
        public class SeqLogin : XSeqFunc
        {
            private const string FuncName = "[SeqLogin]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private bool m_LoginReply = false;
            private string m_RequestUserID = string.Empty;
            private string m_RequestPassword = string.Empty;
            #endregion

            #region Property
            #endregion

            #region Contructor
            public SeqLogin()
            {
                this.SeqName = $"SeqLogin";

                m_OCSManager = OCSCommManager.Instance;
                m_OCSManager.OcsStatus.delMessageReceived += OcsStatus_delMessageReceived;
                EventHandlerManager.Instance.LoginPermissionRequest += Instance_LoginPermissionRequest;
            }

            private void Instance_LoginPermissionRequest(int key, string[] values)
            {
                try
                {
                    // LogIn / LogOut : : m_PermissionKey = -1 (LogIn = 0 / LogOut = -1)
                    if (key == 0)
                    {
                        m_RequestUserID = values[0];
                        m_RequestPassword = values[1];
                        VehicleIF_UserLoginRequest message = new VehicleIF_UserLoginRequest
                        {
                            UserID = m_RequestUserID,
                            Password = m_RequestPassword,
                        };
                        m_OCSManager.OcsComm.SendIFMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }

            private void OcsStatus_delMessageReceived(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_UserLoginReply))
                    {
                        m_LoginReply = true;
                        VehicleIF_UserLoginReply message = (obj2 as VehicleIF_UserLoginReply);
                        if (message.Result == UserLoginResult.Success)
                        {
                            DataItem_UserInfo findUser = new DataItem_UserInfo();
                            findUser.UserName = m_RequestUserID;
                            findUser.Level = message.Level;
                            findUser.Password = m_RequestPassword;
                            findUser.Department = string.Empty;
                            findUser.Description = string.Empty;
                            AccountManager.Instance.ChangeAccount(findUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                int nRv = 0;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_LoginReply)
                            {
                                m_LoginReply = false;
                                AppConfig.Instance.OperatorLoginUserID = m_RequestUserID;
                            }
                        }
                        break;

                    case 10:
                        {
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return nRv;
            }
            #endregion
        }
        public class SeqStatusReport : XSeqFunc
        {
            private const string FuncName = "[SeqOcsStatusReport]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private bool m_StatusDataSendReply = false;
            private uint m_StartNgTicks = 0;
            private AlarmData m_ALM_OCS_Disconnect = null;
            private bool m_NewConnection = false;
            private int m_StatusResponseRetry = 0;
            private int m_ConnectionRetry = 0;
            #endregion

            #region Property
            #endregion

            #region Constructor
            public SeqStatusReport()
            {
                this.SeqName = "SeqOcsStatusReport";
                m_OCSManager = OCSCommManager.Instance;
                m_OCSManager.OcsStatus.delStatusSendReply += OcsStatus_delStatusSendReply;
                m_OCSManager.OcsStatus.delSocketClose += OcsStatus_delSocketClose;
                m_ALM_OCS_Disconnect = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "OCSConnection", "OCS", "Disconnect Alarm");
            }
            private void OcsStatus_delSocketClose(object obj)
            {
                this.SeqNo = 0;
            }
            private void OcsStatus_delStatusSendReply(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_VehicleStatusDataReply))
                    {
                        m_StatusDataSendReply = true;
                        //Debug.WriteLine(string.Format("{0} : Status Reply#2", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (m_OCSManager.OcsComm.OcsUse == false) return -1;

                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            bool network_connected = true;
                        	if (SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use)
							{
                            	network_connected &= NetworkManager.Instance.NetworkUseEnable;
								network_connected |= EqpStateManager.Instance.OpMode != OperateMode.Auto; //Manual 동작 진행하다가 Network가 끊기면 위험하다..특히 Jog이동..
								network_connected |= EqpStateManager.Instance.RunMode != EqpRunMode.Start; //Manual 동작 진행하다가 Network가 끊기면 위험하다..특히 Jog이동..
							}
                            if (network_connected)
                            {
                                if (m_OCSManager.OcsStatus.Connected && !m_OCSManager.OcsStatus.ConnectError)
                                {
                                    m_StatusDataSendReply = false;
                                    VehicleIF_VehicleStatusDataSend message = m_OCSManager.OcsStatus.GetStatusSendMessage();
                                    if (message != null) m_OCSManager.OcsComm.SendIFMessage(message);
                                    //Debug.WriteLine(string.Format("{0} : Status Report", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")));

                                    StartTicks = XFunc.GetTickCount();
                                    m_StartNgTicks = XFunc.GetTickCount();
                                    seqNo = 10;
                                }
                                else if (m_OCSManager.OcsStatus.ConnectError)
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 120;
                                }
                            }
                            else if (SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use)
                            {
                                string msg = string.Format("Network Disconnect Status");
                                SequenceJCSLog.WriteLog(FuncName, msg, seqNo);

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 200;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_StatusDataSendReply)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 20 * 1000)
                            {
                                string msg = string.Format("OCS Status Not Response - Disconnected process");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 100;
                            }
                            else if (XFunc.GetTickCount() - m_StartNgTicks > 3 * 1000)
                            {
                                string msg = string.Format("OCS Status Reply - Not Response");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                                m_StatusDataSendReply = false;
                                VehicleIF_VehicleStatusDataSend message = m_OCSManager.OcsStatus.GetStatusSendMessage();
                                if (message != null) 
                                    m_OCSManager.OcsComm.SendIFMessage(message);

                                m_StartNgTicks = XFunc.GetTickCount();
                            }
                            else if (m_OCSManager.OcsStatus.Connected == false || m_OCSManager.OcsStatus.ConnectError)
                            {
                                string msg = string.Format("Disconnected");
                                if (m_OCSManager.OcsStatus.ConnectError == true)
                                    msg = string.Format("ConnectError");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupOCS.StatusReportIntervalTime)
                            {
                                m_ConnectionRetry = 0;
                                m_StatusResponseRetry = 0;
                                m_NewConnection = false;
                                seqNo = 0;
                            }
                        }
                        break;

                    case 100:
                        {
                            m_OCSManager.DisconnectRequest = true;
                            string msg = string.Format("Socket Disconnection");
                            SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        break;

                    case 110:
                        {
                            if (m_OCSManager.OcsStatus.Connected == false)
                            {
                                string msg = string.Format("Socket Disconnected");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                StartTicks = XFunc.GetTickCount();

                                AlarmId = m_ALM_OCS_Disconnect.ID;
                                EqpAlarm.Set(AlarmId);

                                seqNo = 120;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                            {
                                string msg = string.Format("Socket Disconnection Timeout Retry");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 100;
                            }
                        }
                        break;

                    case 120:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            m_OCSManager.ConnectRequest = true;
                            string msg = string.Format("Socket Connection");
                            SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 130;
                        }
                        break;

                    case 130:
                        {
                            if (m_OCSManager.OcsStatus.Connected)
                            {
								EqpAlarm.Reset(AlarmId);
                                string msg = string.Format("Socket Connected : {0}, {1}", m_NewConnection, m_StatusResponseRetry);
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                m_ConnectionRetry = 0;
                                m_StatusResponseRetry++;
                                if (SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use)
                                {
                                    if (m_NewConnection) // 왜 False가 않되었지 ? Status Report가 되지 않는 군...
                                    {
                                        // Network Restart 후 다시 시작....
                                        if (m_StatusResponseRetry > SetupManager.Instance.SetupOCS.NetworkRestartFailCount) seqNo = 200;
                                        else seqNo = 0;
                                    }
                                    else
                                    {
                                        m_NewConnection = true;
                                        seqNo = 0;
                                    }
                                }
                                else
                                {
                                    seqNo = 0;
                                }
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                            {
                                m_ConnectionRetry++;
                                string msg = string.Format("Socket Connection Timeout Retry [{0}]", m_ConnectionRetry);
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
								bool network_connected = true;
								network_connected &= SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use;
								network_connected &= EqpStateManager.Instance.OpMode == OperateMode.Auto; //Manual 동작 진행하다가 Network가 끊기면 위험하다..특히 Jog이동..
								network_connected &= EqpStateManager.Instance.RunMode == EqpRunMode.Start; //Manual 동작 진행하다가 Network가 끊기면 위험하다..특히 Jog이동..
                            	if (network_connected)
                                {
                                    if (m_ConnectionRetry > SetupManager.Instance.SetupOCS.NetworkRestartFailCount) seqNo = 200;
                                    else seqNo = 120;
                                }
                                else
                                {
                                    seqNo = 120;
                                }
                            }
                        }
                        break;

                    case 200:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto && EqpStateManager.Instance.RunMode == EqpRunMode.Start) //Manual 동작 진행하다가 Network가 끊기면 위험하다..
                            {
                                m_OCSManager.DisconnectRequest = true;
                                string msg = string.Format("Socket Connect NG, Socket Disconnection, {0}, {1}", m_ConnectionRetry, m_StatusResponseRetry);
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 210;
                            }
                            else
                                seqNo = 0;
                        }
                        break;

                    case 210:
                        {
                            if (m_OCSManager.OcsStatus.Connected == false)
                            {
                                string msg = string.Format("Socket Connect NG, Socket Disconnected");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 220;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                            {
                                string msg = string.Format("Socket Connect NG, Socket Disconnection Timeout Retry");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 220;
                            }
                        }
                        break;

                    case 220:
                        {
                            m_ConnectionRetry = 0;
                            m_StatusResponseRetry = 0;
                            m_NewConnection = false;
                            NetworkManager.Instance.NetworkRestart();
                            string msg = string.Format("Network Restart");
                            SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 230;
                        }
                        break;

                    case 230:
                        {
                            if (NetworkManager.Instance.NetworkUseEnable)
                            {
                                string msg = string.Format("Network Ready OK");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 30 * 1000)
                            {
                                string msg = string.Format("Network Ready Timeout Retry");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 220;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
        public class SeqAlarmReport : XSeqFunc
        {
            private const string FuncName = "[SeqAlarmReport]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private bool m_AlarmEventSendReply = false;
            private List<int> m_AutoSetAlarmIds = new List<int>();
            #endregion

            #region Property
            #endregion

            #region Contructor
            public SeqAlarmReport()
            {
                this.SeqName = "SeqAlarmReport";

                m_OCSManager = OCSCommManager.Instance;
                m_OCSManager.OcsStatus.delAlarmReportReply += OcsStatus_delAlarmReportReply;
                EventHandlerManager.Instance.OperateModeChanged += Instance_OperateModeChanged;
            }

            private void Instance_OperateModeChanged(OperateMode opMode)
            {
                try
                {
                    if (opMode == OperateMode.Manual)
                    {
                        // All Cur-Alarm Report
                        AlarmCurrentData[] alarms = AlarmCurrentProvider.Instance.GetCurrentAlarms();
                        if (alarms.Length > 0)
                        {
                            for (int i = 0; i < alarms.Length; i++)
                            {
                                VehicleIF_AlarmEventSend message = new VehicleIF_AlarmEventSend();
                                message.AlarmID = alarms[i].Id;
                                message.AlarmCode = (int)alarms[i].Code;
                                message.AlarmStatus = AlarmSetCode.Reset;
                                message.AlarmType = alarms[i].Level == AlarmLevel.S ? AlarmType.Alarm : AlarmType.Warning;
                                m_OCSManager.OcsComm.SendIFMessage(message);

                                string msg = string.Format("Manual Mode Change! AlarmEventSend : Alarm{0} AlarmID={1}, AlarmCode={2}, Text={3}", AlarmSetCode.Reset.ToString(), alarms[i].Id, alarms[i].Code, alarms[i].Name);
                                SequenceOCSLog.WriteLog(FuncName, msg);
                            }
                            m_AlarmEventSendReply = false;
                        }
                        m_AutoSetAlarmIds.Clear();
                    }
                }
                catch (Exception ex) 
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }

            private void OcsStatus_delAlarmReportReply(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_AlarmEventReply))
                    {
                        m_AlarmEventSendReply = true;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (m_OCSManager.OcsComm.OcsUse == false) return -1;

                int seqNo = this.SeqNo;
                int nRv = 0;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto && EqpStateManager.Instance.RunMode != EqpRunMode.Stop)//Alarm 발생 시 Abort상태로 먼저 빠지는 경우가 있어 Alarm Set 누락 방지..
                            {
                                if (EqpAlarm.EqpAlarmItems.Count > 0)
                                {
                                    EqpAlarm.AlarmItem item = new EqpAlarm.AlarmItem();
                                    item = EqpAlarm.EqpAlarmItems.Dequeue();

                                    AlarmData alarm = AlarmListProvider.Instance.GetAlarm(item.AlarmId);
                                    if (alarm != null)
                                    {
                                        m_AlarmEventSendReply = false;
                                        VehicleIF_AlarmEventSend message = new VehicleIF_AlarmEventSend();
                                        message.AlarmID = alarm.ID;
                                        message.AlarmCode = (int)alarm.Code;
                                        message.AlarmStatus = item.Condition == AlarmCondition.AlarmSet ? AlarmSetCode.Set : AlarmSetCode.Reset;
                                        message.AlarmType = alarm.Level == AlarmLevel.S ? AlarmType.Alarm : AlarmType.Warning;
                                        m_OCSManager.OcsComm.SendIFMessage(message);

                                        string msg = string.Format("AlarmEventSend : {0} AlarmID={1}, AlarmCode={2}, Text={3}", item.Condition.ToString(), alarm.ID, alarm.Code, alarm.Name);
                                        SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                                        if (item.Condition == AlarmCondition.AlarmSet) m_AutoSetAlarmIds.Add(alarm.ID);
                                        else if (m_AutoSetAlarmIds.Contains(alarm.ID)) m_AutoSetAlarmIds.Remove(alarm.ID);

                                        StartTicks = XFunc.GetTickCount();
                                        seqNo = 10;
                                    }
                                    SetAlarmLog(item.Condition, alarm);
                                }
                            }
                            else
                            {
                                if (EqpAlarm.EqpAlarmItems.Count > 0)
                                {
                                    EqpAlarm.AlarmItem item = new EqpAlarm.AlarmItem();
                                    item = EqpAlarm.EqpAlarmItems.Dequeue();
                                    AlarmData alarm = AlarmListProvider.Instance.GetAlarm(item.AlarmId);
                                    if (alarm != null)
                                    {
                                        if (item.Condition == AlarmCondition.AlarmReset && m_AutoSetAlarmIds.Contains(alarm.ID))
                                        {
                                            // Auto일때 발생한 Alarm인 경우 OCS에게 보고하자.
                                            m_AlarmEventSendReply = false;
                                            VehicleIF_AlarmEventSend message = new VehicleIF_AlarmEventSend();
                                            message.AlarmID = alarm.ID;
                                            message.AlarmCode = (int)alarm.Code;
                                            message.AlarmStatus = item.Condition == AlarmCondition.AlarmSet ? AlarmSetCode.Set : AlarmSetCode.Reset;
                                            message.AlarmType = alarm.Level == AlarmLevel.S ? AlarmType.Alarm : AlarmType.Warning;
                                            m_OCSManager.OcsComm.SendIFMessage(message);

                                            string msg = string.Format("AlarmEventSend : {0} AlarmID={1}, AlarmCode={2}, Text={3}", item.Condition.ToString(), alarm.ID, alarm.Code, alarm.Name);
                                            SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                                            m_AutoSetAlarmIds.Remove(alarm.ID);
                                            StartTicks = XFunc.GetTickCount();
                                            seqNo = 10;
                                        }
                                        else
                                        {
                                            string msg = string.Format("AlarmEventManual : {0} AlarmID={1}, AlarmCode={2}, Text={3}", item.Condition.ToString(), alarm.ID, alarm.Code, alarm.Name);
                                            SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                        }
                                    }
                                    SetAlarmLog(item.Condition, alarm);
                                }
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_AlarmEventSendReply)
                            {
                                m_AlarmEventSendReply = false;
                                string msg = string.Format("AlarmEventSend Reply OK");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 100)
                            {
                                string msg = string.Format("AlarmEventSend Not Response");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return nRv;
            }

            private void SetAlarmLog(AlarmCondition condition, AlarmData alarm)
            {
                try
                {
                    string message = "\r\n";
                    message += $"[Alarm Information]";
                    if(alarm != null)
                        message += $"\t{condition} | {alarm.ID} | {alarm.Code} | {alarm.Unit} | {alarm.Name} | {alarm.Level} | {alarm.Comment}\r\n";

                    message += "[Transfer Command]\r\n";
                    message += $"\tCommandID=[{ProcessDataHandler.Instance.CurTransferCommand.CommandID}], " +
                        $"SourceID=[{ProcessDataHandler.Instance.CurTransferCommand.SourceID}], " +
                        $"DestinationID=[{ProcessDataHandler.Instance.CurTransferCommand.DestinationID}]," +
                        $"CommandStatus=[{ProcessDataHandler.Instance.CurTransferCommand.CommandStatus}], " +
                        $"VehicleState=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState}]\r\n";
                    if (ProcessDataHandler.Instance.CurTransferCommand.PathMaps != null && ProcessDataHandler.Instance.CurTransferCommand.PathMaps.Count > 0)
                    {
                        foreach (Sineva.VHL.Data.Process.Path path in ProcessDataHandler.Instance.CurTransferCommand.PathMaps) message += $"\t{path}\r\n";
                    }

                    message += "[EQP Information]\r\n";
                    message += $"\tEQP State : {EqpStateManager.Instance.OpMode}|{EqpStateManager.Instance.RunMode}|{EqpStateManager.Instance.State}\r\n";
                    message += $"\tEqpInitComp={EqpStateManager.Instance.EqpInitComp}\r\n";

                    message += "[Servo Information]\r\n";
                    foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
                    {
                        foreach (_Axis axis in servo.Axes)
                        {
                            IAxisCommand axisCmd = (axis as IAxisCommand);
                            message += $"\t{servo.ServoName}_{axis.AxisName} : {axisCmd.GetAxisCurStatus()}, Pos={axisCmd.GetAxisCurPos()}, Speed={axisCmd.GetAxisCurSpeed()}, Torque={axisCmd.GetAxisCurTorque()}, {axis.AxisStateMsg}\r\n";
                        }
                    }

                    if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Acquiring || ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Depositing)
                    {
                        message += "[Teaching Information]\r\n";
                        message += $"\tPortID=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID}], " +
                            $"LeftBcr TeachingData=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.BarcodeLeft + ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.DriveLeftOffset}], " +
                            $"RightBcr TeachingData=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.BarcodeRight + ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.DriveRightOffset}], " +
                            $"Hoist TeachingData=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.HoistPosition + ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.HoistOffset}], " +
                            $"Slide TeachingData=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.SlidePosition + ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.SlideOffset}], " +
                            $"Rotate TeachingData=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.RotatePosition + ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.RotateOffset}]\r\n " +
                        $"\tCurrent PortID=[{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID}], " +
                        $"Current LeftBcr=[{ProcessDataHandler.Instance.Tpd[enTPD.CurLeftBCR]}], " +
                        $"Current RightBcr=[{ProcessDataHandler.Instance.Tpd[enTPD.CurRightBCR]}], " +
                        $"UpSensorDetectPosition=[{Convert.ToDouble(ProcessDataHandler.Instance.Tpd[enTPD.HoistPosition]) - SetupManager.Instance.SetupHoist.HoistSensorDetectMoveDistance:F2}], " +
                        $"Current SlidePosition=[{ProcessDataHandler.Instance.Tpd[enTPD.SlidePosition]}], " +
                        $"Current RotatePosition=[{ProcessDataHandler.Instance.Tpd[enTPD.RotatePosition]}]\r\n";
                    }
                    message += "[IO Information]\r\n";
                    message += $"\tINPUT : {string.Join(",", OCSCommManager.Instance.OcsStatus.InputStatus)}\r\n";
                    message += $"\tOUTPUT : {string.Join(",", OCSCommManager.Instance.OcsStatus.OutputStatus)}\r\n";

                    message += "[TPD Information]\r\n";
                    foreach (enTPD tpd in Enum.GetValues(typeof(enTPD)))
                        message += $"\t{tpd}={ProcessDataHandler.Instance.Tpd[tpd]}\r\n";

                    message += "[Sequence Information]\r\n";
                    List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                    foreach (XSeqFunc func0 in funcs)
                    {
                        string msg = GetXFuncInformation(func0, 0);
                        if (msg != string.Empty) message += msg;
                        if (func0.SeqName == "SeqAuto")
                        {
                            List<XSeqFunc> auto_funcs = (func0 as SeqAuto).SubSeqLists;
                            foreach (XSeqFunc func1 in auto_funcs)
                            {
                                msg = GetXFuncInformation(func1, 1);
                                if (msg != string.Empty) message += msg;
                                if (func1.SeqName == "SeqTransfer")
                                {
                                    List<XSeqFunc> transfer_funcs = (func1 as SeqTransfer).SubSeqLists;
                                    foreach (XSeqFunc func2 in transfer_funcs)
                                    {
                                        msg = GetXFuncInformation(func2, 2);
                                        if (msg != string.Empty) message += msg;
                                    }
                                }
                            }
                        }
                    }

                    AlarmLog.WriteLog(message);
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            private string GetXFuncInformation(XSeqFunc func, int step)
            {
                string message = string.Empty;

                if (func.SeqNo > 0)
                {
                    string case_message = $"SequenceName=[{func.SeqName}], ";
                    if (func.SeqCaseMemoLists.Count > 0 && func.SeqCaseMemoLists.ContainsKey(func.SeqNo))
                        case_message += $"SeqNo=[({func.SeqNo}) {func.SeqCaseMemoLists[func.SeqNo]}], ";
                    else case_message += $"SeqNo=[{func.SeqNo}], ";
                    if (func.SeqCaseMemoLists.Count > 0 && func.SeqCaseMemoLists.ContainsKey(func.ReturnSeqNo))
                        case_message += $"ReturnSeqNo=[({func.ReturnSeqNo}) {func.SeqCaseMemoLists[func.ReturnSeqNo]}], ";
                    else case_message += $"ReturnSeqNo=[{func.ReturnSeqNo}], ";
                    case_message += $"AlarmId=[{func.AlarmId}]";

                    if (step == 0) message = $"\t{case_message}\r\n";
                    else if (step == 1) message = $"\t\t{case_message}\r\n";
                    else if (step == 2) message = $"\t\t\t{case_message}\r\n";
                }
                return message;
            }

            #endregion
        }
        public class SeqEventReport : XSeqFunc
        {
            private const string FuncName = "[SeqEventReport]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private bool m_EventReply = false;
            #endregion

            #region Property
            #endregion

            #region Contructor
            public SeqEventReport()
            {
                this.SeqName = "SeqEventReport";

                m_OCSManager = OCSCommManager.Instance;
                m_OCSManager.OcsStatus.delEventReportReply += OcsStatus_delEventReportReply;
            }
            private void OcsStatus_delEventReportReply(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_EventReply))
                    {
                        m_EventReply = true;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                if (m_OCSManager.OcsComm.OcsUse == false) return -1;

                int seqNo = this.SeqNo;
                int nRv = 0;

                switch (seqNo)
                {
                    case 0:
                        {
                            //if (OCSCommManager.Instance.OcsStatus.Connected == false) break; // 여기서 막하야 하나 ? QUE를 뽑아 사용하고 나니 없어지네...

                            if (m_OCSManager.OcsStatus.EventQueue.Count > 0)
                            {
                                VehicleEvent vehicle_event = m_OCSManager.OcsStatus.EventQueue.Dequeue();
                                int vehicle_number = AppConfig.Instance.VehicleNumber;
                                int node_number = ProcessDataHandler.Instance.CurVehicleStatus.CurrentNode.NodeID;
                                string transfer_command = ProcessDataHandler.Instance.CurTransferCommand.CommandID;
                                if (vehicle_event == VehicleEvent.CancelCompleted || vehicle_event == VehicleEvent.AbortCompleted)
                                    transfer_command = m_OCSManager.OcsStatus.AbortCancelCommandID;

                                VehicleIF_EventSend vehicleIF_EventSend = new VehicleIF_EventSend
                                {
                                    VehicleNumber = vehicle_number,
                                    NodeNumber = node_number,
                                    TransferCommandID = transfer_command,
                                    Event = vehicle_event,
                                };

                                m_EventReply = false;
                                m_OCSManager.OcsComm.SendIFMessage(vehicleIF_EventSend);
                                string msg = string.Format("VehicleEvent Send : CommandID = {0}, Node={1}, Event={2}", 
                                    vehicleIF_EventSend.TransferCommandID.Trim('\0'), vehicleIF_EventSend.NodeNumber, vehicleIF_EventSend.Event);
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);

                                // Event 발생시간 기록
                                switch (vehicle_event)
                                {
                                    case VehicleEvent.DepartToFromPosition:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.MoveToSource);
                                        break;
                                    case VehicleEvent.ArrivedAtFromPosition:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.ArrivedSource);
                                        break;
                                    case VehicleEvent.AcquireStart:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.AquireStart);
                                        break;
                                    case VehicleEvent.CarrierInstalled:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.Install);
                                        break;
                                    case VehicleEvent.AcquireCompleted:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.AquireEnd);
                                        break;
                                    case VehicleEvent.DepartToToPosition:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.MoveToDestination);
                                        break;
                                    case VehicleEvent.ArrivedAtToPosition:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.ArrivedDestination);
                                        break;
                                    case VehicleEvent.DepositStart:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.DepositStart);
                                        break;
                                    case VehicleEvent.CarrierRemoved:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.Install);
                                        break;
                                    case VehicleEvent.DepositCompleted:
                                        ProcessDataHandler.Instance.CurTransferCommand.UpdateTransferTime(transferUpdateTime.DepositEnd);
                                        break;
                                    case VehicleEvent.AutoTeachingCompleted:
                                        {
                                            int target_port = ProcessDataHandler.Instance.CurTransferCommand.DestinationID;
                                            DataItem_Port port = DatabaseHandler.Instance.DictionaryPortDataList[target_port];
                                            VehicleIF_AutoTeachingResultSend message = new VehicleIF_AutoTeachingResultSend
                                            {
                                                VehicleNumber = vehicle_number,
                                                PortNumber = target_port,
                                                Result = OCSCommManager.Instance.OcsStatus.AutoTeachingResult,
                                                BarcodeValue1 = (int)port.BarcodeLeft,
                                                BarcodeValue2 = (int)port.BarcodeRight,
                                                SlideValue = port.SlidePosition,
                                                RotateValue = port.RotatePosition,
                                            };
                                            m_OCSManager.OcsComm.SendIFMessage(message);
                                            msg = string.Format("AutoTeachingResult Send : BarcodeLeft={0}, BarcodeRight={1}, SlidePosition={2}, RotatePosition={3}",
                                                port.BarcodeLeft, port.BarcodeRight, port.SlidePosition, port.RotatePosition);
                                            SequenceOCSLog.WriteLog(FuncName, msg);
                                        }
                                        break;
                                    case VehicleEvent.CarrierIDMismatch:
                                        {
                                            string carrier_id = ProcessDataHandler.Instance.CurTransferCommand.CassetteID;
                                            string reading_id = m_OCSManager.OcsStatus.CarrierIDScanRFIDTag;
                                            msg = string.Format("CarrierID Mismatch Report - {0}, {1}", carrier_id, reading_id);
                                            SequenceOCSLog.WriteLog(FuncName, msg);
                                        }
                                        break;
                                }
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (m_EventReply)
                            {
                                m_EventReply = false;
                                string msg = string.Format("VehicleEventSend Reply OK");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 0;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 100)
                            {
                                string msg = string.Format("VehicleEventSend Not Response");
                                SequenceOCSLog.WriteLog(FuncName, msg, seqNo);
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return nRv;
            }
            #endregion
        }
        public class SeqOcsCommonProcess : XSeqFunc
        {
            private const string FuncName = "[SeqOcsCommonProcess]";

            #region Fields
            private OCSCommManager m_OCSManager = null;

            #endregion

            #region Property
            #endregion

            #region Contructor
            public SeqOcsCommonProcess()
            {
                this.SeqName = "SeqOcsCommonProcess";

                m_OCSManager = OCSCommManager.Instance;
                m_OCSManager.OcsStatus.delCommonProcessRequest += OcsStatus_delCommonProcessRequest;
            }
            private void OcsStatus_delCommonProcessRequest(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_DataVersionRequest))
                    {
                        ProcessDataVersionReply(obj1, obj2);
                    }
                    else if (obj2.GetType() == typeof(VehicleIF_PathSend))
                    {
                        ProcessPathSendReceived(obj1, obj2);
                    }
                    else if (obj2.GetType() == typeof(VehicleIF_OperationConfigDataSend))
                    {
                        ProcessConfigDataUpdate(obj1, obj2);
                    }
                    else if (obj2.GetType() == typeof(VehicleIF_CommandStatusRequest))
                    {
                        ProcessCommandStatusRequest(obj1, obj2);
                    }
                    else if (obj2.GetType() == typeof(VehicleIF_LocationInformationSend))
                    {
                        ProcessPortInformationUpdate(obj1, obj2);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                int nRv = 0;

                switch (seqNo)
                {
                    case 0:
                        {
                            bool pass_request = m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PathRequest) == FlagValue.ON;
                            if (pass_request)
                            {
                                m_OCSManager.OcsStatus.ResetFlag(InterfaceFlag.PathReceived);

                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PathRequest, FlagValue.OFF);
                                PathRequestType type = m_OCSManager.OcsStatus.PathRequestType;
                                int startNode = m_OCSManager.OcsStatus.PathRequestStartNode;
                                int endNode = m_OCSManager.OcsStatus.PathRequestEndNode;

                                VehicleIF_PathRequest message = new VehicleIF_PathRequest
                                {
                                    RequestType = type,
                                    StartNodeNo = startNode,
                                    EndNodeNo = endNode,
                                    VehicleNumber = AppConfig.Instance.VehicleNumber,
                                };
                                m_OCSManager.OcsComm.SendIFMessage(message);
                            }

                            bool id_report = m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.CarrierIdReadingComp) == FlagValue.ON;
                            if (id_report)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.CarrierIdReadingComp, FlagValue.OFF);

                                IDReadResult result = m_OCSManager.OcsStatus.CarrierIDScanResult;
                                string commadID = ProcessDataHandler.Instance.CurTransferCommand.CommandID;
                                string carrierID = m_OCSManager.OcsStatus.CarrierIDScanRFIDTag;

                                VehicleIF_IDReadResultSend message = new VehicleIF_IDReadResultSend
                                {
                                    Result = result,
                                    CommandID = commadID,
                                    CarrierID = carrierID,
                                    VehicleNumber = AppConfig.Instance.VehicleNumber,
                                };
                                m_OCSManager.OcsComm.SendIFMessage(message);
                            }
                        }
                        break;

                    case 10:
                        {

                        }
                        break;
                }

                this.SeqNo = seqNo;
                return nRv;
            }
            #endregion

            #region Process Function
            private void ProcessPortInformationUpdate(object sender, object eventData)
            {
                VehicleIF_LocationInformationSend msg = (eventData as VehicleIF_LocationInformationSend);
                VehicleIF_LocationInformationReply reply = new VehicleIF_LocationInformationReply
                {
                    VehicleNumber = msg.VehicleNumber,
                    Acknowledge = IFAcknowledge.NAK,
                };
                reply.SetSystemByte(msg.SystemByte);
                string strLog = $"[Port Location Info Request] PORT ID [{msg.LocationNo}] Change Code [{msg.ChangeCode}]";

                try
                {
                    bool data_check_OK = true;

                    string version = msg.Version;
                    DataItem_Port port = new DataItem_Port
                    {
                        HoistPosition = msg.HoistPosition,
                        SlidePosition = msg.SlidePosition,
                        RotatePosition = msg.RotatePosition,
                        UnloadHoistPosition = msg.UnloadHoistPosition,
                        UnloadSlidePosition = msg.UnloadSlidePoistion,
                        UnloadRotatePosition = msg.UnloadRotatePosition,
                        PortType = msg.TypeOfPort,
                        PortID = msg.LocationNo,
                        LinkID = msg.LinkID,
                        NodeID = msg.NodeID,
                        BarcodeLeft = msg.BarcodeLeft,
                        BarcodeRight = msg.BarcodeRight,
                        PIOID = msg.PIOID,
                        PIOCH = msg.PIOCH,
                        PIOUsed = msg.PIOUsed,
                        PortProhibition = false,
                        OffsetUsed = msg.PortProhibition,
                        PBSUsed = msg.PBSUsed,
                        PBSSelectNo = msg.PBSSelectNumber,
                    };
                    if (msg.ChangeCode == LocationInformationChangeCode.Add || msg.ChangeCode == LocationInformationChangeCode.Modify)
                    {
                        if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(port.PortID))
                        {
                            DatabaseHandler.Instance.DictionaryPortDataList[port.PortID] = port;
                            int result = DatabaseHandler.Instance.QueryPort.Update(port);
                            if (result > 0)
                            {
                                strLog += $"Nak!! result={result}, Reason [Update NG !!]";
                                data_check_OK = false;
                            }
                        }
                        else
                        {
                            DatabaseHandler.Instance.DictionaryPortDataList.Add(port.PortID, port);
                            int result = DatabaseHandler.Instance.QueryPort.Insert(port);
                            if (result > 0)
                            {
                                strLog += $"Nak!! result={result}, Reason [Insert NG !!]";
                                data_check_OK = false;
                            }
                        }
                    }
                    else if (msg.ChangeCode == LocationInformationChangeCode.Remove)
                    {
                        if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(port.PortID))
                        {
                            DatabaseHandler.Instance.DictionaryPortDataList.Remove(port.PortID);
                            int result = DatabaseHandler.Instance.QueryPort.Delete(port.PortID);
                        }
                        else
                        {
                            strLog += $"Nak!! Reason [PortID={port.PortID} not Exist !!]";
                            data_check_OK = false;
                        }
                    }
                    else
                    {
                        strLog += $"Nak!! Reason [ChangeCode Abnormal !!]";
                        data_check_OK = false;
                    }

                    if (data_check_OK)
                    {
                        UpdateGeneralInfo(GeneralInformationItemName.PortDataVersion, version);
                        strLog += $"Ack!! Port Data Version Update";
                        reply.Acknowledge = IFAcknowledge.ACK;
                    }
                    else
                    {
                        reply.Acknowledge = IFAcknowledge.NAK;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            private void ProcessCommandStatusRequest(object sender, object eventData)
            {
                VehicleIF_CommandStatusRequest msg = (eventData as VehicleIF_CommandStatusRequest);
                VehicleIF_CommandStatusReply reply = new VehicleIF_CommandStatusReply
                {
                    VehicleNumber = msg.VehicleNumber,
                };
                reply.SetSystemByte(msg.SystemByte);
                string strLog = $"[Command Status Request] CMD ID [{msg.TransferCommandID}] ";

                try
                {
                    bool not_received = true;
                    DataItem_TransferInfo find_command = DatabaseHandler.Instance.GetTransferCommand(msg.TransferCommandID);
                    if (find_command != null) not_received = false;
                    if (ProcessDataHandler.Instance.CurTransferCommand.CommandID == msg.TransferCommandID) not_received = false;

                    if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                    {
                        reply.TransferCommandID = msg.TransferCommandID;
                        reply.VehicleNumber = AppConfig.Instance.VehicleNumber;
                        strLog += $"Nak!! CurrentCMD Valid == false.";
                    }
                    else
                    {
                        m_OCSManager.OcsStatus.OCSRestartReschedule = true;

                        reply.TransferCommandID = ProcessDataHandler.Instance.CurTransferCommand.CommandID;
                        reply.VehicleNumber = AppConfig.Instance.VehicleNumber;
                        strLog += $"Ack!!";
                    }

                    if (not_received)
                    {
                        reply.StatusCode = CommandStatusCode.NotReceived;
                    }
                    else if (ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Completed ||
                        ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Aborted ||
                        ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Canceled ||
                        ProcessDataHandler.Instance.CurTransferCommand.CommandStatus == ProcessStatus.Deleted)
                    {
                        reply.StatusCode = CommandStatusCode.Completed;
                    }
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.NotAssigned)
                        reply.StatusCode = CommandStatusCode.NotReceived;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.EnRouteToSource)
                        reply.StatusCode = CommandStatusCode.MovingToSource;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.EnRouteToDest)
                        reply.StatusCode = CommandStatusCode.MovingToDestination;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Acquiring)
                        reply.StatusCode = CommandStatusCode.Acquiring;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Depositing)
                        reply.StatusCode = CommandStatusCode.Depositing;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.SourceEmpty)
                        reply.StatusCode = CommandStatusCode.SourceEmpty;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.AcquireFailed)
                        reply.StatusCode = CommandStatusCode.AcquireFailed;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.DestinationDouble)
                        reply.StatusCode = CommandStatusCode.DestinationDouble;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.DepositFailed)
                        reply.StatusCode = CommandStatusCode.DepositFailed;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.AutoTeaching)
                        reply.StatusCode = CommandStatusCode.MovingToDestination;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Go)
                        reply.StatusCode = CommandStatusCode.MovingToDestination;
                    else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.AcquireCompleted)
                        reply.StatusCode = CommandStatusCode.MovingToDestination;
                    else // 이때는 뭔가 이상함. NotAssign 상태면 실행전인데 뭘로 보고 해야 하나 ?
                        reply.StatusCode = CommandStatusCode.Completed;

                    strLog += $" reply.StatusCode={reply.StatusCode}";
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            private void ProcessConfigDataUpdate(object sender, object eventData)
            {
                VehicleIF_OperationConfigDataSend msg = (eventData as VehicleIF_OperationConfigDataSend);
                VehicleIF_OperationConfigDataReply reply = new VehicleIF_OperationConfigDataReply
                {
                    VehicleNumber = msg.VehicleNumber,
                    Acknowledge = IFAcknowledge.NAK,
                };
                reply.SetSystemByte(msg.SystemByte);
                string strLog = $"[ProcessCommand] Config Data Update ";

                try
                {
                    bool data_check_OK = true;

                    if (AppConfig.Instance.VehicleNumber != msg.VehicleNumber)
                    {
                        strLog += $"Nak!! Vehicle Number [{AppConfig.Instance.VehicleNumber} != {msg.VehicleNumber}] Reason [Vehicle Number Difference !!]";
                        data_check_OK = false;
                    }
                    else
                    {
                        SetupManager.Instance.SetupWheel.StraightSpeed = (eventData as VehicleIF_OperationConfigDataSend).StraightSpeed;
                        SetupManager.Instance.SetupWheel.CurveSpeed = (eventData as VehicleIF_OperationConfigDataSend).CurveSpeed;
                        SetupManager.Instance.SetupWheel.BranchSpeed = (eventData as VehicleIF_OperationConfigDataSend).BranchSpeed;
                        SetupManager.Instance.SetupWheel.SBranchSpeed = (eventData as VehicleIF_OperationConfigDataSend).SBranchSpeed;
                        SetupManager.Instance.SetupWheel.JunctionSpeed = (eventData as VehicleIF_OperationConfigDataSend).JunctionSpeed;
                        SetupManager.Instance.SetupWheel.SJunctionSpeed = (eventData as VehicleIF_OperationConfigDataSend).SJunctionSpeed;
                        SetupManager.Instance.SetupWheel.BranchStraightSpeed = (eventData as VehicleIF_OperationConfigDataSend).BranchStraightSpeed;
                        SetupManager.Instance.SetupWheel.JunctionStraightSpeed = (eventData as VehicleIF_OperationConfigDataSend).JunctionStraightSpeed;
                        SetupManager.Instance.SetupOperation.AutoDoor1Use = (eventData as VehicleIF_OperationConfigDataSend).AutoDoor1Mode == 1 ? Use.Use : Use.NoUse;
                        SetupManager.Instance.SetupOperation.AutoDoor2Use = (eventData as VehicleIF_OperationConfigDataSend).AutoDoor2Mode == 1 ? Use.Use : Use.NoUse;
                    }

                    if (data_check_OK)
                    {
                        SetupManager.Instance.SaveSetupWheel();
                        SetupManager.Instance.SaveSetupOperation();
                        strLog += $"Ack!! Config Data Update Complete !!]";
                        reply.Acknowledge = IFAcknowledge.ACK;
                    }
                    else
                    {
                        reply.Acknowledge = IFAcknowledge.NAK;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            private void ProcessDataVersionReply(object sender, object eventData)
            {
                VehicleIF_DataVersionRequest msg = (eventData as VehicleIF_DataVersionRequest);
                VehicleIF_DataVersionReply reply = new VehicleIF_DataVersionReply();
                reply.SetSystemByte(Convert.ToByte(msg.SystemByte));
                string strLog = $"[ProcessCommand] Data Version Request ";

                try
                {
                    if (DatabaseHandler.Instance.DictionaryGeneralInformation.ContainsKey(GeneralInformationItemName.NodeDataVersion))
                        reply.NodeDataVersion = DatabaseHandler.Instance.DictionaryGeneralInformation[GeneralInformationItemName.NodeDataVersion].ItemValue;
                    else reply.NodeDataVersion = string.Empty;
                    if (DatabaseHandler.Instance.DictionaryGeneralInformation.ContainsKey(GeneralInformationItemName.LinkDataVersion))
                        reply.LinkDataVersion = DatabaseHandler.Instance.DictionaryGeneralInformation[GeneralInformationItemName.LinkDataVersion].ItemValue;
                    else reply.LinkDataVersion = string.Empty;
                    if (DatabaseHandler.Instance.DictionaryGeneralInformation.ContainsKey(GeneralInformationItemName.PortDataVersion))
                        reply.PortDataVersion = DatabaseHandler.Instance.DictionaryGeneralInformation[GeneralInformationItemName.PortDataVersion].ItemValue;
                    else reply.PortDataVersion = string.Empty;
                    if (DatabaseHandler.Instance.DictionaryGeneralInformation.ContainsKey(GeneralInformationItemName.PIODeviceDataVersion))
                        reply.PIODeviceDataVersion = DatabaseHandler.Instance.DictionaryGeneralInformation[GeneralInformationItemName.PIODeviceDataVersion].ItemValue;
                    else reply.PIODeviceDataVersion = string.Empty;
                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.VersionRequest, FlagValue.ON);
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            private void ProcessPathSendReceived(object sender, object eventData)
            {
                VehicleIF_PathSend msg = (eventData as VehicleIF_PathSend);
                string strLog = $"[ProcessCommand] Path Received ";

                try
                {
                    bool data_check_OK = true;
                    if (msg.PathNodes.Count != 0)
                    {
                        m_OCSManager.OcsStatus.PathRequestRecvNodes.Clear();
                        // 만일 current node가 진행하는 path들 사이에 있는 경우는 current node를 삽입할 필요 없다. 지나갈 거니까 ~~~~
                        List<int> newPaths = new List<int>();
                        data_check_OK &= CheckCurrentNode(msg.PathNodes, ref newPaths);
                        data_check_OK &= newPaths != null;
                        data_check_OK &= newPaths.Count > 1;
                        if (data_check_OK)
                        {
                            m_OCSManager.OcsStatus.PathRequestRecvNodes.AddRange(newPaths);
                            strLog += $"Ack!! ReceivedNodes [{string.Join("-", msg.PathNodes)}] newPaths [{string.Join("-", newPaths)}]";
                        }
                        else
                        {
                            m_OCSManager.OcsStatus.PathRequestRecvNodes.AddRange(msg.PathNodes);
                            strLog += $"Nak!! ReceivedNodes [{string.Join("-", msg.PathNodes)}]";
                        }

                        if (data_check_OK)
                            m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PathReceived, FlagValue.ON);
                        else m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PathReceivedNG, FlagValue.ON);
                    }
                    else
                    {
                        strLog += $"Nak!! Recive Node is Null!";
                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PathReceivedNG, FlagValue.ON);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                }
            }
            private bool CheckCurrentNode(List<int> nodes, ref List<int> newPaths)
            {
                bool rv = true;
                try
                {
                    newPaths.AddRange(nodes); // 비정상일때 자신을 되돌려 주기 위해..

                    int fromNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID;
                    int toNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToNodeID;
                    int startNode = m_OCSManager.OcsStatus.PathRequestStartNode;
                    int endNode = m_OCSManager.OcsStatus.PathRequestEndNode;
                    rv &= nodes.Count > 0;
                    if (rv)
                    {
                        bool first_ok = false;
                        first_ok |= nodes.First() == fromNode;
                        first_ok |= nodes.First() == toNode;
                        first_ok |= nodes.First() == startNode;
                        rv &= first_ok;
                        rv &= nodes.Last() == endNode;

                        if (rv)
                        {
                            int path_count = newPaths.Count;
                            if (path_count > 0)
                            {

                                if (path_count == 1)
                                {
                                    if (nodes[0] != fromNode)
                                    {
                                        newPaths.Clear();
                                        newPaths.Add(fromNode);
                                        newPaths.Add(nodes[0]);
                                    }
                                    else;//abnormal case
                                }
                                else if (path_count > 1)
                                {
                                    // 중복 경로 일때도 정상동작 TEST 해보자
                                    newPaths.Clear();
                                    if (fromNode != nodes[0]) newPaths.Add(fromNode);
                                    int get_count = path_count;
                                    newPaths.AddRange(nodes.GetRange(0, get_count));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return rv;
            }
            #endregion
        }
        public class SeqOcsCommandProcess : XSeqFunc
        {
            private const string FuncName = "[SeqOcsCommandProcess]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private DevGripperPIO m_GripperPio = null;
            private bool m_OcsAbort = false;
            private bool m_OcsCancel = false;
            private bool m_ManualOperation = false;
            private VehicleInRailStatus m_OldInRailStatus = VehicleInRailStatus.OutOfRail;
            System.Diagnostics.Stopwatch m_StopWatch = new System.Diagnostics.Stopwatch();
            #endregion

            #region Property
            #endregion

            #region Contructor
            public SeqOcsCommandProcess()
            {
                this.SeqName = "SeqOcsCommandProcess";

                m_OCSManager = OCSCommManager.Instance;
                m_GripperPio = DevicesManager.Instance.DevGripperPIO;

                m_OCSManager.OcsStatus.delCommandProcessRequest += OcsStatus_delCommandProcessRequest;
            }
            private void OcsStatus_delCommandProcessRequest(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_CommandSend))
                    {
                        ProcessCommand(obj1, obj2);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                int nRv = 0;

                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PauseRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PauseRequest, FlagValue.OFF);
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Pause);
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.ResumeRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.ResumeRequest, FlagValue.OFF);
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.WaitCancelRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.WaitCancelRequest, FlagValue.OFF);
                                m_OcsCancel = true;
                                ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Canceling);
                                m_StopWatch.Stop();
                                seqNo = 10;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.CancelRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.CancelRequest, FlagValue.OFF);
                                m_OcsCancel = false;
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                                ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Canceling);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.WaitAbortRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.WaitAbortRequest, FlagValue.OFF);
                                m_OcsAbort = true;
                                ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Aborting);
                                m_StopWatch.Stop();
                                seqNo = 10;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.AbortRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.AbortRequest, FlagValue.OFF);
                                m_OcsAbort = false;
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                                ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Aborting);
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PowerOnRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PowerOnRequest, FlagValue.OFF);
                                // Power ON Action
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PowerOffRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PowerOffRequest, FlagValue.OFF);
                                // Power OFF Action
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.AlarmClearRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.AlarmClearRequest, FlagValue.OFF);
                                IsPushedSwitch.m_AlarmRstPushed = true;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.BuzzerOffRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.BuzzerOffRequest, FlagValue.OFF);
                                IsPushedSwitch.m_BuzzerOffPushed = true;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.TeachingDataRequest) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.TeachingDataRequest, FlagValue.OFF);
                                ProcessTeachingDataSend();
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.RouteChange) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.RouteChange, FlagValue.OFF);
                                ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.RouteChanging);
                                m_StopWatch.Stop();
                                seqNo = 10;
                            }
                            else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.InstallPermit) == FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.InstallPermit, FlagValue.OFF);
                                ProcessDataHandler.Instance.CurVehicleStatus.SetVehicleStatus(VehicleState.NotAssigned); // Vehicle을 InRail 될수 있도록 하자 ~~
                            }
                            else
                            {
                                if (m_OldInRailStatus != ProcessDataHandler.Instance.CurVehicleStatus.InRail)
                                {
                                    m_OldInRailStatus = ProcessDataHandler.Instance.CurVehicleStatus.InRail;
                                    if (m_OldInRailStatus == VehicleInRailStatus.InRail) seqNo = 200;
                                }
                            }
                            //else if (ProcessDataHandler.Instance.CurVehicleStatus.InRail == VehicleInRailStatus.InRail &&
                            //    m_OldInRailStatus == VehicleInRailStatus.OutOfRail)
                            //{
                            //    m_OldInRailStatus = VehicleInRailStatus.InRail;
                            //    seqNo = 200;
                            //}
                            //else if (ProcessDataHandler.Instance.CurVehicleStatus.GetVehicleStatus() == VehicleState.Removed &&
                            //    m_OldInRailStatus == VehicleInRailStatus.InRail)
                            //{
                            //    m_OldInRailStatus = VehicleInRailStatus.OutOfRail;
                            //}
                        }
                        break;

                    case 10:
                        {
                            // Abort 가능 위치 확인
                            bool vehicle_working_state = false;
                            vehicle_working_state |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Acquiring;
                            vehicle_working_state |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Depositing;

                            bool vehicle_moving_state = false;
                            vehicle_moving_state |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.EnRouteToSource;
                            vehicle_moving_state |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.EnRouteToDest;
                            vehicle_moving_state |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.AutoTeaching;
                            vehicle_moving_state |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Go;

                            double enable_distance = SetupManager.Instance.SetupOCS.CancelAbortEnableDistance;
                            bool stop_enable_area = true;
                            stop_enable_area &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.Straight;
                            stop_enable_area &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RemainDistanceOfLink > enable_distance;
                            stop_enable_area &= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RunPositionOfLink > 500.0f;
                            stop_enable_area &= DevicesManager.Instance.DevEqpPIO.IsPioRun() ? false : true;

                            bool abort_run = false;
                            if (vehicle_working_state == false) // abort  / cancel은 이동하라는 명령일때 ... 처리
                            {
                                if (vehicle_moving_state) // 명령어 진행 중
                                {
                                    if (GV.WheelBusy) // 이동 중인 경우
                                    {
                                        // 명령 수행이 끝날때까지 기다리자~~
                                        abort_run = stop_enable_area;
                                    }
                                    else
                                    {
                                        if (m_StopWatch.IsRunning == false)
                                        {
                                            m_StopWatch.Reset();
                                            m_StopWatch.Start();
                                        }
                                        float lastScanMilliSec = (float)((double)m_StopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency);
                                        if (lastScanMilliSec > 5000.0f)
                                        {
                                            abort_run = true;
                                            m_StopWatch.Stop();
                                        }
                                        if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false) abort_run = true;
                                        StartTicks = XFunc.GetTickCount();
                                    }
                                }
                                else
                                {
                                    abort_run |= GV.WheelBusy == false; // SeqAuto를 완료하고 나서 Busy 확인하고 abort를 처리하자...
                                }
                            }
                            abort_run |= AlarmCurrentProvider.Instance.IsHeavyAlarm();

                            if (abort_run)
                            {
                                seqNo = 20;
                            }
                            else if (GV.scmAbortSeqRun.Ing)
                            {
                                SequenceOCSLog.WriteLog(FuncName, "Operator Manual Abort Start");
                                m_ManualOperation = true;
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 110;
                            }
                            //if (AppConfig.Instance.Simulation.MY_DEBUG)
                            //    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} : " +
                            //        $"Command Cancel {stop_enable_area}, " +
                            //        $"{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type}, " +
                            //        $"{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RemainDistanceOfLink}, " +
                            //        $"{ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RunPositionOfLink}, " +
                            //        $"{ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState}, " +
                            //        $"{GV.WheelBusy}, " +
                            //        $"{AlarmCurrentProvider.Instance.IsHeavyAlarm()}");
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            ProcessStatus status = ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus();
                            SequenceOCSLog.WriteLog(FuncName, string.Format("Abort Sequence Start. Process Status : {0}", status));
                            EqpStateManager.Instance.SetRunMode(EqpRunMode.Abort);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 100;
                        }
                        break;

                    case 100:
                        {
                            if (GV.scmAbortSeqRun.Ing || GV.scmAbortSeqRun.End)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 110;
                            }
                            else if (XFunc.GetTickCount() - StartTicks > 10000) //Abort 하는 시간이 10초는 필요하네...
                            {
                                SequenceOCSLog.WriteLog(FuncName, "Abort Sequence Timeover");
                                // 왜 Abort 진행을 못하는거지 ?
                                // 다시 Retry 해라...!
                                List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                                foreach (XSeqFunc func in funcs)
                                {
                                    if (func.SeqName == "SeqAbortMonitor") { func.SeqAbort(); break; }
                                }
                                StartTicks = XFunc.GetTickCount();
                                if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort) 
                                    seqNo = 20; //이상하네 왜 Abort가 아니지 ?
                            }
                        }
                        break;

                    case 110:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            if (GV.scmAbortSeqRun.End)
                            {
                                SequenceOCSLog.WriteLog(FuncName, "Abort Sequence Completed");

                                bool command_not_assigned = ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() == ProcessStatus.Completed ||
                                    ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() == ProcessStatus.Deleted;
                                bool cancel = m_OcsCancel && command_not_assigned;
                                bool abort = m_OcsAbort && command_not_assigned;

                                if (ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() == ProcessStatus.Canceling || cancel)
                                {
                                    m_OcsCancel = true;
                                    ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Canceled);
                                    //OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CancelCompleted);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 120;
                                }
                                else if (ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() == ProcessStatus.Aborting || abort)
                                {
                                    m_OcsAbort = true;
                                    ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Aborted);
                                    //OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AbortCompleted);
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 120;
                                }
                                else if (ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() == ProcessStatus.RouteChanging)
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 150;
                                }
                                else
                                {
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 150;
                                }
                            }
                        }
                        break;

                    case 120:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 200) break;
                            // 만일 진행 중인 Command가 있다면 지우자!
                            if (ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() != ProcessStatus.Completed &&
                                    ProcessDataHandler.Instance.CurTransferCommand.GetProcessStatus() != ProcessStatus.Deleted)
                            {
                                ProcessDataHandler.Instance.DeleteTransferCommand(ProcessDataHandler.Instance.CurTransferCommand);
                                ProcessDataHandler.Instance.CurTransferCommand.SetProcessStatus(ProcessStatus.Deleted);
                            }
                            //List<XSeqFunc> funcs = TaskHandler.Instance.GetFuncLists();
                            //foreach (XSeqFunc func in funcs)
                            //{
                            //    if (func.SeqName == "SeqModeMonitor" || func.SeqName == "SeqAbortMonitor") continue;
                            //    func.SeqAbort();
                            //}

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 130;
                        }
                        break;

                    case 130:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;
                            if (m_OcsCancel) OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.CancelCompleted); //Event Report를 Command Delete 후 하자~~
                            else if (m_OcsAbort) OCSCommManager.Instance.OcsStatus.SendVehicleEvent(VehicleEvent.AbortCompleted);

                            if (m_ManualOperation)
                            {
                                SequenceOCSLog.WriteLog(FuncName, "Operator Manual Abort Completed");
                            }
                            else if (AlarmHandler.AutoRecoveryNeed)
                            {
                                AlarmHandler.AutoRecoveryNeed = false;
                                GV.AutoRecoveryStart = true; // 자동 Recovery 시도해라 ~~~
                                SequenceOCSLog.WriteLog(FuncName, "OCS Cancel/Abort Auto Recovery Start");
                            }
                            else
                            {
                                // 만일 Alarm이 없는데 OCS에서 강제적으로 Cancel/Abort 할 경우.
                                // Abort 실행 후 Auto/Start로 변경해야 한다.
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                                SequenceOCSLog.WriteLog(FuncName, "OCS Cancel/Abort Run Mode Start");
                            }
                            m_OcsCancel = false;
                            m_OcsAbort = false;
                            m_ManualOperation = false;
                            seqNo = 0;
                        }
                        break;

                    case 150:
                        {
                            if (m_ManualOperation)
                            {
                                SequenceOCSLog.WriteLog(FuncName, "Operator Manual Abort Completed");

                            }
                            else
                            {
                                EqpStateManager.Instance.SetRunMode(EqpRunMode.Start);
                                SequenceOCSLog.WriteLog(FuncName, "OCS Route Change Run Mode Start");
                            }
                            m_OcsCancel = false;
                            m_OcsAbort = false;
                            m_ManualOperation = false;
                            seqNo = 0;
                        }
                        break;

                    case 200:
                        {
                            // Install Report
                            VehicleIF_EventSend vehicleIF_EventSend = new VehicleIF_EventSend
                            {
                                VehicleNumber = AppConfig.Instance.VehicleNumber,
                                Event = VehicleEvent.Installed
                            };
                            m_OCSManager.OcsComm.SendIFMessage(vehicleIF_EventSend);
                            seqNo = 0;
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return nRv;
            }
            #endregion

            #region Process Function
            private bool IsEqpReady()
            {
                bool isReady = true;
                isReady &= !GV.ThreadStop;
                isReady &= !GV.BeltCutInterlock;
                isReady &= !GV.SwingSensorInterlock;
                isReady &= !GV.BumpCollisionInterlock;
                isReady &= GV.PowerOn;
                isReady &= !GV.EmoAlarm;
                isReady &= !GV.SaftyAlarm;
                isReady &= !GV.CleanerDoorOpenInterlock;
                return isReady;
            }
            private void ProcessCommand(object sender, object eventData)
            {
                VehicleIF_CommandSend msg = (eventData as VehicleIF_CommandSend);
                VehicleIF_CommandReply reply = new VehicleIF_CommandReply
                {
                    VehicleNumber = msg.VehicleNumber,
                    TransferCommandID = msg.TransferCommandID,
                    Command = msg.ProcessCommand,
                    Acknowledge = IFAcknowledge.NAK,
                };
                reply.SetSystemByte(Convert.ToByte(msg.SystemByte));
                string strLog = $"[ProcessCommand] {msg.ProcessCommand.ToString()}. ";

                string CommandID = string.Empty;
                bool auto = EqpStateManager.Instance.OpMode == OperateMode.Auto;

                try
                {
                    bool data_check_OK = true;

                    switch (msg.ProcessCommand)
                    {
                        case OCSCommand.Transfer:
                        case OCSCommand.CarrierIdScan:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x=>x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool existSourceID = msg.SourceID != 0 ? DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(msg.SourceID) : true;
                                bool existDestinationID = msg.DestinationID != 0 ? DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(msg.DestinationID) : true;
                                bool destinationIDnull = msg.DestinationID == 0; // Destination 위치가 없다. TransferCommand에는 항상 Destination 위치가 있어야 함.
                                bool notExistFoup = msg.ProcessCommand == OCSCommand.Transfer && msg.SourceID == 0 && msg.DestinationID != 0 && !m_GripperPio.IsProductExist(); //Deposit 상황인데 Foup가 없다.
                                bool abnormal_command = false;
                                abnormal_command |= notExistFoup;
                                abnormal_command |= destinationIDnull;
                                abnormal_command |= existSameCommand;
                                abnormal_command |= existSourceID == false;
                                abnormal_command |= existDestinationID == false;

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (IsEqpReady() == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Axis Not Ready!!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_command)
                                {
                                    if (existSameCommand)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                    else if (existDestinationID == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Not Find Destination!]";
                                    else if (existSourceID == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Not Find Source!]";
                                    else if (destinationIDnull)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Destination is Null!]";
                                    else if (notExistFoup)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Can not excute Command. Not Exist Foup!]";
                                    data_check_OK = false;
                                }
                                else if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Clean Vehicle Can't Run Transfer Command!!]";
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    int rv = ProcessDataHandler.Instance.CreateTransferCommand(msg);
                                    if (rv == 0)
                                    {
                                        strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Create Transfer Command";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        if (rv == 1) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                        else if (rv == 2) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command Create Fail!!]";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.Go:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool existDestinationID = msg.DestinationID != 0 && msg.TypeOfDestination == (int)enGoCommandType.ByLocation ? DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(msg.DestinationID) : true;
                                bool destinationIDnull = msg.DestinationID == 0; // Destination 위치가 없다. TransferCommand에는 항상 Destination 위치가 있어야 함.
                                bool abnormal_command = false;
                                abnormal_command |= destinationIDnull;
                                abnormal_command |= existSameCommand;
                                abnormal_command |= existDestinationID == false;

                                bool abnormal_go_distance = false;
                                if (msg.TypeOfDestination == (int)enGoCommandType.ByDistance)
                                {
                                    List<DataItem_Link> links = DatabaseHandler.Instance.DictionaryLinkDataList.Values.ToList();
                                    List<DataItem_Node> nodes = DatabaseHandler.Instance.DictionaryNodeDataList.Values.ToList();

                                    bool notExistNode = DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(msg.DestinationID) == false;
                                    bool distance_check = true;
                                    if (!notExistNode)
                                    {
                                        distance_check &= DatabaseHandler.Instance.DictionaryNodeDataList[msg.DestinationID].Type != NodeType.Lifter;
                                        distance_check &= DatabaseHandler.Instance.DictionaryNodeDataList[msg.DestinationID].Type != NodeType.MTL;
                                    }

                                    List<DataItem_Link> selected_links = links.Where(x => x.FromNodeID == msg.DestinationID).ToList();
                                    bool distanceZero = distance_check ? msg.TargetNodeToDistance <= 0.0f : false;
                                    bool curveType = false;
                                    bool distanceOver = false;
                                    bool autoDoorLink = false;
                                    double link_distance = 0.0f;
                                    foreach (DataItem_Link link in selected_links)
                                    {
                                        curveType |= link.IsCorner();
                                        autoDoorLink |= link.Type == LinkType.AutoDoorCurve;
                                        autoDoorLink |= link.Type == LinkType.AutoDoorStraight;
                                        distanceOver |= link.Distance < msg.TargetNodeToDistance;
                                        if (distanceOver) link_distance = link.Distance;
                                    }

                                    abnormal_go_distance |= notExistNode; if (notExistNode) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Not exist Node!!]";
                                    abnormal_go_distance |= distanceZero; if (distanceZero) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [DistanceFromEndNode is Zero! Check {msg.TargetNodeToDistance}!]";
                                    abnormal_go_distance |= curveType; if (curveType) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Destnation is Corner Area !]";
                                    abnormal_go_distance |= distanceOver; if (distanceOver) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Distance Over {link_distance} < {msg.TargetNodeToDistance}!]";
                                    abnormal_go_distance |= autoDoorLink; if (autoDoorLink) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Destination is Auto Door Area!!]";

                                    // Loop Case일때 CurrentPath Node와 msg.DestinationID 같을 경우
                                    // TargetNodeToDistance와 Current Position의 차이가 Inrange 내일 경우 Finished 처리되는 문제가 있음.
                                    if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID == msg.DestinationID)
                                    {
                                        double leftBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                                        double rightBCR = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                                        double leftBegin = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LeftBCRBegin;
                                        double rightBegin = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RightBCRBegin;
                                        double curDistance = Math.Min(leftBCR - leftBegin, rightBCR - rightBegin);
                                        double setDistance = msg.TargetNodeToDistance;
                                        double diff = setDistance - curDistance;
                                        if (Math.Abs(diff) < 100.0f)
                                        {
                                            if (diff > 0) msg.TargetNodeToDistance += 100.0f;
                                            else msg.TargetNodeToDistance -= 100.0f;
                                        }
                                    }
                                }

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (IsEqpReady() == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Axis Not Ready!!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_command)
                                {
                                    if (existSameCommand)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                    else if (existDestinationID == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Not Find Destination!]";
                                    else if (destinationIDnull)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Destination is Null!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_go_distance)
                                {
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    int rv = ProcessDataHandler.Instance.CreateTransferCommand(msg);
                                    if (rv == 0)
                                    {
                                        strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Create Go Command";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        if (rv == 1) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                        else if (rv == 2) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command Create Fail!!]";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.Teaching:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool existDestinationID = msg.DestinationID != 0 ? DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(msg.DestinationID) : true;
                                bool destinationIDnull = msg.DestinationID == 0; // Destination 위치가 없다. TransferCommand에는 항상 Destination 위치가 있어야 함.
                                bool notExistFoup = msg.ProcessCommand == OCSCommand.Transfer && msg.SourceID == 0 && msg.DestinationID != 0 && !m_GripperPio.IsProductExist(); //Deposit 상황인데 Foup가 없다.
                                bool abnormal_command = false;
                                abnormal_command |= notExistFoup;
                                abnormal_command |= destinationIDnull;
                                abnormal_command |= existSameCommand;
                                abnormal_command |= existDestinationID == false;

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (IsEqpReady() == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Axis Not Ready!!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_command)
                                {
                                    if (existSameCommand)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                    else if (existDestinationID == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Not Find Destination!]";
                                    else if (destinationIDnull)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Destination is Null!]";
                                    else if (notExistFoup)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Can not excute Command. Not Exist Foup!]";
                                    data_check_OK = false;
                                }
                                else if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Clean Vehicle Can't Run Transfer Command!!]";
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    int rv = ProcessDataHandler.Instance.CreateTransferCommand(msg);
                                    if (rv == 0)
                                    {
                                        strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Create AutoTeaching Command";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        if (rv == 1) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                        else if (rv == 2) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command Create Fail!!]";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.DestinationChange:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool destinationIDnull = msg.DestinationID == 0; // Destination 위치가 없다. TransferCommand에는 항상 Destination 위치가 있어야 함.
                                bool existDestinationID = DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(msg.DestinationID);
                                bool validBeforeCommand = curTransferCommand.IsValid;
                                    
                                bool abnormal_command = false;
                                abnormal_command |= destinationIDnull;
                                abnormal_command |= existSameCommand == false;
                                abnormal_command |= existDestinationID == false;
                                abnormal_command |= validBeforeCommand == false;

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (IsEqpReady() == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Axis Not Ready!!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_command)
                                {
                                    if (validBeforeCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Before Command Not Valid]";
                                    else if (existSameCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID} != {curTransferCommand.CommandID}] Reason [Transfer Command ID Difference !!]";
                                    else if (existDestinationID == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Not Find Destination!]";
                                    else if (destinationIDnull)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Destination is Null!]";
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    ProcessDataHandler.Instance.CreateDestinationChangeCommand(msg);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Create Destination Change Command";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.RouteChange:
                            {
                                // HJYOU OCS->VHL RouteChange는 미사용.
                                // 주기적으로 물어보니까 불필요할 것 같다..
                                

                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                VehicleStatus curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;

                                DataItem_Node FromNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsFromNode();
                                DataItem_Node ToNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsToNode();

                                bool isNakState = false;
                                isNakState |= FromNode.Type == NodeType.MTL;
                                isNakState |= FromNode.Type == NodeType.MTLIn;
                                isNakState |= FromNode.Type == NodeType.LifterIn;
                                isNakState |= FromNode.Type == NodeType.Lifter;
                                isNakState |= FromNode.Type == NodeType.LifterOut;
                                isNakState |= FromNode.Type == NodeType.AutoDoorOut1;
                                isNakState |= FromNode.Type == NodeType.AutoDoorOut2;

                                isNakState |= ToNode.Type == NodeType.LifterIn;
                                isNakState |= ToNode.Type == NodeType.Lifter;
                                isNakState |= ToNode.Type == NodeType.LifterOut;
                                isNakState |= ToNode.Type == NodeType.AutoDoorIn1;
                                isNakState |= ToNode.Type == NodeType.AutoDoorIn2;

                                isNakState |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorCurve;
                                isNakState |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorStraight;

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (IsEqpReady() == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Axis Not Ready!!]";
                                    data_check_OK = false;
                                }
                                else if (curTransferCommand.IsValid == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command is Not Valid !!]";
                                    data_check_OK = false;
                                }
                                else if (AlarmCurrentProvider.Instance.IsHeavyAlarm())
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Heavy Alarm Occur State !!]";
                                    data_check_OK = false;
                                }
                                //Transfer Status 확인 필요..
                                else if (curVehicleStatus.CurrentVehicleState == VehicleState.Acquiring)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current State is Acquiring!!]";
                                    data_check_OK = false;
                                }
                                else if (curVehicleStatus.CurrentVehicleState == VehicleState.Depositing)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current State is Depositing!!]";
                                    data_check_OK = false;
                                }
                                else if (isNakState)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current is Route Change Imposible Position!!]";
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    strLog += $"ACK !! CMD ID [{msg.TransferCommandID}] Route Change Command";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.RouteChange, FlagValue.ON);
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.Pause:
                            {
                                bool request_state = m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PauseRequest) == FlagValue.ON;
                                bool pause_state = EqpStateManager.Instance.RunMode == EqpRunMode.Pause;
                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                if (data_check_OK)
                                {
                                    if (!request_state && !pause_state)
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PauseRequest, FlagValue.ON);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Pause";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.Resume:
                            {
                                bool request_state = m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.ResumeRequest) == FlagValue.ON;
                                bool pause_state = EqpStateManager.Instance.RunMode == EqpRunMode.Pause;
                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                if (data_check_OK)
                                {
                                    if (!request_state || pause_state)
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.ResumeRequest, FlagValue.ON);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Resume";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.Cancel:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                VehicleStatus curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool validBeforeCommand = curTransferCommand.IsValid;
                                bool command_state_ng = false;
                                bool command_state_ng1 = false;
                                if (existSameCommand)
                                {
                                    List<TransferCommand> cmds = ProcessDataHandler.Instance.CommandQueue.Where(x => x.CommandID == msg.TransferCommandID).ToList();
                                    foreach (TransferCommand cmd in cmds) { command_state_ng |= cmd.CommandStatus < ProcessStatus.Queued; }

                                    if (curTransferCommand.CommandStatus == ProcessStatus.Canceling) command_state_ng1 = true;
                                }

                                bool command_state_ng_distance = GV.WheelBusy && AlarmCurrentProvider.Instance.IsHeavyAlarm() == false;
                                command_state_ng_distance &= curVehicleStatus.CurrentNode.NodeID == curTransferCommand.TargetNode;
                                command_state_ng_distance &= curTransferCommand.RemainBcrDistance < 1000.0f;

                                bool abnormal_command = false;
                                abnormal_command |= validBeforeCommand == false;
                                abnormal_command |= existSameCommand == false;
                                abnormal_command |= command_state_ng;
                                abnormal_command |= command_state_ng1;
                                abnormal_command |= command_state_ng_distance;

                                if (abnormal_command)
                                {
                                    if (validBeforeCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Before Command Not Valid]";
                                    else if (existSameCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Transfer Command Not Exist !!]";
                                    else if (command_state_ng)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Transfer State is Not Queued!!]";
                                    else if (command_state_ng1)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command Status is Already Canceling!!]";
                                    else if (command_state_ng_distance)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Vehicle is too close to the target position !!]";
                                    data_check_OK = false;
                                }
                                else
                                {
                                    if (curTransferCommand.ProcessCommand == OCSCommand.Transfer)
                                    {
                                        if (AlarmCurrentProvider.Instance.IsHeavyAlarm() == false)
                                        {
                                            if (curVehicleStatus.CurrentVehicleState == VehicleState.Acquiring)
                                            {
                                                strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current State is Acquiring!!]";
                                                data_check_OK = false;
                                            }
                                            else if (curVehicleStatus.CurrentVehicleState == VehicleState.Depositing)
                                            {
                                                strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current State is Depositing!!]";
                                                data_check_OK = false;
                                            }
                                        }
                                    }
                                    if (AlarmCurrentProvider.Instance.IsHeavyAlarm() == false)
                                    {
                                        DataItem_Node FromNode = curVehicleStatus.CurrentPath.IsFromNode();
                                        DataItem_Node ToNode = curVehicleStatus.CurrentPath.IsToNode();

                                        bool isNakState = false;
                                        isNakState |= FromNode.Type == NodeType.MTL;
                                        isNakState |= FromNode.Type == NodeType.MTLIn;
                                        isNakState |= FromNode.Type == NodeType.LifterIn;
                                        isNakState |= FromNode.Type == NodeType.Lifter;
                                        isNakState |= FromNode.Type == NodeType.LifterOut;
                                        isNakState |= FromNode.Type == NodeType.AutoDoorOut1;
                                        isNakState |= FromNode.Type == NodeType.AutoDoorOut2;

                                        isNakState |= ToNode.Type == NodeType.LifterIn;
                                        isNakState |= ToNode.Type == NodeType.Lifter;
                                        isNakState |= ToNode.Type == NodeType.LifterOut;
                                        isNakState |= ToNode.Type == NodeType.AutoDoorIn1;
                                        isNakState |= ToNode.Type == NodeType.AutoDoorIn2;

                                        isNakState |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorCurve;
                                        isNakState |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorStraight;

                                        if (isNakState)
                                        {
                                            strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current is Cancel Imposible Position!!]";
                                            data_check_OK = false;
                                        }
                                    }
                                    else
                                    {
                                        if (AlarmHandler.AutoRecoveryNeed == false) //Alarm 발생했는데 AutoRecovery가 않되는 종류일 경우 Nak
                                        {
                                            strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current Alarm IrrecoverableError !!]";
                                            data_check_OK = false;
                                        }
                                    }
                                }

                                if (data_check_OK)
                                {
                                    // Interface Flag를 모두 지워야 한다.
                                    m_OCSManager.OcsStatus.ResetAllFlag();
                                    if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort) this.InitSeq();
                                    m_OCSManager.OcsStatus.AbortCancelCommandID = msg.TransferCommandID;

                                    if (AlarmCurrentProvider.Instance.IsHeavyAlarm())
                                    {
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.CancelRequest, FlagValue.ON);
                                    }
                                    else
                                    {
                                        AlarmHandler.AutoRecoveryNeed = false;
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.WaitCancelRequest, FlagValue.ON);
                                    }
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Cancel";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.Abort:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                VehicleStatus curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool validBeforeCommand = curTransferCommand.IsValid;
                                bool command_state_ng = false;
                                bool command_state_ng1 = false;
                                if (existSameCommand)
                                {
                                    List<TransferCommand> cmds = ProcessDataHandler.Instance.CommandQueue.Where(x => x.CommandID == msg.TransferCommandID).ToList();
                                    foreach (TransferCommand cmd in cmds) { command_state_ng |= cmd.CommandStatus < ProcessStatus.Queued; }

                                    if (curTransferCommand.CommandStatus == ProcessStatus.Aborting) command_state_ng1 = true;
                                }

                                bool abnormal_command = false;
                                abnormal_command |= validBeforeCommand == false;
                                abnormal_command |= existSameCommand == false;
                                abnormal_command |= command_state_ng;
                                abnormal_command |= command_state_ng1;

                                if (abnormal_command)
                                {
                                    if (validBeforeCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Before Command Not Valid]";
                                    else if (existSameCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Transfer Command Not Exist !!]";
                                    else if (command_state_ng)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Transfer State is Not Queued!!]";
                                    else if (command_state_ng1)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command Status is Already Aborting!!]";
                                    data_check_OK = false;
                                }
                                else
                                {
                                    if (curTransferCommand.ProcessCommand == OCSCommand.Transfer)
                                    {
                                        if (AlarmCurrentProvider.Instance.IsHeavyAlarm() == false)
                                        {
                                            if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Acquiring)
                                            {
                                                strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current State is Acquiring!!]";
                                                data_check_OK = false;
                                            }
                                            else if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Depositing)
                                            {
                                                strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current State is Depositing!!]";
                                                data_check_OK = false;
                                            }
                                        }
                                    }
                                    if (AlarmCurrentProvider.Instance.IsHeavyAlarm() == false)
                                    {
                                        DataItem_Node FromNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsFromNode();
                                        DataItem_Node ToNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsToNode();

                                        bool isNakState = false;
                                        isNakState |= FromNode.Type == NodeType.MTL;
                                        isNakState |= FromNode.Type == NodeType.MTLIn;
                                        isNakState |= FromNode.Type == NodeType.LifterIn;
                                        isNakState |= FromNode.Type == NodeType.Lifter;
                                        isNakState |= FromNode.Type == NodeType.LifterOut;
                                        isNakState |= FromNode.Type == NodeType.AutoDoorOut1;
                                        isNakState |= FromNode.Type == NodeType.AutoDoorOut2;

                                        isNakState |= ToNode.Type == NodeType.LifterIn;
                                        isNakState |= ToNode.Type == NodeType.Lifter;
                                        isNakState |= ToNode.Type == NodeType.LifterOut;
                                        isNakState |= ToNode.Type == NodeType.AutoDoorIn1;
                                        isNakState |= ToNode.Type == NodeType.AutoDoorIn2;

                                        isNakState |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorCurve;
                                        isNakState |= curVehicleStatus.CurrentPath.Type == LinkType.AutoDoorStraight;

                                        if (isNakState)
                                        {
                                            strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current is Cancel Imposible Position!!]";
                                            data_check_OK = false;
                                        }
                                    }
                                    else
                                    {
                                        if (AlarmHandler.AutoRecoveryNeed == false)//Alarm 발생했는데 AutoRecovery가 않되는 종류일 경우 Nak
                                        {
                                            strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Current Alarm IrrecoverableError !!]";
                                            data_check_OK = false;
                                        }
                                    }
                                }

                                if (data_check_OK)
                                {
                                    // Interface Flag를 모두 지워야 한다.
                                    m_OCSManager.OcsStatus.ResetAllFlag();
                                    if (EqpStateManager.Instance.RunMode != EqpRunMode.Abort) this.InitSeq();

                                    m_OCSManager.OcsStatus.AbortCancelCommandID = msg.TransferCommandID;
                                    if (AlarmCurrentProvider.Instance.IsHeavyAlarm())
                                    {
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.AbortRequest, FlagValue.ON);
                                    }
                                    else
                                    {
                                        AlarmHandler.AutoRecoveryNeed = false;
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.WaitAbortRequest, FlagValue.ON);
                                    }
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Abort";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.PowerOn:
                            {
                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                if (data_check_OK)
                                {
                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PowerOnRequest, FlagValue.ON);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Power ON";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.PowerOff:
                            {
                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                if (data_check_OK)
                                {
                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PowerOffRequest, FlagValue.ON);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Power OFF";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.ErrorReset:
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.AlarmClearRequest, FlagValue.ON);
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Error Reset";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;

                        case OCSCommand.BuzzerOff:
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.BuzzerOffRequest, FlagValue.ON);
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Buzzer OFF";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;

                        case OCSCommand.AutoTeachingStart:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool validBeforeCommand = curTransferCommand.IsValid;
                                bool autoTeachingStop = m_OCSManager.OcsStatus.AutoTeachingStatus;
                                autoTeachingStop &= EqpStateManager.Instance.RunMode == EqpRunMode.Stop;

                                bool AutoTeachingRun = m_OCSManager.OcsStatus.AutoTeachingStatus;

                                bool abnormal_command = false;
                                abnormal_command |= validBeforeCommand == false;
                                abnormal_command |= existSameCommand == false;

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_command)
                                {
                                    if (validBeforeCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Before Command Not Valid]";
                                    else if (existSameCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Transfer Command Not Exist !!]";
                                }
                                else if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Clean Vehicle Can't Run Transfer Command!!]";
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    if (autoTeachingStop)
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.AutoTeachingStart, FlagValue.ON);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Auto Teaching Start";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.AutoTeachingStop:
                            {
                                TransferCommand curTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
                                List<string> cmdIDs = ProcessDataHandler.Instance.CommandQueue.Select(x => x.CommandID).ToList();
                                bool existSameCommand = cmdIDs.Count > 0 ? cmdIDs.Contains(msg.TransferCommandID) : false;
                                existSameCommand |= curTransferCommand.IsValid ? curTransferCommand.CommandID == msg.TransferCommandID : false;
                                bool validBeforeCommand = curTransferCommand.IsValid;
                                bool autoTeachingRun = m_OCSManager.OcsStatus.AutoTeachingStatus;
                                autoTeachingRun &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;

                                bool abnormal_command = false;
                                abnormal_command |= validBeforeCommand == false;
                                abnormal_command |= existSameCommand == false;

                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (abnormal_command)
                                {
                                    if (validBeforeCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Before Command Not Valid]";
                                    else if (existSameCommand == false)
                                        strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Transfer Command Not Exist !!]";
                                }
                                else if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Clean Vehicle Can't Run Transfer Command!!]";
                                    data_check_OK = false;
                                }

                                if (data_check_OK)
                                {
                                    if (autoTeachingRun)
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.AutoTeachingStop, FlagValue.ON);
                                    strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Auto Teaching Stop";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.FDCReportStart:
                            break;
                        case OCSCommand.FDCReportStop:
                            break;

                        case OCSCommand.TeachingDataRequest:
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.TeachingDataRequest, FlagValue.ON);
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Teaching Data Request";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;

                        case OCSCommand.PermitInstall:
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.InstallPermit, FlagValue.ON);
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Permit Install";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;

                        case OCSCommand.RefuseInstall:
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.InstallRefuse, FlagValue.ON);
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Refuse Install";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;

                        case OCSCommand.VehicleRemove:
                            {
                                if (!auto)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [OperationMode Not Auto!!]";
                                    data_check_OK = false;
                                }
                                else if (IsEqpReady() == false)
                                {
                                    strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Axis Not Ready!!]";
                                    data_check_OK = false;
                                }
                                if (data_check_OK)
                                {
                                    int rv = ProcessDataHandler.Instance.CreateTransferCommand(msg);
                                    if (rv == 0)
                                    {
                                        strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Create Vehicle Remove Command";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        if (rv == 1) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Already Command!!]";
                                        else if (rv == 2) strLog += $"Nak!! CMD ID [{msg.TransferCommandID}] Reason [Command Create Fail!!]";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;

                        case OCSCommand.DateTimeSet:
                            {
                                ProcessSystemTimeSet(msg.DateTimeSet);
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Set System Time";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;

                        case OCSCommand.DataSendIntervalChange:
                            {
                                m_OCSManager.OcsStatus.MessageSendInterval = msg.DataSendInterval;
                                SetupManager.Instance.SetupOCS.StatusReportIntervalTime = msg.DataSendInterval;
                                SetupManager.Instance.SetupJCS.StatusReportIntervalTime = msg.DataSendInterval;
                                strLog += $"Ack!! CMD ID [{msg.TransferCommandID}] Command Data Send Interval Change - {msg.DataSendInterval}";
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            private void ProcessSystemTimeSet(string CmdTime)
            {
                try
                {
                    SystemTime SetTime = new SystemTime();
                    SetTime.wYear = Convert.ToUInt16(CmdTime.Substring(0, 4));
                    SetTime.wMonth = Convert.ToUInt16(CmdTime.Substring(4, 2));
                    SetTime.wDay = Convert.ToUInt16(CmdTime.Substring(6, 2));
                    SetTime.wHour = Convert.ToUInt16(CmdTime.Substring(8, 2));
                    SetTime.wMinute = Convert.ToUInt16(CmdTime.Substring(10, 2));
                    SetTime.wSecond = Convert.ToUInt16(CmdTime.Substring(12, 2));

                    bool bRv = XFunc.Win32SetLocalTime(ref SetTime);
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog($"ProcessCommand DateTimeSet OK : {CmdTime}");
                }
            }
            private void ProcessTeachingDataSend()
            {
                try
                {
                    List<DataItem_Port> ports = DatabaseHandler.Instance.QueryPort.SelectAllOrNull();

                    VehicleIF_TeachingDataSend message = new VehicleIF_TeachingDataSend();
                    StringBuilder mapData = new StringBuilder();

                    foreach (DataItem_Port port in ports)
                    {
                        mapData.AppendLine(string.Format("{0},{1},{2},", port.PortID, port.BarcodeLeft, port.BarcodeRight));
                    }

                    byte[] memoryBytes = Encoding.ASCII.GetBytes(mapData.ToString());
                    message.SetBinaryDataFromByteArray(memoryBytes);
                    m_OCSManager.OcsComm.SendIFMessage(message);
                }
                catch (Exception ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }
            #endregion
        }
        public class SeqOcsMapDataProcess : XSeqFunc
        {
            private const string FuncName = "[SeqOcsMapDataProcess]";

            #region Fields
            private OCSCommManager m_OCSManager = null;
            private Dictionary<GeneralInformationItemName, string> m_Version = new Dictionary<GeneralInformationItemName, string>();
            private List<DataItem_Node> m_ReceivedNodes = new List<DataItem_Node>();
            private List<DataItem_Link> m_ReceivedLinks = new List<DataItem_Link>();
            private List<DataItem_Port> m_ReceivedPorts = new List<DataItem_Port>();
            private List<DataItem_ErrorList> m_ReceivedErrorLists = new List<DataItem_ErrorList>();
            private List<DataItem_PIODevice> m_ReceivedPioDevices = new List<DataItem_PIODevice>();
            private MapDataType m_CurRequestType = MapDataType.Unknown;
            #endregion

            #region Property
            #endregion

            #region Contructor
            public SeqOcsMapDataProcess()
            {
                this.SeqName = "SeqOcsMapDataProcess";

                m_OCSManager = OCSCommManager.Instance;
                m_Version.Clear();
                m_Version.Add(GeneralInformationItemName.NodeDataVersion, GetGeneralInfo(GeneralInformationItemName.NodeDataVersion));
                m_Version.Add(GeneralInformationItemName.LinkDataVersion, GetGeneralInfo(GeneralInformationItemName.LinkDataVersion));
                m_Version.Add(GeneralInformationItemName.PortDataVersion, GetGeneralInfo(GeneralInformationItemName.PortDataVersion));
                m_Version.Add(GeneralInformationItemName.ErrorListVersion, GetGeneralInfo(GeneralInformationItemName.ErrorListVersion));
                m_Version.Add(GeneralInformationItemName.PIODeviceDataVersion, GetGeneralInfo(GeneralInformationItemName.PIODeviceDataVersion));
                m_OCSManager.OcsStatus.delMapDataProcessRequest += OcsStatus_delMapDataProcessRequest;
            }
            private void OcsStatus_delMapDataProcessRequest(object obj1, object obj2)
            {
                try
                {
                    if (obj2.GetType() == typeof(VehicleIF_MapDataSend))
                    {
                        ProcessCommand(obj1, obj2);
                    }
                    else if (obj2.GetType() == typeof(VehicleIF_MapDataRequest))
                    {
                        ProcessMapDataRequest(obj1, obj2);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion

            #region XSeqFunction overrides
            public override void SeqAbort()
            {
            }

            public override int Do()
            {
                int seqNo = this.SeqNo;
                int nRv = 0;
                try
                {
                    switch (seqNo)
                    {
                        case 0:
                            {
                                if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.NodeDataReceived) == FlagValue.ON)
                                {

                                    // 없어진건 정리하자 !
                                    List<int> remove_keys = new List<int>();
                                    foreach (KeyValuePair<int, DataItem_Node> item in DatabaseHandler.Instance.DictionaryNodeDataList)
                                        if (m_ReceivedNodes.Where(x => x.NodeID == item.Key).ToList().Count == 0) remove_keys.Add(item.Key);
                                    foreach (int key in remove_keys)
                                    {
                                        DatabaseHandler.Instance.DictionaryNodeDataList.Remove(key);
                                        DatabaseHandler.Instance.QueryNode.Delete(key);
                                    }
                                    // DictionaryNodeDataList Update 하자.
                                    foreach (DataItem_Node item in m_ReceivedNodes)
                                    {
                                        if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(item.NodeID))
                                        {
                                            if (DatabaseHandler.Instance.DictionaryNodeDataList[item.NodeID].CompareWith(item) == false)
                                            {
                                                DatabaseHandler.Instance.DictionaryNodeDataList[item.NodeID].SetCopy(item);
                                                DatabaseHandler.Instance.QueryNode.Update(item);
                                            }
                                        }
                                        else
                                        {
                                            DatabaseHandler.Instance.DictionaryNodeDataList.Add(item.NodeID, item);
                                            DatabaseHandler.Instance.QueryNode.Insert(item);
                                        }
                                    }
                                    string version = m_Version[GeneralInformationItemName.NodeDataVersion];
                                    UpdateGeneralInfo(GeneralInformationItemName.NodeDataVersion, version);

                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.NodeDataReceived, FlagValue.OFF);
                                    //if (m_CurRequestType != MapDataType.Unknown) seqNo = 100;
                                }
                                else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.LinkDataReceived) == FlagValue.ON)
                                {
                                    // 없어진건 정리하자 !
                                    List<int> remove_keys = new List<int>();
                                    foreach (KeyValuePair<int, DataItem_Link> item in DatabaseHandler.Instance.DictionaryLinkDataList)
                                        if (m_ReceivedLinks.Where(x => x.LinkID == item.Key).ToList().Count == 0) remove_keys.Add(item.Key);
                                    foreach (int key in remove_keys)
                                    {
                                        DatabaseHandler.Instance.DictionaryLinkDataList.Remove(key);
                                        DatabaseHandler.Instance.QueryLink.Delete(key);
                                    }

                                    // DictionaryLinkDataList Update 하자.
                                    foreach (DataItem_Link item in m_ReceivedLinks)
                                    {
                                        string msg = "";
                                        if (DatabaseHandler.Instance.DictionaryLinkDataList.ContainsKey(item.LinkID))
                                        {
                                            if (DatabaseHandler.Instance.DictionaryLinkDataList[item.LinkID].CompareWith(item, ref msg) == false)
                                            {
                                                DatabaseHandler.Instance.DictionaryLinkDataList[item.LinkID].SetCopy(item);
                                                DatabaseHandler.Instance.QueryLink.Update(item);
                                            }
                                        }
                                        else
                                        {
                                            DatabaseHandler.Instance.DictionaryLinkDataList.Add(item.LinkID, item);
                                            DatabaseHandler.Instance.QueryLink.Insert(item);
                                        }
                                    }
                                    string version = m_Version[GeneralInformationItemName.LinkDataVersion];
                                    UpdateGeneralInfo(GeneralInformationItemName.LinkDataVersion, version);

                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.LinkDataReceived, FlagValue.OFF);
                                    //if (m_CurRequestType != MapDataType.Unknown) seqNo = 100;
                                }
                                else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PortDataReceived) == FlagValue.ON)
                                {
                                    // 없어진건 정리하자 !
                                    List<int> remove_keys = new List<int>();
                                    foreach (KeyValuePair<int, DataItem_Port> item in DatabaseHandler.Instance.DictionaryPortDataList)
                                    {
                                        if (m_ReceivedPorts.Where(x => x.PortID == item.Key).ToList().Count == 0) 
                                        { 
                                            remove_keys.Add(item.Key); 
                                        }

                                    }
                                    foreach (int key in remove_keys)
                                    {
                                        DatabaseHandler.Instance.DictionaryPortDataList.Remove(key);
                                        DatabaseHandler.Instance.QueryPort.Delete(key);
                                    }

                                    // DictionaryLinkDataList Update 하자.
                                    foreach (DataItem_Port item in m_ReceivedPorts)
                                    {
                                        if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(item.PortID))
                                        {
                                            if (DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].CompareWith(item) == false)
                                            {
                                                DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].SetCopyTeachingData(item); //필요한 Data만 Update 하자...!
                                                DatabaseHandler.Instance.QueryPort.Update(item);
                                            }
                                        }
                                        else
                                        {
                                            var sameTypePorts = DatabaseHandler.Instance.DictionaryPortDataList.Values
                                                                .Where(p => p.PortType == item.PortType)
                                                                .ToList();

                                            double GetMostCommon(Func<DataItem_Port, double> selector, double defaultValue)
                                            {
                                                if (sameTypePorts.Count == 0) return defaultValue;
                                                return sameTypePorts
                                                    .GroupBy(selector)
                                                    .OrderByDescending(g => g.Count())
                                                    .First().Key;
                                            }
                                            item.DriveLeftOffset = GetMostCommon(p => p.DriveLeftOffset, item.DriveLeftOffset);
                                            item.DriveRightOffset = GetMostCommon(p => p.DriveRightOffset, item.DriveRightOffset);
                                            item.SlideOffset = GetMostCommon(p => p.SlideOffset, item.SlideOffset);
                                            item.HoistOffset = GetMostCommon(p => p.HoistOffset, item.HoistOffset);
                                            item.RotateOffset = GetMostCommon(p => p.RotateOffset, item.RotateOffset);
                                        
                                            DatabaseHandler.Instance.DictionaryPortDataList.Add(item.PortID, item);
                                            DatabaseHandler.Instance.QueryPort.Insert(item);
                                        }
                                    }
                                    string version = m_Version[GeneralInformationItemName.PortDataVersion];
                                    UpdateGeneralInfo(GeneralInformationItemName.PortDataVersion, version);

                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PortDataReceived, FlagValue.OFF);
                                    //if (m_CurRequestType != MapDataType.Unknown) seqNo = 100;
                                }
                                else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.ErrorListReceived) == FlagValue.ON)
                                {
                                    List<int> remove_keys = new List<int>();
                                    foreach (KeyValuePair<int, DataItem_ErrorList> item in DatabaseHandler.Instance.DictionaryErrorList)
                                        if (m_ReceivedErrorLists.Where(x => x.ID == item.Key).ToList().Count == 0) remove_keys.Add(item.Key);
                                    foreach (int key in remove_keys)
                                    {
                                        DatabaseHandler.Instance.DictionaryErrorList.Remove(key);
                                        DatabaseHandler.Instance.QueryErrorList.Delete(key);
                                    }

                                    // DictionaryLinkDataList Update 하자.
                                    foreach (DataItem_ErrorList item in m_ReceivedErrorLists)
                                    {
                                        if (DatabaseHandler.Instance.DictionaryErrorList.ContainsKey(item.ID))
                                        {
                                            if (DatabaseHandler.Instance.DictionaryErrorList[item.ID].CompareWith(item) == false)
                                            {
                                                DatabaseHandler.Instance.DictionaryErrorList[item.ID].SetCopy(item); //필요한 Data만 Update 하자...!
                                                DatabaseHandler.Instance.QueryErrorList.Update(item);
                                            }
                                        }
                                        else
                                        {
                                            DatabaseHandler.Instance.DictionaryErrorList.Add(item.ID, item);
                                            DatabaseHandler.Instance.QueryErrorList.Insert(item);
                                        }
                                    }
                                    string version = m_Version[GeneralInformationItemName.ErrorListVersion];
                                    UpdateGeneralInfo(GeneralInformationItemName.ErrorListVersion, version);

                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.ErrorListReceived, FlagValue.OFF);
                                    //if (m_CurRequestType != MapDataType.Unknown) seqNo = 100;
                                }
                                else if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PIODataReceived) == FlagValue.ON)
                                {
                                    // 없어진건 정리하자 !
                                    List<int> remove_keys = new List<int>();
                                    foreach (KeyValuePair<int, DataItem_PIODevice> item in DatabaseHandler.Instance.DictionaryPIODevice)
                                        if (m_ReceivedPioDevices.Where(x => x.NodeID == item.Key).ToList().Count == 0) remove_keys.Add(item.Key);
                                    foreach (int key in remove_keys)
                                    {
                                        DatabaseHandler.Instance.DictionaryPIODevice.Remove(key);
                                        DatabaseHandler.Instance.QueryPIODevice.Delete(key);
                                    }

                                    // DictionaryLinkDataList Update 하자.
                                    foreach (DataItem_PIODevice item in m_ReceivedPioDevices)
                                    {
                                        if (DatabaseHandler.Instance.DictionaryPIODevice.ContainsKey(item.NodeID))
                                        {
                                            DatabaseHandler.Instance.DictionaryPIODevice[item.NodeID] = item;
                                            DatabaseHandler.Instance.QueryPIODevice.Update(item);
                                        }
                                        else
                                        {
                                            DatabaseHandler.Instance.DictionaryPIODevice.Add(item.NodeID, item);
                                            DatabaseHandler.Instance.QueryPIODevice.Insert(item);
                                        }
                                    }
                                    string version = m_Version[GeneralInformationItemName.PIODeviceDataVersion];
                                    UpdateGeneralInfo(GeneralInformationItemName.PIODeviceDataVersion, version);

                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PIODataReceived, FlagValue.OFF);
                                    //if (m_CurRequestType != MapDataType.Unknown) seqNo = 100;
                                }

                                if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.MapDataRequest) == FlagValue.ON)
                                {
                                    // MapDataSync
                                    seqNo = 110;
                                }
                            }
                            break;

                        case 100:
                            {
                                m_CurRequestType = MapDataType.Unknown;
                                if (m_OCSManager.OcsStatus.MapDataRequestType == MapDataType.ALL)
                                {
                                    if (m_CurRequestType == MapDataType.ALL) m_CurRequestType = MapDataType.NodeData;
                                    else if (m_CurRequestType == MapDataType.NodeData) m_CurRequestType = MapDataType.LinkData;
                                    else if (m_CurRequestType == MapDataType.LinkData) m_CurRequestType = MapDataType.PortData;
                                    else if (m_CurRequestType == MapDataType.PortData) m_CurRequestType = MapDataType.PIODeviceData;
                                }
                                else
                                {
                                    m_CurRequestType = m_OCSManager.OcsStatus.MapDataRequestType;
                                }
                                seqNo = 110;
                            }
                            break;

                        case 110:
                            {
                                //if (m_CurRequestType != MapDataType.Unknown)
                                SendMapData(m_OCSManager.OcsStatus.MapDataRequestType);

                                //bool finished = m_OCSManager.OcsStatus.MapDataRequestType == MapDataType.ALL && m_CurRequestType == MapDataType.PIODeviceData;
                                //finished |= m_OCSManager.OcsStatus.MapDataRequestType != MapDataType.ALL;
                                //if (finished)
                                {
                                    // 끝 ... 정리하자...
                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.MapDataRequest, FlagValue.OFF);
                                    m_OCSManager.OcsStatus.MapDataRequestType = MapDataType.Unknown;
                                    m_CurRequestType = MapDataType.Unknown;
                                }
                                seqNo = 0;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                this.SeqNo = seqNo;
                return nRv;
            }
            #endregion

            #region Process Function
            private void ProcessCommand(object sender, object eventData)
            {
                VehicleIF_MapDataSend msg = (eventData as VehicleIF_MapDataSend);
                VehicleIF_MapDataReply reply = new VehicleIF_MapDataReply
                {
                    VehicleNumber = msg.VehicleNumber,
                    DataType = msg.DataType,
                    Acknowledge = IFAcknowledge.NAK,
                };
                reply.SetSystemByte(msg.SystemByte);
                string strLog = $"[ProcessCommand] {msg.DataType.ToString()}. ";

                try
                {
                    bool data_check_OK = true;

                    switch (msg.DataType)
                    {
                        case MapDataType.NodeData:
                            {
                                var parseNodes = ParseToNodes(msg);
                                if (parseNodes.Count == 0) data_check_OK = false;

                                if (data_check_OK)
                                {
                                    if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.NodeDataReceived) != FlagValue.ON)
                                    {
                                        m_ReceivedNodes = parseNodes;
                                        m_Version[GeneralInformationItemName.NodeDataVersion] = msg.DataVersion;
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.NodeDataReceived, FlagValue.ON);
                                        strLog += $"Ack!! Node Data Received OK";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        strLog += $"Nak!! Node Data is Processing, Please waitting for finish!";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    strLog += $"Nak!! Node Data Received NG";
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;
                        case MapDataType.LinkData:
                            {
              
                                var parseLinks = ParseToLinks(msg);
                                if (parseLinks.Count == 0) data_check_OK = false;

                                if (data_check_OK)
                                {
                                    if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.LinkDataReceived) != FlagValue.ON)
                                    {
                                        m_ReceivedLinks = parseLinks;
                                        m_Version[GeneralInformationItemName.LinkDataVersion] = msg.DataVersion;
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.LinkDataReceived, FlagValue.ON);
                                        strLog += $"Ack!! Link Data Received OK";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        strLog += $"Nak!! Link Data is Processing, Please waitting for finish!";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    strLog += $"Nak!! Link Data Received NG";
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;
                        case MapDataType.PortData:
                            {
                                var parsePorts = ParseToPorts(msg);
                                if (parsePorts.Count == 0) data_check_OK = false;

                                if (data_check_OK)
                                {
                                    if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.PortDataReceived) != FlagValue.ON)
                                    {
                                        m_ReceivedPorts = parsePorts;
                                        m_Version[GeneralInformationItemName.PortDataVersion] = msg.DataVersion;
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PortDataReceived, FlagValue.ON);

                                        strLog += $"Ack!! Port Data Received OK";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        strLog += $"Nak!! Port Data is Processing, Please waitting for finish!";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    strLog += $"Nak!! Port Data Received NG";
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;
                        case MapDataType.ErrorList:
                            {
                                var parseErrorLists = ParseToErrorList(msg);
                                if (parseErrorLists.Count == 0) data_check_OK = false;

                                if (data_check_OK)
                                {
                                    if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.ErrorListReceived) != FlagValue.ON)
                                    {
                                        m_ReceivedErrorLists = parseErrorLists;
                                        m_Version[GeneralInformationItemName.ErrorListVersion] = msg.DataVersion;
                                        m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.ErrorListReceived, FlagValue.ON);

                                        strLog += $"Ack!! ErrorList Data Received OK";
                                        reply.Acknowledge = IFAcknowledge.ACK;
                                    }
                                    else
                                    {
                                        strLog += $"Nak!! ErrorList Data is Processing, Please waitting for finish!";
                                        reply.Acknowledge = IFAcknowledge.NAK;
                                    }
                                }
                                else
                                {
                                    strLog += $"Nak!! ErrorList Data Received NG";
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;
                        case MapDataType.PIODeviceData:
                            {
                                m_ReceivedPioDevices.Clear();
                                m_ReceivedPioDevices = ParseToPioDevices(msg);
                                if (m_ReceivedPioDevices.Count == 0) data_check_OK = false;

                                if (data_check_OK)
                                {
                                    m_Version[GeneralInformationItemName.PIODeviceDataVersion] = msg.DataVersion;
                                    m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.PIODataReceived, FlagValue.ON);

                                    strLog += $"Ack!! PIO Data Received OK";
                                    reply.Acknowledge = IFAcknowledge.ACK;
                                }
                                else
                                {
                                    strLog += $"Nak!! PIO Data Received NG";
                                    reply.Acknowledge = IFAcknowledge.NAK;
                                }
                            }
                            break;
                        default: break;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            private void ProcessMapDataRequest(object sender, object eventData)
            {
                VehicleIF_MapDataRequest msg = (eventData as VehicleIF_MapDataRequest);
                VehicleIF_MapDataRequestAcknowledge reply = new VehicleIF_MapDataRequestAcknowledge
                {
                    DataType = msg.DataType,
                    VehicleNumber = msg.VehicleNumber,
                    Acknowledge = IFAcknowledge.NAK,
                };
                reply.SetSystemByte(msg.SystemByte);
                string strLog = $"[Map Data Request] DataType [{msg.DataType}] ";

                try
                {
                    bool data_check_OK = true;

                    if (data_check_OK)
                    {
                        m_OCSManager.OcsStatus.MapDataRequestType = msg.DataType;
                        if (msg.DataType == MapDataType.NodeData || msg.DataType == MapDataType.LinkData || msg.DataType == MapDataType.PortData || msg.DataType == MapDataType.ErrorList || msg.DataType == MapDataType.ALL)
                        {
                            if (m_OCSManager.OcsStatus.GetFlag(InterfaceFlag.MapDataRequest) != FlagValue.ON)
                            {
                                m_OCSManager.OcsStatus.SetFlag(InterfaceFlag.MapDataRequest, FlagValue.ON);
                                reply.Acknowledge = IFAcknowledge.ACK;
                            }
                            else
                            {
                                reply.Acknowledge = IFAcknowledge.NAK;
                                strLog += $"Process NG, Reason: Same MapDataRequest is processing, please waitting for finish!";
                            }
                        }
                        else
                        {
                            reply.Acknowledge = IFAcknowledge.NAK;
                            strLog += $"Process NG, Reason: MapDataType is not support!";
                        }
                    }
                    else
                    {
                        reply.Acknowledge = IFAcknowledge.NAK;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    m_OCSManager.OcsComm.SendIFMessage(reply);
                }
            }
            #endregion

            #region Methods - Map Data Update Process
            private List<DataItem_Node> ParseToNodes(VehicleIF_MapDataSend message)
            {
                List < DataItem_Node > nodes = new List < DataItem_Node >();
                try
                {
                    string mapData = Encoding.ASCII.GetString(message.Data);
                    string[] readLines = mapData.Split('\n');

                    foreach (string line in readLines)
                    {
                        if (line.Trim() == string.Empty) continue;

                        string[] datas = line.Trim().Split(',');
                        if (datas.Length < 8) continue;

                        DataItem_Node node = new DataItem_Node
                        {
                            NodeID = Convert.ToInt32(datas[0]),
                            UseFlag = (Convert.ToInt32(datas[1]) == 1) ? true : false,
                            LocationValue1 = Convert.ToDouble(datas[2]),
                            LocationValue2 = Convert.ToDouble(datas[3]),
                            Type = (NodeType)Convert.ToInt32(datas[4]),
                            UBSLevel = Convert.ToInt32(datas[5]),
                            UBSCheckSensor = Convert.ToInt32(datas[6]),
                            JCSCheck = Convert.ToInt32(datas[7])
                        };
                        nodes.Add(node);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return nodes;
            }
            private List<DataItem_Link> ParseToLinks(VehicleIF_MapDataSend message)
            {
                List<DataItem_Link> links = new List<DataItem_Link>();
                try
                {
                    string mapData = Encoding.ASCII.GetString(message.Data);
                    string[] readLines = mapData.Split('\n');

                    foreach (string line in readLines)
                    {
                        if (line.Trim() == string.Empty) continue;

                        string[] datas = line.Trim().Split(',');
                        if (datas.Length < 23) continue;

                        DataItem_Link link = new DataItem_Link
                        {
                            LinkID = Convert.ToInt32(datas[0]), 
                            UseFlag = datas[1]=="1", 
                            FromNodeID = Convert.ToInt32(datas[2]), 
                            ToNodeID = Convert.ToInt32(datas[3]), 
                            Type = (LinkType)Convert.ToInt32(datas[4]), 
                            BCRMatchType = datas[5], 
                            SteerDirectionValue = (enSteerDirection)Convert.ToInt32(datas[6]), 
                            SteerChangeLeftBCR = Convert.ToInt32(datas[7]), 
                            SteerChangeRightBCR = Convert.ToInt32(datas[8]), 
                            Time = Convert.ToDouble(datas[9]), 
                            Weight = 0, // always zer0 
                            Distance = Convert.ToDouble(datas[10]), 
                            Velocity = Convert.ToDouble(datas[11]), 
                            Acceleration = Convert.ToDouble(datas[12]), 
                            Deceleration = Convert.ToDouble(datas[13]), 
                            Jerk = Convert.ToDouble(datas[14]), 
                            UBSLevel0 = Convert.ToInt32(datas[15]), 
                            UBSLevel1 = Convert.ToInt32(datas[16]), 
                            RouteChangeCheck = datas[17]=="1", 
                            SteerGuideLengthFromNode = Convert.ToInt32(datas[18]), 
                            SteerGuideLengthToNode = Convert.ToInt32(datas[19]), 
                            JCSAreaFlag = datas[20] == "1",
                            FromExtensionDistance = Convert.ToInt32(datas[21]),
                            ToExtensionDistance = Convert.ToInt32(datas[22]),
                        };
                        links.Add(link);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return links;
            }
            private List<DataItem_Port> ParseToPorts(VehicleIF_MapDataSend message)
            {
                List<DataItem_Port> Ports = new List<DataItem_Port>();
                try
                {
                    string mapData = Encoding.ASCII.GetString(message.Data);
                    string[] readLines = mapData.Split('\n');

                    foreach (string line in readLines)
                    {
                        if (line.Trim() == string.Empty) continue;

                        string[] datas = line.Trim().Split(',');
                        if (datas.Length < 22) continue;

                        DataItem_Port port = new DataItem_Port
                        {
                            BeforeHoistPosition = Convert.ToDouble(datas[0]),
                            HoistPosition = Convert.ToDouble(datas[1]),
                            SlidePosition = Convert.ToDouble(datas[2]),
                            RotatePosition = Convert.ToDouble(datas[3]),
                            BeforeUnloadHoistPosition = Convert.ToDouble(datas[4]),
                            UnloadHoistPosition = Convert.ToDouble(datas[5]),
                            UnloadSlidePosition = Convert.ToDouble(datas[6]),
                            UnloadRotatePosition = Convert.ToDouble(datas[7]),
                            PortType = (PortType)Convert.ToInt32(datas[8]),
                            PortID = Convert.ToInt32(datas[9]),
                            LinkID = Convert.ToInt32(datas[10]),
                            NodeID = Convert.ToInt32(datas[11]),
                            BarcodeLeft = Convert.ToDouble(datas[12]),
                            BarcodeRight = Convert.ToDouble(datas[13]),
                            PIOID = Convert.ToInt32(datas[14]),
                            PIOCH = Convert.ToInt32(datas[15]),
                            PIOCS = Convert.ToInt32(datas[16]),
                            PIOUsed = Convert.ToBoolean(Convert.ToInt32(datas[17])),
                            PortProhibition = false,      //Port Prohibition은 일단 0으로 주,
                            OffsetUsed = Convert.ToBoolean(Convert.ToInt32(datas[18])),
                            PBSUsed = Convert.ToBoolean(Convert.ToInt32(datas[19])),
                            PBSSelectNo = Convert.ToInt32(datas[20]),
                            ProfileExistPosition = (enProfileExistPosition)Convert.ToInt32(datas[21]),

                        };
                        Ports.Add(port);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return Ports;
            }
            private List<DataItem_ErrorList> ParseToErrorList(VehicleIF_MapDataSend message)
            {
                List<DataItem_ErrorList> errorLists = new List<DataItem_ErrorList>();
                try
                {
                    string mapData = Encoding.ASCII.GetString(message.Data);
                    string[] readLines = mapData.Split('\n');
                    foreach (string line in readLines)
                    {
                        if (line.Trim() == string.Empty) continue;

                        string[] datas = line.Trim().Split(',');
                        if (datas.Length < 6) continue;

                        DataItem_ErrorList error = new DataItem_ErrorList
                        {
                            ID = Convert.ToInt32(datas[0]),
                            Code = Convert.ToInt32(datas[1]),
                            Level = (AlarmType)Convert.ToInt32(datas[2]),
                            Unit = datas[3],
                            Description = datas[4],
                            Comment = datas[5],
                        };
                        errorLists.Add(error);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return errorLists;
            }
            private List<DataItem_PIODevice> ParseToPioDevices(VehicleIF_MapDataSend message)
            {
                List<DataItem_PIODevice> devices = new List<DataItem_PIODevice>();
                try
                {
                    string mapData = Encoding.ASCII.GetString(message.Data);
                    string[] readLines = mapData.Split('\n');

                    foreach (string line in readLines)
                    {
                        if (line.Trim() == string.Empty) continue;

                        string[] datas = line.Trim().Split(',');
                        if (datas.Length < 6) continue;

                        DataItem_PIODevice device = new DataItem_PIODevice
                        {
                            NodeID = Convert.ToInt32(datas[0]),
                            //DriveOffset = Convert.ToDouble(datas[1]), //이건 불필요...
                            PIOID = Convert.ToInt32(datas[2]),
                            PIOCH = Convert.ToInt32(datas[3]),
                            DeviceType = (PIODeviceType)Convert.ToInt32(datas[4]),
                            //OffsetUsed = Convert.ToBoolean(datas[5]),//이건 불필요...
                        };
                        devices.Add(device);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return devices;
            }
            #endregion

            #region Methods - Map Data Request
            private void SendMapData(MapDataType type)
            {
                VehicleIF_MapDataSend msg = new VehicleIF_MapDataSend();
                msg.VehicleNumber = AppConfig.Instance.VehicleNumber;
                msg.DataType = type;

                string strLog = $"[Map Data Send] DataType [{type}] ";

                StringBuilder dataStream = new StringBuilder();
                try
                {
                    if (type == MapDataType.PortData)
                    {
                        msg.DataVersion = GetGeneralInfo(GeneralInformationItemName.PortDataVersion);
                        foreach (DataItem_Port port in DatabaseHandler.Instance.DictionaryPortDataList.Values)
                        {
                            dataStream.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                                                port.BeforeHoistPosition,
                                                port.HoistPosition,
                                                port.SlidePosition,
                                                port.RotatePosition,
                                                port.BeforeUnloadHoistPosition,
                                                port.UnloadHoistPosition,
                                                port.UnloadSlidePosition,
                                                port.UnloadRotatePosition,
                                                (int)port.PortType,
                                                port.PortID,
                                                port.LinkID,
                                                port.NodeID,
                                                port.BarcodeLeft,
                                                port.BarcodeRight,
                                                port.PIOID,
                                                port.PIOCH,
                                                port.PIOCS,
                                                port.PIOUsed ? "1" : "0",
                                                port.OffsetUsed ? "1" : "0",
                                                port.PBSUsed ? "1" : "0",
                                                port.PBSSelectNo,
                                                (int)port.ProfileExistPosition));
                        }
                    }
                    else if (type == MapDataType.NodeData)
                    {
                        msg.DataVersion = GetGeneralInfo(GeneralInformationItemName.NodeDataVersion);
                        foreach (DataItem_Node node in DatabaseHandler.Instance.DictionaryNodeDataList.Values)
                        {
                            dataStream.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                                        node.NodeID,
                                                        node.UseFlag ? "1" : "0",
                                                        node.LocationValue1,
                                                        node.LocationValue2,
                                                        (int)node.Type,
                                                        node.UBSLevel,
                                                        node.UBSCheckSensor,
                                                        node.JCSCheck));
                        }
                    }
                    else if (type == MapDataType.LinkData)
                    {
                        msg.DataVersion = GetGeneralInfo(GeneralInformationItemName.LinkDataVersion);
                        foreach (DataItem_Link link in DatabaseHandler.Instance.DictionaryLinkDataList.Values)
                        {
                            if (DatabaseHandler.Instance.DictionaryNodeDataList.Values.Where(item => item.NodeID == link.FromNodeID).Count() == 0 ||
                                DatabaseHandler.Instance.DictionaryNodeDataList.Values.Where(item => item.NodeID == link.ToNodeID).Count() == 0) continue;

                            dataStream.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20} ",
                                                link.LinkID,
                                                link.UseFlag ? "1" : "0",
                                                link.FromNodeID,
                                                link.ToNodeID,
                                                (int)link.Type,
                                                link.BCRMatchType,
                                                (int)link.SteerDirectionValue,
                                                link.SteerChangeLeftBCR,
                                                link.SteerChangeRightBCR,
                                                link.Time,
                                                link.Distance,
                                                link.Velocity,
                                                link.Acceleration,
                                                link.Deceleration,
                                                link.Jerk,
                                                link.UBSLevel0,
                                                link.UBSLevel1,
                                                link.RouteChangeCheck ? "1" : "0",
                                                link.SteerGuideLengthFromNode,
                                                link.SteerGuideLengthToNode,
                                                link.JCSAreaFlag ? "1" : "0",
												link.FromExtensionDistance,
												link.ToExtensionDistance));
                        }
                    }
                    else if (type == MapDataType.ErrorList)
                    {
                        msg.DataVersion = GetGeneralInfo(GeneralInformationItemName.ErrorListVersion);
                        foreach (DataItem_ErrorList error in DatabaseHandler.Instance.DictionaryErrorList.Values)
                        {
                            dataStream.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}",
                                        error.ID,
                                        error.Code,
                                        (int)error.Level,
                                        error.Unit,
                                        error.Description,
                                        error.Comment));
                        }
                    }
                    else if (type == MapDataType.PIODeviceData)
                    {
                        msg.DataVersion = GetGeneralInfo(GeneralInformationItemName.PIODeviceDataVersion);
                        foreach (DataItem_PIODevice device in DatabaseHandler.Instance.DictionaryPIODevice.Values)
                        {
                            dataStream.AppendLine(string.Format("{0},{1},{2},{3}", device.DeviceType, device.NodeID, device.PIOID, device.PIOCH));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    SequenceOCSLog.WriteLog(strLog);
                    msg.Data = Encoding.ASCII.GetBytes(dataStream.ToString());
                    m_OCSManager.OcsComm.SendIFMessage(msg);
                }
            }
            #endregion
        }
        static public void UpdateGeneralInfo(GeneralInformationItemName itemName, string itemValue)
        {
            try
            {
                if (DatabaseHandler.Instance.DictionaryGeneralInformation.ContainsKey(itemName) == true)
                {
                    DataItem_GeneralInformation info = DatabaseHandler.Instance.DictionaryGeneralInformation[itemName];
                    info.ItemValue = itemValue;
                    DatabaseHandler.Instance.QueryGeneralInformation.Update(info);
                }
                else
                {
                    DataItem_GeneralInformation info = new DataItem_GeneralInformation();
                    info.ItemName = itemName;
                    info.ItemValue = itemValue;
                    DatabaseHandler.Instance.DictionaryGeneralInformation.Add(itemName, info);
                    DatabaseHandler.Instance.QueryGeneralInformation.Insert(info);
                }

                EventHandlerManager.Instance.InvokeDataVersionChanged(itemName, itemValue);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        static public string GetGeneralInfo(GeneralInformationItemName itemName)
        {
            string version = string.Empty;
            try
            {
                if (DatabaseHandler.Instance.DictionaryGeneralInformation.ContainsKey(itemName))
                {
                    version = DatabaseHandler.Instance.DictionaryGeneralInformation[itemName].ItemValue;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return version;
        }
        #endregion
    }
}
