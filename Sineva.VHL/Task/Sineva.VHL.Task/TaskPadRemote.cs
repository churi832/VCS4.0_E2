using Sineva.VHL.Data;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Device;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Task
{
    public class TaskPadRemote : XSequence
    {
        public static readonly TaskPadRemote Instance = new TaskPadRemote();

        public TaskPadRemote()
        {
            this.RegSeq(new SeqRemoteState());
            this.RegSeq(new SeqRemoteAction());
        }
        public class SeqRemoteState : XSeqFunc
        {
            public override int Do()
            {
                int seqNo = this.SeqNo;
                switch (seqNo)
                {
                    case 0:
                        {
                            if (RemoteManager.PadInstance.Conneted && !GV.ThreadStop)
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 10;
                            }
                            else
                            {
                                StartTicks = XFunc.GetTickCount();
                                seqNo = 20;
                            }
                        }
                        break;

                    case 10:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 200)
                            {
                                RemoteManager.PadInstance.Remoting.RemoteGUI.HeartBitRun = true;
                                RemoteManager.PadInstance.Remoting.RemoteGUI.HeartBitCount++;
                                UpdateRemoteGUI();
                                seqNo = 0;
                            }
                        }
                        break;

                    case 20:
                        {
                            if (XFunc.GetTickCount() - StartTicks > 5000)
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
            private void UpdateRemoteGUI()
            {
                try
                {
                    PadRemoteGUI remote = new PadRemoteGUI();

                    remote.OpMode = EqpStateManager.Instance.OpMode;
                    remote.EqState = EqpStateManager.Instance.State;
                    remote.EqRunMode = EqpStateManager.Instance.RunMode;
                    bool enable;
                    enable = true;
                    enable &= !EqpStateManager.Instance.EqpInitComp & !EqpStateManager.Instance.EqpInitIng;
                    remote.IsReady = enable;
                    remote.JcsState = JCSCommManager.Instance.JcsStatus.Connected ? "Ready" : "Not Ready";
                    remote.OcsState = OCSCommManager.Instance.OcsStatus.Connected ? "Ready" : "Not Ready";
                    remote.EqState = EqpStateManager.Instance.State;
                    remote.Velocity = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetCurVelocity();

                    remote.FrontSteerStatus = DevicesManager.Instance.DevSteer.FrontSteer.GetFw(-1) ? "LEFT" : "RIGHT";
                    remote.RearSteerStatus = DevicesManager.Instance.DevSteer.RearSteer.GetFw(-1) ? "LEFT" : "RIGHT";

                    var open = DevicesManager.Instance.DevGripperPIO.DoGripperOpen.IsDetected ? true : false;
                    var close = DevicesManager.Instance.DevGripperPIO.DoGripperClose.IsDetected ? true : false;
                    if (!open && !close)
                    {
                        remote.GripperStatus = "None";
                    }
                    else
                    {
                        remote.GripperStatus = open ? "Open" : "Close";
                    }

                    remote.MasterPosition = DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().GetCurPosition();
                    remote.SlavePosition = DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().GetCurPosition();
                    if(!DevicesManager.Instance.DevFoupGripper.IsValid)
                    {
                        remote.SlidePosition = 0.0;
                        remote.RotatePosition = 0.0;
                        remote.HoistPosition = 0.0;
                    }
                    else
                    {
                        remote.SlidePosition = DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis() != null ? DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().GetCurPosition() : 0.0;
                        remote.RotatePosition = DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis() != null ? DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().GetCurPosition() : 0.0;
                        remote.HoistPosition = DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis() != null ? DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().GetCurPosition() : 0.0;
                    }
                    remote.MasterHomeDone = ((DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                    remote.SlaveHomeDone = ((DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                    if (DevicesManager.Instance.DevFoupGripper.IsValid && DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis() != null)
                    {
                        remote.SlideHomeDone = ((DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                        remote.SlideServoState = ((DevicesManager.Instance.DevFoupGripper.AxisSlide.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";
                    }
                    if (DevicesManager.Instance.DevFoupGripper.IsValid && DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis() != null)
                    {
                        remote.RotateHomeDone = ((DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                        remote.RotateServoState = ((DevicesManager.Instance.DevFoupGripper.AxisTurn.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";
                    }
                    if (DevicesManager.Instance.DevFoupGripper.IsValid && DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis() != null)
                    {
                        remote.HoistHomeDone = ((DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                        remote.HoistServoState = ((DevicesManager.Instance.DevFoupGripper.AxisHoist.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";
                    }
                    remote.MasterServoState = ((DevicesManager.Instance.DevTransfer.AxisMaster.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";
                    remote.SlaveServoState = ((DevicesManager.Instance.DevTransfer.AxisSlave.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";

                    if (DevicesManager.Instance.DevFrontAntiDrop.IsValid)
                    {
                        remote.FrontAntiDropStatus = DevicesManager.Instance.DevFrontAntiDrop.GetLock() ? "Lock" : "UnLock";
                        remote.FrontAntiDropPosition = DevicesManager.Instance.DevFrontAntiDrop.Axis.GetDevAxis().GetCurPosition();
                        remote.FrontAntiDropHomeDone = ((DevicesManager.Instance.DevFrontAntiDrop.Axis.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                        remote.FrontAntiDropServoState = ((DevicesManager.Instance.DevFrontAntiDrop.Axis.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";
                    }
                    if (DevicesManager.Instance.DevRearAntiDrop.IsValid)
                    {
                        remote.RearAntiDropStatus = DevicesManager.Instance.DevRearAntiDrop.GetLock() ? "Lock" : "UnLock";
                        remote.RearAntiDropPosition = DevicesManager.Instance.DevRearAntiDrop.Axis.GetDevAxis().GetCurPosition();
                        remote.RearAntiDropHomeDone = ((DevicesManager.Instance.DevRearAntiDrop.Axis.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.HEnd) == enAxisInFlag.HEnd;
                        remote.RearAntiDropServoState = ((DevicesManager.Instance.DevRearAntiDrop.Axis.GetDevAxis().AxisTag.GetAxis() as IAxisCommand).GetAxisCurStatus() & enAxisInFlag.SvOn) == enAxisInFlag.SvOn ? "Servo On" : "Servo Off";
                    }

                    if (AlarmCurrentProvider.Instance.IsHeavyAlarm())
                    {
                        remote.IsAlarm = true;
                        remote.AlarmIds = AlarmCurrentProvider.Instance.GetCurrentAlarmIds();
                        remote.AlarmName = AlarmCurrentProvider.Instance.GetCurrentAlarms().Select(x => x.Name).ToList();
                    }
                    else
                    {
                        remote.IsAlarm = false;
                        remote.AlarmIds = new List<int>();
                    }

                    RemoteManager.PadInstance.Remoting.RemoteGUI = remote;
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(string.Format("UpdateRemoteTouch Exception {0}", ex.ToString()));
                }
            }
        }

        public class SeqRemoteAction : XSeqFunc
        {
            private _Axis currentAxis = null;
            private ServoUnit currentServoUnit = null;
            private VelSet set = new VelSet();
            private RemoteItem remoteItem;

            private bool m_FrontLeft = false;
            private bool m_FrontRight = false;
            private bool m_RearLeft = false;
            private bool m_RearRight = false;

            public override int Do()
            {
                int seqNo = this.SeqNo;
                if (seqNo == 0)
                {
                    if (RemoteManager.PadInstance.Remoting == null) return -1;
                    remoteItem = RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem;
                }
                switch (seqNo)
                {
                    case 0:
                        {
                            if (RemoteManager.PadInstance.Conneted && !GV.ThreadStop)
                            {
                                seqNo = 10;
                            }

                        }
                        break;

                    case 10:
                        {
                            if (EqpStateManager.Instance.OpMode == OperateMode.Auto || remoteItem == null)
                            {
                                seqNo = 0;
                            }
                            else if (remoteItem.RemoteAction == RemoteItem.ActionType.None)
                            {
                                seqNo = 0;
                            }
                            else if (remoteItem.RemoteAction == RemoteItem.ActionType.JogStop)
                            {
                                seqNo = 130;
                            }
                            else if (remoteItem.RemoteAction == RemoteItem.ActionType.SteerLeft)
                            {
                                seqNo = 200;
                            }
                            else if (remoteItem.RemoteAction == RemoteItem.ActionType.SteerRight)
                            {
                                seqNo = 201;
                            }
                            else if (remoteItem.RemoteDevice != RemoteItem.DeviceType.None
                                || remoteItem.RemoteAction != RemoteItem.ActionType.None)
                            {
                                seqNo = 20;
                            }
                            else
                            {
                                seqNo = 0;
                                RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                            }
                        }
                        break;

                    case 20:
                        enAxisCoord enAxis = enAxisCoord.X;
                        switch (remoteItem.RemoteDevice)
                        {
                            case RemoteItem.DeviceType.None:
                                break;
                            case RemoteItem.DeviceType.MasterTransfer:
                            case RemoteItem.DeviceType.SlaveTransfer:
                                enAxis = enAxisCoord.X;
                                set.AxisCoord = enAxisCoord.X;
                                break;
                            case RemoteItem.DeviceType.Slide:
                                enAxis = enAxisCoord.Y;
                                set.AxisCoord = enAxisCoord.Y;
                                break;
                            case RemoteItem.DeviceType.Rotate:
                                enAxis = enAxisCoord.T;
                                set.AxisCoord = enAxisCoord.T;
                                break;
                            case RemoteItem.DeviceType.Hoist:
                                enAxis = enAxisCoord.Z;
                                set.AxisCoord = enAxisCoord.Z;
                                break;
                            default:
                                break;
                        }

                        foreach (_Axis axis in ServoManager.Instance.AxisSource)
                        {
                            if (axis.AxisCoord == enAxis)
                            {
                                if (axis.CommandSkip) continue;
                                currentAxis = axis;
                                break;
                            }
                        }
                        if (currentAxis == null)
                        {
                            seqNo = 0;
                            RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        }
                        else
                        {
                            foreach (ServoUnit unit in ServoManager.Instance.ServoUnits)
                            {
                                if (unit == null) continue;
                                if (unit.Axes.Contains(currentAxis))
                                {
                                    currentServoUnit = unit;
                                    break;
                                }
                            }
                            if (currentServoUnit == null)
                            {
                                seqNo = 0;
                                RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                            }
                            else
                            {
                                switch (remoteItem.RemoteVelocity)
                                {
                                    case RemoteItem.VelocitySelect.Slow:
                                        if (enAxis == enAxisCoord.X)
                                        {
                                            set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevTransfer.TeachingVelocitySearch.PropId);
                                        }
                                        else
                                        {
                                            set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevFoupGripper.TeachingVelocityLow.PropId);
                                        }
                                        break;
                                    case RemoteItem.VelocitySelect.Mid:
                                        if (enAxis == enAxisCoord.X)
                                        {
                                            set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevTransfer.TeachingVelocitySearch.PropId);
                                        }
                                        else
                                        {
                                            set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevFoupGripper.TeachingVelocityMid.PropId);
                                        }
                                        break;
                                    case RemoteItem.VelocitySelect.High:
                                        if (enAxis == enAxisCoord.X)
                                        {
                                            set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevTransfer.TeachingVelocitySearch.PropId);
                                        }
                                        else
                                        {
                                            set = currentServoUnit.GetVel(currentAxis, DevicesManager.Instance.DevFoupGripper.TeachingVelocityHigh.PropId);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                seqNo = 100;
                            }
                        }
                        break;
                    case 100:
                        switch (remoteItem.RemoteAction)
                        {
                            case RemoteItem.ActionType.None:
                                seqNo = 0;
                                break;
                            case RemoteItem.ActionType.JogPlus:
                                seqNo = 110;
                                break;
                            case RemoteItem.ActionType.JogMinus:
                                seqNo = 120;
                                break;
                            case RemoteItem.ActionType.JogStop:
                                seqNo = 130;
                                break;
                            case RemoteItem.ActionType.ServoOn:
                                seqNo = 140;
                                break;
                            case RemoteItem.ActionType.ServoOff:
                                seqNo = 150;
                                break;
                            case RemoteItem.ActionType.ErrorReset:
                                seqNo = 160;
                                break;
                            default:
                                seqNo = 0;
                                RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                                break;
                        }
                        break;
                    case 110:
                        currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogPlus, set);
                        if (!RemoteManager.PadInstance.Conneted || remoteItem.RemoteAction == RemoteItem.ActionType.None)
                        {
                            remoteItem.RemoteAction = RemoteItem.ActionType.JogStop;
                            seqNo = 130;
                        }
                        break;
                    case 120:
                        currentServoUnit.JogMove(currentAxis, enAxisOutFlag.JogMinus, set);
                        if (!RemoteManager.PadInstance.Conneted || remoteItem.RemoteAction == RemoteItem.ActionType.None)
                        {
                            remoteItem.RemoteAction = RemoteItem.ActionType.JogStop;
                            seqNo = 130;
                        }
                        break;
                    case 130:
                        foreach (ServoUnit unit in ServoManager.Instance.ServoUnits)
                        {
                            if (unit == null) continue;
                            unit.Stop(currentAxis);
                            unit.ResetJogSpeed();
                            unit.ResetCommand();
                        }
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        seqNo = 0;
                        break;
                    case 140:
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        seqNo = 0;

                        break;
                    case 150:
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        seqNo = 0;

                        break;
                    case 160:
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        seqNo = 0;

                        break;


                    case 200:
                        int rv1 = -1;
                        if (remoteItem.RemoteDevice == RemoteItem.DeviceType.FrontSteer)
                        {
                            rv1 = DevicesManager.Instance.DevSteer.FrontLeft();
                            if (rv1 >= 0)
                            {
                                m_FrontLeft = false;
                                DevicesManager.Instance.DevSteer.FrontSteer.ResetOutput();
                                if (rv1 > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv1));
                            }
                        }
                        else if (remoteItem.RemoteDevice == RemoteItem.DeviceType.RearSteer)
                        {
                            rv1 = DevicesManager.Instance.DevSteer.RearLeft();
                            if (rv1 >= 0)
                            {
                                m_FrontLeft = false;
                                DevicesManager.Instance.DevSteer.RearSteer.ResetOutput();
                                if (rv1 > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv1));
                            }
                        }
                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        seqNo = 0;
                        break;

                    case 201:
                        rv1 = -1;
                        if (remoteItem.RemoteDevice == RemoteItem.DeviceType.FrontSteer)
                        {
                            rv1 = DevicesManager.Instance.DevSteer.FrontRight();
                            if (rv1 >= 0)
                            {
                                m_FrontLeft = false;
                                DevicesManager.Instance.DevSteer.FrontSteer.ResetOutput();
                                if (rv1 > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv1));
                            }
                        }
                        else if (remoteItem.RemoteDevice == RemoteItem.DeviceType.RearSteer)
                        {
                            rv1 = DevicesManager.Instance.DevSteer.RearRight();
                            if (rv1 >= 0)
                            {
                                m_FrontLeft = false;
                                DevicesManager.Instance.DevSteer.RearSteer.ResetOutput();
                                if (rv1 > 0) MessageBox.Show("Lock Alarm : {0}", EqpAlarm.GetAlarmMsg(rv1));
                            }
                        }

                        RemoteManager.PadInstance.Remoting.RemoteGUI.RemoteItem = new RemoteItem(RemoteItem.DeviceType.None, RemoteItem.ActionType.None);
                        seqNo = 0;
                        break;
                }
                this.SeqNo = seqNo;
                return -1;
            }
        }
    }
}
