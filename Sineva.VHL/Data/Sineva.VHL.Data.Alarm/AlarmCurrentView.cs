using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Data.Alarm
{
	public partial class AlarmCurrentView : UserControl
	{
        private delegate void AlarmCallback(AlarmData alarm);

        private AlarmCurrentProvider m_Provider;
        private BindingList<AlarmData> m_BindingList = new BindingList<AlarmData>();
        private BindingSource m_BindSource = new BindingSource();

        [Category("ICS : UI")]
        public bool ColumnHeadersVisible
        {
            get { return this.dataGridViewCurrentAlarms.ColumnHeadersVisible; }
            set { this.dataGridViewCurrentAlarms.ColumnHeadersVisible = value; }
        }

		public AlarmCurrentView()
		{
			InitializeComponent();

			CheckForIllegalCrossThreadCalls = false;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            m_BindingList.Clear();
		}

		public void InitGridView(AlarmCurrentProvider provider)
		{
			m_Provider = provider;
            m_Provider.eventSetAlarm += new Del_AlarmSet(m_eventSetAlarm);
            m_Provider.eventResetAlarm += new Del_AlarmReset(m_eventResetAlarm);
			m_Provider.Viewer = this;

			//Biding to DataTable
            m_BindSource.DataSource = m_BindingList;
            this.dataGridViewCurrentAlarms.AutoGenerateColumns = false;
			this.dataGridViewCurrentAlarms.DataSource = m_BindSource;
			//m_BindSource.DataSource = m_Provider.AlarmTable.Items;

            this.dataGridViewCurrentAlarms.RowHeadersVisible = false;

			DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.DataPropertyName = "Id"; // "AlarmId";
			colId.HeaderText = "Id";
			colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			this.dataGridViewCurrentAlarms.Columns.Add(colId);

			DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.DataPropertyName = "Name"; // "AlarmName";
			colName.HeaderText = "Description";
			colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewCurrentAlarms.Columns.Add(colName);
		}

        void m_eventSetAlarm(AlarmData alarm)
        {
            if (dataGridViewCurrentAlarms.InvokeRequired)
            {
                Del_AlarmSet d = new Del_AlarmSet(m_eventSetAlarm);
                this.Invoke(d, alarm);
            }
            else
            {
                m_BindingList.Add(alarm);
                m_Provider.InvokeAdd(alarm);
            }
        }

        void m_eventResetAlarm(AlarmData alarm)
        {
            if (dataGridViewCurrentAlarms.InvokeRequired)
            {
                Del_AlarmReset d = new Del_AlarmReset(m_eventResetAlarm);
                this.Invoke(d, alarm);
            }
            else
            {
                AlarmData find_data = null;
                foreach (AlarmData data in m_BindingList) if (data.ID == alarm.ID) find_data = data;
                if (find_data != null) m_BindingList.Remove(find_data);
                m_Provider.InvokeRemove(alarm);
            }
        }

		private void dataGridViewCurrentAlarms_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

		public void AddAlarm(AlarmData alarm)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (this.dataGridViewCurrentAlarms.InvokeRequired)
			{
				AlarmCallback d = new AlarmCallback(AddAlarm);
				this.Invoke(d, new object[] { alarm });
			}
			else
			{
				m_Provider.InvokeAdd(alarm);
			}
		}

		public void RemoveAlarm(AlarmData alarm)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (this.dataGridViewCurrentAlarms.InvokeRequired)
			{
				AlarmCallback d = new AlarmCallback(RemoveAlarm);
				this.Invoke(d, new object[] { alarm });
			}
			else
			{
				m_Provider.InvokeRemove(alarm);
			}
		}

        private void dataGridViewCurrentAlarms_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int columnId = e.ColumnIndex;
            int rowId = e.RowIndex;
            if(columnId < 0 || columnId >= this.dataGridViewCurrentAlarms.ColumnCount) return;
            if(rowId < 0 || rowId >= this.dataGridViewCurrentAlarms.RowCount) return;

            int alarmId = (int)this.dataGridViewCurrentAlarms.Rows[rowId].Cells[0].Value;
            AlarmData alarm = new AlarmData();
            
            //if(AlarmListProvider.Instance.GetAlarm(alarmId, alarm))
            //{
            //    if(m_TroubleShootingDialog == null || m_TroubleShootingDialog.IsDisposed)
            //        m_TroubleShootingDialog = new AlarmCurrentHelpDialog();

            //    m_TroubleShootingDialog.PopupDialog(alarm);
            //}
        }
	}
}
