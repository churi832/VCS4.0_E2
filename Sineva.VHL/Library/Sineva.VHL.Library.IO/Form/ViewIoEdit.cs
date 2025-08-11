/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml.Serialization;

namespace Sineva.VHL.Library.IO
{
    public partial class ViewIoEdit : UserControl
    {
        #region Fields
        private BindingSource[] m_BindSource;
        private bool m_Initialized = false;
        private IoManager m_Pool = null;
        private List<DataGridView> m_DataGridViews = new List<DataGridView>();
        private bool m_ViewLoaded = false;
        private bool m_BindingComp = false;
        private bool m_RunTime = false;
        private IoChannel m_SelectedCh = null;
        //private XExcel m_XExcel = new XExcel();
        private bool m_Shown = false;
        private Stack m_stackOldGrid = new Stack();
        private IFormUpdate m_FormUpdate;

        private List<string> m_SelectedNames = new List<string>();
        private bool m_ForceEnable = true;
        #endregion

        #region Property
        [Browsable(false), XmlIgnore()]
        public List<string> SelectedNames
        {
            get
            {
                m_SelectedNames.Clear();
                int tabIndex = this.tabControl1.SelectedIndex;
                int count = m_DataGridViews[tabIndex].SelectedCells.Count;
                List<int> selectedRows = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    if (selectedRows.Contains(m_DataGridViews[tabIndex].SelectedCells[i].RowIndex) == false)
                        selectedRows.Add(m_DataGridViews[tabIndex].SelectedCells[i].RowIndex);
                }

                SortedList<int, string> selectIoList = new SortedList<int, string>();
                for (int i = 0; i < selectedRows.Count; i++)
                {
                    string ioName = m_DataGridViews[tabIndex].Rows[selectedRows[i]].DataBoundItem.ToString();
                    selectIoList.Add(selectedRows[i], ioName);
                    //m_SelectedNames.Add(ioName);
                }
                m_SelectedNames = selectIoList.Values.ToList();
                return m_SelectedNames;
            }
        }

        public bool ForceEnable
        {
            get { return m_ForceEnable; }
            set { m_ForceEnable = value; }
        }
        #endregion

        #region Constructor
        public ViewIoEdit()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.CacheText, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            m_BindSource = new BindingSource[4];

            this.dataGridViewDi.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseClick);
            this.dataGridViewDo.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseClick);
            this.dataGridViewAi.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseClick);
            this.dataGridViewAo.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseClick);

            this.dataGridViewDi.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseDoubleClick);
            this.dataGridViewDo.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseDoubleClick);
            this.dataGridViewAi.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseDoubleClick);
            this.dataGridViewAo.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseDoubleClick);

            this.dataGridViewDi.KeyDown += new KeyEventHandler(dataGridView_KeyDown);
            this.dataGridViewDo.KeyDown += new KeyEventHandler(dataGridView_KeyDown);
            this.dataGridViewAi.KeyDown += new KeyEventHandler(dataGridView_KeyDown);
            this.dataGridViewAo.KeyDown += new KeyEventHandler(dataGridView_KeyDown);

            this.dataGridViewDi.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView_DataBindingComplete);
            this.dataGridViewDo.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView_DataBindingComplete);
            this.dataGridViewAi.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView_DataBindingComplete);
            this.dataGridViewAo.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(dataGridView_DataBindingComplete);
        }
        #endregion

        #region Events
        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView view = sender as DataGridView;
            SetGridColor(view);
            m_BindingComp = true;
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    string s = Clipboard.GetText();
                    char[] splitter = { '\r', '\n' };
                    string[] lines = s.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    DataGridView view = sender as DataGridView;
                    BackupData(view); // Data Backup
                    DataGridViewCell cell;
                    int row = view.CurrentCell.RowIndex;
                    int col = view.CurrentCell.ColumnIndex;
                    if (col != 2 && col != 3) return;
                    int colCount = view.ColumnCount;
                    int lineCount = lines.Length;
                    string line;
                    for (int i = 0; i < lineCount; i++)
                    {
                        line = lines[i];
                        if (row < view.RowCount && line.Length > 0)
                        {
                            string[] cells = line.Split('\t');
                            int cellCount = cells.Length;
                            for (int j = 0; j < cellCount; j++)
                            {
                                if (col + j < colCount)
                                {
                                    cell = view[col + j, row];
                                    if (cell.ValueType == typeof(ActiveType))
                                    {
                                        cell.Value = cells[j] == "A" ? ActiveType.A : ActiveType.B;
                                    }
                                    else cell.Value = cells[j];
                                }
                                else break;
                            }
                            row++;
                        }
                        else break;
                    }
                }
                else if (e.Control && e.KeyCode == Keys.Z)
                {
                    if (m_stackOldGrid.Count <= 0) return;
                    DataGridView view = sender as DataGridView;
                    RedoData(view); // Redo
                }
                else if (e.Control && e.KeyCode == Keys.X)
                {
                    DataGridView view = sender as DataGridView;
                    BackupData(view); // Data Backup
                    BindingSource source = view.DataSource as BindingSource;
                    IoChannel ch = source.Current as IoChannel;

                    string sbuf = "";
                    Clipboard.Clear();
                    view.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                    DataGridViewSelectedCellCollection cells = view.SelectedCells;
                    foreach (DataGridViewCell cell in cells)
                    {
                        if (cell.ColumnIndex != 2) return;
                        sbuf += (cell.Value.ToString() + "\r\n");
                        ResetCellData(ch, cell);
                    }
                    Clipboard.SetText(sbuf);
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DataGridView view = sender as DataGridView;
                    BackupData(view); // Data Backup
                    BindingSource source = view.DataSource as BindingSource;
                    IoChannel ch = source.Current as IoChannel;
                    DataGridViewCell cell = view.CurrentCell;
                    if (cell.ColumnIndex != 2) return;
                    ResetCellData(ch, cell);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridView view = sender as DataGridView;
            BindingSource source = view.DataSource as BindingSource;

            m_SelectedCh = source.Current as IoChannel;
        }

        private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
            if (m_ForceEnable)
            {
                DataGridView view = sender as DataGridView;
                BindingSource source = view.DataSource as BindingSource;
                if (true)
                {
                    IoChannel ch = source.Current as IoChannel;
                    switch (ch.IoType)
                    {
                        case IoType.DI:
                            {
                                string curVal = IoManager.DiChannels[ch.Id].State;
                                IoManager.DiChannels[ch.Id].State = string.Equals(curVal, "OFF") ? "ON" : "OFF";
                            }
                            break;

                        case IoType.DO:
                            {
                                if (IoManager.DoChannels[ch.Id].DoManualNoUse) break;
                                if (IoManager.DoChannels[ch.Id].DiSafetyIntr.Count > 0)
                                {
                                    bool intr = false;
                                    bool inverse = IoManager.DoChannels[ch.Id].SafetyIntrInverse;
                                    for (int i = 0; i < IoManager.DoChannels[ch.Id].DiSafetyIntr.Count; i++)
                                    {
                                        if (inverse)
                                            intr |= IoManager.DoChannels[ch.Id].DiSafetyIntr[i].GetChannel().GetDi() ? false : true;
                                        else intr |= IoManager.DoChannels[ch.Id].DiSafetyIntr[i].GetChannel().GetDi() ? true : false;
                                    }
                                    if (intr) break;
                                }

                                string curVal = IoManager.DoChannels[ch.Id].State;
                                IoManager.DoChannels[ch.Id].State = string.Equals(curVal, "OFF") ? "ON" : "OFF";
                            }
                            break;

                        case IoType.AI:
                            {
                                using (FormAnalogScale dlg = new FormAnalogScale(IoManager.AiChannels[ch.Id]))
                                {
                                    if (dlg.ShowDialog() == DialogResult.OK)
                                    {
                                        AnalogScaleManager.Instance.UpdateScale(IoManager.AiChannels[ch.Id].Scale);
                                    }
                                }
                            }
                            break;

                        case IoType.AO:
                            {
                                KeyInValidation validation = new KeyInValidation();
                                string newValueString = "";
                                validation.Format = OptionFormat.Digit;
                                validation.High = ushort.MaxValue.ToString();
                                validation.Low = ushort.MinValue.ToString();
                                newValueString = validation.ShowEditDialog("Key in value", IoManager.AoChannels[ch.Id].State);
                                IoManager.AoChannels[ch.Id].State = newValueString;
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("The current user does not have permission!");
            }
        }

        private void ViewIoEdit_Load(object sender, EventArgs e)
        {
            m_DataGridViews.Add(this.dataGridViewDi);
            m_DataGridViews.Add(this.dataGridViewDo);
            m_DataGridViews.Add(this.dataGridViewAi);
            m_DataGridViews.Add(this.dataGridViewAo);

            this.tabControl1.Select();
            m_ViewLoaded = true;
        }

        private void cbFilterOn_CheckedChanged(object sender, EventArgs e)
        {
            //int selectedTabIndex = this.tabControl1.SelectedIndex;
            //DataGridView view;
            //view = m_DataGridViews[selectedTabIndex];

            if (this.cbFilterOn.Checked == true)
            {
                string[] stringArray = this.textFilter.Text.Split(' ');

                UpdateGridViewByFilter(dataGridViewDi, IoManager.DiChannels, stringArray);
                UpdateGridViewByFilter(dataGridViewDo, IoManager.DoChannels, stringArray);
                UpdateGridViewByFilter(dataGridViewAi, IoManager.AiChannels, stringArray);
                UpdateGridViewByFilter(dataGridViewAo, IoManager.AoChannels, stringArray);
            }
            else
            {
                UpdateGridView(dataGridViewDi, IoManager.DiChannels);
                UpdateGridView(dataGridViewDo, IoManager.DoChannels);
                UpdateGridView(dataGridViewAi, IoManager.AiChannels);
                UpdateGridView(dataGridViewAo, IoManager.AoChannels);
            }
        }

        private void textFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.cbFilterOn.Checked = false;
                this.cbFilterOn.Checked = true;
            }
        }

        public void UpdateState()
        {
            if (m_Pool == null ||
                m_ViewLoaded == false ||
                m_BindingComp == false ||
                m_RunTime == false)
                return;

            if (m_FormUpdate != null && m_FormUpdate.UpdateNeed == false) return;

            int selectedTabIndex = this.tabControl1.SelectedIndex;
            DataGridView view;
            view = m_DataGridViews[selectedTabIndex];

            int stateColumnIndex;
            stateColumnIndex = view.ColumnCount - 1;

            foreach (DataGridViewRow row in view.Rows)
            {
                if (row.Displayed)
                    SetStateCellBackColor(row.Cells[stateColumnIndex]);
            }
        }
        #endregion

        #region Methods
        public void Initialize(IoManager pool)
        {
            if (m_Initialized) return;

            if (m_Pool == null)
                m_Pool = pool;

            for (int i = 0; i < 4; i++)
            {
                m_BindSource[i] = new BindingSource();
            }
            dataGridViewDi.Tag = 0;
            dataGridViewDo.Tag = 1;
            dataGridViewAi.Tag = 2;
            dataGridViewAo.Tag = 3;

            UpdateGridView(dataGridViewDi, IoManager.DiChannels);
            UpdateGridView(dataGridViewDo, IoManager.DoChannels);
            UpdateGridView(dataGridViewAi, IoManager.AiChannels);
            UpdateGridView(dataGridViewAo, IoManager.AoChannels);

            m_Initialized = true;
        }

        public void Initialize(IoManager pool, IFormUpdate formUpdate, bool runTime)
        {
            if (m_Initialized) return;

            m_FormUpdate = formUpdate;

            m_RunTime = runTime;

            if (m_Pool == null)
                m_Pool = pool;

            for (int i = 0; i < 4; i++)
            {
                m_BindSource[i] = new BindingSource();
            }
            dataGridViewDi.Tag = 0;
            dataGridViewDo.Tag = 1;
            dataGridViewAi.Tag = 2;
            dataGridViewAo.Tag = 3;

            UpdateGridView(dataGridViewDi, IoManager.DiChannels);
            UpdateGridView(dataGridViewDo, IoManager.DoChannels);
            UpdateGridView(dataGridViewAi, IoManager.AiChannels);
            UpdateGridView(dataGridViewAo, IoManager.AoChannels);

            m_Initialized = true;
        }

        public void SetShown(bool shown)
        {
            m_Shown = shown;
        }
        public void UpdateGridView(DataGridView view, List<IoChannel> list)
        {
            m_BindSource[(int)view.Tag].DataSource = list;
            view.AutoGenerateColumns = false;
            view.DataSource = m_BindSource[(int)view.Tag];
            view.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            if (!m_Initialized)
            {
                DataGridViewTextBoxColumn[] col = new DataGridViewTextBoxColumn[5];

                int i = 0;
                col[i] = new DataGridViewTextBoxColumn();
                col[i].DataPropertyName = "Id";
                col[i].HeaderText = "ID";
                col[i].ReadOnly = true;
                view.Columns.Add(col[i]);
                i++;

                col[i] = new DataGridViewTextBoxColumn();
                col[i].DataPropertyName = "WiringNo";
                col[i].HeaderText = "WiringNo";
                col[i].ReadOnly = true;
                view.Columns.Add(col[i]);
                i++;

                col[i] = new DataGridViewTextBoxColumn();
                col[i].DataPropertyName = "Name";
                col[i].HeaderText = "Name";
                if (m_RunTime) col[i].ReadOnly = true;
                col[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                view.Columns.Add(col[i]);
                i++;

                col[i] = new DataGridViewTextBoxColumn();
                col[i].DataPropertyName = "Description";
                col[i].HeaderText = "Description";
                if (m_RunTime) col[i].ReadOnly = true;
                col[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                view.Columns.Add(col[i]);
                i++;

                if (view == dataGridViewDi)
                {
                    DataGridViewCheckBoxColumn colB = new DataGridViewCheckBoxColumn();
                    colB.DataPropertyName = "IsBContact";
                    colB.HeaderText = "B-Cont.";
                    if (m_RunTime) colB.ReadOnly = true;
                    view.Columns.Add(colB);
                }

                if (m_RunTime)
                {
                    col[i] = new DataGridViewTextBoxColumn();
                    col[i].DataPropertyName = "State";
                    col[i].HeaderText = "State";
                    col[i].ReadOnly = true;
                    view.Columns.Add(col[i]);

                    if (view == dataGridViewAi)
                    {
                        DataGridViewTextBoxColumn colCalValue = new DataGridViewTextBoxColumn();
                        colCalValue.DataPropertyName = "StateAdcCal";
                        colCalValue.HeaderText = "Cal. State";
                        colCalValue.ReadOnly = true;
                        view.Columns.Add(colCalValue);
                    }
                }
            }
        }

        public void UpdateGridViewByFilter(DataGridView view, List<IoChannel> list, params string[] keys)
        {
            List<IoChannel> filteredList = Filterling(list, keys);
            //m_BindSource[(int)view.Tag].DataSource = filteredList;
            UpdateGridView(view, filteredList);
        }

        private List<IoChannel> Filterling(List<IoChannel> source, params string[] keys)
        {
            List<IoChannel> list = new List<IoChannel>();
            if (keys.Length > 0)
            {
                foreach (IoChannel dev in source)
                {
                    bool matchAll = true;
                    foreach (string key in keys)
                    {
                        matchAll &= dev.Name.ToLower().Contains(key.ToLower());
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

        public void SetGridColor(DataGridView view)
        {
            view.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
            int blockIdCol = 0;
            foreach (DataGridViewColumn col in view.Columns)
            {
                if (col.HeaderText == "Block")
                {
                    break;
                }
                blockIdCol++;
            }
            return;
            foreach (DataGridViewRow row in view.Rows)
            {
                row.Cells[0].Style.BackColor = Color.LightGray;
                int even = (int)row.Cells[blockIdCol].Value % 2;
                if (even == 0)
                    row.Cells[1].Style.BackColor = Color.SkyBlue;
                else
                    row.Cells[1].Style.BackColor = Color.AliceBlue;
            }
        }

        private void SetStateCellBackColor(DataGridViewCell cell)
        {
            if (cell.Value != null)
            {
                Color onColor = Color.GreenYellow;
                Color offColor = Color.WhiteSmoke;

                string value = cell.Value.ToString();

                if (value == "ON")
                {
                    cell.Style.BackColor = onColor;
                }
                else if (value == "OFF")
                {
                    cell.Style.BackColor = offColor;
                }
                else
                {
                    //AI/AO값을 update 할려면 아래처럼 해줘야 한다. 이상하다.
                    //if (string.Compare((string)(cell.Tag), (string)(cell.Value)) != 0)
                    if (cell.Tag == null || string.Equals((string)(cell.Tag), (string)(cell.Value)) == false) // Tag를 이용해서 update cost를 줄이려 노력~
                    {
                        cell.Style.BackColor = onColor;
                        cell.Style.BackColor = offColor;
                        cell.Tag = cell.Value;
                    }
                }
            }
        }

        private void BackupData(DataGridView view)
        {
            BindingSource source = view.DataSource as BindingSource;
            List<IoChannel> channelList = source.DataSource as List<IoChannel>;

            List<IoChannel> oldChannelList = new List<IoChannel>();
            for (int i = 0; i < channelList.Count; i++)
            {
                IoChannel ch = new IoChannel();
                ch.Id = channelList[i].Id;
                ch.Node = channelList[i].Node;
                ch.Terminal = channelList[i].Terminal;
                ch.ChannelNo = channelList[i].ChannelNo;
                ch.WiringNo = channelList[i].WiringNo;
                ch.Name = channelList[i].Name;
                ch.Description = channelList[i].Description;
                ch.IoType = channelList[i].IoType;
                ch.State = channelList[i].State;
                oldChannelList.Add(ch);
            }
            m_stackOldGrid.Push(oldChannelList);
        }

        private void RedoData(DataGridView view)
        {
            BindingSource source = view.DataSource as BindingSource;
            List<IoChannel> list = source.DataSource as List<IoChannel>;

            List<IoChannel> oldList = m_stackOldGrid.Pop() as List<IoChannel>;

            if (list[0].IoType != oldList[0].IoType) return;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Name = oldList[i].Name;
            }

            UpdateGridView(view, list);
            view.Invalidate();
        }

        public IoChannel GetSelectedCh()
        {
            return m_SelectedCh;
        }

        private void ResetCellData(IoChannel ch, DataGridViewCell cell)
        {
            switch (ch.IoType)
            {
                case IoType.DI:
                    cell.Value = "di__";
                    break;
                case IoType.DO:
                    cell.Value = "do__";
                    break;
                case IoType.AI:
                    cell.Value = "ai__";
                    break;
                case IoType.AO:
                    cell.Value = "ao__";
                    break;
            }
        }
        #endregion

        private void dataGridViewDi_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
