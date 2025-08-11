using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using System.Runtime.Remoting.Contexts;

namespace Sineva.VHL.Library
{
	public partial class JogButton : System.Windows.Forms.Button
	{
		#region Fields
		private Color m_ColorMouseUp;
		private Color m_ColorMouseDown;
        private MouseEventArgs m_MouseDownEvent = null;
        private BackgroundWorker myWorker = null;

        private string m_IpAddress = string.Empty;
        private ProcessStartInfo m_StartInfo = null;
        #endregion

        #region Properties
        public Color ColorMouseUp
		{
			get { return m_ColorMouseUp; }
			set { m_ColorMouseUp = value; }
		}

		public Color ColorMouseDown
		{
			get { return m_ColorMouseDown; }
			set { m_ColorMouseDown = value; }
		}
        #endregion

        #region Constructor
        public JogButton()
		{
			InitializeComponent();
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorButton_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorButton_MouseUp);
            this.MouseLeave += new System.EventHandler(this.ColorButton_MouseLeave);

            this.BackColor = m_ColorMouseUp;

            m_StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "netstat -ano | findstr :3389",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Worker Process 생성
            myWorker = new BackgroundWorker();
            myWorker.WorkerSupportsCancellation = true;
            myWorker.DoWork += new DoWorkEventHandler(DoWork);
            myWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
        }
        #endregion

        #region Methods
        private int m_DownCount = 0;
        private void ColorButton_MouseDown(object sender, MouseEventArgs e)
		{
            try
            {
                this.BackColor = m_ColorMouseDown;

                m_MouseDownEvent = e;
                //////////////////////////////////////////////////////////////
                if (myWorker != null && !myWorker.CancellationPending)
                    myWorker.RunWorkerAsync(new string[] { this.Text });
                //////////////////////////////////////////////////////////////
                if (AppConfig.Instance.Simulation.MY_DEBUG) Console.WriteLine($"JogButton Down - {m_DownCount++}");
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private int m_UpCount = 0;
        private void ColorButton_MouseUp(object sender, MouseEventArgs e)
		{
            try
            {
                m_MouseDownEvent = null;
                this.BackColor = m_ColorMouseUp;
                if ((myWorker != null) && myWorker.IsBusy) myWorker.CancelAsync();
                if (AppConfig.Instance.Simulation.MY_DEBUG) Console.WriteLine($"JogButton Up - {m_UpCount++}");
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        private void ColorButton_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (m_MouseDownEvent != null)
                {
                    this.OnMouseUp(m_MouseDownEvent);
                    if (AppConfig.Instance.Simulation.MY_DEBUG) Console.WriteLine($"JogButton Leave");
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region Background Work
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            string[] info = (string[])e.Argument;

            int count = 0;
            bool init_state = true;
            try
            {
                bool connect = true;
                while (connect && !myWorker.CancellationPending)
                {
                    connect = RemoteControlConnected();
                    if (count > 0) init_state = connect;
                    count++;
                    if (AppConfig.Instance.Simulation.MY_DEBUG) Console.WriteLine($"JogButton = {info[0]}, connect = {connect}");

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            if (init_state == true)
                e.Result = new string[] { info[0], "Connected" };
            else e.Result = new string[] { info[0], "Disconnected" };
        }
        private int m_CompletedCount = 0;
        private void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] info = (string[])e.Result;
            if (info.Length == 2)
            {
                if (info[1] == "Disconnected")
                {
                    if (m_MouseDownEvent != null) this.OnMouseUp(m_MouseDownEvent);
                    EventHandlerManager.Instance.InvokeRemoteControlConnected(false);
                    ExceptionLog.WriteLog(string.Format("JogButton.{0} Released, RemoteControl Disconnected", info[0]));
                }
            }
            if (AppConfig.Instance.Simulation.MY_DEBUG) Console.WriteLine($"Remote Monitoring Completed - {info[0]},{info[1]} {m_CompletedCount++}");

        }
        #endregion
        #region Remote Control Monitor
        private bool RemoteControlConnected()
        {
            try
            {
                if (AppConfig.Instance.Simulation.MY_DEBUG) return true;
                using (Process process = new Process { StartInfo = m_StartInfo })
                {
                    process.Start();
                    List<string> outputs = new List<string>();
                    string output = process.StandardOutput.ReadToEnd();
                    outputs = output.Split('\n').ToList();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    foreach (var item in outputs)
                    {
                        if (item.Contains("ESTABLISHED"))
                        {
                            var ipList = Regex.Split(item, "\\s+", RegexOptions.IgnoreCase);
                            if (ipList.Length > 3)
                            {
                                if (ipList[3] != m_IpAddress)
                                {
                                    m_IpAddress = ipList[3];
                                    return false;
                                }
                            }
                            m_IpAddress = ipList[3];
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.Message);
            }
            return false;
        }

        #endregion

    }
}
