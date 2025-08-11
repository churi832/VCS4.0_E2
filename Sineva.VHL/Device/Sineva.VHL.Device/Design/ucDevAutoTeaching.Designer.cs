namespace Sineva.VHL.Device
{
    partial class ucDevAutoTeaching
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucDevAutoTeaching));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ucServoStatusValueMaster = new Sineva.VHL.Device.ucServoStatusValue();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ucServoStatusValueHoist = new Sineva.VHL.Device.ucServoStatusValue();
            this.ucServoStatusValueRotate = new Sineva.VHL.Device.ucServoStatusValue();
            this.ucServoStatusValueSlide = new Sineva.VHL.Device.ucServoStatusValue();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbEthernetConnection = new System.Windows.Forms.Label();
            this.lbPIOConnection = new System.Windows.Forms.Label();
            this.lbReflectiveSensor = new System.Windows.Forms.Label();
            this.lbLeftDoubleStorage = new System.Windows.Forms.Label();
            this.lbRightDoubleStorage = new System.Windows.Forms.Label();
            this.lbPIOGo = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbResultT = new System.Windows.Forms.Label();
            this.lbResultY = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbResultX = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lbOffsetMaster = new System.Windows.Forms.Label();
            this.lbOffsetHoist = new System.Windows.Forms.Label();
            this.lbOffsetSlide = new System.Windows.Forms.Label();
            this.lbOffsetRotate = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnVisionFind = new Sineva.VHL.Library.IButton();
            this.cbVisionDeviceEqp = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbOnlySensorOffsetFind = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lbLOhbOffsetDistance = new System.Windows.Forms.Label();
            this.lbPortOffsetDistance = new System.Windows.Forms.Label();
            this.lbEEIPConnection = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lbIpAddress = new System.Windows.Forms.Label();
            this.lbHeightSensor = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnVisionStop = new Sineva.VHL.Library.IButton();
            this.btnVisionStart = new Sineva.VHL.Library.IButton();
            this.cbVisionDeviceRightOHB = new System.Windows.Forms.CheckBox();
            this.cbVisionDeviceLeftOHB = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lbROhbOffsetDistance = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.ucServoStatusValueMaster, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label10, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label11, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.ucServoStatusValueHoist, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ucServoStatusValueRotate, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.ucServoStatusValueSlide, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 236);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(419, 55);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ucServoStatusValueMaster
            // 
            this.ucServoStatusValueMaster.AxisTag = ((Sineva.VHL.Library.Servo.AxisTag)(resources.GetObject("ucServoStatusValueMaster.AxisTag")));
            this.ucServoStatusValueMaster.BackColor = System.Drawing.Color.Transparent;
            this.ucServoStatusValueMaster.ColorOfBox = System.Drawing.Color.White;
            this.ucServoStatusValueMaster.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ucServoStatusValueMaster.DecimalPoint = 4;
            this.ucServoStatusValueMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucServoStatusValueMaster.Location = new System.Drawing.Point(10, 33);
            this.ucServoStatusValueMaster.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.ucServoStatusValueMaster.Name = "ucServoStatusValueMaster";
            this.ucServoStatusValueMaster.Size = new System.Drawing.Size(84, 19);
            this.ucServoStatusValueMaster.TabIndex = 7;
            this.ucServoStatusValueMaster.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ucServoStatusValueMaster.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ucServoStatusValueMaster.TextValue = "label1";
            this.ucServoStatusValueMaster.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label8.Location = new System.Drawing.Point(3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 30);
            this.label8.TabIndex = 4;
            this.label8.Text = "Master";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label9.Location = new System.Drawing.Point(107, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 30);
            this.label9.TabIndex = 4;
            this.label9.Text = "Hoist";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label10.Location = new System.Drawing.Point(211, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 30);
            this.label10.TabIndex = 4;
            this.label10.Text = "Slide";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label11.Location = new System.Drawing.Point(315, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(101, 30);
            this.label11.TabIndex = 4;
            this.label11.Text = "Rotate";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucServoStatusValueHoist
            // 
            this.ucServoStatusValueHoist.AxisTag = ((Sineva.VHL.Library.Servo.AxisTag)(resources.GetObject("ucServoStatusValueHoist.AxisTag")));
            this.ucServoStatusValueHoist.BackColor = System.Drawing.Color.Transparent;
            this.ucServoStatusValueHoist.ColorOfBox = System.Drawing.Color.White;
            this.ucServoStatusValueHoist.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ucServoStatusValueHoist.DecimalPoint = 4;
            this.ucServoStatusValueHoist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucServoStatusValueHoist.Location = new System.Drawing.Point(114, 33);
            this.ucServoStatusValueHoist.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.ucServoStatusValueHoist.Name = "ucServoStatusValueHoist";
            this.ucServoStatusValueHoist.Size = new System.Drawing.Size(84, 19);
            this.ucServoStatusValueHoist.TabIndex = 7;
            this.ucServoStatusValueHoist.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ucServoStatusValueHoist.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ucServoStatusValueHoist.TextValue = "label1";
            this.ucServoStatusValueHoist.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ucServoStatusValueRotate
            // 
            this.ucServoStatusValueRotate.AxisTag = ((Sineva.VHL.Library.Servo.AxisTag)(resources.GetObject("ucServoStatusValueRotate.AxisTag")));
            this.ucServoStatusValueRotate.BackColor = System.Drawing.Color.Transparent;
            this.ucServoStatusValueRotate.ColorOfBox = System.Drawing.Color.White;
            this.ucServoStatusValueRotate.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ucServoStatusValueRotate.DecimalPoint = 4;
            this.ucServoStatusValueRotate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucServoStatusValueRotate.Location = new System.Drawing.Point(322, 33);
            this.ucServoStatusValueRotate.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.ucServoStatusValueRotate.Name = "ucServoStatusValueRotate";
            this.ucServoStatusValueRotate.Size = new System.Drawing.Size(87, 19);
            this.ucServoStatusValueRotate.TabIndex = 7;
            this.ucServoStatusValueRotate.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ucServoStatusValueRotate.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ucServoStatusValueRotate.TextValue = "label1";
            this.ucServoStatusValueRotate.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ucServoStatusValueSlide
            // 
            this.ucServoStatusValueSlide.AxisTag = ((Sineva.VHL.Library.Servo.AxisTag)(resources.GetObject("ucServoStatusValueSlide.AxisTag")));
            this.ucServoStatusValueSlide.BackColor = System.Drawing.Color.Transparent;
            this.ucServoStatusValueSlide.ColorOfBox = System.Drawing.Color.White;
            this.ucServoStatusValueSlide.ColorOfText = System.Drawing.SystemColors.ControlText;
            this.ucServoStatusValueSlide.DecimalPoint = 4;
            this.ucServoStatusValueSlide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucServoStatusValueSlide.Location = new System.Drawing.Point(218, 33);
            this.ucServoStatusValueSlide.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.ucServoStatusValueSlide.Name = "ucServoStatusValueSlide";
            this.ucServoStatusValueSlide.Size = new System.Drawing.Size(84, 19);
            this.ucServoStatusValueSlide.TabIndex = 7;
            this.ucServoStatusValueSlide.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ucServoStatusValueSlide.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ucServoStatusValueSlide.TextValue = "label1";
            this.ucServoStatusValueSlide.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ethernet Connection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(8, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "PIO Connection";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(8, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Reflective Sensor";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(8, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Double Storage Right";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(267, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "GO Signal";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(8, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "Double Storage Left";
            // 
            // lbEthernetConnection
            // 
            this.lbEthernetConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbEthernetConnection.Location = new System.Drawing.Point(142, 17);
            this.lbEthernetConnection.Name = "lbEthernetConnection";
            this.lbEthernetConnection.Size = new System.Drawing.Size(100, 23);
            this.lbEthernetConnection.TabIndex = 3;
            this.lbEthernetConnection.Text = "--";
            this.lbEthernetConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPIOConnection
            // 
            this.lbPIOConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbPIOConnection.Location = new System.Drawing.Point(142, 48);
            this.lbPIOConnection.Name = "lbPIOConnection";
            this.lbPIOConnection.Size = new System.Drawing.Size(100, 23);
            this.lbPIOConnection.TabIndex = 4;
            this.lbPIOConnection.Text = "--";
            this.lbPIOConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbReflectiveSensor
            // 
            this.lbReflectiveSensor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbReflectiveSensor.Location = new System.Drawing.Point(142, 79);
            this.lbReflectiveSensor.Name = "lbReflectiveSensor";
            this.lbReflectiveSensor.Size = new System.Drawing.Size(100, 23);
            this.lbReflectiveSensor.TabIndex = 4;
            this.lbReflectiveSensor.Text = "--";
            this.lbReflectiveSensor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLeftDoubleStorage
            // 
            this.lbLeftDoubleStorage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbLeftDoubleStorage.Location = new System.Drawing.Point(142, 110);
            this.lbLeftDoubleStorage.Name = "lbLeftDoubleStorage";
            this.lbLeftDoubleStorage.Size = new System.Drawing.Size(100, 23);
            this.lbLeftDoubleStorage.TabIndex = 4;
            this.lbLeftDoubleStorage.Text = "--";
            this.lbLeftDoubleStorage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbRightDoubleStorage
            // 
            this.lbRightDoubleStorage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbRightDoubleStorage.Location = new System.Drawing.Point(142, 141);
            this.lbRightDoubleStorage.Name = "lbRightDoubleStorage";
            this.lbRightDoubleStorage.Size = new System.Drawing.Size(100, 23);
            this.lbRightDoubleStorage.TabIndex = 4;
            this.lbRightDoubleStorage.Text = "--";
            this.lbRightDoubleStorage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPIOGo
            // 
            this.lbPIOGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbPIOGo.Location = new System.Drawing.Point(337, 48);
            this.lbPIOGo.Name = "lbPIOGo";
            this.lbPIOGo.Size = new System.Drawing.Size(100, 23);
            this.lbPIOGo.TabIndex = 5;
            this.lbPIOGo.Text = "--";
            this.lbPIOGo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(8, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "Current Motor Position";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel2.Controls.Add(this.lbResultT, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.lbResultY, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label16, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label15, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label14, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbResultX, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(21, 26);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(419, 57);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // lbResultT
            // 
            this.lbResultT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbResultT.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbResultT.Location = new System.Drawing.Point(281, 30);
            this.lbResultT.Name = "lbResultT";
            this.lbResultT.Size = new System.Drawing.Size(135, 27);
            this.lbResultT.TabIndex = 10;
            this.lbResultT.Text = "0.0";
            this.lbResultT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbResultY
            // 
            this.lbResultY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbResultY.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbResultY.Location = new System.Drawing.Point(142, 30);
            this.lbResultY.Name = "lbResultY";
            this.lbResultY.Size = new System.Drawing.Size(133, 27);
            this.lbResultY.TabIndex = 9;
            this.lbResultY.Text = "0.0";
            this.lbResultY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label16.Location = new System.Drawing.Point(142, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(133, 30);
            this.label16.TabIndex = 7;
            this.label16.Text = "dy";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label15.Location = new System.Drawing.Point(281, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(135, 30);
            this.label15.TabIndex = 6;
            this.label15.Text = "dt";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label14.Location = new System.Drawing.Point(3, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(133, 30);
            this.label14.TabIndex = 5;
            this.label14.Text = "dx";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbResultX
            // 
            this.lbResultX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbResultX.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbResultX.Location = new System.Drawing.Point(3, 30);
            this.lbResultX.Name = "lbResultX";
            this.lbResultX.Size = new System.Drawing.Size(133, 27);
            this.lbResultX.TabIndex = 8;
            this.lbResultX.Text = "0.0";
            this.lbResultX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.lbOffsetMaster, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lbOffsetHoist, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lbOffsetSlide, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lbOffsetRotate, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label21, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.label20, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label19, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(21, 102);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(419, 56);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // lbOffsetMaster
            // 
            this.lbOffsetMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOffsetMaster.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbOffsetMaster.Location = new System.Drawing.Point(3, 30);
            this.lbOffsetMaster.Name = "lbOffsetMaster";
            this.lbOffsetMaster.Size = new System.Drawing.Size(98, 26);
            this.lbOffsetMaster.TabIndex = 12;
            this.lbOffsetMaster.Text = "0.0";
            this.lbOffsetMaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbOffsetHoist
            // 
            this.lbOffsetHoist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOffsetHoist.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbOffsetHoist.Location = new System.Drawing.Point(107, 30);
            this.lbOffsetHoist.Name = "lbOffsetHoist";
            this.lbOffsetHoist.Size = new System.Drawing.Size(98, 26);
            this.lbOffsetHoist.TabIndex = 11;
            this.lbOffsetHoist.Text = "0.0";
            this.lbOffsetHoist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbOffsetSlide
            // 
            this.lbOffsetSlide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOffsetSlide.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbOffsetSlide.Location = new System.Drawing.Point(211, 30);
            this.lbOffsetSlide.Name = "lbOffsetSlide";
            this.lbOffsetSlide.Size = new System.Drawing.Size(98, 26);
            this.lbOffsetSlide.TabIndex = 10;
            this.lbOffsetSlide.Text = "0.0";
            this.lbOffsetSlide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbOffsetRotate
            // 
            this.lbOffsetRotate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOffsetRotate.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbOffsetRotate.Location = new System.Drawing.Point(315, 30);
            this.lbOffsetRotate.Name = "lbOffsetRotate";
            this.lbOffsetRotate.Size = new System.Drawing.Size(101, 26);
            this.lbOffsetRotate.TabIndex = 9;
            this.lbOffsetRotate.Text = "0.0";
            this.lbOffsetRotate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label21.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label21.Location = new System.Drawing.Point(315, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(101, 30);
            this.label21.TabIndex = 8;
            this.label21.Text = "Rotate";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label20.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label20.Location = new System.Drawing.Point(211, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(98, 30);
            this.label20.TabIndex = 7;
            this.label20.Text = "Slide";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label19.Location = new System.Drawing.Point(107, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(98, 30);
            this.label19.TabIndex = 6;
            this.label19.Text = "Hoist";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label18.Location = new System.Drawing.Point(3, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(98, 30);
            this.label18.TabIndex = 5;
            this.label18.Text = "Master";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnVisionFind
            // 
            this.btnVisionFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnVisionFind.BackgroundImage")));
            this.btnVisionFind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVisionFind.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnVisionFind.BgDefault")));
            this.btnVisionFind.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnVisionFind.BgDisable")));
            this.btnVisionFind.BgOver = ((System.Drawing.Image)(resources.GetObject("btnVisionFind.BgOver")));
            this.btnVisionFind.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnVisionFind.BgPushed")));
            this.btnVisionFind.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnVisionFind.ConnectedLabel = null;
            this.btnVisionFind.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnVisionFind.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnVisionFind.DefaultImage = global::Sineva.VHL.Device.Properties.Resources.motor_stop;
            this.btnVisionFind.Description = null;
            this.btnVisionFind.DownImage = global::Sineva.VHL.Device.Properties.Resources.motor_stop;
            this.btnVisionFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisionFind.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVisionFind.ForeColor = System.Drawing.Color.White;
            this.btnVisionFind.Image = global::Sineva.VHL.Device.Properties.Resources.motor_stop;
            this.btnVisionFind.Location = new System.Drawing.Point(307, 18);
            this.btnVisionFind.Name = "btnVisionFind";
            this.btnVisionFind.OverImage = global::Sineva.VHL.Device.Properties.Resources.motor_stop;
            this.btnVisionFind.Size = new System.Drawing.Size(134, 71);
            this.btnVisionFind.TabIndex = 13;
            this.btnVisionFind.Text = "Vision Find";
            this.btnVisionFind.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnVisionFind.UpImage = global::Sineva.VHL.Device.Properties.Resources.motor_stop;
            this.btnVisionFind.UseOneImage = false;
            this.btnVisionFind.UseVisualStyleBackColor = true;
            this.btnVisionFind.Click += new System.EventHandler(this.btnVisionFind_Click);
            // 
            // cbVisionDeviceEqp
            // 
            this.cbVisionDeviceEqp.AutoSize = true;
            this.cbVisionDeviceEqp.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbVisionDeviceEqp.Location = new System.Drawing.Point(24, 31);
            this.cbVisionDeviceEqp.Name = "cbVisionDeviceEqp";
            this.cbVisionDeviceEqp.Size = new System.Drawing.Size(49, 16);
            this.cbVisionDeviceEqp.TabIndex = 14;
            this.cbVisionDeviceEqp.Text = "EQP";
            this.cbVisionDeviceEqp.UseVisualStyleBackColor = true;
            this.cbVisionDeviceEqp.CheckedChanged += new System.EventHandler(this.cbVisionDevice_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lbROhbOffsetDistance);
            this.groupBox1.Controls.Add(this.cbOnlySensorOffsetFind);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.lbLOhbOffsetDistance);
            this.groupBox1.Controls.Add(this.lbPortOffsetDistance);
            this.groupBox1.Controls.Add(this.lbEEIPConnection);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.lbIpAddress);
            this.groupBox1.Controls.Add(this.lbEthernetConnection);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lbPIOConnection);
            this.groupBox1.Controls.Add(this.lbReflectiveSensor);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Controls.Add(this.lbHeightSensor);
            this.groupBox1.Controls.Add(this.lbPIOGo);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lbLeftDoubleStorage);
            this.groupBox1.Controls.Add(this.lbRightDoubleStorage);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(3, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 302);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Vision Unit State";
            // 
            // cbOnlySensorOffsetFind
            // 
            this.cbOnlySensorOffsetFind.AutoSize = true;
            this.cbOnlySensorOffsetFind.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbOnlySensorOffsetFind.Location = new System.Drawing.Point(304, 208);
            this.cbOnlySensorOffsetFind.Name = "cbOnlySensorOffsetFind";
            this.cbOnlySensorOffsetFind.Size = new System.Drawing.Size(130, 16);
            this.cbOnlySensorOffsetFind.TabIndex = 15;
            this.cbOnlySensorOffsetFind.Text = "Only Sensor Offset";
            this.cbOnlySensorOffsetFind.UseVisualStyleBackColor = true;
            this.cbOnlySensorOffsetFind.CheckedChanged += new System.EventHandler(this.cbOnlySensorOffsetFind_CheckedChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label25.Location = new System.Drawing.Point(260, 114);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(69, 12);
            this.label25.TabIndex = 13;
            this.label25.Text = "lohb offset-";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label24.Location = new System.Drawing.Point(259, 84);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 12);
            this.label24.TabIndex = 13;
            this.label24.Text = "-port offset-";
            // 
            // lbLOhbOffsetDistance
            // 
            this.lbLOhbOffsetDistance.BackColor = System.Drawing.Color.White;
            this.lbLOhbOffsetDistance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbLOhbOffsetDistance.Location = new System.Drawing.Point(337, 109);
            this.lbLOhbOffsetDistance.Name = "lbLOhbOffsetDistance";
            this.lbLOhbOffsetDistance.Size = new System.Drawing.Size(101, 24);
            this.lbLOhbOffsetDistance.TabIndex = 11;
            this.lbLOhbOffsetDistance.Text = "0.0";
            this.lbLOhbOffsetDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPortOffsetDistance
            // 
            this.lbPortOffsetDistance.BackColor = System.Drawing.Color.White;
            this.lbPortOffsetDistance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbPortOffsetDistance.Location = new System.Drawing.Point(337, 78);
            this.lbPortOffsetDistance.Name = "lbPortOffsetDistance";
            this.lbPortOffsetDistance.Size = new System.Drawing.Size(101, 24);
            this.lbPortOffsetDistance.TabIndex = 10;
            this.lbPortOffsetDistance.Text = "0.0";
            this.lbPortOffsetDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbEEIPConnection
            // 
            this.lbEEIPConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbEEIPConnection.Location = new System.Drawing.Point(142, 172);
            this.lbEEIPConnection.Name = "lbEEIPConnection";
            this.lbEEIPConnection.Size = new System.Drawing.Size(100, 23);
            this.lbEEIPConnection.TabIndex = 9;
            this.lbEEIPConnection.Text = "--";
            this.lbEEIPConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label13.Location = new System.Drawing.Point(8, 178);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 12);
            this.label13.TabIndex = 8;
            this.label13.Text = "EEIP Connection";
            // 
            // lbIpAddress
            // 
            this.lbIpAddress.Location = new System.Drawing.Point(252, 18);
            this.lbIpAddress.Name = "lbIpAddress";
            this.lbIpAddress.Size = new System.Drawing.Size(197, 23);
            this.lbIpAddress.TabIndex = 7;
            this.lbIpAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbHeightSensor
            // 
            this.lbHeightSensor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.lbHeightSensor.Location = new System.Drawing.Point(337, 172);
            this.lbHeightSensor.Name = "lbHeightSensor";
            this.lbHeightSensor.Size = new System.Drawing.Size(100, 23);
            this.lbHeightSensor.TabIndex = 5;
            this.lbHeightSensor.Text = "--";
            this.lbHeightSensor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label17.Location = new System.Drawing.Point(267, 178);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 12);
            this.label17.TabIndex = 2;
            this.label17.Text = "Height";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnVisionStop);
            this.groupBox2.Controls.Add(this.btnVisionStart);
            this.groupBox2.Controls.Add(this.cbVisionDeviceRightOHB);
            this.groupBox2.Controls.Add(this.cbVisionDeviceLeftOHB);
            this.groupBox2.Controls.Add(this.cbVisionDeviceEqp);
            this.groupBox2.Controls.Add(this.btnVisionFind);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(3, 316);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(455, 100);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vision Find";
            // 
            // btnVisionStop
            // 
            this.btnVisionStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.BackgroundImage")));
            this.btnVisionStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVisionStop.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.BgDefault")));
            this.btnVisionStop.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.BgDisable")));
            this.btnVisionStop.BgOver = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.BgOver")));
            this.btnVisionStop.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.BgPushed")));
            this.btnVisionStop.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnVisionStop.ConnectedLabel = null;
            this.btnVisionStop.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnVisionStop.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnVisionStop.DefaultImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.DefaultImage")));
            this.btnVisionStop.Description = null;
            this.btnVisionStop.DownImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.DownImage")));
            this.btnVisionStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisionStop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVisionStop.ForeColor = System.Drawing.Color.White;
            this.btnVisionStop.Image = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.Image")));
            this.btnVisionStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVisionStop.Location = new System.Drawing.Point(130, 56);
            this.btnVisionStop.Name = "btnVisionStop";
            this.btnVisionStop.OverImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.OverImage")));
            this.btnVisionStop.Size = new System.Drawing.Size(98, 33);
            this.btnVisionStop.TabIndex = 15;
            this.btnVisionStop.Text = "Stop";
            this.btnVisionStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVisionStop.UpImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStop.UpImage")));
            this.btnVisionStop.UseOneImage = false;
            this.btnVisionStop.UseVisualStyleBackColor = true;
            this.btnVisionStop.Click += new System.EventHandler(this.btnVisionStop_Click);
            // 
            // btnVisionStart
            // 
            this.btnVisionStart.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.BackgroundImage")));
            this.btnVisionStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVisionStart.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.BgDefault")));
            this.btnVisionStart.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.BgDisable")));
            this.btnVisionStart.BgOver = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.BgOver")));
            this.btnVisionStart.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.BgPushed")));
            this.btnVisionStart.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnVisionStart.ConnectedLabel = null;
            this.btnVisionStart.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnVisionStart.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnVisionStart.DefaultImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.DefaultImage")));
            this.btnVisionStart.Description = null;
            this.btnVisionStart.DownImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.DownImage")));
            this.btnVisionStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisionStart.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVisionStart.ForeColor = System.Drawing.Color.White;
            this.btnVisionStart.Image = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.Image")));
            this.btnVisionStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVisionStart.Location = new System.Drawing.Point(24, 56);
            this.btnVisionStart.Name = "btnVisionStart";
            this.btnVisionStart.OverImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.OverImage")));
            this.btnVisionStart.Size = new System.Drawing.Size(98, 33);
            this.btnVisionStart.TabIndex = 15;
            this.btnVisionStart.Text = "Start";
            this.btnVisionStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVisionStart.UpImage = ((System.Drawing.Image)(resources.GetObject("btnVisionStart.UpImage")));
            this.btnVisionStart.UseOneImage = false;
            this.btnVisionStart.UseVisualStyleBackColor = true;
            this.btnVisionStart.Click += new System.EventHandler(this.btnVisionStart_Click);
            // 
            // cbVisionDeviceRightOHB
            // 
            this.cbVisionDeviceRightOHB.AutoSize = true;
            this.cbVisionDeviceRightOHB.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbVisionDeviceRightOHB.Location = new System.Drawing.Point(206, 31);
            this.cbVisionDeviceRightOHB.Name = "cbVisionDeviceRightOHB";
            this.cbVisionDeviceRightOHB.Size = new System.Drawing.Size(88, 16);
            this.cbVisionDeviceRightOHB.TabIndex = 14;
            this.cbVisionDeviceRightOHB.Text = "Right Buffer";
            this.cbVisionDeviceRightOHB.UseVisualStyleBackColor = true;
            this.cbVisionDeviceRightOHB.CheckedChanged += new System.EventHandler(this.cbVisionDevice_CheckedChanged);
            // 
            // cbVisionDeviceLeftOHB
            // 
            this.cbVisionDeviceLeftOHB.AutoSize = true;
            this.cbVisionDeviceLeftOHB.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.cbVisionDeviceLeftOHB.Location = new System.Drawing.Point(108, 31);
            this.cbVisionDeviceLeftOHB.Name = "cbVisionDeviceLeftOHB";
            this.cbVisionDeviceLeftOHB.Size = new System.Drawing.Size(80, 16);
            this.cbVisionDeviceLeftOHB.TabIndex = 14;
            this.cbVisionDeviceLeftOHB.Text = "Left Buffer";
            this.cbVisionDeviceLeftOHB.UseVisualStyleBackColor = true;
            this.cbVisionDeviceLeftOHB.CheckedChanged += new System.EventHandler(this.cbVisionDevice_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel2);
            this.groupBox3.Controls.Add(this.tableLayoutPanel3);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Location = new System.Drawing.Point(3, 422);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(455, 181);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Vision Result";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label12.Location = new System.Drawing.Point(260, 145);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 12);
            this.label12.TabIndex = 17;
            this.label12.Text = "rohb offset-";
            // 
            // lbROhbOffsetDistance
            // 
            this.lbROhbOffsetDistance.BackColor = System.Drawing.Color.White;
            this.lbROhbOffsetDistance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbROhbOffsetDistance.Location = new System.Drawing.Point(337, 140);
            this.lbROhbOffsetDistance.Name = "lbROhbOffsetDistance";
            this.lbROhbOffsetDistance.Size = new System.Drawing.Size(101, 24);
            this.lbROhbOffsetDistance.TabIndex = 16;
            this.lbROhbOffsetDistance.Text = "0.0";
            this.lbROhbOffsetDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ucDevAutoTeaching
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ucDevAutoTeaching";
            this.Size = new System.Drawing.Size(462, 611);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbEthernetConnection;
        private System.Windows.Forms.Label lbPIOConnection;
        private System.Windows.Forms.Label lbReflectiveSensor;
        private System.Windows.Forms.Label lbLeftDoubleStorage;
        private System.Windows.Forms.Label lbRightDoubleStorage;
        private System.Windows.Forms.Label lbPIOGo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private ucServoStatusValue ucServoStatusValueMaster;
        private ucServoStatusValue ucServoStatusValueHoist;
        private ucServoStatusValue ucServoStatusValueRotate;
        private ucServoStatusValue ucServoStatusValueSlide;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lbResultT;
        private System.Windows.Forms.Label lbResultY;
        private System.Windows.Forms.Label lbResultX;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lbOffsetMaster;
        private System.Windows.Forms.Label lbOffsetHoist;
        private System.Windows.Forms.Label lbOffsetSlide;
        private System.Windows.Forms.Label lbOffsetRotate;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private Library.IButton btnVisionFind;
        private System.Windows.Forms.CheckBox cbVisionDeviceEqp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbVisionDeviceRightOHB;
        private System.Windows.Forms.CheckBox cbVisionDeviceLeftOHB;
        private System.Windows.Forms.Label lbIpAddress;
        private Library.IButton btnVisionStop;
        private Library.IButton btnVisionStart;
        private System.Windows.Forms.Label lbEEIPConnection;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lbHeightSensor;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label lbLOhbOffsetDistance;
        private System.Windows.Forms.Label lbPortOffsetDistance;
        private System.Windows.Forms.CheckBox cbOnlySensorOffsetFind;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbROhbOffsetDistance;
    }
}
