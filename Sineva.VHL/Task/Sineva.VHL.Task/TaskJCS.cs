using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.IF.OCS;
using static Sineva.VHL.Task.TaskOCS;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data;
using Sineva.VHL.Data.Process;
using System.Xml.Linq;
using Sineva.VHL.Device;
using System.Runtime.Remoting.Messaging;
using Sineva.VHL.Device.ServoControl;
using System.Diagnostics;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Library.SimpleWifi;

namespace Sineva.VHL.Task
{
    public class TaskJCS : XSequence
    {
        #region Field
        public static readonly TaskJCS Instance = new TaskJCS();
        public SeqJcsControl JcsControl = null;
        #endregion

        #region Constructor
        public TaskJCS()
        {
            ThreadInfo.Name = string.Format("TaskJCS");

            JcsControl = new SeqJcsControl();
            this.RegSeq(new SeqStatusReport());
            this.RegSeq(JcsControl);
        }
        #endregion
    }
    public class SeqStatusReport : XSeqFunc
    {
        public readonly string FuncName = "SeqJCSStatusReport";

        #region Field
        private JCSCommManager m_JCSManager = null;
        private bool m_StatusDataSendReply = false;
        private uint m_StartNgTicks = 0;
        private AlarmData m_ALM_JCS_Disconnect = null;
        private bool m_NewConnection = false;
        private int m_StatusResponseRetry = 0;
        private int m_ConnectionRetry = 0;
        #endregion

        #region Constructor
        public SeqStatusReport()
        {
            this.SeqName = "SeqJCSStatusReport";
            m_JCSManager = JCSCommManager.Instance;
            m_JCSManager.JcsStatus.delMessageReceived += JcsStatus_delMessageReceived;
            m_JCSManager.JcsStatus.delSocketClose += JcsStatus_delSocketClose;
            m_ALM_JCS_Disconnect = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "JCSConnection", "JCS", "Disconnect Alarm");
        }

        private void JcsStatus_delSocketClose(object obj)
        {
            this.SeqNo = 0;
        }

        private void JcsStatus_delMessageReceived(object obj1, object obj2)
        {
            try
            {
                if (obj2.GetType() == typeof(JCSIF_SRR))
                {
                    m_StatusDataSendReply = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region XSeqFunction overrides
        public override int Do()
        {
            if (m_JCSManager.JcsComm.JcsUse == false) return -1;

            int seqNo = SeqNo;
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
                            if (m_JCSManager.JcsStatus.Connected && !m_JCSManager.JcsStatus.ConnectError)
                            {
                                m_StatusDataSendReply = false;
                                JCSIF_SR message = m_JCSManager.JcsStatus.GetStatusSendMessage();
                                if (message != null)
                                {
                                    bool message_check = true;
                                    message_check &= message.CurrentNode < 0 ? false : true;
                                    message_check &= message.CurrentLink < 0 ? false : true;
                                    message_check &= message.PositionInLink < 0 ? false : true;
                                    if (message_check) m_JCSManager.JcsComm.JCSSendMessage(message);
                                }
                                StartTicks = XFunc.GetTickCount();
                                m_StartNgTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else if (m_JCSManager.JcsStatus.ConnectError)
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
                            string msg = string.Format("JCS Status Not Response - Disconnected process");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
                            seqNo = 100;
                        }
                        else if (XFunc.GetTickCount() - m_StartNgTicks > 3 * 1000)
                        {
                            string msg = string.Format("JCS Status Reply - Not Response");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);

                            JCSIF_SR message = m_JCSManager.JcsStatus.GetStatusSendMessage();
                            if (message != null)
                            {
                                bool message_check = true;
                                message_check &= message.CurrentNode < 0 ? false : true;
                                message_check &= message.CurrentLink < 0 ? false : true;
                                message_check &= message.PositionInLink < 0 ? false : true;
                                if (message_check) m_JCSManager.JcsComm.JCSSendMessage(message);
                            }
                            m_StatusDataSendReply = false;
                            m_StartNgTicks = XFunc.GetTickCount();
                        }
                        else if (m_JCSManager.JcsStatus.Connected == false || m_JCSManager.JcsStatus.ConnectError)
                        {
                            string msg = string.Format("Disconnected");
                            if (m_JCSManager.JcsStatus.ConnectError == true)
                                msg = string.Format("ConnectError");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);

                            seqNo = 0;
                        }
                    }
                    break;

                case 20:
                    {
                        if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupJCS.StatusReportIntervalTime)
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
                        m_JCSManager.DisconnectRequest = true;
                        string msg = string.Format("Socket Disconnection");
                        SequenceJCSLog.WriteLog(FuncName, msg, seqNo);

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 110;
                    }
                    break;

                case 110:
                    {
                        if (m_JCSManager.JcsStatus.Connected == false)
                        {
                            string msg = string.Format("Socket Disconnected");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
                            StartTicks = XFunc.GetTickCount();

                            AlarmId = m_ALM_JCS_Disconnect.ID;
                            EqpAlarm.Set(AlarmId);

                            seqNo = 120;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            string msg = string.Format("Socket Disconnection Timeout Retry");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
                            seqNo = 100;
                        }
                    }
                    break;

                case 120:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 1000) break;

                        m_JCSManager.ConnectRequest = true;
                        string msg = string.Format("Socket Connection");
                        SequenceJCSLog.WriteLog(FuncName, msg, seqNo);

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 130;
                    }
                    break;

                case 130:
                    {
                        if (m_JCSManager.JcsStatus.Connected)
                        {
							EqpAlarm.Reset(AlarmId);
                            string msg = string.Format("Socket Connected : {0}, {1}", m_NewConnection, m_StatusResponseRetry);
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
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
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
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
                            m_JCSManager.DisconnectRequest = true;
                            string msg = string.Format("Socket Connect NG, Socket Disconnection, {0}, {1}", m_ConnectionRetry, m_StatusResponseRetry);
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 210;
                        }
                        else
                            seqNo = 0;
                    }
                    break;

                case 210:
                    {
                        if (m_JCSManager.JcsStatus.Connected == false)
                        {
                            string msg = string.Format("Socket Connect NG, Socket Disconnected");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 220;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            string msg = string.Format("Socket Connect NG, Socket Disconnection Timeout Retry");
                            SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
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
                        SequenceJCSLog.WriteLog(FuncName, msg, seqNo);
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
                    }
                    break;
            }

            SeqNo = seqNo;
            return -1;
        }
        #endregion
   }
    public class SeqJcsControl : XSeqFunc
    {
        public readonly string FuncName = "[SeqJcsControl]";

        #region Field
        private _DevAxis m_MasterAxis = null;

        private JCSCommManager m_JCSManager = null;
        private TransferCommand m_CurTransferCommand = null;
        private VehicleStatus m_CurVehicleStatus = null;

        private bool m_JcsStart = false;
        private bool m_PassRequestReply = false;
        private bool m_PassRequestReplyNg = false;
        private bool m_PermitMesaageRecv = false;
        private List<int> m_FullPaths = new List<int>();
        private List<int> m_FullNodes = new List<int>();
        private JunctionControl m_CurJunction = new JunctionControl();

        private bool m_JcsRunning = false;
        private bool m_ReceivedPermit = false;
        private bool m_PassRequest = false;
        private bool m_PassPermitCheck = false;
        private bool m_ReceivedTimeover = false;
        private UInt32 m_PermitWaitTime = 0;

        private int m_OldLinkId = 0;
        private int m_RequestRetry = 0;
        private int m_PermitRetry = 0;
        private uint m_RequestTimeout = 1000;

        private bool m_JcsNewStart = true;

        private AlarmData m_ALM_JCSPermitWait = null;
        #endregion

        #region Properties
        public bool JcsRunning { get {return m_JcsRunning; } set {m_JcsRunning = value; } }
        public bool ReceivedPermit { get { return m_ReceivedPermit; } set { m_ReceivedPermit = value; } }
        public bool PassRequest { get { return m_PassRequest; } set { m_PassRequest = value; } }
        public bool PassPermitCheck { get { return m_PassPermitCheck; } set { m_PassPermitCheck = value; } }
        public bool ReceivedTimeover { get { return m_ReceivedTimeover; } set { m_ReceivedTimeover = value; } }
        public bool IsJcsArea 
        { 
            get 
            {
                bool rv = false;
                if (m_CurJunction != null) rv = m_CurJunction.IsJcsArea;
                return rv; 
            } 
        }
        #endregion

        #region Constructor
        public SeqJcsControl()
        {
            this.SeqName = "SeqJcsControl";
            m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
            m_JCSManager = JCSCommManager.Instance;
            m_JCSManager.JcsStatus.delMessageReceived += JcsPermit_delMessageReceived;
            if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                m_ALM_JCSPermitWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqJcsControl", "JCS", "Permit Wait Warning");
            else
                m_ALM_JCSPermitWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "SeqJcsControl", "JCS", "Permit Wait Warning");
        }
        private void JcsPermit_delMessageReceived(object obj1, object obj2)
        {
            try
            {
                if (obj2.GetType() == typeof(JCSIF_PS))
                {
                    List<int> jpNumbers = new List<int>();
                    JCSIF_PS msg = (obj2 as JCSIF_PS);
                    string strLog = $"ProcessCommand : Pass Send ";
                    //////////////////////////////////////////////////////////////////////////////
                    if (msg.SendCode == PassSendCode.PassPermit && m_CurJunction != null)
                    {
                        int cur_node = 0;
                        cur_node = m_CurJunction.NodeId;

                        if (msg.PassJunctionNumber.Count == 0)
                        {
                            if (msg.JunctionNumber != 0)
                            {
                                msg.PassJunctionNumber.Clear();
                                msg.PassJunctionNumber.Add(msg.JunctionNumber);
                            }
                            else
                            {
                                if (msg.PassJunctionNumber.Count == 0)
                                {
                                    msg.PassJunctionNumber.Add(cur_node);
                                }
                            }
                        }
                        else
                        {
                            if (msg.PassJunctionNumber.Count == 1 && msg.PassJunctionNumber[0] == 0)
                            {
                                msg.PassJunctionNumber[0] = cur_node;
                            }
                        }

                        jpNumbers.AddRange(msg.PassJunctionNumber);
                        if (m_CurTransferCommand.PathMaps.Count > 0)
                        {
                            foreach (int jp in jpNumbers)
                            {
                                bool last_toNode = false;
                                strLog += string.Format("Received JC_Number-{0}, ", jp);
                                
                                int index0 = m_CurTransferCommand.PathMaps.FindIndex(x => x.ToNodeID == jp && x.JcsPermit == false);
                                int index1 = -1;
                                
                                if (index0 >= 0)
                                {
                                    //명령생성이 되어있을 때 내 앞에 있는거를 확인할것이다..
                                    int find_index = m_CurTransferCommand.PathMaps.FindIndex(x => x.ToNodeID == jp && (x.MotionProc == enMotionProc.inProc || x.MotionProc == enMotionProc.wait));
                                    if (find_index == -1)
                                    {
                                        //합류구간이라 명령생성이 되지않았다.. 
                                        int last_index = m_CurTransferCommand.RunPathMaps.Last().Index;
                                        find_index = m_CurTransferCommand.PathMaps.FindIndex(x => x.ToNodeID == jp && x.JcsPermit == false && x.Index == last_index + 1);
                                    }
                                    if (find_index != -1)
                                    {
                                        for (int i = 0; i <= find_index; i++)
                                            m_CurTransferCommand.PathMaps[i].JcsPermit = true;
                                    }
                                    else
                                    {
                                        strLog += $"Not Find JCS Index. index0 : {index0}, jp : {jp}";
                                    }
                                }
                                else
                                {
                                    index1 = m_CurTransferCommand.PathMaps.FindIndex(x => x.ToNodeID == jp);
                                    if (index1 == m_CurTransferCommand.PathMaps.Count - 1)
                                    {
                                        last_toNode = true; // 마지막이라 JunctionControl에 포함되지 않을 수 있다.
                                        strLog += string.Format("Last ToNode Permit");
                                    }
                                    else if (index1 >= 0)
                                    {
                                        strLog += string.Format("Already Permit, ");
                                    }
                                    else
                                    {
                                        strLog += string.Format("Not include PathMaps, ");
                                    }
                                }

                                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                                {
                                    //int index = m_JCSManager.JcsStatus.JunctionControl.FindLastIndex(x => x.NodeId == jp && x.PermitDone == false); // jp는 stop node 다음이니까 그 전까지 permitdone 해도 될것 같음.
                                    if (jp == 0) // JCS Area 내에서 출발할 경우 붙어 있는 JunctionControl은 PermitDone 처리
                                    {
                                        int index = m_JCSManager.JcsStatus.JunctionControl.FindIndex(x => x.NodeId == jp && x.PermitDone == false); // Node 중복 될 경우 last부터 찾으면 않된다.
                                        if (last_toNode && index < 0) index = index1;
                                        if (index >= 0)
                                        {
                                            for (int i = index; i < m_JCSManager.JcsStatus.JunctionControl.Count; i++)
                                            {
                                                int linkid = m_JCSManager.JcsStatus.JunctionControl[i].LinkId;
                                                if (IsJcsInArea(linkid)) m_JCSManager.JcsStatus.JunctionControl[i].PermitDone = true;
                                                else break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int index = m_JCSManager.JcsStatus.JunctionControl.FindIndex(x => x.NodeId == jp && x.PermitDone == false); // Node 중복 될 경우 last부터 찾으면 않된다.
                                        if (index < 0)
                                        {
                                            if (index1 < 0) //PathMaps에 없는 경우
                                            {
                                                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                                                {
                                                    if (m_JCSManager.JcsStatus.JunctionControl[0].IsJcsArea) // 시작점이 JCS Area 내에서 출발하는 경우냐 ?
                                                    {
                                                        for (int i = 0; i < m_JCSManager.JcsStatus.JunctionControl.Count; i++)
                                                        {
                                                            int linkid = m_JCSManager.JcsStatus.JunctionControl[i].LinkId;
                                                            if (IsJcsInArea(linkid)) m_JCSManager.JcsStatus.JunctionControl[i].PermitDone = true;
                                                            else break;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (last_toNode && index1 >= 0)
                                            {
                                                index = index1;
                                            }
                                        }
                                        for (int i = 0; i <= index; i++)
                                        {
                                            if (i < m_JCSManager.JcsStatus.JunctionControl.Count)
                                                m_JCSManager.JcsStatus.JunctionControl[i].PermitDone = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            strLog += $"need Check PathMap. pathMap Count = {m_CurTransferCommand.PathMaps.Count}. Not Excuted JCS Pass.";
                        }
                        m_PermitMesaageRecv = true;
                    }

                    m_JCSManager.JcsStatus.CommMessage = string.Format("{0}-{1}, Junction : ", msg.SendCode, string.Join("-", jpNumbers));
                    strLog += m_JCSManager.JcsStatus.CommMessage;
                    strLog += m_CurJunction != null ? m_CurJunction.ToString() : "null";
                    //////////////////////////////////////////////////////////////////////////////

                    //////////////////////////////////////////////////////////////////////////////
                    JCSIF_PSR replyMessage = new JCSIF_PSR();
                    replyMessage.SetSystemByte((UInt16)msg.SystemByte);
                    replyMessage.VehicleNumber = AppConfig.Instance.VehicleNumber;
                    replyMessage.JunctionNumber = msg.JunctionNumber;
                    replyMessage.ReplyCode = msg.PassJunctionCount == 0 ? PassSendCode.PassWait : PassSendCode.PassPermit;
                    if (msg.PassJunctionCount > 0) jpNumbers.AddRange(msg.PassJunctionNumber);
                    m_JCSManager.JcsComm.JCSSendMessage(replyMessage);
                    //////////////////////////////////////////////////////////////////////////////
                    SequenceJCSLog.WriteLog(FuncName, strLog);
                }
                else if (obj2.GetType() == typeof(JCSIF_PRR))
                {
                    JCSIF_PRR msg = (obj2 as JCSIF_PRR);
                    if (msg.Result == PassRequestReply.OK) m_PassRequestReply = true;
                    else m_PassRequestReplyNg = true;
                    
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public override void SeqAbort()
        {
            if (AlarmId > 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }

            m_JCSManager.JcsStatus.JunctionControl.Clear();
            m_FullPaths.Clear();
            m_FullNodes.Clear();
            m_PermitMesaageRecv = false;
            m_PassRequestReply = false;
            m_JcsStart = false;
            if (m_JcsRunning)
            {
                m_JcsRunning = false;
                // Cancel
                int cur_node = 0;
                if (m_CurJunction != null) cur_node = m_CurJunction.NodeId;
                JCSCancelRequest(cur_node);
            }
            InitSeq();
        }
        #endregion

        #region Method - Override
        /// <summary>
        /// 1. JCS Start => JCS Running
        /// 2. m_CurJunction = GetNextJunction()
        /// 3. Request Condition => Permit Request Send 
        /// 4. Reply Message Timeover => Retry
        /// </summary>
        /// <returns></returns>
        public override int Do()
        {
            if (m_JCSManager.JcsComm.JcsUse == false) return -1;

            m_CurTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
            m_CurVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;

            ReceivedPermit = IsPermit(m_CurVehicleStatus.CurrentPath.Index, m_CurVehicleStatus.CurrentPath.ToNodeID);
            PassRequest = IsPassRequest(m_CurVehicleStatus.CurrentPath.Index, m_CurVehicleStatus.CurrentPath.ToNodeID);
            PassPermitCheck = m_CurJunction != null ? m_CurTransferCommand.TargetNodeOfCommandSet == m_CurJunction.NodeId : false;
            //if (AppConfig.Instance.Simulation.MY_DEBUG && m_CurJunction != null && m_CurJunction.NodeId != 0)
            //{
            //    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
            //    System.Diagnostics.Debug.WriteLine($"{time}, JCS_PassPermitCheck,      {PassPermitCheck},{PassRequest},{ReceivedPermit}, TargetNode, {m_CurTransferCommand.TargetNodeOfCommandSet}, NodeId, {m_CurJunction.NodeId},  {m_CurVehicleStatus.CurrentPath},  {ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink}");
            //}

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (m_JcsStart)
                        {
                            m_JcsStart = false;
                            this.scm.Start = false;

                            m_JCSManager.JcsStatus.JunctionControl.Clear();
                            // 만일 출발점이 JCSArea 내에 있는 경우. PassRequest를 받고 출발 하자..
                            int nn = 0;
                            if (IsJcsInArea())
                            {
                                //double bcrLeft = m_CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                                //double bcrRight = m_CurVehicleStatus.CurrentBcrStatus.RightBcr;
                                //DataItem_Node nearNode = m_CurVehicleStatus.CurrentPath.IsNearNode(bcrLeft, bcrRight); // 무조건 ToNode를 선택할 수 없다.
                                //JCS Check를 강제로 만들자 !
                                JunctionControl jc = new JunctionControl()
                                {
                                    index = nn++,
                                    //NodeId = nearNode.JCSCheck > 0 ? nearNode.NodeID : 0,
                                    NodeId = 0,
                                    LinkId = m_CurVehicleStatus.CurrentPath.LinkID,
                                    JcsStop = true,
                                    RequestDone = false, // RequestDone true일 경우 재 명령을 못 내리도록 막자 !,,, 단 시간이 지나도 PermitDone이 오지 않는 경우는 Retry...
                                    PermitDone = false,  // Pass Permit 받으면 true,
                                    CheckDistance = 6200, // default 값을 넣자 !
                                    IsJcsArea = true, // JCS Area에서 출발할때만 설정하자 !
                                };
                               m_JCSManager.JcsStatus.JunctionControl.Add(jc);
                            }

                            int path_count = m_FullPaths.Count;
                            for (int i = 0; i < path_count; i++) //마지막이 JCS Stop일 경우에만 JCS Request 할 필요 없다.
                            {
                                int linkId = 0;
                                linkId = m_FullPaths[i];
                                int node_index = i + 1;
                                if (node_index >= m_FullNodes.Count) continue;
                                DataItem_Node node = DatabaseHandler.Instance.GetNodeDataOrNull(m_FullNodes[node_index]);
                                if (node_index == m_FullNodes.Count - 1 &&  node.JCSCheck > 0) continue; 

                                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                                {
                                    if (m_JCSManager.JcsStatus.JunctionControl.Last().NodeId == node.NodeID) continue; //연속으로 들어가는건 방지하자 ~~~
                                }
                                JunctionControl jc = new JunctionControl()
                                {
                                    index = nn++,
                                    NodeId = node.NodeID,
                                    LinkId = linkId,
                                    JcsStop = node.JCSCheck > 0,
                                    RequestDone = false, // RequestDone true일 경우 재 명령을 못 내리도록 막자 !,,, 단 시간이 지나도 PermitDone이 오지 않는 경우는 Retry...
                                    PermitDone = false,  // Pass Permit 받으면 true,
                                    CheckDistance = node.JCSCheck,
                                    IsJcsArea = IsJcsInArea(linkId),
                                };
                                m_JCSManager.JcsStatus.JunctionControl.Add(jc);

                                if (m_CurTransferCommand.PathMaps.Count > 0)
                                {
                                    Path path = m_CurTransferCommand.PathMaps.Find(x => x.ToNodeID == node.NodeID && x.Index >= i);
                                    if (path != null) { path.JcsPermit = false; }
                                }
                            }
                            m_JcsRunning = true;
                            m_JcsNewStart = true;
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Start ! {0}", string.Join("-", m_FullNodes)));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        m_CurJunction = GetNextJunction();
                        if (m_CurJunction == null)
                        {
                            // 더이상 찾을게 없다 모든 정보를 지우자 !
                            //m_JcsRunning = false;
                            //m_JCSManager.JcsStatus.JunctionControl.Clear();
                            //m_FullNodes.Clear();
                            SequenceJCSLog.WriteLog(FuncName, string.Format("Selected Next Junction null"));

                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                        else
                        {
                            m_RequestRetry = 0;
                            m_PermitRetry = 0;
                            m_OldLinkId = m_CurJunction.LinkId; // m_CurVehicleStatus.CurrentPath.LinkID;
                            SequenceJCSLog.WriteLog(FuncName, string.Format("Selected Next Junction {0}", m_CurJunction.ToString()));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        // Pass Request Time
                        // 0. Target Node == JCSStop (JCSCheck > 0)
                        // 1. JCSReqDone == false && JCSPermitDone == false
                        // 2. Override가 걸린 상태 && collision distance < target distance  == false
                        // 3. IsCorner() == false
                        // 4. RemainBcrDistance < JCSCheck
                        // 5. 이동중일때
                        //   i) vitual bcr 값이 > 0
                        // JCS Area 내에서 출발할 경우, JSC Stop 위치에서 출발할때...
                        // 이때는 무조건 PassRequest를 날려 허락을 받고 출발해야 한다.
                        double bcrLeft = m_CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                        double bcrRight = m_CurVehicleStatus.CurrentBcrStatus.RightBcr;
                        DataItem_Node nearNode = m_CurVehicleStatus.CurrentPath.IsNearNode(bcrLeft, bcrRight); // 무조건 ToNode를 선택할 수 없다.
                        int toNode = m_CurVehicleStatus.CurrentPath.ToNodeID;

                        int targetNode = m_CurTransferCommand.TargetNodeOfCommandSet;

                        bool vehicle_moving_cmd = false; // 이동한다는 명령을 받고 이동 중인 상황
                        vehicle_moving_cmd |= m_CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource;
                        vehicle_moving_cmd |= m_CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest;
                        vehicle_moving_cmd |= m_CurVehicleStatus.GetVehicleStatus() == VehicleState.Go;
                        bool pass_reqeust = true;
                        pass_reqeust &= m_CurJunction.RequestDone == false || m_ReceivedTimeover;
                        pass_reqeust &= m_CurJunction.PermitDone == false || m_JcsNewStart;
                        if (!GV.WheelBusy)
                        {
                            // 멈춘 상태에서 출발할때는 만일 nearNode가 JCS 일 경우는 request 해야 한다.
                            // Auto Door 일 경우에는 NodeID 비교를 하지 말자. nearNode.NodeID가 CurJunction.NodeId가 다를 경우가 생긴다.
                            bool auto_door = false;
                            auto_door |= nearNode.Type == NodeType.AutoDoorIn1;
                            auto_door |= nearNode.Type == NodeType.AutoDoorOut1;
                            auto_door |= nearNode.Type == NodeType.AutoDoorIn2;
                            auto_door |= nearNode.Type == NodeType.AutoDoorOut2;
                            if (!auto_door)
                                pass_reqeust &= nearNode.NodeID == m_CurJunction.NodeId || toNode == m_CurJunction.NodeId || m_CurJunction.NodeId == 0;                             
                            pass_reqeust &= m_CurVehicleStatus.CurrentPath.RemainDistanceOfLink < m_CurJunction.CheckDistance || m_CurJunction.IsJcsArea;
                            pass_reqeust &= m_CurVehicleStatus.CurrentPath.RemainDistanceOfLink < m_CurVehicleStatus.ObsStatus.CollisionDistance || m_CurJunction.IsJcsArea;
                            pass_reqeust &= m_CurVehicleStatus.ObsStatus.ObsUpperSensorState < enFrontDetectState.enDeccelation6; // 3000mm 이상인 경우
                            pass_reqeust &= m_CurVehicleStatus.ObsStatus.MxpOverrideRatio > 0;
                        }
                        else
                        {
                            pass_reqeust &= targetNode == m_CurJunction.NodeId || m_CurJunction.IsJcsArea; // JCS Area 내에서 출발할때는 Target Node 확인 불필요.
                            pass_reqeust &= m_CurTransferCommand.RemainBcrDistance < m_CurJunction.CheckDistance || m_CurJunction.IsJcsArea; // JCS Area 내에서 출발할때는 Remain Distance 확인 불필요.
                            pass_reqeust &= m_CurTransferCommand.RemainBcrDistance < m_CurVehicleStatus.ObsStatus.CollisionDistance;
                            pass_reqeust &= m_CurVehicleStatus.ObsStatus.OverrideRatio > 0;
                        }

                        // 신규 명령이 TargetNode 설정 후 TargetBcrDistance가 update 되기때문에 조건이 만족되어 pass_request가 바로 나갈수 있다....
                        // TargetBcrDistance가 계산된 후 확인할수 있는 TargetNode가 필요하다.

                        pass_reqeust &= GV.WheelBusy ? m_CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr > 0 : true; //처음 출발할때는 BCR값이 0일거다...
                        pass_reqeust &= vehicle_moving_cmd;
                        if (pass_reqeust)
                        {
                            m_JcsNewStart = false;
                            string msg = string.Format("RemainBcrDistance={0}, CheckDistance={1}， CollisionDistance={2}, VirtualRunBcr={3}", 
                                m_CurTransferCommand.RemainBcrDistance, m_CurJunction.CheckDistance, 
                                m_CurVehicleStatus.ObsStatus.CollisionDistance, m_CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr);
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Pass Request Condition {0}, {1}", m_CurJunction.ToString(), msg));

                            StatusReport();
                            seqNo = 25;
                        }
                        else if (m_CurJunction.PermitDone)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("Already Permit Done Junction {0}", m_CurJunction.ToString()));
                            //seqNo = 10;

                            m_OldLinkId = m_CurJunction.LinkId; // m_CurVehicleStatus.CurrentPath.LinkID; //cur junction permit done이 했으니 move out 되면 다음꺼 확인하는게 맞을것 같음.
                            seqNo = 110;
                        }
                        else if (m_JcsStart)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Start Requested !"));
                            seqNo = 0;
                        }
						else if (XFunc.GetTickCount() - StartTicks > 5 * 1000 && (targetNode == m_CurJunction.NodeId || m_CurJunction.IsJcsArea))
                        {
                            ///////////////////////////////////////////////////////////////
                            string msg = string.Format("PassRequest TimeOver [SeqJcsControlInformation]\r\n");
                            msg += string.Format("[CurJunction, CurJunctionRequestDone, CurJunctionPermitDone] [ReceivedTimeover, JcsNewStart, CurJunctionNodeId, NearNode] [JcsArea, CurJunctionCheckDistance, RemainDistanceOfLink, RemainBcrDistance] [targetNode, VirtualRunBcr, CollisionDistance]\r\n");
                            msg += string.Format("[{0}, {1}, {2}], ", m_CurJunction, m_CurJunction.RequestDone, m_CurJunction.PermitDone);
                            msg += string.Format("[{0}, {1}, {2}, {3}], ", m_ReceivedTimeover, m_JcsNewStart, m_CurJunction.NodeId, nearNode.NodeID);
                            msg += string.Format("[{0}, {1}, {2}, {3}], ", m_CurJunction.IsJcsArea, m_CurJunction.CheckDistance, m_CurVehicleStatus.CurrentPath.RemainDistanceOfLink, m_CurTransferCommand.RemainBcrDistance);
                            msg += string.Format("[{0}, {1}, {2}]\r\n", targetNode, m_CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr, m_CurVehicleStatus.ObsStatus.CollisionDistance);
                            ///////////////////////////////////////////////////////////////
                            SequenceJCSLog.WriteLog(FuncName, msg);

                            StartTicks = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 25:
                    {
                        int jp = m_CurJunction.NodeId; // JP Area IN,
                        int curNode = m_CurVehicleStatus.CurrentPath.FromNodeID;
                        if (m_FullNodes.Contains(curNode) == false) curNode = m_CurVehicleStatus.CurrentPath.ToNodeID;
                        int curLink = m_CurVehicleStatus.CurrentPath.LinkID;
                        int lastlink_Remain = 1000;
                        if (m_CurTransferCommand.PathMaps.Count > 0)
                        {
                            lastlink_Remain = m_CurTransferCommand.PathMaps.Count > 0 ? (int)m_CurTransferCommand.PathMaps.Last().RunDistance : 0;
                        }

                        int cur_index = m_CurJunction.index - 1;
                        if (cur_index < 0) cur_index = 0;
                        int path_count = m_FullNodes.Count - cur_index;
                        List<int> request_paths = m_FullNodes.GetRange(cur_index, path_count);
                        if (request_paths.Count > 0 && request_paths[0] != curNode)
                        {
                            request_paths.Clear();
                            int path_index = m_CurVehicleStatus.CurrentPath.Index;
                            if (GV.WheelBusy) path_index--; //이동 중일때는 이전꺼까지 포함하자....!
                            if (path_index < 0) path_index = 0;
                            path_count = m_FullNodes.Count - path_index;
                            request_paths = m_FullNodes.GetRange(path_index, path_count);
                        }
                        JCSPassRequest(jp, curNode, curLink, request_paths, lastlink_Remain);

                        int set_time = (m_RequestRetry + 1) * 1000; // 처음에는 빨리 보냈다가 천천히 반복하자
                        if (set_time > SetupManager.Instance.SetupJCS.MessageRepayTimeOut)
                            set_time = SetupManager.Instance.SetupJCS.MessageRepayTimeOut;
                        m_RequestTimeout = (uint)set_time;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 30;
                    }
                    break;

                case 30:
                    {
                        // 받았다고 하네...
                        if (m_PermitMesaageRecv)
                        {
                            if (m_CurJunction.PermitDone) // 엉뚱한 놈을 받았으면....100으로 가지 말자~~
                            {
                                SequenceJCSLog.WriteLog(FuncName, string.Format("Permit Received 1-st"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 100;
                            }
                            else
                            {
                                SequenceJCSLog.WriteLog(FuncName, $"Permit PermitDone Fail.");
                                m_PermitMesaageRecv = false; //이게 문제가 된다...
                            }
                        }
                        else if (m_PassRequestReply)
                        {
                            m_PassRequestReply = false;
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassRequest Reply OK"));

                            //////////////////////////////////////////////////////////////////////////////
                            ///뒤쪽에서 부터 찾으면 Node 중복 이동할때 문제가 발생하네....
                            m_CurJunction.RequestDone = true;
                            foreach (JunctionControl junction in m_JCSManager.JcsStatus.JunctionControl)
                            {
                                if (junction.index < m_CurJunction.index) junction.RequestDone = true; //이전꺼도 모두 완료 시키자~~
                            }
                            
                            //int jp = m_CurJunction.NodeId;
                            //JunctionControl jc = m_JCSManager.JcsStatus.JunctionControl.FindLast(x => x.NodeId == jp);
                            //JunctionControl jc = m_JCSManager.JcsStatus.JunctionControl.Find(x => x.NodeId == jp && x.RequestDone == false);             
                            //if (jc != null) { jc.RequestDone = true; }
                            //////////////////////////////////////////////////////////////////////////////

                            StartTicks = XFunc.GetTickCount();

                            double velocity = m_MasterAxis.GetCurVelocity();
                            m_PermitWaitTime = Math.Abs(velocity) < 1.0f ? 0 : (UInt32)(2 * m_CurTransferCommand.RemainBcrDistance / velocity) * 1000;
                            if (m_PermitWaitTime < SetupManager.Instance.SetupJCS.PermitWaitTime)
                                m_PermitWaitTime = (UInt32)SetupManager.Instance.SetupJCS.PermitWaitTime;
                            seqNo = 100;
                        }
                        else if (m_PassRequestReplyNg)
                        {
                            m_PassRequestReplyNg = false;
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassRequest Reply NG"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 40;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > m_RequestTimeout)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassRequest Reply NG, Retry"));
                            m_RequestRetry++;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (m_JcsStart)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Start Requested !"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 40:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 5000)
                        {
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 100:
                    { 
                        if (m_PermitMesaageRecv)
                        {
                            if (AlarmId != 0)
                            {
                                SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassPermit Wait Reset Warning"));
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                            }
                            m_ReceivedTimeover = false;
                            m_PermitMesaageRecv = false;
                            if (m_CurJunction.PermitDone)
                            {
                                SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassPermit Received OK"));
                                m_OldLinkId = m_CurJunction.LinkId; // m_CurVehicleStatus.CurrentPath.LinkID;
                                seqNo = 110; // 다음꺼 진행해라
                            }
                            else
                            {
                                // 정상적으로 junction number를 받지 못했다. 다시 요청하자 !
                                m_CurJunction.RequestDone = false;
                                SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassPermit Received, Junction Number null ! Retry Request"));
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupJCS.PermitWaitTime)
                        {
                            m_ReceivedTimeover = true;
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassPermit TimeOver, Retry !"));

                            if (m_PermitRetry > SetupManager.Instance.SetupJCS.PermitRetryCount && AlarmId == 0)//60s
                            {
                                SequenceJCSLog.WriteLog(FuncName, string.Format("JCS PassPermit Wait Set Warning"));
                                AlarmId = m_ALM_JCSPermitWait.ID;
                                EqpAlarm.Set(AlarmId);
                            }

                            m_PermitRetry++;

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                        else if (m_JcsStart)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Start Requested !"));
                            seqNo = 0;
                        }
                    }
                    break;

                case 110:
                    {
                        bool move_out = true;
                        bool now_jcs_area = IsJcsInArea(m_CurVehicleStatus.CurrentPath.LinkID);
                        // 마지막 Permit Done을 지나간 후
                        int jc_index = -1, cur_index = -1;
                        if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                        {
                            int last_permit_index = m_JCSManager.JcsStatus.JunctionControl.FindLastIndex(x => x.PermitDone == true);
                            int cur_node_index = -1;
                            if (m_JCSManager.JcsStatus.JunctionControl.Count(x => x.LinkId == m_CurVehicleStatus.CurrentPath.LinkID) > 1) //이때는 loop로 판단...
                            {
                                cur_node_index = now_jcs_area ? GetNodeIndex(m_CurVehicleStatus.CurrentPath.ToNodeID) : GetNodeIndex(m_CurVehicleStatus.CurrentPath.FromNodeID);
                            }
                            else
                            {
                                cur_node_index = now_jcs_area ? m_JCSManager.JcsStatus.JunctionControl.FindLastIndex(x => x.NodeId == m_CurVehicleStatus.CurrentPath.ToNodeID) :
                                    m_JCSManager.JcsStatus.JunctionControl.FindLastIndex(x => x.NodeId == m_CurVehicleStatus.CurrentPath.FromNodeID); // ToNodeID를 찾으니까 마지막껀 해당사항이 없어 완료되지 못함.
                            }
                            jc_index = m_CurJunction.index; // GetNodeIndex(m_CurJunction.NodeId);
                            cur_index = m_CurVehicleStatus.CurrentPath.Index; // GetNodeIndex(m_CurVehicleStatus.CurrentPath.ToNodeID);
                            if (cur_index > 0)//cur_index는 0보다 클때 의미가 있겠지...
                            {
                                if (m_JCSManager.JcsStatus.JunctionControl.Count >= m_FullPaths.Count) move_out &= cur_index >= jc_index;
                                else move_out &= cur_index > jc_index;
                            }
                            move_out &= cur_node_index >= last_permit_index;
                            //if (cur_node_index > -1) // ToNodeID를 찾지 못했다 .... 굳이 확인하지 말자~~
                            //{
                            //}
                            //move_out &= cur_index > last_permit_index;
                        }

                        if (m_CurVehicleStatus.CurrentPath.Type != LinkType.LeftBranchStraight && 
                            m_CurVehicleStatus.CurrentPath.Type != LinkType.RightBranchStraight &&
                            m_CurVehicleStatus.CurrentPath.Type != LinkType.LeftJunctionStraight &&
                            m_CurVehicleStatus.CurrentPath.Type != LinkType.RightJunctionStraight &&
                            m_CurVehicleStatus.CurrentPath.Type != LinkType.Straight) move_out &= IsJcsInArea() == false; // Branch Straight 같은 경우 다음으로 넘어가자 ! 다음 JCS까지 거리가 짧은 경우 감속되는 현상이 있다.
                        move_out &= m_OldLinkId != m_CurVehicleStatus.CurrentPath.LinkID;
                        if (move_out)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Move Out Complete OK _ CurrentLinkID={0}, m_OldLinkId={1}, jc_index={2}, cur_index={3}", m_CurVehicleStatus.CurrentPath.LinkID, m_OldLinkId, jc_index, cur_index));
                            seqNo = 10; // 다음꺼 진행해라
                        }
                        else if (m_JcsStart)
                        {
                            SequenceJCSLog.WriteLog(FuncName, string.Format("JCS Start Requested !"));
                            seqNo = 0;
                        }

                        //if (AppConfig.Instance.Simulation.MY_DEBUG && m_CurJunction != null && m_CurJunction.NodeId != 0)
                        //{
                        //    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                        //    System.Diagnostics.Debug.WriteLine($"{time} JCS_PassPermitCheck {cur_index}>{jc_index},{cur_node_index}>={last_permit_index},{m_OldLinkId}!={m_CurVehicleStatus.CurrentPath.LinkID}, {m_CurVehicleStatus.CurrentPath}");
                        //}
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            EqpAlarm.Reset(AlarmId);
                            SequenceJCSLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
                            AlarmId = 0;
                            seqNo = ReturnSeqNo;
                        }
                    }
                    break;
            }

            this.SeqNo = seqNo;
            return -1;
        }
        #endregion

        #region Methods
        public void JCSStart(TransferCommand command /*List<int> fullPath*/)
        {
            try
            {
                m_JcsRunning = false;
                this.InitSeq();
                m_FullPaths.Clear();
                m_FullPaths = command.PathMaps.Select(x => x.LinkID).ToList();

                m_FullNodes.Clear();
                // First / Last는 지우자 ~~ Last만 지우자 ... 마지막꺼를 포함하도록 수정하자 ~~~ JCS에서 마지막꺼는 뺀다고 하네...
                for (int i = 0; i < command.FullPathNodes.Count; i++)
                {
                    m_FullNodes.Add(command.FullPathNodes[i]);
                }
                // 만일 m_FullNodes의 마지막이 JCS Node 일 경우에는 fullPath 마지막껄 추가하자~~
                //if (m_FullNodes.Count > 0)
                //{
                //    DataItem_Node node = DatabaseHandler.Instance.GetNodeDataOrNull(m_FullNodes.Last());
                //    if (node != null) 
                //    {
                //        if (node.JCSCheck > 0) { m_FullNodes.Add(command.FullPathNodes.Last()); }
                //    }
                //}

                for (int i = 0; i <= m_CurTransferCommand.PathMaps.Count - 1; i++)
                {
                    m_CurTransferCommand.PathMaps[i].JcsPermit = false;
                }

                m_JcsStart = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public void JCSCancelRequest(int jpNo)
        {
            try
            {
                JCSIF_CR message = new JCSIF_CR()
                {
                    JunctionNumber = jpNo,
                };
                if (message != null)
                {
                    m_JCSManager.JcsStatus.CommMessage = string.Format("{0}-Cancel Permit", jpNo);
                    m_JCSManager.JcsComm.JCSSendMessage(message);
                }
                SeqAbort();

                string strLog = $"ProcessCommand : Cancel Request ";
                strLog += m_JCSManager.JcsStatus.CommMessage;
                SequenceJCSLog.WriteLog(FuncName, strLog);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        private bool IsPermit(int index, int jp)
        {
            bool rv = false;
            try
            {
                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                {
                    JunctionControl jc = m_JCSManager.JcsStatus.JunctionControl.Find(x => x.NodeId == jp && x.index >= index);
                    if (jc != null) { rv = jc.PermitDone; }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return rv;
        }
        private bool IsPassRequest(int index, int jp)
        {
            bool rv = false;
            try
            {
                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                {
                    JunctionControl jc = m_JCSManager.JcsStatus.JunctionControl.Find(x => x.NodeId == jp && x.index >= index);
                    if (jc != null) { rv = jc.RequestDone; }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return rv;
        }
        /// <summary>
        /// Next JP가 0인 경우는 더 이상 진행할 Junction이 없다는 뜻
        /// </summary>
        /// <returns></returns>
        private JunctionControl GetNextJunction()
        {
            JunctionControl jc = null;
            try
            {
                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                {
                    int cur_link = m_CurVehicleStatus.CurrentPath.LinkID;
                    int cur_index = m_JCSManager.JcsStatus.JunctionControl.FindIndex(x => x.LinkId == cur_link && x.PermitDone == false);
                    jc = m_JCSManager.JcsStatus.JunctionControl.Find(x => x.JcsStop == true && x.PermitDone == false && x.index >= cur_index);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return jc;
        }
        private bool IsJcsInArea()
        {
            bool jcsArea = false;
            try
            {
                int linkId = m_CurVehicleStatus.CurrentPath.LinkID;
                jcsArea = m_JCSManager.JcsStatus.JunctionAreaList.Contains(linkId);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return jcsArea;
        }
        private bool IsJcsInArea(int linkid)
        {
            bool jcsArea = false;
            try
            {
                jcsArea = m_JCSManager.JcsStatus.JunctionAreaList.Contains(linkid);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return jcsArea;
        }
        private void JCSPassRequest(int jp, int curNode, int curLink, List<int> nodes, int lastlink_Remain)
        {
            try
            {
                m_PermitMesaageRecv = false;
                m_PassRequestReply = false;
                m_PassRequestReplyNg = false;

                JCSIF_PR message = new JCSIF_PR()
                {
                    JunctionNumber = jp,
                    CurrentNode = curNode,
                    CurrentLink = curLink,
                    PathNodeCount = nodes.Count,
                    PathNodes = nodes,
                    TargetDistance = lastlink_Remain,
                };
                if (message != null)
                {
                    m_JCSManager.JcsComm.JCSSendMessage(message);
                    m_JCSManager.JcsStatus.CommMessage = string.Format("{0},{1},{2},{3}", jp, curNode, curLink, string.Join("-", nodes));
                }

                string strLog = $"ProcessCommand : Pass Request ";
                strLog += m_JCSManager.JcsStatus.CommMessage;
                SequenceJCSLog.WriteLog(FuncName, strLog);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        private void StatusReport()
        {
            try
            {
                JCSIF_SR message = m_JCSManager.JcsStatus.GetStatusSendMessage();
                if (message != null)
                {
                    bool message_check = true;
                    message_check &= message.CurrentNode < 0 ? false : true;
                    message_check &= message.CurrentLink < 0 ? false : true;
                    message_check &= message.PositionInLink < 0 ? false : true;
                    if (message_check) m_JCSManager.JcsComm.JCSSendMessage(message);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        private int GetNodeIndex(int nodeId)
        {
            int index = -1;
            try
            {
                if (m_JCSManager.JcsStatus.JunctionControl.Count > 0)
                {
                    index = m_JCSManager.JcsStatus.JunctionControl.FindIndex(x => x.NodeId == nodeId && x.PermitDone);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return index;
        }

        #endregion
    }
}
