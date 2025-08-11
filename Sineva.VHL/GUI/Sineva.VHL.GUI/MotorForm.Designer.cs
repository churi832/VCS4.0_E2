namespace Sineva.VHL.GUI
{
    partial class MotorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotorForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlMotor = new Sineva.VHL.Library.FlatTabControl();
            this.tabPageServo = new System.Windows.Forms.TabPage();
            this.servoControlView1 = new Sineva.VHL.Device.viewServoControl();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControlMotor.SuspendLayout();
            this.tabPageServo.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.Command_bg;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Font = new System.Drawing.Font("Arial", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.toolStripButtonSave});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(1819, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(97, 842);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(95, 6);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(95, 82);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolStripButtonSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "motor_servo.ico");
            this.imageList1.Images.SetKeyName(1, "motor_maxon.ico");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControlMotor, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1916, 842);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tabControlMotor
            // 
            this.tabControlMotor.Controls.Add(this.tabPageServo);
            this.tabControlMotor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMotor.ImageList = this.imageList1;
            this.tabControlMotor.Location = new System.Drawing.Point(3, 3);
            this.tabControlMotor.myBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tabControlMotor.mySelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabControlMotor.myTextColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabControlMotor.Name = "tabControlMotor";
            this.tabControlMotor.SelectedIndex = 0;
            this.tabControlMotor.Size = new System.Drawing.Size(1813, 836);
            this.tabControlMotor.TabIndex = 3;
            this.tabControlMotor.Tag = "Motor";
            // 
            // tabPageServo
            // 
            this.tabPageServo.BackColor = System.Drawing.Color.Transparent;
            this.tabPageServo.Controls.Add(this.servoControlView1);
            this.tabPageServo.ImageIndex = 0;
            this.tabPageServo.Location = new System.Drawing.Point(4, 26);
            this.tabPageServo.Name = "tabPageServo";
            this.tabPageServo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageServo.Size = new System.Drawing.Size(1805, 806);
            this.tabPageServo.TabIndex = 0;
            this.tabPageServo.Tag = "SERVO";
            this.tabPageServo.Text = "Motor";
            // 
            // servoControlView1
            // 
            this.servoControlView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.servoControlView1.Font = new System.Drawing.Font("Arial", 9F);
            this.servoControlView1.Location = new System.Drawing.Point(3, 3);
            this.servoControlView1.Name = "servoControlView1";
            this.servoControlView1.Size = new System.Drawing.Size(1799, 800);
            this.servoControlView1.TabIndex = 0;
            // 
            // MotorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1916, 842);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MotorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "MotorForm";
            this.Load += new System.EventHandler(this.MotorForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControlMotor.ResumeLayout(false);
            this.tabPageServo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Library.FlatTabControl tabControlMotor;
        private System.Windows.Forms.TabPage tabPageServo;
        private Device.viewServoControl servoControlView1;
    }
}