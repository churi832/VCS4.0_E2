using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.Touch
{
    public delegate void DelVoid_Int(int a);

    public class ProcessTrays : IDisposable
    {
        #region Fields
        private NotifyIcon ni;
        MainForm MAIN_Form;

        public event DelVoid_Int UpdateTrayIconStatus = null;
        #endregion

        #region Constructor
        public ProcessTrays()
        {
            TrayIconRefresh.RefreshTrayArea();

            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            Application.ThreadExit += new EventHandler(this.OnThreadExit);
            // Instantiate the NotifyIcon object.
            ni = new NotifyIcon();
        }
        /// <summary>
        /// Displays the icon in the system tray.
        /// </summary>
        public void Display()
        {
            // Put the icon in the system tray and allow it react to mouse clicks.			
            ni.MouseClick += new MouseEventHandler(ni_MouseClick);
            ni.Icon = Sineva.VHL.GUI.Touch.Properties.Resources.Comp26;
            ni.Text = "Touch Program";
            ni.Visible = true;

            // Attach a context menu.
            ContextMenus menu = new ContextMenus();
            menu.ProgramExit += Menu_ProgramExit;
            menu.MainFormShow += Menu_MainFormShow;
            ni.ContextMenuStrip = menu.menu_strip;

            MAIN_Form = new MainForm();
            MAIN_Form.Hide();
        }

        private void Menu_MainFormShow(bool a)
        {
            try
            {
                if (!MAIN_Form.Visible)
                {
                    if (MAIN_Form.IsDisposed)
                    {
                        MAIN_Form.Dispose();
                        MAIN_Form = null;
                        MAIN_Form = new MainForm();
                    }
                    MAIN_Form.Show();
                }
                else
                {
                    MAIN_Form.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            // When the application closes, this will remove the icon from the system tray immediately.
            ni.Dispose();
            MAIN_Form.Dispose();

            Application.Exit();
        }
        #endregion

        #region Methods
        private void OnApplicationExit(object sender, EventArgs e)
        {

        }
        private void OnThreadExit(object sender, EventArgs e)
        {

        }
        private void Menu_ProgramExit(bool b)
        {
            Dispose();
        }
        /// <summary>
        /// Handles the MouseClick event of the ni control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void ni_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                // Handle mouse button clicks.
                if (e.Button == MouseButtons.Left)
                {
                    Menu_MainFormShow(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please wait until Program ready.");
            }
        }
        #endregion
    }
}
