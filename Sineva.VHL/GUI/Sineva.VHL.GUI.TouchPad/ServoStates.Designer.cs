namespace Sineva.VHL.GUI.TouchPad
{
    partial class ServoStates
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
            this.ssvSlavePosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.lbServoName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ssvMasterPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.label7 = new System.Windows.Forms.Label();
            this.ssvHoistPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.label8 = new System.Windows.Forms.Label();
            this.ssvRotatePosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.label9 = new System.Windows.Forms.Label();
            this.ssvSlidePosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.label10 = new System.Windows.Forms.Label();
            this.ssvFrontAntiDropPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.label11 = new System.Windows.Forms.Label();
            this.ssvRearAntiDropPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ssdRearAntiDropHomeDone = new System.Windows.Forms.Label();
            this.ssdRearAntiDropStatus = new System.Windows.Forms.Label();
            this.ssdFrontAntiDropHomeDone = new System.Windows.Forms.Label();
            this.ssdFrontAntiDropStatus = new System.Windows.Forms.Label();
            this.ssdSlideHomeDone = new System.Windows.Forms.Label();
            this.ssdSlideStatus = new System.Windows.Forms.Label();
            this.ssdRotateHomeDone = new System.Windows.Forms.Label();
            this.ssdRotateStatus = new System.Windows.Forms.Label();
            this.ssdHoistHomeDone = new System.Windows.Forms.Label();
            this.ssdHoistStatus = new System.Windows.Forms.Label();
            this.ssdMasterHomeDone = new System.Windows.Forms.Label();
            this.ssdMasterStatus = new System.Windows.Forms.Label();
            this.ssdSlaveHomeDone = new System.Windows.Forms.Label();
            this.ssdSlaveStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssvSlavePosition
            // 
            this.ssvSlavePosition.AxisTag = null;
            this.ssvSlavePosition.BackColor = System.Drawing.Color.White;
            this.ssvSlavePosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvSlavePosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvSlavePosition.DecimalPoint = 4;
            this.ssvSlavePosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvSlavePosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvSlavePosition.Location = new System.Drawing.Point(168, 38);
            this.ssvSlavePosition.Name = "ssvSlavePosition";
            this.ssvSlavePosition.Size = new System.Drawing.Size(84, 29);
            this.ssvSlavePosition.TabIndex = 6;
            this.ssvSlavePosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvSlavePosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvSlavePosition.TextValue = "Value";
            this.ssvSlavePosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // lbServoName
            // 
            this.lbServoName.AutoSize = true;
            this.lbServoName.BackColor = System.Drawing.Color.LightGray;
            this.lbServoName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbServoName.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbServoName.Location = new System.Drawing.Point(3, 3);
            this.lbServoName.Margin = new System.Windows.Forms.Padding(3);
            this.lbServoName.Name = "lbServoName";
            this.lbServoName.Size = new System.Drawing.Size(159, 29);
            this.lbServoName.TabIndex = 0;
            this.lbServoName.Text = "伺服名称";
            this.lbServoName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightGray;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(168, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前位置";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightGray;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(258, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "伺服状态";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.LightGray;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(348, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 29);
            this.label3.TabIndex = 3;
            this.label3.Text = "复位情况";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.LightGray;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(3, 38);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 29);
            this.label5.TabIndex = 5;
            this.label5.Text = "Slave";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.LightGray;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(3, 73);
            this.label6.Margin = new System.Windows.Forms.Padding(3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(159, 29);
            this.label6.TabIndex = 10;
            this.label6.Text = "Master";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssvMasterPosition
            // 
            this.ssvMasterPosition.AxisTag = null;
            this.ssvMasterPosition.BackColor = System.Drawing.Color.White;
            this.ssvMasterPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvMasterPosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvMasterPosition.DecimalPoint = 4;
            this.ssvMasterPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvMasterPosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvMasterPosition.Location = new System.Drawing.Point(168, 73);
            this.ssvMasterPosition.Name = "ssvMasterPosition";
            this.ssvMasterPosition.Size = new System.Drawing.Size(84, 29);
            this.ssvMasterPosition.TabIndex = 11;
            this.ssvMasterPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvMasterPosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvMasterPosition.TextValue = "Value";
            this.ssvMasterPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.LightGray;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(3, 108);
            this.label7.Margin = new System.Windows.Forms.Padding(3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(159, 29);
            this.label7.TabIndex = 15;
            this.label7.Text = "Hoist";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssvHoistPosition
            // 
            this.ssvHoistPosition.AxisTag = null;
            this.ssvHoistPosition.BackColor = System.Drawing.Color.White;
            this.ssvHoistPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvHoistPosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvHoistPosition.DecimalPoint = 4;
            this.ssvHoistPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvHoistPosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvHoistPosition.Location = new System.Drawing.Point(168, 108);
            this.ssvHoistPosition.Name = "ssvHoistPosition";
            this.ssvHoistPosition.Size = new System.Drawing.Size(84, 29);
            this.ssvHoistPosition.TabIndex = 16;
            this.ssvHoistPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvHoistPosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvHoistPosition.TextValue = "Value";
            this.ssvHoistPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.LightGray;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(3, 143);
            this.label8.Margin = new System.Windows.Forms.Padding(3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(159, 29);
            this.label8.TabIndex = 20;
            this.label8.Text = "Rotate";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssvRotatePosition
            // 
            this.ssvRotatePosition.AxisTag = null;
            this.ssvRotatePosition.BackColor = System.Drawing.Color.White;
            this.ssvRotatePosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvRotatePosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvRotatePosition.DecimalPoint = 4;
            this.ssvRotatePosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvRotatePosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvRotatePosition.Location = new System.Drawing.Point(168, 143);
            this.ssvRotatePosition.Name = "ssvRotatePosition";
            this.ssvRotatePosition.Size = new System.Drawing.Size(84, 29);
            this.ssvRotatePosition.TabIndex = 21;
            this.ssvRotatePosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvRotatePosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvRotatePosition.TextValue = "Value";
            this.ssvRotatePosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.LightGray;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(3, 178);
            this.label9.Margin = new System.Windows.Forms.Padding(3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(159, 29);
            this.label9.TabIndex = 25;
            this.label9.Text = "Slide";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssvSlidePosition
            // 
            this.ssvSlidePosition.AxisTag = null;
            this.ssvSlidePosition.BackColor = System.Drawing.Color.White;
            this.ssvSlidePosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvSlidePosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvSlidePosition.DecimalPoint = 4;
            this.ssvSlidePosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvSlidePosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvSlidePosition.Location = new System.Drawing.Point(168, 178);
            this.ssvSlidePosition.Name = "ssvSlidePosition";
            this.ssvSlidePosition.Size = new System.Drawing.Size(84, 29);
            this.ssvSlidePosition.TabIndex = 26;
            this.ssvSlidePosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvSlidePosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvSlidePosition.TextValue = "Value";
            this.ssvSlidePosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.LightGray;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(3, 213);
            this.label10.Margin = new System.Windows.Forms.Padding(3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(159, 29);
            this.label10.TabIndex = 30;
            this.label10.Text = "FrontAntiDrop";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssvFrontAntiDropPosition
            // 
            this.ssvFrontAntiDropPosition.AxisTag = null;
            this.ssvFrontAntiDropPosition.BackColor = System.Drawing.Color.White;
            this.ssvFrontAntiDropPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvFrontAntiDropPosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvFrontAntiDropPosition.DecimalPoint = 4;
            this.ssvFrontAntiDropPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvFrontAntiDropPosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvFrontAntiDropPosition.Location = new System.Drawing.Point(168, 213);
            this.ssvFrontAntiDropPosition.Name = "ssvFrontAntiDropPosition";
            this.ssvFrontAntiDropPosition.Size = new System.Drawing.Size(84, 29);
            this.ssvFrontAntiDropPosition.TabIndex = 31;
            this.ssvFrontAntiDropPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvFrontAntiDropPosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvFrontAntiDropPosition.TextValue = "Value";
            this.ssvFrontAntiDropPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.LightGray;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(3, 248);
            this.label11.Margin = new System.Windows.Forms.Padding(3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(159, 29);
            this.label11.TabIndex = 35;
            this.label11.Text = "RearAntiDrop";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssvRearAntiDropPosition
            // 
            this.ssvRearAntiDropPosition.AxisTag = null;
            this.ssvRearAntiDropPosition.BackColor = System.Drawing.Color.White;
            this.ssvRearAntiDropPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvRearAntiDropPosition.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ssvRearAntiDropPosition.DecimalPoint = 4;
            this.ssvRearAntiDropPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvRearAntiDropPosition.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvRearAntiDropPosition.Location = new System.Drawing.Point(168, 248);
            this.ssvRearAntiDropPosition.Name = "ssvRearAntiDropPosition";
            this.ssvRearAntiDropPosition.Size = new System.Drawing.Size(84, 29);
            this.ssvRearAntiDropPosition.TabIndex = 36;
            this.ssvRearAntiDropPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvRearAntiDropPosition.TextFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssvRearAntiDropPosition.TextValue = "Value";
            this.ssvRearAntiDropPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.Controls.Add(this.ssdRearAntiDropHomeDone, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.ssdRearAntiDropStatus, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.ssdFrontAntiDropHomeDone, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.ssdFrontAntiDropStatus, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlideHomeDone, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlideStatus, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.ssdRotateHomeDone, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.ssdRotateStatus, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.ssdHoistHomeDone, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.ssdHoistStatus, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.ssdMasterHomeDone, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.ssdMasterStatus, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlaveHomeDone, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlaveStatus, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.ssvRearAntiDropPosition, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.ssvFrontAntiDropPosition, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.ssvSlidePosition, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.ssvRotatePosition, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.ssvHoistPosition, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ssvMasterPosition, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbServoName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ssvSlavePosition, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(435, 297);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ssdRearAntiDropHomeDone
            // 
            this.ssdRearAntiDropHomeDone.AutoSize = true;
            this.ssdRearAntiDropHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdRearAntiDropHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRearAntiDropHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdRearAntiDropHomeDone.Location = new System.Drawing.Point(348, 248);
            this.ssdRearAntiDropHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdRearAntiDropHomeDone.Name = "ssdRearAntiDropHomeDone";
            this.ssdRearAntiDropHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdRearAntiDropHomeDone.TabIndex = 52;
            this.ssdRearAntiDropHomeDone.Text = "NG";
            this.ssdRearAntiDropHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdRearAntiDropHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdRearAntiDropStatus
            // 
            this.ssdRearAntiDropStatus.AutoSize = true;
            this.ssdRearAntiDropStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdRearAntiDropStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRearAntiDropStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdRearAntiDropStatus.Location = new System.Drawing.Point(258, 248);
            this.ssdRearAntiDropStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdRearAntiDropStatus.Name = "ssdRearAntiDropStatus";
            this.ssdRearAntiDropStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdRearAntiDropStatus.TabIndex = 51;
            this.ssdRearAntiDropStatus.Tag = "RearAntiDrop";
            this.ssdRearAntiDropStatus.Text = "ServoOff";
            this.ssdRearAntiDropStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdRearAntiDropStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdFrontAntiDropHomeDone
            // 
            this.ssdFrontAntiDropHomeDone.AutoSize = true;
            this.ssdFrontAntiDropHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdFrontAntiDropHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdFrontAntiDropHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdFrontAntiDropHomeDone.Location = new System.Drawing.Point(348, 213);
            this.ssdFrontAntiDropHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdFrontAntiDropHomeDone.Name = "ssdFrontAntiDropHomeDone";
            this.ssdFrontAntiDropHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdFrontAntiDropHomeDone.TabIndex = 50;
            this.ssdFrontAntiDropHomeDone.Text = "NG";
            this.ssdFrontAntiDropHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdFrontAntiDropHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdFrontAntiDropStatus
            // 
            this.ssdFrontAntiDropStatus.AutoSize = true;
            this.ssdFrontAntiDropStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdFrontAntiDropStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdFrontAntiDropStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdFrontAntiDropStatus.Location = new System.Drawing.Point(258, 213);
            this.ssdFrontAntiDropStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdFrontAntiDropStatus.Name = "ssdFrontAntiDropStatus";
            this.ssdFrontAntiDropStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdFrontAntiDropStatus.TabIndex = 49;
            this.ssdFrontAntiDropStatus.Tag = "FrontAntiDrop";
            this.ssdFrontAntiDropStatus.Text = "ServoOff";
            this.ssdFrontAntiDropStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdFrontAntiDropStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdSlideHomeDone
            // 
            this.ssdSlideHomeDone.AutoSize = true;
            this.ssdSlideHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlideHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlideHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdSlideHomeDone.Location = new System.Drawing.Point(348, 178);
            this.ssdSlideHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdSlideHomeDone.Name = "ssdSlideHomeDone";
            this.ssdSlideHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdSlideHomeDone.TabIndex = 48;
            this.ssdSlideHomeDone.Text = "NG";
            this.ssdSlideHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdSlideHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdSlideStatus
            // 
            this.ssdSlideStatus.AutoSize = true;
            this.ssdSlideStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlideStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlideStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdSlideStatus.Location = new System.Drawing.Point(258, 178);
            this.ssdSlideStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdSlideStatus.Name = "ssdSlideStatus";
            this.ssdSlideStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdSlideStatus.TabIndex = 47;
            this.ssdSlideStatus.Tag = "Slide";
            this.ssdSlideStatus.Text = "ServoOff";
            this.ssdSlideStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdSlideStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdRotateHomeDone
            // 
            this.ssdRotateHomeDone.AutoSize = true;
            this.ssdRotateHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdRotateHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRotateHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdRotateHomeDone.Location = new System.Drawing.Point(348, 143);
            this.ssdRotateHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdRotateHomeDone.Name = "ssdRotateHomeDone";
            this.ssdRotateHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdRotateHomeDone.TabIndex = 46;
            this.ssdRotateHomeDone.Text = "NG";
            this.ssdRotateHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdRotateHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdRotateStatus
            // 
            this.ssdRotateStatus.AutoSize = true;
            this.ssdRotateStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdRotateStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRotateStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdRotateStatus.Location = new System.Drawing.Point(258, 143);
            this.ssdRotateStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdRotateStatus.Name = "ssdRotateStatus";
            this.ssdRotateStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdRotateStatus.TabIndex = 45;
            this.ssdRotateStatus.Tag = "Rotate";
            this.ssdRotateStatus.Text = "ServoOff";
            this.ssdRotateStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdRotateStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdHoistHomeDone
            // 
            this.ssdHoistHomeDone.AutoSize = true;
            this.ssdHoistHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdHoistHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdHoistHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdHoistHomeDone.Location = new System.Drawing.Point(348, 108);
            this.ssdHoistHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdHoistHomeDone.Name = "ssdHoistHomeDone";
            this.ssdHoistHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdHoistHomeDone.TabIndex = 44;
            this.ssdHoistHomeDone.Text = "NG";
            this.ssdHoistHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdHoistHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdHoistStatus
            // 
            this.ssdHoistStatus.AutoSize = true;
            this.ssdHoistStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdHoistStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdHoistStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdHoistStatus.Location = new System.Drawing.Point(258, 108);
            this.ssdHoistStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdHoistStatus.Name = "ssdHoistStatus";
            this.ssdHoistStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdHoistStatus.TabIndex = 43;
            this.ssdHoistStatus.Tag = "Hoist";
            this.ssdHoistStatus.Text = "ServoOff";
            this.ssdHoistStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdHoistStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdMasterHomeDone
            // 
            this.ssdMasterHomeDone.AutoSize = true;
            this.ssdMasterHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdMasterHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdMasterHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdMasterHomeDone.Location = new System.Drawing.Point(348, 73);
            this.ssdMasterHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdMasterHomeDone.Name = "ssdMasterHomeDone";
            this.ssdMasterHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdMasterHomeDone.TabIndex = 42;
            this.ssdMasterHomeDone.Text = "NG";
            this.ssdMasterHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdMasterHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdMasterStatus
            // 
            this.ssdMasterStatus.AutoSize = true;
            this.ssdMasterStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdMasterStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdMasterStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdMasterStatus.Location = new System.Drawing.Point(258, 73);
            this.ssdMasterStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdMasterStatus.Name = "ssdMasterStatus";
            this.ssdMasterStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdMasterStatus.TabIndex = 41;
            this.ssdMasterStatus.Tag = "Master";
            this.ssdMasterStatus.Text = "ServoOff";
            this.ssdMasterStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdMasterStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdSlaveHomeDone
            // 
            this.ssdSlaveHomeDone.AutoSize = true;
            this.ssdSlaveHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlaveHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlaveHomeDone.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdSlaveHomeDone.Location = new System.Drawing.Point(348, 38);
            this.ssdSlaveHomeDone.Margin = new System.Windows.Forms.Padding(3);
            this.ssdSlaveHomeDone.Name = "ssdSlaveHomeDone";
            this.ssdSlaveHomeDone.Size = new System.Drawing.Size(84, 29);
            this.ssdSlaveHomeDone.TabIndex = 40;
            this.ssdSlaveHomeDone.Text = "NG";
            this.ssdSlaveHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdSlaveHomeDone.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            // 
            // ssdSlaveStatus
            // 
            this.ssdSlaveStatus.AutoSize = true;
            this.ssdSlaveStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlaveStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlaveStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ssdSlaveStatus.Location = new System.Drawing.Point(258, 38);
            this.ssdSlaveStatus.Margin = new System.Windows.Forms.Padding(3);
            this.ssdSlaveStatus.Name = "ssdSlaveStatus";
            this.ssdSlaveStatus.Size = new System.Drawing.Size(84, 29);
            this.ssdSlaveStatus.TabIndex = 39;
            this.ssdSlaveStatus.Tag = "Slave";
            this.ssdSlaveStatus.Text = "ServoOff";
            this.ssdSlaveStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssdSlaveStatus.TextChanged += new System.EventHandler(this.ssdSlaveStatus_TextChanged);
            this.ssdSlaveStatus.DoubleClick += new System.EventHandler(this.ssdSlaveStatus_DoubleClick);
            // 
            // ServoStates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ServoStates";
            this.Size = new System.Drawing.Size(435, 297);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Device.ucServoStatusValue ssvSlavePosition;
        private System.Windows.Forms.Label lbServoName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Device.ucServoStatusValue ssvMasterPosition;
        private System.Windows.Forms.Label label7;
        private Device.ucServoStatusValue ssvHoistPosition;
        private System.Windows.Forms.Label label8;
        private Device.ucServoStatusValue ssvRotatePosition;
        private System.Windows.Forms.Label label9;
        private Device.ucServoStatusValue ssvSlidePosition;
        private System.Windows.Forms.Label label10;
        private Device.ucServoStatusValue ssvFrontAntiDropPosition;
        private System.Windows.Forms.Label label11;
        private Device.ucServoStatusValue ssvRearAntiDropPosition;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label ssdRearAntiDropHomeDone;
        private System.Windows.Forms.Label ssdRearAntiDropStatus;
        private System.Windows.Forms.Label ssdFrontAntiDropHomeDone;
        private System.Windows.Forms.Label ssdFrontAntiDropStatus;
        private System.Windows.Forms.Label ssdSlideHomeDone;
        private System.Windows.Forms.Label ssdSlideStatus;
        private System.Windows.Forms.Label ssdRotateHomeDone;
        private System.Windows.Forms.Label ssdRotateStatus;
        private System.Windows.Forms.Label ssdHoistHomeDone;
        private System.Windows.Forms.Label ssdHoistStatus;
        private System.Windows.Forms.Label ssdMasterHomeDone;
        private System.Windows.Forms.Label ssdMasterStatus;
        private System.Windows.Forms.Label ssdSlaveHomeDone;
        private System.Windows.Forms.Label ssdSlaveStatus;
    }
}
