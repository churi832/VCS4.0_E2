namespace Sineva.VHL.GUI
{
    partial class AlarmForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmForm));
            this.tabControlAlarm = new Sineva.VHL.Library.FlatTabControl();
            this.tabPageList = new System.Windows.Forms.TabPage();
            this.tabPageHistory = new System.Windows.Forms.TabPage();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCurAlarmSave = new Sineva.VHL.Library.IButton();
            this.btnSave = new Sineva.VHL.Library.IButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.alarmListView1 = new Sineva.VHL.Data.Alarm.AlarmListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.alarmCurrentView1 = new Sineva.VHL.Data.Alarm.AlarmCurrentView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.alarmHistoryView1 = new Sineva.VHL.Data.Alarm.AlarmHistoryView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlAlarm.SuspendLayout();
            this.tabPageList.SuspendLayout();
            this.tabPageHistory.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlAlarm
            // 
            this.tabControlAlarm.Controls.Add(this.tabPageList);
            this.tabControlAlarm.Controls.Add(this.tabPageHistory);
            this.tabControlAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlAlarm.ImageList = this.imageList1;
            this.tabControlAlarm.Location = new System.Drawing.Point(0, 0);
            this.tabControlAlarm.myBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tabControlAlarm.mySelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabControlAlarm.myTextColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabControlAlarm.Name = "tabControlAlarm";
            this.tabControlAlarm.SelectedIndex = 0;
            this.tabControlAlarm.Size = new System.Drawing.Size(870, 431);
            this.tabControlAlarm.TabIndex = 0;
            this.tabControlAlarm.Tag = "Alarm";
            // 
            // tabPageList
            // 
            this.tabPageList.BackColor = System.Drawing.Color.Transparent;
            this.tabPageList.Controls.Add(this.tableLayoutPanel1);
            this.tabPageList.ImageIndex = 0;
            this.tabPageList.Location = new System.Drawing.Point(4, 26);
            this.tabPageList.Name = "tabPageList";
            this.tabPageList.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageList.Size = new System.Drawing.Size(862, 401);
            this.tabPageList.TabIndex = 0;
            this.tabPageList.Tag = "LIST";
            this.tabPageList.Text = "List";
            // 
            // tabPageHistory
            // 
            this.tabPageHistory.BackColor = System.Drawing.Color.Transparent;
            this.tabPageHistory.Controls.Add(this.tableLayoutPanel3);
            this.tabPageHistory.ImageIndex = 1;
            this.tabPageHistory.Location = new System.Drawing.Point(4, 26);
            this.tabPageHistory.Name = "tabPageHistory";
            this.tabPageHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHistory.Size = new System.Drawing.Size(862, 401);
            this.tabPageHistory.TabIndex = 1;
            this.tabPageHistory.Tag = "HISTORY";
            this.tabPageHistory.Text = "Histroy";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "alarm_List.ico");
            this.imageList1.Images.SetKeyName(1, "alarm_history.ico");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(856, 395);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnCurAlarmSave
            // 
            this.btnCurAlarmSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCurAlarmSave.BackgroundImage")));
            this.btnCurAlarmSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCurAlarmSave.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnCurAlarmSave.BgDefault")));
            this.btnCurAlarmSave.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnCurAlarmSave.BgDisable")));
            this.btnCurAlarmSave.BgOver = ((System.Drawing.Image)(resources.GetObject("btnCurAlarmSave.BgOver")));
            this.btnCurAlarmSave.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnCurAlarmSave.BgPushed")));
            this.btnCurAlarmSave.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnCurAlarmSave.ConnectedLabel = null;
            this.btnCurAlarmSave.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnCurAlarmSave.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnCurAlarmSave.DefaultImage = null;
            this.btnCurAlarmSave.Description = "Current Alarm Save";
            this.btnCurAlarmSave.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCurAlarmSave.DownImage = null;
            this.btnCurAlarmSave.Location = new System.Drawing.Point(0, 0);
            this.btnCurAlarmSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnCurAlarmSave.Name = "btnCurAlarmSave";
            this.btnCurAlarmSave.OverImage = ((System.Drawing.Image)(resources.GetObject("btnCurAlarmSave.OverImage")));
            this.btnCurAlarmSave.Size = new System.Drawing.Size(31, 30);
            this.btnCurAlarmSave.TabIndex = 0;
            this.btnCurAlarmSave.UpImage = null;
            this.btnCurAlarmSave.UseOneImage = false;
            this.btnCurAlarmSave.UseVisualStyleBackColor = true;
            this.btnCurAlarmSave.Click += new System.EventHandler(this.btnCurAlarmSave_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSave.BackgroundImage")));
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnSave.BgDefault")));
            this.btnSave.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnSave.BgDisable")));
            this.btnSave.BgOver = ((System.Drawing.Image)(resources.GetObject("btnSave.BgOver")));
            this.btnSave.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnSave.BgPushed")));
            this.btnSave.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnSave.ConnectedLabel = null;
            this.btnSave.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnSave.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnSave.DefaultImage = null;
            this.btnSave.Description = "Alarm List Save";
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.DownImage = null;
            this.btnSave.Location = new System.Drawing.Point(785, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.OverImage = ((System.Drawing.Image)(resources.GetObject("btnSave.OverImage")));
            this.btnSave.Size = new System.Drawing.Size(31, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.UpImage = null;
            this.btnSave.UseOneImage = false;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnCurAlarmSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(20, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(816, 30);
            this.panel1.TabIndex = 2;
            // 
            // alarmListView1
            // 
            this.alarmListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmListView1.Location = new System.Drawing.Point(0, 0);
            this.alarmListView1.Name = "alarmListView1";
            this.alarmListView1.Size = new System.Drawing.Size(606, 339);
            this.alarmListView1.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(23, 33);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.alarmListView1);
            this.splitContainer1.Size = new System.Drawing.Size(810, 339);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 4;
            // 
            // alarmCurrentView1
            // 
            this.alarmCurrentView1.ColumnHeadersVisible = true;
            this.alarmCurrentView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmCurrentView1.Location = new System.Drawing.Point(3, 35);
            this.alarmCurrentView1.Name = "alarmCurrentView1";
            this.alarmCurrentView1.Size = new System.Drawing.Size(194, 301);
            this.alarmCurrentView1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(206)))), ((int)(((byte)(214)))));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.alarmCurrentView1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 99.99999F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 339);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current Alarm";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // alarmHistoryView1
            // 
            this.alarmHistoryView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmHistoryView1.Location = new System.Drawing.Point(23, 33);
            this.alarmHistoryView1.Name = "alarmHistoryView1";
            this.alarmHistoryView1.Size = new System.Drawing.Size(810, 339);
            this.alarmHistoryView1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.alarmHistoryView1, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(856, 395);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // AlarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(870, 431);
            this.Controls.Add(this.tabControlAlarm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AlarmForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "AlarmForm";
            this.Load += new System.EventHandler(this.AlarmForm_Load);
            this.tabControlAlarm.ResumeLayout(false);
            this.tabPageList.ResumeLayout(false);
            this.tabPageHistory.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Library.FlatTabControl tabControlAlarm;
        private System.Windows.Forms.TabPage tabPageList;
        private System.Windows.Forms.TabPage tabPageHistory;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Library.IButton btnCurAlarmSave;
        private System.Windows.Forms.Panel panel1;
        private Library.IButton btnSave;
        private Data.Alarm.AlarmListView alarmListView1;
        private Data.Alarm.AlarmCurrentView alarmCurrentView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Data.Alarm.AlarmHistoryView alarmHistoryView1;
    }
}