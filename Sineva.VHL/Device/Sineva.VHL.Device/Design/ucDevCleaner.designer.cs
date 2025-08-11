
namespace Sineva.VHL.Device
{
    partial class ucDevCleaner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucDevCleaner));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnStart = new Sineva.VHL.Library.IButton();
            this.lbRun = new System.Windows.Forms.Label();
            this.lbDoorClose = new System.Windows.Forms.Label();
            this.lbTempAlarm1 = new System.Windows.Forms.Label();
            this.lbTempAlarm2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnStart, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbRun, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbDoorClose, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbTempAlarm1, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbTempAlarm2, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 46);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStart.BackgroundImage")));
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStart.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnStart.BgDefault")));
            this.btnStart.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnStart.BgDisable")));
            this.btnStart.BgOver = ((System.Drawing.Image)(resources.GetObject("btnStart.BgOver")));
            this.btnStart.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnStart.BgPushed")));
            this.btnStart.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnStart.ConnectedLabel = null;
            this.btnStart.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnStart.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnStart.DefaultImage = null;
            this.btnStart.Description = null;
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStart.DownImage = null;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.OverImage = null;
            this.btnStart.Padding = new System.Windows.Forms.Padding(3);
            this.btnStart.Size = new System.Drawing.Size(94, 40);
            this.btnStart.TabIndex = 77;
            this.btnStart.Text = "Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStart.UpImage = null;
            this.btnStart.UseOneImage = true;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lbRun
            // 
            this.lbRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRun.Location = new System.Drawing.Point(103, 3);
            this.lbRun.Margin = new System.Windows.Forms.Padding(3);
            this.lbRun.Name = "lbRun";
            this.lbRun.Size = new System.Drawing.Size(94, 40);
            this.lbRun.TabIndex = 78;
            this.lbRun.Text = "RUN";
            this.lbRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbDoorClose
            // 
            this.lbDoorClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbDoorClose.Location = new System.Drawing.Point(203, 3);
            this.lbDoorClose.Margin = new System.Windows.Forms.Padding(3);
            this.lbDoorClose.Name = "lbDoorClose";
            this.lbDoorClose.Size = new System.Drawing.Size(94, 40);
            this.lbDoorClose.TabIndex = 78;
            this.lbDoorClose.Text = "Door Close";
            this.lbDoorClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTempAlarm1
            // 
            this.lbTempAlarm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTempAlarm1.Location = new System.Drawing.Point(303, 3);
            this.lbTempAlarm1.Margin = new System.Windows.Forms.Padding(3);
            this.lbTempAlarm1.Name = "lbTempAlarm1";
            this.lbTempAlarm1.Size = new System.Drawing.Size(94, 40);
            this.lbTempAlarm1.TabIndex = 78;
            this.lbTempAlarm1.Text = "Temp Alarm1";
            this.lbTempAlarm1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTempAlarm2
            // 
            this.lbTempAlarm2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTempAlarm2.Location = new System.Drawing.Point(403, 3);
            this.lbTempAlarm2.Margin = new System.Windows.Forms.Padding(3);
            this.lbTempAlarm2.Name = "lbTempAlarm2";
            this.lbTempAlarm2.Size = new System.Drawing.Size(94, 40);
            this.lbTempAlarm2.TabIndex = 78;
            this.lbTempAlarm2.Text = "Temp Alarm2";
            this.lbTempAlarm2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucDevCleaner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ucDevCleaner";
            this.Size = new System.Drawing.Size(500, 46);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Library.IButton btnStart;
        private System.Windows.Forms.Label lbRun;
        private System.Windows.Forms.Label lbDoorClose;
        private System.Windows.Forms.Label lbTempAlarm1;
        private System.Windows.Forms.Label lbTempAlarm2;
    }
}
