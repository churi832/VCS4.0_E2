using ICS.GUI;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI
{
    static class Program
    {
        private static uint m_RecentActivationTick = 0;
        private static bool m_AutoScreenLockActivated = false;

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                ThreadStart threadDelegate = new ThreadStart(SplashWindow.DoWork);
                Thread newThread = new Thread(threadDelegate);
                newThread.IsBackground = true;
                newThread.Start();

                Process[] process_gui = Process.GetProcessesByName("Sineva.VHL.GUI");
                if (process_gui.Length <= 1)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    ProcessPriorityClass priotiry = ProcessPriorityClass.RealTime;
                    process_gui[0].PriorityClass = priotiry;

                    AppConfig.Instance.Initialize();
                    /// RemoteTouch Program Start
                    ExecuteRemoteTouch();

                    m_RecentActivationTick = XFunc.GetTickCount();
                    System.Windows.Forms.Timer tmrActiveMonitor = new System.Windows.Forms.Timer();
                    tmrActiveMonitor.Interval = 1000;
                    tmrActiveMonitor.Tick += tmrActiveMonitor_Tick;
                    tmrActiveMonitor.Start();
                    Sineva.VHL.Library.InteropServices.KeyboardHooking keyboardHook = new Sineva.VHL.Library.InteropServices.KeyboardHooking(false, true);
                    Sineva.VHL.Library.InteropServices.MouseHooking mouseHook = new Sineva.VHL.Library.InteropServices.MouseHooking(false);
                    keyboardHook.KeyDown += keyboardHook_KeyDown;
                    mouseHook.MouseClick += mouseHook_MouseClick;
                    mouseHook.MouseDblClick += mouseHook_MouseDblClick;

                    Application.Run(new MainForm());
                    string s = Application.ProductName;
                    AppConfig.AppMainDisposed = true;
                    keyboardHook.Dispose();

                    Application.ExitThread();
                    Application.Exit();
                }
                else
                {
                    ////중복실행에 대한 처리
                    //System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("[{0}] Program Running... System OFF!", process_gui[0].ToString());
                    foreach (Process p in process_gui) p.Kill();
                }
            }
            catch
            {
                MessageBox.Show("Program Exit !");
            }
        }

        // hjyou : Splash Window display 
        class SplashWindow
        {
            public static void DoWork()
            {
                Application.Run(new SplashScreen(Sineva.VHL.GUI.Properties.Resources.logo, 5000));
                //Application.Run(new FormConfig());
            }
        }

        #region 자동 LogOff
        static void mouseHook_MouseDblClick(MouseEventArgs e)
        {
            m_RecentActivationTick = XFunc.GetTickCount();
        }

        static void mouseHook_MouseClick(MouseEventArgs e)
        {
            m_RecentActivationTick = XFunc.GetTickCount();
        }

        static void keyboardHook_KeyDown(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            m_RecentActivationTick = XFunc.GetTickCount();
        }

        static void tmrActiveMonitor_Tick(object sender, EventArgs e)
        {
            uint curTickCount = XFunc.GetTickCount();
            if (Sineva.VHL.Data.LogIn.AccountManager.Instance.CurAccount != null)
            {
                if (curTickCount - m_RecentActivationTick > AppConfig.Instance.AutoLogoutTime * 60 * 1000)
                {
                    Sineva.VHL.Data.LogIn.AccountManager.Instance.LogOff(true);
                }

                if (m_AutoScreenLockActivated == true)
                {
                    m_AutoScreenLockActivated = false;
                    EventHandlerManager.Instance.InvokeAutoScreenLockActivate(false);
                }
            }

            if (m_AutoScreenLockActivated == false && EqpStateManager.Instance.OpMode == OperateMode.Manual)
            {
                if (curTickCount - m_RecentActivationTick > AppConfig.Instance.AutoScreenLockTime * 60 * 1000)
                {
                    m_AutoScreenLockActivated = true;
                    EventHandlerManager.Instance.InvokeAutoScreenLockActivate(true);
                }
            }
        }
        #endregion
        #region Remote Touch Execution
        static void ExecuteRemoteTouch()
        {
            //////////////////////////////////////////////////////////////////////
            // GUI.Vision Program Start
            //////////////////////////////////////////////////////////////////////
            Process[] process_gui1 = Process.GetProcessesByName("Sineva.VHL.GUI.Touch");
            Process[] process_gui2 = Process.GetProcessesByName("Sineva.VHL.GUI.Touch.vshost");
            Process[] process_gui3 = Process.GetProcessesByName("Touch");
            if (process_gui1.Length < 1 && process_gui2.Length < 1 && process_gui3.Length < 1)
            {
                string vision_exe_path = AppConfig.GetAppRootPath() + "\\Sineva.VHL.GUI.Touch.exe";
                if (System.IO.File.Exists(vision_exe_path))
                {
                    ProcessStartInfo info = new ProcessStartInfo(vision_exe_path);
                    Process.Start(info);
                }
            }
            //////////////////////////////////////////////////////////////////////
        }
        #endregion 
    }
}
