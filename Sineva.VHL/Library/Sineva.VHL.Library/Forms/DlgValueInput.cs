using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class DlgValueInput : Form
    {
        #region Field
        private bool m_OldValueVisible = false;
        private string m_OldValue = string.Empty;
        private string m_NewValue = string.Empty;
        private Type m_ValueType;
        #endregion

        #region Property
        public string Value
        {
            get { return m_NewValue; }
        }
        #endregion

        #region Constructor
        public DlgValueInput()
        {
            InitializeComponent();
            SetDoubleBuffer();

            m_OldValueVisible = false;
            m_ValueType = typeof(string);
        }
        public DlgValueInput(string value)
            : this()
        {
            m_OldValue = value;
            m_OldValueVisible = true;
        }
        public DlgValueInput(string value, Type type)
            : this(value)
        {
            m_ValueType = type;
        }
        #endregion

        #region Event Handler
        private void DlgValueInput_Load(object sender, EventArgs e)
        {
            if(m_OldValueVisible)
            {
                textBoxOldValue.Text = m_OldValue.ToString();
                panelOldValue.Visible = true;
            }
            else
            {
                panelOldValue.Visible = false;
            }    
        }
        private void iButtonCancel_Click(object sender, EventArgs e)
        {
            ButtonLog.WriteLog(this.Name.ToString(), (sender as Control).Text);
            m_NewValue = string.Empty;

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        private void iButtonOK_Click(object sender, EventArgs e)
        {
            ButtonLog.WriteLog(this.Name.ToString(), (sender as Control).Text);
            if (CheckValidation(ref m_NewValue) == false)
            {
                MessageBox.Show(string.Format("Input error: Not valid for specified data type - {0}", m_ValueType.ToString()));
                textBoxNewValue.Clear();
            }
            else
            {
                ButtonLog.WriteLog(this.Name.ToString(), string.Format("OK Click ! New Value={0}", m_NewValue.ToString()));

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
        #endregion

        #region Method
        private bool CheckValidation(ref string newValue)
        {
            if(m_ValueType == typeof(string))
            {
                newValue = textBoxNewValue.Text;
                return true;
            }

            // Declare Value Type
            bool typeIntFamily = false;
            bool typeFloatFamily = false;
            bool typeIpAddress = false;

            typeIntFamily |= m_ValueType == typeof(Int16) | m_ValueType == typeof(Int32) | m_ValueType == typeof(Int64);
            typeIntFamily |= m_ValueType == typeof(UInt16) | m_ValueType == typeof(UInt32) | m_ValueType == typeof(UInt64);
            typeFloatFamily |= m_ValueType == typeof(Single) | m_ValueType == typeof(Double);
            typeIpAddress |= m_ValueType == typeof(System.Net.IPAddress);

            try
            {
                // Try Parsing by declared type
                string value = textBoxNewValue.Text;
                if(typeIntFamily) newValue = Int64.Parse(value).ToString();
                else if(typeFloatFamily) newValue = Double.Parse(value).ToString();
                else if(typeIpAddress) newValue = System.Net.IPAddress.Parse(value).ToString();

                return true;
            }
            catch(Exception ex)
            {
                // Parsing Fail
				ExceptionLog.WriteLog(ex.ToString()); 
				newValue = string.Empty;
                return false;
            }
        }

        private void SetDoubleBuffer()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.CacheText, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        #endregion
    }
}
