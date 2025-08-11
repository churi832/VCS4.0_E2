using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.IF.OCS;
using System.Security.Policy;
using System.Windows.Forms;
using System.Reflection.Emit;
using System.Xml.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using Sineva.VHL.Data.PathFinder;
using Sineva.VHL.Library.Common;
using Sineva.VHL.Data.Setup;
using System.IO;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;

namespace Sineva.VHL.Data.Process
{
    /// <summary>
    ///  반송에 필요한 모든 정보를 여기에 넣어두자...
    /// </summary>
    [Serializable]
    public class TransferCommand : Command
    {
        #region Fields
        public bool _SaveCurState = false;

        private List<Path> m_PathMaps = new List<Path>(); // <linkID, Path>
        private List<Path> m_RunPathMaps = new List<Path>(); // PathMaps에서 실행중인 Link Map
        private int m_TargetNode = 0; //PathMap에서 중간에 멈춰야 하는 경우...ToNode, Last Path는 ... FromNode
        private int m_TargetNodeOfCommandSet = 0; //Motion 명령이 모두 설정된 후 확인 용

        private enBcrCheckDirection m_TargetBcrCheckDirection = enBcrCheckDirection.Both; //도착 위치에서 사용할 BCR 방향
        private double m_TargetLeftBcrPosition = 0.0f;
        private double m_TargetRightBcrPosition = 0.0f;
        private double m_TargetMotorPosition = 0.0f;
        private double m_RemainBcrDistance = 0.0f;
        private double m_RemainMotorDistance = 0.0f;
        private double m_TargetBcrDistance = 0.0f; // Total Distance
        private double m_StopCheckDistance = 0.0f; // JCS Permit 후 연속 진행 결정 위치, 문제)1. Stop위치가 여러개의 짧은 link로 묶여져 있는 경우 last_link최대거리, 2. BcrRunDistance보다 짧으면 않됨.
        private MotionProfile m_BcrScanMotion = new MotionProfile(); // Run Distance 포함. 속도, 감속, jerk 포함.
        private List<int> m_BcrRunLinkList = new List<int>(); // BCR Start 시점 관련용
        private bool m_IsSetExternalEncorder = false; // 출발 할때 결정
        private bool m_IsSequenceMoving = false;
        private bool m_IsSequenceStopping = false;
        private bool m_IsBcrScanMoving = false;
        private bool m_IsReceivePath_byOCS = false;
        private bool m_IsReceivePath_byRCCheck = false;
        private bool m_OnlyTransferMove = false;
        private bool m_IsPathLoop = false;
        private bool m_IsStopCheckVelocity = true; // JCS Permit 후 연속진행하기 위한 속도 확인
        private double m_StopCheckVelocity = 0.0f;
        private bool m_IsCarrierIdScan = false; // Tranfer 명령과 동일한데 Acquire 후 RF ID 확인...
        private bool m_IsNeedMakeRouteFullPath = true; // Virtual BCR 계산 문제. Abort or Manual 전환 후 Command가 True인 경우.. Simulation 이상이 생김.

        private ProcessStatus m_CommandStatus = ProcessStatus.Queued;
        private string m_CarrierLocation = string.Empty;
        #endregion

        #region Properties
        [XmlIgnore()]
        public List<Path> PathMaps
        {
            get { return m_PathMaps; }
            set { m_PathMaps = value; }
        }
        [XmlIgnore()]
        public List<Path> RunPathMaps
        {
            get { return m_RunPathMaps; }
            set { m_RunPathMaps = value; }
        }
        [Browsable(false)]
        public int TargetNode
        {
            get { return m_TargetNode; }
            set { m_TargetNode = value; }
        }
        [Browsable(false)]
        public int TargetNodeOfCommandSet
        {
            get { return m_TargetNodeOfCommandSet; }
            set { m_TargetNodeOfCommandSet = value; }
        }
        
        public enBcrCheckDirection TargetBcrCheckDirection
        {
            get { return m_TargetBcrCheckDirection; }
            set { m_TargetBcrCheckDirection = value; }
        }
        [Browsable(false)]
        public double TargetLeftBcrPosition
        {
            get { return m_TargetLeftBcrPosition; }
            set { m_TargetLeftBcrPosition = value; }
        }
        [Browsable(false)]
        public double TargetRightBcrPosition
        {
            get { return m_TargetRightBcrPosition; }
            set { m_TargetRightBcrPosition = value; }
        }
        [Browsable(false)]
        public double TargetMotorPosition
        {
            get { return m_TargetMotorPosition; }
            set { m_TargetMotorPosition = value; }
        }
        [Browsable(false)]
        public double RemainBcrDistance
        {
            get { return m_RemainBcrDistance; }
            set { m_RemainBcrDistance = value; }
        }
        [Browsable(false)]
        public double RemainMotorDistance
        {
            get { return m_RemainMotorDistance; }
            set { m_RemainMotorDistance = value; }
        }
        [Browsable(false)]
        public double TargetBcrDistance
        {
            get { return m_TargetBcrDistance; }
            set { m_TargetBcrDistance = value; }
        }
        [Browsable(false)]
        public double StopCheckDistance
        {
            get { return m_StopCheckDistance; }
            set { m_StopCheckDistance = value; }
        }
        [Browsable(false)]
        public MotionProfile BcrScanMotion
        {
            get { return m_BcrScanMotion; }
            set { m_BcrScanMotion = value; }
        }
        [Browsable(false)]
        public bool IsSetExternalEncorder
        {
            get { return m_IsSetExternalEncorder; }
            set { m_IsSetExternalEncorder = value; }
        }
        [Browsable(false)]
        public bool IsSequenceMoving
        {
            get { return m_IsSequenceMoving; }
            set { m_IsSequenceMoving = value; }
        }
        [Browsable(false)]
        public bool IsSequenceStopping
        {
            get { return m_IsSequenceStopping; }
            set { m_IsSequenceStopping = value; }
        }
        [Browsable(false)]
        public bool IsBcrScanMoving
        {
            get { return m_IsBcrScanMoving; }
            set { m_IsBcrScanMoving = value; }
        }
        [Browsable(false)]
        public bool IsReceivePath_byOCS
        {
            get { return m_IsReceivePath_byOCS; }
            set { m_IsReceivePath_byOCS = value; }
        }
        [Browsable(false)]
        public bool IsReceivePath_byRCCheck
        {
            get { return m_IsReceivePath_byRCCheck; }
            set { m_IsReceivePath_byRCCheck = value; }
        }
        [Browsable(false)]
        public ProcessStatus CommandStatus
        {
            get { return m_CommandStatus; }
            set { m_CommandStatus = value; } 
        }
        [Browsable(false)]
        public string CarrierLocation
        {
            get { return m_CarrierLocation; }
            set { m_CarrierLocation = value; } 
        }
        [Browsable(false)]
        public bool OnlyTransferMove
        {
            get { return m_OnlyTransferMove; }
            set { m_OnlyTransferMove = value; }
        }
        [Browsable(false)]
        public bool IsPathLoop
        {
            get { return m_IsPathLoop; }
            set { m_IsPathLoop = value; }
        }
        [Browsable(false)]
        public bool IsStopCheckVelocity
        {
            get { return m_IsStopCheckVelocity; }
            set { m_IsStopCheckVelocity = value; }
        }
        [Browsable(false)]
        public double StopCheckVelocity
        {
            get { return m_StopCheckVelocity; }
            set { m_StopCheckVelocity = value; }
        }        
        [Browsable(false)]
        public bool IsCarrierIdScan
        {
            get { return m_IsCarrierIdScan; }
            set { m_IsCarrierIdScan = value; }
        }
        [Browsable(false)]
        public bool IsNeedMakeRouteFullPath 
        {
            get { return m_IsNeedMakeRouteFullPath; }
            set { m_IsNeedMakeRouteFullPath = value; }
        }
        #endregion

        #region Constructor
        public TransferCommand()
        {
        }
        // Transfer Command를 만드는 방법을 두가지로 제공
        // 1. OCS -> VHL Command가 내려 왔을 경우
        // 2. Database에서 가져와서 명령을 만들 경우 
        public TransferCommand(Command command)
        {
            try
            {
                ClearCommand();
                if (command.ProcessCommand == OCSCommand.CarrierIdScan)
                {
                    m_IsCarrierIdScan = true;
                    ProcessCommand = OCSCommand.Transfer;
                }
                else if (command.ProcessCommand == OCSCommand.VehicleRemove)
                {
                    ProcessCommand = OCSCommand.Go;
                    command.SourceID = 0;
                    command.DestinationID = SetupManager.Instance.SetupOCS.VehicleRemoveMTLNodeId;
                    command.TypeOfDestination = enGoCommandType.ByDistance;
                    command.TargetNodeToDistance = 0.0f;
                }
                else
                    ProcessCommand = command.ProcessCommand;

                CommandID = command.CommandID;
                CassetteID = command.CassetteID;
                SourceID = command.SourceID;
                DestinationID = command.DestinationID;
                StartNode = command.StartNode;
                EndNode = command.EndNode;
                FullPathNodes = command.FullPathNodes;
                TypeOfDestination = command.TypeOfDestination;
                TargetNodeToDistance = command.TargetNodeToDistance;
                CommandStatus = ProcessStatus.Queued;

                if (CommandID.Contains("SCENARIOMOVING"))
                    m_OnlyTransferMove = true;
                else m_OnlyTransferMove = false;
                UpdateTargetBcrPosition();

                IsValid = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public TransferCommand(DataItem_TransferInfo transferInfo)
        {
            try
            {
                ClearCommand();
                SetTransferInformation(transferInfo);
                IsValid = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        #endregion

        #region methods
        public void SetTransferInformation(DataItem_TransferInfo transferInfo)
        {
            try
            {
                ProcessCommand = CovertCommandTransferToOCS(transferInfo.Type);
                CommandID = transferInfo.CommandID;
                CassetteID = transferInfo.CarrierID;
                int outValue = 0;
                SourceID = int.TryParse(transferInfo.Source, out outValue) ? outValue : 0;
                DestinationID = int.TryParse(transferInfo.Destination, out outValue) ? outValue : 0;
                StartNode = 0;
                EndNode = 0;
                FullPathNodes = ObjectCopier.Clone(transferInfo.PathNodes);
                m_CommandStatus = transferInfo.Status;
                m_CarrierLocation = transferInfo.CarrierLocation;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        public DataItem_TransferInfo GetTransferInformation()
        {
            DataItem_TransferInfo transferInfo = null;
            try
            {
                transferInfo = new DataItem_TransferInfo()
                {
                    Type = CovertCommandOCSToTransfer(ProcessCommand),
                    CommandID = CommandID,
                    CarrierID = CassetteID,
                    Source = SourceID.ToString(),
                    Destination = DestinationID.ToString(),
                    PathNodes = ObjectCopier.Clone(FullPathNodes),
                    Status = m_CommandStatus,
                    CarrierLocation = m_CarrierLocation,
                };
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return transferInfo;
        }
        public void ClearCommand()
        {
            try
            {
                IsValid = false;
                ProcessCommand = OCSCommand.None;
                CommandID = string.Empty;
                CassetteID = string.Empty;
                SourceID = 0;
                DestinationID = 0;
                StartNode = 0;
                EndNode = 0;
                PathNodes.Clear();
                FullPathNodes.Clear();
                TypeOfDestination = 0;
                TargetNodeToDistance = 0.0f;

                m_PathMaps.Clear();
                m_RunPathMaps.Clear();
                m_TargetNode = 0;
                m_TargetNodeOfCommandSet = 0;
                m_TargetBcrCheckDirection = enBcrCheckDirection.Both; //도착 위치에서 사용할 BCR 방향
                m_TargetLeftBcrPosition = 0.0f;
                m_TargetRightBcrPosition = 0.0f;
                m_TargetMotorPosition = 0.0f;
                m_RemainBcrDistance = 0.0f;
                m_RemainMotorDistance = 0.0f;
                m_BcrScanMotion.Clear();
                m_BcrRunLinkList.Clear();
                m_IsSetExternalEncorder = false;
                m_IsSequenceMoving = false;
                m_IsSequenceStopping = false;
                m_IsBcrScanMoving = false;
                m_IsReceivePath_byOCS = false;
                m_IsReceivePath_byRCCheck = false;
                m_IsStopCheckVelocity = false;
                m_IsCarrierIdScan = false;
                m_OnlyTransferMove = false;

                m_CommandStatus = ProcessStatus.None;
                m_CarrierLocation = string.Empty;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        
        public void UpdateTransferTime(transferUpdateTime enTime)
        {
            try
            {
                DatabaseHandler.Instance.UpdateTransferTime(CommandID, enTime);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }

        public void SetProcessStatus(ProcessStatus status)
        {
            try
            {
                if (m_CommandStatus != status)
                {
                    // Command가 Cancelling/Aborting 중일때는 Completed로 넘어가면 않된다.
                    bool old_command = m_CommandStatus == ProcessStatus.Canceling || m_CommandStatus == ProcessStatus.Aborting;
                    bool new_command = status == ProcessStatus.Processing || status == ProcessStatus.Completed;
                    if (old_command && new_command)
                    {
                        SequenceOCSLog.WriteLog("[TransferCommand]", string.Format("Transfer Status Change [Update Skip] : {0} -> {1}", m_CommandStatus, status));
                    }
                    else
                    {
                        SequenceOCSLog.WriteLog("[TransferCommand]", string.Format("Transfer Status Change : {0} -> {1}", m_CommandStatus, status));
                        m_CommandStatus = status;
                    }
                    DataItem_TransferInfo info = GetTransferInformation();
                    DatabaseHandler.Instance.UpdateTransferCommand(info);
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }

        public ProcessStatus GetProcessStatus()
        {
            ProcessStatus status = ProcessStatus.None;
            try
            {
                status = m_CommandStatus;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return status;
        }
        public OCSCommand CovertCommandTransferToOCS(TransferType type)
        {
            OCSCommand rv = OCSCommand.None;
            if (type == TransferType.Transfer) rv = OCSCommand.Transfer;
            else if (type == TransferType.Go) rv = OCSCommand.Go;
            else if (type == TransferType.Teaching) rv = OCSCommand.Teaching;
            else if (type == TransferType.Cancel) rv = OCSCommand.Cancel;
            else if (type == TransferType.Abort) rv = OCSCommand.Abort;
            else if (type == TransferType.Abort) rv = OCSCommand.Abort;
            return rv;
        }
        public TransferType CovertCommandOCSToTransfer(OCSCommand cmd)
        {
            TransferType rv = TransferType.None;
            if (cmd == OCSCommand.Transfer) rv = TransferType.Transfer;
            else if (cmd == OCSCommand.Go) rv = TransferType.Go;
            else if (cmd == OCSCommand.Teaching) rv = TransferType.Teaching;
            else if (cmd == OCSCommand.Cancel) rv = TransferType.Cancel;
            else if (cmd == OCSCommand.Abort) rv = TransferType.Abort;
            else if (cmd == OCSCommand.Abort) rv = TransferType.Abort;
            return rv;
        }
        public void SetFullPathNodes(List<int> nodes)
        {
            FullPathNodes.Clear();
            FullPathNodes.AddRange(ObjectCopier.Clone(nodes));
        }
        public void SetPathNodes(List<int> nodes)
        {
            PathNodes.Clear();
            PathNodes.AddRange(ObjectCopier.Clone(nodes));
        }
        /// <summary>
        /// TransferCommand 생성 시점에 StartNode와 EndNode를 찾아 Update 해라
        /// </summary>
        /// <param name="vehicle"></param>
        public void UpdateStartEndNode(VehicleStatus vehicle)
        {
            try
            {
                bool carrier_installed = vehicle.CarrierStatus == IF.OCS.CarrierState.Installed;
                this.StartNode = vehicle.CurrentPath.ToNodeID; // Start Node는 Run 시점에 새로 계산 필요함.
                int portId = 0;
                if (this.ProcessCommand == OCSCommand.Transfer)
                {
                    if (carrier_installed) portId = this.DestinationID; // Destination
                    else portId = this.SourceID;
                }
                else if (this.ProcessCommand == OCSCommand.Go)
                {
                    if (this.TypeOfDestination == enGoCommandType.ByLocation)
                        portId = this.DestinationID;
                    else if (this.TypeOfDestination == enGoCommandType.ByDistance)
                        this.EndNode = this.DestinationID;
                }
                else if (this.ProcessCommand == OCSCommand.Teaching)
                {
                    portId = this.DestinationID;
                }
                if (portId != 0)
                {
                    if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portId))
                    {
                        this.EndNode = DatabaseHandler.Instance.DictionaryPortDataList[portId].NodeID;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
        }
        /// <summary>
        /// Target Position과 Current Position을 비교
        /// 동일 Path 이동인지 한바퀴 돌아야 하는지 판단.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public void UpdateTargetBcrPosition()
        {
            bool go_command = this.ProcessCommand == OCSCommand.Go ? true : false;
            if (go_command && this.TypeOfDestination == enGoCommandType.ByDistance) //Distance Mode
            {
                if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(this.DestinationID))
                {
                    TargetLeftBcrPosition = DatabaseHandler.Instance.DictionaryNodeDataList[this.DestinationID].LocationValue1 + this.TargetNodeToDistance;
                    TargetRightBcrPosition = DatabaseHandler.Instance.DictionaryNodeDataList[this.DestinationID].LocationValue2 + this.TargetNodeToDistance;
                }
            }
            else
            {
                int targetPort = 0;
                if (this.ProcessCommand == OCSCommand.Go || this.ProcessCommand == OCSCommand.Teaching)
                {
                    targetPort = this.DestinationID;
                }
                else
                {
                    if (this.SourceID != 0) targetPort = this.SourceID; // Source 위치 이동
                    else targetPort = this.DestinationID; // Destination 위치 이동, Transfer에서 Acquire가 끝나면 
                }
                if (targetPort != 0)
                {
                    if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(targetPort))
                    {
                        double offset_left = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].DriveLeftOffset;
                        double offset_right = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].DriveRightOffset;
                        TargetLeftBcrPosition = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].BarcodeLeft + offset_left;
                        TargetRightBcrPosition = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].BarcodeRight + offset_right;
                    }
                }
            }
        }
        private bool IsCommandLoopLink(VehicleStatus vehicle, ref int targetNode)
        {
            bool loop_link = false; // 한바퀴 돌거니 ?
            try
            {
                targetNode = 0;
                bool carrier_installed = vehicle.CarrierStatus == IF.OCS.CarrierState.Installed;
                double leftBcr = vehicle.CurrentBcrStatus.LeftBcr;
                double rightBcr = vehicle.CurrentBcrStatus.RightBcr;
                double inRange = SetupManager.Instance.SetupWheel.InRange;

                bool go_command = this.ProcessCommand == OCSCommand.Go ? true : false;
                if (go_command && this.TypeOfDestination == enGoCommandType.ByDistance) //Distance Mode
                {
                    targetNode = this.DestinationID;
                    if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(targetNode))
                    {
                        TargetLeftBcrPosition = DatabaseHandler.Instance.DictionaryNodeDataList[targetNode].LocationValue1 + this.TargetNodeToDistance;
                        TargetRightBcrPosition = DatabaseHandler.Instance.DictionaryNodeDataList[targetNode].LocationValue2 + this.TargetNodeToDistance;
                        if (targetNode == vehicle.CurrentPath.FromNodeID)
                        {
                            // 둘중 하나라도 InRange 안에 들어오면 loop 아님
                            // 둘다 InRange 안에 들어와야 Loop 아님...이상하면 그냥 loop로 돌리자 ....20250410
                            double leftDiff = TargetLeftBcrPosition - leftBcr;
                            double rightDiff = TargetRightBcrPosition - rightBcr;
                            if (Math.Abs(leftDiff) > inRange || Math.Abs(rightDiff) > inRange)
                            {
                                loop_link |= leftDiff < 0.0f;
                                loop_link |= rightDiff < 0.0f;
                            }
                            else
                            {
                                if (Math.Abs(leftDiff) < Math.Abs(rightDiff)) TargetBcrCheckDirection = enBcrCheckDirection.Left;
                                else TargetBcrCheckDirection = enBcrCheckDirection.Right;
                            }
                            //if (Math.Abs(leftDiff) > inRange && Math.Abs(rightDiff) > inRange)
                            //{
                            //    loop_link |= leftDiff < 0.0f;
                            //    loop_link |= rightDiff < 0.0f;
                            //}
                            //else
                            //{
                            //    if (Math.Abs(leftDiff) <= inRange) TargetBcrCheckDirection = enBcrCheckDirection.Left;
                            //    else if (Math.Abs(rightDiff) <= inRange) TargetBcrCheckDirection = enBcrCheckDirection.Right;
                            //}
                        }
                    }
                }
                else
                {
                    int targetPort = 0;
                    if (this.ProcessCommand == OCSCommand.Go || this.ProcessCommand == OCSCommand.Teaching)
                    {
                        targetPort = this.DestinationID;
                    }
                    else
                    {
                        if (carrier_installed && this.DestinationID != 0) targetPort = this.DestinationID; // Destination 위치 이동, Transfer에서 Acquire가 끝나면 Source를 0으로 바꾸어야 겠다 !
                        else if (this.SourceID != 0) targetPort = this.SourceID; // Source 위치 이동
                    }
                    if (targetPort != 0)
                    {
                        if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(targetPort))
                        {
                            double offset_left = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].DriveLeftOffset;
                            double offset_right = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].DriveRightOffset;
                            TargetLeftBcrPosition = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].BarcodeLeft + offset_left;
                            TargetRightBcrPosition = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].BarcodeRight + offset_right;

                            targetNode = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].NodeID;
                            if (targetNode == vehicle.CurrentPath.FromNodeID)
                            {
                                // 둘중 하나라도 InRange 안에 들어오면 loop 아님
                                double leftDiff = TargetLeftBcrPosition - leftBcr;
                                double rightDiff = TargetRightBcrPosition - rightBcr;
                                if (Math.Abs(leftDiff) > inRange || Math.Abs(rightDiff) > inRange)
                                {
                                    loop_link |= leftDiff < 0.0f;
                                    loop_link |= rightDiff < 0.0f;
                                }
                                else
                                {
                                    if (Math.Abs(leftDiff) < Math.Abs(rightDiff)) TargetBcrCheckDirection = enBcrCheckDirection.Left;
                                    else TargetBcrCheckDirection = enBcrCheckDirection.Right;
                                }
                                //if (Math.Abs(leftDiff) > inRange && Math.Abs(rightDiff) > inRange) // inRange보다 크고 -방향에 있는 경우
                                //{
                                //    loop_link |= leftDiff < 0.0f;
                                //    loop_link |= rightDiff < 0.0f;
                                //}
                                //else
                                //{
                                //    if (Math.Abs(leftDiff) <= inRange) TargetBcrCheckDirection = enBcrCheckDirection.Left;
                                //    else if (Math.Abs(rightDiff) <= inRange) TargetBcrCheckDirection = enBcrCheckDirection.Right;
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return loop_link;
        }
        /// <summary>
        /// Start<->End 이동할 최적 Node List를 찾자.
        /// </summary>
        /// <param name="pathNodes"></param>
        /// <returns></returns>
        public List<int> GetFindNodes(List<int> pathNodes)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            string timeMsg = string.Empty;
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                stopWatch.Reset();
                stopWatch.Start();
            }

            List<int> fullNodes = new List<int>();
            try
            {
                PathFinderMain.Instance.Initialize();
                int sourceNode = 0, targetNode = 0;
                double EstTime = 0, Cost = 0;

                for (int i = 0; i < pathNodes.Count - 1; i++)
                {
                    sourceNode = pathNodes[i];
                    targetNode = pathNodes[i + 1];
                    List<int> pathLists = PathFinderMain.Instance.GetPath(PathFindMethod.ByDistance, sourceNode, targetNode, true, out EstTime, out Cost);
                    timeMsg += string.Format(",{0}", EstTime);
                    if (i == 0) fullNodes.AddRange(pathLists.GetRange(0, pathLists.Count));
                    else if (pathLists.Count > 1) fullNodes.AddRange(pathLists.GetRange(1, pathLists.Count - 1)); // Node 중복 회피
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                fullNodes = null;
            }

            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                double watch = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine($"{time} : [GetFindNodes-{this.PathNodes.Count}] [{watch}] [{timeMsg}]");
            }
            return fullNodes;
        }
        /// <summary>
        /// DataItem_CommandData에 Motion Profile 작성 정보가 들어 있음. (targetNode를 결정하고 targetNode를 향해서 가는 방법 구하기)
        /// if, Source, Destination이 모두 0인 경우 : Current Node -> End Node 이동
        /// if, Source != 0, Desitnation != 0 : Current Node -> Source 이동
        /// if, Source == 0, Desitnation != 0 : Current Node -> Destination 이동
        /// </summary>
        /// 1. targetNode를 결정하자 : 어느 노드까지 가는지 알아야 Path Finder가 가능할거다..
        /// 2. loop_link를 확인하자 : 동일 Link 이동일 경우 한바퀴 도는 경우가 있다.
        /// 3. PathNodes 에 CurrentLink.FromNodeID, CurrentLink.ToNodeID, targetNode가 있는 확인하고 삽입하자.
        /// 4. FullPath Find를 완료하자...!
        /// 5. LastLink가 결정되면 LastLink.ToNodeID를 삽입하자.
        /// <param name="command"></param>
        /// <returns></returns>
        public bool MakeNodes(VehicleStatus vehicle, bool pathNodesClear = false)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            double time_check1 = 0;
            double time_check2 = 0;
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                stopWatch.Reset();
                stopWatch.Start();
            }

            bool rv = true;
            bool carrier_installed = vehicle.CarrierStatus == IF.OCS.CarrierState.Installed;
            if (pathNodesClear) this.PathNodes.Clear();
            List<int> pathNodes = ObjectCopier.Clone(this.PathNodes);
            try
            {
                int fromNodeId = vehicle.CurrentPath.FromNodeID;
                int toNodeId = vehicle.CurrentPath.ToNodeID;

                int targetNode = 0;
                m_IsPathLoop = IsCommandLoopLink(vehicle, ref targetNode); // targetNode는 last Link의 FromNodeID
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check1 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;

                // 출발지는 CurrentLink.ToNodeID로 하자!
                // targetNode는 last Link.FromNodeID 이다. Link.ToNodeID는 PathFinder후 넣어 줘야한다.
                if (targetNode != 0)
                {
                    if (fromNodeId == targetNode && !m_IsPathLoop) // 동일 Path 이동
                    {
                        // 모두 지우고 From만 넣자 ! PathNodes는 사용할 필요 없다..
                        pathNodes.Clear();
                    }
                    else if (toNodeId == targetNode) // 이웃 Link까지 이동
                    {
                        // Received PathNodes는 사용할 필요 없다..모두 지우고 From -> To 만 넣자 !
                        pathNodes.Clear();
                        pathNodes.Add(toNodeId);
                    }
                    else
                    {
                        if (pathNodes.Count <= 2)
                        {
                            pathNodes.Clear();
                            pathNodes.Add(toNodeId);
                            pathNodes.Add(targetNode);
                        }
                        else
                        {
                            if (pathNodes[0] != toNodeId) pathNodes.Insert(1, toNodeId);
                            if (pathNodes.Last() != targetNode) pathNodes.Add(targetNode);
                        }
                    }
                    SetPathNodes(pathNodes); // first path ToNodeID ~ last Path FromNodeID, 자기 자신 이동이면 count가 0
                    if (pathNodes.Count == 0) pathNodes.Insert(0, fromNodeId); // From
                    if (pathNodes.First() != fromNodeId) pathNodes.Insert(0, fromNodeId); // From
                }
                if (pathNodes.Count > 0)
                {
                    List<int> fullPathNodeList = GetFindNodes(pathNodes);
                    if (AppConfig.Instance.Simulation.MY_DEBUG) time_check2 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;

                    if (targetNode == 0) targetNode = pathNodes.Last();
                    if (fullPathNodeList == null) fullPathNodeList = new List<int>();
                    if (fullPathNodeList.Count == 0) fullPathNodeList.AddRange(pathNodes);
                    DataItem_Node target_node = DatabaseHandler.Instance.GetNodeDataOrNull(targetNode);
                    bool stop_node = false;
                    if (target_node != null)
                    {
                        stop_node |= target_node.Type == NodeType.MTL;
                        stop_node |= target_node.Type == NodeType.Lifter;
                    }

                    int last_node = 0;
                    if (!stop_node) // stop node가 아닐 경우 last node 추가한다.
                    {
                        List<DataItem_Link> links = DatabaseHandler.Instance.DictionaryLinkDataList.Values.Where(x => x.FromNodeID == targetNode).ToList();
                        if (links.Count == 1) // 1개 이상인 node는 분기점... 분기점 멈추는것 이상한일...
                        {
                            last_node = links.FirstOrDefault().ToNodeID;
                        }
                        else
                        {
                            //Port가 분기점에 있을수도있다..
                            // Go 할때만 예외처리하자....!
                            if (this.TypeOfDestination != enGoCommandType.ByDistance)
                            {
                                int targetPort = 0;
                                if (this.ProcessCommand == OCSCommand.Go || this.ProcessCommand == OCSCommand.Teaching)
                                {
                                    targetPort = this.DestinationID;
                                }
                                else
                                {
                                    if (carrier_installed && this.DestinationID != 0) targetPort = this.DestinationID;
                                    else if (this.SourceID != 0) targetPort = this.SourceID;
                                }
                                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(targetPort))
                                {
                                    int targetLinkID = DatabaseHandler.Instance.DictionaryPortDataList[targetPort].LinkID;
                                    links = DatabaseHandler.Instance.DictionaryLinkDataList.Values.Where(x => x.LinkID == targetLinkID).ToList();
                                    if (links.Count > 0)
                                    {
                                        last_node = links.FirstOrDefault().ToNodeID;
                                    }
                                }
                            }
                        }
                    }

                    if (last_node != 0 && fullPathNodeList.Count > 0 && fullPathNodeList.Last() != last_node) 
                        fullPathNodeList.Add(last_node);

                    pathNodes.Clear();
                    pathNodes.AddRange(fullPathNodeList);
                }

                // 만일 연속된 Node 번호가 같다면 지우자 !
                if (pathNodes.Count > 1)
                {
                    for (int i = 1; i < pathNodes.Count; i++)
                    {
                        if (pathNodes[i] == pathNodes[i - 1]) pathNodes.RemoveAt(i);
                    }
                }

                m_TargetNode = targetNode; // Last Link의 FromNode여야 함.... 이동 중 멈추었다가...바뀔 경우도 있을거임...
                this.StartNode = toNodeId;
                this.EndNode = targetNode;
                if (pathNodes.Count > 0)
                    SetFullPathNodes(pathNodes);
                else rv = false;
            }
            catch (Exception ex)
            {
                rv = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }

            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                double watch = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine($"{time} : [MakeNodes-{this.PathNodes.Count}] [{watch}] [{time_check1}] [{time_check2}]");
            }

            return rv;
        }
        /// <summary>
        /// Node List 순서대로 Database Link 정보를 가지고 PathMaps를 만들자.
        /// Before/Next LinkID 
        /// BCR Direction Ordering
        /// Steer Direction Ordering
        /// Link Velocity Optimizer (LinkMergeArea로 검토)
        ///   : 경로가 Curve로 연결될때와 Straight로 연결될때 속도 구분
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public bool MakePathMaps(List<int> nodeList, VehicleStatus vehicle)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            double time_check1 = 0;
            double time_check2 = 0;
            double time_check3 = 0;
            double time_check4 = 0;
            double time_check5 = 0;
            double time_check6 = 0;
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                stopWatch.Reset();
                stopWatch.Start();
            }

            bool rv = true;
            try
            {
                // make PathMaps 
                List<DataItem_Link> links = DatabaseHandler.Instance.DictionaryLinkDataList.Values.ToList();
                List<Path> pathLists = new List<Path>();
                for (int i = 0; i < nodeList.Count - 1; i++)
                {
                    int source = nodeList[i];
                    int detination = nodeList[i + 1];
                    int index = links.FindIndex(x=>x.FromNodeID == source && x.ToNodeID == detination);
                    if (index == -1) { rv = false; break; }

                    DataItem_Link link = links[index].GetCopyOrNull();
                    pathLists.Add(new Path(true, i, link));
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check1 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;

                for (int i = 0; i < pathLists.Count; i++)
                {
                    double applyLinkVelocity = ApplyLinkTypeToVelocity(pathLists[i].Velocity, pathLists[i].Type);
                    if (applyLinkVelocity != 0) { pathLists[i].Velocity = applyLinkVelocity; pathLists[i].LinkVelocity = applyLinkVelocity; }
                    double applyLinkAcceleration = ApplyLinkTypeToAcceleration(pathLists[i].Acceleration, pathLists[i].Type);
                    if (applyLinkAcceleration != 0) pathLists[i].Acceleration = applyLinkAcceleration;
                    double applyLinkDeceleration = ApplyLinkTypeToDeceleration(pathLists[i].Deceleration, pathLists[i].Type);
                    if (applyLinkDeceleration != 0) pathLists[i].Deceleration = applyLinkDeceleration;
                    double applyLinkJerk = ApplyLinkTypeToJerk(pathLists[i].Jerk, pathLists[i].Type);
                    if (applyLinkJerk != 0) pathLists[i].Jerk = applyLinkJerk;

                    if (i > 0) pathLists[i].FromLinkID = pathLists[i - 1].LinkID;
                    if (i < pathLists.Count - 1) pathLists[i].ToLinkID = pathLists[i + 1].LinkID;
                }
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check2 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                // BCR Order
                rv &= ApplyBCRDirectionOptimizer(pathLists, vehicle);
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check3 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                // Steer Order
                rv &= ApplySteerDirectionOptimizer(pathLists);
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check4 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                // Velocity Optimizer
                rv &= ApplyLinkVelocityOptimizer(pathLists);
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check5 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                // Distance Optimizer
                rv &= ApplyLinkPositionOptimizer(pathLists, vehicle);
                if (AppConfig.Instance.Simulation.MY_DEBUG) time_check6 = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                if (rv)
                {
                    // 여기까지는 Database에 있는 정보를 가지고 Path를 만드는 과정.
                    // PathMaps Change
                    m_PathMaps.Clear();
                    m_PathMaps.AddRange(ObjectCopier.Clone(pathLists));
                }
            }
            catch (Exception ex)
            {
                rv = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }

            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                double watch = (double)stopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine($"{time} : [MakePathMaps-{this.PathNodes.Count}] [{watch}] [{time_check1}] [{time_check2}] [{time_check3}] [{time_check4}] [{time_check5}] [{time_check6}]");
            }

            return rv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool MakeRouteFullPath(VehicleStatus vehicle)
        {
            bool ok = true;
            try
            {
                if (ok)
                {
                    bool Judge_MakeNode = MakeNodes(vehicle, false); // Nodes를 구하자 !
                    ok &= Judge_MakeNode;
                    if(!Judge_MakeNode)
                        SequenceLog.WriteLog("[TransferCommand]", "MakeRouteFullPath Make Node Error, pathNodes Count = 0 !");
                }
                if (ok)
                {
                    bool Judge_MakePathMap = MakePathMaps(this.FullPathNodes, vehicle); // PathMap을 만들자 !
                    ok &= Judge_MakePathMap;
                    if(!Judge_MakePathMap)
                        SequenceLog.WriteLog("[TransferCommand]", "MakeRouteFullPath Make Path Map Error");
                }
                if (ok)
                {
                    if (m_PathMaps.Count > 0)
                    {
                        this.StartNode = m_PathMaps.First().ToNodeID;
                        this.EndNode = m_PathMaps.Last().FromNodeID;

                        // SPL은 정위치 정지를 해야 한다.
                        DataItem_Node spl_node = DatabaseHandler.Instance.GetNodeDataOrNull(m_PathMaps.Last().ToNodeID);
                        bool bcr_scan_command = false;
                        bcr_scan_command |= ProcessCommand == OCSCommand.Transfer;
                        bcr_scan_command |= ProcessCommand == OCSCommand.Go && TypeOfDestination == enGoCommandType.ByLocation;
                        bcr_scan_command |= ProcessCommand == OCSCommand.Teaching;
                        if (bcr_scan_command)
                        {
                            m_PathMaps.Last().BcrScanUse = true;
                        }
                        EventHandlerManager.Instance.InvokeUpdatePathMapChanged(this);
                    }
                    else
                    {
                        ok = false;
                        SequenceLog.WriteLog("[TransferCommand]", "MakeRouteFullPath Make Path Error, PathMaps Count = 0 !");
                    }
                }

            }
            catch (Exception ex)
            {
                ok = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            IsNeedMakeRouteFullPath = ok ? false : true;

            return ok;
        }
        private bool ApplySteerDirectionOptimizer(List<Path> pathLists)
        {
            bool rv = true;
            try
            {
                if (pathLists != null && pathLists.Count > 0)
                {
                    bool find = false;
                    enSteerDirection last_saved = enSteerDirection.DontCare;
                    int dontCareIndex = pathLists.Count - 1;
                    for (int i = pathLists.Count - 1; i >= 0; i--)
                    {
                        if (!find && pathLists[i].SteerDirection != enSteerDirection.DontCare) 
                        {
                            last_saved = pathLists[i].SteerDirection;
                            dontCareIndex = i; 
                            find = true; 
                        }
                        if (find)
                        {
                            if (pathLists[i].SteerDirection == enSteerDirection.DontCare) pathLists[i].SteerDirection = last_saved;
                            else last_saved = pathLists[i].SteerDirection;
                        }
                    }
                    for (int i = dontCareIndex; i < pathLists.Count; i++)
                    {
                        if (pathLists[i].SteerDirection == enSteerDirection.DontCare) pathLists[i].SteerDirection = last_saved;
                        else
                        {
                            bool lastBeforeCurve = pathLists[i].SteerChangeLeftBCR != 0.0f || pathLists[i].SteerChangeRightBCR != 0.0f;
                            lastBeforeCurve &= i > pathLists.Count - 2;
                            if (lastBeforeCurve) // S-Curve
                                last_saved = pathLists[i].SteerDirection == enSteerDirection.Right ? enSteerDirection.Left : enSteerDirection.Right; //다음꺼 반대로 설정.
                            else last_saved = pathLists[i].SteerDirection;
                        }
                    }
                }
                else
                {
                    rv = false;
                }
            }
            catch (Exception ex)
            {
                rv = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return rv;
        }
        private bool ApplyBCRDirectionOptimizer(List<Path> pathLists, VehicleStatus vehicle)
        {
            bool rv = true;
            try
            {
                if (pathLists != null && pathLists.Count > 0)
                {
                    bool find = false;
                    enBcrCheckDirection last_saved = enBcrCheckDirection.Both;

                    double leftBcr = vehicle.CurrentBcrStatus.LeftBcr;
                    double rightBcr = vehicle.CurrentBcrStatus.RightBcr;
                    double inRange = SetupManager.Instance.SetupWheel.InRange;
                    if (pathLists.Count == 1)
                    {
                        // BCR 방향은 가까운쪽으로 선택하자... InRange 안에 들어온것이 있으면 그걸 선택
                        double leftDiff = TargetLeftBcrPosition - leftBcr;
                        double rightDiff = TargetRightBcrPosition - rightBcr;
                        if (Math.Abs(leftDiff) < inRange) last_saved = enBcrCheckDirection.Left;
                        else if (Math.Abs(rightDiff) < inRange) last_saved = enBcrCheckDirection.Right;
                        if (Math.Abs(leftDiff) < inRange && Math.Abs(rightDiff) < inRange)
                        {
                            if (Math.Abs(leftDiff) < Math.Abs(rightDiff)) last_saved = enBcrCheckDirection.Left;
                            else last_saved = enBcrCheckDirection.Right;
                            pathLists[0].BcrDirection = last_saved;
                        }
                        if (last_saved != enBcrCheckDirection.Both) TargetBcrCheckDirection = last_saved;
                    }

                    int dontCareIndex = pathLists.Count - 1;
                    for (int i = pathLists.Count - 1; i >= 0; i--)
                    {
                        if (!find && pathLists[i].BcrDirection != enBcrCheckDirection.Both)
                        {
                            last_saved = pathLists[i].BcrDirection;
                            dontCareIndex = i;
                            find = true;
                        }
                        if (find)
                        {
                            if (pathLists[i].BcrDirection == enBcrCheckDirection.Both) pathLists[i].BcrDirection = last_saved;
                            else last_saved = pathLists[i].BcrDirection;
                        }
                    }
                    for (int i = dontCareIndex; i < pathLists.Count; i++)
                    {
                        if (pathLists[i].BcrDirection == enBcrCheckDirection.Both) pathLists[i].BcrDirection = last_saved;
                        else
                        {
                            bool lastBeforeCurve = pathLists[i].SteerChangeLeftBCR != 0.0f || pathLists[i].SteerChangeRightBCR != 0.0f;
                            lastBeforeCurve &= i > pathLists.Count - 2;
                            if (lastBeforeCurve) // S-Curve
                                last_saved = pathLists[i].BcrDirection == enBcrCheckDirection.Right ? enBcrCheckDirection.Left : enBcrCheckDirection.Right; //다음꺼 반대로 설정.
                            else last_saved = pathLists[i].BcrDirection;
                        }
                    }
                }
                else
                {
                    rv = false;
                }
            }
            catch (Exception ex)
            {
                rv = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return rv;
        }
        private bool ApplyLinkVelocityOptimizer(List<Path> pathLists)
        {
            bool rv = true;
            try
            {
                if (DatabaseHandler.Instance.DictionaryLinkMergeArea.Count > 0)
                {
                    if (pathLists != null && pathLists.Count > 0)
                    {
                        List<int> linkIds = pathLists.Select(x => x.LinkID).ToList();
                        // Link Merage의 NodeList와 LinkLists의 노드 순서가 일치하는 부분이 있을때 속도 조절
                        List< DataItem_LinkMergeArea > linkMergeArea = DatabaseHandler.Instance.DictionaryLinkMergeArea.Values.ToList();
                        foreach (DataItem_LinkMergeArea area in linkMergeArea)
                        {
                            if (area == null) continue;
                            if (area.AreaCount == 0) continue;
                            double newVel = area.AreaVelocity;
                            int index = area.CompareWithLinkID(linkIds);
                            if (index > -1 && newVel > 0)
                            {
                                for (int i = 0; i < area.AreaCount; i++)
                                {
                                    int n = index + i;
                                    if (n < linkIds.Count) { pathLists[n].Velocity = newVel; pathLists[n].LinkVelocity = newVel; }
                                }
                            }
                        }
                    }
                    else
                    {
                        rv = false;
                    }
                }
            }
            catch (Exception ex)
            {
                rv = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return rv;
        }
        private bool ApplyLinkPositionOptimizer(List<Path> pathLists, VehicleStatus vehicle)
        {
            bool rv = true;
            try
            {
                if (pathLists != null && pathLists.Count > 0)
                {
                    // Distance 보정이 필요하다.
                    // 첫번째 Current 위치에서 Link.ToNode 까지의 거리
                    // 마지막 Link.FromNode에서 Destination까지의 거리
                    bool carrier_installed = vehicle.CarrierStatus == IF.OCS.CarrierState.Installed;
                    double curLeftBCR = vehicle.CurrentBcrStatus.LeftBcr;
                    double curRightBCR = vehicle.CurrentBcrStatus.RightBcr;
                    if (pathLists.Count == 1)
                    {
                        // 출발점
                        pathLists.First().LeftBCRStart = curLeftBCR;
                        pathLists.First().RightBCRStart = curRightBCR;

                        // 도착점
                        enBcrCheckDirection check = pathLists.Last().BcrDirection;
                        double curBCR = check == enBcrCheckDirection.Right ? curRightBCR : curLeftBCR;
                        double targetBCR = 0.0f;
                        if (this.ProcessCommand == OCSCommand.Go && this.TypeOfDestination == enGoCommandType.ByDistance)
                        {
                            if (this.DestinationID == pathLists.Last().FromNodeID)
                            {
                                double fromBCR = check == enBcrCheckDirection.Right ? pathLists.Last().RightBCRBegin : pathLists.Last().LeftBCRBegin;
                                pathLists.Last().LeftBCRTarget = pathLists.Last().LeftBCRBegin + this.TargetNodeToDistance;
                                pathLists.Last().RightBCRTarget = pathLists.Last().RightBCRBegin + this.TargetNodeToDistance;
                                targetBCR = fromBCR;
                                targetBCR += this.TargetNodeToDistance;
                            }
                            else
                            {
                                SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer#1 NG : DestinationID={0}, pathLists.Last().FromNodeID={1}", this.DestinationID, pathLists.Last().FromNodeID));
                                rv = false;
                            }
                        }
                        else
                        {
                            int portID = 0;
                            if (this.ProcessCommand == OCSCommand.Go || this.ProcessCommand == OCSCommand.Teaching)
                            {
                                portID = this.DestinationID;
                            }
                            else
                            {
                                if (carrier_installed && this.DestinationID != 0) portID = this.DestinationID;
                                else if (this.SourceID != 0) portID = this.SourceID;
                            }
                            if (portID != 0 && DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID))
                            {
                                double offset_left = DatabaseHandler.Instance.DictionaryPortDataList[portID].DriveLeftOffset;
                                double offset_right = DatabaseHandler.Instance.DictionaryPortDataList[portID].DriveRightOffset;
                                pathLists.Last().LeftBCRTarget = DatabaseHandler.Instance.DictionaryPortDataList[portID].BarcodeLeft + offset_left;
                                pathLists.Last().RightBCRTarget = DatabaseHandler.Instance.DictionaryPortDataList[portID].BarcodeRight + offset_right;
                                targetBCR = check == enBcrCheckDirection.Right ? pathLists.Last().RightBCRTarget : pathLists.Last().LeftBCRTarget;
                            }
                            else
                            {
                                SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer#2 NG : portID={0}", portID));
                                rv = false;
                            }
                        }
                        if (rv)
                        {
                            double distance = targetBCR - curBCR;
                            bool move_enable = Math.Abs(distance) <= SetupManager.Instance.SetupWheel.InRangeMakePathStartPosition;
                            move_enable |= distance > -1.0f; // Margin 생각
                            if (move_enable && distance < pathLists.Last().Distance)
                            {
                                pathLists.Last().RunDistance = distance;
                            }
                            else
                            {
                                SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer#3 NG : distance={0}, InRangeMakePathStartPosition={1}", distance, SetupManager.Instance.SetupWheel.InRangeMakePathStartPosition));
                                rv = false;
                            }
                        }
                    }
                    else
                    {
                        // 출발점
                        pathLists.First().LeftBCRStart = curLeftBCR;
                        pathLists.First().RightBCRStart = curRightBCR;
                        enBcrCheckDirection check = pathLists.First().BcrDirection;
                        double curBCR = check == enBcrCheckDirection.Right ? curRightBCR : curLeftBCR;
                        double toBCR = check == enBcrCheckDirection.Right ? pathLists.First().RightBCREnd : pathLists.First().LeftBCREnd;
                        double distance = toBCR - curBCR;
                        if (distance > 0 && distance <= pathLists.First().Distance)
                        {
                            pathLists.First().RunDistance = distance;
                        }
                        else
                        {
                            SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer#4 NG : distance={0}, pathLists.First().Distance={1}", distance, pathLists.First().Distance));
                            rv = false;
                        }

                        // 도착점
                        check = pathLists.Last().BcrDirection;
                        double targetBCR = 0.0f;
                        double fromBCR = check == enBcrCheckDirection.Right ? pathLists.Last().RightBCRBegin : pathLists.Last().LeftBCRBegin;
                        if (this.ProcessCommand == OCSCommand.Go && this.TypeOfDestination == enGoCommandType.ByDistance)
                        {
                            //if (this.DestinationID == vehicle.CurrentPath.FromNodeID) // loop case ... multi path & destination is cur-path
                            //{
                            //}
                            pathLists.Last().LeftBCRTarget = pathLists.Last().LeftBCRBegin + this.TargetNodeToDistance;
                            pathLists.Last().RightBCRTarget = pathLists.Last().RightBCRBegin + this.TargetNodeToDistance;
                            targetBCR = fromBCR;
                            targetBCR += this.TargetNodeToDistance;
                        }
                        else
                        {
                            int portID = 0;
                            if (this.ProcessCommand == OCSCommand.Go || this.ProcessCommand == OCSCommand.Teaching)
                            {
                                portID = this.DestinationID;
                            }
                            else
                            {
                                if (carrier_installed && this.DestinationID != 0) portID = this.DestinationID;
                                else if (this.SourceID != 0) portID = this.SourceID;
                            }
                            if (portID != 0 && DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(portID))
                            {
                                double offset_left = DatabaseHandler.Instance.DictionaryPortDataList[portID].DriveLeftOffset;
                                double offset_right = DatabaseHandler.Instance.DictionaryPortDataList[portID].DriveRightOffset;
                                pathLists.Last().LeftBCRTarget = DatabaseHandler.Instance.DictionaryPortDataList[portID].BarcodeLeft + offset_left;
                                pathLists.Last().RightBCRTarget = DatabaseHandler.Instance.DictionaryPortDataList[portID].BarcodeRight + offset_right;
                                targetBCR = check == enBcrCheckDirection.Right ? pathLists.Last().RightBCRTarget : pathLists.Last().LeftBCRTarget;
                            }
                            else
                            {
                                SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer#5 NG : portID={0}", portID));
                                rv = false;
                            }
                        }
                        if (rv)
                        {
                            distance = targetBCR - fromBCR;
                            bool move_enable = Math.Abs(distance) <= SetupManager.Instance.SetupWheel.InRangeMakePathStartPosition;
                            move_enable |= distance > -1.0f; // Margin 생각
                            if (move_enable && distance <= pathLists.Last().Distance)
                            {
                                pathLists.Last().RunDistance = distance;
                            }
                            else
                            {
                                SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer#6 NG : distance={0}, pathLists.Last().Distance={1}", distance, pathLists.Last().Distance));
                                rv = false;
                            }
                        }
                    }

                    // Accumulate Distance Calculate
                    double sum = 0.0f;
                    foreach (Path path in pathLists)
                    {
                        sum += path.RunDistance;
                        path.TotalDistance = sum;
                        path.MotionProc = enMotionProc.ready;
                    }

                    //////////////////////////////////////////////////
                    m_TargetBcrCheckDirection = pathLists.Last().BcrDirection;
                    m_TargetLeftBcrPosition = pathLists.Last().LeftBCRTarget;
                    m_TargetRightBcrPosition = pathLists.Last().RightBCRTarget;
                    //////////////////////////////////////////////////
                }
                else
                {
                    SequenceLog.WriteLog("[TransferCommand]", string.Format("ApplyLinkPositionOptimizer NG : Path Count NULL"));
                    rv = false;
                }
            }
            catch (Exception ex)
            {
                rv = false;
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return rv;
        }
        private double ApplyLinkTypeToVelocity(double velocity, LinkType myLinkType)
        {
            double applyVelocity = 0.0f;
            try
            {
                double limitVelocity = velocity;
                switch (myLinkType)
                {
                    case LinkType.Ascent:
                    case LinkType.Straight:
                    case LinkType.AutoDoorStraight:
                        limitVelocity = SetupManager.Instance.SetupWheel.StraightSpeed;
                        break;
                    case LinkType.LeftBranchStraight:
                    case LinkType.RightBranchStraight:
                    case LinkType.SideStraight:
                        limitVelocity = SetupManager.Instance.SetupWheel.BranchStraightSpeed;
                        break;
                    case LinkType.JunctionStraight:
                    case LinkType.LeftJunctionStraight:
                    case LinkType.RightJunctionStraight:
                    case LinkType.SideLeftJunctionStraight:
                    case LinkType.SideRightJunctionStraight:
                        limitVelocity = SetupManager.Instance.SetupWheel.JunctionStraightSpeed;
                        break;
                    case LinkType.LeftCurve:
                    case LinkType.RightCurve:
                    case LinkType.AutoDoorCurve:
                        limitVelocity = SetupManager.Instance.SetupWheel.CurveSpeed;
                        break;
                    case LinkType.LeftBranch:
                    case LinkType.RightBranch:
                        limitVelocity = SetupManager.Instance.SetupWheel.BranchSpeed;
                        break;
                    case LinkType.LeftJunction:
                    case LinkType.RightJunction:
                        limitVelocity = SetupManager.Instance.SetupWheel.JunctionSpeed;
                        break;
                    case LinkType.LeftCompositedSCurveBranch:
                    case LinkType.RightCompositedSCurveBranch:
                    case LinkType.LeftSBranch:
                    case LinkType.RightSBranch:
                        limitVelocity = SetupManager.Instance.SetupWheel.SBranchSpeed;
                        break;
                    case LinkType.LeftSJunction:
                    case LinkType.RightSJunction:
                    case LinkType.LeftCompositedSCurveJunction:
                    case LinkType.RightCompositedSCurveJunction:
                        limitVelocity = SetupManager.Instance.SetupWheel.SJunctionSpeed;
                        break;
                    case LinkType.Descent:
                        break;
                    default:
                        break;
                }
                applyVelocity = velocity > limitVelocity ? limitVelocity : velocity;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return applyVelocity;
        }
        private double ApplyLinkTypeToAcceleration(double acceleration, LinkType myLinkType)
        {
            double applyAcceleration = 0.0f;
            try
            {
                double limitAcceleration = acceleration;
                switch (myLinkType)
                {
                    case LinkType.Straight:
                    case LinkType.LeftBranchStraight:
                    case LinkType.RightBranchStraight:
                    case LinkType.LeftJunctionStraight:
                    case LinkType.RightJunctionStraight:
                    case LinkType.SideStraight:
                    case LinkType.SideLeftJunctionStraight:
                    case LinkType.SideRightJunctionStraight:
                    case LinkType.JunctionStraight:
                    case LinkType.Ascent:
                    case LinkType.Descent:
                        limitAcceleration = SetupManager.Instance.SetupWheel.StraightMaxAcc;
                        break;
                    case LinkType.LeftBranch:
                    case LinkType.RightBranch:
                        limitAcceleration = SetupManager.Instance.SetupWheel.BranchMaxAcc;
                        break;
                    case LinkType.LeftCurve:
                    case LinkType.RightCurve:
                    case LinkType.LeftCompositedSCurveBranch:
                    case LinkType.RightCompositedSCurveBranch:
                    case LinkType.LeftCompositedSCurveJunction:
                    case LinkType.RightCompositedSCurveJunction:
                        limitAcceleration = SetupManager.Instance.SetupWheel.CurveMaxAcc;
                        break;
                    case LinkType.LeftJunction:
                    case LinkType.RightJunction:
                    case LinkType.LeftSJunction:
                    case LinkType.RightSJunction:
                        limitAcceleration = SetupManager.Instance.SetupWheel.JunctionMaxAcc;
                        break;
                    default:
                        break;
                }
                applyAcceleration = acceleration > limitAcceleration ? limitAcceleration : acceleration;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return applyAcceleration;
        }
        private double ApplyLinkTypeToDeceleration(double deceleration, LinkType myLinkType)
        {
            double applyDeceleration = 0.0f;
            try
            {
                double limitDeceleration = deceleration;
                switch (myLinkType)
                {
                    case LinkType.Straight:
                    case LinkType.LeftBranchStraight:
                    case LinkType.RightBranchStraight:
                    case LinkType.LeftJunctionStraight:
                    case LinkType.RightJunctionStraight:
                    case LinkType.SideStraight:
                    case LinkType.SideLeftJunctionStraight:
                    case LinkType.SideRightJunctionStraight:
                    case LinkType.JunctionStraight:
                    case LinkType.Ascent:
                    case LinkType.Descent:
                        limitDeceleration = SetupManager.Instance.SetupWheel.StraightMaxDec;
                        break;
                    case LinkType.LeftBranch:
                    case LinkType.RightBranch:
                        limitDeceleration = SetupManager.Instance.SetupWheel.BranchMaxDec;
                        break;
                    case LinkType.LeftCurve:
                    case LinkType.RightCurve:
                    case LinkType.LeftCompositedSCurveBranch:
                    case LinkType.RightCompositedSCurveBranch:
                    case LinkType.LeftCompositedSCurveJunction:
                    case LinkType.RightCompositedSCurveJunction:
                        limitDeceleration = SetupManager.Instance.SetupWheel.CurveMaxDec;
                        break;
                    case LinkType.LeftJunction:
                    case LinkType.RightJunction:
                    case LinkType.LeftSJunction:
                    case LinkType.RightSJunction:
                        limitDeceleration = SetupManager.Instance.SetupWheel.JunctionMaxDec;
                        break;
                    default:
                        break;
                }
                applyDeceleration = deceleration > limitDeceleration ? limitDeceleration : deceleration;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return applyDeceleration;
        }
        private double ApplyLinkTypeToJerk(double jerk, LinkType myLinkType)
        {
            double applyJerk = 0.0f;
            try
            {
                double limitJerk = jerk;
                switch (myLinkType)
                {
                    case LinkType.Straight:
                    case LinkType.LeftBranchStraight:
                    case LinkType.RightBranchStraight:
                    case LinkType.LeftJunctionStraight:
                    case LinkType.RightJunctionStraight:
                    case LinkType.SideStraight:
                    case LinkType.SideLeftJunctionStraight:
                    case LinkType.SideRightJunctionStraight:
                    case LinkType.JunctionStraight:
                    case LinkType.Ascent:
                    case LinkType.Descent:
                        limitJerk = SetupManager.Instance.SetupWheel.StraightJerk;
                        break;
                    case LinkType.LeftBranch:
                    case LinkType.RightBranch:
                        limitJerk = SetupManager.Instance.SetupWheel.BranchJerk;
                        break;
                    case LinkType.LeftCurve:
                    case LinkType.RightCurve:
                    case LinkType.LeftCompositedSCurveBranch:
                    case LinkType.RightCompositedSCurveBranch:
                    case LinkType.LeftCompositedSCurveJunction:
                    case LinkType.RightCompositedSCurveJunction:
                        limitJerk = SetupManager.Instance.SetupWheel.CurveJerk;
                        break;
                    case LinkType.LeftJunction:
                    case LinkType.RightJunction:
                    case LinkType.LeftSJunction:
                    case LinkType.RightSJunction:
                        limitJerk = SetupManager.Instance.SetupWheel.JunctionJerk;
                        break;
                    default:
                        break;
                }
                applyJerk = jerk > limitJerk ? limitJerk : jerk;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
            }
            return applyJerk;
        }
        /// <summary>
        /// Next Stop Node Find
        /// make RunPathMaps
        /// curNode 는 IsNearNode로 판단...
        /// </summary>
        /// <returns></returns>
        public bool MakeNextTargetPath(Path myPath, VehicleStatus vehicle)
        {
            bool rv = true;
            try
            {
                List<Path> newPaths = new List<Path>();
                rv &= m_PathMaps.Count > 0 ? m_PathMaps.Find(x => x.LinkID == myPath.LinkID) != null : false;
                if (rv)
                {
                    string msg = $"[MakeNextTargetPath] m_PathMaps Count:{m_PathMaps.Count}\r\n";
                    string finished_msg = $"Motion Process is Finished : ";

                    double bcrLeft = vehicle.CurrentBcrStatus.LeftBcr;
                    double bcrRight = vehicle.CurrentBcrStatus.RightBcr;
                    DataItem_Node curNode = vehicle.CurrentPath.IsNearNode(bcrLeft, bcrRight); // 무조건 ToNode를 선택할 수 없다.

                    // myPath 이후 Path를 순서대로 처리하자 !!
                    bool stopNode = false;
                    bool find_link = false;
                    int pathindex = 0;
                    foreach (Path path in m_PathMaps)
                    {
                        pathindex++;
                        if (path.MotionProc == enMotionProc.finished)
                        {
                            finished_msg += $"[{path.Index},{path.LinkID}],";
                            continue;
                        }
                        //if (path.Index < myPath.Index)
                        if (pathindex < myPath.Index)
                        {
                            msg += $"Index={path.Index} Skip Path, MyIndex={myPath.Index}\r\n";
                            continue; // 자기 이후의 path만 사용하자 !
                        }
                        /// 이상상황
                        if (newPaths.Count == 0)
                        {
                            /// SPL도착 했는데 Finished 않돼었네... Node 위치 도착이라 그럴수 있음.
                            bool spl_skip = curNode.Type == NodeType.Lifter || curNode.Type == NodeType.MTL;
                            spl_skip &= path.ToNodeID == curNode.NodeID;
                            if (spl_skip)
                            {
                                msg += $"Index={path.Index} SPL Skip, curNode.Type={curNode.Type}, path.ToNodeID={path.ToNodeID} ,curNode.NodeID={curNode.NodeID}\r\n";
                                find_link = true;
                                continue;
                            }
                        }
                        if (find_link == false)
                        {
                            find_link = path.LinkID == myPath.LinkID;//path.Value.IsSame(myPath);
                            if (find_link == false)
                            {
                                msg += $"Index={path.Index} path.LinkID={path.LinkID} != myPath.LinkID={myPath.LinkID}\r\n";
                                continue;
                            }
                        }
                        Path toPath = GetToPath(path); // toPath가 null일 경우는 마지막 Path 이다...
                        if (toPath != null && path.ToNodeID != curNode.NodeID)
                        {
                            DataItem_Node node = DatabaseHandler.Instance.GetNodeDataOrNull(path.ToNodeID);
                            stopNode |= node.Type == NodeType.MTLIn;
                            stopNode |= node.Type == NodeType.LifterIn;
                            stopNode |= node.Type == NodeType.Lifter;

                            bool jcs_stop = node.JCSCheck > 0;
                            jcs_stop &= !path.JcsPermit;
                            bool auto_door_stop = node.Type == NodeType.AutoDoorIn1;
                            auto_door_stop |= node.Type == NodeType.AutoDoorIn2;
                            auto_door_stop &= (toPath.Type == LinkType.AutoDoorCurve || toPath.Type == LinkType.AutoDoorStraight);
                            auto_door_stop &= !path.AutoDoorPermit;

                            stopNode |= jcs_stop;
                            stopNode |= auto_door_stop;
                            msg += $"Index={path.Index} jcs_stop:{jcs_stop}, jcspermit:{path.JcsPermit}, NodeId:{node}, auto_door_stop:{auto_door_stop}, auto_door_permit:{path.AutoDoorPermit}\r\n";
                        }

                        path.MotionProc = enMotionProc.wait;
                        newPaths.Add(ObjectCopier.Clone(path)); //복사해서 가지고 놀자 !
                        if (stopNode) break;
                    }
                    SequenceLog.WriteLog("[TransferCommand]", finished_msg + "\r\n" + msg);

                    bool finished = true;
                    finished &= m_PathMaps.Last().MotionProc == enMotionProc.finished;
                    finished &= GetToPath(vehicle.CurrentPath) == null;
                    if (newPaths.Count == 0 || finished)
                    {
                        SequenceLog.WriteLog("[TransferCommand]", string.Format("newPaths Count is Zero or LastPath is Finished"));
                        //m_TargetNode = 0;
                        rv = false; //더이상 진행할 path가 없다.
                    }
                    else
                    {
                        // 마지막 Node가 MTL/SPL인 경우 정위치 정지
                        bool bcrUsingNode = false;
                        DataItem_Node node = DatabaseHandler.Instance.GetNodeDataOrNull(newPaths.Last().ToNodeID);
                        bcrUsingNode |= node.Type == NodeType.MTL;
                        bcrUsingNode |= node.Type == NodeType.Lifter;
                        if (bcrUsingNode)
                        {
                            newPaths.Last().BcrScanUse = true;
                        }

                        // 출발 위치 갱신
                        newPaths.First().LeftBCRStart = bcrLeft;
                        newPaths.First().RightBCRStart = bcrRight;
                        enBcrCheckDirection check = newPaths.First().BcrDirection;
                        double curBCR = check == enBcrCheckDirection.Right ? bcrRight : bcrLeft;
                        double toBCR = check == enBcrCheckDirection.Right ? newPaths.First().RightBCREnd : newPaths.First().LeftBCREnd;
                        if (newPaths.First().FromNodeID == this.EndNode)
                        {
                            toBCR = check == enBcrCheckDirection.Right ? newPaths.First().RightBCRTarget : newPaths.First().LeftBCRTarget;
                        }
                        double distance = toBCR - curBCR;
                        if (distance > 0 && distance <= newPaths.First().Distance)
                        {
                            newPaths.First().RunDistance = distance;
                        }
                        // 도착점
                        if (stopNode && newPaths.Last().BcrScanUse == false)
                        {
                            double last_run_distance = newPaths.Last().RunDistance;
                            double target_offset = SetupManager.Instance.SetupWheel.StopPositionOffset;
                            if (last_run_distance + 2 * target_offset > 0)
                            {
                                newPaths.Last().RunDistance += target_offset;
                                newPaths.Last().LeftBCRTarget += target_offset;
                                newPaths.Last().RightBCRTarget += target_offset;
                            }
                            else
                            {
                                target_offset = -1 * (last_run_distance / 3);
                                newPaths.Last().RunDistance += target_offset;
                                newPaths.Last().LeftBCRTarget += target_offset;
                                newPaths.Last().RightBCRTarget += target_offset;
                            }
                        }

                        // Accumulate Distance Calculate
                        // path near from.to path check ... path 연속성 확인
                        bool continuePath = true;
                        int preLinkId = 0;
                        double sum = 0.0f;
                        foreach (Path path in newPaths)
                        {
                            sum += path.RunDistance;
                            path.TotalDistance = sum;
                            if (preLinkId != 0) continuePath &= preLinkId == path.FromLinkID;
                            preLinkId = path.LinkID;
                        }

                        if (continuePath)
                        {
                            //////////////////////////////////////////////////
                            m_TargetBcrCheckDirection = newPaths.Last().BcrDirection;
                            m_TargetLeftBcrPosition = newPaths.Last().LeftBCRTarget;
                            m_TargetRightBcrPosition = newPaths.Last().RightBCRTarget;
                            m_TargetBcrDistance = newPaths.Last().TotalDistance;
                            //////////////////////////////////////////////////

                            m_RunPathMaps.Clear();
                            m_RunPathMaps.AddRange(newPaths);
                            if (m_RunPathMaps.Last().ToNodeID == m_PathMaps.Last().ToNodeID && 
                                m_RunPathMaps.Last().Index == m_PathMaps.Last().Index && 
                                node.Type != NodeType.MTL)
                            {
                                m_TargetNode = m_RunPathMaps.Last().FromNodeID;
                            }
                            else
                            {
                                m_TargetNode = m_RunPathMaps.Last().ToNodeID; // 중간에 멈출 경우에는 ToNode
                            }
                        }
                        else
                        {
                            SequenceLog.WriteLog("[TransferCommand]", string.Format("PathMaps is Not Continue"));
                            rv = false; // 불연속 path schedule
                        }
                    }
                }
                else
                {
                    SequenceLog.WriteLog("[TransferCommand]", string.Format("myPath Link ID not Exist in PathMaps : {0}", myPath.ToString()));
                }
                rv &= m_RunPathMaps.Count > 0;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }

        public void SetPathMotionRun(List<Path> runPaths)
        {
            try
            {
                if (runPaths == null || runPaths.Count == 0 || m_PathMaps == null || m_PathMaps.Count == 0) return;
                List<int> indexs = runPaths.Select(x=>x.Index).ToList();
                foreach (Path path in m_PathMaps) if (indexs.Contains(path.Index)) path.MotionRun = true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public Path GetFromPath(Path myPath)
        {
            Path path = null;
            try
            {
                lock (m_PathMaps)
                {
                    if (myPath != null && m_PathMaps.Count > 0)
                    {
                        path = m_PathMaps.Find(x => x.LinkID == myPath.FromLinkID && x.Index < myPath.Index);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return path;
        }
        public Path GetToPath(Path myPath)
        {
            Path path = null;
            try
            {
                lock (m_PathMaps)
                {
                    if (myPath != null && m_PathMaps.Count > 0)
                    {
                        path = m_PathMaps.Find(x => x.LinkID == myPath.ToLinkID && x.Index > myPath.Index);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return path;
        }
        /// <summary>
        /// PathMaps에서 myPath 이전 path는 enMotionProc.finished 
        /// onlyOne == true일때는 자기 자신만 변경, Link Find가 잘 되지 않을 경우 대비
        /// </summary>
        /// <param name="myPath"></param>
        public void UpdateMotionProcess(Path myPath, bool onlyOne)
        {
            try
            {
                string message = $"UpdateMotionProcess\r\n";
                if (!onlyOne)
                {
                    if (m_PathMaps.Count > 0 && m_PathMaps.Find(x => x.LinkID == myPath.LinkID) != null)
                    {
                        bool complete = myPath.LinkID == m_PathMaps.Last().LinkID;
                        complete &= myPath.Index == m_PathMaps.Last().Index;
                        foreach (Path path in m_PathMaps)
                        {
                            if (path.IsSame(myPath) && !complete)
                            {
                                path.MotionProc = enMotionProc.inProc;
                                message += $"{path}->myPath1:{path.MotionProc}\r\n";
                                break;
                            }
                            path.MotionProc = enMotionProc.finished;
                            message += $"{path}->myPath2:{path.MotionProc}\r\n";
                        }
                        SequenceLog.WriteLog(message);
                        EventHandlerManager.Instance.InvokeUpdateMotionProcess(null);
                    }
                }
                else
                {
                    myPath.MotionProc = enMotionProc.inProc;
                    Path fromPath = GetFromPath(myPath);
                    EventHandlerManager.Instance.InvokeUpdateMotionProcess(myPath);
                    if (fromPath != null)
                    {
                        fromPath.MotionProc = enMotionProc.finished;
                        EventHandlerManager.Instance.InvokeUpdateMotionProcess(fromPath);
						message += $"{myPath}->fromPath:{fromPath.MotionProc}";
                        SequenceLog.WriteLog(message);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void SetMakeNodes(VehicleStatus vehicle)
        {
            m_ResultMakeNodes = -1;
            // Worker Process 생성
            BackgroundWorker makeNodesWorker = new BackgroundWorker();
            makeNodesWorker.DoWork += new DoWorkEventHandler(MakeNodes_DoWork);
            makeNodesWorker.RunWorkerAsync(vehicle);
        }
        public int ResultMakeNodes()
        {
            return m_ResultMakeNodes;
        }
        public void SetMakeRouteFullPath(VehicleStatus vehicle)
        {
            m_ResultMakeRouteFullPath = -1;
            // Worker Process 생성
            BackgroundWorker makeRouteFullPathWorker = new BackgroundWorker();
            makeRouteFullPathWorker.DoWork += new DoWorkEventHandler(MakeRouteFullPath_DoWork);
            makeRouteFullPathWorker.RunWorkerAsync(vehicle);
        }
        public int ResultMakeRouteFullPath()
        {
            return m_ResultMakeRouteFullPath;
        }
        #endregion

        #region Background Work
        private int m_ResultMakeNodes = -1;
        private void MakeNodes_DoWork(object sender, DoWorkEventArgs e)
        {
            VehicleStatus info = (VehicleStatus)e.Argument;

            try
            {
                bool ok = MakeNodes(info, true);
                if (ok) m_ResultMakeNodes = 0;
                else m_ResultMakeNodes = 1;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private int m_ResultMakeRouteFullPath = -1;
        private void MakeRouteFullPath_DoWork(object sender, DoWorkEventArgs e)
        {
            VehicleStatus info = (VehicleStatus)e.Argument;

            try
            {
                bool ok = MakeRouteFullPath(info);
                if (ok) m_ResultMakeRouteFullPath = 0;
                else m_ResultMakeRouteFullPath = 1;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

    }
}