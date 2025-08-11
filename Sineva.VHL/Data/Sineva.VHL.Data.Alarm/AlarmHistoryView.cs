using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.Alarm
{
	public partial class AlarmHistoryView : UserControl
	{
		private delegate void AddAlarmCallback(AlarmHistoryData alarm);

		private AlarmHistoryProvider m_Provider;
        private BindingList<AlarmHistoryData> m_BindingList = new BindingList<AlarmHistoryData>();
        private BindingSource m_BindSource = new BindingSource();
		private bool m_ColumnCreated = false;
		private string m_QueryDateTimeFilter = "";

		public DataGridView GridView
		{
			get { return this.doubleBufferedGridView1; }
		}

		public DataGridViewSelectedRowCollection SelectedRows
		{
			get { return this.doubleBufferedGridView1.SelectedRows; }
		}

        public AlarmHistoryView()
        {
            InitializeComponent();

            if(XFunc.IsRunTime())
            {
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
            }
        }

		public void InitGridView(AlarmHistoryProvider provider)
		{
            // Initialzie AlarmList
            m_Provider = provider;
			m_Provider.Viewer = this;

            try
            {
                m_BindingList.Clear();
                foreach (AlarmHistoryData alarm in m_Provider.AlarmHistoryTable.OrderByDescending(x => x.Key).Select(x => x.Value))
                    m_BindingList.Add(alarm);

                //Biding to DataTable
                m_BindSource.DataSource = m_BindingList; // m_Provider.AlarmHistoryTable.Values;
				this.doubleBufferedGridView1.AutoGenerateColumns = false;
				this.doubleBufferedGridView1.DataSource = m_BindSource;
				//m_BindSource.Filter = "AlarmTime > #2014/1/23#";

				if (m_ColumnCreated) return;

				DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn();
				colTime.DataPropertyName = "Time";
				colTime.HeaderText = "Time";
				colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                colTime.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
				colTime.SortMode = DataGridViewColumnSortMode.Automatic;
				this.doubleBufferedGridView1.Columns.Add(colTime);

				DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
				colId.DataPropertyName = "Id";
				colId.HeaderText = "Id";
				colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
				this.doubleBufferedGridView1.Columns.Add(colId);

				DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
				colName.DataPropertyName = "Name";
				colName.HeaderText = "Description";
				colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				this.doubleBufferedGridView1.Columns.Add(colName);

				DataGridViewTextBoxColumn colLevel = new DataGridViewTextBoxColumn();
				colLevel.DataPropertyName = "Level";
				colLevel.HeaderText = "Level";
				colLevel.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
				this.doubleBufferedGridView1.Columns.Add(colLevel);

				//this.doubleBufferedGridView1.Sort(colTime, ListSortDirection.Descending);
				//if (this.doubleBufferedGridView1.CurrentCell != null) this.doubleBufferedGridView1.CurrentCell.Selected = false;
			}
			catch (Exception err)
			{
				ExceptionLog.WriteLog(err.ToString());
			}

			m_ColumnCreated = true;
		}

		public void AddAlarm(AlarmHistoryData alarm)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (this.doubleBufferedGridView1.InvokeRequired)
			{
				AddAlarmCallback d = new AddAlarmCallback(AddAlarm);
				this.Invoke(d, new object[] { alarm });
			}
			else
			{
                m_BindingList.Add(alarm);
                m_Provider.InvokeAdd(alarm);
			}
		}

		private void dataGridViewAlarmHistory_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			SetGridColor(sender as DataGridView);
		}

		private static readonly string _ALARM_L = AlarmLevel.L.ToString();
		private void SetGridColor(DataGridView gridView)
		{
			try
			{
				int count = gridView.Rows.Count;
				DataGridViewRowCollection rows = gridView.Rows;
				DataGridViewRow row;
				for (int i = 0; i < count; i++)
				{
					row = rows[i];
					if (row.Cells[3].Value.ToString() == _ALARM_L)
					{
						row.DefaultCellStyle.ForeColor = Color.Gray;
					}
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message.ToString());
				ExceptionLog.WriteLog(err.ToString());
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
				string.Format("AlarmTime >= '{0:yyyy-MM-dd}'",
				from.Value, DateTime.Now);
			}
			else
			{
				this.m_QueryDateTimeFilter =
				string.Format("AlarmTime >= '{0:yyyy-MM-dd}' AND AlarmTime <= '{1:yyyy-MM-dd}'",
				from.Value, to.Value);
			}
			m_BindSource.Filter = this.m_QueryDateTimeFilter;
		}
	}
}
