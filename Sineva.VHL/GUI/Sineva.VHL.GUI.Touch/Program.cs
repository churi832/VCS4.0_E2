using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.Touch
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Process[] process_gui1 = Process.GetProcessesByName("Sineva.VHL.GUI.Touch");
            Process[] process_gui2 = Process.GetProcessesByName("Sineva.VHL.GUI.Touch.vshost");
            Process[] process_gui3 = Process.GetProcessesByName("Touch");
            if (process_gui1.Length >= 0 && process_gui2.Length > 0 && process_gui3.Length > 0)
            {
                for (int i = 0; i < process_gui1.Length; i++) process_gui1[i].Kill();
                for (int i = 0; i < process_gui2.Length; i++) process_gui2[i].Kill();
                for (int i = 0; i < process_gui3.Length; i++) process_gui3[i].Kill();
            }
            // Show the system tray icon.					
            using (ProcessTrays pi = new ProcessTrays())
            {
                pi.Display();
                Application.Run();
            }
        }
    }
}
