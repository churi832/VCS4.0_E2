using Sineva.VHL.Data.DbAdapter;
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

namespace Sineva.VHL.Data.Process
{
    public partial class UIEditorPortSelectForm : Form
    {
        #region Fields
        private int m_SelectPortId = 0;
        private List<int> m_PortIds = new List<int>();
        private static string m_FilterText = string.Empty;
        #endregion

        #region Properties
        public int SelectPortID { get => m_SelectPortId; set => m_SelectPortId = value; }
        #endregion

        #region Constructor
        /// <summary>
        /// porttype = -1 :All, 0 : EQP
        /// </summary>
        /// <param name="porttype"></param>
        public UIEditorPortSelectForm(int porttype = -1)
        {
            InitializeComponent();

            try
            {
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.CacheText, true);
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

                if (DatabaseHandler.Instance.DictionaryPortDataList == null || DatabaseHandler.Instance.DictionaryPortDataList.Count == 0)
                    DatabaseHandler.Instance.Initialize_Port();
                if (porttype == -1)
                {
                    m_PortIds.AddRange(DatabaseHandler.Instance.DictionaryPortDataList.Keys);
                }
                else if (porttype == 0)
                {
                    List<DataItem_Port> tempLists = new List<DataItem_Port>();
                    tempLists = DatabaseHandler.Instance.DictionaryPortDataList.Values.Where(x => x.PortType == Library.PortType.LeftEQPort || x.PortType == Library.PortType.RightEQPort || x.PortType == Library.PortType.LeftSTKPort || x.PortType == Library.PortType.RightSTKPort).ToList();
                    m_PortIds = tempLists.Select(x => x.PortID).ToList();
                }

                this.listBoxIds.DataSource = Filterling(m_PortIds, m_FilterText, ' ', ',');
                this.textBox1.Text = m_FilterText;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region Methods
        private List<int> Filterling(List<int> source, string input, params char[] separator)
        {
            try
            {
                List<int> list = new List<int>();
                string[] keys = input.Split(separator);
                if (keys.Length > 0)
                {
                    foreach (int port in source)
                    {
                        bool matchAll = true;
                        foreach (string key in keys)
                        {
                            matchAll &= port.ToString().Contains(key.ToLower());
                        }
                        if (matchAll)
                        {
                            list.Add(port);
                        }
                    }
                }
                if (list.Count > 0)
                    return list;
                else
                    return source;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return source;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_SelectPortId = (int)this.listBoxIds.SelectedItem;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }

        private void btnCanel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                m_FilterText = textBox1.Text;
                this.listBoxIds.DataSource = Filterling(m_PortIds, m_FilterText, ' ', ',');
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            if (text == string.Empty) return;
            m_FilterText = textBox1.Text;
            this.listBoxIds.DataSource = Filterling(m_PortIds, m_FilterText, ' ', ',');
        }
        #endregion
    }
}
