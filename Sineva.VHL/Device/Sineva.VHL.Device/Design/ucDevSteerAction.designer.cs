
namespace Sineva.VHL.Device
{
    partial class ucDevSteerAction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucDevSteerAction));
            this.cbRepeat = new System.Windows.Forms.CheckBox();
            this.cbAllSelect = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnFrontSteerLeft = new Sineva.VHL.Library.IButton();
            this.btnRearSteerLeft = new Sineva.VHL.Library.IButton();
            this.dsdRearSteerStatus_Manual = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.dsdFrontSteerStatus_Manual = new Sineva.VHL.Device.ucDevStatusDisplay();
            this.btnFrontSteerRight = new Sineva.VHL.Library.IButton();
            this.btnRearSteerRight = new Sineva.VHL.Library.IButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbRepeat
            // 
            this.cbRepeat.AutoSize = true;
            this.cbRepeat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbRepeat.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F);
            this.cbRepeat.ForeColor = System.Drawing.Color.White;
            this.cbRepeat.Location = new System.Drawing.Point(63, 43);
            this.cbRepeat.Name = "cbRepeat";
            this.cbRepeat.Padding = new System.Windows.Forms.Padding(3);
            this.cbRepeat.Size = new System.Drawing.Size(104, 34);
            this.cbRepeat.TabIndex = 83;
            this.cbRepeat.Text = "Repeat";
            this.cbRepeat.UseVisualStyleBackColor = true;
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
            this.cbAllSelect.TabIndex = 82;
            this.cbAllSelect.Text = "All Select";
            this.cbAllSelect.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.ForeColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(3, 43);
            this.label17.Margin = new System.Windows.Forms.Padding(3);
            this.label17.Name = "label17";
            this.label17.Padding = new System.Windows.Forms.Padding(3);
            this.label17.Size = new System.Drawing.Size(54, 34);
            this.label17.TabIndex = 70;
            this.label17.Text = "Rear:";
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
            this.label18.Padding = new System.Windows.Forms.Padding(3);
            this.label18.Size = new System.Drawing.Size(54, 34);
            this.label18.TabIndex = 68;
            this.label18.Text = "Front:";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFrontSteerLeft
            // 
            this.btnFrontSteerLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerLeft.BackgroundImage")));
            this.btnFrontSteerLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontSteerLeft.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerLeft.BgDefault")));
            this.btnFrontSteerLeft.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerLeft.BgDisable")));
            this.btnFrontSteerLeft.BgOver = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerLeft.BgOver")));
            this.btnFrontSteerLeft.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerLeft.BgPushed")));
            this.btnFrontSteerLeft.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnFrontSteerLeft.ConnectedLabel = null;
            this.btnFrontSteerLeft.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnFrontSteerLeft.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnFrontSteerLeft.DefaultImage = null;
            this.btnFrontSteerLeft.Description = null;
            this.btnFrontSteerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFrontSteerLeft.DownImage = null;
            this.btnFrontSteerLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontSteerLeft.ForeColor = System.Drawing.Color.White;
            this.btnFrontSteerLeft.Image = global::Sineva.VHL.Device.Properties.Resources.button_left;
            this.btnFrontSteerLeft.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFrontSteerLeft.Location = new System.Drawing.Point(173, 3);
            this.btnFrontSteerLeft.Name = "btnFrontSteerLeft";
            this.btnFrontSteerLeft.OverImage = null;
            this.btnFrontSteerLeft.Padding = new System.Windows.Forms.Padding(3);
            this.btnFrontSteerLeft.Size = new System.Drawing.Size(109, 34);
            this.btnFrontSteerLeft.TabIndex = 76;
            this.btnFrontSteerLeft.Text = "Left";
            this.btnFrontSteerLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFrontSteerLeft.UpImage = null;
            this.btnFrontSteerLeft.UseOneImage = true;
            this.btnFrontSteerLeft.UseVisualStyleBackColor = true;
            this.btnFrontSteerLeft.Click += new System.EventHandler(this.btnFrontSteerLeft_Click);
            // 
            // btnRearSteerLeft
            // 
            this.btnRearSteerLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRearSteerLeft.BackgroundImage")));
            this.btnRearSteerLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearSteerLeft.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnRearSteerLeft.BgDefault")));
            this.btnRearSteerLeft.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnRearSteerLeft.BgDisable")));
            this.btnRearSteerLeft.BgOver = ((System.Drawing.Image)(resources.GetObject("btnRearSteerLeft.BgOver")));
            this.btnRearSteerLeft.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnRearSteerLeft.BgPushed")));
            this.btnRearSteerLeft.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRearSteerLeft.ConnectedLabel = null;
            this.btnRearSteerLeft.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRearSteerLeft.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRearSteerLeft.DefaultImage = null;
            this.btnRearSteerLeft.Description = null;
            this.btnRearSteerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRearSteerLeft.DownImage = null;
            this.btnRearSteerLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearSteerLeft.ForeColor = System.Drawing.Color.White;
            this.btnRearSteerLeft.Image = global::Sineva.VHL.Device.Properties.Resources.button_left;
            this.btnRearSteerLeft.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRearSteerLeft.Location = new System.Drawing.Point(173, 43);
            this.btnRearSteerLeft.Name = "btnRearSteerLeft";
            this.btnRearSteerLeft.OverImage = null;
            this.btnRearSteerLeft.Padding = new System.Windows.Forms.Padding(3);
            this.btnRearSteerLeft.Size = new System.Drawing.Size(109, 34);
            this.btnRearSteerLeft.TabIndex = 77;
            this.btnRearSteerLeft.Text = "Left";
            this.btnRearSteerLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRearSteerLeft.UpImage = null;
            this.btnRearSteerLeft.UseOneImage = true;
            this.btnRearSteerLeft.UseVisualStyleBackColor = true;
            this.btnRearSteerLeft.Click += new System.EventHandler(this.btnRearSteerLeft_Click);
            // 
            // dsdRearSteerStatus_Manual
            // 
            this.dsdRearSteerStatus_Manual.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdRearSteerStatus_Manual.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdRearSteerStatus_Manual.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdRearSteerStatus_Manual.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.Steer;
            this.dsdRearSteerStatus_Manual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsdRearSteerStatus_Manual.FirstStateIoTag = null;
            this.dsdRearSteerStatus_Manual.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdRearSteerStatus_Manual.ForeColor = System.Drawing.Color.White;
            this.dsdRearSteerStatus_Manual.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdRearSteerStatus_Manual.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdRearSteerStatus_Manual.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdRearSteerStatus_Manual.Location = new System.Drawing.Point(286, 41);
            this.dsdRearSteerStatus_Manual.Margin = new System.Windows.Forms.Padding(1);
            this.dsdRearSteerStatus_Manual.Name = "dsdRearSteerStatus_Manual";
            this.dsdRearSteerStatus_Manual.Padding = new System.Windows.Forms.Padding(3);
            this.dsdRearSteerStatus_Manual.SecondStateIoTag = null;
            this.dsdRearSteerStatus_Manual.Size = new System.Drawing.Size(98, 38);
            this.dsdRearSteerStatus_Manual.TabIndex = 78;
            this.dsdRearSteerStatus_Manual.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdRearSteerStatus_Manual.TextValue = "None";
            // 
            // dsdFrontSteerStatus_Manual
            // 
            this.dsdFrontSteerStatus_Manual.BackColorOfFirstState = System.Drawing.Color.Salmon;
            this.dsdFrontSteerStatus_Manual.BackColorOfMiddleState = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.dsdFrontSteerStatus_Manual.BackColorOfSecondState = System.Drawing.Color.DeepSkyBlue;
            this.dsdFrontSteerStatus_Manual.CurrentDeviceType = Sineva.VHL.Library.enDeviceType.Steer;
            this.dsdFrontSteerStatus_Manual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsdFrontSteerStatus_Manual.FirstStateIoTag = null;
            this.dsdFrontSteerStatus_Manual.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdFrontSteerStatus_Manual.ForeColor = System.Drawing.Color.White;
            this.dsdFrontSteerStatus_Manual.ForeColorOfFirstState = System.Drawing.Color.Black;
            this.dsdFrontSteerStatus_Manual.ForeColorOfMiddleState = System.Drawing.Color.Black;
            this.dsdFrontSteerStatus_Manual.ForeColorOfSecondState = System.Drawing.Color.Black;
            this.dsdFrontSteerStatus_Manual.Location = new System.Drawing.Point(286, 1);
            this.dsdFrontSteerStatus_Manual.Margin = new System.Windows.Forms.Padding(1);
            this.dsdFrontSteerStatus_Manual.Name = "dsdFrontSteerStatus_Manual";
            this.dsdFrontSteerStatus_Manual.Padding = new System.Windows.Forms.Padding(3);
            this.dsdFrontSteerStatus_Manual.SecondStateIoTag = null;
            this.dsdFrontSteerStatus_Manual.Size = new System.Drawing.Size(98, 38);
            this.dsdFrontSteerStatus_Manual.TabIndex = 79;
            this.dsdFrontSteerStatus_Manual.TextFont = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dsdFrontSteerStatus_Manual.TextValue = "None";
            // 
            // btnFrontSteerRight
            // 
            this.btnFrontSteerRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerRight.BackgroundImage")));
            this.btnFrontSteerRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontSteerRight.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerRight.BgDefault")));
            this.btnFrontSteerRight.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerRight.BgDisable")));
            this.btnFrontSteerRight.BgOver = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerRight.BgOver")));
            this.btnFrontSteerRight.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnFrontSteerRight.BgPushed")));
            this.btnFrontSteerRight.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnFrontSteerRight.ConnectedLabel = null;
            this.btnFrontSteerRight.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnFrontSteerRight.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnFrontSteerRight.DefaultImage = null;
            this.btnFrontSteerRight.Description = null;
            this.btnFrontSteerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFrontSteerRight.DownImage = null;
            this.btnFrontSteerRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontSteerRight.ForeColor = System.Drawing.Color.White;
            this.btnFrontSteerRight.Image = global::Sineva.VHL.Device.Properties.Resources.button_right;
            this.btnFrontSteerRight.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFrontSteerRight.Location = new System.Drawing.Point(388, 3);
            this.btnFrontSteerRight.Name = "btnFrontSteerRight";
            this.btnFrontSteerRight.OverImage = null;
            this.btnFrontSteerRight.Padding = new System.Windows.Forms.Padding(3);
            this.btnFrontSteerRight.Size = new System.Drawing.Size(109, 34);
            this.btnFrontSteerRight.TabIndex = 80;
            this.btnFrontSteerRight.Text = "Right";
            this.btnFrontSteerRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFrontSteerRight.UpImage = null;
            this.btnFrontSteerRight.UseOneImage = true;
            this.btnFrontSteerRight.UseVisualStyleBackColor = true;
            this.btnFrontSteerRight.Click += new System.EventHandler(this.btnFrontSteerRight_Click);
            // 
            // btnRearSteerRight
            // 
            this.btnRearSteerRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRearSteerRight.BackgroundImage")));
            this.btnRearSteerRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearSteerRight.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnRearSteerRight.BgDefault")));
            this.btnRearSteerRight.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnRearSteerRight.BgDisable")));
            this.btnRearSteerRight.BgOver = ((System.Drawing.Image)(resources.GetObject("btnRearSteerRight.BgOver")));
            this.btnRearSteerRight.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnRearSteerRight.BgPushed")));
            this.btnRearSteerRight.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnRearSteerRight.ConnectedLabel = null;
            this.btnRearSteerRight.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnRearSteerRight.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnRearSteerRight.DefaultImage = null;
            this.btnRearSteerRight.Description = null;
            this.btnRearSteerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRearSteerRight.DownImage = null;
            this.btnRearSteerRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearSteerRight.ForeColor = System.Drawing.Color.White;
            this.btnRearSteerRight.Image = global::Sineva.VHL.Device.Properties.Resources.button_right;
            this.btnRearSteerRight.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRearSteerRight.Location = new System.Drawing.Point(388, 43);
            this.btnRearSteerRight.Name = "btnRearSteerRight";
            this.btnRearSteerRight.OverImage = null;
            this.btnRearSteerRight.Padding = new System.Windows.Forms.Padding(3);
            this.btnRearSteerRight.Size = new System.Drawing.Size(109, 34);
            this.btnRearSteerRight.TabIndex = 81;
            this.btnRearSteerRight.Text = "Right";
            this.btnRearSteerRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRearSteerRight.UpImage = null;
            this.btnRearSteerRight.UseOneImage = true;
            this.btnRearSteerRight.UseVisualStyleBackColor = true;
            this.btnRearSteerRight.Click += new System.EventHandler(this.btnRearSteerRight_Click);
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
            this.tableLayoutPanel1.Controls.Add(this.btnRearSteerRight, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnFrontSteerRight, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label17, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dsdFrontSteerStatus_Manual, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbAllSelect, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbRepeat, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dsdRearSteerStatus_Manual, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnFrontSteerLeft, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRearSteerLeft, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 80);
            this.tableLayoutPanel1.TabIndex = 84;
            // 
            // ucDevSteerAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Margin = new System.Windows.Forms.Padding(55, 24, 55, 24);
            this.Name = "ucDevSteerAction";
            this.Size = new System.Drawing.Size(500, 80);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private Library.IButton btnFrontSteerLeft;
        private Library.IButton btnRearSteerLeft;
        private ucDevStatusDisplay dsdRearSteerStatus_Manual;
        private ucDevStatusDisplay dsdFrontSteerStatus_Manual;
        private Library.IButton btnFrontSteerRight;
        private Library.IButton btnRearSteerRight;
        private System.Windows.Forms.CheckBox cbAllSelect;
        private System.Windows.Forms.CheckBox cbRepeat;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
