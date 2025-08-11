using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI
{
    public partial class AlarmForm : GeneralForm, IFormUpdate
    {
        #region Fields
        private bool m_UpdateNeed = false;
        #endregion

        #region Constructor
        public AlarmForm()
        {
            InitializeComponent();
        }
        #endregion

        #region IFormUpdate 멤버
        public bool UpdateNeed
        {
            get { return m_UpdateNeed; }
            set { m_UpdateNeed = value; }
        }
        public void KillTimer()
        {
        }
        #endregion

        private void AlarmForm_Load(object sender, EventArgs e)
        {
            if (XFunc.IsRunTime())
            {
                this.alarmListView1.InitGridView(AlarmListProvider.Instance);
                this.alarmHistoryView1.InitGridView(AlarmHistoryProvider.Instance);
                this.alarmCurrentView1.InitGridView(AlarmCurrentProvider.Instance);
            }
        }

        private void btnCurAlarmSave_Click(object sender, EventArgs e)
        {
            AlarmCurrentProvider.Instance.WriteCsv();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AlarmListProvider.Instance.WriteCsv();
        }
    }
}
