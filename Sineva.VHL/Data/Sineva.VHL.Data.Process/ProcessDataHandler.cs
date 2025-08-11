using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Data;
using Sineva.VHL.Data.DbAdapter;
using System.Xml.Serialization;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Setup;

namespace Sineva.VHL.Data.Process
{
    [Serializable]
    public class ProcessDataHandler
    {
        #region Singltone
        public readonly static ProcessDataHandler Instance = new ProcessDataHandler();
        #endregion

        #region Fields
        private bool m_Intialized = false;
        public bool _SaveCurState = false;

        private Queue<TransferCommand> m_CommandQueue = new Queue<TransferCommand>(); // Auto Sequence에서 사용할 공간
        private List<TransferCommand> m_TransferCommands = new List<TransferCommand>(); // 저장용.
        private TransferCommand m_CurTransferCommand = new TransferCommand(); //신규 명령이 내려 오면 생성해야 함.
        private TransferCommand m_DestinationChangeCommand = new TransferCommand();
        private VehicleStatus m_CurVehicleStatus = new VehicleStatus();
        private BcrMapData m_CurBcrMapData = new BcrMapData();
        private Dictionary<enTPD, string> m_Tpd = new Dictionary<enTPD, string>();

        private AlarmData m_ALM_CommandExistError = null;
        private AlarmData m_ALM_CommandNotExistError = null;
        private AlarmData m_ALM_SourcePortMismatchError = null;
        private AlarmData m_ALM_DestinationPortMismatchError = null;
        #endregion

        #region Properties
        public List<TransferCommand> TransferCommands
        {
            get { return m_TransferCommands; }
            set { m_TransferCommands = value; }
        }
        public TransferCommand CurTransferCommand 
        {
            get { return m_CurTransferCommand; }
            set { m_CurTransferCommand = value; }
        }
        public TransferCommand DestinationChangeCommand
        {
            get { return m_DestinationChangeCommand; }
            set { m_DestinationChangeCommand = value; }
        }
        public VehicleStatus CurVehicleStatus 
        {
            get { return m_CurVehicleStatus; }
            set { m_CurVehicleStatus = value; }
        }
        [XmlIgnore()]
        public BcrMapData CurBcrMapData 
        {
            get { return m_CurBcrMapData; }
            set { m_CurBcrMapData = value; }
        }
        [XmlIgnore()]
        public Queue<TransferCommand> CommandQueue 
        {
            get { return m_CommandQueue; }
            set { m_CommandQueue = value; }
        }
        [XmlIgnore()]
        public Dictionary<enTPD, string> Tpd
        {
            get { return m_Tpd; }
        }
        [XmlIgnore()]
        public AlarmData ALM_SourcePortMismatchError
        {
            get { return m_ALM_SourcePortMismatchError; }
            set { m_ALM_SourcePortMismatchError = value; }
        }
        [XmlIgnore()]
        public AlarmData ALM_DestinationPortMismatchError
        {
            get { return m_ALM_DestinationPortMismatchError; }
            set { m_ALM_DestinationPortMismatchError = value; }
        }
        [XmlIgnore()]
        public AlarmData ALM_CommandExistError
        {
            get { return m_ALM_CommandExistError; }
            set { m_ALM_CommandExistError = value; }
        }
        [XmlIgnore()]
        public AlarmData ALM_CommandNotExistError
        {
            get { return m_ALM_CommandNotExistError; }
            set { m_ALM_CommandNotExistError = value; }
        }
        #endregion

        #region Constructor
        public ProcessDataHandler() 
        {
        }

        #endregion

        #region Methods
        public bool Initialize()
        {
            bool rv = true;
            try
            {
                if (m_Intialized == false)
                {
                    rv &= ReadXml();
                    if (rv)
                    {
                        if (m_TransferCommands.Count > 0)
                        {
                            m_CommandQueue.Clear();
                            foreach (TransferCommand cmd in m_TransferCommands) m_CommandQueue.Enqueue(cmd);
                        }
                    }
                    rv &= CurBcrMapData.Initialize();
                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                    {
                        if (CurVehicleStatus.CurrentBcrStatus.LeftBcr == 0.0f && CurVehicleStatus.CurrentPort.PortID != 0)
                            CurVehicleStatus.CurrentBcrStatus.LeftBcr = CurVehicleStatus.CurrentPort.BarcodeLeft;
                        if (CurVehicleStatus.CurrentBcrStatus.LeftBcr == 0.0f && CurVehicleStatus.CurrentPath.LinkID != 0)
                            CurVehicleStatus.CurrentBcrStatus.LeftBcr = CurVehicleStatus.CurrentPath.LeftBCRStart;
                        if (CurVehicleStatus.CurrentBcrStatus.RightBcr == 0.0f && CurVehicleStatus.CurrentPort.PortID != 0)
                            CurVehicleStatus.CurrentBcrStatus.RightBcr = CurVehicleStatus.CurrentPort.BarcodeRight;
                        if (CurVehicleStatus.CurrentBcrStatus.RightBcr == 0.0f && CurVehicleStatus.CurrentPath.LinkID != 0)
                            CurVehicleStatus.CurrentBcrStatus.RightBcr = CurVehicleStatus.CurrentPath.RightBCRStart;
                    }

                    #region TPD Data
                    foreach (enTPD tpd in Enum.GetValues(typeof(enTPD))) m_Tpd.Add(tpd, "");
                    #endregion
                    m_Intialized = rv;

                    m_ALM_CommandExistError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DATA", "Data Error", "Transfer Command Exist Alarm");
                    m_ALM_CommandNotExistError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DATA", "Data Error", "Transfer Command Not Exist Alarm");
                    m_ALM_SourcePortMismatchError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DATA", "Data Error", "Source Port And Current Port Mismatch Alarm");
                    m_ALM_DestinationPortMismatchError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "DATA", "Data Error", "Destination Port And Current Port Mismatch Alarm");
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return rv;
        }
        public void Save()
        {
            try
            {
                _SaveCurState = false;
                CurTransferCommand._SaveCurState = false;
                CurVehicleStatus._SaveCurState = false;
                WriteXml();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void CreateTransferCommand(Command cmd)
        {
            try
            {
                TransferCommand curCmd = ProcessDataHandler.Instance.CurTransferCommand;
                bool aging = curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleHoistAging;
                aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleSteerAging;
                aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleAntiDropAging;
                aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleWheelMoveAging;
                aging &= curCmd.IsValid;
                bool command_create_enable = !aging;
                if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                {
                    command_create_enable &= cmd.ProcessCommand == OCSCommand.Go;
                }

                if (command_create_enable)
                {
                    TransferCommand command = new TransferCommand(cmd);
                    command.UpdateStartEndNode(m_CurVehicleStatus);
                    if (m_TransferCommands.Select(x => x.CommandID).Contains(cmd.CommandID) == false)
                    {
                        DataItem_TransferInfo info = command.GetTransferInformation();
                        DatabaseHandler.Instance.UpdateTransferCommand(info);

                        m_CommandQueue.Enqueue(command);
                        m_TransferCommands.Clear();
                        m_TransferCommands = m_CommandQueue.ToList();

                        _SaveCurState = true;
                    }
                }

                EventHandlerManager.Instance.InvokeUpdateCommandChanged(m_TransferCommands);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public int CreateTransferCommand(VehicleIF_CommandSend msg)
        {
            int rv = 0;
            try
            {
                TransferCommand curCmd = ProcessDataHandler.Instance.CurTransferCommand;
                bool aging = curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleHoistAging;
                aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleSteerAging;
                aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleAntiDropAging;
                aging |= curCmd.ProcessCommand == IF.OCS.OCSCommand.CycleWheelMoveAging;
                aging &= curCmd.IsValid;
                bool command_create_enable = !aging;
                if (AppConfig.Instance.VehicleType == VehicleType.Clean)
                {
                    command_create_enable &= msg.ProcessCommand == OCSCommand.Go;
                }
                if (command_create_enable)
                {
                    Command cmd = new Command()
                    {
                        CommandID = msg.TransferCommandID.Replace("\0", ""),
                        CassetteID = msg.CassetteID,
                        SourceID = msg.SourceID,
                        DestinationID = msg.DestinationID,
                        ProcessCommand = msg.ProcessCommand,
                        FullPathNodes = msg.PathNodes,
                        TypeOfDestination = msg.TypeOfDestination == 1 ? enGoCommandType.ByLocation : msg.TypeOfDestination == 2 ? enGoCommandType.ByDistance : enGoCommandType.None,
                        TargetNodeToDistance = msg.TargetNodeToDistance,
                    };

                    TransferCommand command = new TransferCommand(cmd);
                    command.UpdateStartEndNode(m_CurVehicleStatus);
                    if (m_TransferCommands.Select(x => x.CommandID).Contains(cmd.CommandID) == false)
                    {
                        DataItem_TransferInfo info = command.GetTransferInformation();
                        DatabaseHandler.Instance.UpdateTransferCommand(info);
                        command.UpdateTransferTime(transferUpdateTime.Install);

                        m_CommandQueue.Enqueue(command);
                        m_TransferCommands.Clear();
                        m_TransferCommands = m_CommandQueue.ToList();

                        _SaveCurState = true;
                    }
                    else
                    {
                        rv = 1;
                        SequenceLog.WriteLog("[ProcessDataHandler]", "OCS->VHL : Dupulicate CommandID Send");
                    }
                }
                else
                {
                    if (aging)
                    {
                        rv = 2;
                        SequenceLog.WriteLog("[ProcessDataHandler]", "OCS->VHL : The ProcessCommand of Received can't run, CurCommand Aging Process!");
                    }
                    else
                    {
                        rv = 3;
                        SequenceLog.WriteLog("[ProcessDataHandler]", "OCS->VHL : Clean Vehicle Can't run the ProcessCommand of Received");
                    }
                }
                EventHandlerManager.Instance.InvokeUpdateCommandChanged(m_TransferCommands);
            }
            catch (Exception ex)
            {
                rv = 2;
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        public void DeleteTransferCommand(Command cmd)
        {
            try
            {
                TransferCommand find_cmd = m_TransferCommands.Find(x => x.ToString() == cmd.ToString());
                if (find_cmd != null)
                {
                    foreach (TransferCommand c in m_TransferCommands)
                    {
                        if (c.ToString() == cmd.ToString())
                        {
                            m_TransferCommands.Remove(c);
                            break;
                        }
                    }
                    m_CommandQueue.Clear();
                    if (m_TransferCommands.Count > 0)
                    {
                        foreach (TransferCommand c in m_TransferCommands) m_CommandQueue.Enqueue(c);
                    }
                }
                if (cmd.CommandID == m_CurTransferCommand.CommandID) 
                    CurTransferCommandFinished();

                _SaveCurState = true;
                EventHandlerManager.Instance.InvokeUpdateCommandChanged(m_TransferCommands);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void SaveTransferCommand(Command cmd)
        {
            try
            {
                TransferCommand find_cmd = m_TransferCommands.Find(x => x.CommandID == cmd.CommandID);
                if (find_cmd != null)
                {
                    foreach (TransferCommand c in m_TransferCommands)
                    {
                        if (c.CommandID == cmd.CommandID)
                        {
                            c.SetCopy(cmd);
                            break;
                        }
                    }
                    m_CommandQueue.Clear();
                    if (m_TransferCommands.Count > 0)
                    {
                        foreach (TransferCommand c in m_TransferCommands) m_CommandQueue.Enqueue(c);
                    }
                }
                _SaveCurState = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void CurTransferCommandFinished()
        {
            try
            {
                m_CurTransferCommand.RunPathMaps.Clear();
                m_CurTransferCommand.PathMaps.Clear();
                m_CurTransferCommand.SetProcessStatus(ProcessStatus.Completed);
                m_CurTransferCommand.IsValid = false;

                List<string> commandIds = m_TransferCommands.Select(x => x.CommandID).ToList();
                int index0 = commandIds.Count > 0 ? commandIds.IndexOf(m_CurTransferCommand.CommandID) : -1;
                if (index0 >= 0) m_TransferCommands.RemoveAt(index0);
                // Que에서는 이미 뽑아서 CurTransferCommand로 사용 했으니 삭제코드가 불필요
                DataItem_TransferInfo info = m_CurTransferCommand.GetTransferInformation();
                DatabaseHandler.Instance.UpdateTransferCommand(info);
                m_CurTransferCommand.UpdateTransferTime(transferUpdateTime.CommandCompleted);

                _SaveCurState = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public void CreateDestinationChangeCommand(VehicleIF_CommandSend msg)
        {
            try
            {
                Command cmd = new Command()
                {
                    CommandID = msg.TransferCommandID.Replace("\0", ""),
                    CassetteID = msg.CassetteID,
                    SourceID = msg.SourceID,
                    DestinationID = msg.DestinationID,
                    ProcessCommand = msg.ProcessCommand,
                    FullPathNodes = msg.PathNodes,
                    TypeOfDestination = msg.TypeOfDestination == 1 ? enGoCommandType.ByLocation : msg.TypeOfDestination == 2 ? enGoCommandType.ByDistance : enGoCommandType.None,
                    TargetNodeToDistance = msg.TargetNodeToDistance,
                };

                TransferCommand command = new TransferCommand(cmd);
                m_DestinationChangeCommand = command;

                DataItem_TransferInfo info = command.GetTransferInformation();
                DatabaseHandler.Instance.UpdateTransferCommand(info);
                command.UpdateTransferTime(transferUpdateTime.Install);

                _SaveCurState = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        public bool IsSaveCurState()
        {
            bool rv = _SaveCurState;
            rv |= CurTransferCommand._SaveCurState;
            rv |= CurVehicleStatus._SaveCurState;
            return rv;
        }
        #endregion

        #region [Xml Read/Write]
        public bool ReadXml()
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

                var helperXml = new XmlHelper<ProcessDataHandler>();
                ProcessDataHandler mng = helperXml.Read(fileName);
                if (mng != null)
                {
                    this.TransferCommands = mng.TransferCommands;
                    this.CurTransferCommand = mng.CurTransferCommand;
                    this.CurVehicleStatus = mng.CurVehicleStatus;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString() + this.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }

            return true;
        }

        public void WriteXml()
        {
            string fileName = "";
            bool fileCheck = CheckPath(ref fileName);
            if (fileCheck == false) return;

            try
            {
                var helperXml = new XmlHelper<ProcessDataHandler>();
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
            string filePath = AppConfig.Instance.AppConfigPath;

            if (Directory.Exists(filePath) == false)
            {
                MessageBox.Show("ProcessData folder was not found");
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "ProcessData folder select";
                dlg.SelectedPath = AppConfig.GetSolutionPath();
                dlg.ShowNewFolderButton = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filePath = dlg.SelectedPath;
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
            string fileName;
            fileName = this.GetType().Name;
            return fileName;
        }
        #endregion

    }
}
