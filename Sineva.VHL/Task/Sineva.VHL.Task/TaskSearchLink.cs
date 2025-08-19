using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Device;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sineva.VHL.Task.TaskMonitor;

namespace Sineva.VHL.Task
{
    public class TaskSearchLink : XSequence
    {
        public static readonly TaskSearchLink Instance = new TaskSearchLink();
        public SeqSearchLink SearchLink = null;
        public TaskSearchLink()
        {
            SearchLink = new SeqSearchLink();
            this.RegSeq(SearchLink);
        }
        public class SeqSearchLink : XSeqFunc
        {
            private const string FuncName = "[SeqSearchLink]";

            #region Fields
            private ProcessDataHandler m_ProcessDataHandler = null;
            private DatabaseHandler m_DatabaseHandler = null;
            private _DevAxis m_MasterAxis = null;

            private double m_OldCurPosition = 0.0f;
            private double m_OldBcrPosition = 0.0f;
            private int m_OldLinkID = 0;
            private bool m_UpdateCurrentPath = false;
            private bool m_UpdateCurrentPathComplete = false;

            private System.Diagnostics.Stopwatch m_StopWatch = new System.Diagnostics.Stopwatch();
            #endregion

            #region Properties
            public bool UpdateCurrentPathComplete { get { return m_UpdateCurrentPathComplete; } set { m_UpdateCurrentPathComplete = value; } }
            #endregion

            #region Contructor
            public SeqSearchLink()
            {
                this.SeqName = $"SeqSearchLink";

                m_ProcessDataHandler = ProcessDataHandler.Instance;
                m_DatabaseHandler = DatabaseHandler.Instance;
                m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();

                EventHandlerManager.Instance.UpdatePathMapChanged += Instance_UpdatePathMapChanged;
            }
            private void Instance_UpdatePathMapChanged(object obj)
            {
                try
                {
                    m_UpdateCurrentPath = true;
                    UpdateCurrentPathComplete = false;
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
                switch (seqNo)
                {
                    case 0:
                        {
                            if (AppConfig.Instance.Simulation.MY_DEBUG)
                            {
                                m_StopWatch.Reset();
                                m_StopWatch.Start();
                            }

                            int curLinkId = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.LinkID;
                            double leftBCR = m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBCR = m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            double search_range = SetupManager.Instance.SetupOperation.LinkSearchRange;
                            double diff_range = SetupManager.Instance.SetupOperation.LinkSearchDiffRange;
                            enBcrCheckDirection bcrDirection = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.BcrDirection;
                            bool nonPath = m_ProcessDataHandler.CurTransferCommand.PathMaps.Count == 0 ? true : false;
                            if (nonPath)
                                bcrDirection = DevicesManager.Instance.DevSteer.GetFrontSteerDirection() == enSteerDirection.Left ? enBcrCheckDirection.Left : enBcrCheckDirection.Right;
                            Dictionary<int, BcrMapItem> searchDatas = m_ProcessDataHandler.CurBcrMapData.SearchSimilarLinks(leftBCR, rightBCR, search_range, diff_range, bcrDirection, curLinkId, nonPath);
                            if (searchDatas.Count > 0 || m_UpdateCurrentPath)
                            {
                                bool all_finished = EqpStateManager.Instance.OpMode == OperateMode.Manual;
                                int linkId = -1;
                                int nextLinkId = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.ToLinkID;
                                int index = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Index;

                                // PathList가 있는 경우 먼저 찾자
                                if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count > 0)
                                {
                                    int find_offset = 5;
                                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                                    {
                                        find_offset = m_ProcessDataHandler.CurTransferCommand.PathMaps.Count -
                                        m_ProcessDataHandler.CurTransferCommand.PathMaps.Count(x => x.MotionProc == enMotionProc.finished);
                                    }
                                    double curPos = m_MasterAxis.GetCurPosition();

                                    //all_finished &= m_ProcessDataHandler.CurTransferCommand.PathMaps.Count == 
                                    //    m_ProcessDataHandler.CurTransferCommand.PathMaps.Count(x => x.MotionProc == enMotionProc.finished);
                                    //int key = searchDatas.Keys.First();
                                    //Sineva.VHL.Data.Process.Path find_path = m_ProcessDataHandler.CurTransferCommand.PathMaps.Find(x => x.LinkID == key && x.Index >= index);
                                    //Sineva.VHL.Data.Process.Path find_path = m_ProcessDataHandler.CurTransferCommand.PathMaps.Find(x => x.LinkID == key && x.Index >= index && x.MotionProc != enMotionProc.finished);
                                    Sineva.VHL.Data.Process.Path myPath = ObjectCopier.Clone(m_ProcessDataHandler.CurVehicleStatus.CurrentPath);
                                    if (searchDatas.Keys.Contains(nextLinkId))
                                    {
                                        myPath = m_ProcessDataHandler.CurTransferCommand.PathMaps.Find(x => x.LinkID == nextLinkId &&
                                                                                                            x.Index >= index &&
                                                                                                            x.MotionProc != enMotionProc.finished);
                                    }
                                    else
                                    {
                                        List<Path> paths = m_ProcessDataHandler.CurTransferCommand.PathMaps.Where(x => x.Index >= index &&
                                                                                                                        x.Index < index + find_offset &&
                                                                                                                        x.MotionProc != enMotionProc.finished).ToList();
                                        foreach (Path path in paths)
                                            if (searchDatas.Keys.Contains(path.LinkID)) { myPath = path; break; }
                                    }

                                    if (m_UpdateCurrentPath)
                                    {
                                        myPath = m_ProcessDataHandler.CurTransferCommand.PathMaps.Find(x => x.LinkID == curLinkId &&
                                                                                                            x.Index >= 0 &&
                                                                                                            x.Index < index + find_offset &&
                                                                                                            x.MotionProc != enMotionProc.finished);
                                    }
                                    if (myPath != null)
                                    {
                                        //link가 바뀌었네...한바퀴 돌아가는 경우 같은 Node를 지나가면 문제가 생길수 있다. BCR Left/Right 값에 따라 변화가 생긴다. 자기부터 두단계까지만 정답.
                                        bool isSame = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.IsSame(myPath);
                                        if (!isSame || m_UpdateCurrentPath)
                                        {
                                            bool bcr_position_check = true;
                                            if (myPath.BcrDirection == enBcrCheckDirection.Left) bcr_position_check &= leftBCR > myPath.LeftBCRBegin;
                                            else if (myPath.BcrDirection == enBcrCheckDirection.Right) bcr_position_check &= rightBCR > myPath.RightBCRBegin;
                                            else
                                            {
                                                bcr_position_check &= leftBCR > myPath.LeftBCRBegin;
                                                bcr_position_check &= rightBCR > myPath.RightBCRBegin;
                                            }
                                            // CurrentPath가 Straight일 경우 myPath의 Steer 방향이 같을 경우 Update 가능하다.
                                            // 직선 다음 분기점일 경우 BCR 값이 동일하게 나오는 경우가 있어 헷갈린다.
                                            // 직선 구간의 Steer 방향으로 다음 Path를 선택하자 ~~~
                                            bool steer_direction_check = true;
                                            if (m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Type == LinkType.Straight &&
                                                m_ProcessDataHandler.CurVehicleStatus.CurrentPath.SteerDirection != enSteerDirection.DontCare)
                                            {
                                                steer_direction_check &= m_ProcessDataHandler.CurVehicleStatus.CurrentPath.SteerDirection == myPath.SteerDirection;
                                            }

                                            if ((bcr_position_check && steer_direction_check) || m_UpdateCurrentPath)
                                            {
                                                // Motor Target Position Setting
                                                myPath.MotorTargetPosition = curPos + myPath.RunDistance;
                                                m_ProcessDataHandler.CurVehicleStatus.CurrentPath = myPath;
                                                if (m_DatabaseHandler.DictionaryNodeDataList.ContainsKey(myPath.FromNodeID))
                                                    m_ProcessDataHandler.CurVehicleStatus.CurrentNode = m_DatabaseHandler.DictionaryNodeDataList[myPath.FromNodeID].GetCopyOrNull();
                                                EventHandlerManager.Instance.InvokeLinkChanged(myPath);
                                                SequenceLog.WriteLog(FuncName, $"FoundLink_In_Paths ({curLinkId}->{myPath.LinkID}) LINK:{myPath}_{index} BCR:({leftBCR},{rightBCR})");

                                                if (m_UpdateCurrentPath)
                                                {
                                                    m_UpdateCurrentPath = false;
                                                    UpdateCurrentPathComplete = true;
                                                }
                                                GV.RouteChangeTimeOverCheck = true;
                                            }
                                        }
                                    }
                                }
                                if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count == 0 || all_finished)
                                {
                                    GV.RouteChangeTimeOverCheck = false;

                                    if (linkId == -1)
                                        linkId = searchDatas.First().Key;

                                    bool find_path = true;
                                    find_path &= linkId != -1;
                                    find_path &= (linkId != curLinkId) || m_UpdateCurrentPath;
                                    find_path &= m_ProcessDataHandler.CurTransferCommand.PathMaps.Count == 0 || all_finished;

                                    if (find_path)
                                    {
                                        if (m_DatabaseHandler.DictionaryLinkDataList.ContainsKey(linkId))
                                        {
                                            DataItem_Link newLink = ObjectCopier.Clone(m_DatabaseHandler.DictionaryLinkDataList[linkId]);
                                            Data.Process.Path newPath = new Data.Process.Path(false, 0, newLink);
                                            if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count > 0)
                                            {
                                                Sineva.VHL.Data.Process.Path tempPath = m_ProcessDataHandler.CurTransferCommand.PathMaps.Find(x => x.LinkID == linkId);
                                                if (tempPath != null)
                                                {
                                                    newPath.Index = tempPath.Index;
                                                    newPath.FromNodeID = tempPath.FromNodeID;
                                                    newPath.ToNodeID = tempPath.ToNodeID;
                                                    newPath.FromLinkID = tempPath.FromLinkID;
                                                    newPath.ToLinkID = tempPath.ToLinkID;
                                                    newPath.TotalDistance = tempPath.TotalDistance;
                                                    newPath.RunDistance = tempPath.RunDistance;
                                                    newPath.SteerDirection = tempPath.SteerDirection;
                                                    newPath.BcrDirection = tempPath.BcrDirection;
                                                    newPath.BcrScanUse = tempPath.BcrScanUse;
                                                    newPath.LeftBCRStart = tempPath.LeftBCRStart;
                                                    newPath.RightBCRStart = tempPath.RightBCRStart;
                                                    newPath.LeftBCRTarget = tempPath.LeftBCRTarget;
                                                    newPath.RightBCRTarget = tempPath.RightBCRTarget;
                                                }
                                            }
                                            m_ProcessDataHandler.CurVehicleStatus.CurrentPath = newPath;
                                            if (m_DatabaseHandler.DictionaryNodeDataList.ContainsKey(newPath.FromNodeID))
                                                m_ProcessDataHandler.CurVehicleStatus.CurrentNode = m_DatabaseHandler.DictionaryNodeDataList[newPath.FromNodeID].GetCopyOrNull();

                                            EventHandlerManager.Instance.InvokeLinkChanged(newPath);
                                            SequenceLog.WriteLog(FuncName, $"FoundLink_In_DB ({curLinkId}->{linkId}) LINK:{newLink} BCR:({leftBCR},{rightBCR})");

                                            if (m_UpdateCurrentPath)
                                            {
                                                m_UpdateCurrentPath = false;
                                                UpdateCurrentPathComplete = true;
                                            }
                                        }
                                    }

                                    if (m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Index != 0)
                                        m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Index = 0;
                                }
                            }

                            // Current Port Update
                            CheckCurrentPort(leftBCR, rightBCR);
                            // Current Link Position Update
                            CheckCurrentPositionOfLink(m_ProcessDataHandler.CurVehicleStatus.CurrentPath, leftBCR, rightBCR, search_range, diff_range);
                            /// Current Path 정보 Update
                            m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.BcrDirection = bcrDirection;
                            m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsUpperAreaValue = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.UBSLevel0;
                            m_ProcessDataHandler.CurVehicleStatus.ObsStatus.ObsLowerAreaValue = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.UBSLevel1;
                            // Target In Range Check
                            CheckInPosition(bcrDirection, leftBCR, rightBCR);

                            //if (AppConfig.Instance.Simulation.MY_DEBUG)
                            //{
                            //    double watch = (double)m_StopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                            //    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                            //    System.Diagnostics.Debug.WriteLine($"{time} : [SeqSearchLink] [{watch}]");
                            //}
                        }
                        break;

                    case 1000:
                        {
                            if (IsPushedSwitch.IsAlarmReset)
                            {
                                IsPushedSwitch.m_AlarmRstPushed = false;
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
            public void CheckCurrentPort(double leftBCR, double rightBCR)
            {
                try
                {
                    // Current Link가 Null이 아닌 경우 해당 Link 위의 Port List를 만들자
                    // Port List 중 Current BCR과 가장 가까운 놈을 찾자
                    // 해당 사항없으면 0으로 만든다.
                    lock (DatabaseHandler.Instance.DictionaryPortDataList)
                    {
                        int link_id = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.LinkID;
                        int node_id = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.FromNodeID;
                        int port_id = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
                        int source_port = m_ProcessDataHandler.CurTransferCommand.SourceID;
                        int destinate_port = m_ProcessDataHandler.CurTransferCommand.DestinationID;
                        if (link_id != 0 && DatabaseHandler.Instance.DictionaryPortDataList != null && DatabaseHandler.Instance.DictionaryPortDataList.Count > 0)
                        {
                            List<DataItem_Port> portsOnLink = DatabaseHandler.Instance.DictionaryPortDataList.Values.Where(x => x.LinkID == link_id).ToList();
                            if (portsOnLink != null && portsOnLink.Count > 0)
                            {
                                List<DataItem_Port> ports = new List<DataItem_Port>();
                                List<DataItem_Port> leftPorts = portsOnLink.Where(x => x.PortType == PortType.LeftEQPort ||
                                                                                            x.PortType == PortType.TeachingStation ||
                                                                                            x.PortType == PortType.LeftSTKPort ||
                                                                                            x.PortType == PortType.LeftBuffer ||
                                                                                            x.PortType == PortType.LeftTeachingStation).ToList();
                                ports.AddRange(leftPorts);
                                List<DataItem_Port> rightPorts = portsOnLink.Where(x => x.PortType == PortType.RightEQPort ||
                                                                                            x.PortType == PortType.RightSTKPort ||
                                                                                            x.PortType == PortType.RightBuffer ||
                                                                                            x.PortType == PortType.RightTeachingStation).ToList();
                                ports.AddRange(rightPorts);
                                double InRange = SetupManager.Instance.SetupWheel.InRangePortFindPosition;
                                List<DataItem_Port> temp_ports = new List<DataItem_Port>();
                                if (ports != null && ports.Count > 0)
                                {
                                    temp_ports.AddRange(ports.Where(item => Math.Abs(item.BarcodeLeft + item.DriveLeftOffset - leftBCR) < InRange));
                                    temp_ports.AddRange(ports.Where(item => Math.Abs(item.BarcodeRight + item.DriveRightOffset - rightBCR) < InRange));
                                }

                                if (temp_ports.Count == 1)
                                {
                                    DataItem_Port find_port = temp_ports.FirstOrDefault();
                                    if (port_id != find_port.PortID && (source_port == find_port.PortID || destinate_port == find_port.PortID))
                                        SequenceLog.WriteLog(FuncName, $"Current Port Find#1 ({port_id}->{find_port.PortID}) LINK:{m_ProcessDataHandler.CurVehicleStatus.CurrentPath})");

                                    ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort = ObjectCopier.Clone(find_port);
                                }
                                else if (temp_ports.Count > 1)
                                {
                                    int target_port = -1;
                                    List<DataItem_Port> selected_ports = temp_ports.OrderBy(item => Math.Sqrt(Math.Pow(item.BarcodeLeft + item.DriveLeftOffset - leftBCR, 2) + Math.Pow(item.BarcodeRight + item.DriveRightOffset - rightBCR, 2))).ToList();
                                    //if (ProcessDataHandler.Instance.CurTransferCommand.IsValid)
                                    if (ProcessDataHandler.Instance.CurTransferCommand.DestinationID != 0 && selected_ports.Count > 0)
                                    {
                                        bool acquire = ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.EnRouteToSource;
                                        acquire |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Acquiring;
                                        acquire |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.AcquireCompleted;
                                        bool deposit = ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.EnRouteToDest;
                                        deposit |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.Depositing;
                                        deposit |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentVehicleState == VehicleState.DepositeCompleted;
                                        deposit |= ProcessDataHandler.Instance.CurTransferCommand.ProcessCommand == OCSCommand.Go;
                                        deposit |= ProcessDataHandler.Instance.CurTransferCommand.ProcessCommand == OCSCommand.Teaching;
                                        target_port = deposit ? ProcessDataHandler.Instance.CurTransferCommand.DestinationID :
                                            acquire ? ProcessDataHandler.Instance.CurTransferCommand.SourceID : -1;

                                        if (target_port != -1)
                                        {
                                            if (selected_ports.Select(x => x.PortID).Contains(target_port))
                                            {
                                                DataItem_Port find_port = selected_ports.Find(x => x.PortID == target_port);
                                                if (port_id != find_port.PortID && (source_port == find_port.PortID || destinate_port == find_port.PortID))
                                                    SequenceLog.WriteLog(FuncName, $"Current Port Find#2 ({port_id}->{find_port.PortID}) LINK:{m_ProcessDataHandler.CurVehicleStatus.CurrentPath})");
                                                ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort = ObjectCopier.Clone(find_port);
                                            }
                                            else
                                            {
                                                target_port = -1;
                                            }
                                        }
                                    }

                                    if (target_port == -1 && selected_ports.Count > 0)
                                    {
                                        DataItem_Port find_port = selected_ports.FirstOrDefault();
                                        if (port_id != selected_ports.FirstOrDefault().PortID && (source_port == find_port.PortID || destinate_port == find_port.PortID))
                                            SequenceLog.WriteLog(FuncName, $"Current Port Find#3 ({port_id}->{find_port.PortID}) LINK:{m_ProcessDataHandler.CurVehicleStatus.CurrentPath})");
                                        ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort = ObjectCopier.Clone(find_port);
                                    }
                                }
                                else if (temp_ports.Count == 0)
                                {
                                    if (port_id != 0 && (source_port == port_id || destinate_port == port_id))
                                        SequenceLog.WriteLog(FuncName, $"Current Port Find (NG) ({port_id}->0) LINK:{m_ProcessDataHandler.CurVehicleStatus.CurrentPath})");

                                    ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID = 0;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
                }
            }

            public void CheckCurrentPositionOfLink(Sineva.VHL.Data.Process.Path curPath, double leftBCR, double rightBCR, double search_range, double diff_range)
            {
                try
                {
                    // Link Vehicle 위치값을 계산하자
                    LinkType linkType = curPath.Type;
                    enBcrCheckDirection bcrDirection = curPath.BcrDirection;
                    double leftBeginBcr = curPath.LeftBCRBegin;
                    double rightBeginBcr = curPath.RightBCRBegin;
                    double leftStartBcr = curPath.LeftBCRStart;
                    double rightStartBcr = curPath.RightBCRStart;
                    // Start 위치값은 RunPathMaps를 사용하자 ! ==> ???? (RunPathMaps의 값은 중간에 멈출때마다 바뀜...
                    //Sineva.VHL.Data.Process.Path curRunPath = m_ProcessDataHandler.CurTransferCommand.RunPathMaps.Find(x => x.LinkID == curPath.LinkID);
                    //if (curRunPath != null)
                    //{
                    //    leftStartBcr = curRunPath.LeftBCRStart;
                    //    rightStartBcr = curRunPath.RightBCRStart;
                    //}
                    double linkDistance = curPath.Distance;
                    bool bcr_valid_check = true;
                    if (curPath.BcrDirection == enBcrCheckDirection.Left || curPath.BcrDirection == enBcrCheckDirection.Both)
                    {
                        bcr_valid_check &= leftBCR > leftBeginBcr;
                        bcr_valid_check &= leftBCR < leftBeginBcr + linkDistance;
                    }
                    if (curPath.BcrDirection == enBcrCheckDirection.Right || curPath.BcrDirection == enBcrCheckDirection.Both)
                    {
                        bcr_valid_check &= rightBCR > rightBeginBcr;
                        bcr_valid_check &= rightBCR < rightBeginBcr + linkDistance;
                    }
                    if (curPath.BcrDirection == enBcrCheckDirection.Both)
                    {
                        bcr_valid_check &= linkType == LinkType.Straight ? Math.Abs((rightBCR - leftBCR) - (rightBeginBcr - leftBeginBcr)) < diff_range : true; // 직선인 경우에만 확인 해야 할 것 같음.
                    }
                    if (bcr_valid_check)
                    {
                        bool sCurveAfterArea = false;
                        double sCurveBeforeStartDistance = 0.0f;
                        double sCurveBeforeBeginDistance = 0.0f;
                        if ((curPath.Type == LinkType.LeftSBranch && curPath.SteerChangeLeftBCR != 0.0f) ||
                            (curPath.Type == LinkType.RightSBranch && curPath.SteerChangeRightBCR != 0.0f))
                        {
                            //S-Curve... Nothing Both
                            bool SCurvePos = false;
                            if (curPath.BcrDirection == enBcrCheckDirection.Left)
                                SCurvePos |= leftBCR > curPath.SteerChangeLeftBCR;
                            else if (curPath.BcrDirection == enBcrCheckDirection.Right)
                                SCurvePos |= rightBCR > curPath.SteerChangeRightBCR;
                            else
                            {
                                SCurvePos |= leftBCR > curPath.SteerChangeLeftBCR;
                                SCurvePos |= rightBCR > curPath.SteerChangeRightBCR;
                            }

                            if (SCurvePos)
                            {
                                if (bcrDirection == enBcrCheckDirection.Left)
                                {
                                    sCurveBeforeStartDistance = curPath.SteerChangeLeftBCR - curPath.LeftBCRStart;
                                    sCurveBeforeBeginDistance = curPath.SteerChangeLeftBCR - curPath.LeftBCRBegin;
                                    bcrDirection = enBcrCheckDirection.Right;
                                }
                                else
                                {
                                    sCurveBeforeStartDistance = curPath.SteerChangeRightBCR - curPath.RightBCRStart;
                                    sCurveBeforeBeginDistance = curPath.SteerChangeRightBCR - curPath.RightBCRBegin;
                                    bcrDirection = enBcrCheckDirection.Left;
                                }
                                sCurveAfterArea = true;

                                leftBeginBcr = curPath.SteerChangeLeftBCR;
                                rightBeginBcr = curPath.SteerChangeRightBCR;
                                leftStartBcr = curPath.SteerChangeLeftBCR;
                                rightStartBcr = curPath.SteerChangeRightBCR;
                            }
                        }

                        double curPos = m_MasterAxis.GetCurPosition();
                        double cur_bcr_position = bcrDirection == enBcrCheckDirection.Right ? rightBCR : leftBCR;
                        if (m_OldLinkID == 0) m_OldLinkID = curPath.LinkID;
                        if (m_OldCurPosition == 0.0f) m_OldCurPosition = curPos;
                        if (m_OldBcrPosition == 0.0f) m_OldBcrPosition = cur_bcr_position;

                        if (m_OldLinkID == curPath.LinkID)
                        {
                            double motor_change = curPos - m_OldCurPosition;
                            double bcr_change = cur_bcr_position - m_OldBcrPosition;
                            if (Math.Abs(bcr_change - motor_change) < search_range)
                            {
                                double link_start_bcr = bcrDirection == enBcrCheckDirection.Right ? rightStartBcr : leftStartBcr;
                                double link_begin_bcr = bcrDirection == enBcrCheckDirection.Right ? rightBeginBcr : leftBeginBcr;
                                double curRunPositionOfLink = cur_bcr_position - link_start_bcr + sCurveBeforeStartDistance;
                                double curPositionOfLink = cur_bcr_position - link_begin_bcr + sCurveBeforeBeginDistance;

                                curPath.RunPositionOfLink = curRunPositionOfLink;
                                curPath.CurrentPositionOfLink = curPositionOfLink;
                                double remain_distance = curPath.Distance - curPositionOfLink;
                                curPath.RemainDistanceOfLink = remain_distance;
                            }
                        }
                        m_OldCurPosition = curPos;
                        m_OldBcrPosition = cur_bcr_position;
                    }
                    m_OldLinkID = curPath.LinkID;
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }

            public void CheckInPosition(enBcrCheckDirection dir, double left, double right)
            {
                try
                {
                    bool near_position = false;
                    bool in_position = false;
                    //if (m_ProcessDataHandler.CurTransferCommand.IsValid)
                    {
                        if (m_ProcessDataHandler.CurTransferCommand.TargetNode == m_ProcessDataHandler.CurVehicleStatus.CurrentPath.ToNodeID ||
                            m_ProcessDataHandler.CurTransferCommand.TargetNode == m_ProcessDataHandler.CurVehicleStatus.CurrentPath.FromNodeID)
                        {
                            near_position = m_ProcessDataHandler.CurTransferCommand.RemainBcrDistance < m_ProcessDataHandler.CurTransferCommand.StopCheckDistance;
                            if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count > 0)
                                near_position |= m_ProcessDataHandler.CurTransferCommand.PathMaps.Last().IsSame(m_ProcessDataHandler.CurVehicleStatus.CurrentPath);

                            double targetLeftBcr = m_ProcessDataHandler.CurTransferCommand.TargetLeftBcrPosition;
                            double targetRightBcr = m_ProcessDataHandler.CurTransferCommand.TargetRightBcrPosition;
                            double inRange = DevicesManager.Instance.DevTransfer.GetInRange();
                            double diff = (dir == enBcrCheckDirection.Right) ? targetRightBcr - right : targetLeftBcr - left;
                            if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count == 1)
                            {
                                bool motion_run = false;
                                motion_run = m_ProcessDataHandler.CurTransferCommand.PathMaps.Last().MotionRun;
                                if (motion_run) in_position = Math.Abs(diff) < inRange; // link가 1개일때는 motion run이후 Monitoring
                            }
                            else in_position = Math.Abs(diff) < inRange;

                            if (near_position)
                                m_ProcessDataHandler.CurTransferCommand.RemainBcrDistance = diff;
                        }

                        // Last 목표 위치에 도달 했으면 Finished 처리하자 !
                        ///////Path Process 상태를 Update 해라//////////////////////////////////////////////////////////////////////////////////
                        Sineva.VHL.Data.Process.Path myPath = m_ProcessDataHandler.CurVehicleStatus.CurrentPath;
                        bool finished = in_position;
                        if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count > 0)
                        {
                            finished &= myPath.LinkID == m_ProcessDataHandler.CurTransferCommand.PathMaps.Last().LinkID;
                            finished &= myPath.Index == m_ProcessDataHandler.CurTransferCommand.PathMaps.Last().Index; // 한바퀴 돌아오는 경우 처음과 끝 구분 필요
                            finished &= m_ProcessDataHandler.CurTransferCommand.PathMaps.Last().MotionProc == enMotionProc.inProc;
                        }
                        if (finished) m_ProcessDataHandler.CurTransferCommand.UpdateMotionProcess(myPath, false);
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    m_ProcessDataHandler.CurVehicleStatus.IsNearPosition = near_position;
                    m_ProcessDataHandler.CurVehicleStatus.IsInPosition = in_position;
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
            }
            #endregion
        }

    }
}
