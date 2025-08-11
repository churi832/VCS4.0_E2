using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
using Sineva.VHL.Library.Servo;
using Sineva.VHL.Device.ServoControl;
using System.Security.Cryptography;

namespace Sineva.VHL.Device
{
	public partial class ucConfigDevices : UserControl
	{
        private DevicesManager m_DevicesFactory = null;
        private InterlockManager m_InterlockFactory = null;
        private TreeNode m_CurNode = null;
        private TreeNode m_OldNode = null;

        public ucConfigDevices()
		{
            this.DoubleBuffered = true;
			InitializeComponent();
		}

        public bool Initialize()
		{
            m_DevicesFactory = DevicesManager.Instance;
            m_DevicesFactory.Initialize();
            m_InterlockFactory = InterlockManager.Instance;
            m_InterlockFactory.Initialize();
            UpdateTreeView();
            return true;
		}

        public TreeNode AddParentNode(TreeView target, object factory)
        {
            TreeNode node = new TreeNode(factory.ToString());
            node.Tag = factory;
            target.Nodes.Add(node);
            return node;
        }

        public TreeNode AddChildNode(TreeNode target, PropertyInfo dev, object factory)
        {
            try
            {
                TreeNode node = new TreeNode(GetNodeName(dev));

                object obj = dev.GetCustomAttribute(typeof(DeviceSettingAttribute));

                bool node_add_condition = true;
                node_add_condition &= obj != null && (obj as DeviceSettingAttribute).IsDeviceSettingType ? true : false;
                node_add_condition &= dev.PropertyType.IsValueType ? false : true;
                node_add_condition &= Type.Equals(dev.PropertyType, typeof(String)) ? false : true;
                if (node_add_condition)
                {
                    node.Tag = dev.GetValue(factory);

                    bool parent_valid = true;
                    if (target.Level > 0)
                    {
                        PropertyInfo _Valid = target.Tag.GetType().GetProperty("IsValid");
                        if (_Valid != null && _Valid.CanRead) parent_valid = (bool)_Valid.GetValue(target.Tag, null);
                    }

                    // _Device Type일때만 IsValid 확인....
                    if (node.Tag != null)
                    {
                        PropertyInfo _Valid = node.Tag.GetType().GetProperty("IsValid");
                        if (_Valid != null && _Valid.CanRead)
                        {
                            object rv = _Valid.GetValue(node.Tag, null);
                            if ((bool)rv == true) 
                            {
                                LoadNodeImage(node, true);
                                if (parent_valid == false) _Valid.SetValue(node.Tag, false);
                            }
                            else LoadNodeImage(node, false);
                        }

                        if (dev.PropertyType.IsGenericType)
                        {
                            System.Collections.IList Items = node.Tag as System.Collections.IList;
                            if (Items.Count == 0) LoadNodeImage(node, false);
                        }

                        if (parent_valid == false) LoadNodeImage(node, false);
                    }
                    else
                    {
                        LoadNodeImage(node, false);
                    }
                    target.Nodes.Add(node);
                }

                if (dev.PropertyType.IsGenericType && obj != null && (obj as DeviceSettingAttribute).IsDeviceSettingType)
                {
                    bool child_valid = false;
                    System.Collections.IList genericItems = (System.Collections.IList)dev.GetValue(target.Tag, null);
                    foreach (var item in genericItems)
                    {
                        TreeNode node1 = new TreeNode(item.ToString());
                        node1.Tag = item;
                        if (node1.Tag != null)
                        {
                            PropertyInfo _Valid1 = node1.Tag.GetType().GetProperty("IsValid");
                            if (_Valid1 != null && _Valid1.CanRead)
                            {
                                object rv = _Valid1.GetValue(node1.Tag, null);
                                if ((bool)rv == true) { LoadNodeImage(node1, true); child_valid = true; }
                                else LoadNodeImage(node1, false);
                            }
                        }
                        else
                        {
                            LoadNodeImage(node1, false);
                        }

                        node.Nodes.Add(node1);

                        foreach (PropertyInfo dev1 in XFunc.GetProperties(item))
                        {
                            if (dev1 == null) continue;
                            object obj1 = dev1.GetCustomAttribute(typeof(DeviceSettingAttribute));
                            bool child_add_condition = true;
                            child_add_condition &= obj1 != null && (obj1 as DeviceSettingAttribute).IsDeviceSettingType ? true : false;
                            child_add_condition &= dev1.PropertyType.IsValueType ? false : true;
                            child_add_condition &= Type.Equals(dev1.PropertyType, typeof(String)) ? false : true;
                            if (child_add_condition)
                            {
                                TreeNode node2 = AddChildNode(node1, dev1, item);
                            }
                        }
                    }

                    if (child_valid) LoadNodeImage(node, true);
                    else LoadNodeImage(node, false);
                }
                else if (obj != null && (obj as DeviceSettingAttribute).IsDeviceSettingType)
                {
                    if (node.Tag != null)
                    {
                        foreach (PropertyInfo dev1 in XFunc.GetProperties(node.Tag))
                        {
                            object obj1 = dev1.GetCustomAttribute(typeof(DeviceSettingAttribute));
                            bool child_add_condition = true;
                            child_add_condition &= obj1 != null && (obj1 as DeviceSettingAttribute).IsDeviceSettingType ? true : false;
                            child_add_condition &= dev1.PropertyType.IsValueType ? false : true;
                            child_add_condition &= Type.Equals(dev1.PropertyType, typeof(String)) ? false : true;
                            if (child_add_condition)
                            {
                                TreeNode node2 = AddChildNode(node, dev1, node.Tag);
                            }
                        }
                    }
                }
                return node;
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString() + dev.Name);
            }

            return null;
        }

        public void UpdateTreeView()
        {
            this.treeView1.BeginUpdate();
            this.treeView1.Nodes.Clear();
            UpdateTreeView(m_DevicesFactory);
            UpdateTreeView(m_InterlockFactory);
            this.treeView1.EndUpdate();
        }
        public void UpdateTreeView(object factory)
		{
            TreeNode node0 = AddParentNode(treeView1, factory);

            foreach (PropertyInfo dev0 in XFunc.GetProperties(factory))
            {
                object obj = dev0.GetCustomAttribute(typeof(DeviceSettingAttribute));
                if (obj != null && (obj as DeviceSettingAttribute).IsDeviceSettingType)
                {
                    TreeNode node1 = AddChildNode(node0, dev0, factory);
                }
            }

            node0.Expand();
		}

        private void LoadNodeImage(TreeNode node, bool IsValid)
        {
            int imageId = 0;
            if (IsValid) imageId = 0;
            else imageId = 1;
            node.ImageIndex = imageId;
            node.SelectedImageIndex = imageId;
            node.StateImageIndex = imageId;
        }

        public void Save()
        {
            m_DevicesFactory.WriteXml();
            m_InterlockFactory.WriteXml();
            UpdateTreeView();
        }

        private string GetNodeName(PropertyInfo dev)
        {
            bool rv = false;

            rv |= Type.Equals(dev.PropertyType, typeof(IoTag));
            rv |= Type.Equals(dev.PropertyType, typeof(List<IoTag>));
            rv |= Type.Equals(dev.PropertyType, typeof(List<TeachingData>));
            rv |= Type.Equals(dev.PropertyType, typeof(List<VelocityData>));

            string name = dev.Name;
            if (rv) name += "*";
            return name;
        }

		private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
            TreeView view = sender as TreeView;
            m_CurNode = e.Node;
            if (m_CurNode.Tag == null) return;

            //this.propertyGrid1.DeviceRuntimeSettignProperty(m_CurNode.Tag);
            propertyGrid1.SelectedObject= m_CurNode.Tag;

            MouseButtons button = e.Button;
            if (button == System.Windows.Forms.MouseButtons.Right)
            {
                bool generic_type = m_CurNode.Tag.GetType().IsGenericType;
                bool iotaglist_type = Type.Equals(m_CurNode.Tag.GetType(), typeof(List<IoTag>));
                bool iotag_type = Type.Equals(m_CurNode.Tag.GetType(), typeof(IoTag));

                if (iotag_type || iotaglist_type)
                {
                    if (m_CurNode.ContextMenuStrip == null)
                    {
                        m_CurNode.ContextMenuStrip = this.contextMenuStripIO;
                    }
                    if (generic_type)
                    {
                        m_CurNode.ContextMenuStrip.Items["IoAdd"].Enabled = true;
                        m_CurNode.ContextMenuStrip.Items["IoChange"].Enabled = false;
                        m_CurNode.ContextMenuStrip.Items["IoRemove"].Enabled = true;
                    }
                    else
                    {
                        m_CurNode.ContextMenuStrip.Items["IoAdd"].Enabled = false;
                        m_CurNode.ContextMenuStrip.Items["IoChange"].Enabled = true;
                        m_CurNode.ContextMenuStrip.Items["IoRemove"].Enabled = false;
                    }
                }

                bool teaching_data_type = Type.Equals(m_CurNode.Tag.GetType(), typeof(List<TeachingData>));
                bool velocity_data_type = Type.Equals(m_CurNode.Tag.GetType(), typeof(List<VelocityData>));
                if (generic_type && (teaching_data_type || velocity_data_type))
                {
                    if (m_CurNode.ContextMenuStrip == null)
                    {
                        m_CurNode.ContextMenuStrip = this.contextMenuStripTeachingPoint;
                    }
                    if (teaching_data_type)
                    {
                        m_CurNode.ContextMenuStrip.Items["TeachingDataAdd"].Enabled = true;
                        m_CurNode.ContextMenuStrip.Items["VelocityDataAdd"].Enabled = false;
                    }
                    else
                    {
                        m_CurNode.ContextMenuStrip.Items["TeachingDataAdd"].Enabled = false;
                        m_CurNode.ContextMenuStrip.Items["VelocityDataAdd"].Enabled = true;
                    }
                }
            }

            if (m_OldNode != null) this.m_OldNode.ForeColor = Color.Black;
            this.m_CurNode.ForeColor = Color.Red;
            this.m_OldNode = m_CurNode;
		}

        private void IoAdd_Click(object sender, EventArgs e)
        {
			using (FormIoSelect form = new FormIoSelect())
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                	List<IoTag> Devs = m_CurNode.Tag as List<IoTag>;
                    Devs.Add(form.m_Tag);
                    UpdateTreeView();
                }
			}
        }

        private void IoRemove_Click(object sender, EventArgs e)
        {
            bool generic_type = m_CurNode.Parent.Tag.GetType().IsGenericType;
            bool iotaglist_type = Type.Equals(m_CurNode.Parent.Tag.GetType(), typeof(List<IoTag>));
            if (generic_type && iotaglist_type)
            {
                List<IoTag> Devs = m_CurNode.Parent.Tag as List<IoTag>;
                Devs.Remove(m_CurNode.Tag as IoTag);
                UpdateTreeView();
            }
            else
            {
                MessageBox.Show("Cann't Io Tag");
            }
        }

        private void IoChange_Click(object sender, EventArgs e)
        {
            using (FormIoSelect form = new FormIoSelect())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    (m_CurNode.Tag as IoTag).Name = form.m_Tag.Name;
                    (m_CurNode.Tag as IoTag).IoType = form.m_Tag.IoType;
                    (m_CurNode.Tag as IoTag).IsInverted = form.m_Tag.IsInverted;
                    UpdateTreeView();
                }
            }
        }

        private void TeachingDataAdd_Click(object sender, EventArgs e)
        {
            using (FormTeachingPositionSelect form = new FormTeachingPositionSelect())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<TeachingData> Devs = m_CurNode.Tag as List<TeachingData>;
                    Devs.Add(form.SelectedTeachingPoint);
                    UpdateTreeView();
                }
            }
        }

        private void VelocityDataAdd_Click(object sender, EventArgs e)
        {
            using (FormTeachingVelocitySelect form = new FormTeachingVelocitySelect())
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<VelocityData> Devs = m_CurNode.Tag as List<VelocityData>;
                    Devs.Add(form.SelectedVelocityPoint);
                    UpdateTreeView();
                }
            }
        }
    }
}
