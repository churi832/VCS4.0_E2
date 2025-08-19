using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Data;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.Device;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sineva.VHL.Task.TaskMonitor;
using Sineva.VHL.Library.Common;

namespace Sineva.VHL.Task
{
    public class TaskUpdateMotionData : XSequence
    {
        public static readonly TaskUpdateMotionData Instance = new TaskUpdateMotionData();

        public TaskUpdateMotionData()
        {
            ThreadInfo.Name = string.Format("TaskUpdateMotionData");

            this.RegSeq(new SeqUpdateMotionData());
        }
        public class SeqUpdateMotionData : XSeqFunc
        {
            private const string FuncName = "[SeqUpdateMotionData]";

            #region Fields
            private _DevAxis m_MasterAxis = null;
            private bool m_LogWrite = false;
            #endregion

            #region Constructor
            public SeqUpdateMotionData()
            {
                this.SeqName = $"SeqUpdateMotionData";
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
                            UpdateMotionData();
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }

            public void UpdateMotionData()
            {
                try
                {
                    // BCR Update
                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                    {
                        m_MasterAxis.SetAxisLeftBarcode(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr);
                        m_MasterAxis.SetAxisRightBarcode(ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr);
                    }
                    else
                    {
                        ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr = m_MasterAxis.GetAxisCurLeftBarcode();
                        ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr = m_MasterAxis.GetAxisCurRightBarcode();
                    }

                    if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner() || ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.JCSAreaFlag)
                        (m_MasterAxis.GetAxis() as MpAxis).OverrideStopDistance = 0.5f * SetupManager.Instance.SetupWheel.OverrideStopDistance;
                    else (m_MasterAxis.GetAxis() as MpAxis).OverrideStopDistance = SetupManager.Instance.SetupWheel.OverrideStopDistance;

                    (m_MasterAxis.GetAxis() as MpAxis).OverrideLimitDistance = SetupManager.Instance.SetupWheel.OverrideLimitDistance;
                    (m_MasterAxis.GetAxis() as MpAxis).OverrideAcceleration = SetupManager.Instance.SetupWheel.OverrideAcceleration;
                    (m_MasterAxis.GetAxis() as MpAxis).OverrideDeceleration = SetupManager.Instance.SetupWheel.OverrideDeceleration;
                    (m_MasterAxis.GetAxis() as MpAxis).OverrideMaxDistance = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionMaxDistance;
                    (m_MasterAxis.GetAxis() as MpAxis).OverrideMinDistance = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionMinDistance;
                    if (ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsCorner())
                    {
                        // Link Velocity 변경시점과 TrajectoryTargetVelocity 변경 시점에 다른 경우. 곡선->직선->곡선 인 경우 감속 요인이 된다. 곡선 최대 속도를 Fix 시키자~~
                        (m_MasterAxis.GetAxis() as MpAxis).OverrideMaxVelocity = 710.0f + 10.0f;
                    }
                    else
                    {
                        (m_MasterAxis.GetAxis() as MpAxis).OverrideMaxVelocity = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.LinkVelocity + 10.0f;
                    }
                    (m_MasterAxis.GetAxis() as MpAxis).SetOverrideSensorState(ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperSensorState);
                    (m_MasterAxis.GetAxis() as MpAxis).SensorRemainDistance = ProcessDataHandler.Instance.CurTransferCommand.RemainBcrDistance;
                    (m_MasterAxis.GetAxis() as MpAxis).SetSpeedOverrideRate(ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideRatio);

                    // Override Update
                    ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.MxpOverrideRatio = (m_MasterAxis.GetAxis() as IAxisCommand).GetSpeedOverrideRate();
                    double collisionDistance = ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.CollisionDistance;
                    if (collisionDistance < 10000.0f)
                    {
                        (m_MasterAxis.GetAxis() as MpAxis).OverrideCollisionDistance = collisionDistance;
                        m_LogWrite = false;
                    }
                    else
                    {
                        if ((m_MasterAxis.GetAxis() as MpAxis).OverrideCollisionDistance < 10000.0f && !m_LogWrite)
                        {
                            m_LogWrite = true;
                            SequenceLog.WriteLog(FuncName, string.Format("Collision Distance Set : {0}, {1}, {2}",
                                collisionDistance,
                                (m_MasterAxis.GetAxis() as MpAxis).OverrideCollisionDistance, ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperSensorState));
                        }

                        // NONE 상태가 아닌데 자꾸 10000값이 들어가네... 이상하다...
                        if (ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.ObsUpperSensorState == enFrontDetectState.enNone)
                        {
                            (m_MasterAxis.GetAxis() as MpAxis).OverrideCollisionDistance = collisionDistance;
                        }
                    }
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
