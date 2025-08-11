namespace Sineva.VHL.GUI
{
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.tabControlSetup = new Sineva.VHL.Library.FlatTabControl();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.viewSetup1 = new Sineva.VHL.Data.Setup.ViewSetup();
            this.tabPageDevice = new System.Windows.Forms.TabPage();
            this.viewDevProperty1 = new Sineva.VHL.Device.viewDevProperty();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.tabControlSetup.SuspendLayout();
            this.tabPageConfig.SuspendLayout();
            this.tabPageDevice.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlSetup
            // 
            this.tabControlSetup.Controls.Add(this.tabPageConfig);
            this.tabControlSetup.Controls.Add(this.tabPageDevice);
            this.tabControlSetup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSetup.ImageList = this.imageList1;
            this.tabControlSetup.Location = new System.Drawing.Point(0, 0);
            this.tabControlSetup.myBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tabControlSetup.mySelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabControlSetup.myTextColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabControlSetup.Name = "tabControlSetup";
            this.tabControlSetup.SelectedIndex = 0;
            this.tabControlSetup.Size = new System.Drawing.Size(1807, 874);
            this.tabControlSetup.TabIndex = 0;
            this.tabControlSetup.Tag = "SETUP";
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.BackColor = System.Drawing.Color.Transparent;
            this.tabPageConfig.Controls.Add(this.viewSetup1);
            this.tabPageConfig.ImageIndex = 1;
            this.tabPageConfig.Location = new System.Drawing.Point(4, 26);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfig.Size = new System.Drawing.Size(1799, 844);
            this.tabPageConfig.TabIndex = 0;
            this.tabPageConfig.Tag = "SETUP";
            this.tabPageConfig.Text = "Config";
            // 
            // viewSetup1
            // 
            this.viewSetup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewSetup1.Location = new System.Drawing.Point(3, 3);
            this.viewSetup1.Name = "viewSetup1";
            this.viewSetup1.Size = new System.Drawing.Size(1793, 838);
            this.viewSetup1.TabIndex = 0;
            // 
            // tabPageDevice
            // 
            this.tabPageDevice.BackColor = System.Drawing.Color.Transparent;
            this.tabPageDevice.Controls.Add(this.viewDevProperty1);
            this.tabPageDevice.ImageIndex = 0;
            this.tabPageDevice.Location = new System.Drawing.Point(4, 26);
            this.tabPageDevice.Name = "tabPageDevice";
            this.tabPageDevice.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDevice.Size = new System.Drawing.Size(1799, 844);
            this.tabPageDevice.TabIndex = 1;
            this.tabPageDevice.Tag = "SETUP";
            this.tabPageDevice.Text = "Device";
            // 
            // viewDevProperty1
            // 
            this.viewDevProperty1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewDevProperty1.Location = new System.Drawing.Point(3, 3);
            this.viewDevProperty1.Name = "viewDevProperty1";
            this.viewDevProperty1.Size = new System.Drawing.Size(1793, 838);
            this.viewDevProperty1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "setup_device.ico");
            this.imageList1.Images.SetKeyName(1, "setup_setup.ico");
            this.imageList1.Images.SetKeyName(2, "setup_frame.ico");
            this.imageList1.Images.SetKeyName(3, "setup_mapping.ico");
            this.imageList1.Images.SetKeyName(4, "setup_stick.ico");
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.Command_bg;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1807, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(97, 874);
            this.panel1.TabIndex = 4;
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(95, 872);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(93, 6);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolStripButtonSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1904, 874);
            this.ControlBox = false;
            this.Controls.Add(this.tabControlSetup);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SetupForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SetupForm";
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.tabControlSetup.ResumeLayout(false);
            this.tabPageConfig.ResumeLayout(false);
            this.tabPageDevice.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Library.FlatTabControl tabControlSetup;
        private System.Windows.Forms.TabPage tabPageConfig;
        private System.Windows.Forms.TabPage tabPageDevice;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private Data.Setup.ViewSetup viewSetup1;
        private Device.viewDevProperty viewDevProperty1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
    }
}