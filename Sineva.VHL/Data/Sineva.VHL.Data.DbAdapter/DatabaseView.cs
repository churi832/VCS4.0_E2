using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using System.Reflection;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;

namespace Sineva.VHL.Data.DbAdapter
{
	public partial class DatabaseView : UserControl
	{
		private BindingSource m_BindSource = new BindingSource();
        TreeNode m_CurNode = null;
        TreeNode m_OldNode = null;

        public DatabaseView()
        {
            InitializeComponent();

            if(XFunc.IsRunTime())
            {
                CheckForIllegalCrossThreadCalls = false;

                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.CacheText, true);
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }
        }
        public bool Initialize()
        {
            try
            {
                this.cbUpdateTeachingSamePortType.Visible = false;
                this.treeView.BeginUpdate();
                this.treeView.Nodes.Clear();

                foreach (PropertyInfo info in XFunc.GetProperties(DatabaseHandler.Instance))
                {
                    object obj = info.GetCustomAttribute(typeof(DatabaseSettingAttribute));
                    if (obj != null && (obj as DatabaseSettingAttribute).IsSettingType)
                    {
                        string name = info.Name;
                        object[] attributeDisplayName = info.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                        if (attributeDisplayName != null && attributeDisplayName.Length > 0)
                            name = (attributeDisplayName[0] as DisplayNameAttribute).DisplayName;

                        TreeNode node = new TreeNode(name);
                        node.Tag = info.GetValue(DatabaseHandler.Instance, null);
                        this.treeView.Nodes.Add(node);
                    }
                }
                this.treeView.EndUpdate();

                DatabaseHandler.Instance.CallBack_NonQuery += QueryPort_CallBack_NonQuery;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return true;
        }

        private void QueryPort_CallBack_NonQuery(int nResult)
        {
            try
            {
                if (this.lbProcessCount.InvokeRequired)
                {
                    NonQueryCallback d = new NonQueryCallback(QueryPort_CallBack_NonQuery);
                    this.Invoke(d, nResult);
                }
                else
                {
                    if (nResult > 0) lbProcessCount.Text += string.Format("\r\n OK");
                    else lbProcessCount.Text += string.Format("\r\n NG");
                }
            }
            catch (Exception ex)
            {
                string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, ex.Message);
                ExceptionLog.WriteLog(log);
            }
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                System.Windows.Forms.TreeView view = sender as System.Windows.Forms.TreeView;
                m_CurNode = e.Node;
                if (m_CurNode.Tag == null) return;

                UpdateGridView(m_CurNode.Tag);
                bool enable_button = false;
                enable_button |= m_CurNode.Text == "Port" ? true : false;
                enable_button |= m_CurNode.Text == "Link" ? true : false;
                enable_button |= m_CurNode.Text == "Node" ? true : false;
                //enable_button |= m_CurNode.Text == "DestinationChange" ? true : false;
                enable_button |= m_CurNode.Text == "ErrorList" ? true : false;
                enable_button |= m_CurNode.Text == "FrontDetectFilter" ? true : false;
                enable_button |= m_CurNode.Text == "LinkMergeArea" ? true : false;
                enable_button |= m_CurNode.Text == "PIODevice" ? true : false;
                enable_button |= m_CurNode.Text == "VelocityLimit" ? true : false;
                btnSave.Enabled = enable_button;
                enable_button = m_CurNode.Text == "Port" ? true : false;
                btnImportOffset.Enabled = enable_button;
                lbProcessCount.Text = string.Format("0/{0}", this.doubleBufferedGridView1.Rows.Count);

                if (m_OldNode != null)
                {
                    m_OldNode.BackColor = Color.White;
                    m_CurNode.BackColor = Color.Yellow;
                }
                m_OldNode = m_CurNode;

                ButtonLog.WriteLog(this.Name.ToString(), "treeView_NodeMouseClick");
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void UpdateGridView(object tag)
		{
            try
            {
                this.cbUpdateTeachingSamePortType.Visible = false;
                this.doubleBufferedGridView1.AutoGenerateColumns = false;
                this.doubleBufferedGridView1.DataSource = m_BindSource;

                this.doubleBufferedGridView1.Columns.Clear();
                m_BindSource.DataSource = null;

                object obj = null;
                DataItem item = null;
                if (m_CurNode.Text == "Node")
                {
                    if ((tag as Dictionary<int, DataItem_Node>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_Node>).Values;
                        item = (tag as Dictionary<int, DataItem_Node>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "Link")
                {
                    if ((tag as Dictionary<int, DataItem_Link>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_Link>).Values;
                        item = (tag as Dictionary<int, DataItem_Link>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "CommentTag")
                {
                    if ((tag as Dictionary<string, DataItem_CommentTag>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<string, DataItem_CommentTag>).Values;
                        item = (tag as Dictionary<string, DataItem_CommentTag>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "ComponentLife")
                {
                    if ((tag as Dictionary<ComponentLocation, List<DataItem_ComponentLife>>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<ComponentLocation, List<DataItem_ComponentLife>>).Values;
                        item = (tag as Dictionary<ComponentLocation, List<DataItem_ComponentLife>>).Values.First().First();
                    }
                }
                else if (m_CurNode.Text == "DestinationChange")
                {
                    if ((tag as Dictionary<int, DataItem_DestinationChange>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_DestinationChange>).Values;
                        item = (tag as Dictionary<int, DataItem_DestinationChange>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "ErrorHistory")
                {
                    if ((tag as Dictionary<DateTime, DataItem_ErrorHistory>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<DateTime, DataItem_ErrorHistory>).Values;
                        item = (tag as Dictionary<DateTime, DataItem_ErrorHistory>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "ErrorList")
                {
                    if ((tag as Dictionary<int, DataItem_ErrorList>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_ErrorList>).Values;
                        item = (tag as Dictionary<int, DataItem_ErrorList>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "FrontDetectFilter")
                {
                    if ((tag as Dictionary<int, DataItem_FrontDetectFilter>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_FrontDetectFilter>).Values;
                        item = (tag as Dictionary<int, DataItem_FrontDetectFilter>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "GeneralInformation")
                {
                    if ((tag as Dictionary<GeneralInformationItemName, DataItem_GeneralInformation>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<GeneralInformationItemName, DataItem_GeneralInformation>).Values;
                        item = (tag as Dictionary<GeneralInformationItemName, DataItem_GeneralInformation>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "LinkMergeArea")
                {
                    if ((tag as Dictionary<int, DataItem_LinkMergeArea>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_LinkMergeArea>).Values;
                        item = (tag as Dictionary<int, DataItem_LinkMergeArea>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "PIODevice")
                {
                    if ((tag as Dictionary<int, DataItem_PIODevice>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_PIODevice>).Values;
                        item = (tag as Dictionary<int, DataItem_PIODevice>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "Port")
                {
                    this.cbUpdateTeachingSamePortType.Visible = true;
                    if ((tag as Dictionary<int, DataItem_Port>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_Port>).Values;
                        item = (tag as Dictionary<int, DataItem_Port>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "TransferList")
                {
                    if ((tag as Dictionary<string, DataItem_TransferInfo>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<string, DataItem_TransferInfo>).Values;
                        item = (tag as Dictionary<string, DataItem_TransferInfo>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "UserList")
                {
                    if ((tag as Dictionary<string, DataItem_UserInfo>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<string, DataItem_UserInfo>).Values;
                        item = (tag as Dictionary<string, DataItem_UserInfo>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "EventLog")
                {
                    if ((tag as Dictionary<DateTime, DataItem_EventLog>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<DateTime, DataItem_EventLog>).Values;
                        item = (tag as Dictionary<DateTime, DataItem_EventLog>).Values.First();
                    }
                }
                else if (m_CurNode.Text == "VelocityLimit")
                {
                    if ((tag as Dictionary<int, DataItem_VelocityLimit>).Values.Count > 0)
                    {
                        obj = (tag as Dictionary<int, DataItem_VelocityLimit>).Values;
                        item = (tag as Dictionary<int, DataItem_VelocityLimit>).Values.First();
                    }
                }
                if (item == null) return;
                PropertyInfo[] properties = item.GetProperties();
                if (obj != null)
                {
                    m_BindSource.DataSource = obj;
                }

                if (properties != null && properties.Length > 0)
                {
                    foreach (PropertyInfo info in properties)
                    {
                        object attr = info.GetCustomAttribute(typeof(DatabaseSettingAttribute));
                        if (attr != null && (attr as DatabaseSettingAttribute).IsSettingType)
                        {
                            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                            colId.DataPropertyName = info.Name;
                            colId.HeaderText = info.Name;
                            colId.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                            colId.ReadOnly = true;
                            this.doubleBufferedGridView1.Columns.Add(colId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void doubleBufferedGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                object obj = (sender as DoubleBufferedGridView).SelectedRows[0];
                object value = (obj as DataGridViewRow).DataBoundItem;
                using (FormPropertyEdit form = new FormPropertyEdit(value))
                {
                    if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        value = form.m_Object;
                        if (m_CurNode.Text == "Port" && this.cbUpdateTeachingSamePortType.Checked)
                        {
                            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                            DataItem_Port selected_port = value as DataItem_Port;
                            List<DataItem_Port> ports = DatabaseHandler.Instance.DictionaryPortDataList.Values.Where(x => x.PortType == selected_port.PortType).ToList();
                            foreach (DataItem_Port item in ports)
                            {
                                item.DriveLeftOffset = selected_port.DriveLeftOffset;
                                item.DriveRightOffset = selected_port.DriveRightOffset;
                                item.HoistOffset = selected_port.HoistOffset;
                                item.SlideOffset = selected_port.SlideOffset;
                                item.RotateOffset = selected_port.RotateOffset;
                            }

                            this.Cursor = System.Windows.Forms.Cursors.Arrow;
                        }
                    }
                    ButtonLog.WriteLog(this.Name.ToString(), "doubleBufferedGridView1_CellContentClick");
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnImportOffset_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_CurNode.Text != "Port") return;

                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.InitialDirectory = Application.StartupPath;
                    dlg.Title = "Select xml file : TeachingOffset";
                    dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                    dlg.FileName = "TeachingOffsetAdapter.xml";
                    if (DialogResult.OK == dlg.ShowDialog())
                    {
                        var helperXml = new XmlHelper<TeachingOffsetAdapter>();
                        TeachingOffsetAdapter mng = helperXml.Read(dlg.FileName);
                        if (mng != null)
                        {
                            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                            List<TeachingOffset> offsets = mng.Offsets;
                            int index = 1;
                            lbProcessCount.Text = string.Format("{0}/{1}", index, offsets.Count);
                            foreach (TeachingOffset item in offsets)
                            {
                                if (DatabaseHandler.Instance.DictionaryPortDataList.ContainsKey(item.PortID))
                                {
                                    DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].DriveLeftOffset = item.DriveLeftOffset;
                                    DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].DriveRightOffset = item.DriveRightOffset;
                                    DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].HoistOffset = item.HoistOffset;
                                    DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].SlideOffset = item.SlideOffset;
                                    DatabaseHandler.Instance.DictionaryPortDataList[item.PortID].RotateOffset = item.RotateOffset;
                                    TeachingOffsetAdapter.Instance.UpdateOffset(DatabaseHandler.Instance.DictionaryPortDataList[item.PortID]);
                                }
                                lbProcessCount.Text = string.Format("{0}/{1}", index++, offsets.Count);
                                Thread.Sleep(1);
                            }
                            TeachingOffsetAdapter.Instance.Save();
                            this.Cursor = System.Windows.Forms.Cursors.Arrow;
                        }
                        else
                        {
                            MessageBox.Show(string.Format("{0} can't read. check the file", dlg.FileName));
                        }
                    }

                    ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnImportOffset_Click, UserID : {0}", AppConfig.Instance.OperatorLoginUserID));
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                int total_count = 0;
                object value = null;
                if (cbSelectedItemUpdate.Checked)
                {
                    if (doubleBufferedGridView1.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Not Selected Item !");
                        return;
                    }
                    else
                    {
                        value = (doubleBufferedGridView1.SelectedRows[0] as DataGridViewRow).DataBoundItem;
                        index = doubleBufferedGridView1.SelectedRows[0].Index;
                    }
                }

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                bool query_update = true;
                if (m_CurNode.Text == "Node")
                {
                    total_count = DatabaseHandler.Instance.DictionaryNodeDataList.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_Node item = (DataItem_Node)value;
                        DatabaseHandler.Instance.QueryNode.Update(item);
                    }
                    else
                    {
                        List<DataItem_Node> items = DatabaseHandler.Instance.DictionaryNodeDataList.Values.ToList();
                        DatabaseHandler.Instance.QueryNode.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "Link")
                {
                    total_count = DatabaseHandler.Instance.DictionaryLinkDataList.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_Link item = (DataItem_Link)value;
                        DatabaseHandler.Instance.QueryLink.Update(item);
                    }
                    else
                    {
                        List<DataItem_Link> items = DatabaseHandler.Instance.DictionaryLinkDataList.Values.ToList();
                        DatabaseHandler.Instance.QueryLink.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "CommentTag")
                {
                    query_update = false;
                }
                else if (m_CurNode.Text == "ComponentLife")
                {
                    query_update = false;
                }
                else if (m_CurNode.Text == "DestinationChange")
                {
                    total_count = DatabaseHandler.Instance.DictionaryDestinationChangeAreaList.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_DestinationChange item = (DataItem_DestinationChange)value;
                        DatabaseHandler.Instance.QueryDestinationChange.Update(item);
                    }
                    else
                    {
                        List<DataItem_DestinationChange> items = DatabaseHandler.Instance.DictionaryDestinationChangeAreaList.Values.ToList();
                        DatabaseHandler.Instance.QueryDestinationChange.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "ErrorList")
                {
                    total_count = DatabaseHandler.Instance.DictionaryErrorList.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_ErrorList item = (DataItem_ErrorList)value;
                        DatabaseHandler.Instance.QueryErrorList.Update(item);
                    }
                    else
                    {
                        List<DataItem_ErrorList> items = DatabaseHandler.Instance.DictionaryErrorList.Values.ToList();
                        DatabaseHandler.Instance.QueryErrorList.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "FrontDetectFilter")
                {
                    total_count = DatabaseHandler.Instance.DictionaryFrontDetectFilter.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_FrontDetectFilter item = (DataItem_FrontDetectFilter)value;
                        DatabaseHandler.Instance.QueryFrontDetectFilter.Update(item);
                    }
                    else
                    {
                        List<DataItem_FrontDetectFilter> items = DatabaseHandler.Instance.DictionaryFrontDetectFilter.Values.ToList();
                        DatabaseHandler.Instance.QueryFrontDetectFilter.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "GeneralInformation")
                {
                    query_update = false;
                }
                else if (m_CurNode.Text == "LinkMergeArea")
                {
                    total_count = DatabaseHandler.Instance.DictionaryLinkMergeArea.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_LinkMergeArea item = (DataItem_LinkMergeArea)value;
                        DatabaseHandler.Instance.QueryLinkMergeArea.Update(item);
                    }
                    else
                    {
                        List<DataItem_LinkMergeArea> items = DatabaseHandler.Instance.DictionaryLinkMergeArea.Values.ToList();
                        DatabaseHandler.Instance.QueryLinkMergeArea.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "PIODevice")
                {
                    total_count = DatabaseHandler.Instance.DictionaryPIODevice.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_PIODevice item = (DataItem_PIODevice)value;
                        DatabaseHandler.Instance.QueryPIODevice.Update(item);
                    }
                    else
                    {
                        List<DataItem_PIODevice> items = DatabaseHandler.Instance.DictionaryPIODevice.Values.ToList();
                        DatabaseHandler.Instance.QueryPIODevice.Update(items);
                        index = total_count;
                    }
                }
                else if (m_CurNode.Text == "Port")
                {
                    total_count = DatabaseHandler.Instance.DictionaryPortDataList.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_Port item = (DataItem_Port)value;
                        DatabaseHandler.Instance.QueryPort.Update(item);
                        TeachingOffsetAdapter.Instance.UpdateOffset(item);
                    }
                    else
                    {
                        List<DataItem_Port> items = DatabaseHandler.Instance.DictionaryPortDataList.Values.ToList();
                        DatabaseHandler.Instance.QueryPort.Update(items);
                        TeachingOffsetAdapter.Instance.UpdateOffset(items);
                        index = total_count;
                    }
                    TeachingOffsetAdapter.Instance.Save();
                }
                else if (m_CurNode.Text == "TransferList")
                {
                    query_update = false;
                }
                else if (m_CurNode.Text == "UserList")
                {
                    query_update = false;
                }
                else if (m_CurNode.Text == "EventLog")
                {
                    query_update = false;
                }
                else if (m_CurNode.Text == "VelocityLimit")
                {
                    total_count = DatabaseHandler.Instance.DictionaryVelocityLimit.Count;
                    if (cbSelectedItemUpdate.Checked)
                    {
                        DataItem_VelocityLimit item = (DataItem_VelocityLimit)value;
                        DatabaseHandler.Instance.QueryVelocityLimit.Update(item);
                    }
                    else
                    {
                        List<DataItem_VelocityLimit> items = DatabaseHandler.Instance.DictionaryVelocityLimit.Values.ToList();
                        DatabaseHandler.Instance.QueryVelocityLimit.Update(items);
                        index = total_count;
                    }
                }
                if (query_update)
                    lbProcessCount.Text = string.Format("{0}/{1}", index, total_count);
                else { this.Cursor = System.Windows.Forms.Cursors.Arrow; lbProcessCount.Text = string.Format("Cann't Update Category"); }

                ButtonLog.WriteLog(this.Name.ToString(), string.Format("btnSave_Click : {0}, UserID : {1}", m_CurNode.Text, AppConfig.Instance.OperatorLoginUserID));
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
    }
}
