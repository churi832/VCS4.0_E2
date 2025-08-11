using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.IF.OCS
{

    public partial class ucOCSCommandList : UserControl
    {
        private OCSCommManager m_OCSCommManager;
        private BindingList<OCSCommandData> m_BindingCommands = new BindingList<OCSCommandData>();
        public delegate void CommandIndexChange(OCSCommandData commandData);
        public event CommandIndexChange CommandIndexChangeEvent;
        public ucOCSCommandList()
        {
            InitializeComponent();
        }

        public bool Initialize()
        {
            bool rv = true;
            m_OCSCommManager = OCSCommManager.Instance;
            m_OCSCommManager.OcsStatus.delMessageReceived += OcsStatus_delMessageReceived;
            m_OCSCommManager.OcsStatus.delMessageSent += OcsStatus_delMessageSent;
            IniDataGird();
            return rv;
        }
        private void IniDataGird()
        {
            dgOCSCommand.AutoGenerateColumns = false;
            dgOCSCommand.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgOCSCommand.MultiSelect = false;
            dgOCSCommand.ReadOnly = true;
            dgOCSCommand.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Souce-Target", DataPropertyName = "OCSCommandType", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgOCSCommand.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "MessageCode", DataPropertyName = "MessageCode", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgOCSCommand.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "DateTime", DataPropertyName = "CommandDateTime", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            //dgOCSCommand.DataSource = m_OCSCommandData.OrderByDescending(x => x.CommandDateTime);
            dgOCSCommand.DataSource = m_BindingCommands;

        }
        private void OcsStatus_delMessageSent(object obj1, object obj2)
        {
            if (dgOCSCommand.InvokeRequired)
            {
                DelVoid_ObjectObject d = new DelVoid_ObjectObject(OcsStatus_delMessageSent);
                this.Invoke(d, obj1, obj2);
            }
            else
            {
                try
                {
                    if (obj2 is VehicleIF)
                    {
                        switch (((VehicleIF)obj2).MessageCode)
                        {
                            case IFMessage.CommandSend:
                            case IFMessage.CommandReply:
                            case IFMessage.EventSend:
                            case IFMessage.EventReply:
                            case IFMessage.AlarmEventSend:
                            case IFMessage.AlarmEventReply:
                            case IFMessage.MapDataSend:
                            case IFMessage.MapDataReply:
                            case IFMessage.TeachingDataSend:
                            case IFMessage.TeachingDataReply:
                            case IFMessage.DataVersionRequest:
                            case IFMessage.DataVersionReply:
                            case IFMessage.MapDataRequest:
                            case IFMessage.MapDataRequestAcknowledge:
                            case IFMessage.FDCDataSend:
                            case IFMessage.FDCDataReply:
                            case IFMessage.OperationConfigDataSend:
                            case IFMessage.OperationConfigDataReply:
                            case IFMessage.AutoTeachingResultSend:
                            case IFMessage.AutoTeachingResultReply:
                            case IFMessage.UserLoginRequest:
                            case IFMessage.UserLoginReply:
                            case IFMessage.PathRequest:
                            case IFMessage.PathSend:
                            case IFMessage.CommandStatusRequest:
                            case IFMessage.CommandStatusReply:
                            case IFMessage.LocationInformationSend:
                            case IFMessage.LocationInformationSendReply:
                                OCSCommandData data = new OCSCommandData();
                                data.OCSCommandType = "VHL->OCS";
                                data.CommandMessage = ((VehicleIF)obj2);
                                data.MessageCode = ((VehicleIF)obj2).MessageCode;
                                data.CommandDateTime = DateTime.Now;
                                m_BindingCommands.Add(data);
                                if (dgOCSCommand.Rows.Count > 0)
                                {
                                    dgOCSCommand.FirstDisplayedScrollingRowIndex = dgOCSCommand.RowCount - 1;
                                }
                                if (m_BindingCommands.Count > 100)
                                {
                                    m_BindingCommands.RemoveAt(0);
                                }
                                break;
                            case IFMessage.StatusDataSend:
                            case IFMessage.StatusDataReply:
                                break;
                            default:
                                break;
                        }

                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

        private void OcsStatus_delMessageReceived(object obj1, object obj2)
        {
            if (dgOCSCommand.InvokeRequired)
            {
                DelVoid_ObjectObject d = new DelVoid_ObjectObject(OcsStatus_delMessageReceived);
                this.Invoke(d, obj1, obj2);
            }
            else
            {
                try
                {
                    if (obj2 is VehicleIF)
                    {
                        switch (((VehicleIF)obj2).MessageCode)
                        {
                            case IFMessage.CommandSend:
                            case IFMessage.CommandReply:
                            case IFMessage.EventSend:
                            case IFMessage.EventReply:
                            case IFMessage.AlarmEventSend:
                            case IFMessage.AlarmEventReply:
                            case IFMessage.MapDataSend:
                            case IFMessage.MapDataReply:
                            case IFMessage.TeachingDataSend:
                            case IFMessage.TeachingDataReply:
                            case IFMessage.DataVersionRequest:
                            case IFMessage.DataVersionReply:
                            case IFMessage.MapDataRequest:
                            case IFMessage.MapDataRequestAcknowledge:
                            case IFMessage.FDCDataSend:
                            case IFMessage.FDCDataReply:
                            case IFMessage.OperationConfigDataSend:
                            case IFMessage.OperationConfigDataReply:
                            case IFMessage.AutoTeachingResultSend:
                            case IFMessage.AutoTeachingResultReply:
                            case IFMessage.UserLoginRequest:
                            case IFMessage.UserLoginReply:
                            case IFMessage.PathRequest:
                            case IFMessage.PathSend:
                            case IFMessage.CommandStatusRequest:
                            case IFMessage.CommandStatusReply:
                            case IFMessage.LocationInformationSend:
                            case IFMessage.LocationInformationSendReply:
                                OCSCommandData data = new OCSCommandData();
                                data.OCSCommandType = "OCS->VHL";
                                data.CommandMessage = ((VehicleIF)obj2);
                                data.MessageCode = ((VehicleIF)obj2).MessageCode;
                                data.CommandDateTime = DateTime.Now;
                                m_BindingCommands.Add(data);
                                if (dgOCSCommand.Rows.Count > 0)
                                {
                                    dgOCSCommand.FirstDisplayedScrollingRowIndex = dgOCSCommand.RowCount - 1;
                                }
                                if (m_BindingCommands.Count > 100)
                                {
                                    m_BindingCommands.RemoveAt(0);
                                }
                                break;
                            case IFMessage.StatusDataSend:
                            case IFMessage.StatusDataReply:
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void OCSCommandList_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void dgOCSCommand_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dgOCSCommand.CurrentRow;
            if (selectedRow != null)
            {
                CommandIndexChangeEvent?.Invoke((OCSCommandData)selectedRow.DataBoundItem);
            }
        }

        private void dgOCSCommand_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.Value != null)
            {
                e.Value = ((DateTime)e.Value).ToString("yyyy-MM-dd HH:mm:ss.fff");
                e.FormattingApplied = true;

            }
        }
    }
    public class OCSCommandData
    {
        public string OCSCommandType { get; set; }
        public DateTime CommandDateTime { get; set; }
        public IFMessage MessageCode { get; set; }
        public VehicleIF CommandMessage { get; set; }
    }
}

