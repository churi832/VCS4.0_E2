using Sineva.VHL.Data;
using Sineva.VHL.Device;
using Sineva.VHL.Device.Assem;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.IF.JCS;
using Sineva.VHL.IF.OCS;
using System.Collections;

namespace Sineva.VHL.Task
{
    public class TaskInterface : XSequence
    {
        #region Fileds
        public static readonly TaskInterface Instance = new TaskInterface();
        public SeqAutoDoorControl AutoDoorControl = null;
        #endregion
        #region Constructor
        public TaskInterface() 
        {
            ThreadInfo.Name = string.Format("TaskInterface");

            AutoDoorControl = new SeqAutoDoorControl();
            this.RegSeq(new SeqPortLoadIF());
            this.RegSeq(new SeqPortUnloadIF());
            this.RegSeq(new SeqSPLIF());
            this.RegSeq(new SeqMTLIF());
            this.RegSeq(AutoDoorControl);
        }
        #endregion
    }
    /// <summary>
    /// 1. IfStartReq : Port 위치에 도착 후 I/F Start해라
    /// 2. IfBusy : iCS -> oValid -> iLREQ -> oTRREQ -> iREADY -> oBUSY 신호 확인 후 Hoist Down 동작을 시작해라
    /// 3. IfOnIng : iLREQ 신호가 OFF 되었다. 정상적으로 Material Move 완료 ... Hoist Up 동작을 시작해라
    /// 4. IfComplete : Hoist Up 동작 완료 후 Interface를 완료 시켜라...
    /// 5.            : oBUSY -> oTRREQ -> oCOMP -> iREADY -> iCS -> oCOMP -> oVALID 순서로 신호 주고 받기
    /// 6. IfOperateCancel : Alarm 발생으로 멈춰 있는 경우 Recovery 해라
    ///                    : Load 중 Alarm.... iLREQ OFF 기준으로 복구 방법 구분 (전이면 Gripper Close상태에서 UP -> Deposit Fail, 후이면 Gripper Open상태에서 UP -> Deposite Comp)
    ///                    : Unload 중 Alarm ... IUREQ OFF 기준으로 복구 방법 구분 (Load 동작과 동일...)
    /// </summary>
    public class SeqPortLoadIF : XSeqFunc
    {
        private const string FuncName = "[SeqPortLoadIF]";

        #region Field
        private DevEqPIO m_devEqPio = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;

        private XTimer m_Timer = new XTimer("SeqPortLoadIF");
        private bool m_Simul = false;

        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;
        private bool m_MoveComp3 = false;

        private bool m_ESCheckStart = false;
        #endregion

        #region Constructor
        public SeqPortLoadIF()
        {
            this.SeqName = $"SeqPortLoadIF";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;

            m_Simul = AppConfig.Instance.Simulation.IO;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInterface;
            StartTicks = XFunc.GetTickCount();
        }
        public override void SeqAbort()
        {
            if (this.scm.Ing)
            {
                m_devEqPio.SeqAbort();
                this.scm.Abort = true;
            }
            if (AlarmId > 0)
            {
                SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset (Abort status)");
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            m_ESCheckStart = false;
            this.InitSeq();
            StartTicks = XFunc.GetTickCount();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("[IfFlagSend.Start TRUE] => Loading Start Condition Check"));
                this.SeqCaseMemoLists.Add(10, string.Format("AntiDrop Unlock"));
                this.SeqCaseMemoLists.Add(20, string.Format("PIO Connection"));
                this.SeqCaseMemoLists.Add(100, string.Format("PIO Signal Abnormal Status Check => [CS ON]"));
                this.SeqCaseMemoLists.Add(110, string.Format("HO_VABL ON => [VALID ON]"));
                this.SeqCaseMemoLists.Add(120, string.Format("L_REQ ON => [TR_REQ ON]"));
                this.SeqCaseMemoLists.Add(130, string.Format("READY ON => [BUSY ON][IfFlagSend.Busy TRUE]"));
                this.SeqCaseMemoLists.Add(140, string.Format("L_REQ OFF => [IfFlagSend.OnIng TRUE]"));
                this.SeqCaseMemoLists.Add(150, string.Format("[IfFlagSend.Complete TRUE] => [COMPT ON][TR_REQ OFF][BUSY OFF]"));
                this.SeqCaseMemoLists.Add(160, string.Format("READY OFF => [COMPT OFF][CS OFF][VALID OFF]"));
                this.SeqCaseMemoLists.Add(200, string.Format("Time Delay (100msec)"));
                this.SeqCaseMemoLists.Add(210, string.Format("PIO Disconnection"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset or Operate Cancel Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            if (AppConfig.Instance.VehicleType == VehicleType.Clean) return -1;

            int seqNo = SeqNo;
            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int rv3 = -1;

            bool eqpStateReady = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            //eqpStateReady &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            if (m_ESCheckStart) ESSignalCheck();

            switch (seqNo)
            {
                case 0:
                    {
                        bool loadReady = true;
                        loadReady &= m_devEqPio.IfFlagSend.StartReq & !m_devEqPio.IfFlagRecv.StartReq;
                        loadReady &= !m_devEqPio.IfFlagSend.Busy & !m_devEqPio.IfFlagRecv.Busy;
                        loadReady &= !m_devEqPio.IfFlagSend.OnIng & !m_devEqPio.IfFlagRecv.OnIng;
                        loadReady &= !m_devEqPio.IfFlagSend.Complete & !m_devEqPio.IfFlagRecv.Complete;
                        loadReady &= eqpStateReady;

                        if (loadReady)
                        {
                            this.scm.Start = true;

                            bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                            // SeqDeposit에서 Check 하고 StartReq 하겠지만 .... 한번더 확인 사살하자...
                            int target_port = ProcessDataHandler.Instance.CurTransferCommand.DestinationID;
                            int current_port = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
                            if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Port Loading Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 200;
                                seqNo = 1000;
                            }
                            else if (current_port != target_port)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_DestinationPortMismatchError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Destination ID isn't same Current Port ID - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 200;
                                seqNo = 1000;
                            }
                            else if (IsFoupExist() == false)
                            {
                                if (m_devGripperPio.DiLeftProductExist.IsDetected == false)
                                    AlarmId = m_devGripperPio.ALM_ProductLeftNotExistAlarm.ID;
                                else AlarmId = m_devGripperPio.ALM_ProductRightNotExistAlarm.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Port Loading Interface, Foup Not Exist Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 200;
                                seqNo = 1000;
                            }
                            else if (IsAntiDropUnlock() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("AntiDrop Lock Status. Unlock Start"));
                                m_MoveComp1 = false;
                                m_MoveComp2 = false;
                                seqNo = 10; // 10으로 이동하여 Unlock 실행하자!
                            }
                            else if (pio_used && (m_devEqPio.PioComm.IsGo() == false || m_devEqPio.PioComm.IsOpen() == false))
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Not Ready"));
                                seqNo = 20;
                            }
                            else
                            {
                                if (pio_used)
                                {
                                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                                    {
                                        m_devEqPio.DiES.SetDi(true);
                                    }

                                    m_ESCheckStart = true;
                                    m_devEqPio.ResetIfSend();
                                    m_devEqPio.ResetPIO();

                                    m_devEqPio.IfFlagSend.CS = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOCS;
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("Load PIO Start"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else
                                {
                                    m_devEqPio.ResetIfSend();
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("Load PIO Simulation Start, PORT:{0}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort));
                                    seqNo = 300;
                                }
                            }
                        }

                        if (XFunc.GetTickCount() - StartTicks > 5 * 1000 && m_devEqPio.IfFlagSend.StartReq)
                        {
                            ///////////////////////////////////////////////////////////////
                            string msg = string.Format("[SeqPortLoadIF Information]\r\n");
                            msg += string.Format("[AcquireStartReq, AcquireBusy, AcquireOnIng, AcquireComplete] [DepositStartReq, DepositBusy, DepositOnIng, DepositComplete] [OpMode, RunMode]\r\n");
                            msg += string.Format("[{0}, {1}, {2}, {3}], ", m_devEqPio.IfFlagRecv.StartReq, m_devEqPio.IfFlagRecv.Busy, m_devEqPio.IfFlagRecv.OnIng, m_devEqPio.IfFlagRecv.Complete);
                            msg += string.Format("[{0}, {1}, {2}, {3}], ", m_devEqPio.IfFlagSend.StartReq, m_devEqPio.IfFlagSend.Busy, m_devEqPio.IfFlagSend.OnIng, m_devEqPio.IfFlagSend.Complete);
                            msg += string.Format("[{0}, {1}], ", EqpStateManager.Instance.OpMode, EqpStateManager.Instance.RunMode);
                            ///////////////////////////////////////////////////////////////
                            SequenceInterfaceLog.WriteLog(FuncName, msg);

                            StartTicks = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 10:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Unlock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Unlock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock OK"));
                            seqNo = 0;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("RearAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 20:
                    {
                        int id = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOID;
                        int ch = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOCH;
                        rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel OK", id, ch));
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));
                            m_devEqPio.IfFlagSend.Cancel = true;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 100:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(false);
                            m_devEqPio.DiREADY.SetDi(false);
                            m_devEqPio.DiHOAVBL.SetDi(true);
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : 0;

                        bool signal_abnormal = false;
                        if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                        {
                            signal_abnormal |= m_devEqPio.GetLdReq();
                            signal_abnormal |= m_devEqPio.GetUlReq();
                            signal_abnormal |= m_devEqPio.GetReady();
                        }

                        if (signal_abnormal == false)
                        {
                            bool cs1 = (m_devEqPio.IfFlagSend.CS & (0x01 << 0)) == 0x01;
                            bool cs2 = (m_devEqPio.IfFlagSend.CS & (0x01 << 1)) == 0x02;
                            m_devEqPio.SetCs1(cs1);
                            m_devEqPio.SetCs2(cs2);
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PORT <- VHL : CS1 Signal {0}", cs1 ? "ON" : "OFF"));
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PORT <- VHL : CS2 Signal {0}", cs2 ? "ON" : "OFF"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagSend.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_LReq_SignalAlreadyOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            { 
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_ULReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetReady())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_Ready_SignalAlreadyOn.ID;
                            }
							else
							{
								AlarmId = m_devEqPio.ALM_Load_SignalAlreadyOn.ID;
							}
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Already EQ PIO Signal On!. Check PIO Log - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 110:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiHOAVBL.SetDi(true);
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : 1;

                        bool hand_off_available = true;
                        if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                        {
                            hand_off_available &= m_devEqPio.GetHandOffAvailable();
                            hand_off_available &= m_devEqPio.GetLdReq() == false;
                            hand_off_available &= m_devEqPio.GetUlReq() == false;
                            hand_off_available &= m_devEqPio.GetReady() == false;
                        }

                        if (hand_off_available && XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTD0ValidOnDelay)
                        {
                            m_devEqPio.SetValid(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : Valid Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagSend.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetHandOffAvailable() == false)
                            {
                                AlarmId = m_devEqPio.ALM_Load_HO_Signal_Off.ID;
                                EqpAlarm.Set(AlarmId);
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                            }
                            else
                            {
                                
                                if (m_devEqPio.GetLdReq())
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                    AlarmId = m_devEqPio.ALM_Load_LReq_SignalAlreadyOn.ID;
                                }
                                else if (m_devEqPio.GetUlReq())
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                    AlarmId = m_devEqPio.ALM_Load_ULReq_Signal_AbnormalOn.ID; 
                                }
                                else if (m_devEqPio.GetReady())
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                    AlarmId = m_devEqPio.ALM_Load_Ready_SignalAlreadyOn.ID; 
                                }
								else
								{
									AlarmId = m_devEqPio.ALM_Load_SignalAlreadyOn.ID;
								}
                            }

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Available Handoff Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 120:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.ES_Signal_Off_Before_Deposit) == enSimulationFlag.ES_Signal_Off_Before_Deposit)
                            {
                                m_devEqPio.DiES.SetDi(false);
                            }
                            if ((GV.SimulationFlag & enSimulationFlag.HO_Signal_Off_Before_Deposit) == enSimulationFlag.HO_Signal_Off_Before_Deposit)
                            {
                                m_devEqPio.DiHOAVBL.SetDi(false);
                            }
                            if ((GV.SimulationFlag & enSimulationFlag.PIO_TA1_Deposit) == enSimulationFlag.PIO_TA1_Deposit)
                            {
                                m_devEqPio.DiLDREQ.SetDi(false);
                            }
                            else
                            {
                                m_devEqPio.DiLDREQ.SetDi(true);
                            }
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : (int)m_devEqPio.TimerTa1;

                        bool ldReq = m_devEqPio.GetLdReq();
                        ldReq &= m_devEqPio.GetUlReq() == false;
                        ldReq &= m_devEqPio.GetReady() == false;
                        ldReq &= m_devEqPio.GetHandOffAvailable();
                        if (ldReq)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal ON");

                            m_devEqPio.SetTrReq(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : TR_REQ Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagSend.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            

                            if (m_devEqPio.GetHandOffAvailable() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Load_HO_Signal_Off.ID;
                            }
                            else if (m_devEqPio.GetLdReq() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Load_TA1_LReqNotGoOn_Timeout.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_ULReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetReady())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_TA1_Ready_Signal_AlreadyOn.ID;
                            }
							else
							{
								AlarmId = m_devEqPio.ALM_Load_TA1_LReqNotGoOn_Timeout.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("L_REQ Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 130:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.PIO_TA2_Deposit) == enSimulationFlag.PIO_TA2_Deposit)
                            {
                                m_devEqPio.DiREADY.SetDi(false);
                            }
                            else
                            {
                                m_devEqPio.DiREADY.SetDi(true);
                            }
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : (int)m_devEqPio.TimerTa2;

                        bool ready = m_devEqPio.GetReady();
                        ready &= m_devEqPio.GetLdReq();
                        ready &= m_devEqPio.GetUlReq() == false;
                        ready &= m_devEqPio.GetHandOffAvailable();

                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal ON");

                            m_devEqPio.IfFlagSend.Busy = true; //Deposit 동작을 시작해라....
                            m_devEqPio.SetBusy(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : BUSY Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagSend.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetHandOffAvailable() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Load_HO_Signal_Off.ID;
                            }
                            else if (m_devEqPio.GetLdReq() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Load_TA2_LReq_Signal_AbnormalOff.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_ULReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetReady() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Load_TA2_ReadyNotGoOn_Timeout.ID;
                            }
							else
							{
								AlarmId = m_devEqPio.ALM_Load_TA2_ReadyNotGoOn_Timeout.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("READY Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 140:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (XFunc.GetTickCount() - StartTicks > 2000)
                            {
                                if ((GV.SimulationFlag & enSimulationFlag.ES_Signal_Off_Acquiring) == enSimulationFlag.ES_Signal_Off_Acquiring)
                                {
                                    m_devEqPio.DiES.SetDi(false);
                                }
                                if ((GV.SimulationFlag & enSimulationFlag.HO_Signal_Off_Acquiring) == enSimulationFlag.HO_Signal_Off_Acquiring)
                                {
                                    m_devEqPio.DiHOAVBL.SetDi(false);
                                }
                            }

                            if ((GV.SimulationFlag & enSimulationFlag.PIO_TP3_Deposit) == enSimulationFlag.PIO_TP3_Deposit)
                            {
                                m_devEqPio.DiLDREQ.SetDi(true);
                            }
                            else
                            {
                                if (XFunc.GetTickCount() - StartTicks > 10000)
                                    m_devEqPio.DiLDREQ.SetDi(false);
                            }
                        }

                        bool ldReq = m_devEqPio.GetLdReq() == false;
                        ldReq &= m_devEqPio.GetUlReq() == false;
                        ldReq &= m_devEqPio.GetReady();
                        //ldReq &= m_devEqPio.GetHandOffAvailable();
                        if (ldReq)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal OFF");

                            m_devEqPio.IfFlagSend.OnIng = true; //정상적으로 안착이 되었으니 Up 해도 된다.....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 150;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTp3 * 1000)
                        {
                            m_devEqPio.IfFlagSend.Abort = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            //if (m_devEqPio.GetHandOffAvailable() == false) SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                            if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_LReqRemainOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_ULReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetReady() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Load_Ready_off_during_Busy_TA3.ID;
                            }
							else
							{
								AlarmId = m_devEqPio.ALM_Load_LReqRemainOn.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("L_REQ Singal OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                        else if (m_devEqPio.IfFlagSend.OperateCancel)
                        {
                            m_devEqPio.IfFlagSend.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("L_REQ Signal Not OFF, Operator Cancel"));
                            seqNo = 200; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 150:
                    {
                        bool complete = m_devEqPio.IfFlagSend.Complete;
                        if (complete)
                        {
                            m_devEqPio.IfFlagSend.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Deposit Complete"));

                            m_devEqPio.SetBusy(false);
                            m_devEqPio.SetTrReq(false);
                            m_devEqPio.SetComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : BUSY Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : TR_REQ Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 160;
                        }
                        else if (m_devEqPio.IfFlagSend.OperateCancel)
                        {
                            m_devEqPio.IfFlagSend.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Deposit not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 160:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.PIO_TA3_Deposit) == enSimulationFlag.PIO_TA3_Deposit)
                            {
                                m_devEqPio.DiREADY.SetDi(true);
                            }
                            else
                            {
                                m_devEqPio.DiREADY.SetDi(false);
                            }
                        }

                        bool ready = m_devEqPio.GetReady() == false;
                        ready &= m_devEqPio.GetLdReq() == false;
                        ready &= m_devEqPio.GetUlReq() == false;
                        //ready &= m_devEqPio.GetHandOffAvailable();
                        if (ready)
                        {
                            m_devEqPio.SetCs1(false);
                            m_devEqPio.SetCs2(false);
                            m_devEqPio.SetValid(false);
                            m_devEqPio.SetComplete(false);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : CS1 Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : CS2 Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : Valid Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : COMPLETE Signal OFF");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTa3 * 1000)
                        {
                            m_devEqPio.IfFlagSend.Abort = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            //if (m_devEqPio.GetHandOffAvailable() == false) SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                            if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_LReqRemainOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_ULReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetReady())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Load_TA3_ReadyNotGoOff_Timeout.ID;
                            }
							else
							{
                            	AlarmId = m_devEqPio.ALM_Load_TA3_ReadyNotGoOff_Timeout.ID;
							}

                            
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("READY Singal OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 200:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 100) break; // 끈기전 delay가 필요하네...
                        m_ESCheckStart = false;

                        // PIO Comm을 닫고 마무리 하자...!
                        m_devEqPio.ResetIfSend();
                        m_devEqPio.ResetPIO();

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        int id = 0;
                        int ch = 0;
                        rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel OK", id, ch));

                            m_devEqPio.IfFlagSend.PioOff = true;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Load PIO Finished"));

                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            m_devEqPio.IfFlagSend.PioOffNg = true; // Interface Fail Event 처리하자 ~~~

                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel Alarm - {0}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 300:
                    {
                        m_devEqPio.IfFlagSend.Busy = true; //Deposit 동작을 시작해라....
                        SequenceInterfaceLog.WriteLog(FuncName, "IfFlagSend.Busy = true, Deposit Start");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 310;
                    }
                    break;

                case 310:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            m_devEqPio.IfFlagSend.OnIng = true; //Up 동작을 시작해라....
                            SequenceInterfaceLog.WriteLog(FuncName, "IfFlagSend.OnIng = true, Up Action Enable");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 320;
                        }
                    }
                    break;

                case 320:
                    {
                        bool complete = m_devEqPio.IfFlagSend.Complete;
                        if (complete)
                        {
                            m_ESCheckStart = false;
                            m_devEqPio.IfFlagSend.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, "IfFlagSend.Complete = true, Deposit Complete");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 330;
                        }
                        else if (m_devEqPio.IfFlagSend.OperateCancel)
                        {
                            m_devEqPio.IfFlagSend.OperateCancel = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 330; 
                        }
                    }
                    break;

                case 330:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 100)
                        {
                            m_ESCheckStart = false;
                            m_devEqPio.ResetIfSend();

                            m_devEqPio.IfFlagRecv.PioOff = true;
                            SequenceInterfaceLog.WriteLog(FuncName, "Load PIO Simulation Finished");
                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset || m_devEqPio.IfFlagSend.OperateCancel)
                    {
                        if (m_devEqPio.IfFlagSend.OperateCancel)
                        {
                            m_devEqPio.IfFlagRecv.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, "Operator Cancel Alarm Reset");
                        }
                        else
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset");
                        }
                        //EqpAlarm.Reset(AlarmId);
                        EqpAlarm.ResetAll();
                        AlarmId = 0;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = ReturnSeqNo;
                    }
                    break;
            }

            SeqNo = seqNo;
            return rv;
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

        private bool IsAntiDropUnlock()
        {
            bool anti_drop_unlock = true;
            try
            {
                anti_drop_unlock &= m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() : true;
                anti_drop_unlock &= m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() : true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return anti_drop_unlock;
        }

        private void ESSignalCheck()
        {
            try
            {
                if (m_devEqPio.PioComm.IsGo() || AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    //if (m_devEqPio.GetLdReq())
                    {
                        if (m_devEqPio.GetEs() == false && m_devEqPio.IfFlagSend.ES == false)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : ES Signal OFF");
                            m_devEqPio.IfFlagSend.ES = true; // Hoist Stop
                        }
                        else if (m_devEqPio.GetEs() && m_devEqPio.IfFlagSend.ES)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : ES Signal ON");
                            m_devEqPio.IfFlagSend.ES = false; // Hoist Release
                        }
                        if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                        {
                            if (m_devEqPio.GetHandOffAvailable() == false && m_devEqPio.IfFlagSend.HO == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal OFF");
                                m_devEqPio.IfFlagSend.HO = true;
                            }
                            else if (m_devEqPio.GetHandOffAvailable() && m_devEqPio.IfFlagSend.HO)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal ON");
                                m_devEqPio.IfFlagSend.HO = false;
                            }
                        }
                    }
                }
                else
                {
                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : GO Signal OFF");
                    m_devEqPio.IfFlagSend.ES = true; // Hoist Stop
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion
    }

    public class SeqPortUnloadIF : XSeqFunc
    {
        private const string FuncName = "[SeqPortUnloadIF]";

        #region Field
        private DevEqPIO m_devEqPio = null;
        private DevGripperPIO m_devGripperPio = null;
        private DevAntiDrop m_devAntiDropFront = null;
        private DevAntiDrop m_devAntiDropRear = null;

        private XTimer m_Timer = new XTimer("SeqPortUnloadIF");
        private bool m_Simul = false;

        private bool m_MoveComp1 = false;
        private bool m_MoveComp2 = false;
        private bool m_MoveComp3 = false;

        private bool m_ESCheckStart = false;
        #endregion

        #region Constructor
        public SeqPortUnloadIF()
        {
            this.SeqName = $"SeqPortUnloadIF";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_devGripperPio = DevicesManager.Instance.DevGripperPIO;
            m_devAntiDropFront = DevicesManager.Instance.DevFrontAntiDrop;
            m_devAntiDropRear = DevicesManager.Instance.DevRearAntiDrop;

            m_Simul = AppConfig.Instance.Simulation.IO;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInterface;
            StartTicks = XFunc.GetTickCount();
        }
        public override void SeqAbort()
        {
            if (this.scm.Ing)
            {
                m_devEqPio.SeqAbort();
                this.scm.Abort = true;
            }
            if (AlarmId > 0)
            {
                SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset (Abort status)");
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            m_ESCheckStart = false;
            this.InitSeq();
			StartTicks = XFunc.GetTickCount();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("[IfFlagRecv.Start TRUE] => Unloading Start Condition Check"));
                this.SeqCaseMemoLists.Add(10, string.Format("AntiDrop Unlock"));
                this.SeqCaseMemoLists.Add(20, string.Format("PIO Connection"));
                this.SeqCaseMemoLists.Add(100, string.Format("PIO Signal Abnormal Status Check => [CS ON]"));
                this.SeqCaseMemoLists.Add(110, string.Format("HO_VABL ON => [VALID ON]"));
                this.SeqCaseMemoLists.Add(120, string.Format("L_REQ ON => [TR_REQ ON]"));
                this.SeqCaseMemoLists.Add(130, string.Format("READY ON => [BUSY ON][IfFlagRecv.Busy TRUE]"));
                this.SeqCaseMemoLists.Add(140, string.Format("L_REQ OFF => [IfFlagRecv.OnIng TRUE]"));
                this.SeqCaseMemoLists.Add(150, string.Format("[IfFlagRecv.Complete TRUE] => [COMPT ON][TR_REQ OFF][BUSY OFF]"));
                this.SeqCaseMemoLists.Add(160, string.Format("READY OFF => [COMPT OFF][CS OFF][VALID OFF]"));
                this.SeqCaseMemoLists.Add(200, string.Format("Time Delay (100msec)"));
                this.SeqCaseMemoLists.Add(210, string.Format("PIO Disconnection"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset or Operate Cancel Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;
            if (AppConfig.Instance.VehicleType == VehicleType.Clean) return -1;

            int seqNo = SeqNo;
            int rv = -1;
            int rv1 = -1;
            int rv2 = -1;
            int rv3 = -1;

            bool eqpStateReady = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            //eqpStateReady &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            if (m_ESCheckStart) ESSignalCheck();

            switch (seqNo)
            {
                case 0:
                    {
                        bool unloadReady = true;
                        unloadReady &= m_devEqPio.IfFlagRecv.StartReq & !m_devEqPio.IfFlagSend.StartReq;
                        unloadReady &= !m_devEqPio.IfFlagRecv.Busy & !m_devEqPio.IfFlagSend.Busy;
                        unloadReady &= !m_devEqPio.IfFlagRecv.OnIng & !m_devEqPio.IfFlagSend.OnIng;
                        unloadReady &= !m_devEqPio.IfFlagRecv.Complete & !m_devEqPio.IfFlagSend.Complete;
                        unloadReady &= eqpStateReady;

                        if (unloadReady)
                        {
                            this.scm.Start = true;

                            bool pio_used = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOUsed;
                            // SeqAcquire에서 Check 하고 StartReq 하겠지만 .... 한번더 확인 사살하자...
                            int target_port = ProcessDataHandler.Instance.CurTransferCommand.SourceID;
                            int current_port = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortID;
                            if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 200;
                                seqNo = 1000;
                            }
                            else if (current_port != target_port)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_DestinationPortMismatchError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Destination ID isn't same Current Port ID - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 200;
                                seqNo = 1000;
                            }
                            else if (IsFoupExist())
                            {
                                if (m_devGripperPio.DiLeftProductExist.IsDetected)
                                    AlarmId = m_devGripperPio.ALM_ProductLeftExistAlarm.ID;
                                else AlarmId = m_devGripperPio.ALM_ProductRightExistAlarm.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Port Unloading Interface, Foup Exist Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 200;
                                seqNo = 1000;
                            }
                            else if (IsAntiDropUnlock() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("AntiDrop Lock Status. Unlock Start"));
                                m_MoveComp1 = false;
                                m_MoveComp2 = false;
                                seqNo = 10; // 10으로 이동하여 Unlock 실행하자!
                            }
                            else if (pio_used && (m_devEqPio.PioComm.IsGo() == false || m_devEqPio.PioComm.IsOpen() == false))
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Not Ready"));
                                seqNo = 20;
                            }
                            else
                            {
                                if (pio_used)
                                {
                                    if (AppConfig.Instance.Simulation.MY_DEBUG)
                                    {
                                        m_devEqPio.DiES.SetDi(true);
                                    }
                                    m_ESCheckStart = true;
                                    m_devEqPio.ResetIfRecv();
                                    m_devEqPio.ResetPIO();

                                    m_devEqPio.IfFlagRecv.CS = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOCS;
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("Unload PIO Start"));
                                    StartTicks = XFunc.GetTickCount();
                                    seqNo = 100;
                                }
                                else
                                {
                                    m_devEqPio.ResetIfRecv();
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("Unload PIO Simulation Start, PORT:{0}", ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort));
                                    seqNo = 300;
                                }
                            }
                        }

                        if (XFunc.GetTickCount() - StartTicks > 5 * 1000 && m_devEqPio.IfFlagRecv.StartReq)
                        {
                            ///////////////////////////////////////////////////////////////
                            string msg = string.Format("[SeqPortLoadIF Information]\r\n");
                            msg += string.Format("[AcquireStartReq, AcquireBusy, AcquireOnIng, AcquireComplete] [DepositStartReq, DepositBusy, DepositOnIng, DepositComplete] [OpMode, RunMode]\r\n");
                            msg += string.Format("[{0}, {1}, {2}, {3}], ", m_devEqPio.IfFlagRecv.StartReq, m_devEqPio.IfFlagRecv.Busy, m_devEqPio.IfFlagRecv.OnIng, m_devEqPio.IfFlagRecv.Complete);
                            msg += string.Format("[{0}, {1}, {2}, {3}], ", m_devEqPio.IfFlagSend.StartReq, m_devEqPio.IfFlagSend.Busy, m_devEqPio.IfFlagSend.OnIng, m_devEqPio.IfFlagSend.Complete);
                            msg += string.Format("[{0}, {1}], ", EqpStateManager.Instance.OpMode, EqpStateManager.Instance.RunMode);
                            ///////////////////////////////////////////////////////////////
                            SequenceInterfaceLog.WriteLog(FuncName, msg);

                            StartTicks = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 10:
                    {
                        if (!m_MoveComp1)
                        {
                            rv1 = m_devAntiDropFront.Unlock();
                            if (rv1 == 0) m_MoveComp1 = true;
                        }
                        if (!m_MoveComp2)
                        {
                            rv2 = m_devAntiDropRear.Unlock();
                            if (rv2 == 0) m_MoveComp2 = true;
                        }

                        if (m_MoveComp1 && m_MoveComp2)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AntiDrop Unlock OK"));
                            seqNo = 0;
                        }
                        else if (rv1 > 0 || rv2 > 0)
                        {
                            if (rv1 > 0)
                            {
                                AlarmId = rv1;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("FrontAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            else if (rv2 > 0)
                            {
                                AlarmId = rv2;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("RearAntiDrop Unlock Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            }
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 20:
                    {
                        int id = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOID;
                        int ch = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PIOCH;
                        rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel OK", id, ch));
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));
                            m_devEqPio.IfFlagRecv.Cancel = true;
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 100:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(false);
                            m_devEqPio.DiREADY.SetDi(false);
                            m_devEqPio.DiHOAVBL.SetDi(true);
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : 0;

                        bool signal_abnormal = false;
                        if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                        {
                            signal_abnormal |= m_devEqPio.GetLdReq();
                            signal_abnormal |= m_devEqPio.GetUlReq();
                            signal_abnormal |= m_devEqPio.GetReady();
                        }

                        if (signal_abnormal == false)
                        {
                            bool cs1 = (m_devEqPio.IfFlagRecv.CS & (0x01 << 0)) == 0x01;
                            bool cs2 = (m_devEqPio.IfFlagRecv.CS & (0x01 << 1)) == 0x02;
                            m_devEqPio.SetCs1(cs1);
                            m_devEqPio.SetCs2(cs2);
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PORT <- VHL : CS1 Signal {0}", cs1 ? "ON" : "OFF"));
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PORT <- VHL : CS2 Signal {0}", cs2 ? "ON" : "OFF"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagRecv.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_ULReq_SignalAlreadyOn.ID;
                            }
                            else if (m_devEqPio.GetReady())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                            }
							else
							{
                            	AlarmId = m_devEqPio.ALM_Unload_SignalAlreadyOn.ID;
							}

                            
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Already EQ PIO Signal On!. Check PIO Log - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 110:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiHOAVBL.SetDi(true);
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : 1;

                        bool hand_off_available = true;
                        if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                        {
                            hand_off_available &= m_devEqPio.GetHandOffAvailable();
                            hand_off_available &= m_devEqPio.GetLdReq() == false;
                            hand_off_available &= m_devEqPio.GetUlReq() == false;
                            hand_off_available &= m_devEqPio.GetReady() == false;
                        }
                        if (hand_off_available && XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTD0ValidOnDelay)
                        {
                            m_devEqPio.SetValid(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : Valid Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagRecv.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetHandOffAvailable() == false)
                            {
                                AlarmId = m_devEqPio.ALM_Unload_HO_Signal_Off.ID;
                                    EqpAlarm.Set(AlarmId);
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                            }
                            else
                            {
                                if (m_devEqPio.GetLdReq())
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                    AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                                }
                                else if (m_devEqPio.GetUlReq())
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                    AlarmId = m_devEqPio.ALM_Unload_ULReq_SignalAlreadyOn.ID;
                                }
                                else if (m_devEqPio.GetReady())
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                    AlarmId = m_devEqPio.ALM_Unload_Ready_SignalAlreadyOn.ID;
                                }
								else
								{
	                                AlarmId = m_devEqPio.ALM_Unload_SignalAlreadyOn.ID;
								}
                            }

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Available Handoff Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 120:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiULREQ.SetDi(true);

                            if ((GV.SimulationFlag & enSimulationFlag.ES_Signal_Off_Before_Acquire) == enSimulationFlag.ES_Signal_Off_Before_Acquire)
                            {
                                m_devEqPio.DiES.SetDi(false);
                            }
                            if ((GV.SimulationFlag & enSimulationFlag.HO_Signal_Off_Before_Acquire) == enSimulationFlag.HO_Signal_Off_Before_Acquire)
                            {
                                m_devEqPio.DiHOAVBL.SetDi(false);
                            }
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : (int)m_devEqPio.TimerTa1;

                        bool ldReq = m_devEqPio.GetUlReq();
                        ldReq &= m_devEqPio.GetLdReq() == false;
                        ldReq &= m_devEqPio.GetReady() == false;
                        ldReq &= m_devEqPio.GetHandOffAvailable();
                        if (ldReq)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal ON");

                            m_devEqPio.SetTrReq(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : TR_REQ Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagRecv.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetHandOffAvailable() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Unload_HO_Signal_Off.ID;
                            }
                            else if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Unload_TA1_UReqNotGoOn_Timeout.ID;
                            }
                            else if (m_devEqPio.GetReady())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_TA1_Ready_Signal_AlreadyOn.ID;
                            }
							else
							{
                            	AlarmId = m_devEqPio.ALM_Unload_TA1_UReqNotGoOn_Timeout.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("L_REQ Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 130:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiREADY.SetDi(true);
                        }
                        bool stk = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.LeftSTKPort;
                        stk |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPort.PortType == PortType.RightSTKPort;
                        int wait_time = stk ? SetupManager.Instance.SetupPIO.PioSTKReadyWaitTime : (int)m_devEqPio.TimerTa2;

                        bool ready = m_devEqPio.GetReady();
                        ready &= m_devEqPio.GetUlReq();
                        ready &= m_devEqPio.GetLdReq() == false;
                        ready &= m_devEqPio.GetHandOffAvailable();
                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal ON");

                            m_devEqPio.IfFlagRecv.Busy = true; //Acquire 동작을 시작해라....
                            m_devEqPio.SetBusy(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : BUSY Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > wait_time * 1000)
                        {
                            m_devEqPio.IfFlagRecv.Cancel = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            if (m_devEqPio.GetHandOffAvailable() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Unload_HO_Signal_Off.ID;
                            }
                            else if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Unload_TA2_ULReq_Signal_AbnormalOff.ID;
                            }
                            else if (m_devEqPio.GetReady() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Unload_ReadyNotGoOn_TA2Timeout.ID;
                            }
							else
							{
                            	AlarmId = m_devEqPio.ALM_Unload_ReadyNotGoOn_TA2Timeout.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("READY Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 140:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if (XFunc.GetTickCount() - StartTicks > 10000)
                                m_devEqPio.DiULREQ.SetDi(false);
                            if (XFunc.GetTickCount() - StartTicks > 2000)
                            {
                                if ((GV.SimulationFlag & enSimulationFlag.ES_Signal_Off_Acquiring) == enSimulationFlag.ES_Signal_Off_Acquiring)
                                {
                                    m_devEqPio.DiES.SetDi(false);
                                }
                                if ((GV.SimulationFlag & enSimulationFlag.HO_Signal_Off_Acquiring) == enSimulationFlag.HO_Signal_Off_Acquiring)
                                {
                                    m_devEqPio.DiHOAVBL.SetDi(false);
                                }
                            }
                        }

                        bool ulReq = m_devEqPio.GetUlReq() == false;
                        ulReq &= m_devEqPio.GetLdReq() == false;
                        ulReq &= m_devEqPio.GetReady();
                        //ulReq &= m_devEqPio.GetHandOffAvailable();

                        if (ulReq)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal OFF");

                            m_devEqPio.IfFlagRecv.OnIng = true; //정상적으로 안착이 되었으니 Up 해도 된다.....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 150;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTp3 * 1000)
                        {
                            m_devEqPio.IfFlagRecv.Abort = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            //if (m_devEqPio.GetHandOffAvailable() == false) SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                            if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_UReqRemainOn.ID;
                            }
                            else if (m_devEqPio.GetReady() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal OFF");
                                AlarmId = m_devEqPio.ALM_Unload_Ready_off_during_Busy_TA3.ID;
                            }
							else
							{
                            	AlarmId = m_devEqPio.ALM_Unload_UReqRemainOn.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("U_REQ Singal OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                        else if (m_devEqPio.IfFlagRecv.OperateCancel)
                        {
                            m_devEqPio.IfFlagRecv.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("U_REQ Signal Not OFF, Operator Cancel"));
                            seqNo = 200; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 150:
                    {
                        bool complete = m_devEqPio.IfFlagRecv.Complete;
                        if (complete)
                        {
                            m_devEqPio.IfFlagRecv.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Acquire Complete"));

                            m_devEqPio.SetBusy(false);
                            m_devEqPio.SetTrReq(false);
                            m_devEqPio.SetComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : BUSY Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : TR_REQ Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 160;
                        }
                        else if (m_devEqPio.IfFlagRecv.OperateCancel)
                        {
                            m_devEqPio.IfFlagRecv.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Acquire not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 160:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            if ((GV.SimulationFlag & enSimulationFlag.PIO_TA3_Acquire) == enSimulationFlag.PIO_TA3_Acquire)
                            {
                                m_devEqPio.DiREADY.SetDi(true);
                            }
                            else
                            {
                                m_devEqPio.DiREADY.SetDi(false);
                            }
                        }

                        bool ready = m_devEqPio.GetReady() == false;
                        ready &= m_devEqPio.GetUlReq() == false;
                        ready &= m_devEqPio.GetLdReq() == false;
                        //ready &= m_devEqPio.GetHandOffAvailable();

                        if (ready)
                        {
                            m_devEqPio.SetCs1(false);
                            m_devEqPio.SetCs2(false);
                            m_devEqPio.SetValid(false);
                            m_devEqPio.SetComplete(false);
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : CS1 Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : CS2 Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : Valid Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT <- VHL : COMPLETE Signal OFF");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > m_devEqPio.TimerTa3 * 1000)
                        {
                            m_devEqPio.IfFlagRecv.Abort = true; // Deposit Sequence에서 이걸 보고 OperatorCancel 하도록 하자....

                            //if (m_devEqPio.GetHandOffAvailable() == false) SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal Abnormal OFF");
                            if (m_devEqPio.GetLdReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : L_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_LReq_Signal_AbnormalOn.ID;
                            }
                            else if (m_devEqPio.GetUlReq())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : U_REQ Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_UReqRemainOn.ID;
                            }
                            else if (m_devEqPio.GetReady())
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : READY Signal Abnormal ON");
                                AlarmId = m_devEqPio.ALM_Unload_TA3_ReadyNotGoOff_Timeout.ID;
                            }
							else
							{
                            	AlarmId = m_devEqPio.ALM_Unload_TA3_ReadyNotGoOff_Timeout.ID;
							}

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("READY Singal OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 200; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 200:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 100) break; // 끈기전 delay가 필요하네...
                        m_ESCheckStart = false;

                        // PIO Comm을 닫고 마무리 하자...!
                        m_devEqPio.ResetIfRecv();
                        m_devEqPio.ResetPIO();

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        int id = 0;
                        int ch = 0;
                        rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel OK", id, ch));

                            m_devEqPio.IfFlagRecv.PioOff = true;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Unload PIO Finished"));
                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            m_devEqPio.IfFlagRecv.PioOffNg = true; // Interface Fail Event 처리하자 ~~~

                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("EQP_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 300:
                    {
                        m_devEqPio.IfFlagRecv.Busy = true; //Deposit 동작을 시작해라....
                        SequenceInterfaceLog.WriteLog(FuncName, "IfFlagRecv.Busy = true, Deposit Start");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 310;
                    }
                    break;

                case 310:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 1000)
                        {
                            m_devEqPio.IfFlagRecv.OnIng = true; //Up 동작을 시작해라....
                            SequenceInterfaceLog.WriteLog(FuncName, "IfFlagRecv.OnIng = true, Up Action Enable");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 320;
                        }
                    }
                    break;

                case 320:
                    {
                        bool complete = m_devEqPio.IfFlagRecv.Complete;
                        if (complete)
                        {
                            m_ESCheckStart = false;

                            m_devEqPio.IfFlagRecv.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, "IfFlagRecv.Complete = true, Deposit Complete");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 330;
                        }
                        else if (m_devEqPio.IfFlagRecv.OperateCancel)
                        {
                            m_devEqPio.IfFlagRecv.OperateCancel = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 330;
                        }
                    }
                    break;

                case 330:
                    {
                        if (XFunc.GetTickCount() - StartTicks > 100)
                        {
                            m_ESCheckStart = false;
                            m_devEqPio.ResetIfRecv();

                            m_devEqPio.IfFlagRecv.PioOff = true;
                            SequenceInterfaceLog.WriteLog(FuncName, "Unload PIO Simulation Finished");
                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset || m_devEqPio.IfFlagRecv.OperateCancel)
                    {
                        if (m_devEqPio.IfFlagRecv.OperateCancel)
                        {
                            m_devEqPio.IfFlagRecv.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, "Operator Cancel Alarm Reset");
                        }
                        else
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset");
                        }
                        EqpAlarm.ResetAll();
                        //EqpAlarm.Reset(AlarmId);
                        AlarmId = 0;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = ReturnSeqNo;
                    }
                    break;
            }

            SeqNo = seqNo;
            return rv;
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

        private bool IsAntiDropUnlock()
        {
            bool anti_drop_unlock = true;
            try
            {
                anti_drop_unlock &= m_devAntiDropFront.IsValid ? m_devAntiDropFront.GetUnlock() : true;
                anti_drop_unlock &= m_devAntiDropRear.IsValid ? m_devAntiDropRear.GetUnlock() : true;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return anti_drop_unlock;
        }

        private void ESSignalCheck()
        {
            try
            {
                if (m_devEqPio.PioComm.IsGo() || AppConfig.Instance.Simulation.MY_DEBUG)
                {
                    //if (m_devEqPio.GetUlReq())
                    {
                        if (m_devEqPio.GetEs() == false && m_devEqPio.IfFlagRecv.ES == false)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : ES Signal OFF");
                            m_devEqPio.IfFlagRecv.ES = true; // Hoist Stop
                        }
                        else if (m_devEqPio.GetEs() && m_devEqPio.IfFlagRecv.ES)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : ES Signal ON");
                            m_devEqPio.IfFlagRecv.ES = false; // Hoist Release
                        }

                        if (AppConfig.Instance.ProjectType == enProjectType.ESWIN)
                        {
                            if (m_devEqPio.GetHandOffAvailable() == false && m_devEqPio.IfFlagRecv.HO == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal OFF");
                                m_devEqPio.IfFlagRecv.HO = true;
                            }
                            else if (m_devEqPio.GetHandOffAvailable() && m_devEqPio.IfFlagRecv.HO)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : HO Signal ON");
                                m_devEqPio.IfFlagRecv.HO = false;
                            }
                        }
                    }
                }
                else
                {
                    SequenceInterfaceLog.WriteLog(FuncName, "PORT -> VHL : GO Signal OFF");
                    m_devEqPio.IfFlagRecv.ES = true; // Hoist Stop
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion
    }

    public class SeqSPLIF : XSeqFunc
    {
        private const string FuncName = "[SeqSPLIF]";

        #region Field
        private DevEqPIO m_devEqPio = null;
        private DataItem_PIODevice m_SPLPioDevice = new DataItem_PIODevice();
        private XTimer m_Timer = new XTimer("SeqSPLIF");
        private bool m_Simul = false;
        private bool m_ESCheckStart = false;

        private AlarmData m_ALM_SplIFNodeTypeMismatchError = null;
        private AlarmData m_ALM_SplIFPioDeviceNotExistError = null;
        #endregion

        #region Constructor
        public SeqSPLIF()
        {
            this.SeqName = $"SeqSPLIF";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_Simul = AppConfig.Instance.Simulation.IO;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInterface;

            m_ALM_SplIFNodeTypeMismatchError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SPLIF", "SPL Sequence", "In Node Type Mismatch Alarm");
            m_ALM_SplIFPioDeviceNotExistError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "SPLIF", "SPL Sequence", "PIO Device Not Exist Alarm");
        }
        public override void SeqAbort()
        {
            if (this.scm.Ing)
            {
                if (m_devEqPio.IsValid) m_devEqPio.SeqAbort();
                this.scm.Abort = true;
            }
            if (AlarmId > 0)
            {
                SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset (Abort status)");
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            m_ESCheckStart = false;
            this.InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("[IfFlagSpl.Start TRUE] => Loading Start Condition Check"));
                this.SeqCaseMemoLists.Add(20, string.Format("PIO Connection"));
                this.SeqCaseMemoLists.Add(100, string.Format("[SPL<-VHL] : CS ON, Valid ON"));
                this.SeqCaseMemoLists.Add(110, string.Format("[SPL->VHL] : Start ON after MoveReq ON"));
                this.SeqCaseMemoLists.Add(120, string.Format("[SPL->VHL] : Insert Ready ON after MoveStart ON"));
                this.SeqCaseMemoLists.Add(130, string.Format("[SPL->VHL] : Insert Request OFF"));
                this.SeqCaseMemoLists.Add(140, string.Format("[SPL<-VHL] : Move Complete ON"));
                this.SeqCaseMemoLists.Add(150, string.Format("[SPL->VHL] : Start OFF after PIO Reset"));
                this.SeqCaseMemoLists.Add(200, string.Format("[SPL<-VHL] : CS ON, Valid ON"));
                this.SeqCaseMemoLists.Add(210, string.Format("[SPL->VHL] : Start ON after MoveReq ON"));
                this.SeqCaseMemoLists.Add(220, string.Format("[SPL->VHL] : Export Ready ON after MoveStart ON"));
                this.SeqCaseMemoLists.Add(230, string.Format("[SPL->VHL] : Export Request OFF"));
                this.SeqCaseMemoLists.Add(240, string.Format("[SPL<-VHL] : Move Complete ON"));
                this.SeqCaseMemoLists.Add(250, string.Format("[SPL->VHL] : Start OFF"));
                this.SeqCaseMemoLists.Add(300, string.Format("Time Delay (100msec) after PIO Reset"));
                this.SeqCaseMemoLists.Add(310, string.Format("PIO Disconnection"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset or Operate Cancel Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;

            int seqNo = SeqNo;
            int rv = -1;
            int rv1 = -1;

            bool eqpStateReady = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            //eqpStateReady &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            if (m_ESCheckStart) ESSignalCheck();

            switch (seqNo)
            {
                case 0:
                    {
                        bool loadReady = true;
                        bool pioReady = true;
                        if (m_devEqPio.IsValid)
                        {
                            loadReady &= m_devEqPio.IfFlagSpl.StartReq;
                            loadReady &= !m_devEqPio.IfFlagSpl.Busy;
                            loadReady &= !m_devEqPio.IfFlagSpl.OnIng;
                            loadReady &= !m_devEqPio.IfFlagSpl.Complete;
                            loadReady &= eqpStateReady;

                            pioReady &= m_devEqPio.PioComm.IsOpen();
                            pioReady &= m_devEqPio.PioComm.IsGo();
                        }

                        if (loadReady)
                        {
                            this.scm.Start = true;
                            double leftBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            DataItem_Node toNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsNearNode(leftBcr, rightBcr);
                            if (DatabaseHandler.Instance.DictionaryPIODevice.ContainsKey(toNode.NodeID))
                                m_SPLPioDevice = DatabaseHandler.Instance.DictionaryPIODevice[toNode.NodeID];
                            else m_SPLPioDevice = null;

                            if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL PIO Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 300;
                                seqNo = 1000;
                            }
                            else if (toNode.Type != NodeType.LifterIn)
                            {
                                AlarmId = m_ALM_SplIFNodeTypeMismatchError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL PIO Interface Node Type Mismatch [{0}!={1}]", toNode.Type, NodeType.LifterIn));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 300;
                                seqNo = 1000;
                            }
                            else if (m_SPLPioDevice == null)
                            {
                                AlarmId = m_ALM_SplIFPioDeviceNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL PIO Device Not Exist Alarm [Node:{0}]", toNode.ToString()));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 300;
                                seqNo = 1000;
                            }
                            else if (pioReady == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Not Ready"));
                                seqNo = 20;
                            }
                            else
                            {
                                m_ESCheckStart = true;
                                if (m_devEqPio.IsValid)
                                {
                                    m_devEqPio.IfFlagSpl.Reset();
                                    m_devEqPio.ResetPIO();
                                }
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Load PIO Start"));
                                seqNo = 100;
                            }
                        }
                    }
                    break;

                case 20:
                    {
                        int id = m_SPLPioDevice.PIOID;
                        int ch = m_SPLPioDevice.PIOCH;
                        if (m_devEqPio.IsValid)
                            rv1 = m_devEqPio.SetChannelId(id, ch, true);
                        else rv1 = 0;
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL_PIO [{0}, {1}] Set Channel OK", id, ch));
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 100:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiREADY.SetDi(false);
                        }

                        bool signal_abnormal = false;
                        signal_abnormal |= m_devEqPio.GetSplPioStart();
                        signal_abnormal |= m_devEqPio.GetSplInsertReq();
                        signal_abnormal |= m_devEqPio.GetSplExportReq();

                        if (signal_abnormal == false)
                        {
                            m_devEqPio.SetSplValid(true);
                            m_devEqPio.SetSplCs1(true); // 진입 1 사용
                            m_devEqPio.SetSplCs2(false); // 진출 2 사용
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : VALID Signal ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : CS1 Signal ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : CS2 Signal OFF");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            if (m_devEqPio.GetSplPioStart()) SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : PIO START Signal Abnormal ON");
                            if (m_devEqPio.GetSplInsertReq()) SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : INSERT Signal Abnormal ON");
                            if (m_devEqPio.GetSplExportReq()) SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : EXPORT Signal Abnormal ON");

                            AlarmId = m_devEqPio.ALM_Load_SignalAlreadyOn.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Already SPL PIO Signal On!. Check PIO Log - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 110:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiHOAVBL.SetDi(true);
                            m_devEqPio.DiLDREQ.SetDi(true);
                            m_devEqPio.DiES.SetDi(true);
                        }
                        bool spl_pio_start = true;
                        spl_pio_start &= m_devEqPio.GetSplPioStart();
                        spl_pio_start &= m_devEqPio.GetSplInsertReq();
                        spl_pio_start &= m_devEqPio.GetSplES();
                        if (spl_pio_start)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Start ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Insert Request ON");

                            m_devEqPio.SetSplMoveRequest(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : Move Request ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        //else if (m_devEqPio.GetSplES() == false)
                        //{
                        // m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                        //AlarmId = m_devEqPio.ALM_Spl_ESSignalOff_Alarm.ID;
                        //SequenceInterfaceLog.WriteLog(FuncName, string.Format("ES Singal OFF Alarm - SPL Not Ready"));
                        //EqpAlarm.Set(AlarmId);
                        //ReturnSeqNo = 300; // IF 종료 위치로 가자....
                        //seqNo = 1000;
                        //}
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioSplStartOnTimeOut)
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_StartOn_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Start && Insert Request Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 120:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiREADY.SetDi(true);
                            m_devEqPio.DiVA.SetDi(false);
                        }
                        bool ready = m_devEqPio.GetSplInsertReady();
                        ready &= m_devEqPio.GetSplBusy() == false;
                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Insert Ready Signal ON");

                            m_devEqPio.SetSplMoveStart(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : Move Start Signal ON");

                            m_devEqPio.IfFlagSpl.Busy = true; //SeqVehicleMove 동작을 시작해라....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioSplInsertReadyTimeOut)
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_InsertReady_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL Insert Ready ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 130:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiLDREQ.SetDi(false);
                        }
                        bool request_off = m_devEqPio.GetSplInsertReq() == false; // Sensor가 감지 되었군...
                        if (request_off || m_devEqPio.IfFlagSpl.Complete)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Insert Request Signal OFF");

                            m_devEqPio.IfFlagSpl.OnIng = true; //센서가 감지 되었다고 하네....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioSplInsertReqOffTimeOut)
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_InsertRequestOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL Insert Request OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 140:
                    {
                        bool complete = m_devEqPio.IfFlagSpl.Complete;
                        if (complete)
                        {
                            m_devEqPio.IfFlagSpl.Busy = false;
                            m_devEqPio.IfFlagSpl.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL IN Complete"));

                            m_devEqPio.SetSplMoveStart(false);
                            m_devEqPio.SetSplMoveComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : Move Start Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 150;
                        }
                        else if (m_devEqPio.IfFlagSpl.OperateCancel)
                        {
                            m_devEqPio.IfFlagSpl.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SeqVehicleMove not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 300; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 150:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiHOAVBL.SetDi(false);
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiREADY.SetDi(false);
                        }
                        bool finished = true;
                        finished &= m_devEqPio.GetSplPioStart() == false;
                        finished &= m_devEqPio.GetSplInsertReq() == false;
                        finished &= m_devEqPio.GetSplInsertReady() == false;
                        if (finished)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Start OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Request OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Ready OFF");

                            m_devEqPio.ResetPIO();
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : PIO Signal All OFF");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 200;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            m_devEqPio.IfFlagSpl.Abort = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_PioSignalOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Signal All OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 200:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;

                        // SPL OUT 을 시작하자 !
                        m_devEqPio.SetSplValid(true);
                        m_devEqPio.SetSplCs1(false); // 진입 1사용
                        m_devEqPio.SetSplCs2(true); // 진출 2사용
                        SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : VALID Signal ON");
                        SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : CS1 Signal OFF");
                        SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : CS2 Signal ON");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiHOAVBL.SetDi(true);
                            m_devEqPio.DiULREQ.SetDi(true);
                        }
                        bool spl_pio_start = true;
                        spl_pio_start &= m_devEqPio.GetSplPioStart();
                        spl_pio_start &= m_devEqPio.GetSplExportReq();

                        if (spl_pio_start)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Start ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Request ON");

                            m_devEqPio.SetSplMoveRequest(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : Move Request ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 220;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioSplStartOnTimeOut)
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_StartOn_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Start && Export Request Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }

                    }
                    break;

                case 220:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS0.SetDi(true);
                            m_devEqPio.DiVA.SetDi(false);
                        }

                        bool ready = m_devEqPio.GetSplExportReady();
                        ready &= m_devEqPio.GetSplBusy() == false;
                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Ready Signal ON");

                            m_devEqPio.SetSplMoveStart(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : Move Start Signal ON");

                            m_devEqPio.IfFlagSpl.Busy = true; //SeqVehicleMove 동작을 시작해라....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 230;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioSplExportReadyTimeOut)
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_ExportReady_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL Export Ready ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 230:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiULREQ.SetDi(false);
                        }

                        bool complete = IsSplInArea() == false; // m_devEqPio.IfFlagSpl.Complete;
                        bool request_off = m_devEqPio.GetSplExportReq() == false; // Sensor가 감지 되었군...
                        if (request_off || complete)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Request Signal OFF");

                            m_devEqPio.IfFlagSpl.OnIng = false; //센서가 OFF 되었다고 하네....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 240;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioSplExportReqOffTimeOut)
                        {
                            m_devEqPio.IfFlagSpl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_ExportRequestOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL Export Request OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 240:
                    {
                        bool complete = IsSplInArea() == false; // m_devEqPio.IfFlagSpl.Complete;
                        if (complete)
                        {
                            m_devEqPio.IfFlagSpl.Busy = false;
                            m_devEqPio.IfFlagSpl.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL OUT Complete"));

                            m_devEqPio.SetSplMoveStart(false);
                            m_devEqPio.SetSplMoveComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : Move Start Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 250;
                        }
                        else if (m_devEqPio.IfFlagSpl.OperateCancel)
                        {
                            m_devEqPio.IfFlagSpl.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SeqVehicleMove not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 300; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 250:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiHOAVBL.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(false);
                            m_devEqPio.DiVS0.SetDi(false);
                        }
                        bool finished = true;
                        finished &= m_devEqPio.GetSplPioStart() == false;
                        finished &= m_devEqPio.GetSplExportReq() == false;
                        finished &= m_devEqPio.GetSplExportReady() == false;
                        if (finished)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Start OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Request OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : Export Ready OFF");

                            m_ESCheckStart = false;
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 300;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            m_devEqPio.IfFlagSpl.Abort = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Spl_PioSignalOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Signal All OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 300; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 300:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 100) break; // 끈기전 delay가 필요하네...
                        m_ESCheckStart = false;
                        ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideDontControl = false;

                        // PIO Comm을 닫고 마무리 하자...!
                        m_devEqPio.IfFlagSpl.Reset();
                        m_devEqPio.ResetPIO();
                        SequenceInterfaceLog.WriteLog(FuncName, "SPL <- VHL : PIO Signal All OFF");

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 310;
                    }
                    break;

                case 310:
                    {
                        int id = 0;
                        int ch = 0;
                        rv1 = m_devEqPio.SetChannelId(id, ch, true);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL_PIO [{0}, {1}] Set Channel OK", id, ch));

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL PIO Finished"));
                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SPL_PIO [{0}, {1}] Set Channel Alarm - {0}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset || m_devEqPio.IfFlagSpl.OperateCancel)
                    {
                        if (m_devEqPio.IfFlagSpl.OperateCancel)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "Operator Cancel Alarm Reset");
                            m_devEqPio.IfFlagSpl.OperateCancel = false;
                        }
                        else
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset");
                        }

                        EqpAlarm.Reset(AlarmId);
                        AlarmId = 0;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = ReturnSeqNo;
                    }
                    break;
            }

            SeqNo = seqNo;
            return rv;
        }

        private void ESSignalCheck()
        {
            try
            {
                if (m_devEqPio.PioComm.IsGo())
                {
                    if (m_devEqPio.GetSplES() == false && m_devEqPio.IfFlagSpl.ES == false)
                    {
                        SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : ES Signal OFF");
                        m_devEqPio.IfFlagSpl.ES = true; // Hoist Stop
                    }
                    else if (m_devEqPio.GetSplES() && m_devEqPio.IfFlagSpl.ES)
                    {
                        SequenceInterfaceLog.WriteLog(FuncName, "SPL -> VHL : ES Signal ON");
                        m_devEqPio.IfFlagSpl.ES = false; // Hoist Release
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private bool IsSplInArea()
        {
            bool inArea = false;
            try
            {
                int fromNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID;
                int toNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToNodeID;
                if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(fromNode))
                {
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.LifterIn;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.Lifter;
                }
                if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(toNode))
                {
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[toNode].Type == NodeType.LifterIn;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[toNode].Type == NodeType.Lifter;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[toNode].Type == NodeType.LifterOut;
                }
                inArea |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.SPL;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return inArea;
        }
        #endregion
    }

    public class SeqMTLIF : XSeqFunc
    {
        private const string FuncName = "[SeqMTLIF]";

        #region Field
        private DevEqPIO m_devEqPio = null;
        private DataItem_PIODevice m_MTLPioDevice = new DataItem_PIODevice();
        private XTimer m_Timer = new XTimer("SeqMTLIF");
        private bool m_Simul = false;
        private bool m_ESCheckStart = false;

        private AlarmData m_ALM_MtlIFNodeTypeMismatchError = null;
        private AlarmData m_ALM_MtlIFPioDeviceNotExistError = null;
        #endregion

        #region Constructor
        public SeqMTLIF()
        {
            this.SeqName = $"SeqMTLIF";

            m_devEqPio = DevicesManager.Instance.DevEqpPIO;
            m_Simul = AppConfig.Instance.Simulation.IO;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInterface;

            m_ALM_MtlIFNodeTypeMismatchError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "MTLIF", "MTL", "In Node Type Mismatch Alarm");
            m_ALM_MtlIFPioDeviceNotExistError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "MTLIF", "MTL", "Database PIO Device Null Alarm");
        }
        public override void SeqAbort()
        {
            if (this.scm.Ing)
            {
                m_devEqPio.SeqAbort();
                this.scm.Abort = true;
            }
            if (AlarmId > 0)
            {
                SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset (Abort status)");
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            this.InitSeq();
            m_ESCheckStart = false;
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("[IfFlagMtl.Start TRUE] => Loading Start Condition Check"));
                this.SeqCaseMemoLists.Add(10, string.Format("PIO Connection"));
                this.SeqCaseMemoLists.Add(100, string.Format("[MTL<-VHL] : CS ON, Valid ON"));
                this.SeqCaseMemoLists.Add(110, string.Format("[MTL->VHL] : Start ON after MoveReq ON"));
                this.SeqCaseMemoLists.Add(120, string.Format("[MTL->VHL] : Export Ready ON after MoveStart ON"));
                this.SeqCaseMemoLists.Add(130, string.Format("[MTL<-VHL] : Move Complete ON"));
                this.SeqCaseMemoLists.Add(140, string.Format("[MTL->VHL] : Start OFF"));
                this.SeqCaseMemoLists.Add(200, string.Format("[MTL<-VHL] : CS ON, Valid ON"));
                this.SeqCaseMemoLists.Add(210, string.Format("[MTL->VHL] : Export Ready ON"));
                this.SeqCaseMemoLists.Add(220, string.Format("[MTL<-VHL] : Move Complete ON"));
                this.SeqCaseMemoLists.Add(230, string.Format("[MTL->VHL] : Export Ready OFF"));
                this.SeqCaseMemoLists.Add(300, string.Format("Time Delay (100msec) after PIO Reset"));
                this.SeqCaseMemoLists.Add(310, string.Format("PIO Disconnection"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset or Operate Cancel Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Override
        public override int Do()
        {
            if (this.scm.Pause) return -1;

            int seqNo = SeqNo;
            int rv = -1;
            int rv1 = -1;

            bool eqpStateReady = EqpStateManager.Instance.OpMode == OperateMode.Auto;
            //eqpStateReady &= EqpStateManager.Instance.RunMode == EqpRunMode.Start;
            if (m_ESCheckStart) ESSignalCheck();

            switch (seqNo)
            {
                case 0:
                    {
                        bool loadReady = true;
                        loadReady &= m_devEqPio.IfFlagMtl.StartReq;
                        loadReady &= !m_devEqPio.IfFlagMtl.Busy;
                        loadReady &= !m_devEqPio.IfFlagMtl.OnIng;
                        loadReady &= !m_devEqPio.IfFlagMtl.Complete;
                        loadReady &= eqpStateReady;

                        if (loadReady)
                        {
                            this.scm.Start = true;
                            double leftBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.LeftBcr;
                            double rightBcr = ProcessDataHandler.Instance.CurVehicleStatus.CurrentBcrStatus.RightBcr;
                            DataItem_Node toNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.IsNearNode(leftBcr, rightBcr);

                            if (DatabaseHandler.Instance.DictionaryPIODevice.ContainsKey(toNode.NodeID))
                                m_MTLPioDevice = DatabaseHandler.Instance.DictionaryPIODevice[toNode.NodeID];
                            else m_MTLPioDevice = null;

                            if (ProcessDataHandler.Instance.CurTransferCommand.IsValid == false)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL PIO Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 500;
                                seqNo = 1000;
                            }
                            else if (toNode.Type != NodeType.MTLIn && toNode.Type != NodeType.MTL)
                            {
                                AlarmId = m_ALM_MtlIFNodeTypeMismatchError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL PIO Interface Node Type Mismatch [{0}!={1}]", toNode.Type, NodeType.MTLIn));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 500;
                                seqNo = 1000;
                            }
                            else if (m_MTLPioDevice == null)
                            {
                                AlarmId = m_ALM_MtlIFPioDeviceNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL PIO Device Not Exist Alarm [Node:{0}]", toNode.ToString()));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 500;
                                seqNo = 1000;
                            }
                            else if (m_devEqPio.PioComm.IsGo() == false || m_devEqPio.PioComm.IsOpen() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Not Ready"));
                                seqNo = 10;
                            }
                            else
                            {
                                m_ESCheckStart = true;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Load PIO Start"));

                                m_devEqPio.IfFlagMtl.Reset();
                                m_devEqPio.ResetPIO();
                                if (m_devEqPio.IfFlagMtl.CS == 1)
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("Rail In Mode"));
                                    seqNo = 300;
                                }
                                else if (m_devEqPio.IfFlagMtl.CS == 2)
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("Rail Out Mode"));
                                    seqNo = 100;
                                }
                                else if (m_devEqPio.IfFlagMtl.CS == 3)
                                {
                                    ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideDontControl = false;
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL Pass Mode"));
                                    seqNo = 200;
                                }
                                else
                                {
                                    SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL CS No-Setting Status (0) => Out(1), Pass(3)"));
                                }
                            }
                        }
                    }
                    break;

                case 10:
                    {
                        int id = m_MTLPioDevice.PIOID;
                        int ch = m_MTLPioDevice.PIOCH;
                        rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL_PIO [{0}, {1}] Set Channel OK", id, ch));
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 100:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS1.SetDi(false);
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(false);
                            m_devEqPio.DiES.SetDi(true);
                        }

                        bool signal_abnormal = false;
                        signal_abnormal |= m_devEqPio.GetMtlPioStart();
                        signal_abnormal |= m_devEqPio.GetMtlInsertReq();
                        signal_abnormal |= m_devEqPio.GetMtlExportReq();

                        if (signal_abnormal == false)
                        {
                            bool cs1 = (m_devEqPio.IfFlagMtl.CS & (0x01 << 0)) == 0x01;
                            bool cs2 = (m_devEqPio.IfFlagMtl.CS & (0x01 << 1)) == 0x02;
                            m_devEqPio.SetCs1(cs1);
                            m_devEqPio.SetCs2(cs2);
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL <- VHL : CS1 Signal {0}", cs1 ? "ON" : "OFF"));
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL <- VHL : CS2 Signal {0}", cs2 ? "ON" : "OFF"));

                            m_devEqPio.SetMtlValid(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : VALID Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 110;
                        }
                        else
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            if (m_devEqPio.GetMtlPioStart()) SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : PIO START Signal Abnormal ON");
                            if (m_devEqPio.GetMtlInsertReq()) SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : INSERT Signal Abnormal ON");
                            if (m_devEqPio.GetMtlExportReq()) SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : EXPORT Signal Abnormal ON");

                            AlarmId = m_devEqPio.ALM_Load_SignalAlreadyOn.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Already MTL PIO Signal On!. Check PIO Log - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 110:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS1.SetDi(true);
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(true);
                        }
                        bool mtl_pio_start = true;
                        mtl_pio_start &= m_devEqPio.GetMtlPioStart();
                        mtl_pio_start &= m_devEqPio.GetMtlExportReq();
                        mtl_pio_start &= m_devEqPio.GetMtlES();

                        if (mtl_pio_start)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Start ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Export Request ON");

                            m_devEqPio.SetMtlMoveRequest(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : Move Request ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                        else if (m_devEqPio.GetMtlES() == false)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_ESSignalOff_Alarm.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("ES Singal OFF Alarm - MTL Not Ready"));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlStartOnTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_StartOn_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Start && Insert Request Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 120:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVA.SetDi(false);
                            m_devEqPio.DiVS0.SetDi(true);
                        }

                        bool ready = m_devEqPio.GetMtlExportReady();
                        ready &= m_devEqPio.GetMtlBusy() == false;
                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Export Ready Signal ON");

                            m_devEqPio.SetMtlMoveStart(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : Move Start Signal ON");

                            m_devEqPio.IfFlagMtl.Busy = true; //SeqVehicleMove 동작을 시작해라....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 130;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlExportReadyTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_ExportReady_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL Export Ready ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 130:
                    {
                        bool complete = m_devEqPio.IfFlagMtl.Complete;
                        if (complete)
                        {
                            m_devEqPio.IfFlagMtl.Busy = false;
                            m_devEqPio.IfFlagMtl.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL IN Complete"));

                            m_devEqPio.SetMtlMoveStart(false);
                            m_devEqPio.SetMtlMoveComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : Move Start Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 140;
                        }
                        else if (m_devEqPio.IfFlagMtl.OperateCancel)
                        {
                            m_devEqPio.IfFlagMtl.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SeqVehicleMove not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 500; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 140:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS1.SetDi(false);
                            m_devEqPio.DiVS0.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(false);
                        }

                        bool finished = true;
                        finished &= m_devEqPio.GetMtlPioStart() == false;
                        finished &= m_devEqPio.GetMtlExportReq() == false;
                        finished &= m_devEqPio.GetMtlExportReady() == false;
                        if (finished)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Start OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Export Request OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Export Ready OFF");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 500;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlExportReqOffTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Abort = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_PioSignalOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Signal All OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 200:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;

                        // MTL PASS 을 시작하자 !
                        bool cs1 = (m_devEqPio.IfFlagMtl.CS & (0x01 << 0)) == 0x01;
                        bool cs2 = (m_devEqPio.IfFlagMtl.CS & (0x01 << 1)) == 0x02;
                        m_devEqPio.SetCs1(cs1);
                        m_devEqPio.SetCs2(cs2);
                        SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL <- VHL : CS1 Signal {0}", cs1 ? "ON" : "OFF"));
                        SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL <- VHL : CS2 Signal {0}", cs2 ? "ON" : "OFF"));

                        m_devEqPio.SetMtlValid(true);
                        SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : VALID Signal ON");
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        bool ready = m_devEqPio.GetMtlExportReady();
                        ready &= m_devEqPio.GetMtlBusy() == false;
                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Export Ready Signal ON");

                            m_devEqPio.IfFlagMtl.Busy = true; //SeqVehicleMove 동작을 시작해라....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 220;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlExportReadyTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_ExportReady_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL Export Ready ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 220:
                    {
                        bool complete = IsMtlInArea() == false; //m_devEqPio.IfFlagMtl.Complete;
                        if (complete)
                        {
                            m_devEqPio.IfFlagMtl.Busy = false;
                            m_devEqPio.IfFlagMtl.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL OUT Complete"));

                            m_devEqPio.SetMtlMoveComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 230;
                        }
                        else if (m_devEqPio.IfFlagMtl.OperateCancel)
                        {
                            m_devEqPio.IfFlagMtl.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SeqVehicleMove not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 500; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 230:
                    {
                        bool finished = true;
                        finished &= m_devEqPio.GetMtlExportReady() == false;
                        if (finished)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Export Ready OFF");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 500;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            m_devEqPio.IfFlagMtl.Abort = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_PioSignalOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Signal All OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 300:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS1.SetDi(false);
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiULREQ.SetDi(false);
                            m_devEqPio.DiES.SetDi(true);
                        }

                        bool signal_abnormal = false;
                        //signal_abnormal |= m_devEqPio.GetMtlPioStart();
                        //signal_abnormal |= m_devEqPio.GetMtlInsertReq();
                        //signal_abnormal |= m_devEqPio.GetMtlExportReq();

                        if (signal_abnormal == false)
                        {
                            bool cs1 = (m_devEqPio.IfFlagMtl.CS & (0x01 << 0)) == 0x01;
                            bool cs2 = (m_devEqPio.IfFlagMtl.CS & (0x01 << 1)) == 0x02;
                            m_devEqPio.SetCs1(cs1);
                            m_devEqPio.SetCs2(cs2);
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL <- VHL : CS1 Signal {0}", cs1 ? "ON" : "OFF"));
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL <- VHL : CS2 Signal {0}", cs2 ? "ON" : "OFF"));

                            m_devEqPio.SetMtlValid(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : VALID Signal ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 310;
                        }
                        else
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            if (m_devEqPio.GetMtlPioStart()) SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : PIO START Signal Abnormal ON");
                            if (m_devEqPio.GetMtlInsertReq()) SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : INSERT Signal Abnormal ON");
                            if (m_devEqPio.GetMtlExportReq()) SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : EXPORT Signal Abnormal ON");

                            AlarmId = m_devEqPio.ALM_Load_SignalAlreadyOn.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Already MTL PIO Signal On!. Check PIO Log - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 310:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS1.SetDi(true);
                            m_devEqPio.DiLDREQ.SetDi(true);
                            m_devEqPio.DiULREQ.SetDi(true);
                        }
                        bool mtl_pio_start = true;
                        mtl_pio_start &= m_devEqPio.GetMtlPioStart();
                        mtl_pio_start &= m_devEqPio.GetMtlInsertReq();
                        mtl_pio_start &= m_devEqPio.GetMtlES();

                        if (mtl_pio_start)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Start ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Insert Request ON");

                            m_devEqPio.SetMtlMoveRequest(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : Move Request ON");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 320;
                        }
                        else if (m_devEqPio.GetMtlES() == false)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_ESSignalOff_Alarm.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("ES Singal OFF Alarm - MTL Not Ready"));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlStartOnTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_StartOn_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Start && Insert Request Singal ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 320:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVA.SetDi(false);
                            m_devEqPio.DiREADY.SetDi(true);
                        }

                        bool ready = m_devEqPio.GetMtlInsertReady();
                        ready &= m_devEqPio.GetMtlBusy() == false;
                        if (ready)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Insert Ready Signal ON");

                            m_devEqPio.SetMtlMoveStart(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : Move Start Signal ON");

                            m_devEqPio.IfFlagMtl.Busy = true; //SeqVehicleMove 동작을 시작해라....
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 330;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlInsertReadyTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Cancel = true; // SeqVehicle Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_ExportReady_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL Insert Ready ON Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;

                case 330:
                    {
                        bool complete = IsMtlInArea() == false; // m_devEqPio.IfFlagSpl.Complete;
                        bool request_off = m_devEqPio.GetMtlInsertReq() == false; // Sensor가 감지 되었군...
                        if (complete || request_off)
                        {
                            m_devEqPio.IfFlagMtl.Busy = false;
                            m_devEqPio.IfFlagMtl.Complete = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL Out Complete"));

                            m_devEqPio.SetMtlMoveStart(false);
                            m_devEqPio.SetMtlMoveComplete(true);
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : Move Start Signal OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : COMPLETE Signal ON");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 340;
                        }
                        else if (m_devEqPio.IfFlagMtl.OperateCancel)
                        {
                            m_devEqPio.IfFlagMtl.OperateCancel = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("SeqVehicleMove not Complete , Operator Cancel"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 500; // IF 종료 위치로 가자....    
                        }
                    }
                    break;

                case 340:
                    {
                        if (AppConfig.Instance.Simulation.MY_DEBUG)
                        {
                            m_devEqPio.DiVS1.SetDi(false);
                            m_devEqPio.DiLDREQ.SetDi(false);
                            m_devEqPio.DiREADY.SetDi(false);
                        }

                        bool finished = IsMtlInArea() == false; // m_devEqPio.IfFlagSpl.Complete;
                        finished &= m_devEqPio.GetMtlPioStart() == false;
                        finished &= m_devEqPio.GetMtlInsertReq() == false;
                        finished &= m_devEqPio.GetMtlInsertReady() == false;
                        if (finished)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Start OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Insert Request OFF");
                            SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : Insert Ready OFF");

                            StartTicks = XFunc.GetTickCount();
                            seqNo = 500;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > SetupManager.Instance.SetupPIO.PioMtlInsertReqOffTimeOut)
                        {
                            m_devEqPio.IfFlagMtl.Abort = true; // SeqVehicleMove Sequence에서 이걸 보고 OperatorCancel 하도록 하자....
                            AlarmId = m_devEqPio.ALM_Mtl_PioSignalOff_Timeout.ID;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Signal All OFF Timeout Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 500; // IF 종료 위치로 가자....
                            seqNo = 1000;
                        }
                    }
                    break;
                case 500:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 100) break; // 끈기전 delay가 필요하네...
                        m_ESCheckStart = false;

                        // PIO Comm을 닫고 마무리 하자...!
                        m_devEqPio.ResetIfSend();
                        m_devEqPio.ResetPIO();
                        SequenceInterfaceLog.WriteLog(FuncName, "MTL <- VHL : PIO Signal All OFF");

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 510;
                    }
                    break;

                case 510:
                    {
                        int id = 0;
                        int ch = 0;
                        rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL_PIO [{0}, {1}] Set Channel OK", id, ch));

                            ProcessDataHandler.Instance.CurVehicleStatus.ObsStatus.OverrideDontControl = false;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL PIO Finished"));
                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("MTL_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 1000:
                    if (IsPushedSwitch.IsAlarmReset || m_devEqPio.IfFlagMtl.OperateCancel)
                    {
                        if (m_devEqPio.IfFlagMtl.OperateCancel)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "Operator Cancel Alarm Reset");
                            m_devEqPio.IfFlagMtl.OperateCancel = false;
                        }
                        else
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "Alarm Reset");
                        }
                        EqpAlarm.Reset(AlarmId);
                        AlarmId = 0;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = ReturnSeqNo;
                    }
                    break;
            }

            SeqNo = seqNo;
            return rv;
        }

        private void ESSignalCheck()
        {
            try
            {
                if (m_devEqPio.PioComm.IsGo())
                {
                    if (m_devEqPio.GetMtlES() == false && m_devEqPio.IfFlagMtl.ES == false)
                    {
                        SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : ES Signal OFF");
                        m_devEqPio.IfFlagMtl.ES = true; // Hoist Stop
                    }
                    else if (m_devEqPio.GetMtlES() && m_devEqPio.IfFlagMtl.ES)
                    {
                        SequenceInterfaceLog.WriteLog(FuncName, "MTL -> VHL : ES Signal ON");
                        m_devEqPio.IfFlagMtl.ES = false; // Hoist Release
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private bool IsMtlInArea()
        {
            bool inArea = false;
            try
            {
                int fromNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.FromNodeID;
                int toNode = ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.ToNodeID;
                if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(fromNode))
                {
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.MTLIn;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.MTL;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.MTLOut;
                }
                if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(toNode))
                {
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[toNode].Type == NodeType.MTLIn;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[toNode].Type == NodeType.MTL;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[toNode].Type == NodeType.MTLOut;
                }
                inArea |= ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.Type == LinkType.MTL;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return inArea;
        }
        #endregion

    }

    public class SeqAutoDoorControl : XSeqFunc
    {
        public static readonly string FuncName = "[SeqAutoDoorControl]";

        #region Field
        private DevEqPIO m_devEqPio = null;
        private DataItem_PIODevice m_ADPioDevice = new DataItem_PIODevice();

        private TransferCommand m_CurTransferCommand = null;
        private VehicleStatus m_CurVehicleStatus = null;

        private bool m_AutoDoorStart = false;
        private List<int> m_FullPath = new List<int>();
        private AutoDoorControl m_CurAutoDoor = new AutoDoorControl();
        private List<AutoDoorControl> m_AutoDoorControl = new List<AutoDoorControl>();

        private bool m_AutoDoorRunning = false;
        private bool m_ReceivedPermit = false;
        private bool m_PassRequest = false;
        private bool m_PassPermitCheck = false;
        private bool m_ReceivedTimeover = false;
        private UInt32 m_PermitWaitTime = 0;
        private int m_PermitRetry = 0;
        private int m_ConnectionRetry = 0;

        private AlarmData m_ALM_ADIFNodeTypeMismatchError = null;
        private AlarmData m_ALM_ADIFPioDeviceNotExistError = null;
        private AlarmData m_ALM_ADPermitWait = null;

        private bool m_AutoDoorNewStart = true;
        #endregion

        #region Properties
        public bool AutoDoorRunning { get { return m_AutoDoorRunning; } set { m_AutoDoorRunning = value; } }
        public bool ReceivedPermit { get { return m_ReceivedPermit; } set { m_ReceivedPermit = value; } }
        public bool PassRequest { get { return m_PassRequest; } set { m_PassRequest = value; } }
        public bool PassPermitCheck { get { return m_PassPermitCheck; } set { m_PassPermitCheck = value; } }
        public bool ReceivedTimeover { get { return m_ReceivedTimeover; } set { m_ReceivedTimeover = value; } }
        #endregion

        #region Constructor
        public SeqAutoDoorControl()
        {
            this.SeqName = "SeqAutoDoorControl";
            m_devEqPio = DevicesManager.Instance.DevEqpPIO;

            AddCaseMemo();
            this.TaskLayer = (int)enTaskLayer.layerInterface;

            m_ALM_ADIFNodeTypeMismatchError = AlarmListProvider.Instance.NewAlarm(AlarmCode.AttentionFlags, true, "AutoDoorIF", "Auto Door", "In Node Type Mismatch Alarm");
            m_ALM_ADIFPioDeviceNotExistError = AlarmListProvider.Instance.NewAlarm(AlarmCode.ParameterControlError, true, "AutoDoorIF", "Auto Door", "Database PIO Device Null Alarm");
            m_ALM_ADPermitWait = AlarmListProvider.Instance.NewAlarm(AlarmCode.EquipmentStatusWarning, false, "AutoDoorIF", "Auto Door", "Permit Wait Warning");
        }
        public override void SeqAbort()
        {
            if (AlarmId != 0)
            {
                EqpAlarm.Reset(AlarmId);
                AlarmId = 0;
            }
            m_FullPath.Clear();
            m_AutoDoorStart = false;
            m_AutoDoorRunning = false;
            InitSeq();
        }
        private void AddCaseMemo()
        {
            try
            {
                this.SeqCaseMemoLists.Add(0, string.Format("[IfFlagAutoDoor.Start TRUE] => Loading Start Condition Check"));
                this.SeqCaseMemoLists.Add(20, string.Format("PIO Connection"));
                this.SeqCaseMemoLists.Add(100, string.Format("[AutoDoor<-VHL] : CS ON, Valid ON"));
                this.SeqCaseMemoLists.Add(110, string.Format("[AutoDoor->VHL] : Start ON after MoveReq ON"));
                this.SeqCaseMemoLists.Add(120, string.Format("[AutoDoor->VHL] : Insert Ready ON after MoveStart ON"));
                this.SeqCaseMemoLists.Add(130, string.Format("[AutoDoor->VHL] : Insert Request OFF"));
                this.SeqCaseMemoLists.Add(140, string.Format("[AutoDoor<-VHL] : Move Complete ON"));
                this.SeqCaseMemoLists.Add(150, string.Format("[AutoDoor->VHL] : Start OFF after PIO Reset"));
                this.SeqCaseMemoLists.Add(200, string.Format("[AutoDoor<-VHL] : CS ON, Valid ON"));
                this.SeqCaseMemoLists.Add(210, string.Format("[AutoDoor->VHL] : Start ON after MoveReq ON"));
                this.SeqCaseMemoLists.Add(220, string.Format("[AutoDoor->VHL] : Export Ready ON after MoveStart ON"));
                this.SeqCaseMemoLists.Add(230, string.Format("[AutoDoor->VHL] : Export Request OFF"));
                this.SeqCaseMemoLists.Add(240, string.Format("[AutoDoor<-VHL] : Move Complete ON"));
                this.SeqCaseMemoLists.Add(250, string.Format("[AutoDoor->VHL] : Start OFF"));
                this.SeqCaseMemoLists.Add(300, string.Format("Time Delay (100msec) after PIO Reset"));
                this.SeqCaseMemoLists.Add(310, string.Format("PIO Disconnection"));
                this.SeqCaseMemoLists.Add(1000, string.Format("Alarm Reset or Operate Cancel Wait"));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(string.Format("{0} EXCEPTION : {1}", FuncName, ex.ToString()));
            }
        }
        #endregion

        #region Method - Override
        /// <summary>
        /// 1. AutoDoor Start => AutoDoor Running , Move 시작 시점에 알려줘...
        /// 2. m_CurAutoDoor = GetNextAutoDoor()
        /// 3. Request Condition => Permit PIO Interface 
        /// 4. Reply Wait
        /// 5. AutoDoor In/Out 사이의 모든 Link는 Auto Door Area 판단하여 내부에 있을때 처리 필요함.
        /// </summary>
        /// <returns></returns>
        public override int Do()
        {
            m_CurTransferCommand = ProcessDataHandler.Instance.CurTransferCommand;
            m_CurVehicleStatus = ProcessDataHandler.Instance.CurVehicleStatus;

            if (m_CurVehicleStatus.CurrentPath.Type == LinkType.AutoDoorStraight || m_CurVehicleStatus.CurrentPath.Type == LinkType.AutoDoorCurve)
                ReceivedPermit = IsPermit(m_CurVehicleStatus.CurrentPath.FromNodeID); // 지나간 상태이니까 From을 확인하자~~
            else ReceivedPermit = IsPermit(m_CurVehicleStatus.CurrentPath.ToNodeID);
            PassRequest = m_CurAutoDoor != null ? IsPassRequest(m_CurAutoDoor.NodeId) : false;
            PassPermitCheck = m_CurAutoDoor != null ? m_CurTransferCommand.TargetNode == m_CurAutoDoor.NodeId : false;
            if (AppConfig.Instance.Simulation.MY_DEBUG && m_CurAutoDoor != null && m_CurAutoDoor.NodeId != 0)
            {
                string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                System.Diagnostics.Debug.WriteLine($"{time}, AutoDoor_PassPermitCheck, {PassPermitCheck},{PassRequest},{ReceivedPermit}, TargetNode, {m_CurTransferCommand.TargetNodeOfCommandSet}, NodeId, {m_CurAutoDoor.NodeId},  {m_CurVehicleStatus.CurrentPath},  {ProcessDataHandler.Instance.CurVehicleStatus.CurrentPath.CurrentPositionOfLink}");
            }

            int seqNo = this.SeqNo;
            switch (seqNo)
            {
                case 0:
                    {
                        if (m_AutoDoorStart)
                        {
                            m_AutoDoorStart = false;
                            this.scm.Start = true;

                            m_AutoDoorControl.Clear();
                            int nn = 0;
                            for (int i = 0; i < m_FullPath.Count; i++)
                            {
                                DataItem_Node node = DatabaseHandler.Instance.GetNodeDataOrNull(m_FullPath[i]);
                                bool autoDoor = IsAutoDoorStop(i);
                                double checkdistance = node.Type == NodeType.AutoDoorIn1 ? SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance : SetupManager.Instance.SetupOperation.AutoDoor2CheckStartDistance;
                                AutoDoorControl control = new AutoDoorControl()
                                {
                                    index = nn++,
                                    NodeId = m_FullPath[i],
                                    AutoDoorStop = autoDoor,
                                    RequestDone = false, //autoDoor ? false : true, // RequestDone true일 경우 재 명령을 못 내리도록 막자 !,,, 단 시간이 지나도 PermitDone이 오지 않는 경우는 Retry...
                                    PermitDone = false, // autoDoor ? false : true,  // Pass Permit 받으면 true,
                                    CheckDistance = autoDoor ? checkdistance : 0,
                                    IsAutoDoorArea = false,
                                };
                                m_AutoDoorControl.Add(control);
                                if (m_CurTransferCommand.PathMaps.Count > 0)
                                {
                                    Path path = m_CurTransferCommand.PathMaps.Find(x => x.ToNodeID == node.NodeID);
                                    if (path != null) { path.AutoDoorPermit = autoDoor ? false : true; }
                                }
                            }
                            // 만일 출발점이 JCSArea 내에 있는 경우. PassRequest를 받고 출발 하자..
                            if (IsAutoDoorInArea())
                            {
                                double checkdistance = SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance;

                                DataItem_Node from_node = DatabaseHandler.Instance.GetNodeDataOrNull(m_CurVehicleStatus.CurrentPath.FromNodeID);
                                DataItem_Node to_node = DatabaseHandler.Instance.GetNodeDataOrNull(m_CurVehicleStatus.CurrentPath.ToNodeID);
                                if (from_node.Type == NodeType.AutoDoorIn1 || from_node.Type == NodeType.AutoDoorIn2)
                                {
                                    if (from_node.Type == NodeType.AutoDoorIn1)
                                        checkdistance = SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance;
                                    else
                                        checkdistance = SetupManager.Instance.SetupOperation.AutoDoor2CheckStartDistance;

                                    AutoDoorControl control = m_AutoDoorControl.Find(x => x.NodeId == from_node.NodeID);
                                    if (control != null)
                                    {
                                        control.AutoDoorStop = true;
                                        control.RequestDone = false;
                                        control.PermitDone = false;
                                        control.CheckDistance = checkdistance; // default 값을 넣자 !
                                        control.IsAutoDoorArea = true;
                                    }
                                }
                                if (to_node.Type == NodeType.AutoDoorIn1 || to_node.Type == NodeType.AutoDoorIn2)
                                {
                                    if (to_node.Type == NodeType.AutoDoorIn1)
                                        checkdistance = SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance;
                                    else
                                        checkdistance = SetupManager.Instance.SetupOperation.AutoDoor2CheckStartDistance;

                                    AutoDoorControl control = m_AutoDoorControl.Find(x => x.NodeId == to_node.NodeID);
                                    if (control != null)
                                    {
                                        control.AutoDoorStop = true;
                                        control.RequestDone = false;
                                        control.PermitDone = false;
                                        control.CheckDistance = checkdistance; // default 값을 넣자 !
                                        control.IsAutoDoorArea = m_CurTransferCommand.RemainBcrDistance < checkdistance;
                                    }
                                }

                                //JCS Check를 강제로 만들자 !
                                //AutoDoorControl control = new AutoDoorControl()
                                //{
                                //    index = nn++,
                                //    NodeId = m_CurVehicleStatus.CurrentPath.FromNodeID,
                                //    AutoDoorStop = true,
                                //    RequestDone = false, // RequestDone true일 경우 재 명령을 못 내리도록 막자 !,,, 단 시간이 지나도 PermitDone이 오지 않는 경우는 Retry...
                                //    PermitDone = false,  // Pass Permit 받으면 true,
                                //    CheckDistance = SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance, // default 값을 넣자 !
                                //    IsAutoDoorArea = true, // AutoDoor Area에서 출발할때만 설정하자 !
                                //};
                                //m_AutoDoorControl.Add(control);

                                m_AutoDoorNewStart = true;
                            }

                            m_AutoDoorRunning = true;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor Start ! {0}", string.Join("-", m_FullPath)));
                            seqNo = 10;
                        }
                    }
                    break;

                case 10:
                    {
                        m_CurAutoDoor = GetNextAutoDoor();
                        if (m_CurAutoDoor == null)
                        {
                            // 더이상 찾을게 없다 모든 정보를 지우자 !
                            m_AutoDoorRunning = false;
                            m_AutoDoorControl.Clear();
                            m_FullPath.Clear();
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor Selected Next null"));

                            this.scm.Start = false;
                            this.scm.End = true;
                            seqNo = 0;
                        }
                        else
                        {
                            m_ConnectionRetry = 0;
                            m_PermitRetry = 0;

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor Selected Next {0}", m_CurAutoDoor.ToString()));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 20;
                        }
                    }
                    break;

                case 20:
                    {
                        // Pass Request Time
                        // 0. Target Node == AutoDoorStop (AutoDoorIn)
                        // 1. AutoDoorReqDone == false && AutoDoorPermitDone == false
                        // 2. Override가 걸린 상태 && collision distance < target distance  == false
                        // 3. IsCorner() == false
                        // 4. RemainBcrDistance < SetupOperation.AutoDoor1CheckStartDistance
                        // 5. 이동중일때
                        //   i) vitual bcr 값이 > 0
                        // AutoDoor Area 내에서 출발할 경우
                        // 이때는 무조건 PassRequest를 날려 허락을 받고 출발해야 한다.
                        int targetNode = m_CurTransferCommand.TargetNodeOfCommandSet;
                        double checkdistance = SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance;

                        bool vehicle_moving_cmd = false; // 이동한다는 명령을 받고 이동 중인 상황
                        vehicle_moving_cmd |= m_CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToSource;
                        vehicle_moving_cmd |= m_CurVehicleStatus.GetVehicleStatus() == VehicleState.EnRouteToDest;
                        vehicle_moving_cmd |= m_CurVehicleStatus.GetVehicleStatus() == VehicleState.Go;
                        bool pass_reqeust = true;
                        pass_reqeust &= PassRequest == false;
                        pass_reqeust &= ReceivedPermit == false;
                        pass_reqeust &= targetNode == m_CurAutoDoor.NodeId || m_CurAutoDoor.IsAutoDoorArea; // AutoDoor Area 내에서 출발할때는 Target Node 확인 불필요.
                        pass_reqeust &= m_CurVehicleStatus.ObsStatus.OverrideRatio > 0;
                        if (DatabaseHandler.Instance.DictionaryPIODevice.ContainsKey(m_CurAutoDoor.NodeId))
                            m_ADPioDevice = DatabaseHandler.Instance.DictionaryPIODevice[m_CurAutoDoor.NodeId];
                        else m_ADPioDevice = null;

                        if (m_ADPioDevice != null && m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor1)
                            checkdistance = SetupManager.Instance.SetupOperation.AutoDoor1CheckStartDistance;
                        else if (m_ADPioDevice != null && m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor2)
                            checkdistance = SetupManager.Instance.SetupOperation.AutoDoor2CheckStartDistance;
                        pass_reqeust &= m_CurTransferCommand.RemainBcrDistance < m_CurAutoDoor.CheckDistance || m_CurAutoDoor.IsAutoDoorArea; // AutoDoor Area 내에서 출발할때는 Remain Distance 확인 불필요.

                        bool byPass = false;
                        if (m_ADPioDevice != null && m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor1 && SetupManager.Instance.SetupOperation.AutoDoor1Use == Use.NoUse) byPass = true;
                        else if (m_ADPioDevice != null && m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor2 && SetupManager.Instance.SetupOperation.AutoDoor2Use == Use.NoUse) byPass = true;
                        if(!byPass)
                        {
                            pass_reqeust &= m_CurTransferCommand.RemainBcrDistance < m_CurVehicleStatus.ObsStatus.CollisionDistance || m_AutoDoorNewStart;
                        }

                        pass_reqeust &= GV.WheelBusy ? m_CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr > 0 : true; //처음 출발할때는 BCR값이 0일거다...
                        pass_reqeust &= vehicle_moving_cmd;
                        if (pass_reqeust)
                        {
                            m_AutoDoorNewStart = false;
                            // Auto Door Interface를 시작하자 !
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor Interface Start Condition OK [toNode={0}, RemainDistance={1}, CheckDistance={2}]",
                                targetNode, m_CurTransferCommand.RemainBcrDistance, m_CurAutoDoor.CheckDistance));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 100;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            ///////////////////////////////////////////////////////////////
                            string msg = string.Format("\r\n");
                            msg += "[SeqAutoDoorControlInformation]\r\n";
                            msg += string.Format("[CurAutoDoor, CurAutoDoorRequestDone, CurAutoDoorPermitDone]\r\n");
                            msg += string.Format("[{0}, {1}, {2}]\r\n", m_CurAutoDoor, m_CurAutoDoor.RequestDone, m_CurAutoDoor.PermitDone);
                            msg += string.Format("[PassRequest, ReceivedPermit, vehicle_moving_cmd, VehicleStatus]\r\n");
                            msg += string.Format("[{0}, {1}, {2}, {3}]\r\n", PassRequest, ReceivedPermit, vehicle_moving_cmd, m_CurVehicleStatus.GetVehicleStatus());
                            if (m_ADPioDevice != null)
                            {
                                msg += string.Format("[m_ADPioDevice, DeviceType, AutoDoor1Use, AutoDoor2Use]\r\n");
                                msg += string.Format("[{0}, {1}, {2}, {3}]\r\n", m_ADPioDevice, m_ADPioDevice.DeviceType, SetupManager.Instance.SetupOperation.AutoDoor1Use, SetupManager.Instance.SetupOperation.AutoDoor2Use);
                            }
                            msg += string.Format("[ReceivedTimeover, AutoDoorNewStart, CurAutoDoorNodeId, targetNode]\r\n");
                            msg += string.Format("[{0}, {1}, {2}, {3}]\r\n", m_ReceivedTimeover, m_AutoDoorNewStart, m_CurAutoDoor.NodeId, targetNode);
                            msg += string.Format("[JcsArea, CurAutoDoorCheckDistance, RemainDistanceOfLink, RemainBcrDistance]\r\n");
                            msg += string.Format("[{0}, {1}, {2}, {3}]\r\n", m_CurAutoDoor.IsAutoDoorArea, m_CurAutoDoor.CheckDistance, m_CurVehicleStatus.CurrentPath.RemainDistanceOfLink, m_CurTransferCommand.RemainBcrDistance);
                            msg += string.Format("[targetNode, VirtualRunBcr, CollisionDistance]\r\n");
                            msg += string.Format("[{0}, {1}, {2}]\r\n", targetNode, m_CurVehicleStatus.CurrentBcrStatus.VirtualRunBcr, m_CurVehicleStatus.ObsStatus.CollisionDistance);
                            ///////////////////////////////////////////////////////////////
                            SequenceInterfaceLog.WriteLog(FuncName, msg);

                            StartTicks = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 100:
                    {
                        int targetNode = m_CurTransferCommand.TargetNodeOfCommandSet;
                        if (DatabaseHandler.Instance.DictionaryPIODevice.ContainsKey(m_CurAutoDoor.NodeId))
                            m_ADPioDevice = DatabaseHandler.Instance.DictionaryPIODevice[m_CurAutoDoor.NodeId];
                        else m_ADPioDevice = null;

                        bool byPass = false;
                        if (m_ADPioDevice != null && m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor1 && SetupManager.Instance.SetupOperation.AutoDoor1Use == Use.NoUse) byPass = true;
                        else if (m_ADPioDevice != null && m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor2 && SetupManager.Instance.SetupOperation.AutoDoor2Use == Use.NoUse) byPass = true;
                        if (byPass)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor PIO ByPass"));
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 400;
                        }
                        else
                        {
                            if (m_CurTransferCommand.IsValid == false)
                            {
                                AlarmId = ProcessDataHandler.Instance.ALM_CommandNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor PIO Interface, Transfer Command Not Valid Alarm - {0}", EqpAlarm.GetAlarmMsg(AlarmId)));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 300;
                                seqNo = 1000;
                            }
                            else if (m_ADPioDevice == null)
                            {
                                AlarmId = m_ALM_ADIFPioDeviceNotExistError.ID;
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor PIO Device Null Alarm. Check Database.PIODevice [m_CurAutoDoor.NodeId:{0}]", m_CurAutoDoor.NodeId));
                                EqpAlarm.Set(AlarmId);
                                ReturnSeqNo = 300;
                                seqNo = 1000;
                            }
                            else if (m_devEqPio.PioComm.IsGo() == false || m_devEqPio.PioComm.IsOpen() == false)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("PIO Not Ready"));
                                seqNo = 110;
                            }
                            else
                            {
                                m_devEqPio.ResetPIO();
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor PIO Start"));
                                seqNo = 200;
                            }
                        }
                    }
                    break;

                case 110:
                    {
                        int id = m_ADPioDevice.PIOID;
                        int ch = m_ADPioDevice.PIOCH;
                        int rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            if (AlarmId != 0)
                            {
                                //AlarmId = rv1;
                                EqpAlarm.Reset(AlarmId);
                            }

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor_PIO [{0}, {1}] Set Channel OK", id, ch));
                            seqNo = 100;
                        }
                        else if (rv1 > 0)
                        {
                            m_ConnectionRetry++;
                            if (m_ConnectionRetry > 1 && AlarmId == 0)
                            {
                                AlarmId = rv1;
                                EqpAlarm.Set(AlarmId);
                            }

                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor_PIO [{0}, {1}] Set Channel Alarm - {2}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));
                            //EqpAlarm.Set(AlarmId); 여기서는 Alarm을 띄우지 말고 기다리자...!
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 120;
                        }
                    }
                    break;

                case 120:
                    {
                        // 5초에 한번씩 Retry 하자
                        if (XFunc.GetTickCount() - StartTicks < 5000) break;
                        SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor Not Response ! Set Channel Retry"));
                        seqNo = 110;
                    }
                    break;

                case 200:
                    {
                        m_devEqPio.SetAutoDoorStart(true);
                        m_CurAutoDoor.RequestDone = true;
                        SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : Start Signal ON");
                        
                        StartTicks = XFunc.GetTickCount();
                        
                        seqNo = 210;
                    }
                    break;

                case 210:
                    {
                        bool pass_possible = true;
                        pass_possible &= m_devEqPio.GetAutoDoorPassPossible();
                        pass_possible &= m_devEqPio.GetAutoDoorOpen();
                        pass_possible &= !m_devEqPio.GetAutoDoorClose();
                        if (AppConfig.Instance.Simulation.MY_DEBUG) 
                            pass_possible = true;

                        if (pass_possible)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor -> VHL : DoorOpen ON");
                            SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor -> VHL : Pass Possible Signal ON");
                            //m_CurAutoDoor.PermitDone = true;
                            bool permitSet = false;
                            for (int i = 0; i < m_AutoDoorControl.Count; i++)
                            {
                                if (permitSet)
                                {
                                    m_AutoDoorControl[i].PermitDone = true;
                                    break;
                                }
                                if (m_CurAutoDoor.NodeId == m_AutoDoorControl[i].NodeId)
                                {
                                    m_CurAutoDoor.PermitDone = true;
                                    permitSet = true;
                                }
                            }
                            
                            if (m_CurTransferCommand.PathMaps.Count > 0)
                            {
                                Path path = m_CurTransferCommand.PathMaps.Find(x => x.ToNodeID == m_CurAutoDoor.NodeId);
                                if (path != null) { path.AutoDoorPermit = true; }
                            }

                            if (AlarmId != 0)
                            {
                                SequenceJCSLog.WriteLog(FuncName, string.Format("Auto Door PassPermit Wait Reset Warning"));
                                EqpAlarm.Reset(AlarmId);
                                AlarmId = 0;
                            }
                            seqNo = 220;
                        }
                        else if (XFunc.GetTickCount() - StartTicks > 5 * 1000)
                        {
                            ///////////////////////////////////////////////////////////////
                            string msg = string.Format("\r\n");
                            msg += "[SeqAutoDoorPassPossibleInformation]\r\n";
                            if (m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor1)
                            {
                                msg += string.Format("[AutoDoor1 Use]\r\n");
                                msg += string.Format("[{0}]\r\n", SetupManager.Instance.SetupOperation.AutoDoor1Use);
                            }
                            else if(m_ADPioDevice.DeviceType == PIODeviceType.AutoDoor2)
                            {
                                msg += string.Format("[AutoDoor2 Use]\r\n");
                                msg += string.Format("[{0}]\r\n", SetupManager.Instance.SetupOperation.AutoDoor2Use);
                            }
                            msg += string.Format("[GetAutoDoorPassPossible, GetAutoDoorOpen, GetAutoDoorClose]\r\n");
                            msg += string.Format("[{0}, {1}, {2}]\r\n", m_devEqPio.GetAutoDoorPassPossible(), m_devEqPio.GetAutoDoorOpen(), m_devEqPio.GetAutoDoorClose());
                            ///////////////////////////////////////////////////////////////
                            SequenceInterfaceLog.WriteLog(FuncName, msg);

                            m_PermitRetry++;
                            if (m_PermitRetry > 4 && AlarmId == 0)
                            {
                                SequenceInterfaceLog.WriteLog(FuncName, string.Format("Auto Door PassPermit Wait Set Warning"));
                                AlarmId = m_ALM_ADPermitWait.ID;
                                EqpAlarm.Set(AlarmId);
                            }

                            StartTicks = XFunc.GetTickCount();
                        }
                    }
                    break;

                case 220:
                    {
                        // AutoDooor Area를 벗어 났는지 모니터링하자 !!!
                        bool autoDoorArea = IsAutoDoorInArea();
                        if (autoDoorArea)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : Auto Door Area Check OK");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 230;
                        }
                    }
                    break;

                case 230:
                    {
                        // AutoDooor Area를 벗어 났는지 모니터링하자 !!!
                        bool autoDoorArea = IsAutoDoorInArea();
                        if (autoDoorArea == false)
                        {
                            m_devEqPio.SetAutoDoorStart(false);
                            SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : Start Signal OFF");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 300;
                        }
                    }
                    break;

                case 300:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 100) break; // 끈기전 delay가 필요하네...
                        // PIO Comm을 닫고 마무리 하자...!
                        m_devEqPio.ResetPIO();
                        SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : PIO Signal All OFF");

                        StartTicks = XFunc.GetTickCount();
                        seqNo = 310;
                    }
                    break;

                case 310:
                    {
                        int id = 0;
                        int ch = 0;
                        int rv1 = m_devEqPio.SetChannelId(id, ch);
                        if (rv1 == 0)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor_PIO [{0}, {1}] Set Channel OK", id, ch));
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor PIO Finished"));
                            seqNo = 10;
                        }
                        else if (rv1 > 0)
                        {
                            AlarmId = rv1;
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("AutoDoor_PIO [{0}, {1}] Set Channel Alarm - {0}", id, ch, EqpAlarm.GetAlarmMsg(AlarmId)));

                            EqpAlarm.Set(AlarmId);
                            ReturnSeqNo = 0;
                            seqNo = 1000;
                        }
                    }
                    break;

                case 400:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;
                        SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : ByPass Request Done OK");
                        m_CurAutoDoor.RequestDone = true;
                        StartTicks = XFunc.GetTickCount();
                        seqNo = 410;
                    }
                    break;

                case 410:
                    {
                        if (XFunc.GetTickCount() - StartTicks < 200) break;
                        SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : ByPass Permit Done OK");
                        //m_CurAutoDoor.PermitDone = true;
                        bool permitSet = false;
                        for (int i = 0; i < m_AutoDoorControl.Count; i++)
                        {
                            if (permitSet)
                            {
                                m_AutoDoorControl[i].PermitDone = true;
                                break;
                            }
                            if (m_CurAutoDoor.NodeId == m_AutoDoorControl[i].NodeId)
                            {
                                m_CurAutoDoor.PermitDone = true;
                                permitSet = true;
                            }
                        }

                        if (m_CurTransferCommand.PathMaps.Count > 0)
                        {
                            Path path = m_CurTransferCommand.PathMaps.Find(x => x.ToNodeID == m_CurAutoDoor.NodeId);
                            if (path != null) { path.AutoDoorPermit = true; }
                        }

                        seqNo = 420;
                    }
                    break;

                case 420:
                    {
                        // AutoDooor Area를 벗어 났는지 모니터링하자 !!!
                        bool autoDoorArea = IsAutoDoorInArea();
                        if (autoDoorArea)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : Auto Door Area In Check OK");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 430;
                        }
                    }
                    break;

                case 430:
                    {
                        // AutoDooor Area를 벗어 났는지 모니터링하자 !!!
                        bool autoDoorArea = IsAutoDoorInArea();
                        if (autoDoorArea == false)
                        {
                            SequenceInterfaceLog.WriteLog(FuncName, "AutoDoor <- VHL : Auto Door Area Out Check OK");
                            StartTicks = XFunc.GetTickCount();
                            seqNo = 10;
                        }
                    }
                    break;

                case 1000:
                    {
                        if (IsPushedSwitch.IsAlarmReset)
                        {
                            IsPushedSwitch.m_AlarmRstPushed = false;

                            EqpAlarm.Reset(AlarmId);
                            SequenceInterfaceLog.WriteLog(FuncName, string.Format("Reset Alarm : Code[{0}]", AlarmId));
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
        public void AutoDoorStart(List<int> fullPath)
        {
            m_AutoDoorRunning = false;
            this.InitSeq();
            m_FullPath.Clear();
            // Fisrt / Last는 지우자 ~~
            for (int i = 0; i < fullPath.Count - 1; i++)
            {
                m_FullPath.Add(fullPath[i]);
            }
            m_AutoDoorStart = true;
        }
        private bool IsPermit(int next_door)
        {
            bool rv = false;
            try
            {
                AutoDoorControl jc = m_AutoDoorControl.Find(x => x.NodeId == next_door);
                if (jc != null) { rv = jc.PermitDone; }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        private bool IsPassRequest(int next_door)
        {
            bool rv = false;
            try
            {
                AutoDoorControl jc = m_AutoDoorControl.Find(x => x.NodeId == next_door);
                if (jc != null) { rv = jc.RequestDone; }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return rv;
        }
        /// <summary>
        /// Next AutoDoor가 0인 경우는 더 이상 진행할 AutoDoor가 없다는 뜻
        /// </summary>
        /// <returns></returns>
        private AutoDoorControl GetNextAutoDoor()
        {
            AutoDoorControl jc = null;
            try
            {
                int cur_node = m_CurVehicleStatus.CurrentPath.FromNodeID;
                int cur_index = m_AutoDoorControl.FindIndex(x => x.NodeId == cur_node);
                jc = m_AutoDoorControl.Find(x => x.AutoDoorStop == true && x.PermitDone == false && x.index >= cur_index);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return jc;
        }
        private bool IsAutoDoorInArea()
        {
            bool inArea = false;
            try
            {
                int fromNode = m_CurVehicleStatus.CurrentPath.ToNodeID;
                if (DatabaseHandler.Instance.DictionaryNodeDataList.ContainsKey(fromNode))
                {
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.AutoDoorIn1;
                    inArea |= DatabaseHandler.Instance.DictionaryNodeDataList[fromNode].Type == NodeType.AutoDoorIn2;
                }
                inArea |= m_CurVehicleStatus.CurrentPath.Type == LinkType.AutoDoorCurve;
                inArea |= m_CurVehicleStatus.CurrentPath.Type == LinkType.AutoDoorStraight;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return inArea;
        }
        private bool IsAutoDoorStop(int start)
        {
            bool outExist = false;
            try
            {
                if (m_CurTransferCommand.PathMaps.Count > 0)
                {
                    DataItem_Node start_node = DatabaseHandler.Instance.GetNodeDataOrNull(m_FullPath[start]);
                    bool autoDoor1 = start_node.Type == NodeType.AutoDoorIn1;
                    bool autoDoor2 = start_node.Type == NodeType.AutoDoorIn2;
                    if (autoDoor1 || autoDoor2)
                    {
                        int index = m_CurTransferCommand.PathMaps.FindIndex(x => x.ToNodeID == start_node.NodeID);
                        if (index >= 0)
                        {
                            for (int i = index; i < m_CurTransferCommand.PathMaps.Count; i++)
                            {
                                DataItem_Node out_node = DatabaseHandler.Instance.GetNodeDataOrNull(m_CurTransferCommand.PathMaps[i].ToNodeID);
                                if (out_node == null) break;
                                else
                                {
                                    bool autoDoorIn = out_node.Type == NodeType.AutoDoorIn1 || out_node.Type == NodeType.AutoDoorIn2;
                                    if (autoDoorIn) continue;
                                    if (autoDoor1) outExist |= out_node.Type == NodeType.AutoDoorOut1;
                                    if (autoDoor2) outExist |= out_node.Type == NodeType.AutoDoorOut2;
                                    if (outExist) break;
                                    else
                                    {
                                        bool auto_door_link = false;
                                        auto_door_link |= m_CurTransferCommand.PathMaps[i].Type == LinkType.AutoDoorStraight;
                                        auto_door_link |= m_CurTransferCommand.PathMaps[i].Type == LinkType.AutoDoorCurve;
                                        if (auto_door_link == false) break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return outExist;
        }
        //AutoDoor In에서 Out을 지나가는 경로인지 확인
        public bool IsAutoDoorOutExist(int start, int count) 
        {
            bool outExist = false;
            try
            {
                if (m_CurTransferCommand.PathMaps.Count > 0)
                {
                    DataItem_Node start_node = DatabaseHandler.Instance.GetNodeDataOrNull(start);
                    bool autoDoor1 = start_node.Type == NodeType.AutoDoorIn1;
                    bool autoDoor2 = start_node.Type == NodeType.AutoDoorIn2;
                    if (autoDoor1 || autoDoor2)
                    {
                        int index = m_CurTransferCommand.PathMaps.FindIndex(x => x.FromNodeID == start_node.NodeID);
                        if (index >= 0)
                        {
                            int length = index + count;
                            if (length > m_CurTransferCommand.PathMaps.Count) 
                                length = m_CurTransferCommand.PathMaps.Count;

                            for (int i = index; i < length; i++)
                            {
                                DataItem_Node out_node = DatabaseHandler.Instance.GetNodeDataOrNull(m_CurTransferCommand.PathMaps[i].ToNodeID);
                                if (out_node == null) break;
                                else
                                {
                                    bool autoDoorIn = out_node.Type == NodeType.AutoDoorIn1 || out_node.Type == NodeType.AutoDoorIn2;
                                    if (autoDoorIn) continue;
                                    if (autoDoor1) outExist |= out_node.Type == NodeType.AutoDoorOut1;
                                    if (autoDoor2) outExist |= out_node.Type == NodeType.AutoDoorOut2;
                                    if (outExist) break;
                                    else
                                    {
                                        bool auto_door_link = false;
                                        auto_door_link |= m_CurTransferCommand.PathMaps[i].Type == LinkType.AutoDoorStraight;
                                        auto_door_link |= m_CurTransferCommand.PathMaps[i].Type == LinkType.AutoDoorCurve;
                                        if (auto_door_link == false) break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return outExist;
        }
        #endregion
    }
    public class AutoDoorControl
    {
        public int index { get; set; }
        public int NodeId { get; set; }
        public bool AutoDoorStop { get; set; }
        public double CheckDistance { get; set; }
        public bool RequestDone { get; set; }
        public bool PermitDone { get; set; }
        public bool IsAutoDoorArea { get; set; } //AutoDoor 앞에서 출발할때
        public AutoDoorControl()
        {
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4}", NodeId, AutoDoorStop, CheckDistance, RequestDone, PermitDone);
        }
    }

}
