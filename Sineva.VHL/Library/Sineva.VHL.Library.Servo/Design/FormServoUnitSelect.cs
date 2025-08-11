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
	public partial class FormServoUnitSelect : Form
	{
		private ServoUnit m_SelectedServo = null;
		private List<ServoUnit> m_ServoPool = new List<ServoUnit>();
        private ServoUnitTag m_ServoTag = null;
		#region Properties
		public ServoUnit SelectedServo
		{
			get { return m_SelectedServo; }
		}
        public ServoUnitTag ServoTag
        {
            get { return m_ServoTag; }
        }
		#endregion

		#region Constructor
        public FormServoUnitSelect()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			foreach (ServoUnit servo in ServoManager.Instance.ServoUnits)
			{
                m_ServoPool.Add(servo);
			}
            this.listBoxAxisTypes.DataSource = m_ServoPool;
		}
		#endregion

		private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
            ServoUnit servo = this.listBoxAxisTypes.SelectedItem as ServoUnit;
            if (servo == null)
            {
                m_SelectedServo = null;
                m_ServoTag = null;
            }
            else
            {
                m_SelectedServo = servo;
                m_ServoTag = new ServoUnitTag(servo.ServoName);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Dispose();
            }
		}

		private void btnCanel_Click(object sender, EventArgs e)
		{
			this.Dispose();
		}
	}
}
