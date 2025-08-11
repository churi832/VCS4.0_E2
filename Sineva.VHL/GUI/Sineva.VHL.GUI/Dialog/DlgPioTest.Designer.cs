namespace Sineva.VHL.GUI
{
    partial class DlgPioTest
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
            this.tbPortID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbEqp = new System.Windows.Forms.CheckBox();
            this.cbMtl1 = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbAutoDoor2 = new System.Windows.Forms.CheckBox();
            this.cbAutoDoor1 = new System.Windows.Forms.CheckBox();
            this.btnConntect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbInput2 = new System.Windows.Forms.Label();
            this.lbInput1 = new System.Windows.Forms.Label();
            this.lbInput4 = new System.Windows.Forms.Label();
            this.lbInput3 = new System.Windows.Forms.Label();
            this.lbInput5 = new System.Windows.Forms.Label();
            this.lbInput6 = new System.Windows.Forms.Label();
            this.lbInput7 = new System.Windows.Forms.Label();
            this.lbInput8 = new System.Windows.Forms.Label();
            this.lbOutput1 = new System.Windows.Forms.Label();
            this.lbOutput2 = new System.Windows.Forms.Label();
            this.lbOutput3 = new System.Windows.Forms.Label();
            this.lbOutput4 = new System.Windows.Forms.Label();
            this.lbOutput5 = new System.Windows.Forms.Label();
            this.lbOutput6 = new System.Windows.Forms.Label();
            this.lbOutput7 = new System.Windows.Forms.Label();
            this.lbOutput8 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbGo = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPortID
            // 
            this.tbPortID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPortID.Location = new System.Drawing.Point(404, 3);
            this.tbPortID.Name = "tbPortID";
            this.tbPortID.Size = new System.Drawing.Size(128, 21);
            this.tbPortID.TabIndex = 1;
            this.tbPortID.Click += new System.EventHandler(this.tbPortID_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(338, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port ID :";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.tbPortID);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbEqp);
            this.panel1.Controls.Add(this.cbMtl1);
            this.panel1.Location = new System.Drawing.Point(21, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(545, 26);
            this.panel1.TabIndex = 19;
            // 
            // cbEqp
            // 
            this.cbEqp.AutoSize = true;
            this.cbEqp.Location = new System.Drawing.Point(241, 5);
            this.cbEqp.Name = "cbEqp";
            this.cbEqp.Size = new System.Drawing.Size(49, 16);
            this.cbEqp.TabIndex = 1;
            this.cbEqp.Tag = "EQP";
            this.cbEqp.Text = "EQP";
            this.cbEqp.UseVisualStyleBackColor = true;
            this.cbEqp.CheckedChanged += new System.EventHandler(this.cbUsedPIO_CheckedChanged);
            // 
            // cbMtl1
            // 
            this.cbMtl1.AutoSize = true;
            this.cbMtl1.Location = new System.Drawing.Point(15, 6);
            this.cbMtl1.Name = "cbMtl1";
            this.cbMtl1.Size = new System.Drawing.Size(118, 16);
            this.cbMtl1.TabIndex = 0;
            this.cbMtl1.Tag = "MTL";
            this.cbMtl1.Text = "MTL#1(Node 81)";
            this.cbMtl1.UseVisualStyleBackColor = true;
            this.cbMtl1.CheckedChanged += new System.EventHandler(this.cbUsedPIO_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.cbAutoDoor2);
            this.panel2.Controls.Add(this.cbAutoDoor1);
            this.panel2.Location = new System.Drawing.Point(21, 40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(545, 26);
            this.panel2.TabIndex = 20;
            // 
            // cbAutoDoor2
            // 
            this.cbAutoDoor2.AutoSize = true;
            this.cbAutoDoor2.Location = new System.Drawing.Point(187, 6);
            this.cbAutoDoor2.Name = "cbAutoDoor2";
            this.cbAutoDoor2.Size = new System.Drawing.Size(149, 16);
            this.cbAutoDoor2.TabIndex = 0;
            this.cbAutoDoor2.Tag = "AutoDoor2";
            this.cbAutoDoor2.Text = "AutoDoor#2(Node 529)";
            this.cbAutoDoor2.UseVisualStyleBackColor = true;
            this.cbAutoDoor2.CheckedChanged += new System.EventHandler(this.cbUsedPIO_CheckedChanged);
            // 
            // cbAutoDoor1
            // 
            this.cbAutoDoor1.AutoSize = true;
            this.cbAutoDoor1.Location = new System.Drawing.Point(15, 6);
            this.cbAutoDoor1.Name = "cbAutoDoor1";
            this.cbAutoDoor1.Size = new System.Drawing.Size(149, 16);
            this.cbAutoDoor1.TabIndex = 0;
            this.cbAutoDoor1.Tag = "AutoDoor1";
            this.cbAutoDoor1.Text = "AutoDoor#1(Node 527)";
            this.cbAutoDoor1.UseVisualStyleBackColor = true;
            this.cbAutoDoor1.CheckedChanged += new System.EventHandler(this.cbUsedPIO_CheckedChanged);
            // 
            // btnConntect
            // 
            this.btnConntect.Location = new System.Drawing.Point(21, 83);
            this.btnConntect.Name = "btnConntect";
            this.btnConntect.Size = new System.Drawing.Size(215, 47);
            this.btnConntect.TabIndex = 21;
            this.btnConntect.Text = "Connect";
            this.btnConntect.UseVisualStyleBackColor = true;
            this.btnConntect.Click += new System.EventHandler(this.btnConntect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(242, 83);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(218, 47);
            this.btnDisconnect.TabIndex = 22;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lbInput2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbInput1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbInput4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbInput3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbInput5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbInput6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lbInput7, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lbInput8, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput3, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput4, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput5, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput6, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput7, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.lbOutput8, 2, 7);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(21, 145);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(545, 293);
            this.tableLayoutPanel1.TabIndex = 23;
            // 
            // lbInput2
            // 
            this.lbInput2.BackColor = System.Drawing.Color.White;
            this.lbInput2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput2.Location = new System.Drawing.Point(3, 41);
            this.lbInput2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput2.Name = "lbInput2";
            this.lbInput2.Size = new System.Drawing.Size(256, 26);
            this.lbInput2.TabIndex = 1;
            this.lbInput2.Text = "Unload Request";
            this.lbInput2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput1
            // 
            this.lbInput1.BackColor = System.Drawing.Color.White;
            this.lbInput1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput1.Location = new System.Drawing.Point(3, 5);
            this.lbInput1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput1.Name = "lbInput1";
            this.lbInput1.Size = new System.Drawing.Size(256, 26);
            this.lbInput1.TabIndex = 0;
            this.lbInput1.Text = "Load Request";
            this.lbInput1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput4
            // 
            this.lbInput4.BackColor = System.Drawing.Color.White;
            this.lbInput4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput4.Location = new System.Drawing.Point(3, 113);
            this.lbInput4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput4.Name = "lbInput4";
            this.lbInput4.Size = new System.Drawing.Size(256, 26);
            this.lbInput4.TabIndex = 1;
            this.lbInput4.Text = "Ready";
            this.lbInput4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput3
            // 
            this.lbInput3.BackColor = System.Drawing.Color.White;
            this.lbInput3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput3.Location = new System.Drawing.Point(3, 77);
            this.lbInput3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput3.Name = "lbInput3";
            this.lbInput3.Size = new System.Drawing.Size(256, 26);
            this.lbInput3.TabIndex = 1;
            this.lbInput3.Text = "NC";
            this.lbInput3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput5
            // 
            this.lbInput5.BackColor = System.Drawing.Color.White;
            this.lbInput5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput5.Location = new System.Drawing.Point(3, 149);
            this.lbInput5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput5.Name = "lbInput5";
            this.lbInput5.Size = new System.Drawing.Size(256, 26);
            this.lbInput5.TabIndex = 1;
            this.lbInput5.Text = "NC";
            this.lbInput5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput6
            // 
            this.lbInput6.BackColor = System.Drawing.Color.White;
            this.lbInput6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput6.Location = new System.Drawing.Point(3, 185);
            this.lbInput6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput6.Name = "lbInput6";
            this.lbInput6.Size = new System.Drawing.Size(256, 26);
            this.lbInput6.TabIndex = 1;
            this.lbInput6.Text = "NC";
            this.lbInput6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput7
            // 
            this.lbInput7.BackColor = System.Drawing.Color.White;
            this.lbInput7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput7.Location = new System.Drawing.Point(3, 221);
            this.lbInput7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput7.Name = "lbInput7";
            this.lbInput7.Size = new System.Drawing.Size(256, 26);
            this.lbInput7.TabIndex = 1;
            this.lbInput7.Text = "Hand Off Available";
            this.lbInput7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbInput8
            // 
            this.lbInput8.BackColor = System.Drawing.Color.White;
            this.lbInput8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbInput8.Location = new System.Drawing.Point(3, 257);
            this.lbInput8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbInput8.Name = "lbInput8";
            this.lbInput8.Size = new System.Drawing.Size(256, 31);
            this.lbInput8.TabIndex = 1;
            this.lbInput8.Text = "Emergency Stop";
            this.lbInput8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbOutput1
            // 
            this.lbOutput1.BackColor = System.Drawing.Color.White;
            this.lbOutput1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput1.Location = new System.Drawing.Point(285, 5);
            this.lbOutput1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput1.Name = "lbOutput1";
            this.lbOutput1.Size = new System.Drawing.Size(257, 26);
            this.lbOutput1.TabIndex = 1;
            this.lbOutput1.Tag = "0";
            this.lbOutput1.Text = "Valid";
            this.lbOutput1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput2
            // 
            this.lbOutput2.BackColor = System.Drawing.Color.White;
            this.lbOutput2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput2.Location = new System.Drawing.Point(285, 41);
            this.lbOutput2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput2.Name = "lbOutput2";
            this.lbOutput2.Size = new System.Drawing.Size(257, 26);
            this.lbOutput2.TabIndex = 1;
            this.lbOutput2.Tag = "1";
            this.lbOutput2.Text = "CS1";
            this.lbOutput2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput3
            // 
            this.lbOutput3.BackColor = System.Drawing.Color.White;
            this.lbOutput3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput3.Location = new System.Drawing.Point(285, 77);
            this.lbOutput3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput3.Name = "lbOutput3";
            this.lbOutput3.Size = new System.Drawing.Size(257, 26);
            this.lbOutput3.TabIndex = 1;
            this.lbOutput3.Tag = "2";
            this.lbOutput3.Text = "CS2";
            this.lbOutput3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput3.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput4
            // 
            this.lbOutput4.BackColor = System.Drawing.Color.White;
            this.lbOutput4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput4.Location = new System.Drawing.Point(285, 113);
            this.lbOutput4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput4.Name = "lbOutput4";
            this.lbOutput4.Size = new System.Drawing.Size(257, 26);
            this.lbOutput4.TabIndex = 1;
            this.lbOutput4.Tag = "3";
            this.lbOutput4.Text = "NC";
            this.lbOutput4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput4.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput5
            // 
            this.lbOutput5.BackColor = System.Drawing.Color.White;
            this.lbOutput5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput5.Location = new System.Drawing.Point(285, 149);
            this.lbOutput5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput5.Name = "lbOutput5";
            this.lbOutput5.Size = new System.Drawing.Size(257, 26);
            this.lbOutput5.TabIndex = 1;
            this.lbOutput5.Tag = "4";
            this.lbOutput5.Text = "Transfer Request";
            this.lbOutput5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput5.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput6
            // 
            this.lbOutput6.BackColor = System.Drawing.Color.White;
            this.lbOutput6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput6.Location = new System.Drawing.Point(285, 185);
            this.lbOutput6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput6.Name = "lbOutput6";
            this.lbOutput6.Size = new System.Drawing.Size(257, 26);
            this.lbOutput6.TabIndex = 1;
            this.lbOutput6.Tag = "5";
            this.lbOutput6.Text = "Busy";
            this.lbOutput6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput6.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput7
            // 
            this.lbOutput7.BackColor = System.Drawing.Color.White;
            this.lbOutput7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput7.Location = new System.Drawing.Point(285, 221);
            this.lbOutput7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput7.Name = "lbOutput7";
            this.lbOutput7.Size = new System.Drawing.Size(257, 26);
            this.lbOutput7.TabIndex = 1;
            this.lbOutput7.Tag = "6";
            this.lbOutput7.Text = "Complete";
            this.lbOutput7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput7.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // lbOutput8
            // 
            this.lbOutput8.BackColor = System.Drawing.Color.White;
            this.lbOutput8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput8.Location = new System.Drawing.Point(285, 257);
            this.lbOutput8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbOutput8.Name = "lbOutput8";
            this.lbOutput8.Size = new System.Drawing.Size(257, 31);
            this.lbOutput8.TabIndex = 1;
            this.lbOutput8.Tag = "7";
            this.lbOutput8.Text = "Continue";
            this.lbOutput8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOutput8.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbOutput_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbGo
            // 
            this.lbGo.BackColor = System.Drawing.Color.White;
            this.lbGo.Location = new System.Drawing.Point(484, 83);
            this.lbGo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lbGo.Name = "lbGo";
            this.lbGo.Size = new System.Drawing.Size(82, 47);
            this.lbGo.TabIndex = 24;
            this.lbGo.Text = "GO";
            this.lbGo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(430, 442);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(135, 39);
            this.btnClose.TabIndex = 25;
            this.btnClose.Text = "Exit";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // DlgPioTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ClientSize = new System.Drawing.Size(581, 484);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbGo);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConntect);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DlgPioTest";
            this.Text = "DlgPioTest";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox tbPortID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbEqp;
        private System.Windows.Forms.CheckBox cbMtl1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbAutoDoor2;
        private System.Windows.Forms.CheckBox cbAutoDoor1;
        private System.Windows.Forms.Button btnConntect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbInput1;
        private System.Windows.Forms.Label lbInput2;
        private System.Windows.Forms.Label lbInput4;
        private System.Windows.Forms.Label lbInput3;
        private System.Windows.Forms.Label lbInput5;
        private System.Windows.Forms.Label lbInput6;
        private System.Windows.Forms.Label lbInput7;
        private System.Windows.Forms.Label lbInput8;
        private System.Windows.Forms.Label lbOutput1;
        private System.Windows.Forms.Label lbOutput2;
        private System.Windows.Forms.Label lbOutput3;
        private System.Windows.Forms.Label lbOutput4;
        private System.Windows.Forms.Label lbOutput5;
        private System.Windows.Forms.Label lbOutput6;
        private System.Windows.Forms.Label lbOutput7;
        private System.Windows.Forms.Label lbOutput8;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbGo;
        private System.Windows.Forms.Button btnClose;
    }
}