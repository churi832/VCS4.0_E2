using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.EventLog
{
	public partial class EventLogView : UserControl
	{
		private delegate void DelAddEvent(EventLogData data);

		private EventLogProvider m_Provider;
		private BindingSource m_BindSource = new BindingSource();
		private bool m_ColumnCreated = false;
		private string m_QueryDateTimeFilter = "";

		public DataGridView GridView
		{
			get { return this.dataGridView; }
		}

		public DataGridViewSelectedRowCollection SelectedRows
		{
			get { return this.dataGridView.SelectedRows; }
		}

		public EventLogView()
		{
			InitializeComponent();

			CheckForIllegalCrossThreadCalls = false;

			DateTime result = DateTime.Today.Subtract(TimeSpan.FromDays(30));
			dtpFrom.Value = result;

			result = DateTime.Today;
			dtpTo.Value = result;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			//this.backgroundWorker1.DoWork += new DoWorkEventHandler(worker_DoWork);
			//backgroundWorker1.RunWorkerAsync();

            InitGridView(EventLogProvider.Instance);
		}

		public void InitGridView(EventLogProvider provider)
		{
			// Initialzie AlarmList
			m_Provider = provider;
			m_Provider.Viewer = this;

			m_BindSource.DataSource = m_Provider.EventLogTable;

			//Biding to DataTable
			this.dataGridView.AutoGenerateColumns = false;
			this.dataGridView.DataSource = m_BindSource;
			//m_BindSource.Filter = "AlarmTime > #2014/1/23#";

			if (m_ColumnCreated) return;

			DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn();
			colTime.DataPropertyName = "Time";
			colTime.HeaderText = "Time";
			colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colTime.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
			colTime.SortMode = DataGridViewColumnSortMode.Automatic;
			this.dataGridView.Columns.Add(colTime);

			DataGridViewTextBoxColumn colModule1 = new DataGridViewTextBoxColumn();
			colModule1.DataPropertyName = "Module1";
			colModule1.HeaderText = "Module1";
			colModule1.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.dataGridView.Columns.Add(colModule1);

			DataGridViewTextBoxColumn colModule2 = new DataGridViewTextBoxColumn();
			colModule2.DataPropertyName = "Module2";
			colModule2.HeaderText = "Module2";
			colModule2.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.dataGridView.Columns.Add(colModule2);

			DataGridViewTextBoxColumn colMessage = new DataGridViewTextBoxColumn();
			colMessage.DataPropertyName = "Message";
			colMessage.HeaderText = "Message";
			colMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridView.Columns.Add(colMessage);

			this.dataGridView.Sort(colTime, ListSortDirection.Descending);

			m_ColumnCreated = true;
		}

		private void dataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			//m_Provider.UpdateToDB();
		}

		public void AddData(EventLogData data)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (this.dataGridView.InvokeRequired)
			{
				DelAddEvent d = new DelAddEvent(AddData);
				this.Invoke(d, new object[] { data });
			}
			else
			{
				m_Provider.InvokeAdd(data);
			}
		}

		private void dtpFrom_ValueChanged(object sender, EventArgs e)
		{
			DateTime result = dtpFrom.Value;
			//
			if (result > DateTime.Today)
			{
				MessageBox.Show("Serching range overflow", "Select warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			if ((dtpTo.Value - dtpFrom.Value) > TimeSpan.FromDays(30))
			{
				dtpTo.Value = dtpFrom.Value + TimeSpan.FromDays(30);
			}
			//
			SetFilterString(dtpFrom, dtpTo);
		}

		private void dtpTo_ValueChanged(object sender, EventArgs e)
		{
			DateTime result = dtpTo.Value;

			if (dtpTo.Value - dtpFrom.Value > TimeSpan.FromDays(30))
			{
				dtpFrom.Value = result.Subtract(TimeSpan.FromDays(30));
			}

			SetFilterString(dtpFrom, dtpTo);
		}

		private void SetFilterString(DateTimePicker from, DateTimePicker to)
		{
			if (to.Value >= DateTime.Today)
			{
				this.m_QueryDateTimeFilter =
				string.Format("Time >= '{0:yyyy-MM-dd}'",
				from.Value, DateTime.Now);
			}
			else
			{
				this.m_QueryDateTimeFilter =
				string.Format("Time >= '{0:yyyy-MM-dd}' AND Time <= '{1:yyyy-MM-dd}'",
				from.Value, to.Value);
			}
			m_BindSource.Filter = this.m_QueryDateTimeFilter;
		}

        private void btnMemoryClear_Click(object sender, EventArgs e)
        {
            XFunc.CurrentProcessGCMemoryClear();
        }
	}
}
