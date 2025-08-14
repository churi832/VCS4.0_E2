using Sineva.VHL.Library;
using Sineva.VHL.Data.Alarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data
{
    public class GV
    {
        #region Fields
        public static readonly GV Instance = new GV();
        private readonly static object m_LockKey = new object();
        #endregion

        #region Fields - Common
        public static bool OperatorCallBuzzerOn = false;
        public static bool OperatorCallConfirm = false;
        public static bool OperatorCallSelect1 = false;
        public static bool OperatorCallSelect2 = false;
        public static bool OperatorCallTowerLamp = false;
        #endregion

        #region Safty
        public static bool ThreadStop = false;
        public static bool PowerOn = true;
        public static bool EmoAlarm = false;
        public static bool SaftyAlarm = false;
        public static bool CpAlarm = false;
        public static bool ManualPendantActivated = false;
        public static bool AutoModeSwitched = false;
        public static bool SteerNotChangeInterlock = false;
        public static bool BumpCollisionInterlock = false;
        public static bool BeltCutInterlock = false;
        public static bool SwingSensorInterlock = false;
        public static bool WheelBusy = false;
        public static bool AutoRecoveryStart = false;
        public static bool AutoModeServoNotReady = false;
        public static bool AutoTeachingModeOn = false;
        public static bool CleanerDoorOpenInterlock = false;
        public static bool AbnormalFrontDetectInterlock = false;
        public static bool AbnormalVelocityLimitInterlock = false;
        public static bool RouteChangeTimeOverCheck = false;

        public static bool MxpHeartBitNg = false;
        public static bool MxpHeartBitError = false;
        public static bool MxpHeartBitInterlock = false; //이건 MXP가 만드는것...
        public static bool RouteChangeTimeOverInterlock = false;
        public static bool BCRNotChangedInterlock = false;
        public static bool OverrideAbnormalStopInterlock = false;
        public static bool CpsLowVoltageInterlock = false;
        public static bool EtherCatDisconnectError = false;
        public static bool InterlockRecoveryStart = false;
        public static bool FoupCoverNotDetectInterlock = false;
        public static bool InterlockServoStop = false; //scmAbortSeqRun를 Start 못하돌고 막아야 한다. 중복처리 방지
        public static bool TouchHeartBitError = false;
        public static bool TouchHeartBitInterlock = false;
        public static bool VehicleMoveRecovery = false; // Foup이 있는 상태에서 Abort가 내려오지 않는다. InRange Alarm 같은 경우 Retry 해 볼수 있는데...

        public static bool[] TEST_INTERLOCK = new bool[10];
        public static bool OpRouteChange = false;
        public static bool RouteChangeOk = false; //Vehicle Move에서 CASE 30번 간 뒤에는 Abort 하면 안되기 때문에 Flag 만들어서 SeqOcsCommandProcess seqNo = 0으로 보내자..
        #endregion

        #region Interlock
        public static int TransferMoveEnableCode = 0;
        public static bool TransferMoveEnable = false;
        public static int HoistMoveEnableCode = 0;
        public static bool HoistMoveEnable = false;
        public static int SlideMoveEnableCode = 0;
        public static bool SlideMoveEnable = false;
        public static bool GripOpenEnable = false;
        #endregion

        #region Cycle Test
        public static int HoistCycleTotalCount = 10;
        public static int SteerCycleTotalCount = 10;
        public static int AntiDropCycleTotalCount = 10;
        public static int WheelMoveCycleTotalCount = 10;
        public static int HoistCycleWaitTime = 1;
        public static int SteerCycleWaitTime = 1;
        public static int AntiDropCycleWaitTime = 1;
        public static int WheelMoveCycleWaitTime = 1;
        public static int AcquireCycleTime = 1;
        public static int DepositCycleTime = 1;
        public static int SteerCycleTime = 1;
        public static int AntiDropCycleTime = 1;
        public static int WheelMoveCycleTime = 1;
        #endregion

        #region Sequence Command
        public static XSeqCmd scmAbortSeqRun = new XSeqCmd("AbortSeqRun");
        #endregion

        #region Life Time Items
        public static List<GeneralObject> LifeTimeItems = new List<GeneralObject>();
        #endregion

        #region Check Recovery Processing //Run Mode가 만들어지기 전까지 임시로 사용..
        public static bool RecoveryProcess = false;
        public static bool InitStart = false;
        public static bool InitComp = false;
        public static bool InitFail = false;
        #endregion
        public static enSimulationFlag SimulationFlag = enSimulationFlag.None;

    }

    public static class IsPushedSwitch
    {
        #region Fields
        ///<summary> UI Alarm Reset Button
        ///</summary>
        public static bool m_IsAlarmReset = false;
        public static bool m_AlarmRstPushed = false; // Program memory
        ///<summary> UI Buzzer Off Button
        ///</summary>
        public static bool m_IsBuzzerOff = false;
        public static bool m_BuzzerOffPushed = false; // Program memory
        #endregion

        #region Properties
        public static bool IsAlarmReset
        {
            get
            {
                bool pushed = m_AlarmRstPushed; m_AlarmRstPushed = false;
                return m_IsAlarmReset | pushed;
            }
            set { m_IsAlarmReset = value; }
        }

        public static bool IsBuzzerOff
        {
            get
            {
                bool pushed = m_BuzzerOffPushed; m_BuzzerOffPushed = false;
                return m_IsBuzzerOff | pushed;
            }
            set { m_IsBuzzerOff = value; }
        }
        #endregion
    }

    public static class EqpAlarm
    {
        public struct AlarmItem
        {
            public AlarmCondition Condition;
            public int AlarmId;
        }

        // Alarm 발생 유무 확인
        // CIM Report할때 하나씩 가져가서 보고
        public static Queue<AlarmItem> EqpAlarmItems = new Queue<AlarmItem>();

        public static void Set(int id)
        {
            AlarmHandler.Instance.SetQAlarm(id);
            SequenceLog.WriteLog("[EqpAlarm]", string.Format("Set Alarm : Code[{0}]", id));

            AlarmItem item = new AlarmItem();
            item.Condition = AlarmCondition.AlarmSet;
            item.AlarmId = id;
            EqpAlarmItems.Enqueue(item);
            EventHandlerManager.Instance.InvokeAlarmHappened(id);
        }

        public static void Reset(int id)
        {
            if (id == 0) return;
            //if (AlarmHandler.Instance.IsContainAlarm(id))
            {
                AlarmHandler.Instance.RstQAlarm(id);
                SequenceLog.WriteLog("[EqpAlarm]", string.Format("Reset Alarm : Code[{0}]", id));

                AlarmItem item = new AlarmItem();
                item.Condition = AlarmCondition.AlarmReset;
                item.AlarmId = id;
                EqpAlarmItems.Enqueue(item);
            }
        }
        public static void ResetAll()
        {
            List<int> curAlarms = AlarmCurrentProvider.Instance.GetCurrentAlarmIds();
            for (int i = 0; i < curAlarms.Count; i++)
            {
                Reset(curAlarms[i]);
            }
        }

        public static string GetAlarmMsg(int id)
        {
            string alarm_msg = AlarmHandler.Instance.GetAlarmMessage(id);
            return alarm_msg;
        }
    }

}

