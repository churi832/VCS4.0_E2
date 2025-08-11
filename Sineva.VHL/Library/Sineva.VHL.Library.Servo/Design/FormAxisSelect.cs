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
	public partial class FormAxisSelect : Form
	{
        private List<_Axis> m_AxisPool = new List<_Axis>();
        private _Axis m_SelectedAxis = null;
        private AxisTag m_AxisTag = null;
        private static string m_FilterText = string.Empty;

        #region Properties
        public _Axis SelectedAxis
		{
            get { return m_SelectedAxis; }
		}
        public AxisTag AxisTag
        {
            get { return m_AxisTag; }
        }
        #endregion

		#region Constructor
        public FormAxisSelect()
		{
            InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            if (ServoManager.Instance.Initialize())
			{
                foreach (_AxisBlock block in ServoManager.Instance.AxisBlocks)
				{
                    foreach (_Axis axis in block.Axes)
					{
                        if (axis.AxisName == string.Format("Mp{0}Spare{1}", block.BlockId, axis.AxisId)) continue;
                        if (axis.AxisName.Contains("HomeOffset")) continue;
                        m_AxisPool.Add(axis);
					}
				}
                this.listBoxAxisTypes.DataSource = Filterling(m_AxisPool, m_FilterText, ' ', ',');
                this.textBox1.Text = m_FilterText;
            }

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
        }
		#endregion

		private void listBoxAxisTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.propertyGrid1.SelectedObject = this.listBoxAxisTypes.SelectedItem;
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
            _Axis axis = this.listBoxAxisTypes.SelectedItem as _Axis;
            if (axis == null)
            {
                m_SelectedAxis = null;
                m_AxisTag = null;
            }
            else
            {
                m_SelectedAxis = this.listBoxAxisTypes.SelectedItem as _Axis;

                m_AxisTag = new AxisTag();
                m_AxisTag.AxisName = m_SelectedAxis.AxisName;
                m_AxisTag.AxisId = m_SelectedAxis.AxisId;
                //m_AxisTag = new AxisTag(m_SelectedAxis.AxisName, m_SelectedAxis.AxisId);

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
            m_AxisTag = null;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }

		private List<_Axis> Filterling(List<_Axis> source, string input, params char[] separator)
		{
			List<_Axis> list = new List<_Axis>();
			string[] keys = input.Split(separator);
			if (keys.Length > 0)
			{
				foreach (_Axis dev in source)
				{
					bool matchAll = true;
					foreach (string key in keys)
					{
						matchAll &= dev.AxisName.ToLower().Contains(key.ToLower());
					}
					if (matchAll)
					{
						list.Add(dev);
					}
				}
			}
			if (list.Count > 0)
				return list;
			else
				return source;
		}

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                m_FilterText = textBox1.Text;
                this.listBoxAxisTypes.DataSource = Filterling(m_AxisPool, m_FilterText, ' ', ',');
            }
        }
    }
}
