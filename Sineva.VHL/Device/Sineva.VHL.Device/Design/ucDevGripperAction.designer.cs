
namespace Sineva.VHL.Device
{
    partial class ucDevGripperAction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucDevGripperAction));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dsdGripperStatus = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.cbRepeat = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnGripperOpen = new Sineva.VHL.Library.IButton();
            this.btnGripperClose = new Sineva.VHL.Library.IButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnGripperOpen, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.dsdGripperStatus, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbRepeat, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGripperClose, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 46);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dsdGripperStatus
            // 
            this.dsdGripperStatus.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdGripperStatus.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdGripperStatus.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdGripperStatus.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.Gripper;
            this.dsdGripperStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsdGripperStatus.FirstStateIoTag = null;
            this.dsdGripperStatus.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdGripperStatus.ForeColor = System.Drawing.Color.White;
            this.dsdGripperStatus.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdGripperStatus.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdGripperStatus.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdGripperStatus.Location = new System.Drawing.Point(286, 1);
            this.dsdGripperStatus.Margin = new System.Windows.Forms.Padding(1);
            this.dsdGripperStatus.Name = "dsdGripperStatus";
            this.dsdGripperStatus.Padding = new System.Windows.Forms.Padding(3);
            this.dsdGripperStatus.SecondStateIoTag = null;
            this.dsdGripperStatus.Size = new System.Drawing.Size(108, 44);
            this.dsdGripperStatus.TabIndex = 79;
            this.dsdGripperStatus.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdGripperStatus.TextValue = "None";
            // 
            // cbRepeat
            // 
            this.cbRepeat.AutoSize = true;
            this.cbRepeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbRepeat.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.cbRepeat.ForeColor = System.Drawing.Color.White;
            this.cbRepeat.Location = new System.Drawing.Point(83, 3);
            this.cbRepeat.Name = "cbRepeat";
            this.cbRepeat.Padding = new System.Windows.Forms.Padding(3);
            this.cbRepeat.Size = new System.Drawing.Size(94, 40);
            this.cbRepeat.TabIndex = 82;
            this.cbRepeat.Text = "Repeat";
            this.cbRepeat.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.ForeColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(3, 3);
            this.label18.Margin = new System.Windows.Forms.Padding(3);
            this.label18.Name = "label18";
            this.label18.Padding = new System.Windows.Forms.Padding(3);
            this.label18.Size = new System.Drawing.Size(74, 40);
            this.label18.TabIndex = 69;
            this.label18.Text = "Gripper :";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnGripperOpen
            // 
            this.btnGripperOpen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnGripperOpen.BackgroundImage")));
            this.btnGripperOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGripperOpen.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnGripperOpen.BgDefault")));
            this.btnGripperOpen.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnGripperOpen.BgDisable")));
            this.btnGripperOpen.BgOver = ((System.Drawing.Image)(resources.GetObject("btnGripperOpen.BgOver")));
            this.btnGripperOpen.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnGripperOpen.BgPushed")));
            this.btnGripperOpen.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnGripperOpen.ConnectedLabel = null;
            this.btnGripperOpen.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnGripperOpen.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnGripperOpen.DefaultImage = null;
            this.btnGripperOpen.Description = null;
            this.btnGripperOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGripperOpen.DownImage = null;
            this.btnGripperOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGripperOpen.ForeColor = System.Drawing.Color.White;
            this.btnGripperOpen.Image = global::Sineva.VHL.Device.Properties.Resources.button_unlock;
            this.btnGripperOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGripperOpen.Location = new System.Drawing.Point(398, 3);
            this.btnGripperOpen.Name = "btnGripperOpen";
            this.btnGripperOpen.OverImage = null;
            this.btnGripperOpen.Padding = new System.Windows.Forms.Padding(3);
            this.btnGripperOpen.Size = new System.Drawing.Size(99, 40);
            this.btnGripperOpen.TabIndex = 77;
            this.btnGripperOpen.Text = "Open";
            this.btnGripperOpen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGripperOpen.UpImage = null;
            this.btnGripperOpen.UseOneImage = true;
            this.btnGripperOpen.UseVisualStyleBackColor = true;
            this.btnGripperOpen.Click += new System.EventHandler(this.btnGripperOpen_Click);
            // 
            // btnGripperClose
            // 
            this.btnGripperClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnGripperClose.BackgroundImage")));
            this.btnGripperClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGripperClose.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnGripperClose.BgDefault")));
            this.btnGripperClose.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnGripperClose.BgDisable")));
            this.btnGripperClose.BgOver = ((System.Drawing.Image)(resources.GetObject("btnGripperClose.BgOver")));
            this.btnGripperClose.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnGripperClose.BgPushed")));
            this.btnGripperClose.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnGripperClose.ConnectedLabel = null;
            this.btnGripperClose.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnGripperClose.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnGripperClose.DefaultImage = null;
            this.btnGripperClose.Description = null;
            this.btnGripperClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGripperClose.DownImage = null;
            this.btnGripperClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGripperClose.ForeColor = System.Drawing.Color.White;
            this.btnGripperClose.Image = global::Sineva.VHL.Device.Properties.Resources.button_lock;
            this.btnGripperClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGripperClose.Location = new System.Drawing.Point(183, 3);
            this.btnGripperClose.Name = "btnGripperClose";
            this.btnGripperClose.OverImage = null;
            this.btnGripperClose.Padding = new System.Windows.Forms.Padding(3);
            this.btnGripperClose.Size = new System.Drawing.Size(99, 40);
            this.btnGripperClose.TabIndex = 81;
            this.btnGripperClose.Text = "Close";
            this.btnGripperClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGripperClose.UpImage = null;
            this.btnGripperClose.UseOneImage = true;
            this.btnGripperClose.UseVisualStyleBackColor = true;
            this.btnGripperClose.Click += new System.EventHandler(this.btnGripperClose_Click);
            // 
            // ucDevGripperAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ucDevGripperAction";
            this.Size = new System.Drawing.Size(500, 46);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label18;
        private Library.IButton btnGripperOpen;
        private ucDevStatusDisplay dsdGripperStatus;
        private Library.IButton btnGripperClose;
        private System.Windows.Forms.CheckBox cbRepeat;
    }
}
