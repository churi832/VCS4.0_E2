
namespace Sineva.VHL.Device
{
    partial class ucDevOutRiggerAction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucDevOutRiggerAction));
            this.cbRepeat = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnFrontOutRiggerLock = new Sineva.VHL.Library.IButton();
            this.btnRearOutRiggerLock = new Sineva.VHL.Library.IButton();
            this.dsdFrontOutRiggerStatus = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.dsdRearOutRiggerStatus = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.btnFrontOutRiggerUnlock = new Sineva.VHL.Library.IButton();
            this.btnRearOutRiggerUnlock = new Sineva.VHL.Library.IButton();
            this.cbAllSelect = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbRepeat
            // 
            this.cbRepeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbRepeat.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.cbRepeat.ForeColor = System.Drawing.Color.White;
            this.cbRepeat.Location = new System.Drawing.Point(63, 43);
            this.cbRepeat.Name = "cbRepeat";
            this.cbRepeat.Padding = new System.Windows.Forms.Padding(3);
            this.cbRepeat.Size = new System.Drawing.Size(104, 34);
            this.cbRepeat.TabIndex = 81;
            this.cbRepeat.Text = "Repeat";
            this.cbRepeat.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.ForeColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(3, 43);
            this.label17.Margin = new System.Windows.Forms.Padding(3);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(54, 34);
            this.label17.TabIndex = 70;
            this.label17.Text = "Rear :";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.ForeColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(3, 3);
            this.label18.Margin = new System.Windows.Forms.Padding(3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(54, 34);
            this.label18.TabIndex = 68;
            this.label18.Text = "Front :";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFrontOutRiggerLock
            // 
            this.btnFrontOutRiggerLock.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerLock.BackgroundImage")));
            this.btnFrontOutRiggerLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontOutRiggerLock.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerLock.BgDefault")));
            this.btnFrontOutRiggerLock.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerLock.BgDisable")));
            this.btnFrontOutRiggerLock.BgOver = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerLock.BgOver")));
            this.btnFrontOutRiggerLock.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerLock.BgPushed")));
            this.btnFrontOutRiggerLock.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnFrontOutRiggerLock.ConnectedLabel = null;
            this.btnFrontOutRiggerLock.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnFrontOutRiggerLock.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnFrontOutRiggerLock.DefaultImage = null;
            this.btnFrontOutRiggerLock.Description = null;
            this.btnFrontOutRiggerLock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFrontOutRiggerLock.DownImage = null;
            this.btnFrontOutRiggerLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontOutRiggerLock.ForeColor = System.Drawing.Color.White;
            this.btnFrontOutRiggerLock.Image = global::Sineva.VHL.Device.Properties.Resources.button_lock;
            this.btnFrontOutRiggerLock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFrontOutRiggerLock.Location = new System.Drawing.Point(173, 3);
            this.btnFrontOutRiggerLock.Name = "btnFrontOutRiggerLock";
            this.btnFrontOutRiggerLock.OverImage = null;
            this.btnFrontOutRiggerLock.Padding = new System.Windows.Forms.Padding(3);
            this.btnFrontOutRiggerLock.Size = new System.Drawing.Size(109, 34);
            this.btnFrontOutRiggerLock.TabIndex = 76;
            this.btnFrontOutRiggerLock.Text = "Lock";
            this.btnFrontOutRiggerLock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFrontOutRiggerLock.UpImage = null;
            this.btnFrontOutRiggerLock.UseOneImage = true;
            this.btnFrontOutRiggerLock.UseVisualStyleBackColor = true;
            this.btnFrontOutRiggerLock.Click += new System.EventHandler(this.btnFrontOutRiggerLock_Click);
            // 
            // btnRearOutRiggerLock
            // 
            this.btnRearOutRiggerLock.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerLock.BackgroundImage")));
            this.btnRearOutRiggerLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearOutRiggerLock.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerLock.BgDefault")));
            this.btnRearOutRiggerLock.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerLock.BgDisable")));
            this.btnRearOutRiggerLock.BgOver = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerLock.BgOver")));
            this.btnRearOutRiggerLock.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerLock.BgPushed")));
            this.btnRearOutRiggerLock.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRearOutRiggerLock.ConnectedLabel = null;
            this.btnRearOutRiggerLock.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRearOutRiggerLock.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRearOutRiggerLock.DefaultImage = null;
            this.btnRearOutRiggerLock.Description = null;
            this.btnRearOutRiggerLock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRearOutRiggerLock.DownImage = null;
            this.btnRearOutRiggerLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearOutRiggerLock.ForeColor = System.Drawing.Color.White;
            this.btnRearOutRiggerLock.Image = global::Sineva.VHL.Device.Properties.Resources.button_lock;
            this.btnRearOutRiggerLock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRearOutRiggerLock.Location = new System.Drawing.Point(173, 43);
            this.btnRearOutRiggerLock.Name = "btnRearOutRiggerLock";
            this.btnRearOutRiggerLock.OverImage = null;
            this.btnRearOutRiggerLock.Size = new System.Drawing.Size(109, 34);
            this.btnRearOutRiggerLock.TabIndex = 77;
            this.btnRearOutRiggerLock.Text = "Lock";
            this.btnRearOutRiggerLock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRearOutRiggerLock.UpImage = null;
            this.btnRearOutRiggerLock.UseOneImage = true;
            this.btnRearOutRiggerLock.UseVisualStyleBackColor = true;
            this.btnRearOutRiggerLock.Click += new System.EventHandler(this.btnRearOutRiggerLock_Click);
            // 
            // dsdFrontOutRiggerStatus
            // 
            this.dsdFrontOutRiggerStatus.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdFrontOutRiggerStatus.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdFrontOutRiggerStatus.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdFrontOutRiggerStatus.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.OutRigger;
            this.dsdFrontOutRiggerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsdFrontOutRiggerStatus.FirstStateIoTag = null;
            this.dsdFrontOutRiggerStatus.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdFrontOutRiggerStatus.ForeColor = System.Drawing.Color.White;
            this.dsdFrontOutRiggerStatus.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdFrontOutRiggerStatus.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdFrontOutRiggerStatus.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdFrontOutRiggerStatus.Location = new System.Drawing.Point(333, 24);
            this.dsdFrontOutRiggerStatus.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.dsdFrontOutRiggerStatus.Name = "dsdFrontOutRiggerStatus";
            this.dsdFrontOutRiggerStatus.Padding = new System.Windows.Forms.Padding(3);
            this.dsdFrontOutRiggerStatus.SecondStateIoTag = null;
            this.dsdFrontOutRiggerStatus.Size = new System.Drawing.Size(4, 1);
            this.dsdFrontOutRiggerStatus.TabIndex = 78;
            this.dsdFrontOutRiggerStatus.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdFrontOutRiggerStatus.TextValue = "None";
            // 
            // dsdRearOutRiggerStatus
            // 
            this.dsdRearOutRiggerStatus.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdRearOutRiggerStatus.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdRearOutRiggerStatus.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdRearOutRiggerStatus.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.Steer;
            this.dsdRearOutRiggerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsdRearOutRiggerStatus.FirstStateIoTag = null;
            this.dsdRearOutRiggerStatus.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdRearOutRiggerStatus.ForeColor = System.Drawing.Color.White;
            this.dsdRearOutRiggerStatus.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdRearOutRiggerStatus.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdRearOutRiggerStatus.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdRearOutRiggerStatus.Location = new System.Drawing.Point(333, 64);
            this.dsdRearOutRiggerStatus.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.dsdRearOutRiggerStatus.Name = "dsdRearOutRiggerStatus";
            this.dsdRearOutRiggerStatus.Padding = new System.Windows.Forms.Padding(3);
            this.dsdRearOutRiggerStatus.SecondStateIoTag = null;
            this.dsdRearOutRiggerStatus.Size = new System.Drawing.Size(4, 1);
            this.dsdRearOutRiggerStatus.TabIndex = 79;
            this.dsdRearOutRiggerStatus.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdRearOutRiggerStatus.TextValue = "None";
            // 
            // btnFrontOutRiggerUnlock
            // 
            this.btnFrontOutRiggerUnlock.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerUnlock.BackgroundImage")));
            this.btnFrontOutRiggerUnlock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontOutRiggerUnlock.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerUnlock.BgDefault")));
            this.btnFrontOutRiggerUnlock.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerUnlock.BgDisable")));
            this.btnFrontOutRiggerUnlock.BgOver = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerUnlock.BgOver")));
            this.btnFrontOutRiggerUnlock.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnFrontOutRiggerUnlock.BgPushed")));
            this.btnFrontOutRiggerUnlock.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnFrontOutRiggerUnlock.ConnectedLabel = null;
            this.btnFrontOutRiggerUnlock.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnFrontOutRiggerUnlock.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnFrontOutRiggerUnlock.DefaultImage = null;
            this.btnFrontOutRiggerUnlock.Description = null;
            this.btnFrontOutRiggerUnlock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFrontOutRiggerUnlock.DownImage = null;
            this.btnFrontOutRiggerUnlock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontOutRiggerUnlock.ForeColor = System.Drawing.Color.White;
            this.btnFrontOutRiggerUnlock.Image = global::Sineva.VHL.Device.Properties.Resources.button_unlock;
            this.btnFrontOutRiggerUnlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFrontOutRiggerUnlock.Location = new System.Drawing.Point(388, 3);
            this.btnFrontOutRiggerUnlock.Name = "btnFrontOutRiggerUnlock";
            this.btnFrontOutRiggerUnlock.OverImage = null;
            this.btnFrontOutRiggerUnlock.Padding = new System.Windows.Forms.Padding(3);
            this.btnFrontOutRiggerUnlock.Size = new System.Drawing.Size(109, 34);
            this.btnFrontOutRiggerUnlock.TabIndex = 80;
            this.btnFrontOutRiggerUnlock.Text = "Unlock";
            this.btnFrontOutRiggerUnlock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFrontOutRiggerUnlock.UpImage = null;
            this.btnFrontOutRiggerUnlock.UseOneImage = true;
            this.btnFrontOutRiggerUnlock.UseVisualStyleBackColor = true;
            this.btnFrontOutRiggerUnlock.Click += new System.EventHandler(this.btnFrontOutRiggerUnlock_Click);
            // 
            // btnRearOutRiggerUnlock
            // 
            this.btnRearOutRiggerUnlock.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerUnlock.BackgroundImage")));
            this.btnRearOutRiggerUnlock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearOutRiggerUnlock.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerUnlock.BgDefault")));
            this.btnRearOutRiggerUnlock.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerUnlock.BgDisable")));
            this.btnRearOutRiggerUnlock.BgOver = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerUnlock.BgOver")));
            this.btnRearOutRiggerUnlock.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnRearOutRiggerUnlock.BgPushed")));
            this.btnRearOutRiggerUnlock.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRearOutRiggerUnlock.ConnectedLabel = null;
            this.btnRearOutRiggerUnlock.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRearOutRiggerUnlock.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRearOutRiggerUnlock.DefaultImage = null;
            this.btnRearOutRiggerUnlock.Description = null;
            this.btnRearOutRiggerUnlock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRearOutRiggerUnlock.DownImage = null;
            this.btnRearOutRiggerUnlock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearOutRiggerUnlock.ForeColor = System.Drawing.Color.White;
            this.btnRearOutRiggerUnlock.Image = global::Sineva.VHL.Device.Properties.Resources.button_unlock;
            this.btnRearOutRiggerUnlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRearOutRiggerUnlock.Location = new System.Drawing.Point(388, 43);
            this.btnRearOutRiggerUnlock.Name = "btnRearOutRiggerUnlock";
            this.btnRearOutRiggerUnlock.OverImage = null;
            this.btnRearOutRiggerUnlock.Padding = new System.Windows.Forms.Padding(3);
            this.btnRearOutRiggerUnlock.Size = new System.Drawing.Size(109, 34);
            this.btnRearOutRiggerUnlock.TabIndex = 81;
            this.btnRearOutRiggerUnlock.Text = "Unlock";
            this.btnRearOutRiggerUnlock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRearOutRiggerUnlock.UpImage = null;
            this.btnRearOutRiggerUnlock.UseOneImage = true;
            this.btnRearOutRiggerUnlock.UseVisualStyleBackColor = true;
            this.btnRearOutRiggerUnlock.Click += new System.EventHandler(this.btnRearOutRiggerUnlock_Click);
            // 
            // cbAllSelect
            // 
            this.cbAllSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAllSelect.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.cbAllSelect.ForeColor = System.Drawing.Color.White;
            this.cbAllSelect.Location = new System.Drawing.Point(63, 3);
            this.cbAllSelect.Name = "cbAllSelect";
            this.cbAllSelect.Padding = new System.Windows.Forms.Padding(3);
            this.cbAllSelect.Size = new System.Drawing.Size(104, 34);
            this.cbAllSelect.TabIndex = 81;
            this.cbAllSelect.Text = "All Select";
            this.cbAllSelect.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRearOutRiggerUnlock, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnFrontOutRiggerUnlock, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label17, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dsdFrontOutRiggerStatus, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.dsdRearOutRiggerStatus, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbAllSelect, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbRepeat, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRearOutRiggerLock, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnFrontOutRiggerLock, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 80);
            this.tableLayoutPanel1.TabIndex = 82;
            // 
            // ucDevOutRiggerAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(55, 24, 55, 24);
            this.Name = "ucDevOutRiggerAction";
            this.Size = new System.Drawing.Size(500, 80);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private Library.IButton btnFrontOutRiggerLock;
        private Library.IButton btnRearOutRiggerLock;
        private ucDevStatusDisplay dsdFrontOutRiggerStatus;
        private ucDevStatusDisplay dsdRearOutRiggerStatus;
        private Library.IButton btnFrontOutRiggerUnlock;
        private Library.IButton btnRearOutRiggerUnlock;
        private System.Windows.Forms.CheckBox cbAllSelect;
        private System.Windows.Forms.CheckBox cbRepeat;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
