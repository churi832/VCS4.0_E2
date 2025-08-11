using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Remoting
{
    [Serializable]
    public class PadRemoteGUI : MarshalByRefObject
    {
        #region fields
        private static bool m_HeartBitRun = false;
        private static int m_HeartBitCount = 0;
        private static OperateMode m_OpMode = OperateMode.None;
        private static EqpState m_EqState = EqpState.None;
        private static EqpRunMode m_EqRunMode = EqpRunMode.None;
        private static string m_JcsState = string.Empty;
        private static string m_OcsState = string.Empty;
        private static double m_Velocity = 0.0f;
        private static bool m_IsAlarm = false;
        private static List<int> m_AlarmIds = new List<int>();
        private static List<string> m_AlarmName = new List<string>();
        private static RemoteItem m_RemoteItem = null;

        private static double m_SlavePosition = 0.0d;
        private static double m_MasterPosition = 0.0d;
        private static double m_HoistPosition = 0.0d;
        private static double m_RotatePosition = 0.0d;
        private static double m_SlidePosition = 0.0d;
        private static double m_FrontAntiDropPosition = 0.0d;
        private static double m_RearAntiDropPosition = 0.0d;

        private static string m_SlaveServoState = "None";
        private static string m_MasterServoState = "None";
        private static string m_HoistServoState = "None";
        private static string m_RotateServoState = "None";
        private static string m_SlideServoState = "None";
        private static string m_FrontAntiDropServoState = "None";
        private static string m_RearAntiDropServoState = "None";
        private static bool m_SlaveHomeDone = false;
        private static bool m_MasterHomeDone = false;
        private static bool m_HoistHomeDone = false;
        private static bool m_RotateHomeDone = false;
        private static bool m_SlideHomeDone = false;
        private static bool m_FrontAntiDropHomeDone = false;
        private static bool m_RearAntiDropHomeDone = false;
        private static bool m_SlaveAlarm = false;
        private static bool m_MasterAlarm = false;
        private static bool m_HoistAlarm = false;
        private static bool m_RotateAlarm = false;
        private static bool m_SlideAlarm = false;
        private static bool m_FrontAntiDropAlarm = false;
        private static bool m_RearAntiDropAlarm = false;
        private static bool m_IsReady = false;

        private static string m_FrontSteerStatus = "None";
        private static string m_RearSteerStatus = "None";
        private static string m_GripperStatus = "None";
        private static string m_FrontAntiDropStatus = "None";
        private static string m_RearAntiDropStatus = "None";

        public string RearAntiDropStatus { get => m_RearAntiDropStatus; set => m_RearAntiDropStatus = value; }
        public List<string> AlarmName { get => m_AlarmName; set => m_AlarmName = value; }
        public string JcsState { get => m_JcsState; set => m_JcsState = value; }
        public string OcsState { get => m_OcsState; set => m_OcsState = value; }

        private ULongServoStatus m_servoStatus = ULongServoStatus.None;
        #endregion
        [Flags]
        public enum ULongServoStatus : ulong
        {
            None = 0,
            SlaveServoState = 1 << 0,
            MasterServoState = 1 << 1,
            HoistServoState = 1 << 2,
            RotateServoState = 1 << 3,
            SlideServoState = 1 << 4,
            FrontAntiDropServoState = 1 << 5,
            RearAntiDropServoState = 1 << 6,
            SlaveHomeDone = 1 << 7,
            MasterHomeDone = 1 << 8,
            HoistHomeDone = 1 << 9,
            RotateHomeDone = 1 << 10,
            SlideHomeDone = 1 << 11,
            FrontAntiDropHomeDone = 1 << 12,
            RearAntiDropHomeDone = 1 << 13,
            SlaveAlarm = 1 << 14,
            MasterAlarm = 1 << 15,
            HoistAlarm = 1 << 16,
            RotateAlarm = 1 << 17,
            SlideAlarm = 1 << 18,
            FrontAntiDropAlarm = 1 << 19,
            RearAntiDropAlarm = 1 << 20,

            FrontSteerStatus = 1 << 21,
            RearSteerStatus = 1 << 22,
            GripperStatus = 1 << 23,
            FrontAntiDropStatus = 1 << 24,
            RearAntiDropStatus = 1 << 25,
        }
        public ULongServoStatus ServoStatus { get => m_servoStatus; set => m_servoStatus = value; }
        #region properties

        public bool HeartBitRun { get => m_HeartBitRun; set => m_HeartBitRun = value; }
        public int HeartBitCount { get => m_HeartBitCount; set => m_HeartBitCount = value; }
        public OperateMode OpMode { get => m_OpMode; set => m_OpMode = value; }
        public EqpState EqState { get => m_EqState; set => m_EqState = value; }
        public EqpRunMode EqRunMode { get => m_EqRunMode; set => m_EqRunMode = value; }
        public bool IsReady { get => m_IsReady; set => m_IsReady = value; }
        public double Velocity { get => m_Velocity; set => m_Velocity = value; }
        public bool IsAlarm { get => m_IsAlarm; set => m_IsAlarm = value; }
        public List<int> AlarmIds { get => m_AlarmIds; set => m_AlarmIds = value; }
        public double SlavePosition { get => m_SlavePosition; set => m_SlavePosition = value; }
        public double MasterPosition { get => m_MasterPosition; set => m_MasterPosition = value; }
        public double HoistPosition { get => m_HoistPosition; set => m_HoistPosition = value; }
        public double RotatePosition { get => m_RotatePosition; set => m_RotatePosition = value; }
        public double SlidePosition { get => m_SlidePosition; set => m_SlidePosition = value; }
        public double FrontAntiDropPosition { get => m_FrontAntiDropPosition; set => m_FrontAntiDropPosition = value; }
        public double RearAntiDropPosition { get => m_RearAntiDropPosition; set => m_RearAntiDropPosition = value; }
        public string SlaveServoState { get => m_SlaveServoState; set => m_SlaveServoState = value; }
        public string MasterServoState { get => m_MasterServoState; set => m_MasterServoState = value; }
        public string HoistServoState { get => m_HoistServoState; set => m_HoistServoState = value; }
        public string RotateServoState { get => m_RotateServoState; set => m_RotateServoState = value; }
        public string SlideServoState { get => m_SlideServoState; set => m_SlideServoState = value; }
        public string FrontAntiDropServoState { get => m_FrontAntiDropServoState; set => m_FrontAntiDropServoState = value; }
        public string RearAntiDropServoState { get => m_RearAntiDropServoState; set => m_RearAntiDropServoState = value; }
        public bool SlaveHomeDone { get => m_SlaveHomeDone; set => m_SlaveHomeDone = value; }
        public bool MasterHomeDone { get => m_MasterHomeDone; set => m_MasterHomeDone = value; }
        public bool HoistHomeDone { get => m_HoistHomeDone; set => m_HoistHomeDone = value; }
        public bool RotateHomeDone { get => m_RotateHomeDone; set => m_RotateHomeDone = value; }
        public bool SlideHomeDone { get => m_SlideHomeDone; set => m_SlideHomeDone = value; }
        public bool FrontAntiDropHomeDone { get => m_FrontAntiDropHomeDone; set => m_FrontAntiDropHomeDone = value; }
        public bool RearAntiDropHomeDone { get => m_RearAntiDropHomeDone; set => m_RearAntiDropHomeDone = value; }
        public bool SlaveAlarm { get => m_SlaveAlarm; set => m_SlaveAlarm = value; }
        public bool MasterAlarm { get => m_MasterAlarm; set => m_MasterAlarm = value; }
        public bool HoistAlarm { get => m_HoistAlarm; set => m_HoistAlarm = value; }
        public bool RotateAlarm { get => m_RotateAlarm; set => m_RotateAlarm = value; }
        public bool SlideAlarm { get => m_SlideAlarm; set => m_SlideAlarm = value; }
        public bool FrontAntiDropAlarm { get => m_FrontAntiDropAlarm; set => m_FrontAntiDropAlarm = value; }
        public bool RearAntiDropAlarm { get => m_RearAntiDropAlarm; set => m_RearAntiDropAlarm = value; }
        public string FrontSteerStatus { get => m_FrontSteerStatus; set => m_FrontSteerStatus = value; }
        public string RearSteerStatus { get => m_RearSteerStatus; set => m_RearSteerStatus = value; }
        public string GripperStatus { get => m_GripperStatus; set => m_GripperStatus = value; }
        public string FrontAntiDropStatus { get => m_FrontAntiDropStatus; set => m_FrontAntiDropStatus = value; }
        #endregion

        public delegate void ChangeModeHandler(OperateMode mode);
        public static event ChangeModeHandler ChangeMode;
        public delegate void ChangeRunModeHandler(EqpRunMode mode);
        public static event ChangeRunModeHandler ChangeRunMode;
        public RemoteItem RemoteItem { get => m_RemoteItem; set => m_RemoteItem = value; }
        public bool ChangeOpMode(OperateMode mode)
        {
            bool result = false;
            try
            {
                ChangeMode(mode);
                result = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return result;
        }

        public bool ChangeEqpRunMode(EqpRunMode mode)
        {
            bool result = false;
            try
            {
                ChangeRunMode(mode);
                result = true;
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            return result;
        }
    }
}
