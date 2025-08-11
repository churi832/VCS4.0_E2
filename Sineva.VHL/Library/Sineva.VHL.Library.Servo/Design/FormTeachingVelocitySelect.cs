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
	public partial class FormTeachingVelocitySelect : Form
	{
        private VelocityData m_SelectedTeachingVelocity = null;
        private List<VelocityData> m_VelocityPool = new List<VelocityData>();

        #region Properties
        public VelocityData SelectedVelocityPoint
		{
            get { return m_SelectedTeachingVelocity; }
		}
		#endregion

		#region Constructor
        public FormTeachingVelocitySelect()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
			{
                try
                {
                    foreach (VelocityData td in servo.MovingPropTable)
                        m_VelocityPool.Add(td);
                }
                catch(Exception e)
                {
                    System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                    ExceptionLog.WriteLog(method, string.Format("{0} Teaching Table Null", e.ToString()));
                }
			}
            this.listBoxAxisTypes.DataSource = m_VelocityPool;
		}
		#endregion

		private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
            VelocityData vd = this.listBoxAxisTypes.SelectedItem as VelocityData;
            if (vd == null)
            {
                m_SelectedTeachingVelocity = null;
            }
            else
            {
                m_SelectedTeachingVelocity = vd;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Dispose();
            }
		}

		private void btnCanel_Click(object sender, EventArgs e)
		{
			this.Dispose();
		}

        private void btnClear_Click(object sender, EventArgs e)
        {
            m_SelectedTeachingVelocity = null;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }
	}
}
