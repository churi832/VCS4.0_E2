using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Sineva.VHL.Library
{
	public partial class LogList : UserControl
    {
        #region Fields
        private static Queue<LogItem> LogQ = new Queue<LogItem>();
		private static Queue<LogItem> LogTempQ = new Queue<LogItem>();
		private readonly static object m_LockKey = new object();
		private int m_MaxCount = 30;
		private static bool m_LogAble = false;
        private bool m_FilterHide = false;

        private bool m_UseSequenceLog = false;
        private bool m_UseManualLog = false;
        private bool m_UseServoLog = false;
        private bool m_UseExceptionLog = false;
        private bool m_UseOcsLog = false;
        private bool m_UseJcsLog = false;
        private bool m_UseMxpLog = false;
        private bool m_UseSerialLog = false;
        private bool m_UseTactTimeLog = false;
        private bool m_UseServoAxisLog = false;
        private bool m_UseVisionLog = false;
        private bool m_UseButtonLog = false;

        private ToolTip m_ToolTip = new ToolTip();
        private System.Windows.Forms.CheckedListBox clbItems;
        private Queue<string> m_MessageQue = new Queue<string>();
        #endregion

        #region Propertry
        public bool FilterHide
        {
            get { return m_FilterHide; }
            set { m_FilterHide = value; UnitBoxHide(value); }
        }

		public bool LogAble
		{
			get { return m_LogAble; }
			set 
			{ 
				m_LogAble = value;
				tmUpdate.Enabled = value;
				tmUpdate.Interval = 100;
			}
		}

		public int MaxCount
		{
			get { return m_MaxCount; }
			set { m_MaxCount = value; }
		}

        public bool UseSequenceLog
        {
            get { return m_UseSequenceLog; }
            set { m_UseSequenceLog = value; }
        }
        public bool UseManualLog
        {
            get { return m_UseManualLog; }
            set { m_UseManualLog = value; }
        }
        public bool UseServoLog
        {
            get { return m_UseServoLog; }
            set { m_UseServoLog = value; }
        }
        public bool UseExceptionLog
        {
            get { return m_UseExceptionLog; }
            set { m_UseExceptionLog = value; }
        }
        public bool UseOcsLog
        {
            get { return m_UseOcsLog; }
            set { m_UseOcsLog = value; }
        }
        public bool UseJcsLog
        {
            get { return m_UseJcsLog; }
            set { m_UseJcsLog = value; }
        }
        public bool UseMxpLog
        {
            get { return m_UseMxpLog; }
            set { m_UseMxpLog = value; }
        }
        public bool UseSerialLog
        {
            get { return m_UseSerialLog; }
            set { m_UseSerialLog = value; }
        }
        public bool UseTactTimeLog
        {
            get { return m_UseTactTimeLog; }
            set { m_UseTactTimeLog = value; }
        }
        public bool UseServoAxisLog
        {
            get { return m_UseServoAxisLog; }
            set { m_UseServoAxisLog = value; }
        }
        public bool UseVisionLog
        {
            get { return m_UseVisionLog; }
            set { m_UseVisionLog = value; }
        }
        public bool UseButtonLog
        {
            get { return m_UseButtonLog; }
            set { m_UseButtonLog = value; }
        }
        #endregion

        public LogList()
		{
			InitializeComponent();
            if(XFunc.IsRunTime())
            {
                this.clbItems = new System.Windows.Forms.CheckedListBox();
            }
		}

        public bool Initialize()
        {
            if (UseSequenceLog) SequenceLog.logUpdate += Instance_logUpdate;
            if (UseManualLog) ManualLog.logUpdate += Instance_logUpdate;
            if (UseServoLog) ServoLog.logUpdate += Instance_logUpdate;
            if (UseExceptionLog) ExceptionLog.logUpdate += Instance_logUpdate;
            if (UseOcsLog) OcsCommLog.logUpdate += Instance_logUpdate;
            if (UseJcsLog) JcsCommLog.logUpdate += Instance_logUpdate;
            if (UseButtonLog) ButtonLog.logUpdate += Instance_logUpdate;
            if (UseMxpLog) MxpCommLog.logUpdate += Instance_logUpdate;
            if (UseSerialLog) SerialCommLog.logUpdate += Instance_logUpdate;
            if (UseTactTimeLog) TactTimeLog.logUpdate += Instance_logUpdate;
            if (UseVisionLog) VisionLog.logUpdate += Instance_logUpdate;
            if (UseButtonLog) ButtonLog.logUpdate += Instance_logUpdate;

            tmUpdate.Enabled = LogAble;
            
            return true;
        }

        private void UnitBoxHide(bool flg)
        {
            if (!flg)
            {
                // 
                // clbItems
                // 
                this.clbItems.Dock = System.Windows.Forms.DockStyle.Fill;
                this.clbItems.FormattingEnabled = true;
                this.clbItems.Location = new System.Drawing.Point(357, 0);
                this.clbItems.Margin = new System.Windows.Forms.Padding(0);
                this.clbItems.Name = "clbItems";
                this.clbItems.Size = new System.Drawing.Size(200, 186);
                this.clbItems.TabIndex = 0;
                this.clbItems.SelectedIndexChanged += new System.EventHandler(this.clbItems_SelectedIndexChanged);
            }

            this.tableLayoutPanel1.SuspendLayout();
            if (!flg)
            {
                this.tableLayoutPanel1.ColumnCount = 2;
                this.tableLayoutPanel1.Controls.Add(this.clbItems, 1, 0);
            }
            else
            {
                this.tableLayoutPanel1.ColumnCount = 1;
            }
            this.tableLayoutPanel1.ResumeLayout(false);
        }

        private void Instance_logUpdate(string val1, string val2)
        {
            if (this.lbLog.InvokeRequired)
            {
                DelVoid_StringString d = new DelVoid_StringString(Instance_logUpdate);
                this.Invoke(d, val1, val2);
            }
            else
            {
                AddMsg(val1, val2);
            }
        }

        private void AddMsg(string unit, string msg)
        {
			lock (m_LockKey)
            {
                try
                {
                    string sLog = string.Format("{0} | {1}", unit, msg);
                    m_MessageQue.Enqueue(sLog);

                    //lbLog.Items.Add(sLog);
                    //if (lbLog.Items.Count > m_MaxCount) lbLog.Items.RemoveAt(0);
                    //if (lbLog.Items.Count > 0) lbLog.SetSelected(lbLog.Items.Count - 1, true);
                    //if (m_LogAble && !m_FilterHide) EnQ(unit, msg);
                }
                catch(Exception err)
                {
                    ExceptionLog.WriteLog(string.Format("LogList AddMsg Error - {0}", err.ToString()));
                }
            }
        }

		public static void EnQ(string unit, string msg)
		{
			lock (m_LockKey)
			{
				if (m_LogAble)
				{
					string str = string.Format("{0}\t\t{1}\r\n", unit, msg);
					LogTempQ.Enqueue(new LogItem(DateTime.Now, unit, msg));
				}
			}
		}

		private void tmUpdate_Tick(object sender, EventArgs e)
		{
			lock (m_LockKey)
			{
                if (m_MessageQue.Count > 0)
                {
                    lbLog.Items.Add(m_MessageQue.Dequeue());
                    if (lbLog.Items.Count > m_MaxCount) lbLog.Items.RemoveAt(0);
                    if (lbLog.Items.Count > 0) lbLog.SetSelected(lbLog.Items.Count - 1, true);
                    m_MessageQue.Clear();
                }
                //if (LogTempQ.Count > 0)
                //{
                //    do
                //    {
                //        LogItem item = LogTempQ.Dequeue();
                //        if (LogQ.Count > m_MaxCount) LogQ.Dequeue();
                //        LogQ.Enqueue(item);
                //        AddUnit();
                //        ReflashLogBox();
                //    }
                //    while (LogTempQ.Count > 0);
                //}
            }
		}

		private void AddUnit()
		{
            if (FilterHide) return;

			string units = "";
			clbItems.Items.Clear();
			foreach (LogItem log in LogQ.ToList())
			{
				if (!units.Contains(log.UnitName))
				{
					units += string.Format("{0},", log.UnitName);
					clbItems.Items.Add(log.UnitName);
				}
			}
		}

		private string GetFilterUnits()
		{
			string filter = "";
			foreach (object item in clbItems.CheckedItems) filter += string.Format("{0},", item.ToString());
			return filter;
		}

		private void ReflashLogBox()
		{
            bool all = false;
            List<string> datas = new List<string>();
			string filter = GetFilterUnits();
			if (filter == "" || FilterHide) all = true;

			foreach (LogItem log in LogQ.ToList())
			{
                if (all || filter.Contains(log.UnitName)) datas.Add(log.Message);
			}
            lbLog.DataSource = null;
            lbLog.Refresh();
            lbLog.DataSource = datas;
            if (lbLog.Items.Count > 0)
    			lbLog.SetSelected(lbLog.Items.Count - 1, true);
		}

		private void clbItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			ReflashLogBox();
		}

        private void lbLog_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_ToolTip.RemoveAll();
                string msg = string.Format("{0}", lbLog.SelectedItem);
                m_ToolTip.Show(msg, lbLog, 5000);
            }
        }
    }

    public class LogItem
	{
		#region Fields
		private DateTime m_LogTime;
		private string m_UnitName;
		private string m_Message;
		#endregion
		#region Properties
		public DateTime LogTime
		{
			get { return m_LogTime; }
			set { m_LogTime = value; }
		}
		public string UnitName
		{
			get { return m_UnitName; }
			set { m_UnitName = value; }
		}
		public string Message
		{
			get { return m_Message; }
			set { m_Message = value; }
		}
		#endregion

		#region Constructor
		public LogItem(DateTime time, string unit, string msg)
		{
			LogTime = time;
			UnitName = unit;
			Message = msg;
		}
		#endregion
	}
}
