using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Device.ServoControl
{
    public partial class FormSelectDevAxisTags : Form
    {
        private List<_DevAxis> m_AxisPool = new List<_DevAxis>();
        private List<_DevAxis> m_SelectedAxes = new List<_DevAxis>();
        private List<DevAxisTag> m_AxesTag = null;
        private static string m_FilterText = string.Empty;

        #region Properties
        public List<DevAxisTag> AxesTag
        {
            get { return m_AxesTag; }
        }
        #endregion

        public FormSelectDevAxisTags(List<DevAxisTag> Tags)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.CacheText, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            if (ServoControlManager.Instance.Initialize())
            {
                foreach (_DevAxis axis in ServoControlManager.Instance.GetDevAxes()) 
                    if (axis.IsValid) m_AxisPool.Add(axis);

                this.listBoxAxes.DataSource = Filterling(m_AxisPool, m_FilterText, ' ', ',');
                this.textBox1.Text = m_FilterText;
            }

            if (Tags != null)
            {
                foreach (DevAxisTag tag in Tags) m_SelectedAxes.Add(tag.GetDevAxis());
                this.listBoxSelectedAxes.DataSource = m_SelectedAxes;
            }

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
        }

        private List<_DevAxis> Filterling(List<_DevAxis> source, string input, params char[] separator)
        {
            List<_DevAxis> list = new List<_DevAxis>();
            string[] keys = input.Split(separator);
            if (keys.Length > 0)
            {
                foreach (_DevAxis dev in source)
                {
                    bool matchAll = true;
                    foreach (string key in keys)
                    {
                        matchAll &= dev.GetAxis().AxisName.ToLower().Contains(key.ToLower());
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

        private void btnCanel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (m_SelectedAxes.Count > 0)
            {
                m_AxesTag = new List<DevAxisTag>();
                foreach (_DevAxis axis in m_SelectedAxes)
                    m_AxesTag.Add(new DevAxisTag(axis.GetAxis().AxisName, axis.GetAxis().AxisId));
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }

        private void listBoxAxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = this.listBoxAxes.SelectedItem;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                m_FilterText = textBox1.Text;
                this.listBoxAxes.DataSource = Filterling(m_AxisPool, m_FilterText, ' ', ',');
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int selected_counts = this.listBoxAxes.SelectedItems.Count;
            if (selected_counts == 0) return;

            for (int i = 0; i < selected_counts; i++)
            {
                _DevAxis axis = this.listBoxAxes.SelectedItems[i] as _DevAxis;
                m_SelectedAxes.Add(axis);
            }

            this.listBoxSelectedAxes.DataSource = null;
            this.listBoxSelectedAxes.Refresh();
            this.listBoxSelectedAxes.DataSource = m_SelectedAxes;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selected_counts = this.listBoxSelectedAxes.SelectedItems.Count;
            if (selected_counts == 0) return;

            for (int i = 0; i < selected_counts; i++)
            {
                _DevAxis axis = this.listBoxSelectedAxes.SelectedItems[i] as _DevAxis;
                m_SelectedAxes.Remove(axis);
            }
            this.listBoxSelectedAxes.DataSource = null;
            this.listBoxSelectedAxes.Refresh();
            this.listBoxSelectedAxes.DataSource = m_SelectedAxes;
        }
    }
}
