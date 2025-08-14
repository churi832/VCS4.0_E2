using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Data.Process;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Device;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Common;
using Sineva.VHL.Library.Servo;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;
using Sineva.VHL.Library.SimpleWifi;

namespace Sineva.VHL.Task
{
    public class TaskMonitor : XSequence
    {
        public static readonly TaskMonitor Instance = new TaskMonitor();
        public TaskMonitor()
        {
			this.RegSeq(new SeqHomeSensorManage());
            this.RegSeq(new SeqUpdateTargetPosition());
            this.RegSeq(new SeqVirtualBcr());
            this.RegSeq(new SeqSilentStopMonitor());
            this.RegSeq(new SeqTpdManage());
        }
        public class SeqHomeSensorManage : XSeqFunc
        {
            #region Enum
            [Serializable()]
            public enum enHomeSensor : int
            {
                FoupExist,
                OldState,
                NewState,
                CurPos,
            }
            #endregion
            #region Fields
            private const string FuncName = "[SeqHomeSensorManage]";
            private HomeSensorLog m_HomeLogger = null;
            private Dictionary<enHomeSensor, string> m_HomeSensor = new Dictionary<enHomeSensor, string>();
            private _DevAxis m_SlideAxis = null;
            private _DevAxis m_HoistAxis = null;
            private _DevAxis m_RotateAxis = null;

            private DevGripperPIO m_devGripperPio = null;

            private bool m_OldSlideSensor = false;
            private bool m_OldHoistSensor = false;
            private bool m_OldRotateSensor = false;
            #endregion
            public SeqHomeSensorManage()
            {
                this.SeqName = $"SeqHomeSensorManage";

                #region LOG Title
                foreach (enHomeSensor home in Enum.GetValues(typeof(enHomeSensor)))
                {
                    m_HomeSensor.Add(home, "");
                }
                #endregion
                string[] home_names = Enum.GetNames(typeof(enHomeSensor));
                m_HomeLogger = new HomeSensorLog(home_names);
                m_SlideAxis = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis();
                m_HoistAxis = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis();
                m_RotateAxis = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis();
                m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            }
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
                            try
                            {
                                double SlideCurPos = m_SlideAxis != null ? m_SlideAxis.GetCurPosition() : 0.0f;
                                bool homeslide = m_SlideAxis != null ? m_SlideAxis.IsHomeSensor() : false;
                                double HoistCurPos = m_HoistAxis != null ? m_HoistAxis.GetCurPosition() : 0.0f;
                                bool homehoist = m_HoistAxis != null ? m_HoistAxis.IsHomeSensor() : false;
                                double RotateCurPos = m_RotateAxis != null ? m_RotateAxis.GetCurPosition() : 0.0f;
                                bool homerotate = m_RotateAxis != null ? m_RotateAxis.IsHomeSensor() : false;

                                if (homeslide != m_OldSlideSensor)
                                {
                                    m_HomeSensor[enHomeSensor.FoupExist] = IsFoupExist() ? "FoupExist" : "NoFoup";
                                    m_HomeSensor[enHomeSensor.OldState] = m_OldSlideSensor.ToString();
                                    m_HomeSensor[enHomeSensor.NewState] = homeslide.ToString();
                                    m_HomeSensor[enHomeSensor.CurPos] = SlideCurPos.ToString("F2");

                                    m_HomeLogger.WriteSlideLog(m_HomeSensor.Values.ToArray());
                                    m_OldSlideSensor = homeslide;
                                }

                                if (homehoist != m_OldHoistSensor)
                                {
                                    m_HomeSensor[enHomeSensor.FoupExist] = IsFoupExist() ? "FoupExist" : "NoFoup";
                                    m_HomeSensor[enHomeSensor.OldState] = m_OldHoistSensor.ToString();
                                    m_HomeSensor[enHomeSensor.NewState] = homehoist.ToString();
                                    m_HomeSensor[enHomeSensor.CurPos] = HoistCurPos.ToString("F2");

                                    m_HomeLogger.WriteHoistLog(m_HomeSensor.Values.ToArray());
                                    if (homehoist && !m_OldHoistSensor)
                                    {
                                        double offset = 0.0f;
                                        double home_offset = 0.0f;
                                        if (m_HoistAxis != null)
                                        {
                                            offset = Math.Round(m_HoistAxis.GetAxis().HomeOffset + HoistCurPos, 2);
                                            home_offset = m_HoistAxis.GetAxis().HomeOffset;
                                        }
                                        DevicesManager.Instance.DevFoupGripper.Offset_HoistHomeSensorDetect = offset;
                                        SequenceLog.WriteLog(FuncName, string.Format("[Update] Set Transfer Hoist Offset - Hoist Home Offset Error : {0}, Hoist Home Offset : {1}, Sensor Detected Pos : {2}", offset, home_offset, HoistCurPos));
                                        //저장이 필요하면 DevicesManager.Instance.DevFoupGripper.WriteXml()
                                    }
                                    m_OldHoistSensor = homehoist;
                                }

                                if (homerotate != m_OldRotateSensor)
                                {
                                    m_HomeSensor[enHomeSensor.FoupExist] = IsFoupExist() ? "FoupExist" : "NoFoup";
                                    m_HomeSensor[enHomeSensor.OldState] = m_OldRotateSensor.ToString();
                                    m_HomeSensor[enHomeSensor.NewState] = homerotate.ToString();
                                    m_HomeSensor[enHomeSensor.CurPos] = RotateCurPos.ToString("F2");

                                    m_HomeLogger.WriteRotateLog(m_HomeSensor.Values.ToArray());
                                    m_OldRotateSensor = homerotate;
                                }
                                seqNo = 0;
                            }
                            catch (Exception e)
                            {
                                ExceptionLog.WriteLog(e.ToString());
                            }
                            finally
                            {
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return -1;
            }

            private bool IsFoupExist()
            {
                bool foup_exist = true;
                try
                {
                    if (m_devGripperPio.IsValid)
                    {
                        foup_exist &= m_devGripperPio.IsProductExist();
                        foup_exist &= m_devGripperPio.DiGripperClose.IsDetected;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return foup_exist;
            }
        }
        public class SeqUpdateTargetPosition : XSeqFunc
        {
            private const string FuncName = "[SeqUpdateTargetPosition]";

            #region Fields
            private ProcessDataHandler m_ProcessDataHandler = null;
            private DatabaseHandler m_DatabaseHandler = null;
            private _DevAxis m_MasterAxis = null;

            private bool m_UpdateTargetPosition = false;
            private Data.Process.Path m_MyPath = new Data.Process.Path();
            private bool m_MotorEncorderAbnormal = false;
            private double m_MotorLinkStartPosition = 0.0f;
            #endregion
            #region Contructor
            public SeqUpdateTargetPosition()
            {
                this.SeqName = $"SeqUpdateTargetPosition";

                m_ProcessDataHandler = ProcessDataHandler.Instance;
                m_DatabaseHandler = DatabaseHandler.Instance;
                m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();

                //EventHandlerManager.Instance.LinkChanged += Instance_LinkChanged;
                EventHandlerManager.Instance.UpdateTargetPosition += Instance_UpdateTargetPosition;
            }

            private void Instance_UpdateTargetPosition(int a)
            {
                int axisId = a;
                if (axisId == m_MasterAxis.GetAxis().AxisId)
                {
                    m_ProcessDataHandler.CurTransferCommand.TargetMotorPosition = m_MasterAxis.GetAxis().TargetPos;

                    double curPos = m_MasterAxis.GetCurPosition();
                    if (Math.Abs(curPos) < 1.0) m_MotorLinkStartPosition = -1 * ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink;
                    else m_MotorLinkStartPosition = curPos;
                }
            }

            private void Instance_LinkChanged(object obj)
            {
                m_MyPath = (Data.Process.Path)obj;
                if (m_MyPath.Type == LinkType.Straight)
                {
                    m_UpdateTargetPosition = true;
                    if (m_MotorEncorderAbnormal)
                    {
                        double curVel = m_ProcessDataHandler.CurVehicleStatus.MasterWheelVelocity;
                        if (curVel > 2.0f)
                        {
                            // 속도가 없는 경우는 시작 시점이라서 여기서 Reschedule 하라고 할필요 없다.
                            m_ProcessDataHandler.CurVehicleStatus.DistanceErrorBCREncorder = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Motor Encorder Abnormal .. Restart Move !"));
                        }
                    }
                }
            }
            #endregion

            #region Methods
            public override void SeqAbort()
            {
            }
            public override int Do()
            {
                double curPos = m_MasterAxis.GetCurPosition();

                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (m_MyPath.LinkID != m_ProcessDataHandler.CurVehicleStatus.CurrentPath.LinkID ||
                    			m_MyPath.ToLinkID != ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToLinkID)
                            {
                                if (Math.Abs(curPos) < 1.0) m_MotorLinkStartPosition = -1 * ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink;
                                else m_MotorLinkStartPosition = curPos;
                                
                                m_MyPath = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath;
                                m_UpdateTargetPosition = m_ProcessDataHandler.CurVehicleStatus.CurrentPath.Type == LinkType.Straight;
                                if (m_MotorEncorderAbnormal)
                                {
                                    double curVel = m_ProcessDataHandler.CurVehicleStatus.MasterWheelVelocity;
                                    if (curVel > 2.0f)
                                    {
                                        // 속도가 없는 경우는 시작 시점이라서 여기서 Reschedule 하라고 할필요 없다.
                                        m_ProcessDataHandler.CurVehicleStatus.DistanceErrorBCREncorder = true;
                                        SequenceLog.WriteLog(FuncName, string.Format("Motor Encorder Abnormal .. Restart Move !"));
                                    }
                                }
                            }
                            seqNo = 10;
                        }
                        break;

                    case 10:
                        {
                            if (m_UpdateTargetPosition)
                            {
                                //Update Item////////////////////////////////////////////
                                //m_TargetMotorPosition
                                //m_RemainMotorDistance
                                //m_TargetBcrDistance
                                //m_CurrentMotorPositionOfLink
                                //////////////////////////////////////////////
                                m_UpdateTargetPosition = false;
                                if (m_ProcessDataHandler.CurTransferCommand.IsValid && !m_MasterAxis.GetAxis().SequenceState.IsExternalEncoderRun)
                                {
                                    if (m_ProcessDataHandler.CurTransferCommand.RunPathMaps.Count > 0)
                                    {
                                        int myLinkId = m_MyPath.LinkID;
                                        Data.Process.Path myPath = m_ProcessDataHandler.CurTransferCommand.RunPathMaps.Find(x=>x.LinkID == myLinkId && x.MotionProc == enMotionProc.inProc);
                                        if (myPath != null)
                                        {
                                            double targetBCR = myPath.BcrDirection == enBcrCheckDirection.Right ? myPath.RightBCRTarget : myPath.LeftBCRTarget;
                                            double curBCR = myPath.BcrDirection == enBcrCheckDirection.Right ? 
                                                m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.RightBcr : m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                                            double myPathRunDistance = targetBCR - curBCR;
                                            double total_distance = m_ProcessDataHandler.CurTransferCommand.RunPathMaps.Last().TotalDistance;
                                            //double remain_distance = total_distance - (myPath.TotalDistance - myPath.RunDistance);
                                            double remain_distance = total_distance - (myPath.TotalDistance - myPathRunDistance); // SetCommand -> SequenceMove 시간차로 인한 오차가 발생하고 있다.
                                            m_ProcessDataHandler.CurTransferCommand.TargetBcrDistance = remain_distance;
                                            m_ProcessDataHandler.CurTransferCommand.TargetMotorPosition = curPos + remain_distance;
                                            DevicesManager.Instance.DevTransfer.UpdateTargetPosition(m_ProcessDataHandler.CurTransferCommand.TargetMotorPosition);
                                        }
                                    }
                                }
                            }
                            m_ProcessDataHandler.CurTransferCommand.RemainMotorDistance = m_ProcessDataHandler.CurTransferCommand.TargetMotorPosition - curPos;
                            m_ProcessDataHandler.CurVehicleStatus.CurrentPath.CurrentMotorPositionOfLink = curPos - m_MotorLinkStartPosition;

                            // BCR이동 거리와 Motor Encorder 이동 거리 차이가 심할 경우 SequenceMove 재 지령 필요 할 것 같다. /////////////////////
                            double bcr_cur_position = m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr;
                            double motor_cur_position = DevicesManager.Instance.DevTransfer.MotionRunPosition;
                            // 문제가 있다 다시 Schedule 해라 ~~~
                            if (Math.Abs(bcr_cur_position - motor_cur_position) > SetupManager.Instance.SetupWheel.RescheduleDistanceBCREncorder)
                            {
								if(!m_MotorEncorderAbnormal)
									SequenceLog.WriteLog(FuncName, string.Format("Current BCR Position : {0}, Current Motor Encoder Position : {1}, RescheduleDistance : {2}", bcr_cur_position, motor_cur_position, SetupManager.Instance.SetupWheel.RescheduleDistanceBCREncorder));
                                m_MotorEncorderAbnormal = true;                                
                            }
                            else
                            {
                                m_MotorEncorderAbnormal = false;
                                m_ProcessDataHandler.CurVehicleStatus.DistanceErrorBCREncorder = false;
                            }
                            seqNo = 0;
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
        public class SeqVirtualBcr : XSeqFunc
        {
            private const string FuncName = "[SeqVirtualBcr]";

            #region Fields
            private ProcessDataHandler m_ProcessDataHandler = null;
            private DatabaseHandler m_DatabaseHandler = null;
            private _DevAxis m_MasterAxis = null;

            private double m_VirtualLeftBcr = 0.0f;
            private double m_VirtualRightBcr = 0.0f;
            #endregion

            #region Constructor
            public SeqVirtualBcr()
            {
                this.SeqName = $"SeqVirtualBcr";

                m_ProcessDataHandler = ProcessDataHandler.Instance;
                m_DatabaseHandler = DatabaseHandler.Instance;
                m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
            }
            #endregion

            #region Methods
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
                            //Update Item////////////////////////////////////////////
                            //m_RemainBcrDistance
                            //m_VirtualBcr
                            //////////////////////////////////////////////
                            if (m_ProcessDataHandler.CurTransferCommand.IsValid)
                            {
                                if (AppConfig.Instance.Simulation.MY_DEBUG)
                                {
                                    // Generate BCR
                                    bool rv = UpdateVirtualBcr();
                                    if (rv)
                                    {
                                        m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.LeftBcr = m_VirtualLeftBcr;
                                        m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.RightBcr = m_VirtualRightBcr;
                                    }
                                }

                                if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count > 0)
                                {
                                    Data.Process.Path myPath = m_ProcessDataHandler.CurVehicleStatus.CurrentPath;
                                    double virtual_bcr = (myPath.TotalDistance - myPath.RunDistance) + myPath.RunPositionOfLink; 
                                    m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.VirtualBcr = virtual_bcr;
                                    double target_bcr = m_ProcessDataHandler.CurTransferCommand.TargetBcrDistance;
                                    double current_bcr = m_ProcessDataHandler.CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr;
                                    double remain_bcr = target_bcr - current_bcr; // TargetBcrDistance를 update 하면서 SetVirtualStartBcr()을 해야 위치 Sync가 된다.
                                    if (!m_ProcessDataHandler.CurVehicleStatus.IsNearPosition) // near ==> real bcr use
                                        m_ProcessDataHandler.CurTransferCommand.RemainBcrDistance = remain_bcr;
                                }
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return -1;
            }

            public bool UpdateVirtualBcr()
            {
                bool rv = false;
                try
                {
                    if (m_ProcessDataHandler.CurTransferCommand.IsNeedMakeRouteFullPath) return false; // PathMaps을 새로 생성하는 시점에 꼬일수 있음.
                    
                    if (m_ProcessDataHandler.CurTransferCommand.PathMaps.Count > 0)
                    {
                        double curPos = m_MasterAxis.GetCurPosition();
                        List<Data.Process.Path> paths = m_ProcessDataHandler.CurTransferCommand.PathMaps.ToList();
                        if (paths.Count > 0)
                        {
                            Data.Process.Path path = paths.Find(x => x.TotalDistance > curPos && x.MotionProc != enMotionProc.finished);
                            if (path == null) path = paths.Last();
                            if (path != null)
                            {
                                double linkPos = curPos - (path.TotalDistance - path.RunDistance);
                                if (linkPos >= 0)
                                {
                                    m_VirtualLeftBcr = path.LeftBCRStart + linkPos;
                                    m_VirtualRightBcr = path.RightBCRStart + linkPos;
                                    rv = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    rv = false;
                    ExceptionLog.WriteLog(ex.ToString());
                }
                return rv;
            }
            #endregion
        }
        public class SeqSilentStopMonitor : XSeqFunc
        {
            private const string FuncName = "[SeqSilentStopMonitor]";

            #region Fields
            private _DevAxis m_MasterAxis = null;
            private _DevAxis m_SlaveAxis = null;
            private _DevAxis m_SlideAxis = null;
            private _DevAxis m_HoistAxis = null;
            private _DevAxis m_RotateAxis = null;

            private VehicleStatus curVehicleStatus = null;

            private AlarmData m_ALM_SilentStop = null;
            #endregion

            #region Constructor
            public SeqSilentStopMonitor()
            {
                this.SeqName = $"SeqSilentStopMonitor";

                m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
                m_SlaveAxis = DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis();
                m_SlideAxis = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis();
                m_HoistAxis = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis();
                m_RotateAxis = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis();

                curVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;

                m_ALM_SilentStop = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, false, "SeqSilentStopMonitor", "Vehicle Not Response", "Abnormal Stop Warning");

                StartTicks = XFunc.GetTickCount();
            }
            #endregion

            #region Methods
            public override void SeqAbort()
            {
                if (AlarmId > 0)
                {
                    EqpAlarm.Reset(AlarmId);
                    AlarmId = 0;
                }
                InitSeq();
                StartTicks = XFunc.GetTickCount();
            }
            public override int Do()
            {
                if (SetupManager.Instance.SetupOperation.SlientStopAlarmUse == Use.NoUse) return -1;
                
                int seqNo = this.SeqNo;
                switch (seqNo)
                {

                    // * 무언정지 조건 AND조건
                    // 1. 명령이 존재한다
                    // 2. Override가 0이 아니다.
                    // 3. Alarm이 없다.
                    // 4. Auto RUN 상태이다.
                    // 5. 속도가 2mm/sec 이내이다.
                    // 6. Wheel Busy가 false이다.
                    // 7. Hoist/Slide가 ActualPos가 10mm 이내이다.
                    // 8. 120초를 유지했다.
                    // 9. JCS Pass Request를 했는데 Received Permit이 아니면 무언정지 아니다.
                    case 0:
                        {
                            if (XFunc.GetTickCount() - StartTicks < 1000) break;

                            bool SlientStop = true;
                            SlientStop &= ProcessDataHandler.Instance.CurTransferCommand.IsValid;
                            SlientStop &= ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideRatio > 0.0f;
                            SlientStop &= EqpAlarm.EqpAlarmItems.Count == 0;
                            SlientStop &= Math.Abs(m_MasterAxis.GetCurVelocity()) < 3.0f;
                            SlientStop &= Math.Abs(m_SlaveAxis.GetCurVelocity()) < 3.0f;
                            SlientStop &= Math.Abs(m_SlideAxis.GetCurVelocity()) < 3.0f;
                            SlientStop &= Math.Abs(m_HoistAxis.GetCurVelocity()) < 3.0f;
                            SlientStop &= Math.Abs(m_RotateAxis.GetCurVelocity()) < 3.0f;
                            SlientStop &= Math.Abs(m_SlideAxis.GetCurPosition()) < 10.0f;
                            SlientStop &= Math.Abs(m_HoistAxis.GetCurPosition()) < 10.0f;
                            SlientStop &= Math.Abs(m_RotateAxis.GetCurPosition()) < 1.0f;
                            SlientStop &= GV.WheelBusy == false;
                            SlientStop &= EqpStateManager.Instance.OpMode == OperateMode.Auto;
                            SlientStop &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
                            if (TaskJCS.Instance.JcsControl.PassPermitCheck)
                            {
                                if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                                    SlientStop &= TaskJCS.Instance.JcsControl.PassRequest == true; //E2고객 요청으로 JCS Permit을 받지 못해도 무언정지처리..
                                else
                                    SlientStop &= TaskJCS.Instance.JcsControl.PassRequest ? TaskJCS.Instance.JcsControl.ReceivedPermit : true;
                            }
                            //MTL 구역에서는 Silent Alarm 띄우지 말자.. 왜냐면 MTL 안에 다른 Vehicle 있고 내려가 있으면 사람이 뺄 때까지 계속 그상태 유지 할거니까..
                            double bcrLeft = curVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double bcrRight = curVehicleStatus.CurrentBcrStatus.RightBcr;
                            DataItem_Node curNode = curVehicleStatus.CurrentPath.IsNearNode(bcrLeft, bcrRight);
                            if (curNode != null && (curNode.Type == NodeType.MTL || curNode.Type == NodeType.MTLIn))
                                SlientStop &= false;
                            SlientStop &= DevicesManager.Instance.DevEqpPIO.IsPioRun() ? false : true;
                            if (SlientStop)
                            {
                                SlientStop &= AlarmCurrentProvider.Instance.IsHeavyAlarm() == false;
                                SlientStop &= AlarmCurrentProvider.Instance.IsWarningAlarm() == false; // OCS, JCS Disconnect Alarm, JCS Pass Permit Wait Alarm <- Warning 단계임..
                            }
                            if (SlientStop)
                            {
                                int wait_time = SetupManager.Instance.SetupOperation.SilentStopWaitTime * 60 * 1000;
                                if (XFunc.GetTickCount() - StartTicks > wait_time) //2분 유지하면 Slient Stop Alarm 띄우자
                                {
                                    SequenceLog.WriteLog(FuncName, string.Format("Vehicle Current State is Silent Stop !! Check SW"));
                                    int rv1 = m_ALM_SilentStop.ID;
                                    EqpAlarm.Set(rv1);
                                    seqNo = 10;
                                }
                            }
                            else
                            {
                                StartTicks = XFunc.GetTickCount();
                            }
                        }
                        break;

                    // * Alarm 해제 조건 OR조건
                    // 1. 속도가 2mm/sec 이상이거나.
                    // 2. 명령이 삭제 되거나
                    // 3. Manual로 전환되거나
                    // 4. Wheel Busy가 true거나
                    case 10:
                        {
                            bool ResetSilent = false;
                            ResetSilent |= Math.Abs(m_MasterAxis.GetCurVelocity()) >= 3.0f;
                            ResetSilent |= Math.Abs(m_SlaveAxis.GetCurVelocity()) >= 3.0f;
                            ResetSilent |= Math.Abs(m_SlideAxis.GetCurVelocity()) >= 3.0f;
                            ResetSilent |= Math.Abs(m_HoistAxis.GetCurVelocity()) >= 3.0f;
                            ResetSilent |= Math.Abs(m_RotateAxis.GetCurVelocity()) >= 3.0f;
                            ResetSilent |= GV.WheelBusy;
                            ResetSilent |= ProcessDataHandler.Instance.CurTransferCommand.IsValid == false;
                            ResetSilent |= EqpStateManager.Instance.OpMode != OperateMode.Auto;
                            ResetSilent |= EqpStateManager.Instance.RunMode != EqpRunMode.Start;
                            if(ResetSilent)
                            {
                                SequenceLog.WriteLog(FuncName, string.Format("It's back to normal from a silent stop."));
                                int rv1 = m_ALM_SilentStop.ID;
                                EqpAlarm.Reset(rv1);

                                StartTicks = XFunc.GetTickCount();
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return -1;
            }

            #endregion
        }
        public class SeqTpdManage : XSeqFunc
        {
            #region Fields
            private const string FuncName = "[SeqTpdManage]";
            private TpdLog m_TpdLogger = null;
            private Dictionary<enTPD, string> m_Tpd = new Dictionary<enTPD, string>();
            private CpuUsage m_CpuUsage = new CpuUsage();
            private _DevAxis m_MasterAxis = null;
            private _DevAxis m_SlaveAxis = null;
            private _DevAxis m_SlideAxis = null;
            private _DevAxis m_HoistAxis = null;
            private _DevAxis m_RotateAxis = null;

            private System.Diagnostics.Stopwatch m_StopWatch = new System.Diagnostics.Stopwatch();
            #endregion
            public SeqTpdManage()
            {
                this.SeqName = $"SeqTpdManage";

                string[] tpd_names = Enum.GetNames(typeof(enTPD));
                m_TpdLogger = new TpdLog(tpd_names);
                m_Tpd = ProcessDataHandler.Instance.Tpd;
                m_MasterAxis = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis();
                m_SlaveAxis = DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis();
                m_SlideAxis = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis();
                m_HoistAxis = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis();
                m_RotateAxis = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis();
            }
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
                            //////////////////////////////////////////////////////////////////////////////
                            /// Link : 
                            /// 
                            try
                            {
                                List<string> msg = new List<string>();
                                m_Tpd[enTPD.LinkID] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkID.ToString(); //1
                                m_Tpd[enTPD.FromNodeID] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID.ToString(); //2
                                m_Tpd[enTPD.ToNodeID] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToNodeID.ToString(); //3
                                m_Tpd[enTPD.LinkType] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type.ToString(); //4
                                m_Tpd[enTPD.LinkDistance] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Distance.ToString(); //5
                                m_Tpd[enTPD.RunDistance] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RunDistance.ToString(); //6
                                m_Tpd[enTPD.LinkVelocity] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Velocity.ToString(); //7
                                m_Tpd[enTPD.LinkBcrMatch] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.BCRMatchType.ToString(); //8
                                m_Tpd[enTPD.LinkSteerDirection] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.SteerDirection.ToString(); //9
                                m_Tpd[enTPD.CurFrontSteer] = DevicesManager.Instance.DevSteer.GetFrontSteerDirection().ToString(); //10

                                m_Tpd[enTPD.CurRearSteer] = DevicesManager.Instance.DevSteer.GetRearSteerDirection().ToString(); //11
                                m_Tpd[enTPD.LinkObsUpArea] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.UBSLevel0.ToString();//12
                                m_Tpd[enTPD.LinkObsDownArea] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.UBSLevel1.ToString();//13
                                m_Tpd[enTPD.CurObsUpLevel] = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperSensorState.ToString();//14
                                m_Tpd[enTPD.CurObsDownLevel] = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsLowerSensorState.ToString();//15
                                m_Tpd[enTPD.MxpOverrideRatio] = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.MxpOverrideRatio.ToString("F2");//16
                                m_Tpd[enTPD.CurOverrideRatio] = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideRatio.ToString("F2");//17
                                m_Tpd[enTPD.CurOverrideTime] = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideTime.ToString("F2");//18
                                m_Tpd[enTPD.LeftBCRBegin] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LeftBCRBegin.ToString("F2");//19
                                m_Tpd[enTPD.RightBCRBegin] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RightBCRBegin.ToString("F2");//20

                                m_Tpd[enTPD.LeftBCREnd] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LeftBCREnd.ToString("F2");//21
                                m_Tpd[enTPD.RightBCREnd] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RightBCREnd.ToString("F2");//22
                                m_Tpd[enTPD.LeftBCRStart] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LeftBCRStart.ToString("F2");//23
                                m_Tpd[enTPD.RightBCRStart] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RightBCRBegin.ToString("F2");//24
                                m_Tpd[enTPD.LeftBCRTarget] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LeftBCRTarget.ToString("F2");//25
                                m_Tpd[enTPD.RightBCRTarget] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RightBCRTarget.ToString("F2");//26
                                m_Tpd[enTPD.VirtualBCR] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.VirtualBcr.ToString("F2");//27
                                m_Tpd[enTPD.CurLeftBCR] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr.ToString("F2");//28
                                m_Tpd[enTPD.CurRightBCR] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr.ToString("F2");//29
                                m_Tpd[enTPD.PDOMVelocity] = m_MasterAxis.GetPdoVelocity().ToString("F2");//30

                                m_Tpd[enTPD.CmdMVelocity] = m_MasterAxis.GetCommandVelocity().ToString("F2");//31
                                m_Tpd[enTPD.CmdSVelocity] = m_SlaveAxis.GetCommandVelocity().ToString("F2");//32
                                m_Tpd[enTPD.CurMVelocity] = m_MasterAxis.GetCurVelocity().ToString("F2");//33
                                m_Tpd[enTPD.CurSVelocity] = m_SlaveAxis.GetCurVelocity().ToString("F2");//34
                                m_Tpd[enTPD.CmdMPosition] = m_MasterAxis.GetCommandPosition().ToString("F2");//35
                                m_Tpd[enTPD.CurMPosition] = m_MasterAxis.GetCurPosition().ToString("F2");//36
                                m_Tpd[enTPD.CurSPosition] = m_SlaveAxis.GetCurPosition().ToString("F2");//37
                                m_Tpd[enTPD.FollowingError] = m_MasterAxis.GetFollowingError().ToString("F2");//38
                                m_Tpd[enTPD.MTorqueLimit] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.TorqueLimit.ToString("F2");//39
                                m_Tpd[enTPD.STorqueLimit] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.TorqueLimit.ToString("F2");//40

                                m_Tpd[enTPD.CurMTorque] = m_MasterAxis.GetCurTorque().ToString("F2");//41
                                m_Tpd[enTPD.CurSTorque] = m_SlaveAxis.GetCurTorque().ToString("F2");//42
                                m_Tpd[enTPD.MotorTargetPosition] = ProcessDataHandler.Instance.CurTransferCommand.TargetMotorPosition.ToString("F2");//43
                                m_Tpd[enTPD.MotorRemainDistance] = ProcessDataHandler.Instance.CurTransferCommand.RemainMotorDistance.ToString("F2");//44
                                m_Tpd[enTPD.BCRCheckDirection] = ProcessDataHandler.Instance.CurTransferCommand.TargetBcrCheckDirection.ToString();//45
                                m_Tpd[enTPD.BCRTargetPositionLeft] = ProcessDataHandler.Instance.CurTransferCommand.TargetLeftBcrPosition.ToString("F2");//46
                                m_Tpd[enTPD.BCRTargetPositionRight] = ProcessDataHandler.Instance.CurTransferCommand.TargetRightBcrPosition.ToString("F2");//47
                                m_Tpd[enTPD.BCRRemainDistance] = ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance.ToString("F2");//48
                                m_Tpd[enTPD.PositionOfLink] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink.ToString("F2");//49
                                m_Tpd[enTPD.RemainOfLink] = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.RemainDistanceOfLink.ToString("F2");//50

                                m_Tpd[enTPD.CollisionDistance] = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionDistance.ToString("F2");//51
                                m_Tpd[enTPD.TargetBcrDistance] = ProcessDataHandler.Instance.CurTransferCommand.TargetBcrDistance.ToString("F2");//52
                                m_Tpd[enTPD.StopCheckDistance] = ProcessDataHandler.Instance.CurTransferCommand.StopCheckDistance.ToString("F2");//53
                                m_Tpd[enTPD.BcrRunDistance] = ProcessDataHandler.Instance.CurTransferCommand.BcrScanMotion.Distance.ToString("F2");//54
                                m_Tpd[enTPD.SetExternalEncoder] = ProcessDataHandler.Instance.CurTransferCommand.IsSetExternalEncorder.ToString();//55
                                m_Tpd[enTPD.SequenceMoving] = ProcessDataHandler.Instance.CurTransferCommand.IsSequenceMoving.ToString();//56
                                m_Tpd[enTPD.MasterAxisMoving] = m_MasterAxis.IsAxisReady().ToString();//57
                                m_Tpd[enTPD.HeartBitOk] = GV.MxpHeartBitNg.ToString();//58
                                m_Tpd[enTPD.HeartBitError] = GV.MxpHeartBitError.ToString();//59
                                m_Tpd[enTPD.TraTargetTime] = (m_MasterAxis.GetAxis() as MpAxis).TrajectoryTargetTime.ToString("F2");//60

                                m_Tpd[enTPD.TraCurrentTime] = (m_MasterAxis.GetAxis() as MpAxis).TrajectoryCurrentTime.ToString("F2");//61
                                m_Tpd[enTPD.TraTargetVel] = (m_MasterAxis.GetAxis() as MpAxis).TrajectoryTargetVelocity.ToString("F2");//62
                                m_Tpd[enTPD.TraCurStep] = (m_MasterAxis.GetAxis() as MpAxis).TrajectoryCurrentStep.ToString();//63
                                m_Tpd[enTPD.TrajectoryState] = (m_MasterAxis.GetAxis() as MpAxis).TrajectoryState.ToString();//64
                                if (m_SlideAxis != null)
                                {
                                    m_Tpd[enTPD.SlidePosition] = m_SlideAxis.IsValid ? m_SlideAxis.GetCurPosition().ToString("F2") : "0";//65
                                    m_Tpd[enTPD.SlideTorque] = m_SlideAxis.IsValid ? m_SlideAxis.GetCurTorque().ToString("F2") : "0";//66
                                    m_Tpd[enTPD.SlideVelocity] = m_SlideAxis.IsValid ? m_SlideAxis.GetCurVelocity().ToString("F2") : "0";//67
                                }
                                if (m_HoistAxis != null)
                                {
                                    m_Tpd[enTPD.HoistPosition] = m_HoistAxis.GetCurPosition().ToString("F2");//68
                                    m_Tpd[enTPD.HoistTorque] = m_HoistAxis.GetCurTorque().ToString("F2");//69
                                    m_Tpd[enTPD.HoistVelocity] = m_HoistAxis.GetCurVelocity().ToString("F2");//70
                                }
                                if (m_RotateAxis != null)
                                {
                                    m_Tpd[enTPD.RotatePosition] = m_RotateAxis.GetCurPosition().ToString("F2");//71
                                    m_Tpd[enTPD.RotateTorque] = m_RotateAxis.GetCurTorque().ToString("F2");//72
                                    m_Tpd[enTPD.RotateVelocity] = m_RotateAxis.GetCurVelocity().ToString("F2");//73
                                }
                                UInt32 memory = 0;
                                MemoryController.GetProcessMemory(ref memory);
                                XSequence.m_CurrentCPU = m_CpuUsage.GetUsage();
                                m_Tpd[enTPD.CPU] = m_CpuUsage.GetUsage().ToString("F1");//74
                                m_Tpd[enTPD.Memory] = memory.ToString();//75

                                m_Tpd[enTPD.CPS_BoostVoltage] = DevicesManager.Instance.DevCPS.BoostVoltage.ToString("F2");
                                m_Tpd[enTPD.CPS_OutputVoltage] = DevicesManager.Instance.DevCPS.OutputVoltage.ToString("F2");

                                if (SetupManager.Instance.SetupOperation.NetworkControlUse == Use.Use)
                                {
                                    m_Tpd[enTPD.NetSpeed] = NetworkManager.Instance.NetSpeed.ToString();
                                    m_Tpd[enTPD.PacketReceived] = NetworkManager.Instance.NetworkInformation.PacketReceived.ToString();
                                    m_Tpd[enTPD.PacketSend] = NetworkManager.Instance.NetworkInformation.PacketSend.ToString();
                                    m_Tpd[enTPD.UploadSpeed] = NetworkManager.Instance.NetworkInformation.UploadSpeed.ToString();
                                    m_Tpd[enTPD.DownloadSpeed] = NetworkManager.Instance.NetworkInformation.DownloadSpeed.ToString();
                                    //m_Tpd[enTPD.WifiQuality] = "0";
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionLog.WriteLog(e.ToString());
                            }
                            finally
                            {
                                bool logging = ProcessDataHandler.Instance.CurTransferCommand.IsValid;
                                logging |= Math.Abs(m_MasterAxis.GetCommandVelocity()) > 0.1f;
                                logging |= m_HoistAxis != null ? m_HoistAxis.GetCurVelocity() > 0.1f : false;
                                logging |= m_SlideAxis != null ? m_SlideAxis.GetCurVelocity() > 0.1f : false;
                                logging |= m_RotateAxis != null ? m_RotateAxis.GetCurVelocity() > 0.1f : false;
                                if (logging)
                                {
                                    m_TpdLogger.WriteLog(m_Tpd.Values.ToArray());
                                }
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }

                            //if (AppConfig.Instance.Simulation.MY_DEBUG)
                            //{
                            //    double watch = (double)m_StopWatch.ElapsedTicks * 1000.0 / (double)System.Diagnostics.Stopwatch.Frequency;
                            //    string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                            //    System.Diagnostics.Debug.WriteLine($"{time} : [SeqTpdManage] [{watch}]");
                            //}
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > AppConfig.Instance.TpdLogScanTime)
                            {
                                //EventHandlerManager.Instance.InvokeUpdatePathData(m_Tpd, true);
                                seqNo = 0;
                            }
                        }
                        break;
                }

                this.SeqNo = seqNo;
                return -1;
            }
        }
    }
}
