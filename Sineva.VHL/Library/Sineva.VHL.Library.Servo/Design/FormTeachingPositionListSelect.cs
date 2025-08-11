using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library.Servo
{
	public partial class FormTeachingPositionListSelect : Form
	{
        private List<TeachingData> m_SelectedTeachingPoint = new List<TeachingData>();
        private List<TeachingData> m_TeachingPointPool = new List<TeachingData>();

        #region Properties
        public List<TeachingData> SelectedTeachingPoint
        {
            get { return m_SelectedTeachingPoint; }
        }
        #endregion

        #region Constructor
        public FormTeachingPositionListSelect(List<TeachingData> datas)
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
                {
                    foreach (TeachingData td in servo.TeachingTable)
                        m_TeachingPointPool.Add(td);
                }
            }
            catch (Exception e)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, string.Format("{0} Teaching Table Null", e.ToString()));
            }
            this.listBoxAxisTypes.DataSource = m_TeachingPointPool;

            if (datas != null)
            {
                foreach (TeachingData data in datas) m_SelectedTeachingPoint.Add(data);
                this.listBoxSelectedAxes.DataSource = m_SelectedTeachingPoint;
            }
        }
        #endregion

        private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int selected_counts = this.listBoxAxisTypes.SelectedItems.Count;
            if (selected_counts == 0) return;

            for (int i = 0; i < selected_counts; i++)
            {
                TeachingData data = this.listBoxAxisTypes.SelectedItems[i] as TeachingData;
                m_SelectedTeachingPoint.Add(data);
            }

            this.listBoxSelectedAxes.DataSource = null;
            this.listBoxSelectedAxes.Refresh();
            this.listBoxSelectedAxes.DataSource = m_SelectedTeachingPoint;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selected_counts = this.listBoxSelectedAxes.SelectedItems.Count;
            if (selected_counts == 0) return;

            for (int i = 0; i < selected_counts; i++)
            {
                TeachingData data = this.listBoxSelectedAxes.SelectedItems[i] as TeachingData;
                m_SelectedTeachingPoint.Remove(data);
            }
            this.listBoxSelectedAxes.DataSource = null;
            this.listBoxSelectedAxes.Refresh();
            this.listBoxSelectedAxes.DataSource = m_SelectedTeachingPoint;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();

        }
        private void btnCanel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }

    }
}
