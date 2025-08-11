
namespace Sineva.VHL.Device
{
    partial class ucManualTeachingView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucManualTeachingView));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.tbPortID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ltBoxPortList = new System.Windows.Forms.ListBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btnBcrSet = new System.Windows.Forms.Button();
            this.lbUnitSelect = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.ucLedO5 = new Sineva.VHL.Library.IO.ucLed();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.ucLedO4 = new Sineva.VHL.Library.IO.ucLed();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.ucLedO3 = new Sineva.VHL.Library.IO.ucLed();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.ucLedO2 = new Sineva.VHL.Library.IO.ucLed();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ucLedO1 = new Sineva.VHL.Library.IO.ucLed();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.hpTeachingData = new Sineva.VHL.Library.HeaderPanel();
            this.dgvTeachingData = new System.Windows.Forms.DataGridView();
            this.hpCurrentPosition = new Sineva.VHL.Library.HeaderPanel();
            this.dgvCurrentPosition = new System.Windows.Forms.DataGridView();
            this.hpOffsetData = new Sineva.VHL.Library.HeaderPanel();
            this.dgvOffset = new System.Windows.Forms.DataGridView();
            this.tcAxesControl = new System.Windows.Forms.TabControl();
            this.tbpHoist = new System.Windows.Forms.TabPage();
            this.btnHoistMoveToBeforeDown = new Sineva.VHL.Library.IButton();
            this.btnJogZ_plus = new Sineva.VHL.Library.JogButton();
            this.btnJogZ_minus = new Sineva.VHL.Library.JogButton();
            this.btnHoistJogStop = new Sineva.VHL.Library.ColorButton();
            this.btnHoistMoveToHome = new Sineva.VHL.Library.IButton();
            this.btnHoistMoveToTeaching = new Sineva.VHL.Library.IButton();
            this.tbHoistDistance = new System.Windows.Forms.TextBox();
            this.tbHoistVeloctiy = new System.Windows.Forms.TextBox();
            this.lbHoistDistance = new System.Windows.Forms.Label();
            this.lbHoistVelocity = new System.Windows.Forms.Label();
            this.rbHoistStepMove = new System.Windows.Forms.RadioButton();
            this.rbHoistVelocityMove = new System.Windows.Forms.RadioButton();
            this.tbpSlide = new System.Windows.Forms.TabPage();
            this.btnJogY_plus = new Sineva.VHL.Library.JogButton();
            this.btnJogY_minus = new Sineva.VHL.Library.JogButton();
            this.btnSlideMoveToHome = new Sineva.VHL.Library.IButton();
            this.btnSlideMoveToTeaching = new Sineva.VHL.Library.IButton();
            this.btnSlideJogStop = new Sineva.VHL.Library.ColorButton();
            this.tbSlideDistance = new System.Windows.Forms.TextBox();
            this.tbSlideVelocity = new System.Windows.Forms.TextBox();
            this.lbSlideDistance = new System.Windows.Forms.Label();
            this.lbSlideVelocity = new System.Windows.Forms.Label();
            this.rbSlideStepMove = new System.Windows.Forms.RadioButton();
            this.rbSlideVelocityMove = new System.Windows.Forms.RadioButton();
            this.tbpRotate = new System.Windows.Forms.TabPage();
            this.btnJogT_minus = new Sineva.VHL.Library.JogButton();
            this.btnJogT_plus = new Sineva.VHL.Library.JogButton();
            this.btnRotateMoveToHome = new Sineva.VHL.Library.IButton();
            this.btnRotateMoveToTeaching = new Sineva.VHL.Library.IButton();
            this.btnRotateJogStop = new Sineva.VHL.Library.ColorButton();
            this.tbRotateDistance = new System.Windows.Forms.TextBox();
            this.tbRotateVelocity = new System.Windows.Forms.TextBox();
            this.lbRotateDistance = new System.Windows.Forms.Label();
            this.lbRotateVelocity = new System.Windows.Forms.Label();
            this.rbRotateStepMove = new System.Windows.Forms.RadioButton();
            this.rbRotateVelocityMove = new System.Windows.Forms.RadioButton();
            this.tbpMaster = new System.Windows.Forms.TabPage();
            this.btnJogX_minus = new Sineva.VHL.Library.JogButton();
            this.btnJogX_plus = new Sineva.VHL.Library.JogButton();
            this.btnMasterJogStop = new Sineva.VHL.Library.ColorButton();
            this.tbMasterDistance = new System.Windows.Forms.TextBox();
            this.tbMasterVelocity = new System.Windows.Forms.TextBox();
            this.lbMasterDistance = new System.Windows.Forms.Label();
            this.lbMasterVelocity = new System.Windows.Forms.Label();
            this.rbMasterStepMove = new System.Windows.Forms.RadioButton();
            this.rbMasterVelocityMove = new System.Windows.Forms.RadioButton();
            this.hpAxesStatus = new Sineva.VHL.Library.HeaderPanel();
            this.dgvAxesStatus = new Sineva.VHL.Library.DoubleBufferedGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUpdateOffset = new Sineva.VHL.Library.IButton();
            this.rbUnload = new System.Windows.Forms.RadioButton();
            this.rbLoad = new System.Windows.Forms.RadioButton();
            this.cbReferenceVehicle = new System.Windows.Forms.CheckBox();
            this.btnSaveDataBase = new Sineva.VHL.Library.IButton();
            this.btnResettingOffset = new Sineva.VHL.Library.IButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.hpTeachingData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeachingData)).BeginInit();
            this.hpCurrentPosition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrentPosition)).BeginInit();
            this.hpOffsetData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOffset)).BeginInit();
            this.tcAxesControl.SuspendLayout();
            this.tbpHoist.SuspendLayout();
            this.tbpSlide.SuspendLayout();
            this.tbpRotate.SuspendLayout();
            this.tbpMaster.SuspendLayout();
            this.hpAxesStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAxesStatus)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel3);
            this.splitContainer1.Size = new System.Drawing.Size(1203, 500);
            this.splitContainer1.SplitterDistance = 245;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(245, 500);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.searchPanel, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.ltBoxPortList, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.panel7, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(235, 328);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(211, 40);
            this.label6.TabIndex = 1;
            this.label6.Text = "Port Select";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.White;
            this.searchPanel.Controls.Add(this.tbPortID);
            this.searchPanel.Controls.Add(this.label1);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPanel.Location = new System.Drawing.Point(3, 73);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(229, 24);
            this.searchPanel.TabIndex = 16;
            // 
            // tbPortID
            // 
            this.tbPortID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPortID.Location = new System.Drawing.Point(86, 2);
            this.tbPortID.Name = "tbPortID";
            this.tbPortID.Size = new System.Drawing.Size(128, 21);
            this.tbPortID.TabIndex = 1;
            this.tbPortID.TextChanged += new System.EventHandler(this.tbPortID_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(21, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port ID :";
            // 
            // ltBoxPortList
            // 
            this.ltBoxPortList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ltBoxPortList.FormattingEnabled = true;
            this.ltBoxPortList.ItemHeight = 12;
            this.ltBoxPortList.Location = new System.Drawing.Point(3, 103);
            this.ltBoxPortList.Name = "ltBoxPortList";
            this.ltBoxPortList.Size = new System.Drawing.Size(229, 222);
            this.ltBoxPortList.TabIndex = 15;
            this.ltBoxPortList.SelectedIndexChanged += new System.EventHandler(this.ltBoxPortList_SelectedIndexChanged);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.btnBcrSet);
            this.panel7.Controls.Add(this.lbUnitSelect);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 40);
            this.panel7.Margin = new System.Windows.Forms.Padding(0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(235, 30);
            this.panel7.TabIndex = 17;
            // 
            // btnBcrSet
            // 
            this.btnBcrSet.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBcrSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBcrSet.Location = new System.Drawing.Point(199, 0);
            this.btnBcrSet.Name = "btnBcrSet";
            this.btnBcrSet.Size = new System.Drawing.Size(36, 30);
            this.btnBcrSet.TabIndex = 15;
            this.btnBcrSet.Text = "BCR";
            this.btnBcrSet.UseVisualStyleBackColor = true;
            this.btnBcrSet.Click += new System.EventHandler(this.btnBcrSet_Click);
            // 
            // lbUnitSelect
            // 
            this.lbUnitSelect.BackColor = System.Drawing.Color.Transparent;
            this.lbUnitSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbUnitSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbUnitSelect.ForeColor = System.Drawing.Color.White;
            this.lbUnitSelect.Location = new System.Drawing.Point(0, 0);
            this.lbUnitSelect.Name = "lbUnitSelect";
            this.lbUnitSelect.Size = new System.Drawing.Size(235, 30);
            this.lbUnitSelect.TabIndex = 14;
            this.lbUnitSelect.Text = "Select Port  :";
            this.lbUnitSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.panel6, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.panel5, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.panel4, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(5, 341);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(235, 154);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label7);
            this.panel6.Controls.Add(this.ucLedO5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 123);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(229, 28);
            this.panel6.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "Gripper Right Exist";
            // 
            // ucLedO5
            // 
            this.ucLedO5.LedIOTag = null;
            this.ucLedO5.Location = new System.Drawing.Point(11, 6);
            this.ucLedO5.Margin = new System.Windows.Forms.Padding(41, 24, 41, 24);
            this.ucLedO5.Name = "ucLedO5";
            this.ucLedO5.Size = new System.Drawing.Size(15, 15);
            this.ucLedO5.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.ucLedO4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 93);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(229, 24);
            this.panel5.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Gripper Left Exist";
            // 
            // ucLedO4
            // 
            this.ucLedO4.LedIOTag = null;
            this.ucLedO4.Location = new System.Drawing.Point(11, 6);
            this.ucLedO4.Margin = new System.Windows.Forms.Padding(41, 24, 41, 24);
            this.ucLedO4.Name = "ucLedO4";
            this.ucLedO4.Size = new System.Drawing.Size(15, 15);
            this.ucLedO4.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.ucLedO3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 63);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(229, 24);
            this.panel4.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Product DN";
            // 
            // ucLedO3
            // 
            this.ucLedO3.LedIOTag = null;
            this.ucLedO3.Location = new System.Drawing.Point(11, 6);
            this.ucLedO3.Margin = new System.Windows.Forms.Padding(41, 24, 41, 24);
            this.ucLedO3.Name = "ucLedO3";
            this.ucLedO3.Size = new System.Drawing.Size(15, 15);
            this.ucLedO3.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.ucLedO2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(229, 24);
            this.panel3.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Product Limit";
            // 
            // ucLedO2
            // 
            this.ucLedO2.LedIOTag = null;
            this.ucLedO2.Location = new System.Drawing.Point(11, 6);
            this.ucLedO2.Margin = new System.Windows.Forms.Padding(41, 24, 41, 24);
            this.ucLedO2.Name = "ucLedO2";
            this.ucLedO2.Size = new System.Drawing.Size(15, 15);
            this.ucLedO2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.ucLedO1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(229, 24);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Product UP";
            // 
            // ucLedO1
            // 
            this.ucLedO1.LedIOTag = null;
            this.ucLedO1.Location = new System.Drawing.Point(11, 6);
            this.ucLedO1.Margin = new System.Windows.Forms.Padding(41, 24, 41, 24);
            this.ucLedO1.Name = "ucLedO1";
            this.ucLedO1.Size = new System.Drawing.Size(15, 15);
            this.ucLedO1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.hpTeachingData, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.hpCurrentPosition, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.hpOffsetData, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.tcAxesControl, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.hpAxesStatus, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(954, 500);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // hpTeachingData
            // 
            this.hpTeachingData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hpTeachingData.Controls.Add(this.dgvTeachingData);
            this.hpTeachingData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hpTeachingData.Font = new System.Drawing.Font("Arial", 9F);
            this.hpTeachingData.FriendControl = null;
            this.hpTeachingData.Location = new System.Drawing.Point(5, 153);
            this.hpTeachingData.Name = "hpTeachingData";
            this.hpTeachingData.Size = new System.Drawing.Size(944, 64);
            this.hpTeachingData.TabIndex = 0;
            this.hpTeachingData.Title = "Port Teaching Data";
            this.hpTeachingData.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hpTeachingData.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.hpTeachingData.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hpTeachingData.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // dgvTeachingData
            // 
            this.dgvTeachingData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTeachingData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTeachingData.Location = new System.Drawing.Point(0, 19);
            this.dgvTeachingData.Name = "dgvTeachingData";
            this.dgvTeachingData.RowTemplate.Height = 23;
            this.dgvTeachingData.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvTeachingData.Size = new System.Drawing.Size(942, 43);
            this.dgvTeachingData.TabIndex = 1;
            // 
            // hpCurrentPosition
            // 
            this.hpCurrentPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hpCurrentPosition.Controls.Add(this.dgvCurrentPosition);
            this.hpCurrentPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hpCurrentPosition.Font = new System.Drawing.Font("Arial", 9F);
            this.hpCurrentPosition.FriendControl = null;
            this.hpCurrentPosition.Location = new System.Drawing.Point(5, 257);
            this.hpCurrentPosition.Name = "hpCurrentPosition";
            this.hpCurrentPosition.Size = new System.Drawing.Size(944, 64);
            this.hpCurrentPosition.TabIndex = 1;
            this.hpCurrentPosition.Title = "Current Position Data";
            this.hpCurrentPosition.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hpCurrentPosition.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.hpCurrentPosition.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hpCurrentPosition.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // dgvCurrentPosition
            // 
            this.dgvCurrentPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCurrentPosition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCurrentPosition.Location = new System.Drawing.Point(0, 19);
            this.dgvCurrentPosition.Name = "dgvCurrentPosition";
            this.dgvCurrentPosition.RowTemplate.Height = 23;
            this.dgvCurrentPosition.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvCurrentPosition.Size = new System.Drawing.Size(942, 43);
            this.dgvCurrentPosition.TabIndex = 2;
            // 
            // hpOffsetData
            // 
            this.hpOffsetData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hpOffsetData.Controls.Add(this.dgvOffset);
            this.hpOffsetData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hpOffsetData.Font = new System.Drawing.Font("Arial", 9F);
            this.hpOffsetData.FriendControl = null;
            this.hpOffsetData.Location = new System.Drawing.Point(5, 329);
            this.hpOffsetData.Name = "hpOffsetData";
            this.hpOffsetData.Size = new System.Drawing.Size(944, 64);
            this.hpOffsetData.TabIndex = 2;
            this.hpOffsetData.Title = "Offset Data";
            this.hpOffsetData.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hpOffsetData.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.hpOffsetData.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hpOffsetData.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // dgvOffset
            // 
            this.dgvOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOffset.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOffset.Location = new System.Drawing.Point(0, 19);
            this.dgvOffset.Name = "dgvOffset";
            this.dgvOffset.RowTemplate.Height = 23;
            this.dgvOffset.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvOffset.Size = new System.Drawing.Size(942, 43);
            this.dgvOffset.TabIndex = 3;
            // 
            // tcAxesControl
            // 
            this.tcAxesControl.Controls.Add(this.tbpHoist);
            this.tcAxesControl.Controls.Add(this.tbpSlide);
            this.tcAxesControl.Controls.Add(this.tbpRotate);
            this.tcAxesControl.Controls.Add(this.tbpMaster);
            this.tcAxesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcAxesControl.Location = new System.Drawing.Point(5, 401);
            this.tcAxesControl.Name = "tcAxesControl";
            this.tcAxesControl.SelectedIndex = 0;
            this.tcAxesControl.Size = new System.Drawing.Size(944, 94);
            this.tcAxesControl.TabIndex = 3;
            // 
            // tbpHoist
            // 
            this.tbpHoist.Controls.Add(this.btnHoistMoveToBeforeDown);
            this.tbpHoist.Controls.Add(this.btnJogZ_plus);
            this.tbpHoist.Controls.Add(this.btnJogZ_minus);
            this.tbpHoist.Controls.Add(this.btnHoistJogStop);
            this.tbpHoist.Controls.Add(this.btnHoistMoveToHome);
            this.tbpHoist.Controls.Add(this.btnHoistMoveToTeaching);
            this.tbpHoist.Controls.Add(this.tbHoistDistance);
            this.tbpHoist.Controls.Add(this.tbHoistVeloctiy);
            this.tbpHoist.Controls.Add(this.lbHoistDistance);
            this.tbpHoist.Controls.Add(this.lbHoistVelocity);
            this.tbpHoist.Controls.Add(this.rbHoistStepMove);
            this.tbpHoist.Controls.Add(this.rbHoistVelocityMove);
            this.tbpHoist.Location = new System.Drawing.Point(4, 22);
            this.tbpHoist.Name = "tbpHoist";
            this.tbpHoist.Padding = new System.Windows.Forms.Padding(3);
            this.tbpHoist.Size = new System.Drawing.Size(936, 68);
            this.tbpHoist.TabIndex = 0;
            this.tbpHoist.Text = "Hoist";
            this.tbpHoist.UseVisualStyleBackColor = true;
            // 
            // btnHoistMoveToBeforeDown
            // 
            this.btnHoistMoveToBeforeDown.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnHoistMoveToBeforeDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHoistMoveToBeforeDown.BgDefault = null;
            this.btnHoistMoveToBeforeDown.BgDisable = null;
            this.btnHoistMoveToBeforeDown.BgOver = null;
            this.btnHoistMoveToBeforeDown.BgPushed = null;
            this.btnHoistMoveToBeforeDown.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnHoistMoveToBeforeDown.ConnectedLabel = null;
            this.btnHoistMoveToBeforeDown.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnHoistMoveToBeforeDown.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnHoistMoveToBeforeDown.DefaultImage = null;
            this.btnHoistMoveToBeforeDown.Description = null;
            this.btnHoistMoveToBeforeDown.DownImage = null;
            this.btnHoistMoveToBeforeDown.Image = global::Sineva.VHL.Device.Properties.Resources.expand;
            this.btnHoistMoveToBeforeDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHoistMoveToBeforeDown.Location = new System.Drawing.Point(801, 8);
            this.btnHoistMoveToBeforeDown.Name = "btnHoistMoveToBeforeDown";
            this.btnHoistMoveToBeforeDown.OverImage = null;
            this.btnHoistMoveToBeforeDown.Size = new System.Drawing.Size(128, 25);
            this.btnHoistMoveToBeforeDown.TabIndex = 16;
            this.btnHoistMoveToBeforeDown.Text = " => BeforeDown";
            this.btnHoistMoveToBeforeDown.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHoistMoveToBeforeDown.UpImage = null;
            this.btnHoistMoveToBeforeDown.UseOneImage = true;
            this.btnHoistMoveToBeforeDown.UseVisualStyleBackColor = true;
            this.btnHoistMoveToBeforeDown.Click += new System.EventHandler(this.btnHoistMoveToBeforeDown_Click);
            // 
            // btnJogZ_plus
            // 
            this.btnJogZ_plus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogZ_plus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogZ_plus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogZ_plus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogZ_plus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogZ_plus.Image")));
            this.btnJogZ_plus.Location = new System.Drawing.Point(175, 16);
            this.btnJogZ_plus.Name = "btnJogZ_plus";
            this.btnJogZ_plus.Size = new System.Drawing.Size(74, 49);
            this.btnJogZ_plus.TabIndex = 15;
            this.btnJogZ_plus.Tag = "";
            this.btnJogZ_plus.Text = "Z+";
            this.btnJogZ_plus.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnJogZ_plus.UseVisualStyleBackColor = false;
            this.btnJogZ_plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogZ_plus_MouseDown);
            this.btnJogZ_plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogZ_plus_MouseUp);
            // 
            // btnJogZ_minus
            // 
            this.btnJogZ_minus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogZ_minus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogZ_minus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogZ_minus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogZ_minus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogZ_minus.Image")));
            this.btnJogZ_minus.Location = new System.Drawing.Point(15, 16);
            this.btnJogZ_minus.Name = "btnJogZ_minus";
            this.btnJogZ_minus.Size = new System.Drawing.Size(74, 49);
            this.btnJogZ_minus.TabIndex = 14;
            this.btnJogZ_minus.Tag = "";
            this.btnJogZ_minus.Text = "Z-";
            this.btnJogZ_minus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnJogZ_minus.UseVisualStyleBackColor = false;
            this.btnJogZ_minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogZ_minus_MouseDown);
            this.btnJogZ_minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogZ_minus_MouseUp);
            // 
            // btnHoistJogStop
            // 
            this.btnHoistJogStop.BackColor = System.Drawing.Color.Yellow;
            this.btnHoistJogStop.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnHoistJogStop.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnHoistJogStop.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoistJogStop.Location = new System.Drawing.Point(95, 16);
            this.btnHoistJogStop.Name = "btnHoistJogStop";
            this.btnHoistJogStop.Size = new System.Drawing.Size(74, 49);
            this.btnHoistJogStop.TabIndex = 13;
            this.btnHoistJogStop.Text = "STOP";
            this.btnHoistJogStop.UseVisualStyleBackColor = false;
            this.btnHoistJogStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnHoistMoveToHome
            // 
            this.btnHoistMoveToHome.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnHoistMoveToHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHoistMoveToHome.BgDefault = null;
            this.btnHoistMoveToHome.BgDisable = null;
            this.btnHoistMoveToHome.BgOver = null;
            this.btnHoistMoveToHome.BgPushed = null;
            this.btnHoistMoveToHome.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnHoistMoveToHome.ConnectedLabel = null;
            this.btnHoistMoveToHome.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnHoistMoveToHome.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnHoistMoveToHome.DefaultImage = null;
            this.btnHoistMoveToHome.Description = null;
            this.btnHoistMoveToHome.DownImage = null;
            this.btnHoistMoveToHome.Image = global::Sineva.VHL.Device.Properties.Resources.suohui;
            this.btnHoistMoveToHome.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHoistMoveToHome.Location = new System.Drawing.Point(681, 40);
            this.btnHoistMoveToHome.Name = "btnHoistMoveToHome";
            this.btnHoistMoveToHome.OverImage = null;
            this.btnHoistMoveToHome.Size = new System.Drawing.Size(109, 25);
            this.btnHoistMoveToHome.TabIndex = 12;
            this.btnHoistMoveToHome.Text = " => Home";
            this.btnHoistMoveToHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHoistMoveToHome.UpImage = null;
            this.btnHoistMoveToHome.UseOneImage = true;
            this.btnHoistMoveToHome.UseVisualStyleBackColor = true;
            this.btnHoistMoveToHome.Click += new System.EventHandler(this.btn_MoveToHome);
            // 
            // btnHoistMoveToTeaching
            // 
            this.btnHoistMoveToTeaching.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnHoistMoveToTeaching.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHoistMoveToTeaching.BgDefault = null;
            this.btnHoistMoveToTeaching.BgDisable = null;
            this.btnHoistMoveToTeaching.BgOver = null;
            this.btnHoistMoveToTeaching.BgPushed = null;
            this.btnHoistMoveToTeaching.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnHoistMoveToTeaching.ConnectedLabel = null;
            this.btnHoistMoveToTeaching.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnHoistMoveToTeaching.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnHoistMoveToTeaching.DefaultImage = null;
            this.btnHoistMoveToTeaching.Description = null;
            this.btnHoistMoveToTeaching.DownImage = null;
            this.btnHoistMoveToTeaching.Image = global::Sineva.VHL.Device.Properties.Resources.expand;
            this.btnHoistMoveToTeaching.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHoistMoveToTeaching.Location = new System.Drawing.Point(681, 8);
            this.btnHoistMoveToTeaching.Name = "btnHoistMoveToTeaching";
            this.btnHoistMoveToTeaching.OverImage = null;
            this.btnHoistMoveToTeaching.Size = new System.Drawing.Size(109, 25);
            this.btnHoistMoveToTeaching.TabIndex = 11;
            this.btnHoistMoveToTeaching.Text = " => Teaching";
            this.btnHoistMoveToTeaching.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHoistMoveToTeaching.UpImage = null;
            this.btnHoistMoveToTeaching.UseOneImage = true;
            this.btnHoistMoveToTeaching.UseVisualStyleBackColor = true;
            this.btnHoistMoveToTeaching.Click += new System.EventHandler(this.btn_MoveToTeaching);
            // 
            // tbHoistDistance
            // 
            this.tbHoistDistance.Location = new System.Drawing.Point(554, 44);
            this.tbHoistDistance.Name = "tbHoistDistance";
            this.tbHoistDistance.Size = new System.Drawing.Size(76, 21);
            this.tbHoistDistance.TabIndex = 10;
            // 
            // tbHoistVeloctiy
            // 
            this.tbHoistVeloctiy.Location = new System.Drawing.Point(554, 11);
            this.tbHoistVeloctiy.Name = "tbHoistVeloctiy";
            this.tbHoistVeloctiy.Size = new System.Drawing.Size(76, 21);
            this.tbHoistVeloctiy.TabIndex = 9;
            // 
            // lbHoistDistance
            // 
            this.lbHoistDistance.AutoSize = true;
            this.lbHoistDistance.Location = new System.Drawing.Point(475, 47);
            this.lbHoistDistance.Name = "lbHoistDistance";
            this.lbHoistDistance.Size = new System.Drawing.Size(53, 12);
            this.lbHoistDistance.TabIndex = 8;
            this.lbHoistDistance.Text = "Distance";
            // 
            // lbHoistVelocity
            // 
            this.lbHoistVelocity.AutoSize = true;
            this.lbHoistVelocity.Location = new System.Drawing.Point(475, 14);
            this.lbHoistVelocity.Name = "lbHoistVelocity";
            this.lbHoistVelocity.Size = new System.Drawing.Size(53, 12);
            this.lbHoistVelocity.TabIndex = 7;
            this.lbHoistVelocity.Text = "Velocity";
            // 
            // rbHoistStepMove
            // 
            this.rbHoistStepMove.AutoSize = true;
            this.rbHoistStepMove.Location = new System.Drawing.Point(385, 32);
            this.rbHoistStepMove.Name = "rbHoistStepMove";
            this.rbHoistStepMove.Size = new System.Drawing.Size(77, 16);
            this.rbHoistStepMove.TabIndex = 6;
            this.rbHoistStepMove.TabStop = true;
            this.rbHoistStepMove.Text = "Step Move";
            this.rbHoistStepMove.UseVisualStyleBackColor = true;
            // 
            // rbHoistVelocityMove
            // 
            this.rbHoistVelocityMove.AutoSize = true;
            this.rbHoistVelocityMove.Checked = true;
            this.rbHoistVelocityMove.Location = new System.Drawing.Point(270, 32);
            this.rbHoistVelocityMove.Name = "rbHoistVelocityMove";
            this.rbHoistVelocityMove.Size = new System.Drawing.Size(101, 16);
            this.rbHoistVelocityMove.TabIndex = 5;
            this.rbHoistVelocityMove.TabStop = true;
            this.rbHoistVelocityMove.Text = "Velocity Move";
            this.rbHoistVelocityMove.UseVisualStyleBackColor = true;
            // 
            // tbpSlide
            // 
            this.tbpSlide.Controls.Add(this.btnJogY_plus);
            this.tbpSlide.Controls.Add(this.btnJogY_minus);
            this.tbpSlide.Controls.Add(this.btnSlideMoveToHome);
            this.tbpSlide.Controls.Add(this.btnSlideMoveToTeaching);
            this.tbpSlide.Controls.Add(this.btnSlideJogStop);
            this.tbpSlide.Controls.Add(this.tbSlideDistance);
            this.tbpSlide.Controls.Add(this.tbSlideVelocity);
            this.tbpSlide.Controls.Add(this.lbSlideDistance);
            this.tbpSlide.Controls.Add(this.lbSlideVelocity);
            this.tbpSlide.Controls.Add(this.rbSlideStepMove);
            this.tbpSlide.Controls.Add(this.rbSlideVelocityMove);
            this.tbpSlide.Location = new System.Drawing.Point(4, 22);
            this.tbpSlide.Name = "tbpSlide";
            this.tbpSlide.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSlide.Size = new System.Drawing.Size(936, 68);
            this.tbpSlide.TabIndex = 1;
            this.tbpSlide.Text = "Slide";
            this.tbpSlide.UseVisualStyleBackColor = true;
            // 
            // btnJogY_plus
            // 
            this.btnJogY_plus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogY_plus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogY_plus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogY_plus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogY_plus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogY_plus.Image")));
            this.btnJogY_plus.Location = new System.Drawing.Point(175, 16);
            this.btnJogY_plus.Name = "btnJogY_plus";
            this.btnJogY_plus.Size = new System.Drawing.Size(74, 49);
            this.btnJogY_plus.TabIndex = 21;
            this.btnJogY_plus.Tag = "";
            this.btnJogY_plus.Text = "Y+";
            this.btnJogY_plus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnJogY_plus.UseVisualStyleBackColor = false;
            this.btnJogY_plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogY_plus_MouseDown);
            this.btnJogY_plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogY_plus_MouseUp);
            // 
            // btnJogY_minus
            // 
            this.btnJogY_minus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogY_minus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogY_minus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogY_minus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogY_minus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogY_minus.Image")));
            this.btnJogY_minus.Location = new System.Drawing.Point(15, 16);
            this.btnJogY_minus.Name = "btnJogY_minus";
            this.btnJogY_minus.Size = new System.Drawing.Size(74, 49);
            this.btnJogY_minus.TabIndex = 20;
            this.btnJogY_minus.Tag = "";
            this.btnJogY_minus.Text = "Y-";
            this.btnJogY_minus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogY_minus.UseVisualStyleBackColor = false;
            this.btnJogY_minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogY_minus_MouseDown);
            this.btnJogY_minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogY_minus_MouseUp);
            // 
            // btnSlideMoveToHome
            // 
            this.btnSlideMoveToHome.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSlideMoveToHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSlideMoveToHome.BgDefault = null;
            this.btnSlideMoveToHome.BgDisable = null;
            this.btnSlideMoveToHome.BgOver = null;
            this.btnSlideMoveToHome.BgPushed = null;
            this.btnSlideMoveToHome.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnSlideMoveToHome.ConnectedLabel = null;
            this.btnSlideMoveToHome.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnSlideMoveToHome.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnSlideMoveToHome.DefaultImage = null;
            this.btnSlideMoveToHome.Description = null;
            this.btnSlideMoveToHome.DownImage = null;
            this.btnSlideMoveToHome.Image = global::Sineva.VHL.Device.Properties.Resources.suohui;
            this.btnSlideMoveToHome.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSlideMoveToHome.Location = new System.Drawing.Point(681, 40);
            this.btnSlideMoveToHome.Name = "btnSlideMoveToHome";
            this.btnSlideMoveToHome.OverImage = null;
            this.btnSlideMoveToHome.Size = new System.Drawing.Size(109, 25);
            this.btnSlideMoveToHome.TabIndex = 19;
            this.btnSlideMoveToHome.Text = " => Home";
            this.btnSlideMoveToHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSlideMoveToHome.UpImage = null;
            this.btnSlideMoveToHome.UseOneImage = true;
            this.btnSlideMoveToHome.UseVisualStyleBackColor = true;
            this.btnSlideMoveToHome.Click += new System.EventHandler(this.btn_MoveToHome);
            // 
            // btnSlideMoveToTeaching
            // 
            this.btnSlideMoveToTeaching.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSlideMoveToTeaching.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSlideMoveToTeaching.BgDefault = null;
            this.btnSlideMoveToTeaching.BgDisable = null;
            this.btnSlideMoveToTeaching.BgOver = null;
            this.btnSlideMoveToTeaching.BgPushed = null;
            this.btnSlideMoveToTeaching.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnSlideMoveToTeaching.ConnectedLabel = null;
            this.btnSlideMoveToTeaching.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnSlideMoveToTeaching.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnSlideMoveToTeaching.DefaultImage = null;
            this.btnSlideMoveToTeaching.Description = null;
            this.btnSlideMoveToTeaching.DownImage = null;
            this.btnSlideMoveToTeaching.Image = global::Sineva.VHL.Device.Properties.Resources.expand;
            this.btnSlideMoveToTeaching.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSlideMoveToTeaching.Location = new System.Drawing.Point(681, 8);
            this.btnSlideMoveToTeaching.Name = "btnSlideMoveToTeaching";
            this.btnSlideMoveToTeaching.OverImage = null;
            this.btnSlideMoveToTeaching.Size = new System.Drawing.Size(109, 25);
            this.btnSlideMoveToTeaching.TabIndex = 18;
            this.btnSlideMoveToTeaching.Text = " => Teaching";
            this.btnSlideMoveToTeaching.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSlideMoveToTeaching.UpImage = null;
            this.btnSlideMoveToTeaching.UseOneImage = true;
            this.btnSlideMoveToTeaching.UseVisualStyleBackColor = true;
            this.btnSlideMoveToTeaching.Click += new System.EventHandler(this.btn_MoveToTeaching);
            // 
            // btnSlideJogStop
            // 
            this.btnSlideJogStop.BackColor = System.Drawing.Color.Yellow;
            this.btnSlideJogStop.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnSlideJogStop.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnSlideJogStop.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSlideJogStop.Location = new System.Drawing.Point(95, 16);
            this.btnSlideJogStop.Name = "btnSlideJogStop";
            this.btnSlideJogStop.Size = new System.Drawing.Size(74, 49);
            this.btnSlideJogStop.TabIndex = 17;
            this.btnSlideJogStop.Text = "STOP";
            this.btnSlideJogStop.UseVisualStyleBackColor = false;
            this.btnSlideJogStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tbSlideDistance
            // 
            this.tbSlideDistance.Location = new System.Drawing.Point(554, 44);
            this.tbSlideDistance.Name = "tbSlideDistance";
            this.tbSlideDistance.Size = new System.Drawing.Size(76, 21);
            this.tbSlideDistance.TabIndex = 16;
            // 
            // tbSlideVelocity
            // 
            this.tbSlideVelocity.Location = new System.Drawing.Point(554, 11);
            this.tbSlideVelocity.Name = "tbSlideVelocity";
            this.tbSlideVelocity.Size = new System.Drawing.Size(76, 21);
            this.tbSlideVelocity.TabIndex = 15;
            // 
            // lbSlideDistance
            // 
            this.lbSlideDistance.AutoSize = true;
            this.lbSlideDistance.Location = new System.Drawing.Point(475, 47);
            this.lbSlideDistance.Name = "lbSlideDistance";
            this.lbSlideDistance.Size = new System.Drawing.Size(53, 12);
            this.lbSlideDistance.TabIndex = 14;
            this.lbSlideDistance.Text = "Distance";
            // 
            // lbSlideVelocity
            // 
            this.lbSlideVelocity.AutoSize = true;
            this.lbSlideVelocity.Location = new System.Drawing.Point(475, 14);
            this.lbSlideVelocity.Name = "lbSlideVelocity";
            this.lbSlideVelocity.Size = new System.Drawing.Size(53, 12);
            this.lbSlideVelocity.TabIndex = 13;
            this.lbSlideVelocity.Text = "Velocity";
            // 
            // rbSlideStepMove
            // 
            this.rbSlideStepMove.AutoSize = true;
            this.rbSlideStepMove.Location = new System.Drawing.Point(385, 32);
            this.rbSlideStepMove.Name = "rbSlideStepMove";
            this.rbSlideStepMove.Size = new System.Drawing.Size(77, 16);
            this.rbSlideStepMove.TabIndex = 12;
            this.rbSlideStepMove.TabStop = true;
            this.rbSlideStepMove.Text = "Step Move";
            this.rbSlideStepMove.UseVisualStyleBackColor = true;
            // 
            // rbSlideVelocityMove
            // 
            this.rbSlideVelocityMove.AutoSize = true;
            this.rbSlideVelocityMove.Checked = true;
            this.rbSlideVelocityMove.Location = new System.Drawing.Point(270, 32);
            this.rbSlideVelocityMove.Name = "rbSlideVelocityMove";
            this.rbSlideVelocityMove.Size = new System.Drawing.Size(101, 16);
            this.rbSlideVelocityMove.TabIndex = 11;
            this.rbSlideVelocityMove.TabStop = true;
            this.rbSlideVelocityMove.Text = "Velocity Move";
            this.rbSlideVelocityMove.UseVisualStyleBackColor = true;
            // 
            // tbpRotate
            // 
            this.tbpRotate.Controls.Add(this.btnJogT_minus);
            this.tbpRotate.Controls.Add(this.btnJogT_plus);
            this.tbpRotate.Controls.Add(this.btnRotateMoveToHome);
            this.tbpRotate.Controls.Add(this.btnRotateMoveToTeaching);
            this.tbpRotate.Controls.Add(this.btnRotateJogStop);
            this.tbpRotate.Controls.Add(this.tbRotateDistance);
            this.tbpRotate.Controls.Add(this.tbRotateVelocity);
            this.tbpRotate.Controls.Add(this.lbRotateDistance);
            this.tbpRotate.Controls.Add(this.lbRotateVelocity);
            this.tbpRotate.Controls.Add(this.rbRotateStepMove);
            this.tbpRotate.Controls.Add(this.rbRotateVelocityMove);
            this.tbpRotate.Location = new System.Drawing.Point(4, 22);
            this.tbpRotate.Name = "tbpRotate";
            this.tbpRotate.Size = new System.Drawing.Size(936, 68);
            this.tbpRotate.TabIndex = 2;
            this.tbpRotate.Text = "Rotate";
            this.tbpRotate.UseVisualStyleBackColor = true;
            // 
            // btnJogT_minus
            // 
            this.btnJogT_minus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogT_minus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogT_minus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogT_minus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogT_minus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogT_minus.Image")));
            this.btnJogT_minus.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnJogT_minus.Location = new System.Drawing.Point(15, 16);
            this.btnJogT_minus.Name = "btnJogT_minus";
            this.btnJogT_minus.Size = new System.Drawing.Size(74, 49);
            this.btnJogT_minus.TabIndex = 26;
            this.btnJogT_minus.Tag = "";
            this.btnJogT_minus.Text = "CCW(-)";
            this.btnJogT_minus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogT_minus.UseVisualStyleBackColor = false;
            this.btnJogT_minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogT_minus_MouseDown);
            this.btnJogT_minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogT_minus_MouseUp);
            // 
            // btnJogT_plus
            // 
            this.btnJogT_plus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogT_plus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogT_plus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogT_plus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogT_plus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogT_plus.Image")));
            this.btnJogT_plus.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnJogT_plus.Location = new System.Drawing.Point(175, 16);
            this.btnJogT_plus.Name = "btnJogT_plus";
            this.btnJogT_plus.Size = new System.Drawing.Size(74, 49);
            this.btnJogT_plus.TabIndex = 27;
            this.btnJogT_plus.Tag = "";
            this.btnJogT_plus.Text = "CW(+)";
            this.btnJogT_plus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogT_plus.UseVisualStyleBackColor = false;
            this.btnJogT_plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogT_plus_MouseDown);
            this.btnJogT_plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogT_plus_MouseUp);
            // 
            // btnRotateMoveToHome
            // 
            this.btnRotateMoveToHome.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnRotateMoveToHome.AutoSize = true;
            this.btnRotateMoveToHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRotateMoveToHome.BgDefault = null;
            this.btnRotateMoveToHome.BgDisable = null;
            this.btnRotateMoveToHome.BgOver = null;
            this.btnRotateMoveToHome.BgPushed = null;
            this.btnRotateMoveToHome.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRotateMoveToHome.ConnectedLabel = null;
            this.btnRotateMoveToHome.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRotateMoveToHome.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRotateMoveToHome.DefaultImage = null;
            this.btnRotateMoveToHome.Description = null;
            this.btnRotateMoveToHome.DownImage = null;
            this.btnRotateMoveToHome.Image = global::Sineva.VHL.Device.Properties.Resources.suohui;
            this.btnRotateMoveToHome.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRotateMoveToHome.Location = new System.Drawing.Point(681, 40);
            this.btnRotateMoveToHome.Name = "btnRotateMoveToHome";
            this.btnRotateMoveToHome.OverImage = null;
            this.btnRotateMoveToHome.Size = new System.Drawing.Size(109, 25);
            this.btnRotateMoveToHome.TabIndex = 25;
            this.btnRotateMoveToHome.Text = " => Home";
            this.btnRotateMoveToHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRotateMoveToHome.UpImage = null;
            this.btnRotateMoveToHome.UseOneImage = true;
            this.btnRotateMoveToHome.UseVisualStyleBackColor = true;
            this.btnRotateMoveToHome.Click += new System.EventHandler(this.btn_MoveToHome);
            // 
            // btnRotateMoveToTeaching
            // 
            this.btnRotateMoveToTeaching.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnRotateMoveToTeaching.AutoSize = true;
            this.btnRotateMoveToTeaching.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRotateMoveToTeaching.BgDefault = null;
            this.btnRotateMoveToTeaching.BgDisable = null;
            this.btnRotateMoveToTeaching.BgOver = null;
            this.btnRotateMoveToTeaching.BgPushed = null;
            this.btnRotateMoveToTeaching.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRotateMoveToTeaching.ConnectedLabel = null;
            this.btnRotateMoveToTeaching.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRotateMoveToTeaching.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRotateMoveToTeaching.DefaultImage = null;
            this.btnRotateMoveToTeaching.Description = null;
            this.btnRotateMoveToTeaching.DownImage = null;
            this.btnRotateMoveToTeaching.Image = global::Sineva.VHL.Device.Properties.Resources.expand;
            this.btnRotateMoveToTeaching.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRotateMoveToTeaching.Location = new System.Drawing.Point(681, 8);
            this.btnRotateMoveToTeaching.Name = "btnRotateMoveToTeaching";
            this.btnRotateMoveToTeaching.OverImage = null;
            this.btnRotateMoveToTeaching.Size = new System.Drawing.Size(109, 25);
            this.btnRotateMoveToTeaching.TabIndex = 24;
            this.btnRotateMoveToTeaching.Text = " => Teaching";
            this.btnRotateMoveToTeaching.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRotateMoveToTeaching.UpImage = null;
            this.btnRotateMoveToTeaching.UseOneImage = true;
            this.btnRotateMoveToTeaching.UseVisualStyleBackColor = true;
            this.btnRotateMoveToTeaching.Click += new System.EventHandler(this.btn_MoveToTeaching);
            // 
            // btnRotateJogStop
            // 
            this.btnRotateJogStop.BackColor = System.Drawing.Color.Yellow;
            this.btnRotateJogStop.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnRotateJogStop.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnRotateJogStop.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRotateJogStop.Location = new System.Drawing.Point(95, 16);
            this.btnRotateJogStop.Name = "btnRotateJogStop";
            this.btnRotateJogStop.Size = new System.Drawing.Size(74, 49);
            this.btnRotateJogStop.TabIndex = 23;
            this.btnRotateJogStop.Text = "STOP";
            this.btnRotateJogStop.UseVisualStyleBackColor = false;
            this.btnRotateJogStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tbRotateDistance
            // 
            this.tbRotateDistance.Location = new System.Drawing.Point(554, 44);
            this.tbRotateDistance.Name = "tbRotateDistance";
            this.tbRotateDistance.Size = new System.Drawing.Size(76, 21);
            this.tbRotateDistance.TabIndex = 22;
            // 
            // tbRotateVelocity
            // 
            this.tbRotateVelocity.Location = new System.Drawing.Point(554, 11);
            this.tbRotateVelocity.Name = "tbRotateVelocity";
            this.tbRotateVelocity.Size = new System.Drawing.Size(76, 21);
            this.tbRotateVelocity.TabIndex = 21;
            // 
            // lbRotateDistance
            // 
            this.lbRotateDistance.AutoSize = true;
            this.lbRotateDistance.Location = new System.Drawing.Point(475, 47);
            this.lbRotateDistance.Name = "lbRotateDistance";
            this.lbRotateDistance.Size = new System.Drawing.Size(53, 12);
            this.lbRotateDistance.TabIndex = 20;
            this.lbRotateDistance.Text = "Distance";
            // 
            // lbRotateVelocity
            // 
            this.lbRotateVelocity.AutoSize = true;
            this.lbRotateVelocity.Location = new System.Drawing.Point(475, 14);
            this.lbRotateVelocity.Name = "lbRotateVelocity";
            this.lbRotateVelocity.Size = new System.Drawing.Size(53, 12);
            this.lbRotateVelocity.TabIndex = 19;
            this.lbRotateVelocity.Text = "Velocity";
            // 
            // rbRotateStepMove
            // 
            this.rbRotateStepMove.AutoSize = true;
            this.rbRotateStepMove.Location = new System.Drawing.Point(385, 32);
            this.rbRotateStepMove.Name = "rbRotateStepMove";
            this.rbRotateStepMove.Size = new System.Drawing.Size(77, 16);
            this.rbRotateStepMove.TabIndex = 18;
            this.rbRotateStepMove.TabStop = true;
            this.rbRotateStepMove.Text = "Step Move";
            this.rbRotateStepMove.UseVisualStyleBackColor = true;
            // 
            // rbRotateVelocityMove
            // 
            this.rbRotateVelocityMove.AutoSize = true;
            this.rbRotateVelocityMove.Checked = true;
            this.rbRotateVelocityMove.Location = new System.Drawing.Point(270, 32);
            this.rbRotateVelocityMove.Name = "rbRotateVelocityMove";
            this.rbRotateVelocityMove.Size = new System.Drawing.Size(101, 16);
            this.rbRotateVelocityMove.TabIndex = 17;
            this.rbRotateVelocityMove.TabStop = true;
            this.rbRotateVelocityMove.Text = "Velocity Move";
            this.rbRotateVelocityMove.UseVisualStyleBackColor = true;
            // 
            // tbpMaster
            // 
            this.tbpMaster.Controls.Add(this.btnJogX_minus);
            this.tbpMaster.Controls.Add(this.btnJogX_plus);
            this.tbpMaster.Controls.Add(this.btnMasterJogStop);
            this.tbpMaster.Controls.Add(this.tbMasterDistance);
            this.tbpMaster.Controls.Add(this.tbMasterVelocity);
            this.tbpMaster.Controls.Add(this.lbMasterDistance);
            this.tbpMaster.Controls.Add(this.lbMasterVelocity);
            this.tbpMaster.Controls.Add(this.rbMasterStepMove);
            this.tbpMaster.Controls.Add(this.rbMasterVelocityMove);
            this.tbpMaster.Location = new System.Drawing.Point(4, 22);
            this.tbpMaster.Name = "tbpMaster";
            this.tbpMaster.Size = new System.Drawing.Size(936, 68);
            this.tbpMaster.TabIndex = 3;
            this.tbpMaster.Text = "Master";
            this.tbpMaster.UseVisualStyleBackColor = true;
            // 
            // btnJogX_minus
            // 
            this.btnJogX_minus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogX_minus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogX_minus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogX_minus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogX_minus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogX_minus.Image")));
            this.btnJogX_minus.Location = new System.Drawing.Point(15, 16);
            this.btnJogX_minus.Name = "btnJogX_minus";
            this.btnJogX_minus.Size = new System.Drawing.Size(74, 49);
            this.btnJogX_minus.TabIndex = 30;
            this.btnJogX_minus.Tag = "";
            this.btnJogX_minus.Text = "  X-";
            this.btnJogX_minus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJogX_minus.UseVisualStyleBackColor = false;
            this.btnJogX_minus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogX_minus_MouseDown);
            this.btnJogX_minus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogX_minus_MouseUp);
            // 
            // btnJogX_plus
            // 
            this.btnJogX_plus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogX_plus.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnJogX_plus.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnJogX_plus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJogX_plus.Image = ((System.Drawing.Image)(resources.GetObject("btnJogX_plus.Image")));
            this.btnJogX_plus.Location = new System.Drawing.Point(175, 16);
            this.btnJogX_plus.Name = "btnJogX_plus";
            this.btnJogX_plus.Size = new System.Drawing.Size(74, 49);
            this.btnJogX_plus.TabIndex = 31;
            this.btnJogX_plus.Tag = "";
            this.btnJogX_plus.Text = "X+";
            this.btnJogX_plus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnJogX_plus.UseVisualStyleBackColor = false;
            this.btnJogX_plus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnJogX_plus_MouseDown);
            this.btnJogX_plus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnJogX_plus_MouseUp);
            // 
            // btnMasterJogStop
            // 
            this.btnMasterJogStop.BackColor = System.Drawing.Color.Yellow;
            this.btnMasterJogStop.ColorMouseDown = System.Drawing.Color.Chartreuse;
            this.btnMasterJogStop.ColorMouseUp = System.Drawing.Color.Yellow;
            this.btnMasterJogStop.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMasterJogStop.Location = new System.Drawing.Point(95, 16);
            this.btnMasterJogStop.Name = "btnMasterJogStop";
            this.btnMasterJogStop.Size = new System.Drawing.Size(74, 49);
            this.btnMasterJogStop.TabIndex = 29;
            this.btnMasterJogStop.Text = "STOP";
            this.btnMasterJogStop.UseVisualStyleBackColor = false;
            this.btnMasterJogStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tbMasterDistance
            // 
            this.tbMasterDistance.Location = new System.Drawing.Point(554, 44);
            this.tbMasterDistance.Name = "tbMasterDistance";
            this.tbMasterDistance.Size = new System.Drawing.Size(76, 21);
            this.tbMasterDistance.TabIndex = 28;
            // 
            // tbMasterVelocity
            // 
            this.tbMasterVelocity.Location = new System.Drawing.Point(554, 11);
            this.tbMasterVelocity.Name = "tbMasterVelocity";
            this.tbMasterVelocity.Size = new System.Drawing.Size(76, 21);
            this.tbMasterVelocity.TabIndex = 27;
            // 
            // lbMasterDistance
            // 
            this.lbMasterDistance.AutoSize = true;
            this.lbMasterDistance.Location = new System.Drawing.Point(475, 47);
            this.lbMasterDistance.Name = "lbMasterDistance";
            this.lbMasterDistance.Size = new System.Drawing.Size(53, 12);
            this.lbMasterDistance.TabIndex = 26;
            this.lbMasterDistance.Text = "Distance";
            // 
            // lbMasterVelocity
            // 
            this.lbMasterVelocity.AutoSize = true;
            this.lbMasterVelocity.Location = new System.Drawing.Point(475, 14);
            this.lbMasterVelocity.Name = "lbMasterVelocity";
            this.lbMasterVelocity.Size = new System.Drawing.Size(53, 12);
            this.lbMasterVelocity.TabIndex = 25;
            this.lbMasterVelocity.Text = "Velocity";
            // 
            // rbMasterStepMove
            // 
            this.rbMasterStepMove.AutoSize = true;
            this.rbMasterStepMove.Location = new System.Drawing.Point(385, 32);
            this.rbMasterStepMove.Name = "rbMasterStepMove";
            this.rbMasterStepMove.Size = new System.Drawing.Size(77, 16);
            this.rbMasterStepMove.TabIndex = 24;
            this.rbMasterStepMove.TabStop = true;
            this.rbMasterStepMove.Text = "Step Move";
            this.rbMasterStepMove.UseVisualStyleBackColor = true;
            // 
            // rbMasterVelocityMove
            // 
            this.rbMasterVelocityMove.AutoSize = true;
            this.rbMasterVelocityMove.Checked = true;
            this.rbMasterVelocityMove.Location = new System.Drawing.Point(270, 32);
            this.rbMasterVelocityMove.Name = "rbMasterVelocityMove";
            this.rbMasterVelocityMove.Size = new System.Drawing.Size(101, 16);
            this.rbMasterVelocityMove.TabIndex = 23;
            this.rbMasterVelocityMove.TabStop = true;
            this.rbMasterVelocityMove.Text = "Velocity Move";
            this.rbMasterVelocityMove.UseVisualStyleBackColor = true;
            // 
            // hpAxesStatus
            // 
            this.hpAxesStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hpAxesStatus.Controls.Add(this.dgvAxesStatus);
            this.hpAxesStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hpAxesStatus.Font = new System.Drawing.Font("Arial", 9F);
            this.hpAxesStatus.FriendControl = null;
            this.hpAxesStatus.Location = new System.Drawing.Point(5, 5);
            this.hpAxesStatus.Name = "hpAxesStatus";
            this.hpAxesStatus.Size = new System.Drawing.Size(944, 140);
            this.hpAxesStatus.TabIndex = 4;
            this.hpAxesStatus.Title = "Axes Status";
            this.hpAxesStatus.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hpAxesStatus.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.hpAxesStatus.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hpAxesStatus.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // dgvAxesStatus
            // 
            this.dgvAxesStatus.AllowUserToAddRows = false;
            this.dgvAxesStatus.AllowUserToDeleteRows = false;
            this.dgvAxesStatus.AllowUserToOrderColumns = true;
            this.dgvAxesStatus.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvAxesStatus.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvAxesStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAxesStatus.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvAxesStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvAxesStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 9F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAxesStatus.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvAxesStatus.Location = new System.Drawing.Point(3, 21);
            this.dgvAxesStatus.MultiSelect = false;
            this.dgvAxesStatus.Name = "dgvAxesStatus";
            this.dgvAxesStatus.ReadOnly = true;
            this.dgvAxesStatus.RowTemplate.Height = 23;
            this.dgvAxesStatus.Size = new System.Drawing.Size(936, 114);
            this.dgvAxesStatus.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnResettingOffset);
            this.panel1.Controls.Add(this.btnUpdateOffset);
            this.panel1.Controls.Add(this.rbUnload);
            this.panel1.Controls.Add(this.rbLoad);
            this.panel1.Controls.Add(this.cbReferenceVehicle);
            this.panel1.Controls.Add(this.btnSaveDataBase);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 222);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(950, 30);
            this.panel1.TabIndex = 5;
            // 
            // btnUpdateOffset
            // 
            this.btnUpdateOffset.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnUpdateOffset.AutoSize = true;
            this.btnUpdateOffset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUpdateOffset.BgDefault = null;
            this.btnUpdateOffset.BgDisable = null;
            this.btnUpdateOffset.BgOver = null;
            this.btnUpdateOffset.BgPushed = null;
            this.btnUpdateOffset.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnUpdateOffset.ConnectedLabel = null;
            this.btnUpdateOffset.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnUpdateOffset.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnUpdateOffset.DefaultImage = null;
            this.btnUpdateOffset.Description = null;
            this.btnUpdateOffset.DownImage = null;
            this.btnUpdateOffset.Image = global::Sineva.VHL.Device.Properties.Resources.upload;
            this.btnUpdateOffset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdateOffset.Location = new System.Drawing.Point(224, 2);
            this.btnUpdateOffset.Name = "btnUpdateOffset";
            this.btnUpdateOffset.OverImage = null;
            this.btnUpdateOffset.Size = new System.Drawing.Size(181, 26);
            this.btnUpdateOffset.TabIndex = 7;
            this.btnUpdateOffset.Text = "Update Left Buffer Offset";
            this.btnUpdateOffset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdateOffset.UpImage = null;
            this.btnUpdateOffset.UseOneImage = true;
            this.btnUpdateOffset.UseVisualStyleBackColor = true;
            this.btnUpdateOffset.Click += new System.EventHandler(this.btnUpdateOffset_Click);
            // 
            // rbUnload
            // 
            this.rbUnload.AutoSize = true;
            this.rbUnload.Location = new System.Drawing.Point(561, 8);
            this.rbUnload.Name = "rbUnload";
            this.rbUnload.Size = new System.Drawing.Size(59, 16);
            this.rbUnload.TabIndex = 6;
            this.rbUnload.Text = "Unload";
            this.rbUnload.UseVisualStyleBackColor = true;
            // 
            // rbLoad
            // 
            this.rbLoad.AutoSize = true;
            this.rbLoad.Checked = true;
            this.rbLoad.Location = new System.Drawing.Point(465, 8);
            this.rbLoad.Name = "rbLoad";
            this.rbLoad.Size = new System.Drawing.Size(47, 16);
            this.rbLoad.TabIndex = 5;
            this.rbLoad.TabStop = true;
            this.rbLoad.Text = "Load";
            this.rbLoad.UseVisualStyleBackColor = true;
            // 
            // cbReferenceVehicle
            // 
            this.cbReferenceVehicle.AutoSize = true;
            this.cbReferenceVehicle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbReferenceVehicle.Location = new System.Drawing.Point(17, 8);
            this.cbReferenceVehicle.Name = "cbReferenceVehicle";
            this.cbReferenceVehicle.Size = new System.Drawing.Size(126, 16);
            this.cbReferenceVehicle.TabIndex = 2;
            this.cbReferenceVehicle.Text = "Reference Vehicle";
            this.cbReferenceVehicle.UseVisualStyleBackColor = true;
            // 
            // btnSaveDataBase
            // 
            this.btnSaveDataBase.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSaveDataBase.AutoSize = true;
            this.btnSaveDataBase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSaveDataBase.BgDefault = null;
            this.btnSaveDataBase.BgDisable = null;
            this.btnSaveDataBase.BgOver = null;
            this.btnSaveDataBase.BgPushed = null;
            this.btnSaveDataBase.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnSaveDataBase.ConnectedLabel = null;
            this.btnSaveDataBase.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnSaveDataBase.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnSaveDataBase.DefaultImage = null;
            this.btnSaveDataBase.Description = null;
            this.btnSaveDataBase.DownImage = null;
            this.btnSaveDataBase.Image = global::Sineva.VHL.Device.Properties.Resources.upload;
            this.btnSaveDataBase.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveDataBase.Location = new System.Drawing.Point(784, 2);
            this.btnSaveDataBase.Name = "btnSaveDataBase";
            this.btnSaveDataBase.OverImage = null;
            this.btnSaveDataBase.Size = new System.Drawing.Size(136, 26);
            this.btnSaveDataBase.TabIndex = 0;
            this.btnSaveDataBase.Text = "Save To DataBase";
            this.btnSaveDataBase.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveDataBase.UpImage = null;
            this.btnSaveDataBase.UseOneImage = true;
            this.btnSaveDataBase.UseVisualStyleBackColor = true;
            this.btnSaveDataBase.Click += new System.EventHandler(this.btnSaveDataBase_Click);
            // 
            // btnResettingOffset
            // 
            this.btnResettingOffset.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnResettingOffset.AutoSize = true;
            this.btnResettingOffset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnResettingOffset.BgDefault = null;
            this.btnResettingOffset.BgDisable = null;
            this.btnResettingOffset.BgOver = null;
            this.btnResettingOffset.BgPushed = null;
            this.btnResettingOffset.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnResettingOffset.ConnectedLabel = null;
            this.btnResettingOffset.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnResettingOffset.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnResettingOffset.DefaultImage = null;
            this.btnResettingOffset.Description = null;
            this.btnResettingOffset.DownImage = null;
            this.btnResettingOffset.Image = global::Sineva.VHL.Device.Properties.Resources.download;
            this.btnResettingOffset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnResettingOffset.Location = new System.Drawing.Point(642, 2);
            this.btnResettingOffset.Name = "btnResettingOffset";
            this.btnResettingOffset.OverImage = null;
            this.btnResettingOffset.Size = new System.Drawing.Size(136, 26);
            this.btnResettingOffset.TabIndex = 8;
            this.btnResettingOffset.Text = "Resetting Offset";
            this.btnResettingOffset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnResettingOffset.UpImage = null;
            this.btnResettingOffset.UseOneImage = true;
            this.btnResettingOffset.UseVisualStyleBackColor = true;
            this.btnResettingOffset.Click += new System.EventHandler(this.btnResettingOffset_Click);
            // 
            // ucManualTeachingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.Controls.Add(this.splitContainer1);
            this.Name = "ucManualTeachingView";
            this.Size = new System.Drawing.Size(1203, 500);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.hpTeachingData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTeachingData)).EndInit();
            this.hpCurrentPosition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrentPosition)).EndInit();
            this.hpOffsetData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOffset)).EndInit();
            this.tcAxesControl.ResumeLayout(false);
            this.tbpHoist.ResumeLayout(false);
            this.tbpHoist.PerformLayout();
            this.tbpSlide.ResumeLayout(false);
            this.tbpSlide.PerformLayout();
            this.tbpRotate.ResumeLayout(false);
            this.tbpRotate.PerformLayout();
            this.tbpMaster.ResumeLayout(false);
            this.tbpMaster.PerformLayout();
            this.hpAxesStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAxesStatus)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbUnitSelect;
        private System.Windows.Forms.ListBox ltBoxPortList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Library.HeaderPanel hpTeachingData;
        private Library.HeaderPanel hpCurrentPosition;
        private Library.HeaderPanel hpOffsetData;
        private System.Windows.Forms.TabControl tcAxesControl;
        private System.Windows.Forms.TabPage tbpHoist;
        private System.Windows.Forms.TabPage tbpSlide;
        private System.Windows.Forms.TabPage tbpRotate;
        private System.Windows.Forms.TabPage tbpMaster;
        private System.Windows.Forms.DataGridView dgvTeachingData;
        private System.Windows.Forms.DataGridView dgvCurrentPosition;
        private System.Windows.Forms.DataGridView dgvOffset;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox tbPortID;
        private System.Windows.Forms.Label label1;
        private Library.HeaderPanel hpAxesStatus;
        private Library.DoubleBufferedGridView dgvAxesStatus;
        private System.Windows.Forms.Label lbHoistDistance;
        private System.Windows.Forms.Label lbHoistVelocity;
        private System.Windows.Forms.RadioButton rbHoistStepMove;
        private System.Windows.Forms.RadioButton rbHoistVelocityMove;
        private System.Windows.Forms.TextBox tbHoistDistance;
        private System.Windows.Forms.TextBox tbHoistVeloctiy;
        private System.Windows.Forms.TextBox tbSlideDistance;
        private System.Windows.Forms.TextBox tbSlideVelocity;
        private System.Windows.Forms.Label lbSlideDistance;
        private System.Windows.Forms.Label lbSlideVelocity;
        private System.Windows.Forms.RadioButton rbSlideStepMove;
        private System.Windows.Forms.RadioButton rbSlideVelocityMove;
        private System.Windows.Forms.TextBox tbRotateDistance;
        private System.Windows.Forms.TextBox tbRotateVelocity;
        private System.Windows.Forms.Label lbRotateDistance;
        private System.Windows.Forms.Label lbRotateVelocity;
        private System.Windows.Forms.RadioButton rbRotateStepMove;
        private System.Windows.Forms.RadioButton rbRotateVelocityMove;
        private System.Windows.Forms.TextBox tbMasterDistance;
        private System.Windows.Forms.TextBox tbMasterVelocity;
        private System.Windows.Forms.Label lbMasterDistance;
        private System.Windows.Forms.Label lbMasterVelocity;
        private System.Windows.Forms.RadioButton rbMasterStepMove;
        private System.Windows.Forms.RadioButton rbMasterVelocityMove;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel panel1;
        private Library.IButton btnSaveDataBase;
        private System.Windows.Forms.CheckBox cbReferenceVehicle;
        private Library.IButton btnHoistMoveToHome;
        private Library.IButton btnHoistMoveToTeaching;
        private Library.ColorButton btnHoistJogStop;
        private Library.IButton btnSlideMoveToHome;
        private Library.IButton btnSlideMoveToTeaching;
        private Library.ColorButton btnSlideJogStop;
        private Library.IButton btnRotateMoveToHome;
        private Library.IButton btnRotateMoveToTeaching;
        private Library.ColorButton btnRotateJogStop;
        private Library.ColorButton btnMasterJogStop;
        private System.Windows.Forms.RadioButton rbUnload;
        private System.Windows.Forms.RadioButton rbLoad;
        private Library.IO.ucLed ucLedO1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label7;
        private Library.IO.ucLed ucLedO5;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label5;
        private Library.IO.ucLed ucLedO4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private Library.IO.ucLed ucLedO3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private Library.IO.ucLed ucLedO2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Button btnBcrSet;
        private Library.JogButton btnJogZ_minus;
        private Library.JogButton btnJogZ_plus;
        private Library.JogButton btnJogY_minus;
        private Library.JogButton btnJogY_plus;
        private Library.JogButton btnJogT_minus;
        private Library.JogButton btnJogT_plus;
        private Library.JogButton btnJogX_minus;
        private Library.JogButton btnJogX_plus;
        private Library.IButton btnHoistMoveToBeforeDown;
        private Library.IButton btnUpdateOffset;
        private Library.IButton btnResettingOffset;
    }
}
