namespace Sineva.VHL.GUI
{
    partial class ucOcsCommunication
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnCommandAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnCommandDel = new System.Windows.Forms.Button();
            this.btnMakePath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCommandSave = new System.Windows.Forms.Button();
            this.panelCycleInfo = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbAntiDropTime = new System.Windows.Forms.TextBox();
            this.tbSteerChangeTime = new System.Windows.Forms.TextBox();
            this.tbDepositTime = new System.Windows.Forms.TextBox();
            this.tbAquireTime = new System.Windows.Forms.TextBox();
            this.tbCycleWaitTime = new System.Windows.Forms.TextBox();
            this.tbCycleTotalCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSetCurCommand = new System.Windows.Forms.Button();
            this.btnCancelTest = new System.Windows.Forms.Button();
            this.lbAlarmText = new System.Windows.Forms.Label();
            this.lbServoAlarmCode = new System.Windows.Forms.Label();
            this.cbbUpRangeOver_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbFoupSingleDetect_Acquire = new System.Windows.Forms.CheckBox();
            this.Panel_debug = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbbLimitDetect_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbFoupSingleDetect_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbHoistSwing_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbUpRangeOver_Deposit = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbbLimitDetect_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbHoistSwing_Acquire = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbbES_Signal_Off_before_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbHO_Signal_Off_Depositing = new System.Windows.Forms.CheckBox();
            this.cbbHO_Signal_Off_before_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbES_Signal_Off_Depositing = new System.Windows.Forms.CheckBox();
            this.cbbTP3_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbTA2_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbTA3_Deposit = new System.Windows.Forms.CheckBox();
            this.cbbTA1_Deposit = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbbES_Signal_Off_before_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbHO_Signal_Off_Acquiring = new System.Windows.Forms.CheckBox();
            this.cbbHO_Signal_Off_before_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbES_Signal_Off_Acquiring = new System.Windows.Forms.CheckBox();
            this.cbbTA2_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbTA3_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbTA1_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbRangeOver_Acquire = new System.Windows.Forms.CheckBox();
            this.cbbRangeOver_Deposit = new System.Windows.Forms.CheckBox();
            this.dgvPathList = new Sineva.VHL.Library.DoubleBufferedGridView();
            this.filteredPropertyGrid1 = new Sineva.VHL.Library.FilteredPropertyGrid();
            this.panelCycleInfo.SuspendLayout();
            this.Panel_debug.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPathList)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(463, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "[Transfer Command Edit]";
            // 
            // btnCommandAdd
            // 
            this.btnCommandAdd.Location = new System.Drawing.Point(382, 36);
            this.btnCommandAdd.Name = "btnCommandAdd";
            this.btnCommandAdd.Size = new System.Drawing.Size(66, 33);
            this.btnCommandAdd.TabIndex = 2;
            this.btnCommandAdd.Text = "<= Add";
            this.btnCommandAdd.UseVisualStyleBackColor = true;
            this.btnCommandAdd.Click += new System.EventHandler(this.btnCommandAdd_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(11, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Transfer Command List";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(13, 36);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(347, 424);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btnCommandDel
            // 
            this.btnCommandDel.Location = new System.Drawing.Point(382, 75);
            this.btnCommandDel.Name = "btnCommandDel";
            this.btnCommandDel.Size = new System.Drawing.Size(66, 33);
            this.btnCommandDel.TabIndex = 6;
            this.btnCommandDel.Text = "Del =>";
            this.btnCommandDel.UseVisualStyleBackColor = true;
            this.btnCommandDel.Click += new System.EventHandler(this.btnCommandDel_Click);
            // 
            // btnMakePath
            // 
            this.btnMakePath.Location = new System.Drawing.Point(868, 58);
            this.btnMakePath.Name = "btnMakePath";
            this.btnMakePath.Size = new System.Drawing.Size(48, 80);
            this.btnMakePath.TabIndex = 7;
            this.btnMakePath.Text = "Make Path =>";
            this.btnMakePath.UseVisualStyleBackColor = true;
            this.btnMakePath.Click += new System.EventHandler(this.btnMakePath_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(940, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "[Transfer Path List]";
            // 
            // btnCommandSave
            // 
            this.btnCommandSave.Location = new System.Drawing.Point(382, 114);
            this.btnCommandSave.Name = "btnCommandSave";
            this.btnCommandSave.Size = new System.Drawing.Size(66, 33);
            this.btnCommandSave.TabIndex = 10;
            this.btnCommandSave.Text = "< Save";
            this.btnCommandSave.UseVisualStyleBackColor = true;
            this.btnCommandSave.Click += new System.EventHandler(this.btnCommandSave_Click);
            // 
            // panelCycleInfo
            // 
            this.panelCycleInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelCycleInfo.BackColor = System.Drawing.SystemColors.Window;
            this.panelCycleInfo.Controls.Add(this.label9);
            this.panelCycleInfo.Controls.Add(this.label8);
            this.panelCycleInfo.Controls.Add(this.label7);
            this.panelCycleInfo.Controls.Add(this.label6);
            this.panelCycleInfo.Controls.Add(this.tbAntiDropTime);
            this.panelCycleInfo.Controls.Add(this.tbSteerChangeTime);
            this.panelCycleInfo.Controls.Add(this.tbDepositTime);
            this.panelCycleInfo.Controls.Add(this.tbAquireTime);
            this.panelCycleInfo.Controls.Add(this.tbCycleWaitTime);
            this.panelCycleInfo.Controls.Add(this.tbCycleTotalCount);
            this.panelCycleInfo.Controls.Add(this.label5);
            this.panelCycleInfo.Controls.Add(this.label4);
            this.panelCycleInfo.Location = new System.Drawing.Point(833, 271);
            this.panelCycleInfo.Name = "panelCycleInfo";
            this.panelCycleInfo.Size = new System.Drawing.Size(91, 188);
            this.panelCycleInfo.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 11;
            this.label9.Text = "AT :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 146);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "SC :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "DP :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "AQ :";
            // 
            // tbAntiDropTime
            // 
            this.tbAntiDropTime.Location = new System.Drawing.Point(36, 164);
            this.tbAntiDropTime.Name = "tbAntiDropTime";
            this.tbAntiDropTime.Size = new System.Drawing.Size(45, 21);
            this.tbAntiDropTime.TabIndex = 7;
            // 
            // tbSteerChangeTime
            // 
            this.tbSteerChangeTime.Location = new System.Drawing.Point(36, 141);
            this.tbSteerChangeTime.Name = "tbSteerChangeTime";
            this.tbSteerChangeTime.Size = new System.Drawing.Size(45, 21);
            this.tbSteerChangeTime.TabIndex = 6;
            // 
            // tbDepositTime
            // 
            this.tbDepositTime.Location = new System.Drawing.Point(36, 117);
            this.tbDepositTime.Name = "tbDepositTime";
            this.tbDepositTime.Size = new System.Drawing.Size(45, 21);
            this.tbDepositTime.TabIndex = 5;
            // 
            // tbAquireTime
            // 
            this.tbAquireTime.Location = new System.Drawing.Point(36, 94);
            this.tbAquireTime.Name = "tbAquireTime";
            this.tbAquireTime.Size = new System.Drawing.Size(45, 21);
            this.tbAquireTime.TabIndex = 4;
            // 
            // tbCycleWaitTime
            // 
            this.tbCycleWaitTime.Location = new System.Drawing.Point(6, 63);
            this.tbCycleWaitTime.Name = "tbCycleWaitTime";
            this.tbCycleWaitTime.Size = new System.Drawing.Size(75, 21);
            this.tbCycleWaitTime.TabIndex = 3;
            this.tbCycleWaitTime.TextChanged += new System.EventHandler(this.tbCycleInfo_TextChanged);
            // 
            // tbCycleTotalCount
            // 
            this.tbCycleTotalCount.Location = new System.Drawing.Point(6, 19);
            this.tbCycleTotalCount.Name = "tbCycleTotalCount";
            this.tbCycleTotalCount.Size = new System.Drawing.Size(75, 21);
            this.tbCycleTotalCount.TabIndex = 2;
            this.tbCycleTotalCount.TextChanged += new System.EventHandler(this.tbCycleInfo_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Wait Time :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Total Count :";
            // 
            // btnSetCurCommand
            // 
            this.btnSetCurCommand.Location = new System.Drawing.Point(869, 144);
            this.btnSetCurCommand.Name = "btnSetCurCommand";
            this.btnSetCurCommand.Size = new System.Drawing.Size(48, 80);
            this.btnSetCurCommand.TabIndex = 12;
            this.btnSetCurCommand.Text = "Set Command";
            this.btnSetCurCommand.UseVisualStyleBackColor = true;
            this.btnSetCurCommand.Click += new System.EventHandler(this.btnSetCurCommand_Click);
            // 
            // btnCancelTest
            // 
            this.btnCancelTest.Location = new System.Drawing.Point(382, 374);
            this.btnCancelTest.Name = "btnCancelTest";
            this.btnCancelTest.Size = new System.Drawing.Size(66, 33);
            this.btnCancelTest.TabIndex = 10;
            this.btnCancelTest.Text = "Cancel";
            this.btnCancelTest.UseVisualStyleBackColor = true;
            this.btnCancelTest.Click += new System.EventHandler(this.btnCancelTest_Click);
            // 
            // lbAlarmText
            // 
            this.lbAlarmText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbAlarmText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lbAlarmText.Font = new System.Drawing.Font("굴림", 14F);
            this.lbAlarmText.ForeColor = System.Drawing.Color.Red;
            this.lbAlarmText.Location = new System.Drawing.Point(13, 479);
            this.lbAlarmText.Name = "lbAlarmText";
            this.lbAlarmText.Size = new System.Drawing.Size(1253, 53);
            this.lbAlarmText.TabIndex = 13;
            // 
            // lbServoAlarmCode
            // 
            this.lbServoAlarmCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbServoAlarmCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lbServoAlarmCode.Font = new System.Drawing.Font("굴림", 14F);
            this.lbServoAlarmCode.ForeColor = System.Drawing.Color.Red;
            this.lbServoAlarmCode.Location = new System.Drawing.Point(1272, 479);
            this.lbServoAlarmCode.Name = "lbServoAlarmCode";
            this.lbServoAlarmCode.Size = new System.Drawing.Size(143, 53);
            this.lbServoAlarmCode.TabIndex = 14;
            // 
            // cbbUpRangeOver_Acquire
            // 
            this.cbbUpRangeOver_Acquire.AutoSize = true;
            this.cbbUpRangeOver_Acquire.Location = new System.Drawing.Point(6, 16);
            this.cbbUpRangeOver_Acquire.Name = "cbbUpRangeOver_Acquire";
            this.cbbUpRangeOver_Acquire.Size = new System.Drawing.Size(101, 16);
            this.cbbUpRangeOver_Acquire.TabIndex = 15;
            this.cbbUpRangeOver_Acquire.Text = "UpRangeOver";
            this.cbbUpRangeOver_Acquire.UseVisualStyleBackColor = true;
            this.cbbUpRangeOver_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbFoupSingleDetect_Acquire
            // 
            this.cbbFoupSingleDetect_Acquire.AutoSize = true;
            this.cbbFoupSingleDetect_Acquire.Location = new System.Drawing.Point(6, 38);
            this.cbbFoupSingleDetect_Acquire.Name = "cbbFoupSingleDetect_Acquire";
            this.cbbFoupSingleDetect_Acquire.Size = new System.Drawing.Size(122, 16);
            this.cbbFoupSingleDetect_Acquire.TabIndex = 16;
            this.cbbFoupSingleDetect_Acquire.Text = "FoupSingleDetect";
            this.cbbFoupSingleDetect_Acquire.UseVisualStyleBackColor = true;
            this.cbbFoupSingleDetect_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // Panel_debug
            // 
            this.Panel_debug.Controls.Add(this.tabControl1);
            this.Panel_debug.Location = new System.Drawing.Point(13, 194);
            this.Panel_debug.Name = "Panel_debug";
            this.Panel_debug.Size = new System.Drawing.Size(347, 266);
            this.Panel_debug.TabIndex = 21;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(2, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(345, 263);
            this.tabControl1.TabIndex = 22;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(337, 237);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "transfer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbbRangeOver_Deposit);
            this.groupBox2.Controls.Add(this.cbbLimitDetect_Deposit);
            this.groupBox2.Controls.Add(this.cbbFoupSingleDetect_Deposit);
            this.groupBox2.Controls.Add(this.cbbHoistSwing_Deposit);
            this.groupBox2.Controls.Add(this.cbbUpRangeOver_Deposit);
            this.groupBox2.Location = new System.Drawing.Point(169, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(165, 225);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Deposit";
            // 
            // cbbLimitDetect_Deposit
            // 
            this.cbbLimitDetect_Deposit.AutoSize = true;
            this.cbbLimitDetect_Deposit.Location = new System.Drawing.Point(7, 82);
            this.cbbLimitDetect_Deposit.Name = "cbbLimitDetect_Deposit";
            this.cbbLimitDetect_Deposit.Size = new System.Drawing.Size(86, 16);
            this.cbbLimitDetect_Deposit.TabIndex = 20;
            this.cbbLimitDetect_Deposit.Text = "LimitDetect";
            this.cbbLimitDetect_Deposit.UseVisualStyleBackColor = true;
            this.cbbLimitDetect_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbFoupSingleDetect_Deposit
            // 
            this.cbbFoupSingleDetect_Deposit.AutoSize = true;
            this.cbbFoupSingleDetect_Deposit.Location = new System.Drawing.Point(6, 38);
            this.cbbFoupSingleDetect_Deposit.Name = "cbbFoupSingleDetect_Deposit";
            this.cbbFoupSingleDetect_Deposit.Size = new System.Drawing.Size(122, 16);
            this.cbbFoupSingleDetect_Deposit.TabIndex = 16;
            this.cbbFoupSingleDetect_Deposit.Text = "FoupSingleDetect";
            this.cbbFoupSingleDetect_Deposit.UseVisualStyleBackColor = true;
            this.cbbFoupSingleDetect_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbHoistSwing_Deposit
            // 
            this.cbbHoistSwing_Deposit.AutoSize = true;
            this.cbbHoistSwing_Deposit.Location = new System.Drawing.Point(6, 60);
            this.cbbHoistSwing_Deposit.Name = "cbbHoistSwing_Deposit";
            this.cbbHoistSwing_Deposit.Size = new System.Drawing.Size(87, 16);
            this.cbbHoistSwing_Deposit.TabIndex = 19;
            this.cbbHoistSwing_Deposit.Text = "HoistSwing";
            this.cbbHoistSwing_Deposit.UseVisualStyleBackColor = true;
            this.cbbHoistSwing_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbUpRangeOver_Deposit
            // 
            this.cbbUpRangeOver_Deposit.AutoSize = true;
            this.cbbUpRangeOver_Deposit.Location = new System.Drawing.Point(6, 16);
            this.cbbUpRangeOver_Deposit.Name = "cbbUpRangeOver_Deposit";
            this.cbbUpRangeOver_Deposit.Size = new System.Drawing.Size(101, 16);
            this.cbbUpRangeOver_Deposit.TabIndex = 15;
            this.cbbUpRangeOver_Deposit.Text = "UpRangeOver";
            this.cbbUpRangeOver_Deposit.UseVisualStyleBackColor = true;
            this.cbbUpRangeOver_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbbRangeOver_Acquire);
            this.groupBox1.Controls.Add(this.cbbLimitDetect_Acquire);
            this.groupBox1.Controls.Add(this.cbbFoupSingleDetect_Acquire);
            this.groupBox1.Controls.Add(this.cbbHoistSwing_Acquire);
            this.groupBox1.Controls.Add(this.cbbUpRangeOver_Acquire);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(165, 228);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Acquire";
            // 
            // cbbLimitDetect_Acquire
            // 
            this.cbbLimitDetect_Acquire.AutoSize = true;
            this.cbbLimitDetect_Acquire.Location = new System.Drawing.Point(6, 88);
            this.cbbLimitDetect_Acquire.Name = "cbbLimitDetect_Acquire";
            this.cbbLimitDetect_Acquire.Size = new System.Drawing.Size(86, 16);
            this.cbbLimitDetect_Acquire.TabIndex = 20;
            this.cbbLimitDetect_Acquire.Text = "LimitDetect";
            this.cbbLimitDetect_Acquire.UseVisualStyleBackColor = true;
            this.cbbLimitDetect_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbHoistSwing_Acquire
            // 
            this.cbbHoistSwing_Acquire.AutoSize = true;
            this.cbbHoistSwing_Acquire.Location = new System.Drawing.Point(6, 63);
            this.cbbHoistSwing_Acquire.Name = "cbbHoistSwing_Acquire";
            this.cbbHoistSwing_Acquire.Size = new System.Drawing.Size(87, 16);
            this.cbbHoistSwing_Acquire.TabIndex = 19;
            this.cbbHoistSwing_Acquire.Text = "HoistSwing";
            this.cbbHoistSwing_Acquire.UseVisualStyleBackColor = true;
            this.cbbHoistSwing_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(337, 237);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "PIO";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbbES_Signal_Off_before_Deposit);
            this.groupBox3.Controls.Add(this.cbbHO_Signal_Off_Depositing);
            this.groupBox3.Controls.Add(this.cbbHO_Signal_Off_before_Deposit);
            this.groupBox3.Controls.Add(this.cbbES_Signal_Off_Depositing);
            this.groupBox3.Controls.Add(this.cbbTP3_Deposit);
            this.groupBox3.Controls.Add(this.cbbTA2_Deposit);
            this.groupBox3.Controls.Add(this.cbbTA3_Deposit);
            this.groupBox3.Controls.Add(this.cbbTA1_Deposit);
            this.groupBox3.Location = new System.Drawing.Point(169, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(165, 225);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Deposit";
            // 
            // cbbES_Signal_Off_before_Deposit
            // 
            this.cbbES_Signal_Off_before_Deposit.AutoSize = true;
            this.cbbES_Signal_Off_before_Deposit.Location = new System.Drawing.Point(4, 103);
            this.cbbES_Signal_Off_before_Deposit.Name = "cbbES_Signal_Off_before_Deposit";
            this.cbbES_Signal_Off_before_Deposit.Size = new System.Drawing.Size(102, 16);
            this.cbbES_Signal_Off_before_Deposit.TabIndex = 25;
            this.cbbES_Signal_Off_before_Deposit.Text = "ES_Signal_Off";
            this.cbbES_Signal_Off_before_Deposit.UseVisualStyleBackColor = true;
            this.cbbES_Signal_Off_before_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbHO_Signal_Off_Depositing
            // 
            this.cbbHO_Signal_Off_Depositing.AutoSize = true;
            this.cbbHO_Signal_Off_Depositing.Location = new System.Drawing.Point(4, 169);
            this.cbbHO_Signal_Off_Depositing.Name = "cbbHO_Signal_Off_Depositing";
            this.cbbHO_Signal_Off_Depositing.Size = new System.Drawing.Size(168, 16);
            this.cbbHO_Signal_Off_Depositing.TabIndex = 27;
            this.cbbHO_Signal_Off_Depositing.Text = "HO_Signal_Off_Depositing";
            this.cbbHO_Signal_Off_Depositing.UseVisualStyleBackColor = true;
            this.cbbHO_Signal_Off_Depositing.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbHO_Signal_Off_before_Deposit
            // 
            this.cbbHO_Signal_Off_before_Deposit.AutoSize = true;
            this.cbbHO_Signal_Off_before_Deposit.Location = new System.Drawing.Point(4, 147);
            this.cbbHO_Signal_Off_before_Deposit.Name = "cbbHO_Signal_Off_before_Deposit";
            this.cbbHO_Signal_Off_before_Deposit.Size = new System.Drawing.Size(103, 16);
            this.cbbHO_Signal_Off_before_Deposit.TabIndex = 26;
            this.cbbHO_Signal_Off_before_Deposit.Text = "HO_Signal_Off";
            this.cbbHO_Signal_Off_before_Deposit.UseVisualStyleBackColor = true;
            this.cbbHO_Signal_Off_before_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbES_Signal_Off_Depositing
            // 
            this.cbbES_Signal_Off_Depositing.AutoSize = true;
            this.cbbES_Signal_Off_Depositing.Location = new System.Drawing.Point(4, 125);
            this.cbbES_Signal_Off_Depositing.Name = "cbbES_Signal_Off_Depositing";
            this.cbbES_Signal_Off_Depositing.Size = new System.Drawing.Size(167, 16);
            this.cbbES_Signal_Off_Depositing.TabIndex = 28;
            this.cbbES_Signal_Off_Depositing.Text = "ES_Signal_Off_Depositing";
            this.cbbES_Signal_Off_Depositing.UseVisualStyleBackColor = true;
            this.cbbES_Signal_Off_Depositing.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTP3_Deposit
            // 
            this.cbbTP3_Deposit.AutoSize = true;
            this.cbbTP3_Deposit.Location = new System.Drawing.Point(6, 57);
            this.cbbTP3_Deposit.Name = "cbbTP3_Deposit";
            this.cbbTP3_Deposit.Size = new System.Drawing.Size(105, 16);
            this.cbbTP3_Deposit.TabIndex = 24;
            this.cbbTP3_Deposit.Text = "TP3_L_Not_Off";
            this.cbbTP3_Deposit.UseVisualStyleBackColor = true;
            this.cbbTP3_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTA2_Deposit
            // 
            this.cbbTA2_Deposit.AutoSize = true;
            this.cbbTA2_Deposit.Location = new System.Drawing.Point(6, 35);
            this.cbbTA2_Deposit.Name = "cbbTA2_Deposit";
            this.cbbTA2_Deposit.Size = new System.Drawing.Size(135, 16);
            this.cbbTA2_Deposit.TabIndex = 22;
            this.cbbTA2_Deposit.Text = "TA2_Ready_Not_On";
            this.cbbTA2_Deposit.UseVisualStyleBackColor = true;
            this.cbbTA2_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTA3_Deposit
            // 
            this.cbbTA3_Deposit.AutoSize = true;
            this.cbbTA3_Deposit.Location = new System.Drawing.Point(6, 79);
            this.cbbTA3_Deposit.Name = "cbbTA3_Deposit";
            this.cbbTA3_Deposit.Size = new System.Drawing.Size(134, 16);
            this.cbbTA3_Deposit.TabIndex = 23;
            this.cbbTA3_Deposit.Text = "TA3_Ready_Not_Off";
            this.cbbTA3_Deposit.UseVisualStyleBackColor = true;
            this.cbbTA3_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTA1_Deposit
            // 
            this.cbbTA1_Deposit.AutoSize = true;
            this.cbbTA1_Deposit.Location = new System.Drawing.Point(6, 13);
            this.cbbTA1_Deposit.Name = "cbbTA1_Deposit";
            this.cbbTA1_Deposit.Size = new System.Drawing.Size(106, 16);
            this.cbbTA1_Deposit.TabIndex = 21;
            this.cbbTA1_Deposit.Text = "TA1_L_Not_On";
            this.cbbTA1_Deposit.UseVisualStyleBackColor = true;
            this.cbbTA1_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbbES_Signal_Off_before_Acquire);
            this.groupBox4.Controls.Add(this.cbbHO_Signal_Off_Acquiring);
            this.groupBox4.Controls.Add(this.cbbHO_Signal_Off_before_Acquire);
            this.groupBox4.Controls.Add(this.cbbES_Signal_Off_Acquiring);
            this.groupBox4.Controls.Add(this.cbbTA2_Acquire);
            this.groupBox4.Controls.Add(this.cbbTA3_Acquire);
            this.groupBox4.Controls.Add(this.cbbTA1_Acquire);
            this.groupBox4.Location = new System.Drawing.Point(3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(165, 228);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Acquire";
            // 
            // cbbES_Signal_Off_before_Acquire
            // 
            this.cbbES_Signal_Off_before_Acquire.AutoSize = true;
            this.cbbES_Signal_Off_before_Acquire.Location = new System.Drawing.Point(5, 106);
            this.cbbES_Signal_Off_before_Acquire.Name = "cbbES_Signal_Off_before_Acquire";
            this.cbbES_Signal_Off_before_Acquire.Size = new System.Drawing.Size(102, 16);
            this.cbbES_Signal_Off_before_Acquire.TabIndex = 19;
            this.cbbES_Signal_Off_before_Acquire.Text = "ES_Signal_Off";
            this.cbbES_Signal_Off_before_Acquire.UseVisualStyleBackColor = true;
            this.cbbES_Signal_Off_before_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbHO_Signal_Off_Acquiring
            // 
            this.cbbHO_Signal_Off_Acquiring.AutoSize = true;
            this.cbbHO_Signal_Off_Acquiring.Location = new System.Drawing.Point(5, 172);
            this.cbbHO_Signal_Off_Acquiring.Name = "cbbHO_Signal_Off_Acquiring";
            this.cbbHO_Signal_Off_Acquiring.Size = new System.Drawing.Size(162, 16);
            this.cbbHO_Signal_Off_Acquiring.TabIndex = 21;
            this.cbbHO_Signal_Off_Acquiring.Text = "HO_Signal_Off_Acquiring";
            this.cbbHO_Signal_Off_Acquiring.UseVisualStyleBackColor = true;
            this.cbbHO_Signal_Off_Acquiring.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbHO_Signal_Off_before_Acquire
            // 
            this.cbbHO_Signal_Off_before_Acquire.AutoSize = true;
            this.cbbHO_Signal_Off_before_Acquire.Location = new System.Drawing.Point(5, 150);
            this.cbbHO_Signal_Off_before_Acquire.Name = "cbbHO_Signal_Off_before_Acquire";
            this.cbbHO_Signal_Off_before_Acquire.Size = new System.Drawing.Size(103, 16);
            this.cbbHO_Signal_Off_before_Acquire.TabIndex = 20;
            this.cbbHO_Signal_Off_before_Acquire.Text = "HO_Signal_Off";
            this.cbbHO_Signal_Off_before_Acquire.UseVisualStyleBackColor = true;
            this.cbbHO_Signal_Off_before_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbES_Signal_Off_Acquiring
            // 
            this.cbbES_Signal_Off_Acquiring.AutoSize = true;
            this.cbbES_Signal_Off_Acquiring.Location = new System.Drawing.Point(5, 128);
            this.cbbES_Signal_Off_Acquiring.Name = "cbbES_Signal_Off_Acquiring";
            this.cbbES_Signal_Off_Acquiring.Size = new System.Drawing.Size(161, 16);
            this.cbbES_Signal_Off_Acquiring.TabIndex = 22;
            this.cbbES_Signal_Off_Acquiring.Text = "ES_Signal_Off_Acquiring";
            this.cbbES_Signal_Off_Acquiring.UseVisualStyleBackColor = true;
            this.cbbES_Signal_Off_Acquiring.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTA2_Acquire
            // 
            this.cbbTA2_Acquire.AutoSize = true;
            this.cbbTA2_Acquire.Location = new System.Drawing.Point(6, 38);
            this.cbbTA2_Acquire.Name = "cbbTA2_Acquire";
            this.cbbTA2_Acquire.Size = new System.Drawing.Size(135, 16);
            this.cbbTA2_Acquire.TabIndex = 16;
            this.cbbTA2_Acquire.Text = "TA2_Ready_Not_On";
            this.cbbTA2_Acquire.UseVisualStyleBackColor = true;
            this.cbbTA2_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTA3_Acquire
            // 
            this.cbbTA3_Acquire.AutoSize = true;
            this.cbbTA3_Acquire.Location = new System.Drawing.Point(6, 60);
            this.cbbTA3_Acquire.Name = "cbbTA3_Acquire";
            this.cbbTA3_Acquire.Size = new System.Drawing.Size(134, 16);
            this.cbbTA3_Acquire.TabIndex = 17;
            this.cbbTA3_Acquire.Text = "TA3_Ready_Not_Off";
            this.cbbTA3_Acquire.UseVisualStyleBackColor = true;
            this.cbbTA3_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbTA1_Acquire
            // 
            this.cbbTA1_Acquire.AutoSize = true;
            this.cbbTA1_Acquire.Location = new System.Drawing.Point(6, 16);
            this.cbbTA1_Acquire.Name = "cbbTA1_Acquire";
            this.cbbTA1_Acquire.Size = new System.Drawing.Size(114, 16);
            this.cbbTA1_Acquire.TabIndex = 15;
            this.cbbTA1_Acquire.Text = "TA1_UL_Not_On";
            this.cbbTA1_Acquire.UseVisualStyleBackColor = true;
            this.cbbTA1_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbRangeOver_Acquire
            // 
            this.cbbRangeOver_Acquire.AutoSize = true;
            this.cbbRangeOver_Acquire.Location = new System.Drawing.Point(6, 110);
            this.cbbRangeOver_Acquire.Name = "cbbRangeOver_Acquire";
            this.cbbRangeOver_Acquire.Size = new System.Drawing.Size(96, 16);
            this.cbbRangeOver_Acquire.TabIndex = 21;
            this.cbbRangeOver_Acquire.Text = "InRangeOver";
            this.cbbRangeOver_Acquire.UseVisualStyleBackColor = true;
            this.cbbRangeOver_Acquire.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // cbbRangeOver_Deposit
            // 
            this.cbbRangeOver_Deposit.AutoSize = true;
            this.cbbRangeOver_Deposit.Location = new System.Drawing.Point(6, 107);
            this.cbbRangeOver_Deposit.Name = "cbbRangeOver_Deposit";
            this.cbbRangeOver_Deposit.Size = new System.Drawing.Size(96, 16);
            this.cbbRangeOver_Deposit.TabIndex = 22;
            this.cbbRangeOver_Deposit.Text = "InRangeOver";
            this.cbbRangeOver_Deposit.UseVisualStyleBackColor = true;
            this.cbbRangeOver_Deposit.CheckedChanged += new System.EventHandler(this.cbb_CheckedChanged);
            // 
            // dgvPathList
            // 
            this.dgvPathList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPathList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPathList.Location = new System.Drawing.Point(942, 36);
            this.dgvPathList.Name = "dgvPathList";
            this.dgvPathList.RowTemplate.Height = 23;
            this.dgvPathList.Size = new System.Drawing.Size(473, 424);
            this.dgvPathList.TabIndex = 8;
            // 
            // filteredPropertyGrid1
            // 
            this.filteredPropertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.filteredPropertyGrid1.BrowsableProperties = null;
            this.filteredPropertyGrid1.HiddenAttributes = null;
            this.filteredPropertyGrid1.HiddenProperties = null;
            this.filteredPropertyGrid1.Location = new System.Drawing.Point(465, 36);
            this.filteredPropertyGrid1.Name = "filteredPropertyGrid1";
            this.filteredPropertyGrid1.Size = new System.Drawing.Size(352, 424);
            this.filteredPropertyGrid1.TabIndex = 0;
            // 
            // ucOcsCommunication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.Panel_debug);
            this.Controls.Add(this.lbServoAlarmCode);
            this.Controls.Add(this.lbAlarmText);
            this.Controls.Add(this.btnSetCurCommand);
            this.Controls.Add(this.panelCycleInfo);
            this.Controls.Add(this.btnCancelTest);
            this.Controls.Add(this.btnCommandSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvPathList);
            this.Controls.Add(this.btnMakePath);
            this.Controls.Add(this.btnCommandDel);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCommandAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filteredPropertyGrid1);
            this.Name = "ucOcsCommunication";
            this.Size = new System.Drawing.Size(1456, 542);
            this.panelCycleInfo.ResumeLayout(false);
            this.panelCycleInfo.PerformLayout();
            this.Panel_debug.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPathList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Library.FilteredPropertyGrid filteredPropertyGrid1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCommandAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnCommandDel;
        private System.Windows.Forms.Button btnMakePath;
        private Library.DoubleBufferedGridView dgvPathList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCommandSave;
        private System.Windows.Forms.Panel panelCycleInfo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbCycleWaitTime;
        private System.Windows.Forms.TextBox tbCycleTotalCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbAntiDropTime;
        private System.Windows.Forms.TextBox tbSteerChangeTime;
        private System.Windows.Forms.TextBox tbDepositTime;
        private System.Windows.Forms.TextBox tbAquireTime;
        private System.Windows.Forms.Button btnSetCurCommand;
        private System.Windows.Forms.Button btnCancelTest;
        private System.Windows.Forms.Label lbAlarmText;
        private System.Windows.Forms.Label lbServoAlarmCode;
        private System.Windows.Forms.CheckBox cbbUpRangeOver_Acquire;
        private System.Windows.Forms.CheckBox cbbFoupSingleDetect_Acquire;
        private System.Windows.Forms.Panel Panel_debug;
        private System.Windows.Forms.CheckBox cbbHoistSwing_Acquire;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbbFoupSingleDetect_Deposit;
        private System.Windows.Forms.CheckBox cbbHoistSwing_Deposit;
        private System.Windows.Forms.CheckBox cbbUpRangeOver_Deposit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbbTA2_Acquire;
        private System.Windows.Forms.CheckBox cbbTA3_Acquire;
        private System.Windows.Forms.CheckBox cbbTA1_Acquire;
        private System.Windows.Forms.CheckBox cbbTP3_Deposit;
        private System.Windows.Forms.CheckBox cbbTA2_Deposit;
        private System.Windows.Forms.CheckBox cbbTA3_Deposit;
        private System.Windows.Forms.CheckBox cbbTA1_Deposit;
        private System.Windows.Forms.CheckBox cbbLimitDetect_Deposit;
        private System.Windows.Forms.CheckBox cbbLimitDetect_Acquire;
        private System.Windows.Forms.CheckBox cbbES_Signal_Off_before_Deposit;
        private System.Windows.Forms.CheckBox cbbHO_Signal_Off_Depositing;
        private System.Windows.Forms.CheckBox cbbHO_Signal_Off_before_Deposit;
        private System.Windows.Forms.CheckBox cbbES_Signal_Off_Depositing;
        private System.Windows.Forms.CheckBox cbbES_Signal_Off_before_Acquire;
        private System.Windows.Forms.CheckBox cbbHO_Signal_Off_Acquiring;
        private System.Windows.Forms.CheckBox cbbHO_Signal_Off_before_Acquire;
        private System.Windows.Forms.CheckBox cbbES_Signal_Off_Acquiring;
        private System.Windows.Forms.CheckBox cbbRangeOver_Deposit;
        private System.Windows.Forms.CheckBox cbbRangeOver_Acquire;
    }
}
