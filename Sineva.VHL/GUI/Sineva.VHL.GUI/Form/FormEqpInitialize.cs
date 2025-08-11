using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;

namespace Sineva.VHL.GUI
{
	public partial class FormEqpInitialize : Form
    {
        #region Field
        private EqpStateManager m_EqpManager = null;
        private Dictionary<string, InitState> m_InitItems = new Dictionary<string, InitState>();
        private Size m_FormOriginSize = new Size();
        #endregion

        #region Constructor
        public FormEqpInitialize()
        {
            InitializeComponent();

            Initialize();
		}
        private void Initialize()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].HeaderText = "ITEM";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[0].DataPropertyName = XInitTag.PropertyName;
            dataGridView1.Columns[1].HeaderText = "STATUS";
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].DataPropertyName = XInitTag.PropertyState;
            dataGridView1.Columns[2].HeaderText = "CHECK";
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].DataPropertyName = XInitTag.PropertyCheck;

            m_FormOriginSize.Width = this.Width;
            m_FormOriginSize.Height = this.Height;

            m_EqpManager = EqpStateManager.Instance;
            tmrUpdate.Interval = 100;
            tmrUpdate.Start();
        }
		#endregion

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            (sender as DataGridView).ClearSelection();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            try
            {
                bool update = false;
                XInitTag[] stateList = m_EqpManager.GetEqpInitState(out update);

                if (update)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = stateList;
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        switch ((InitState)dataGridView1.Rows[i].Cells[1].Value)
                        {
                            case InitState.Noop:
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                                break;
                            case InitState.Init:
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.DarkOrange;
                                break;
                            case InitState.Fail:
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                                break;
                            case InitState.Comp:
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Green;
                                break;
                            case InitState.Skip:
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Green;
                                break;
                            default:
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        #region Method
        public new void Show()
        {
            this.Visible = true;
        }
        public new void Close()
        {
            this.Visible = false;
        }
        #endregion

        private void btnResize_MouseClick(object sender, MouseEventArgs e)
        {
            if(this.Height >= m_FormOriginSize.Height)
            {
                btnResize.Text = "SHOW PROGRESS";
                this.Height -= (splitContainer1.Panel2.Height + splitContainer1.SplitterWidth);
            }
            else
            {
                btnResize.Text = "HIDE PROGRESS";
                this.Height = m_FormOriginSize.Height;
            }
        }

        private void FormEqpInitialize_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}
