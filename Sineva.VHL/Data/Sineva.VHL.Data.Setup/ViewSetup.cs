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

namespace Sineva.VHL.Data.Setup
{
    public partial class ViewSetup : UserControl
    {
        TreeNode m_CurNode = null;
        TreeNode m_OldNode = null;
        public ViewSetup()
        {
            InitializeComponent();
        }
        public bool Initialize()
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();

            foreach (PropertyInfo info in XFunc.GetProperties(SetupManager.Instance))
            {
                TreeNode node = new TreeNode(info.Name);
                node.Tag = info.GetValue(SetupManager.Instance, null);
                this.treeView.Nodes.Add(node);
            }

            this.treeView.EndUpdate();

            return true;
        }

        public void Save()
        {
            SetupManager.Instance.WriteXml();
            ButtonLog.WriteLog(string.Format("{0}.Setup Save", this.Name.ToString()));
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeView view = sender as TreeView;
            m_CurNode = e.Node;
            this.filteredPropertyGrid1.DeviceRuntimeSettignProperty(m_CurNode.Tag); //.ProjcetSettingProperty(m_CurNode.Tag, false, true);

            if (m_OldNode != null)
            {
                m_OldNode.BackColor = Color.White;
                m_CurNode.BackColor = Color.Yellow;
            }
            m_OldNode = m_CurNode;

            ButtonLog.WriteLog(string.Format("{0}.{1} Mouse Click", this.Name.ToString(), (sender as TreeView).Text));
        }

        public void SetAuthority(AuthorizationLevel level)
        {
            this.filteredPropertyGrid1.SetReadOnlyProperty(level > AuthorizationLevel.Maintenance);
            this.filteredPropertyGrid1.Refresh();
        }
    }
}
