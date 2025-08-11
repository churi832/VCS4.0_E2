using System;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    internal static class Program
    {
        public static string SelectedNum = string.Empty;

        public static NumSelectForm numSelectForm = null;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            numSelectForm = new NumSelectForm();

            Application.Run(numSelectForm);
        }
    }
}
