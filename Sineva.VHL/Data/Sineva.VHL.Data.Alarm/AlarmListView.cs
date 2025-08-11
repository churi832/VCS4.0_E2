using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using System.Reflection;

namespace Sineva.VHL.Data.Alarm
{
	public partial class AlarmListView : UserControl
	{
		private BindingSource m_BindSource = new BindingSource();

        public AlarmListView()
        {
            InitializeComponent();

            if(XFunc.IsRunTime())
            {
                CheckForIllegalCrossThreadCalls = false;

                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.CacheText, true);
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }
        }

		public void InitGridView(AlarmListProvider provider)
		{
			this.doubleBufferedGridView1.AutoGenerateColumns = false;
			this.doubleBufferedGridView1.DataSource = m_BindSource;
			m_BindSource.DataSource = provider.AlarmTable.Values;

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.DataPropertyName = "Id";
            colId.HeaderText = "Id";
            colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colId.ReadOnly = true;
            this.doubleBufferedGridView1.Columns.Add(colId);

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.DataPropertyName = "Name";
            colName.HeaderText = "Description";
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.ReadOnly = true;
            this.doubleBufferedGridView1.Columns.Add(colName);

            DataGridViewTextBoxColumn colLevel = new DataGridViewTextBoxColumn();
            colLevel.DataPropertyName = "Level";
            colLevel.HeaderText = "Level";
            colLevel.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colLevel.ReadOnly = true;
            this.doubleBufferedGridView1.Columns.Add(colLevel);

            DataGridViewTextBoxColumn colCode = new DataGridViewTextBoxColumn();
            colCode.DataPropertyName = "Code";
            colCode.HeaderText = "Code";
            colCode.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            colCode.ReadOnly = true;
            this.doubleBufferedGridView1.Columns.Add(colCode);
        }

        public AlarmData[] GetAlarmList()
        {
            List<AlarmData> alarms = new List<AlarmData>();
            foreach(AlarmData item in m_BindSource)
            {
                alarms.Add(item);
            }

            return alarms.ToArray();
            //return m_BindSource.DataSource;
        }

        public void Save()
        {
            AlarmData[] alarms = GetAlarmList();

        }
	}
}
