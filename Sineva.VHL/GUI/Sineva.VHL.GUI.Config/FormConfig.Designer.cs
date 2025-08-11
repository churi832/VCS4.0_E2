namespace Sineva.VHL.GUI.Config
{
    partial class FormConfig
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfig));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iOCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iOInitializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iORunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iOStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servoCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servoInitializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servoRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servoStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageAppConfig = new System.Windows.Forms.TabPage();
            this.propertyGridAppConfig = new System.Windows.Forms.PropertyGrid();
            this.tabPageIoConfig = new System.Windows.Forms.TabPage();
            this.ucConfigIo1 = new Sineva.VHL.Library.IO.ucConfigIo();
            this.tabPageServoConfig = new System.Windows.Forms.TabPage();
            this.ucConfigServo1 = new Sineva.VHL.Library.Servo.ucConfigServo();
            this.tabPageIOTest = new System.Windows.Forms.TabPage();
            this.viewIoEdit1 = new Sineva.VHL.Library.IO.ViewIoEdit();
            this.tabPageServoTest = new System.Windows.Forms.TabPage();
            this.tabPageDevices = new System.Windows.Forms.TabPage();
            this.ucConfigDevices1 = new Sineva.VHL.Device.ucConfigDevices();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.servoControlView1 = new Sineva.VHL.Device.viewServoControl();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageAppConfig.SuspendLayout();
            this.tabPageIoConfig.SuspendLayout();
            this.tabPageServoConfig.SuspendLayout();
            this.tabPageIOTest.SuspendLayout();
            this.tabPageServoTest.SuspendLayout();
            this.tabPageDevices.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.iOCheckToolStripMenuItem,
            this.servoCheckToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(994, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // iOCheckToolStripMenuItem
            // 
            this.iOCheckToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iOInitializeToolStripMenuItem,
            this.iORunToolStripMenuItem,
            this.iOStopToolStripMenuItem});
            this.iOCheckToolStripMenuItem.Name = "iOCheckToolStripMenuItem";
            this.iOCheckToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.iOCheckToolStripMenuItem.Text = "I/O Check";
            // 
            // iOInitializeToolStripMenuItem
            // 
            this.iOInitializeToolStripMenuItem.Name = "iOInitializeToolStripMenuItem";
            this.iOInitializeToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.iOInitializeToolStripMenuItem.Text = "I/O Initialize";
            this.iOInitializeToolStripMenuItem.Click += new System.EventHandler(this.iOInitializeToolStripMenuItem_Click);
            // 
            // iORunToolStripMenuItem
            // 
            this.iORunToolStripMenuItem.Name = "iORunToolStripMenuItem";
            this.iORunToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.iORunToolStripMenuItem.Text = "I/O Run";
            this.iORunToolStripMenuItem.Click += new System.EventHandler(this.iORunToolStripMenuItem_Click);
            // 
            // iOStopToolStripMenuItem
            // 
            this.iOStopToolStripMenuItem.Name = "iOStopToolStripMenuItem";
            this.iOStopToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.iOStopToolStripMenuItem.Text = "I/O Stop";
            this.iOStopToolStripMenuItem.Click += new System.EventHandler(this.iOStopToolStripMenuItem_Click);
            // 
            // servoCheckToolStripMenuItem
            // 
            this.servoCheckToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.servoInitializeToolStripMenuItem,
            this.servoRunToolStripMenuItem,
            this.servoStopToolStripMenuItem});
            this.servoCheckToolStripMenuItem.Name = "servoCheckToolStripMenuItem";
            this.servoCheckToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.servoCheckToolStripMenuItem.Text = "Servo Check";
            // 
            // servoInitializeToolStripMenuItem
            // 
            this.servoInitializeToolStripMenuItem.Name = "servoInitializeToolStripMenuItem";
            this.servoInitializeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.servoInitializeToolStripMenuItem.Text = "Servo Initialize";
            this.servoInitializeToolStripMenuItem.Click += new System.EventHandler(this.servoInitializeToolStripMenuItem_Click);
            // 
            // servoRunToolStripMenuItem
            // 
            this.servoRunToolStripMenuItem.Name = "servoRunToolStripMenuItem";
            this.servoRunToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.servoRunToolStripMenuItem.Text = "Servo Run";
            this.servoRunToolStripMenuItem.Click += new System.EventHandler(this.servoRunToolStripMenuItem_Click);
            // 
            // servoStopToolStripMenuItem
            // 
            this.servoStopToolStripMenuItem.Name = "servoStopToolStripMenuItem";
            this.servoStopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.servoStopToolStripMenuItem.Text = "Servo Stop";
            this.servoStopToolStripMenuItem.Click += new System.EventHandler(this.servoStopToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageAppConfig);
            this.tabControl.Controls.Add(this.tabPageIoConfig);
            this.tabControl.Controls.Add(this.tabPageServoConfig);
            this.tabControl.Controls.Add(this.tabPageIOTest);
            this.tabControl.Controls.Add(this.tabPageServoTest);
            this.tabControl.Controls.Add(this.tabPageDevices);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 24);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(994, 548);
            this.tabControl.TabIndex = 2;
            // 
            // tabPageAppConfig
            // 
            this.tabPageAppConfig.Controls.Add(this.propertyGridAppConfig);
            this.tabPageAppConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageAppConfig.Name = "tabPageAppConfig";
            this.tabPageAppConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAppConfig.Size = new System.Drawing.Size(986, 522);
            this.tabPageAppConfig.TabIndex = 0;
            this.tabPageAppConfig.Text = "App Config";
            this.tabPageAppConfig.UseVisualStyleBackColor = true;
            // 
            // propertyGridAppConfig
            // 
            this.propertyGridAppConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridAppConfig.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGridAppConfig.Location = new System.Drawing.Point(3, 3);
            this.propertyGridAppConfig.Name = "propertyGridAppConfig";
            this.propertyGridAppConfig.Size = new System.Drawing.Size(980, 516);
            this.propertyGridAppConfig.TabIndex = 0;
            // 
            // tabPageIoConfig
            // 
            this.tabPageIoConfig.Controls.Add(this.ucConfigIo1);
            this.tabPageIoConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageIoConfig.Name = "tabPageIoConfig";
            this.tabPageIoConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIoConfig.Size = new System.Drawing.Size(986, 522);
            this.tabPageIoConfig.TabIndex = 2;
            this.tabPageIoConfig.Text = "IO Config";
            this.tabPageIoConfig.UseVisualStyleBackColor = true;
            // 
            // ucConfigIo1
            // 
            this.ucConfigIo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucConfigIo1.Location = new System.Drawing.Point(3, 3);
            this.ucConfigIo1.Name = "ucConfigIo1";
            this.ucConfigIo1.Size = new System.Drawing.Size(980, 516);
            this.ucConfigIo1.TabIndex = 0;
            // 
            // tabPageServoConfig
            // 
            this.tabPageServoConfig.Controls.Add(this.ucConfigServo1);
            this.tabPageServoConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageServoConfig.Name = "tabPageServoConfig";
            this.tabPageServoConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageServoConfig.Size = new System.Drawing.Size(986, 522);
            this.tabPageServoConfig.TabIndex = 1;
            this.tabPageServoConfig.Text = "Servo Config";
            this.tabPageServoConfig.UseVisualStyleBackColor = true;
            // 
            // ucConfigServo1
            // 
            this.ucConfigServo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucConfigServo1.Location = new System.Drawing.Point(3, 3);
            this.ucConfigServo1.Name = "ucConfigServo1";
            this.ucConfigServo1.Size = new System.Drawing.Size(980, 516);
            this.ucConfigServo1.TabIndex = 0;
            // 
            // tabPageIOTest
            // 
            this.tabPageIOTest.Controls.Add(this.viewIoEdit1);
            this.tabPageIOTest.Location = new System.Drawing.Point(4, 22);
            this.tabPageIOTest.Name = "tabPageIOTest";
            this.tabPageIOTest.Size = new System.Drawing.Size(986, 522);
            this.tabPageIOTest.TabIndex = 4;
            this.tabPageIOTest.Text = "I/O Test";
            this.tabPageIOTest.UseVisualStyleBackColor = true;
            // 
            // viewIoEdit1
            // 
            this.viewIoEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewIoEdit1.Font = new System.Drawing.Font("Arial", 9F);
            this.viewIoEdit1.Location = new System.Drawing.Point(0, 0);
            this.viewIoEdit1.Name = "viewIoEdit1";
            this.viewIoEdit1.Size = new System.Drawing.Size(986, 522);
            this.viewIoEdit1.TabIndex = 0;
            // 
            // tabPageServoTest
            // 
            this.tabPageServoTest.Controls.Add(this.servoControlView1);
            this.tabPageServoTest.Location = new System.Drawing.Point(4, 22);
            this.tabPageServoTest.Name = "tabPageServoTest";
            this.tabPageServoTest.Size = new System.Drawing.Size(986, 522);
            this.tabPageServoTest.TabIndex = 3;
            this.tabPageServoTest.Text = "Servo Test";
            this.tabPageServoTest.UseVisualStyleBackColor = true;
            // 
            // tabPageDevices
            // 
            this.tabPageDevices.Controls.Add(this.ucConfigDevices1);
            this.tabPageDevices.Location = new System.Drawing.Point(4, 22);
            this.tabPageDevices.Name = "tabPageDevices";
            this.tabPageDevices.Size = new System.Drawing.Size(986, 522);
            this.tabPageDevices.TabIndex = 5;
            this.tabPageDevices.Text = "Devices";
            this.tabPageDevices.UseVisualStyleBackColor = true;
            // 
            // ucConfigDevices1
            // 
            this.ucConfigDevices1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucConfigDevices1.Font = new System.Drawing.Font("Arial", 9F);
            this.ucConfigDevices1.Location = new System.Drawing.Point(0, 0);
            this.ucConfigDevices1.Name = "ucConfigDevices1";
            this.ucConfigDevices1.Size = new System.Drawing.Size(986, 522);
            this.ucConfigDevices1.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // servoControlView1
            // 
            this.servoControlView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.servoControlView1.Font = new System.Drawing.Font("Arial", 9F);
            this.servoControlView1.Location = new System.Drawing.Point(0, 0);
            this.servoControlView1.Name = "servoControlView1";
            this.servoControlView1.Size = new System.Drawing.Size(986, 522);
            this.servoControlView1.TabIndex = 0;
            // 
            // FormConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(994, 572);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormConfig";
            this.Text = "Config";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageAppConfig.ResumeLayout(false);
            this.tabPageIoConfig.ResumeLayout(false);
            this.tabPageServoConfig.ResumeLayout(false);
            this.tabPageIOTest.ResumeLayout(false);
            this.tabPageServoTest.ResumeLayout(false);
            this.tabPageDevices.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageAppConfig;
        private System.Windows.Forms.TabPage tabPageServoConfig;
        private System.Windows.Forms.TabPage tabPageIoConfig;
        private System.Windows.Forms.PropertyGrid propertyGridAppConfig;
        private Library.Servo.ucConfigServo ucConfigServo1;
        private Library.IO.ucConfigIo ucConfigIo1;
        private System.Windows.Forms.ToolStripMenuItem iOCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iORunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iOStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servoCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servoRunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servoStopToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageServoTest;
        private System.Windows.Forms.TabPage tabPageIOTest;
        private Library.IO.ViewIoEdit viewIoEdit1;
        private System.Windows.Forms.ToolStripMenuItem iOInitializeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servoInitializeToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabPageDevices;
        private Device.ucConfigDevices ucConfigDevices1;
        private Device.viewServoControl servoControlView1;
    }
}

