namespace Sineva.VHL.GUI
{
    partial class JobsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlJob = new Sineva.VHL.Library.FlatTabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.lbMxpOverride = new System.Windows.Forms.Label();
            this.lbJCSSystemByte = new System.Windows.Forms.Label();
            this.lbOCSSystemByte = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ucMotorStateView1 = new Sineva.VHL.GUI.ucMotorStateView();
            this.ucProcessDataView1 = new Sineva.VHL.GUI.ucProcessDataView();
            this.ucCommandDataView1 = new Sineva.VHL.GUI.ucCommandDataView();
            this.ucDevPio1 = new Sineva.VHL.Device.ucDevPio();
            this.ucDevGripperPio1 = new Sineva.VHL.Device.ucDevGripperPio();
            this.label14 = new System.Windows.Forms.Label();
            this.ssvMasterVelocity = new Sineva.VHL.Device.ucServoStatusValue();
            this.lbMasterTorque = new System.Windows.Forms.Label();
            this.ssvMasterTorque = new Sineva.VHL.Device.ucServoStatusValue();
            this.uamMasterTorque = new Sineva.VHL.Device.uiAnalogMeter();
            this.uamMasterVelocity = new Sineva.VHL.Device.uiAnalogMeter();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pbFOUP = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.dsdRearSteerStatus = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.label15 = new System.Windows.Forms.Label();
            this.dsdFrontSteerStatus = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.pbRearAntiDrop = new System.Windows.Forms.PictureBox();
            this.pbFrontAntiDrop = new System.Windows.Forms.PictureBox();
            this.ucOBSStatusLower = new Sineva.VHL.Device.ucDevOBSStatus();
            this.ucOBSStatusUp = new Sineva.VHL.Device.ucDevOBSStatus();
            this.lbCleaner = new System.Windows.Forms.Label();
            this.pbVHL = new System.Windows.Forms.PictureBox();
            this.ucOBSStatusPBS = new Sineva.VHL.Device.ucDevOBSStatus();
            this.tabPageManual = new System.Windows.Forms.TabPage();
            this.ucManualTeachingView1 = new Sineva.VHL.Device.ucManualTeachingView();
            this.DevicePanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.steerPanel = new Sineva.VHL.Library.HeaderPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucSteerActionView1 = new Sineva.VHL.Device.ucDevSteerAction();
            this.piotestPanel = new Sineva.VHL.Library.HeaderPanel();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnPioTest = new Sineva.VHL.Library.IButton();
            this.autoteachingPanel = new Sineva.VHL.Library.HeaderPanel();
            this.lbVisionResult = new System.Windows.Forms.Label();
            this.btnAutoTeachingMonitor = new Sineva.VHL.Library.IButton();
            this.gripperPanel = new Sineva.VHL.Library.HeaderPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ucGripperActionView1 = new Sineva.VHL.Device.ucDevGripperAction();
            this.rfidPanel = new Sineva.VHL.Library.HeaderPanel();
            this.ucRFIDTag1 = new Sineva.VHL.Device.Design.ucRFIDTag();
            this.btnRFIDRead = new Sineva.VHL.Library.IButton();
            this.panel6 = new System.Windows.Forms.Panel();
            this.antiDropPanel = new Sineva.VHL.Library.HeaderPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ucDevAntiDropAction1 = new Sineva.VHL.Device.ucDevAntiDropAction();
            this.ucDevCleaner1 = new Sineva.VHL.Device.ucDevCleaner();
            this.tabPageOCS = new System.Windows.Forms.TabPage();
            this.ucOcsCommunication1 = new Sineva.VHL.GUI.ucOcsCommunication();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.alarmCurrentView1 = new Sineva.VHL.Data.Alarm.AlarmCurrentView();
            this.label1 = new System.Windows.Forms.Label();
            this.logList1 = new Sineva.VHL.Library.LogList();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pLifeTimeDisplay = new System.Windows.Forms.PictureBox();
            this.pAlarmReset = new System.Windows.Forms.PictureBox();
            this.pBuzzerOff = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControlJob.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFOUP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRearAntiDrop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrontAntiDrop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVHL)).BeginInit();
            this.tabPageManual.SuspendLayout();
            this.DevicePanel.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.steerPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.piotestPanel.SuspendLayout();
            this.autoteachingPanel.SuspendLayout();
            this.gripperPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.rfidPanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.antiDropPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPageOCS.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pLifeTimeDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pAlarmReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBuzzerOff)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelToolbar, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1901, 900);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelToolbar
            // 
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelToolbar.Location = new System.Drawing.Point(1804, 3);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(94, 894);
            this.panelToolbar.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tabControlJob, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1795, 894);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tabControlJob
            // 
            this.tabControlJob.Controls.Add(this.tabPageMain);
            this.tabControlJob.Controls.Add(this.tabPageManual);
            this.tabControlJob.Controls.Add(this.tabPageOCS);
            this.tabControlJob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlJob.ImageList = this.imageList1;
            this.tabControlJob.Location = new System.Drawing.Point(3, 3);
            this.tabControlJob.myBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tabControlJob.mySelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabControlJob.myTextColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabControlJob.Name = "tabControlJob";
            this.tabControlJob.SelectedIndex = 0;
            this.tabControlJob.Size = new System.Drawing.Size(1789, 702);
            this.tabControlJob.TabIndex = 1;
            this.tabControlJob.Tag = "JOB";
            this.tabControlJob.VisibleChanged += new System.EventHandler(this.tabControlJob_VisibleChanged);
            // 
            // tabPageMain
            // 
            this.tabPageMain.BackColor = System.Drawing.Color.Transparent;
            this.tabPageMain.Controls.Add(this.lbMxpOverride);
            this.tabPageMain.Controls.Add(this.lbJCSSystemByte);
            this.tabPageMain.Controls.Add(this.lbOCSSystemByte);
            this.tabPageMain.Controls.Add(this.label2);
            this.tabPageMain.Controls.Add(this.ucMotorStateView1);
            this.tabPageMain.Controls.Add(this.ucProcessDataView1);
            this.tabPageMain.Controls.Add(this.ucCommandDataView1);
            this.tabPageMain.Controls.Add(this.ucDevPio1);
            this.tabPageMain.Controls.Add(this.ucDevGripperPio1);
            this.tabPageMain.Controls.Add(this.label14);
            this.tabPageMain.Controls.Add(this.ssvMasterVelocity);
            this.tabPageMain.Controls.Add(this.lbMasterTorque);
            this.tabPageMain.Controls.Add(this.ssvMasterTorque);
            this.tabPageMain.Controls.Add(this.uamMasterTorque);
            this.tabPageMain.Controls.Add(this.uamMasterVelocity);
            this.tabPageMain.Controls.Add(this.panel5);
            this.tabPageMain.ImageIndex = 1;
            this.tabPageMain.Location = new System.Drawing.Point(4, 26);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(1781, 672);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Tag = "Main";
            this.tabPageMain.Text = "Main";
            // 
            // lbMxpOverride
            // 
            this.lbMxpOverride.BackColor = System.Drawing.Color.White;
            this.lbMxpOverride.Location = new System.Drawing.Point(154, 117);
            this.lbMxpOverride.Name = "lbMxpOverride";
            this.lbMxpOverride.Size = new System.Drawing.Size(86, 37);
            this.lbMxpOverride.TabIndex = 65;
            this.lbMxpOverride.Text = "label4";
            this.lbMxpOverride.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbJCSSystemByte
            // 
            this.lbJCSSystemByte.AutoSize = true;
            this.lbJCSSystemByte.Font = new System.Drawing.Font("Microsoft YaHei Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbJCSSystemByte.ForeColor = System.Drawing.Color.White;
            this.lbJCSSystemByte.Location = new System.Drawing.Point(471, 92);
            this.lbJCSSystemByte.Name = "lbJCSSystemByte";
            this.lbJCSSystemByte.Size = new System.Drawing.Size(31, 16);
            this.lbJCSSystemByte.TabIndex = 64;
            this.lbJCSSystemByte.Text = "JCS :";
            // 
            // lbOCSSystemByte
            // 
            this.lbOCSSystemByte.AutoSize = true;
            this.lbOCSSystemByte.Font = new System.Drawing.Font("Microsoft YaHei Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOCSSystemByte.ForeColor = System.Drawing.Color.White;
            this.lbOCSSystemByte.Location = new System.Drawing.Point(304, 92);
            this.lbOCSSystemByte.Name = "lbOCSSystemByte";
            this.lbOCSSystemByte.Size = new System.Drawing.Size(36, 16);
            this.lbOCSSystemByte.TabIndex = 64;
            this.lbOCSSystemByte.Text = "OCS :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(146, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 20);
            this.label2.TabIndex = 64;
            this.label2.Text = "MXP Override";
            // 
            // ucMotorStateView1
            // 
            this.ucMotorStateView1.Location = new System.Drawing.Point(949, 356);
            this.ucMotorStateView1.Name = "ucMotorStateView1";
            this.ucMotorStateView1.Size = new System.Drawing.Size(348, 251);
            this.ucMotorStateView1.TabIndex = 62;
            // 
            // ucProcessDataView1
            // 
            this.ucProcessDataView1.Location = new System.Drawing.Point(949, 20);
            this.ucProcessDataView1.Name = "ucProcessDataView1";
            this.ucProcessDataView1.Size = new System.Drawing.Size(348, 331);
            this.ucProcessDataView1.TabIndex = 61;
            // 
            // ucCommandDataView1
            // 
            this.ucCommandDataView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucCommandDataView1.Location = new System.Drawing.Point(22, 23);
            this.ucCommandDataView1.Name = "ucCommandDataView1";
            this.ucCommandDataView1.Size = new System.Drawing.Size(841, 52);
            this.ucCommandDataView1.TabIndex = 60;
            // 
            // ucDevPio1
            // 
            this.ucDevPio1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucDevPio1.Location = new System.Drawing.Point(1397, 4);
            this.ucDevPio1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ucDevPio1.Name = "ucDevPio1";
            this.ucDevPio1.Size = new System.Drawing.Size(352, 293);
            this.ucDevPio1.TabIndex = 59;
            // 
            // ucDevGripperPio1
            // 
            this.ucDevGripperPio1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucDevGripperPio1.Location = new System.Drawing.Point(1397, 307);
            this.ucDevGripperPio1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ucDevGripperPio1.Name = "ucDevGripperPio1";
            this.ucDevGripperPio1.Size = new System.Drawing.Size(352, 293);
            this.ucDevGripperPio1.TabIndex = 58;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(157, 312);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 20);
            this.label14.TabIndex = 52;
            this.label14.Text = "Velocity";
            // 
            // ssvMasterVelocity
            // 
            this.ssvMasterVelocity.AxisTag = null;
            this.ssvMasterVelocity.BackColor = System.Drawing.Color.Transparent;
            this.ssvMasterVelocity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ssvMasterVelocity.ColorOfBox = System.Drawing.Color.White;
            this.ssvMasterVelocity.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvMasterVelocity.DecimalPoint = 2;
            this.ssvMasterVelocity.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvMasterVelocity.Location = new System.Drawing.Point(22, 229);
            this.ssvMasterVelocity.Name = "ssvMasterVelocity";
            this.ssvMasterVelocity.Size = new System.Drawing.Size(86, 36);
            this.ssvMasterVelocity.TabIndex = 51;
            this.ssvMasterVelocity.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvMasterVelocity.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvMasterVelocity.TextValue = "0.00";
            this.ssvMasterVelocity.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Velocity;
            // 
            // lbMasterTorque
            // 
            this.lbMasterTorque.AutoSize = true;
            this.lbMasterTorque.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMasterTorque.ForeColor = System.Drawing.Color.White;
            this.lbMasterTorque.Location = new System.Drawing.Point(158, 545);
            this.lbMasterTorque.Name = "lbMasterTorque";
            this.lbMasterTorque.Size = new System.Drawing.Size(57, 20);
            this.lbMasterTorque.TabIndex = 50;
            this.lbMasterTorque.Text = "Torque";
            // 
            // ssvMasterTorque
            // 
            this.ssvMasterTorque.AxisTag = null;
            this.ssvMasterTorque.BackColor = System.Drawing.Color.Transparent;
            this.ssvMasterTorque.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ssvMasterTorque.ColorOfBox = System.Drawing.Color.White;
            this.ssvMasterTorque.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvMasterTorque.DecimalPoint = 2;
            this.ssvMasterTorque.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvMasterTorque.Location = new System.Drawing.Point(22, 463);
            this.ssvMasterTorque.Name = "ssvMasterTorque";
            this.ssvMasterTorque.Size = new System.Drawing.Size(86, 36);
            this.ssvMasterTorque.TabIndex = 49;
            this.ssvMasterTorque.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvMasterTorque.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvMasterTorque.TextValue = "0.00";
            this.ssvMasterTorque.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Torque;
            // 
            // uamMasterTorque
            // 
            this.uamMasterTorque.AxisTag = null;
            this.uamMasterTorque.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.uamMasterTorque.BodyColor = System.Drawing.Color.Transparent;
            this.uamMasterTorque.DecimalPoint = 2;
            this.uamMasterTorque.ForeColor = System.Drawing.Color.Black;
            this.uamMasterTorque.Location = new System.Drawing.Point(108, 396);
            this.uamMasterTorque.MaxValue = 300D;
            this.uamMasterTorque.MinValue = -300D;
            this.uamMasterTorque.Name = "uamMasterTorque";
            this.uamMasterTorque.Renderer = null;
            this.uamMasterTorque.Size = new System.Drawing.Size(180, 180);
            this.uamMasterTorque.TabIndex = 48;
            this.uamMasterTorque.Text = "Torque";
            this.uamMasterTorque.TypeOfValue = Sineva.VHL.Device.uiAnalogMeter.ValueType.Torque;
            this.uamMasterTorque.Value = 0D;
            // 
            // uamMasterVelocity
            // 
            this.uamMasterVelocity.AxisTag = null;
            this.uamMasterVelocity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.uamMasterVelocity.BodyColor = System.Drawing.Color.Transparent;
            this.uamMasterVelocity.DecimalPoint = 2;
            this.uamMasterVelocity.ForeColor = System.Drawing.Color.Black;
            this.uamMasterVelocity.Location = new System.Drawing.Point(108, 163);
            this.uamMasterVelocity.MaxValue = 5000D;
            this.uamMasterVelocity.MinValue = 0D;
            this.uamMasterVelocity.Name = "uamMasterVelocity";
            this.uamMasterVelocity.NeedleColor = System.Drawing.Color.Crimson;
            this.uamMasterVelocity.Renderer = null;
            this.uamMasterVelocity.Size = new System.Drawing.Size(180, 180);
            this.uamMasterVelocity.TabIndex = 9;
            this.uamMasterVelocity.Text = "Velocity";
            this.uamMasterVelocity.TypeOfValue = Sineva.VHL.Device.uiAnalogMeter.ValueType.Velocity;
            this.uamMasterVelocity.Value = 0D;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pbFOUP);
            this.panel5.Controls.Add(this.label16);
            this.panel5.Controls.Add(this.dsdRearSteerStatus);
            this.panel5.Controls.Add(this.label15);
            this.panel5.Controls.Add(this.dsdFrontSteerStatus);
            this.panel5.Controls.Add(this.pbRearAntiDrop);
            this.panel5.Controls.Add(this.pbFrontAntiDrop);
            this.panel5.Controls.Add(this.ucOBSStatusLower);
            this.panel5.Controls.Add(this.ucOBSStatusUp);
            this.panel5.Controls.Add(this.lbCleaner);
            this.panel5.Controls.Add(this.pbVHL);
            this.panel5.Controls.Add(this.ucOBSStatusPBS);
            this.panel5.Location = new System.Drawing.Point(300, 122);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(555, 485);
            this.panel5.TabIndex = 57;
            // 
            // pbFOUP
            // 
            this.pbFOUP.Image = global::Sineva.VHL.GUI.Properties.Resources.FOUP;
            this.pbFOUP.Location = new System.Drawing.Point(215, 251);
            this.pbFOUP.Name = "pbFOUP";
            this.pbFOUP.Size = new System.Drawing.Size(148, 109);
            this.pbFOUP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFOUP.TabIndex = 70;
            this.pbFOUP.TabStop = false;
            this.pbFOUP.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(420, 13);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 20);
            this.label16.TabIndex = 66;
            this.label16.Text = ": RearSteer";
            // 
            // dsdRearSteerStatus
            // 
            this.dsdRearSteerStatus.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdRearSteerStatus.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdRearSteerStatus.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdRearSteerStatus.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.Steer;
            this.dsdRearSteerStatus.FirstStateIoTag = null;
            this.dsdRearSteerStatus.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdRearSteerStatus.ForeColor = System.Drawing.Color.White;
            this.dsdRearSteerStatus.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdRearSteerStatus.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdRearSteerStatus.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdRearSteerStatus.Location = new System.Drawing.Point(318, 6);
            this.dsdRearSteerStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.dsdRearSteerStatus.Name = "dsdRearSteerStatus";
            this.dsdRearSteerStatus.SecondStateIoTag = null;
            this.dsdRearSteerStatus.Size = new System.Drawing.Size(96, 31);
            this.dsdRearSteerStatus.TabIndex = 65;
            this.dsdRearSteerStatus.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdRearSteerStatus.TextValue = "None";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(54, 13);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 20);
            this.label15.TabIndex = 64;
            this.label15.Text = "FrontSteer:";
            // 
            // dsdFrontSteerStatus
            // 
            this.dsdFrontSteerStatus.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdFrontSteerStatus.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdFrontSteerStatus.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdFrontSteerStatus.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.Steer;
            this.dsdFrontSteerStatus.FirstStateIoTag = null;
            this.dsdFrontSteerStatus.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdFrontSteerStatus.ForeColor = System.Drawing.Color.White;
            this.dsdFrontSteerStatus.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdFrontSteerStatus.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdFrontSteerStatus.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdFrontSteerStatus.Location = new System.Drawing.Point(141, 6);
            this.dsdFrontSteerStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dsdFrontSteerStatus.Name = "dsdFrontSteerStatus";
            this.dsdFrontSteerStatus.SecondStateIoTag = null;
            this.dsdFrontSteerStatus.Size = new System.Drawing.Size(96, 31);
            this.dsdFrontSteerStatus.TabIndex = 63;
            this.dsdFrontSteerStatus.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdFrontSteerStatus.TextValue = "None";
            // 
            // pbRearAntiDrop
            // 
            this.pbRearAntiDrop.Image = global::Sineva.VHL.GUI.Properties.Resources.RearAntiDrop_Lock;
            this.pbRearAntiDrop.Location = new System.Drawing.Point(337, 367);
            this.pbRearAntiDrop.Name = "pbRearAntiDrop";
            this.pbRearAntiDrop.Size = new System.Drawing.Size(60, 30);
            this.pbRearAntiDrop.TabIndex = 67;
            this.pbRearAntiDrop.TabStop = false;
            // 
            // pbFrontAntiDrop
            // 
            this.pbFrontAntiDrop.Image = global::Sineva.VHL.GUI.Properties.Resources.FrontAntiDrop_Lock;
            this.pbFrontAntiDrop.Location = new System.Drawing.Point(183, 365);
            this.pbFrontAntiDrop.Name = "pbFrontAntiDrop";
            this.pbFrontAntiDrop.Size = new System.Drawing.Size(60, 30);
            this.pbFrontAntiDrop.TabIndex = 68;
            this.pbFrontAntiDrop.TabStop = false;
            // 
            // ucOBSStatusLower
            // 
            this.ucOBSStatusLower.AreaDirection = Sineva.VHL.Device.ucDevOBSStatus.MeterAreaDirection.Left;
            this.ucOBSStatusLower.AreaNo = ((uint)(1u));
            this.ucOBSStatusLower.DevOBS = null;
            this.ucOBSStatusLower.DevSoS = null;
            this.ucOBSStatusLower.Level1Color = System.Drawing.Color.Yellow;
            this.ucOBSStatusLower.Level2Color = System.Drawing.Color.Orange;
            this.ucOBSStatusLower.Level3Color = System.Drawing.Color.Red;
            this.ucOBSStatusLower.Location = new System.Drawing.Point(7, 332);
            this.ucOBSStatusLower.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ucOBSStatusLower.Name = "ucOBSStatusLower";
            this.ucOBSStatusLower.SensorType = Sineva.VHL.Device.ucDevOBSStatus.FrontSensorType.OBS;
            this.ucOBSStatusLower.Size = new System.Drawing.Size(54, 108);
            this.ucOBSStatusLower.TabIndex = 59;
            this.ucOBSStatusLower.TextColor = System.Drawing.Color.White;
            this.ucOBSStatusLower.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // ucOBSStatusUp
            // 
            this.ucOBSStatusUp.AreaDirection = Sineva.VHL.Device.ucDevOBSStatus.MeterAreaDirection.Left;
            this.ucOBSStatusUp.AreaNo = ((uint)(0u));
            this.ucOBSStatusUp.DevOBS = null;
            this.ucOBSStatusUp.DevSoS = null;
            this.ucOBSStatusUp.Level1Color = System.Drawing.Color.Yellow;
            this.ucOBSStatusUp.Level2Color = System.Drawing.Color.Orange;
            this.ucOBSStatusUp.Level3Color = System.Drawing.Color.Red;
            this.ucOBSStatusUp.Location = new System.Drawing.Point(7, 107);
            this.ucOBSStatusUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ucOBSStatusUp.Name = "ucOBSStatusUp";
            this.ucOBSStatusUp.SensorType = Sineva.VHL.Device.ucDevOBSStatus.FrontSensorType.SOS;
            this.ucOBSStatusUp.Size = new System.Drawing.Size(54, 110);
            this.ucOBSStatusUp.TabIndex = 58;
            this.ucOBSStatusUp.TextColor = System.Drawing.Color.White;
            this.ucOBSStatusUp.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // lbCleaner
            // 
            this.lbCleaner.BackColor = System.Drawing.Color.Gainsboro;
            this.lbCleaner.Font = new System.Drawing.Font("Arial", 12F);
            this.lbCleaner.Location = new System.Drawing.Point(184, 313);
            this.lbCleaner.Name = "lbCleaner";
            this.lbCleaner.Size = new System.Drawing.Size(209, 78);
            this.lbCleaner.TabIndex = 71;
            this.lbCleaner.Text = "Cleaning";
            this.lbCleaner.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbVHL
            // 
            this.pbVHL.Image = global::Sineva.VHL.GUI.Properties.Resources.VHL_NoFOUP;
            this.pbVHL.Location = new System.Drawing.Point(26, 14);
            this.pbVHL.Name = "pbVHL";
            this.pbVHL.Size = new System.Drawing.Size(500, 393);
            this.pbVHL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbVHL.TabIndex = 69;
            this.pbVHL.TabStop = false;
            // 
            // ucOBSStatusPBS
            // 
            this.ucOBSStatusPBS.AreaDirection = Sineva.VHL.Device.ucDevOBSStatus.MeterAreaDirection.Bottom;
            this.ucOBSStatusPBS.AreaNo = ((uint)(2u));
            this.ucOBSStatusPBS.DevOBS = null;
            this.ucOBSStatusPBS.DevSoS = null;
            this.ucOBSStatusPBS.Level1Color = System.Drawing.Color.Yellow;
            this.ucOBSStatusPBS.Level2Color = System.Drawing.Color.Orange;
            this.ucOBSStatusPBS.Level3Color = System.Drawing.Color.Red;
            this.ucOBSStatusPBS.Location = new System.Drawing.Point(402, 397);
            this.ucOBSStatusPBS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ucOBSStatusPBS.Name = "ucOBSStatusPBS";
            this.ucOBSStatusPBS.SensorType = Sineva.VHL.Device.ucDevOBSStatus.FrontSensorType.OBS;
            this.ucOBSStatusPBS.Size = new System.Drawing.Size(82, 81);
            this.ucOBSStatusPBS.TabIndex = 62;
            this.ucOBSStatusPBS.TextColor = System.Drawing.Color.White;
            this.ucOBSStatusPBS.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // tabPageManual
            // 
            this.tabPageManual.BackColor = System.Drawing.Color.Transparent;
            this.tabPageManual.Controls.Add(this.ucManualTeachingView1);
            this.tabPageManual.Controls.Add(this.DevicePanel);
            this.tabPageManual.ImageIndex = 6;
            this.tabPageManual.Location = new System.Drawing.Point(4, 26);
            this.tabPageManual.Name = "tabPageManual";
            this.tabPageManual.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageManual.Size = new System.Drawing.Size(1781, 672);
            this.tabPageManual.TabIndex = 1;
            this.tabPageManual.Tag = "Manual";
            this.tabPageManual.Text = "Manual";
            // 
            // ucManualTeachingView1
            // 
            this.ucManualTeachingView1.BackColor = System.Drawing.SystemColors.Control;
            this.ucManualTeachingView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucManualTeachingView1.Location = new System.Drawing.Point(531, 3);
            this.ucManualTeachingView1.Name = "ucManualTeachingView1";
            this.ucManualTeachingView1.Size = new System.Drawing.Size(1247, 666);
            this.ucManualTeachingView1.TabIndex = 86;
            // 
            // DevicePanel
            // 
            this.DevicePanel.Controls.Add(this.tableLayoutPanel4);
            this.DevicePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.DevicePanel.Location = new System.Drawing.Point(3, 3);
            this.DevicePanel.Name = "DevicePanel";
            this.DevicePanel.Size = new System.Drawing.Size(528, 666);
            this.DevicePanel.TabIndex = 85;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.steerPanel, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.piotestPanel, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.autoteachingPanel, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.gripperPanel, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.rfidPanel, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.panel6, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 6;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(528, 666);
            this.tableLayoutPanel4.TabIndex = 89;
            // 
            // steerPanel
            // 
            this.steerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.steerPanel.Controls.Add(this.panel1);
            this.steerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.steerPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.steerPanel.FriendControl = null;
            this.steerPanel.Location = new System.Drawing.Point(3, 3);
            this.steerPanel.Name = "steerPanel";
            this.steerPanel.Size = new System.Drawing.Size(522, 124);
            this.steerPanel.TabIndex = 83;
            this.steerPanel.Title = "Steer";
            this.steerPanel.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.steerPanel.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.steerPanel.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.steerPanel.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ucSteerActionView1);
            this.panel1.Location = new System.Drawing.Point(3, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 101);
            this.panel1.TabIndex = 84;
            // 
            // ucSteerActionView1
            // 
            this.ucSteerActionView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucSteerActionView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSteerActionView1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.ucSteerActionView1.Location = new System.Drawing.Point(0, 0);
            this.ucSteerActionView1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ucSteerActionView1.Name = "ucSteerActionView1";
            this.ucSteerActionView1.Size = new System.Drawing.Size(511, 101);
            this.ucSteerActionView1.TabIndex = 83;
            // 
            // piotestPanel
            // 
            this.piotestPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.piotestPanel.Controls.Add(this.checkBox6);
            this.piotestPanel.Controls.Add(this.checkBox3);
            this.piotestPanel.Controls.Add(this.checkBox5);
            this.piotestPanel.Controls.Add(this.checkBox2);
            this.piotestPanel.Controls.Add(this.checkBox4);
            this.piotestPanel.Controls.Add(this.checkBox1);
            this.piotestPanel.Controls.Add(this.btnPioTest);
            this.piotestPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.piotestPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.piotestPanel.FriendControl = null;
            this.piotestPanel.Location = new System.Drawing.Point(3, 562);
            this.piotestPanel.Name = "piotestPanel";
            this.piotestPanel.Size = new System.Drawing.Size(522, 101);
            this.piotestPanel.TabIndex = 88;
            this.piotestPanel.Title = "PIO Test";
            this.piotestPanel.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.piotestPanel.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.piotestPanel.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.piotestPanel.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox6.Location = new System.Drawing.Point(393, 57);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(85, 19);
            this.checkBox6.TabIndex = 2;
            this.checkBox6.Text = "checkBox6";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.Visible = false;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox3.Location = new System.Drawing.Point(393, 32);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(85, 19);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Visible = false;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox5.Location = new System.Drawing.Point(302, 57);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(85, 19);
            this.checkBox5.TabIndex = 2;
            this.checkBox5.Text = "checkBox5";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.Visible = false;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox2.Location = new System.Drawing.Point(302, 32);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(85, 19);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox4.Location = new System.Drawing.Point(211, 57);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(85, 19);
            this.checkBox4.TabIndex = 2;
            this.checkBox4.Text = "checkBox4";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.Visible = false;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox1.Location = new System.Drawing.Point(211, 32);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(85, 19);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnPioTest
            // 
            this.btnPioTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPioTest.BgDefault = null;
            this.btnPioTest.BgDisable = null;
            this.btnPioTest.BgOver = null;
            this.btnPioTest.BgPushed = null;
            this.btnPioTest.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnPioTest.ConnectedLabel = null;
            this.btnPioTest.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnPioTest.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnPioTest.DefaultImage = null;
            this.btnPioTest.Description = null;
            this.btnPioTest.DownImage = null;
            this.btnPioTest.Location = new System.Drawing.Point(45, 32);
            this.btnPioTest.Name = "btnPioTest";
            this.btnPioTest.OverImage = null;
            this.btnPioTest.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnPioTest.Size = new System.Drawing.Size(106, 32);
            this.btnPioTest.TabIndex = 1;
            this.btnPioTest.Text = "PIO TEST";
            this.btnPioTest.UpImage = null;
            this.btnPioTest.UseOneImage = false;
            this.btnPioTest.UseVisualStyleBackColor = true;
            this.btnPioTest.Click += new System.EventHandler(this.btnPioTest_Click);
            // 
            // autoteachingPanel
            // 
            this.autoteachingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.autoteachingPanel.Controls.Add(this.lbVisionResult);
            this.autoteachingPanel.Controls.Add(this.btnAutoTeachingMonitor);
            this.autoteachingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoteachingPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.autoteachingPanel.FriendControl = null;
            this.autoteachingPanel.Location = new System.Drawing.Point(3, 457);
            this.autoteachingPanel.Name = "autoteachingPanel";
            this.autoteachingPanel.Size = new System.Drawing.Size(522, 99);
            this.autoteachingPanel.TabIndex = 87;
            this.autoteachingPanel.Title = "AutoTeaching";
            this.autoteachingPanel.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.autoteachingPanel.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.autoteachingPanel.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.autoteachingPanel.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // lbVisionResult
            // 
            this.lbVisionResult.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lbVisionResult.Location = new System.Drawing.Point(157, 37);
            this.lbVisionResult.Name = "lbVisionResult";
            this.lbVisionResult.Size = new System.Drawing.Size(326, 27);
            this.lbVisionResult.TabIndex = 2;
            this.lbVisionResult.Text = "dx,dy,dt";
            this.lbVisionResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAutoTeachingMonitor
            // 
            this.btnAutoTeachingMonitor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAutoTeachingMonitor.BgDefault = null;
            this.btnAutoTeachingMonitor.BgDisable = null;
            this.btnAutoTeachingMonitor.BgOver = null;
            this.btnAutoTeachingMonitor.BgPushed = null;
            this.btnAutoTeachingMonitor.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnAutoTeachingMonitor.ConnectedLabel = null;
            this.btnAutoTeachingMonitor.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnAutoTeachingMonitor.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnAutoTeachingMonitor.DefaultImage = null;
            this.btnAutoTeachingMonitor.Description = null;
            this.btnAutoTeachingMonitor.DownImage = null;
            this.btnAutoTeachingMonitor.Location = new System.Drawing.Point(45, 32);
            this.btnAutoTeachingMonitor.Name = "btnAutoTeachingMonitor";
            this.btnAutoTeachingMonitor.OverImage = null;
            this.btnAutoTeachingMonitor.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnAutoTeachingMonitor.Size = new System.Drawing.Size(106, 32);
            this.btnAutoTeachingMonitor.TabIndex = 1;
            this.btnAutoTeachingMonitor.Text = "Vision Monitor";
            this.btnAutoTeachingMonitor.UpImage = null;
            this.btnAutoTeachingMonitor.UseOneImage = false;
            this.btnAutoTeachingMonitor.UseVisualStyleBackColor = true;
            this.btnAutoTeachingMonitor.Click += new System.EventHandler(this.btnAutoTeachingMonitor_Click);
            // 
            // gripperPanel
            // 
            this.gripperPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gripperPanel.Controls.Add(this.panel4);
            this.gripperPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gripperPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.gripperPanel.FriendControl = null;
            this.gripperPanel.Location = new System.Drawing.Point(3, 263);
            this.gripperPanel.Name = "gripperPanel";
            this.gripperPanel.Size = new System.Drawing.Size(522, 83);
            this.gripperPanel.TabIndex = 85;
            this.gripperPanel.Title = "Gripper";
            this.gripperPanel.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gripperPanel.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.gripperPanel.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.gripperPanel.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.ucGripperActionView1);
            this.panel4.Location = new System.Drawing.Point(2, 31);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(512, 47);
            this.panel4.TabIndex = 2;
            // 
            // ucGripperActionView1
            // 
            this.ucGripperActionView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucGripperActionView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGripperActionView1.Location = new System.Drawing.Point(0, 0);
            this.ucGripperActionView1.Margin = new System.Windows.Forms.Padding(4);
            this.ucGripperActionView1.Name = "ucGripperActionView1";
            this.ucGripperActionView1.Size = new System.Drawing.Size(512, 47);
            this.ucGripperActionView1.TabIndex = 1;
            // 
            // rfidPanel
            // 
            this.rfidPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rfidPanel.Controls.Add(this.ucRFIDTag1);
            this.rfidPanel.Controls.Add(this.btnRFIDRead);
            this.rfidPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rfidPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.rfidPanel.FriendControl = null;
            this.rfidPanel.Location = new System.Drawing.Point(3, 352);
            this.rfidPanel.Name = "rfidPanel";
            this.rfidPanel.Size = new System.Drawing.Size(522, 99);
            this.rfidPanel.TabIndex = 86;
            this.rfidPanel.Title = "RFID";
            this.rfidPanel.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rfidPanel.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.rfidPanel.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rfidPanel.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucRFIDTag1
            // 
            this.ucRFIDTag1.Location = new System.Drawing.Point(173, 32);
            this.ucRFIDTag1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucRFIDTag1.Name = "ucRFIDTag1";
            this.ucRFIDTag1.Size = new System.Drawing.Size(282, 32);
            this.ucRFIDTag1.TabIndex = 2;
            // 
            // btnRFIDRead
            // 
            this.btnRFIDRead.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRFIDRead.BgDefault = null;
            this.btnRFIDRead.BgDisable = null;
            this.btnRFIDRead.BgOver = null;
            this.btnRFIDRead.BgPushed = null;
            this.btnRFIDRead.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRFIDRead.ConnectedLabel = null;
            this.btnRFIDRead.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRFIDRead.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRFIDRead.DefaultImage = null;
            this.btnRFIDRead.Description = null;
            this.btnRFIDRead.DownImage = null;
            this.btnRFIDRead.Location = new System.Drawing.Point(45, 32);
            this.btnRFIDRead.Name = "btnRFIDRead";
            this.btnRFIDRead.OverImage = null;
            this.btnRFIDRead.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnRFIDRead.Size = new System.Drawing.Size(106, 32);
            this.btnRFIDRead.TabIndex = 1;
            this.btnRFIDRead.Text = "Read";
            this.btnRFIDRead.UpImage = null;
            this.btnRFIDRead.UseOneImage = false;
            this.btnRFIDRead.UseVisualStyleBackColor = true;
            this.btnRFIDRead.Click += new System.EventHandler(this.btnRFIDRead_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.antiDropPanel);
            this.panel6.Controls.Add(this.ucDevCleaner1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 133);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(522, 124);
            this.panel6.TabIndex = 89;
            // 
            // antiDropPanel
            // 
            this.antiDropPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.antiDropPanel.Controls.Add(this.panel3);
            this.antiDropPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.antiDropPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.antiDropPanel.FriendControl = null;
            this.antiDropPanel.Location = new System.Drawing.Point(0, 0);
            this.antiDropPanel.Name = "antiDropPanel";
            this.antiDropPanel.Size = new System.Drawing.Size(522, 124);
            this.antiDropPanel.TabIndex = 84;
            this.antiDropPanel.Title = "Anti-Drop";
            this.antiDropPanel.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.antiDropPanel.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.antiDropPanel.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.antiDropPanel.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ucDevAntiDropAction1);
            this.panel3.Location = new System.Drawing.Point(4, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(509, 96);
            this.panel3.TabIndex = 2;
            // 
            // ucDevAntiDropAction1
            // 
            this.ucDevAntiDropAction1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucDevAntiDropAction1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucDevAntiDropAction1.Location = new System.Drawing.Point(0, 0);
            this.ucDevAntiDropAction1.Name = "ucDevAntiDropAction1";
            this.ucDevAntiDropAction1.Size = new System.Drawing.Size(509, 96);
            this.ucDevAntiDropAction1.TabIndex = 1;
            // 
            // ucDevCleaner1
            // 
            this.ucDevCleaner1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucDevCleaner1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucDevCleaner1.Location = new System.Drawing.Point(2, 4);
            this.ucDevCleaner1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ucDevCleaner1.Name = "ucDevCleaner1";
            this.ucDevCleaner1.Size = new System.Drawing.Size(517, 46);
            this.ucDevCleaner1.TabIndex = 85;
            // 
            // tabPageOCS
            // 
            this.tabPageOCS.BackColor = System.Drawing.Color.Transparent;
            this.tabPageOCS.Controls.Add(this.ucOcsCommunication1);
            this.tabPageOCS.ImageIndex = 7;
            this.tabPageOCS.Location = new System.Drawing.Point(4, 26);
            this.tabPageOCS.Name = "tabPageOCS";
            this.tabPageOCS.Size = new System.Drawing.Size(1781, 672);
            this.tabPageOCS.TabIndex = 2;
            this.tabPageOCS.Tag = "OCS";
            this.tabPageOCS.Text = "OCS";
            // 
            // ucOcsCommunication1
            // 
            this.ucOcsCommunication1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucOcsCommunication1.Location = new System.Drawing.Point(0, 0);
            this.ucOcsCommunication1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucOcsCommunication1.Name = "ucOcsCommunication1";
            this.ucOcsCommunication1.Size = new System.Drawing.Size(1781, 672);
            this.ucOcsCommunication1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "battery.png");
            this.imageList1.Images.SetKeyName(1, "book_open.png");
            this.imageList1.Images.SetKeyName(2, "brush3.png");
            this.imageList1.Images.SetKeyName(3, "calculator.png");
            this.imageList1.Images.SetKeyName(4, "cd_music.png");
            this.imageList1.Images.SetKeyName(5, "Close");
            this.imageList1.Images.SetKeyName(6, "google_favicon.png");
            this.imageList1.Images.SetKeyName(7, "WSS.ICO");
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.alarmCurrentView1, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.logList1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 711);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1789, 180);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // alarmCurrentView1
            // 
            this.alarmCurrentView1.ColumnHeadersVisible = true;
            this.alarmCurrentView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmCurrentView1.Location = new System.Drawing.Point(897, 23);
            this.alarmCurrentView1.Name = "alarmCurrentView1";
            this.alarmCurrentView1.Size = new System.Drawing.Size(889, 154);
            this.alarmCurrentView1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(888, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sequence Log";
            // 
            // logList1
            // 
            this.logList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logList1.FilterHide = true;
            this.logList1.Location = new System.Drawing.Point(3, 23);
            this.logList1.LogAble = true;
            this.logList1.MaxCount = 30;
            this.logList1.Name = "logList1";
            this.logList1.Size = new System.Drawing.Size(888, 154);
            this.logList1.TabIndex = 3;
            this.logList1.UseButtonLog = false;
            this.logList1.UseExceptionLog = false;
            this.logList1.UseJcsLog = false;
            this.logList1.UseManualLog = false;
            this.logList1.UseMxpLog = false;
            this.logList1.UseOcsLog = false;
            this.logList1.UseSequenceLog = true;
            this.logList1.UseSerialLog = false;
            this.logList1.UseServoAxisLog = false;
            this.logList1.UseServoLog = false;
            this.logList1.UseTactTimeLog = false;
            this.logList1.UseVisionLog = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pLifeTimeDisplay);
            this.panel2.Controls.Add(this.pAlarmReset);
            this.panel2.Controls.Add(this.pBuzzerOff);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(894, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(895, 20);
            this.panel2.TabIndex = 5;
            // 
            // pLifeTimeDisplay
            // 
            this.pLifeTimeDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pLifeTimeDisplay.Image = ((System.Drawing.Image)(resources.GetObject("pLifeTimeDisplay.Image")));
            this.pLifeTimeDisplay.InitialImage = ((System.Drawing.Image)(resources.GetObject("pLifeTimeDisplay.InitialImage")));
            this.pLifeTimeDisplay.Location = new System.Drawing.Point(800, -4);
            this.pLifeTimeDisplay.Name = "pLifeTimeDisplay";
            this.pLifeTimeDisplay.Size = new System.Drawing.Size(25, 25);
            this.pLifeTimeDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pLifeTimeDisplay.TabIndex = 3;
            this.pLifeTimeDisplay.TabStop = false;
            this.pLifeTimeDisplay.Click += new System.EventHandler(this.pLifeTimeDisplay_Click);
            // 
            // pAlarmReset
            // 
            this.pAlarmReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pAlarmReset.Image = ((System.Drawing.Image)(resources.GetObject("pAlarmReset.Image")));
            this.pAlarmReset.InitialImage = global::Sineva.VHL.GUI.Properties.Resources.IFReset;
            this.pAlarmReset.Location = new System.Drawing.Point(833, -2);
            this.pAlarmReset.Name = "pAlarmReset";
            this.pAlarmReset.Size = new System.Drawing.Size(25, 25);
            this.pAlarmReset.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pAlarmReset.TabIndex = 3;
            this.pAlarmReset.TabStop = false;
            this.pAlarmReset.Click += new System.EventHandler(this.pAlarmReset_Click);
            // 
            // pBuzzerOff
            // 
            this.pBuzzerOff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pBuzzerOff.Image = global::Sineva.VHL.GUI.Properties.Resources.buzzer;
            this.pBuzzerOff.InitialImage = global::Sineva.VHL.GUI.Properties.Resources.buzzer;
            this.pBuzzerOff.Location = new System.Drawing.Point(865, -2);
            this.pBuzzerOff.Name = "pBuzzerOff";
            this.pBuzzerOff.Size = new System.Drawing.Size(25, 25);
            this.pBuzzerOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBuzzerOff.TabIndex = 3;
            this.pBuzzerOff.TabStop = false;
            this.pBuzzerOff.Click += new System.EventHandler(this.pBuzzerOff_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Current Alarm Lists";
            // 
            // JobsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1901, 900);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "JobsForm";
            this.Text = "JobsForm";
            this.Load += new System.EventHandler(this.JobsForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tabControlJob.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabPageMain.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFOUP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRearAntiDrop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrontAntiDrop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVHL)).EndInit();
            this.tabPageManual.ResumeLayout(false);
            this.DevicePanel.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.steerPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.piotestPanel.ResumeLayout(false);
            this.piotestPanel.PerformLayout();
            this.autoteachingPanel.ResumeLayout(false);
            this.gripperPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.rfidPanel.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.antiDropPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tabPageOCS.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pLifeTimeDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pAlarmReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBuzzerOff)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelToolbar;
        private Library.FlatTabControl tabControlJob;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TabPage tabPageManual;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabPageOCS;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private Data.Alarm.AlarmCurrentView alarmCurrentView1;
        private Library.LogList logList1;
        private Sineva.VHL.Device.uiAnalogMeter uamMasterVelocity;
        private Sineva.VHL.Device.uiAnalogMeter uamMasterTorque;
        private System.Windows.Forms.Label label14;
        private Sineva.VHL.Device.ucServoStatusValue ssvMasterVelocity;
        private System.Windows.Forms.Label lbMasterTorque;
        private Sineva.VHL.Device.ucServoStatusValue ssvMasterTorque;
        private System.Windows.Forms.Panel DevicePanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pAlarmReset;
        private System.Windows.Forms.PictureBox pBuzzerOff;
        private System.Windows.Forms.Label label3;
        private Library.HeaderPanel steerPanel;
        private Sineva.VHL.Device.ucDevSteerAction ucSteerActionView1;
        private Library.HeaderPanel antiDropPanel;
        private Library.HeaderPanel gripperPanel;
        private Sineva.VHL.Device.ucDevGripperAction ucGripperActionView1;
        private ucOcsCommunication ucOcsCommunication1;
        private Library.HeaderPanel rfidPanel;
        private Library.IButton btnRFIDRead;
        private Device.ucDevAntiDropAction ucDevAntiDropAction1;
        private Device.ucManualTeachingView ucManualTeachingView1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label16;
        private Device.ucDevStatusDisplay dsdRearSteerStatus;
        private System.Windows.Forms.Label label15;
        private Device.ucDevStatusDisplay dsdFrontSteerStatus;
        private Device.ucDevOBSStatus ucOBSStatusPBS;
        private System.Windows.Forms.PictureBox pbRearAntiDrop;
        private System.Windows.Forms.PictureBox pbFrontAntiDrop;
        private Device.ucDevOBSStatus ucOBSStatusLower;
        private Device.ucDevOBSStatus ucOBSStatusUp;
        private System.Windows.Forms.PictureBox pbVHL;
        private Device.ucDevPio ucDevPio1;
        private Device.ucDevGripperPio ucDevGripperPio1;
        private ucCommandDataView ucCommandDataView1;
        private ucProcessDataView ucProcessDataView1;
        private ucMotorStateView ucMotorStateView1;
        private System.Windows.Forms.Label lbMxpOverride;
        private System.Windows.Forms.Label label2;
        private Device.Design.ucRFIDTag ucRFIDTag1;
        private System.Windows.Forms.PictureBox pbFOUP;
        private Library.HeaderPanel autoteachingPanel;
        private Library.IButton btnAutoTeachingMonitor;
        private System.Windows.Forms.Label lbVisionResult;
        private Library.HeaderPanel piotestPanel;
        private Library.IButton btnPioTest;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbJCSSystemByte;
        private System.Windows.Forms.Label lbOCSSystemByte;
        private System.Windows.Forms.Label lbCleaner;
        private System.Windows.Forms.Panel panel6;
        private Device.ucDevCleaner ucDevCleaner1;
        private System.Windows.Forms.PictureBox pLifeTimeDisplay;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}