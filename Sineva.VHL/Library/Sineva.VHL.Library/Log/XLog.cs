/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team HJYOU
 * Issue Date	: 23.01.16
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Windows.Interop;

namespace Sineva.VHL.Library
{
    public static class SequenceLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "Sequence", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "Sequence");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] Sequence", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] Sequence", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] Sequence", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("[{0}] {1}", method.Name, log));
        }
    }
    public static class SequenceJCSLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "SequenceJCS", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SequenceJCS");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] SequenceJCS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceJCS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceJCS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("[{0}] {1}", method.Name, log));
        }
    }
    public static class SequenceOCSLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "SequenceOCS", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SequenceOCS");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] SequenceOCS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceOCS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceOCS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("[{0}] {1}", method.Name, log));
        }
    }
    public static class SequenceInterfaceLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "SequenceInterface", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SequenceInterface");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] SequenceInterface", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceInterface", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceInterface", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("[{0}] {1}", method.Name, log));
        }
    }
    public static class SequenceDeviceLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "SequenceDevice", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SequenceDevice");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] SequenceDevice", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceDevice", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SequenceDevice", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("[{0}] {1}", method.Name, log));
        }
    }
    public static class ManualLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "Manual", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "Manual");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] Manual", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class ServoLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "Servo", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "Servo");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] Servo", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log)
        {
            string msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] Servo", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), msg);
        }
    }
    public static class ExceptionLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "Exception", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "Exception");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] Exception", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log)
        {
            string msg = string.Format("{0} {1} {2}", method.ReflectedType.FullName, method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] Exception", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), msg);
        }
    }
    public static class OcsCommLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "OcsComm", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "OcsComm");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] OcsComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] OcsComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(string[] logs)
        {
            if (logs.Length > 0)
            {
                m_Logger.SetLog(logs);
                logUpdate?.Invoke(string.Format("[{0}] OcsComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), logs[0]);
            }
        }
    }
    public static class OcsCommStatusLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "OcsCommStatus", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "OcsCommStatus");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string[] logs)
        {
            if (logs.Length > 0)
            {
                m_Logger.SetLog(logs);
                logUpdate?.Invoke(string.Format("[{0}] OcsCommStatus", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), logs[0]);
            }
        }
    }
    public static class JcsCommLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "JcsComm", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "JcsComm");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] JcsComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] JcsComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(string[] logs)
        {
            if (logs.Length > 0)
            {
                m_Logger.SetLog(logs);
                logUpdate?.Invoke(string.Format("[{0}] JcsComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), logs[0]);
            }
        }
    }
    public static class JcsCommStatusLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "JcsCommStatus", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "JcsCommStatus");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string[] logs)
        {
            if (logs.Length > 0)
            {
                m_Logger.SetLog(logs);
                logUpdate?.Invoke(string.Format("[{0}] JcsCommStatus", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), logs[0]);
            }
        }
    }
    public static class MxpCommLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "MxpComm", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "MxpComm");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] MxpComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class TouchGUILog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "TouchGUI", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "TouchGUI");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] TouchGUI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class SerialCommLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "SerialComm", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SerialComm");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] SerialComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class TactTimeLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "TactTime", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "TactTime");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] TactTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] TactTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), msg);
        }
    }
    public static class ServoAxisLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "ServoAxisLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "ServoAxisLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class CrevisCommLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "CrevisComm", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "CrevisComm");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] CrevisComm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class RemotingLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "RemotingLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "RemotingLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] Remoting", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class VisionLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "VisionLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "VisionLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] VisionLog", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class ButtonLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "ButtonLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "ButtonLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] ButtonLog", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log)
        {
            string msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] ButtonLog", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), msg);
        }
    }
    public static class IOMonitorLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "IOMonitorLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "IOMonitorLog");
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
        }
    }
    public class TpdLog
    {
        public List<string> HeaderText = new List<string>();
        public XLog m_Logger = null;

        public TpdLog(string[] header) 
        {
            HeaderText.Clear();
            HeaderText = header.ToList();
            m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "TpdLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "TpdLog", string.Join(",", HeaderText));
        }
        public void WriteLog(string[] logs)
        {
            if (m_Logger != null)
                m_Logger.SetLog(string.Format("{0}", string.Join(",", logs)));
        }
    }
    public static class ThreadCheckLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "ThreadCheckLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "ThreadCheckLog");
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
        }
    }
    public class HomeSensorLog
    {
        public List<string> HeaderText = new List<string>();
        public XLog m_HoistLogger = null;
        public XLog m_SlideLogger = null;
        public XLog m_RotateLogger = null;

        public HomeSensorLog(string[] header)
        {
            HeaderText.Clear();
            HeaderText = header.ToList();
            m_HoistLogger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "HomeSensorLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "HoistHomeSensorLog", string.Join(",", HeaderText));
            m_SlideLogger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "HomeSensorLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SlideHomeSensorLog", string.Join(",", HeaderText));
            m_RotateLogger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "HomeSensorLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "RotateHomeSensorLog", string.Join(",", HeaderText));
        }
        public void WriteHoistLog(string[] logs)
        {
            if (m_HoistLogger != null)
                m_HoistLogger.SetLog(string.Format("{0}", string.Join(",", logs)));
        }
        public void WriteSlideLog(string[] logs)
        {
            if (m_SlideLogger != null)
                m_SlideLogger.SetLog(string.Format("{0}", string.Join(",", logs)));
        }
        public void WriteRotateLog(string[] logs)
        {
            if (m_RotateLogger != null)
                m_RotateLogger.SetLog(string.Format("{0}", string.Join(",", logs)));
        }
    }
    public class PIOSignalLog
    {
        public List<string> HeaderText = new List<string>();
        public XLog m_Logger = null;

        public PIOSignalLog(string[] header)
        {
            HeaderText.Clear();
            HeaderText = header.ToList();
            m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "PIOSignalLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "PIOSignalLog", string.Join(",", HeaderText));
        }
        public void WriteLog(string[] logs)
        {
            if (m_Logger != null)
                m_Logger.SetLog(string.Format("{0}", string.Join(",", logs)));
        }
    }
    public static class CpsLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "CpsLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "CpsLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] CpsLog", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class AlarmLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "AlarmLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "AlarmLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] Alarm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] Alarm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
        public static void WriteLog(System.Reflection.MethodBase method, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", method.Name, seqNo, log);
            else msg = string.Format("{0} {1}", method.Name, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] Alarm", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("[{0}] {1}", method.Name, log));
        }
    }
	public static class OpCallLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "OpCallLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "OpCallLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string log)
        {
            m_Logger.SetLog(log);
            logUpdate?.Invoke(string.Format("[{0}] OpCallLog", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), log);
        }
    }
    public static class SeqAutoLog
    {
        public static XLog m_Logger = new XLog(AppConfig.Instance.LogPath.SelectedFolder, "SeqAutoLog", (int)AppConfig.Instance.LogFileSplit, AppConfig.Instance.MaxRecordLines, "SeqAutoLog");
        public static event DelVoid_StringString logUpdate = null;
        public static void WriteLog(string funcName, string log, int seqNo = -1)
        {
            string msg = string.Empty;
            if (seqNo >= 0) msg = string.Format("{0} {1} {2}", funcName, seqNo, log);
            else msg = string.Format("{0} {1}", funcName, log);
            m_Logger.SetLog(msg);
            logUpdate?.Invoke(string.Format("[{0}] SeqAutoLog", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")), string.Format("{0} {1}", funcName, log));
        }
    }

    public class XLog
    {
        #region Fields
        private string _LogName = string.Empty;
        private string _LogPath = string.Empty;
        private string _SubPath = string.Empty;
        private int _LogSplitType = 0;
        private int _LogMaxLine = 1000000;
        private FileStream _LogFileStream = null;
        private StreamWriter _LogStreamWriter = null;
        private string _OldStreamName = string.Empty;
        private string _LogDate = string.Empty;
        private string _LogDateFolder = string.Empty;
        //private Queue<string> _LogQueue = new Queue<string>();
        private ConcurrentQueue<string> _LogQueue = new ConcurrentQueue<string>();

        private Thread _LogWriteThread;
        private bool _ThreadState = true;

        private bool _DateFolderExist = false;
        private int _WriteCount = 0;
        private int _FileCount = 0;
        private int _RecordLine = 0;
        private string _HeaderLineText = string.Empty;

        //private static XLogGarbageCollector _XLogGarbageCollector = null;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public XLog(string logPath, string logName, string headerText = "")
        {
            try
            {
                _LogPath = logPath;
                _LogName = logName;
                _HeaderLineText = headerText;

                _FileCount = 1;

                _LogWriteThread = new Thread(new ThreadStart(LogWriteThread));
                _LogWriteThread.IsBackground = true;
                _LogWriteThread.Start();

                //if (_XLogGarbageCollector == null)
                //{
                //    _XLogGarbageCollector = XLogGarbageCollector.Instance;
                //    _XLogGarbageCollector.LogFileStreamClose += this.CloseLogFileStreamByFolderName;
                //    _XLogGarbageCollector.Initialize();
                //}
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"XLog, EXCEPTION={ex.ToString()}");
            }
        }
        public XLog(string logPath, int logSplitType, int logMaxLines, string logName, string headerText = "")
        {
            try
            {
                _LogPath = logPath;
                _LogName = logName;
                _LogSplitType = logSplitType;
                _LogMaxLine = logMaxLines;
                _HeaderLineText = headerText;

                _LogWriteThread = new Thread(new ThreadStart(LogWriteThread));
                _LogWriteThread.IsBackground = true;
                _LogWriteThread.Start();

                if (logSplitType == 2)
                {
                    string dateFolderName = string.Empty;// string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyyMMdd"));
                    if (_SubPath == string.Empty)
                    {
                        dateFolderName = string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        dateFolderName = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), _SubPath);
                    }

                    DirectoryInfo dateFolder = new DirectoryInfo(dateFolderName);

                    if (dateFolder.Exists == true)
                    {
                        FileInfo[] files = dateFolder.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains(logName) == false) continue;
                            if (file.Name.Contains(".log") == false) continue;

                            string fileName = file.Name.Split('.')[0];
                            string[] splitName = fileName.Split('_');
                            string fileCountText = splitName[splitName.Length - 1];

                            if (fileCountText.Contains("H") == true) continue;

                            if (_FileCount < Convert.ToInt32(fileCountText))
                            {
                                _FileCount = Convert.ToInt32(fileCountText);
                            }
                        }

                        _FileCount += 1;

                        _OldStreamName = GetFileStreamName(dateFolderName, DateTime.Now);
                        CreateFileStream(_OldStreamName);
                    }
                }

                //if (_XLogGarbageCollector == null)
                //{
                //    _XLogGarbageCollector = XLogGarbageCollector.Instance;
                //    _XLogGarbageCollector.LogFileStreamClose += this.CloseLogFileStreamByFolderName;
                //    _XLogGarbageCollector.Initialize();
                //}
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"XLog, EXCEPTION={ex.ToString()}");
            }
        }
        public XLog(string logPath, string subPath, int logSplitType, int logMaxLines, string logName, string headerText = "")
        {
            try
            {
                _LogPath = logPath;
                _SubPath = subPath;
                _LogName = logName;
                _LogSplitType = logSplitType;
                _LogMaxLine = logMaxLines;
                _HeaderLineText = headerText;

                _LogWriteThread = new Thread(new ThreadStart(LogWriteThread));
                _LogWriteThread.IsBackground = true;
                _LogWriteThread.Start();

                if (logSplitType == 1)
                {
                    string dateFolderName = string.Empty;// string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyyMMdd"));
                    dateFolderName = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH"));
                    DirectoryInfo dateFolder = new DirectoryInfo(dateFolderName);

                    if (dateFolder.Exists == true)
                    {
                        FileInfo[] files = dateFolder.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains(logName) == false) continue;
                            if (file.Name.Contains(".log") == false) continue;

                            string fileName = file.Name.Split('.')[0];
                            string[] splitName = fileName.Split('_');
                            string fileCountText = splitName[splitName.Length - 1];

                            if (fileCountText.Contains("H") == true) continue;

                            if (_FileCount < Convert.ToInt32(fileCountText))
                            {
                                _FileCount = Convert.ToInt32(fileCountText);
                            }
                        }

                        _FileCount += 1;

                        _OldStreamName = GetFileStreamName(dateFolderName, DateTime.Now);
                        CreateFileStream(_OldStreamName);
                    }
                }
                else if (logSplitType == 2)
                {
                    string dateFolderName = string.Empty;// string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyyMMdd"));
                    if (_SubPath == string.Empty)
                    {
                        dateFolderName = string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        dateFolderName = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), _SubPath);
                    }
                    DirectoryInfo dateFolder = new DirectoryInfo(dateFolderName);

                    if (dateFolder.Exists == true)
                    {
                        FileInfo[] files = dateFolder.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains(logName) == false) continue;
                            if (file.Name.Contains(".log") == false) continue;

                            string fileName = file.Name.Split('.')[0];
                            string[] splitName = fileName.Split('_');
                            string fileCountText = splitName[splitName.Length - 1];

                            if (fileCountText.Contains("H") == true) continue;

                            if (_FileCount < Convert.ToInt32(fileCountText))
                            {
                                _FileCount = Convert.ToInt32(fileCountText);
                            }
                        }

                        _FileCount += 1;

                        _OldStreamName = GetFileStreamName(dateFolderName, DateTime.Now);
                        CreateFileStream(_OldStreamName);
                    }
                }

                //if (_XLogGarbageCollector == null)
                //{
                //    _XLogGarbageCollector = XLogGarbageCollector.Instance;
                //    _XLogGarbageCollector.LogFileStreamClose += this.CloseLogFileStreamByFolderName;
                //    _XLogGarbageCollector.Initialize();
                //}
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"XLog, EXCEPTION={ex.ToString()}");
            }
        }
        #endregion

        #region Public Methods
        public void SetLog(string logText)
        {
            try
            {
                //lock (_LogQueue)
                {
                    string formattedText = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logText);
                    if (_LogQueue.Count < 1000) // QUE 처리가 늦으면서 Thread Scan Time이 느려지는 현상이 생긴다....
                        _LogQueue.Enqueue(formattedText);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"SetLog, EXCEPTION={ex.ToString()}");
            }
        }
        public void SetLog(string[] logText)
        {
            try
            {
                string msg = string.Empty;
                foreach (string text in logText)
                {
                    msg += text;
                    msg += "\r\n";
                }
                SetLog(msg);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"SetLog, EXCEPTION={ex.ToString()}");
            }
        }
        public void Exit()
        {
            _ThreadState = false;
        }
        #endregion

        #region Private Methods
        private void CloseLogFileStreamByFolderName(string folderName)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"CloseLogFileStreamByFolderName, EXCEPTION={ex.ToString()}");
            }
        }
        private void CreateFileStream(string streamName)
        {
            try
            {
                CloseFileStream();
                _RecordLine = 0;
                _OldStreamName = streamName;

                _LogFileStream = new FileStream(streamName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                _LogStreamWriter = new StreamWriter(_LogFileStream);

                if (_HeaderLineText != string.Empty && _LogFileStream.Length == 0)// New File Create
                {
                    string formattedText = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), _HeaderLineText);
                    _LogFileStream.Seek(_LogFileStream.Length, SeekOrigin.Begin);
                    _LogStreamWriter.WriteLine(formattedText);
                    _LogStreamWriter.Flush();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"CreateFileStream, EXCEPTION={ex.ToString()}");
            }
        }
        private string GetFileStreamName(string folderName, DateTime createTime)
        {
            try
            {
                string result = string.Empty;

                if (_LogSplitType == 0)
                {
                    result = string.Format("{0}\\{1}.log", folderName, _LogName);
                }
                else if (_LogSplitType == 1)
                {
                    result = string.Format("{0}\\{1}_{2}H.log", folderName, _LogName, createTime.ToString("HH"));
                }
                else if (_LogSplitType == 2)
                {
                    result = string.Format("{0}\\{1}_{2}.log", folderName, _LogName, _FileCount);
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"GetFileStreamName, EXCEPTION={ex.ToString()}");
                return "Error";
            }
        }
        private void CloseFileStream()
        {
            try
            {
                if (_LogStreamWriter != null) _LogStreamWriter.Close();
                if (_LogFileStream != null) _LogFileStream.Close();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"CloseFileStream, EXCEPTION={ex.ToString()}");
            }
        }
        private void WriteLog(string logText)
        {
            try
            {
                string fileName = string.Format("\\{0}.Log", _LogName);
                string dateFolder = string.Empty;
                if (_LogSplitType == 1)
                {
                    dateFolder = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH"));
                }
                else
                {
                    if (_SubPath == string.Empty)
                    {
                        dateFolder = string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        dateFolder = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), _SubPath);
                    }
                }

                string currentStreamName = dateFolder + fileName;

                if ((_DateFolderExist == false) || (dateFolder != _LogDateFolder))
                {
                    if (Directory.Exists(dateFolder) == false)
                    {
                        Directory.CreateDirectory(dateFolder);
                    }
                    _DateFolderExist = true;
                }

                currentStreamName = GetFileStreamName(dateFolder, DateTime.Now);

                if (_LogSplitType == 2)
                {
                    if ((_RecordLine >= _LogMaxLine) && (_LogSplitType == 2))
                    {
                        _RecordLine = 0;
                        _FileCount += 1;
                        currentStreamName = GetFileStreamName(dateFolder, DateTime.Now);
                    }
                    else if (_OldStreamName != currentStreamName)
                    {
                        _RecordLine = 0;
                        _FileCount = 1;
                        currentStreamName = GetFileStreamName(dateFolder, DateTime.Now);
                    }
                }

                _LogDateFolder = dateFolder;

                if (_OldStreamName != currentStreamName)
                {
                    CreateFileStream(currentStreamName);
                }

                _LogFileStream.Seek(_LogFileStream.Length, SeekOrigin.Begin);
                _LogStreamWriter.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}]{logText}");
                _LogStreamWriter.Flush();

                _RecordLine++;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog($"{logText}, EXCEPTION={ex.ToString()}");
            }
        }
        #endregion

        #region Thread
        private void LogWriteThread()
        {
            string temp = string.Empty;
            while (_ThreadState)
            {
                try
                {
                    //lock (_LogQueue)
                    {
                        DateTime dateTime = DateTime.Now;
                        while ((_LogQueue.Count > 0) && (_WriteCount < 100))
                        {
                            if (_LogQueue.TryDequeue(out temp))
                            {
                                if (string.IsNullOrWhiteSpace(temp) == false)
                                {
                                    _WriteCount += 1;
                                    WriteLog($"({_WriteCount}){temp}");
                                }
                            }
                            //string time = string.Format("{0} ", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                            //System.Diagnostics.Debug.WriteLine($"{time} {this._SubPath} _WriteCount({_WriteCount}), _LogQueue ({_LogQueue.Count}), {(DateTime.Now - dateTime).Milliseconds}");

                            Thread.Sleep(1);
                        }
                    }
                    _WriteCount = 0;
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                }
                finally
                {
                    Thread.Sleep(2);
                }
            }
        }
        #endregion

    }
}
