using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Sineva.VHL.Library;
using Sineva.VHL.Device.ServoControl;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.IF.JCS;

namespace Sineva.VHL.Device
{
    public partial class viewDevProperty : UCon
    {
        #region Field
        TreeNode m_CurNode = null;
        TreeNode m_OldNode = null;
        #endregion

        public viewDevProperty() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        public void ClearPanelView()
        {
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel3.ColumnStyles.Clear();
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Clear();
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112));
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ////////////////////////////////////////////////
            
            this.panelDevSettingView.SuspendLayout();
            this.panelDevSettingView.Controls.Clear();
            this.panelDevSettingView.ResumeLayout(false);
        }

        public bool Initialize()
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();

            foreach(PropertyInfo info in XFunc.GetProperties(DevicesManager.Instance))
            {
                System.Reflection.MethodInfo getMethod = info.GetGetMethod();
                object temp = getMethod.Invoke(DevicesManager.Instance, null);

                object[] attributes = info.GetCustomAttributes(typeof(DeviceSettingAttribute), true);
                if (attributes == null || attributes.Length <= 0) continue;

                if ((temp as _Device).IsValid)
                {
                    bool set_type = false;
                    object obj = info.GetCustomAttribute(typeof(DeviceSettingAttribute));
                    //bool IsValid = (obj as DeviceSettingAttribute). ? true : false;
                    if (obj != null) set_type = (obj as DeviceSettingAttribute).IsDeviceSettingType ? true : false;

                    if (set_type)
                    {
                        TreeNode node = new TreeNode(info.Name);
                        node.Tag = info.GetValue(DevicesManager.Instance, null);
                        this.treeView.Nodes.Add(node);
                    }
                }
            }
            foreach (PropertyInfo info in XFunc.GetProperties(OCSCommManager.Instance))
            {
                System.Reflection.MethodInfo getMethod = info.GetGetMethod();
                object temp = getMethod.Invoke(OCSCommManager.Instance, null);

                object[] attributes = info.GetCustomAttributes(typeof(DeviceSettingAttribute), true);
                if (attributes == null || attributes.Length <= 0) continue;
                object obj = info.GetCustomAttribute(typeof(DeviceSettingAttribute));
                if (obj != null && (obj as DeviceSettingAttribute).IsDeviceSettingType)
                {
                    TreeNode node = new TreeNode(info.Name);
                    node.Tag = info.GetValue(OCSCommManager.Instance, null);
                    this.treeView.Nodes.Add(node);
                }
            }
            foreach (PropertyInfo info in XFunc.GetProperties(JCSCommManager.Instance))
            {
                System.Reflection.MethodInfo getMethod = info.GetGetMethod();
                object temp = getMethod.Invoke(JCSCommManager.Instance, null);

                object[] attributes = info.GetCustomAttributes(typeof(DeviceSettingAttribute), true);
                if (attributes == null || attributes.Length <= 0) continue;
                object obj = info.GetCustomAttribute(typeof(DeviceSettingAttribute));
                if (obj != null && (obj as DeviceSettingAttribute).IsDeviceSettingType)
                {
                    TreeNode node = new TreeNode(info.Name);
                    node.Tag = info.GetValue(JCSCommManager.Instance, null);
                    this.treeView.Nodes.Add(node);
                }
            }
            this.treeView.EndUpdate();
            return true;
        }

        public void Save()
        {
            DevicesManager.Instance.WriteXml();
            OCSCommManager.Instance.WriteXml();
            JCSCommManager.Instance.WriteXml();
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeView view = sender as TreeView;
            m_CurNode = e.Node;
            if (m_CurNode.Tag == null) return;

            this.propertyGrid1.DeviceRuntimeSettignProperty(m_CurNode.Tag);

            if (m_OldNode != null)
            {
                m_OldNode.BackColor = Color.White;
                m_CurNode.BackColor = Color.Yellow;
            }
            m_OldNode = m_CurNode;
            SettingpanelDevSettingView(m_CurNode.Text);

        }

        private void SettingpanelDevSettingView(string curNodeName)
        {
            if (curNodeName == "DevTransfer") ;
            else if (curNodeName == "DevAutoTeaching") ;
            else ClearPanelView();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ButtonLog.WriteLog("PropertyGrid", string.Format("Change Value = Old:{0}, New:{1}", e.OldValue, e.ChangedItem));
        }
    }
}
