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
	public partial class FormTeachingPositionSelect : Form
	{
        private TeachingData m_SelectedTeachingPoint = null;
        private List<TeachingData> m_TeachingPointPool = new List<TeachingData>();

        #region Properties
        public TeachingData SelectedTeachingPoint
		{
            get { return m_SelectedTeachingPoint; }
		}
		#endregion

		#region Constructor
        public FormTeachingPositionSelect()
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
		}
		#endregion

		private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
            TeachingData td = this.listBoxAxisTypes.SelectedItem as TeachingData;
            if (td == null)
            {
                m_SelectedTeachingPoint = null;
            }
            else
            {
                m_SelectedTeachingPoint = td;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Dispose();
            }
		}

		private void btnCanel_Click(object sender, EventArgs e)
		{
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
		}

        private void btnClear_Click(object sender, EventArgs e)
        {
            m_SelectedTeachingPoint = null;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }
	}
}
