
namespace Sineva.VHL.GUI
{
    partial class ucMotorStateView
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
            this.ssvRearAntiDropPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.ssvFrontAntiDropPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.ssvSlidePosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.ssvRotatePosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.ssvHoistPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.ssvMasterPosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.ssvSlavePosition = new Sineva.VHL.Device.ucServoStatusValue();
            this.lbRearAntiDrop = new System.Windows.Forms.Label();
            this.ssdRearAntiDropStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.ssdRearAntiDropHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lbFrontAntiDrop = new System.Windows.Forms.Label();
            this.ssdFrontAntiDropStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.ssdFrontAntiDropHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lbSlide = new System.Windows.Forms.Label();
            this.ssdSlideStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.ssdSlideHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lbRotate = new System.Windows.Forms.Label();
            this.ssdRotateStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.ssdRotateHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lbHoist = new System.Windows.Forms.Label();
            this.ssdHoistStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.ssdHoistHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lvMaster = new System.Windows.Forms.Label();
            this.ssdMasterStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.ssdMasterHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lbSlave = new System.Windows.Forms.Label();
            this.ssdSlaveStatus = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.lbServoName = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbHomeDone = new System.Windows.Forms.Label();
            this.lbPosition = new System.Windows.Forms.Label();
            this.ssdSlaveHomeDone = new Sineva.VHL.Device.ucServoStatusDisplay();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssvRearAntiDropPosition
            // 
            this.ssvRearAntiDropPosition.AxisTag = null;
            this.ssvRearAntiDropPosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvRearAntiDropPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvRearAntiDropPosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvRearAntiDropPosition.DecimalPoint = 2;
            this.ssvRearAntiDropPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvRearAntiDropPosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvRearAntiDropPosition.Location = new System.Drawing.Point(150, 213);
            this.ssvRearAntiDropPosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvRearAntiDropPosition.Name = "ssvRearAntiDropPosition";
            this.ssvRearAntiDropPosition.Size = new System.Drawing.Size(78, 24);
            this.ssvRearAntiDropPosition.TabIndex = 79;
            this.ssvRearAntiDropPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvRearAntiDropPosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvRearAntiDropPosition.TextValue = "Value";
            this.ssvRearAntiDropPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ssvFrontAntiDropPosition
            // 
            this.ssvFrontAntiDropPosition.AxisTag = null;
            this.ssvFrontAntiDropPosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvFrontAntiDropPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvFrontAntiDropPosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvFrontAntiDropPosition.DecimalPoint = 2;
            this.ssvFrontAntiDropPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvFrontAntiDropPosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvFrontAntiDropPosition.Location = new System.Drawing.Point(150, 183);
            this.ssvFrontAntiDropPosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvFrontAntiDropPosition.Name = "ssvFrontAntiDropPosition";
            this.ssvFrontAntiDropPosition.Size = new System.Drawing.Size(78, 24);
            this.ssvFrontAntiDropPosition.TabIndex = 78;
            this.ssvFrontAntiDropPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvFrontAntiDropPosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvFrontAntiDropPosition.TextValue = "Value";
            this.ssvFrontAntiDropPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ssvSlidePosition
            // 
            this.ssvSlidePosition.AxisTag = null;
            this.ssvSlidePosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvSlidePosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvSlidePosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvSlidePosition.DecimalPoint = 2;
            this.ssvSlidePosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvSlidePosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvSlidePosition.Location = new System.Drawing.Point(150, 153);
            this.ssvSlidePosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvSlidePosition.Name = "ssvSlidePosition";
            this.ssvSlidePosition.Size = new System.Drawing.Size(78, 24);
            this.ssvSlidePosition.TabIndex = 77;
            this.ssvSlidePosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvSlidePosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvSlidePosition.TextValue = "Value";
            this.ssvSlidePosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ssvRotatePosition
            // 
            this.ssvRotatePosition.AxisTag = null;
            this.ssvRotatePosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvRotatePosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvRotatePosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvRotatePosition.DecimalPoint = 2;
            this.ssvRotatePosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvRotatePosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvRotatePosition.Location = new System.Drawing.Point(150, 123);
            this.ssvRotatePosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvRotatePosition.Name = "ssvRotatePosition";
            this.ssvRotatePosition.Size = new System.Drawing.Size(78, 24);
            this.ssvRotatePosition.TabIndex = 76;
            this.ssvRotatePosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvRotatePosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvRotatePosition.TextValue = "Value";
            this.ssvRotatePosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ssvHoistPosition
            // 
            this.ssvHoistPosition.AxisTag = null;
            this.ssvHoistPosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvHoistPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvHoistPosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvHoistPosition.DecimalPoint = 2;
            this.ssvHoistPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvHoistPosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvHoistPosition.Location = new System.Drawing.Point(150, 93);
            this.ssvHoistPosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvHoistPosition.Name = "ssvHoistPosition";
            this.ssvHoistPosition.Size = new System.Drawing.Size(78, 24);
            this.ssvHoistPosition.TabIndex = 75;
            this.ssvHoistPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvHoistPosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvHoistPosition.TextValue = "Value";
            this.ssvHoistPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ssvMasterPosition
            // 
            this.ssvMasterPosition.AxisTag = null;
            this.ssvMasterPosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvMasterPosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvMasterPosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvMasterPosition.DecimalPoint = 2;
            this.ssvMasterPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvMasterPosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvMasterPosition.Location = new System.Drawing.Point(150, 63);
            this.ssvMasterPosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvMasterPosition.Name = "ssvMasterPosition";
            this.ssvMasterPosition.Size = new System.Drawing.Size(78, 24);
            this.ssvMasterPosition.TabIndex = 74;
            this.ssvMasterPosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvMasterPosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvMasterPosition.TextValue = "Value";
            this.ssvMasterPosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // ssvSlavePosition
            // 
            this.ssvSlavePosition.AxisTag = null;
            this.ssvSlavePosition.BackColor = System.Drawing.Color.Transparent;
            this.ssvSlavePosition.ColorOfBox = System.Drawing.Color.White;
            this.ssvSlavePosition.ColorOfText = System.Drawing.SystemColors.WindowText;
            this.ssvSlavePosition.DecimalPoint = 2;
            this.ssvSlavePosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssvSlavePosition.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssvSlavePosition.Location = new System.Drawing.Point(150, 33);
            this.ssvSlavePosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssvSlavePosition.Name = "ssvSlavePosition";
            this.ssvSlavePosition.Size = new System.Drawing.Size(78, 24);
            this.ssvSlavePosition.TabIndex = 73;
            this.ssvSlavePosition.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ssvSlavePosition.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssvSlavePosition.TextValue = "Value";
            this.ssvSlavePosition.TypeOfValue = Sineva.VHL.Device.ucServoStatusValue.ValueType.Position;
            // 
            // lbRearAntiDrop
            // 
            this.lbRearAntiDrop.BackColor = System.Drawing.Color.LightGray;
            this.lbRearAntiDrop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRearAntiDrop.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRearAntiDrop.ForeColor = System.Drawing.Color.Black;
            this.lbRearAntiDrop.Location = new System.Drawing.Point(3, 213);
            this.lbRearAntiDrop.Margin = new System.Windows.Forms.Padding(3);
            this.lbRearAntiDrop.Name = "lbRearAntiDrop";
            this.lbRearAntiDrop.Size = new System.Drawing.Size(143, 24);
            this.lbRearAntiDrop.TabIndex = 72;
            this.lbRearAntiDrop.Text = "RearAntiDrop";
            this.lbRearAntiDrop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdRearAntiDropStatus
            // 
            this.ssdRearAntiDropStatus.AxisTag = null;
            this.ssdRearAntiDropStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdRearAntiDropStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdRearAntiDropStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdRearAntiDropStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRearAntiDropStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdRearAntiDropStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdRearAntiDropStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdRearAntiDropStatus.Location = new System.Drawing.Point(230, 213);
            this.ssdRearAntiDropStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdRearAntiDropStatus.Name = "ssdRearAntiDropStatus";
            this.ssdRearAntiDropStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdRearAntiDropStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdRearAntiDropStatus.TabIndex = 71;
            this.ssdRearAntiDropStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdRearAntiDropStatus.TextValue = "ServoOff";
            // 
            // ssdRearAntiDropHomeDone
            // 
            this.ssdRearAntiDropHomeDone.AxisTag = null;
            this.ssdRearAntiDropHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdRearAntiDropHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdRearAntiDropHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdRearAntiDropHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRearAntiDropHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdRearAntiDropHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdRearAntiDropHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdRearAntiDropHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdRearAntiDropHomeDone.Location = new System.Drawing.Point(310, 213);
            this.ssdRearAntiDropHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdRearAntiDropHomeDone.Name = "ssdRearAntiDropHomeDone";
            this.ssdRearAntiDropHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdRearAntiDropHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdRearAntiDropHomeDone.TabIndex = 70;
            this.ssdRearAntiDropHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdRearAntiDropHomeDone.TextValue = "NG";
            // 
            // lbFrontAntiDrop
            // 
            this.lbFrontAntiDrop.BackColor = System.Drawing.Color.LightGray;
            this.lbFrontAntiDrop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbFrontAntiDrop.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFrontAntiDrop.ForeColor = System.Drawing.Color.Black;
            this.lbFrontAntiDrop.Location = new System.Drawing.Point(3, 183);
            this.lbFrontAntiDrop.Margin = new System.Windows.Forms.Padding(3);
            this.lbFrontAntiDrop.Name = "lbFrontAntiDrop";
            this.lbFrontAntiDrop.Size = new System.Drawing.Size(143, 24);
            this.lbFrontAntiDrop.TabIndex = 69;
            this.lbFrontAntiDrop.Text = "FrontAntiDrop";
            this.lbFrontAntiDrop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdFrontAntiDropStatus
            // 
            this.ssdFrontAntiDropStatus.AxisTag = null;
            this.ssdFrontAntiDropStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdFrontAntiDropStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdFrontAntiDropStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdFrontAntiDropStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdFrontAntiDropStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdFrontAntiDropStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdFrontAntiDropStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdFrontAntiDropStatus.Location = new System.Drawing.Point(230, 183);
            this.ssdFrontAntiDropStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdFrontAntiDropStatus.Name = "ssdFrontAntiDropStatus";
            this.ssdFrontAntiDropStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdFrontAntiDropStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdFrontAntiDropStatus.TabIndex = 68;
            this.ssdFrontAntiDropStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdFrontAntiDropStatus.TextValue = "ServoOff";
            // 
            // ssdFrontAntiDropHomeDone
            // 
            this.ssdFrontAntiDropHomeDone.AxisTag = null;
            this.ssdFrontAntiDropHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdFrontAntiDropHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdFrontAntiDropHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdFrontAntiDropHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdFrontAntiDropHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdFrontAntiDropHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdFrontAntiDropHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdFrontAntiDropHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdFrontAntiDropHomeDone.Location = new System.Drawing.Point(310, 183);
            this.ssdFrontAntiDropHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdFrontAntiDropHomeDone.Name = "ssdFrontAntiDropHomeDone";
            this.ssdFrontAntiDropHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdFrontAntiDropHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdFrontAntiDropHomeDone.TabIndex = 67;
            this.ssdFrontAntiDropHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdFrontAntiDropHomeDone.TextValue = "NG";
            // 
            // lbSlide
            // 
            this.lbSlide.BackColor = System.Drawing.Color.LightGray;
            this.lbSlide.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSlide.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSlide.ForeColor = System.Drawing.Color.Black;
            this.lbSlide.Location = new System.Drawing.Point(3, 153);
            this.lbSlide.Margin = new System.Windows.Forms.Padding(3);
            this.lbSlide.Name = "lbSlide";
            this.lbSlide.Size = new System.Drawing.Size(143, 24);
            this.lbSlide.TabIndex = 66;
            this.lbSlide.Text = "Slide";
            this.lbSlide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdSlideStatus
            // 
            this.ssdSlideStatus.AxisTag = null;
            this.ssdSlideStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlideStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdSlideStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdSlideStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlideStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdSlideStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdSlideStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdSlideStatus.Location = new System.Drawing.Point(230, 153);
            this.ssdSlideStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdSlideStatus.Name = "ssdSlideStatus";
            this.ssdSlideStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdSlideStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdSlideStatus.TabIndex = 65;
            this.ssdSlideStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdSlideStatus.TextValue = "ServoOff";
            // 
            // ssdSlideHomeDone
            // 
            this.ssdSlideHomeDone.AxisTag = null;
            this.ssdSlideHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlideHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdSlideHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdSlideHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlideHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdSlideHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdSlideHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdSlideHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdSlideHomeDone.Location = new System.Drawing.Point(310, 153);
            this.ssdSlideHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdSlideHomeDone.Name = "ssdSlideHomeDone";
            this.ssdSlideHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdSlideHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdSlideHomeDone.TabIndex = 64;
            this.ssdSlideHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdSlideHomeDone.TextValue = "NG";
            // 
            // lbRotate
            // 
            this.lbRotate.BackColor = System.Drawing.Color.LightGray;
            this.lbRotate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRotate.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRotate.ForeColor = System.Drawing.Color.Black;
            this.lbRotate.Location = new System.Drawing.Point(3, 123);
            this.lbRotate.Margin = new System.Windows.Forms.Padding(3);
            this.lbRotate.Name = "lbRotate";
            this.lbRotate.Size = new System.Drawing.Size(143, 24);
            this.lbRotate.TabIndex = 63;
            this.lbRotate.Text = "Rotate";
            this.lbRotate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdRotateStatus
            // 
            this.ssdRotateStatus.AxisTag = null;
            this.ssdRotateStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdRotateStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdRotateStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdRotateStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRotateStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdRotateStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdRotateStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdRotateStatus.Location = new System.Drawing.Point(230, 123);
            this.ssdRotateStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdRotateStatus.Name = "ssdRotateStatus";
            this.ssdRotateStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdRotateStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdRotateStatus.TabIndex = 62;
            this.ssdRotateStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdRotateStatus.TextValue = "ServoOff";
            // 
            // ssdRotateHomeDone
            // 
            this.ssdRotateHomeDone.AxisTag = null;
            this.ssdRotateHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdRotateHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdRotateHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdRotateHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdRotateHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdRotateHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdRotateHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdRotateHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdRotateHomeDone.Location = new System.Drawing.Point(310, 123);
            this.ssdRotateHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdRotateHomeDone.Name = "ssdRotateHomeDone";
            this.ssdRotateHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdRotateHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdRotateHomeDone.TabIndex = 61;
            this.ssdRotateHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdRotateHomeDone.TextValue = "NG";
            // 
            // lbHoist
            // 
            this.lbHoist.BackColor = System.Drawing.Color.LightGray;
            this.lbHoist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbHoist.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHoist.ForeColor = System.Drawing.Color.Black;
            this.lbHoist.Location = new System.Drawing.Point(3, 93);
            this.lbHoist.Margin = new System.Windows.Forms.Padding(3);
            this.lbHoist.Name = "lbHoist";
            this.lbHoist.Size = new System.Drawing.Size(143, 24);
            this.lbHoist.TabIndex = 60;
            this.lbHoist.Text = "Hoist";
            this.lbHoist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdHoistStatus
            // 
            this.ssdHoistStatus.AxisTag = null;
            this.ssdHoistStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdHoistStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdHoistStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdHoistStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdHoistStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdHoistStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdHoistStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdHoistStatus.Location = new System.Drawing.Point(230, 93);
            this.ssdHoistStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdHoistStatus.Name = "ssdHoistStatus";
            this.ssdHoistStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdHoistStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdHoistStatus.TabIndex = 59;
            this.ssdHoistStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdHoistStatus.TextValue = "ServoOff";
            // 
            // ssdHoistHomeDone
            // 
            this.ssdHoistHomeDone.AxisTag = null;
            this.ssdHoistHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdHoistHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdHoistHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdHoistHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdHoistHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdHoistHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdHoistHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdHoistHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdHoistHomeDone.Location = new System.Drawing.Point(310, 93);
            this.ssdHoistHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdHoistHomeDone.Name = "ssdHoistHomeDone";
            this.ssdHoistHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdHoistHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdHoistHomeDone.TabIndex = 58;
            this.ssdHoistHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdHoistHomeDone.TextValue = "NG";
            // 
            // lvMaster
            // 
            this.lvMaster.BackColor = System.Drawing.Color.LightGray;
            this.lvMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMaster.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvMaster.ForeColor = System.Drawing.Color.Black;
            this.lvMaster.Location = new System.Drawing.Point(3, 63);
            this.lvMaster.Margin = new System.Windows.Forms.Padding(3);
            this.lvMaster.Name = "lvMaster";
            this.lvMaster.Size = new System.Drawing.Size(143, 24);
            this.lvMaster.TabIndex = 57;
            this.lvMaster.Text = "Master";
            this.lvMaster.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdMasterStatus
            // 
            this.ssdMasterStatus.AxisTag = null;
            this.ssdMasterStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdMasterStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdMasterStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdMasterStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdMasterStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdMasterStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdMasterStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdMasterStatus.Location = new System.Drawing.Point(230, 63);
            this.ssdMasterStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdMasterStatus.Name = "ssdMasterStatus";
            this.ssdMasterStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdMasterStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdMasterStatus.TabIndex = 56;
            this.ssdMasterStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdMasterStatus.TextValue = "ServoOff";
            // 
            // ssdMasterHomeDone
            // 
            this.ssdMasterHomeDone.AxisTag = null;
            this.ssdMasterHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdMasterHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdMasterHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdMasterHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdMasterHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdMasterHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdMasterHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdMasterHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdMasterHomeDone.Location = new System.Drawing.Point(310, 63);
            this.ssdMasterHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdMasterHomeDone.Name = "ssdMasterHomeDone";
            this.ssdMasterHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdMasterHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdMasterHomeDone.TabIndex = 55;
            this.ssdMasterHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdMasterHomeDone.TextValue = "NG";
            // 
            // lbSlave
            // 
            this.lbSlave.BackColor = System.Drawing.Color.LightGray;
            this.lbSlave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSlave.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSlave.ForeColor = System.Drawing.Color.Black;
            this.lbSlave.Location = new System.Drawing.Point(3, 33);
            this.lbSlave.Margin = new System.Windows.Forms.Padding(3);
            this.lbSlave.Name = "lbSlave";
            this.lbSlave.Size = new System.Drawing.Size(143, 24);
            this.lbSlave.TabIndex = 54;
            this.lbSlave.Text = "Slave";
            this.lbSlave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdSlaveStatus
            // 
            this.ssdSlaveStatus.AxisTag = null;
            this.ssdSlaveStatus.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlaveStatus.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdSlaveStatus.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdSlaveStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlaveStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdSlaveStatus.ForeColorOfOfSignalOff = System.Drawing.Color.Black;
            this.ssdSlaveStatus.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdSlaveStatus.Location = new System.Drawing.Point(230, 33);
            this.ssdSlaveStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdSlaveStatus.Name = "ssdSlaveStatus";
            this.ssdSlaveStatus.Size = new System.Drawing.Size(78, 24);
            this.ssdSlaveStatus.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.ServoOnOff;
            this.ssdSlaveStatus.TabIndex = 53;
            this.ssdSlaveStatus.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdSlaveStatus.TextValue = "ServoOff";
            // 
            // lbServoName
            // 
            this.lbServoName.BackColor = System.Drawing.Color.LightGray;
            this.lbServoName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbServoName.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbServoName.ForeColor = System.Drawing.Color.Black;
            this.lbServoName.Location = new System.Drawing.Point(3, 3);
            this.lbServoName.Margin = new System.Windows.Forms.Padding(3);
            this.lbServoName.Name = "lbServoName";
            this.lbServoName.Size = new System.Drawing.Size(143, 24);
            this.lbServoName.TabIndex = 52;
            this.lbServoName.Text = "ServoName";
            this.lbServoName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStatus
            // 
            this.lbStatus.BackColor = System.Drawing.Color.LightGray;
            this.lbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbStatus.ForeColor = System.Drawing.Color.Black;
            this.lbStatus.Location = new System.Drawing.Point(230, 3);
            this.lbStatus.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(78, 24);
            this.lbStatus.TabIndex = 51;
            this.lbStatus.Text = "Status";
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbHomeDone
            // 
            this.lbHomeDone.BackColor = System.Drawing.Color.LightGray;
            this.lbHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbHomeDone.ForeColor = System.Drawing.Color.Black;
            this.lbHomeDone.Location = new System.Drawing.Point(310, 3);
            this.lbHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.lbHomeDone.Name = "lbHomeDone";
            this.lbHomeDone.Size = new System.Drawing.Size(78, 24);
            this.lbHomeDone.TabIndex = 50;
            this.lbHomeDone.Text = "HomeDone";
            this.lbHomeDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPosition
            // 
            this.lbPosition.BackColor = System.Drawing.Color.LightGray;
            this.lbPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPosition.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbPosition.ForeColor = System.Drawing.Color.Black;
            this.lbPosition.Location = new System.Drawing.Point(150, 3);
            this.lbPosition.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.lbPosition.Name = "lbPosition";
            this.lbPosition.Size = new System.Drawing.Size(78, 24);
            this.lbPosition.TabIndex = 49;
            this.lbPosition.Text = "Position";
            this.lbPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ssdSlaveHomeDone
            // 
            this.ssdSlaveHomeDone.AxisTag = null;
            this.ssdSlaveHomeDone.BackColor = System.Drawing.Color.Yellow;
            this.ssdSlaveHomeDone.BackColorOfSignalOff = System.Drawing.Color.Yellow;
            this.ssdSlaveHomeDone.BackColorOfSignalOn = System.Drawing.Color.Lime;
            this.ssdSlaveHomeDone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssdSlaveHomeDone.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ssdSlaveHomeDone.ForeColor = System.Drawing.Color.Red;
            this.ssdSlaveHomeDone.ForeColorOfOfSignalOff = System.Drawing.Color.Red;
            this.ssdSlaveHomeDone.ForeColorOfOfSignalOn = System.Drawing.Color.Black;
            this.ssdSlaveHomeDone.Location = new System.Drawing.Point(310, 33);
            this.ssdSlaveHomeDone.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.ssdSlaveHomeDone.Name = "ssdSlaveHomeDone";
            this.ssdSlaveHomeDone.Size = new System.Drawing.Size(78, 24);
            this.ssdSlaveHomeDone.StatusType1 = Sineva.VHL.Device.ucServoStatusDisplay.StatusType.HomeDone;
            this.ssdSlaveHomeDone.TabIndex = 48;
            this.ssdSlaveHomeDone.TextFont = new System.Drawing.Font("Arial", 9F);
            this.ssdSlaveHomeDone.TextValue = "NG";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Controls.Add(this.lbServoName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ssdRearAntiDropHomeDone, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.ssdRearAntiDropStatus, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.ssvRearAntiDropPosition, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.lbSlave, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ssvFrontAntiDropPosition, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.lvMaster, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.ssdFrontAntiDropHomeDone, 3, 6);
            this.tableLayoutPanel1.Controls.Add(this.ssdFrontAntiDropStatus, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.ssvSlidePosition, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lbHoist, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ssvRotatePosition, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbRotate, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlideHomeDone, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlideStatus, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.ssvHoistPosition, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbSlide, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.ssvMasterPosition, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbFrontAntiDrop, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.ssdRotateHomeDone, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.ssdRotateStatus, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.ssvSlavePosition, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbRearAntiDrop, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.lbPosition, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbStatus, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ssdHoistHomeDone, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.ssdHoistStatus, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbHomeDone, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlaveStatus, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.ssdSlaveHomeDone, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.ssdMasterStatus, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.ssdMasterHomeDone, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(389, 253);
            this.tableLayoutPanel1.TabIndex = 80;
            // 
            // ucMotorStateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucMotorStateView";
            this.Size = new System.Drawing.Size(389, 253);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Device.ucServoStatusValue ssvRearAntiDropPosition;
        private Device.ucServoStatusValue ssvFrontAntiDropPosition;
        private Device.ucServoStatusValue ssvSlidePosition;
        private Device.ucServoStatusValue ssvRotatePosition;
        private Device.ucServoStatusValue ssvHoistPosition;
        private Device.ucServoStatusValue ssvMasterPosition;
        private Device.ucServoStatusValue ssvSlavePosition;
        private System.Windows.Forms.Label lbRearAntiDrop;
        private Device.ucServoStatusDisplay ssdRearAntiDropStatus;
        private Device.ucServoStatusDisplay ssdRearAntiDropHomeDone;
        private System.Windows.Forms.Label lbFrontAntiDrop;
        private Device.ucServoStatusDisplay ssdFrontAntiDropStatus;
        private Device.ucServoStatusDisplay ssdFrontAntiDropHomeDone;
        private System.Windows.Forms.Label lbSlide;
        private Device.ucServoStatusDisplay ssdSlideStatus;
        private Device.ucServoStatusDisplay ssdSlideHomeDone;
        private System.Windows.Forms.Label lbRotate;
        private Device.ucServoStatusDisplay ssdRotateStatus;
        private Device.ucServoStatusDisplay ssdRotateHomeDone;
        private System.Windows.Forms.Label lbHoist;
        private Device.ucServoStatusDisplay ssdHoistStatus;
        private Device.ucServoStatusDisplay ssdHoistHomeDone;
        private System.Windows.Forms.Label lvMaster;
        private Device.ucServoStatusDisplay ssdMasterStatus;
        private Device.ucServoStatusDisplay ssdMasterHomeDone;
        private System.Windows.Forms.Label lbSlave;
        private Device.ucServoStatusDisplay ssdSlaveStatus;
        private System.Windows.Forms.Label lbServoName;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbHomeDone;
        private System.Windows.Forms.Label lbPosition;
        private Device.ucServoStatusDisplay ssdSlaveHomeDone;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
