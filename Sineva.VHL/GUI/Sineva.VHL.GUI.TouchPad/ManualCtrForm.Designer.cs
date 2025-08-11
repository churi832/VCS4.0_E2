namespace Sineva.VHL.GUI.TouchPad
{
    partial class ManualCtrForm
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
            this.btnServoOff = new System.Windows.Forms.Button();
            this.btnServoOn = new System.Windows.Forms.Button();
            this.btnVolecityHigh = new System.Windows.Forms.Button();
            this.btnVolecityMid = new System.Windows.Forms.Button();
            this.btnVolecitySlow = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnJogbtnJogT_right = new System.Windows.Forms.Button();
            this.btnJogbtnJogT_stop = new System.Windows.Forms.Button();
            this.btnJogbtnJogT_left = new System.Windows.Forms.Button();
            this.pictureVHL = new System.Windows.Forms.PictureBox();
            this.btnFrontSteerStatus = new System.Windows.Forms.Button();
            this.btnRearSteerStatus = new System.Windows.Forms.Button();
            this.btnGripperStatus = new System.Windows.Forms.Button();
            this.btnAntiDropStatus = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnServoHome = new System.Windows.Forms.Button();
            this.btnAlarmReset = new System.Windows.Forms.Button();
            this.servoStates1 = new Sineva.VHL.GUI.TouchPad.ServoStates();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVHL)).BeginInit();
            this.SuspendLayout();
            // 
            // btnServoOff
            // 
            this.btnServoOff.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnServoOff.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnServoOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnServoOff.FlatAppearance.BorderSize = 0;
            this.btnServoOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoOff.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnServoOff.ForeColor = System.Drawing.Color.White;
            this.btnServoOff.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.关机__1_;
            this.btnServoOff.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnServoOff.Location = new System.Drawing.Point(454, 595);
            this.btnServoOff.Name = "btnServoOff";
            this.btnServoOff.Size = new System.Drawing.Size(222, 130);
            this.btnServoOff.TabIndex = 106;
            this.btnServoOff.Text = "伺服关闭";
            this.btnServoOff.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnServoOff.UseVisualStyleBackColor = true;
            this.btnServoOff.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnServoOff.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // btnServoOn
            // 
            this.btnServoOn.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnServoOn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnServoOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnServoOn.FlatAppearance.BorderSize = 0;
            this.btnServoOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoOn.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnServoOn.ForeColor = System.Drawing.Color.White;
            this.btnServoOn.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.启动__4_;
            this.btnServoOn.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnServoOn.Location = new System.Drawing.Point(145, 595);
            this.btnServoOn.Name = "btnServoOn";
            this.btnServoOn.Size = new System.Drawing.Size(222, 130);
            this.btnServoOn.TabIndex = 105;
            this.btnServoOn.Text = "伺服启动";
            this.btnServoOn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnServoOn.UseVisualStyleBackColor = true;
            this.btnServoOn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnServoOn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // btnVolecityHigh
            // 
            this.btnVolecityHigh.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnVolecityHigh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVolecityHigh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolecityHigh.FlatAppearance.BorderSize = 0;
            this.btnVolecityHigh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolecityHigh.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnVolecityHigh.ForeColor = System.Drawing.Color.White;
            this.btnVolecityHigh.Location = new System.Drawing.Point(836, 853);
            this.btnVolecityHigh.Name = "btnVolecityHigh";
            this.btnVolecityHigh.Size = new System.Drawing.Size(193, 96);
            this.btnVolecityHigh.TabIndex = 102;
            this.btnVolecityHigh.Text = "快速";
            this.btnVolecityHigh.UseVisualStyleBackColor = true;
            this.btnVolecityHigh.Click += new System.EventHandler(this.btnVolecity_Click);
            // 
            // btnVolecityMid
            // 
            this.btnVolecityMid.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnVolecityMid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVolecityMid.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolecityMid.FlatAppearance.BorderSize = 0;
            this.btnVolecityMid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolecityMid.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnVolecityMid.ForeColor = System.Drawing.Color.White;
            this.btnVolecityMid.Location = new System.Drawing.Point(836, 715);
            this.btnVolecityMid.Name = "btnVolecityMid";
            this.btnVolecityMid.Size = new System.Drawing.Size(193, 96);
            this.btnVolecityMid.TabIndex = 101;
            this.btnVolecityMid.Text = "中速";
            this.btnVolecityMid.UseVisualStyleBackColor = true;
            this.btnVolecityMid.Click += new System.EventHandler(this.btnVolecity_Click);
            // 
            // btnVolecitySlow
            // 
            this.btnVolecitySlow.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnVolecitySlow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVolecitySlow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolecitySlow.FlatAppearance.BorderSize = 0;
            this.btnVolecitySlow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolecitySlow.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnVolecitySlow.ForeColor = System.Drawing.Color.White;
            this.btnVolecitySlow.Location = new System.Drawing.Point(836, 579);
            this.btnVolecitySlow.Name = "btnVolecitySlow";
            this.btnVolecitySlow.Size = new System.Drawing.Size(193, 96);
            this.btnVolecitySlow.TabIndex = 100;
            this.btnVolecitySlow.Text = "慢速";
            this.btnVolecitySlow.UseVisualStyleBackColor = true;
            this.btnVolecitySlow.Click += new System.EventHandler(this.btnVolecity_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(825, 476);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 62);
            this.label1.TabIndex = 99;
            this.label1.Text = "速度选择";
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.Yellow;
            this.button6.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.ForeColor = System.Drawing.Color.Black;
            this.button6.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_ccwp;
            this.button6.Location = new System.Drawing.Point(1659, 499);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(166, 133);
            this.button6.TabIndex = 98;
            this.button6.Text = "顺时针旋转";
            this.button6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button6.UseVisualStyleBackColor = false;
            this.button6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.button6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Yellow;
            this.button5.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.ForeColor = System.Drawing.Color.Black;
            this.button5.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_zp;
            this.button5.Location = new System.Drawing.Point(1233, 836);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(166, 133);
            this.button5.TabIndex = 97;
            this.button5.Text = "上升";
            this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button5.UseVisualStyleBackColor = false;
            this.button5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.button5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Yellow;
            this.button4.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_ccwm;
            this.button4.Location = new System.Drawing.Point(1233, 499);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(166, 133);
            this.button4.TabIndex = 96;
            this.button4.Text = "逆时针旋转";
            this.button4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Yellow;
            this.button3.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_ym;
            this.button3.Location = new System.Drawing.Point(1448, 834);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(166, 133);
            this.button3.TabIndex = 95;
            this.button3.Text = "前右侧伸出";
            this.button3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.button3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Yellow;
            this.button2.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_zm;
            this.button2.Location = new System.Drawing.Point(1659, 836);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(166, 133);
            this.button2.TabIndex = 94;
            this.button2.Text = "下降";
            this.button2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.button2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Yellow;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_yp;
            this.button1.Location = new System.Drawing.Point(1442, 499);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(166, 133);
            this.button1.TabIndex = 93;
            this.button1.Text = "前左侧伸出";
            this.button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogbtnJogT_right
            // 
            this.btnJogbtnJogT_right.BackColor = System.Drawing.Color.Yellow;
            this.btnJogbtnJogT_right.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogbtnJogT_right.ForeColor = System.Drawing.Color.Black;
            this.btnJogbtnJogT_right.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_xp;
            this.btnJogbtnJogT_right.Location = new System.Drawing.Point(1659, 673);
            this.btnJogbtnJogT_right.Name = "btnJogbtnJogT_right";
            this.btnJogbtnJogT_right.Size = new System.Drawing.Size(166, 133);
            this.btnJogbtnJogT_right.TabIndex = 91;
            this.btnJogbtnJogT_right.Text = "前进";
            this.btnJogbtnJogT_right.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogbtnJogT_right.UseVisualStyleBackColor = false;
            this.btnJogbtnJogT_right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogbtnJogT_right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogbtnJogT_stop
            // 
            this.btnJogbtnJogT_stop.BackColor = System.Drawing.Color.Yellow;
            this.btnJogbtnJogT_stop.Font = new System.Drawing.Font("微软雅黑", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogbtnJogT_stop.ForeColor = System.Drawing.Color.Black;
            this.btnJogbtnJogT_stop.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_stop;
            this.btnJogbtnJogT_stop.Location = new System.Drawing.Point(1448, 672);
            this.btnJogbtnJogT_stop.Name = "btnJogbtnJogT_stop";
            this.btnJogbtnJogT_stop.Size = new System.Drawing.Size(166, 133);
            this.btnJogbtnJogT_stop.TabIndex = 90;
            this.btnJogbtnJogT_stop.Text = "停止";
            this.btnJogbtnJogT_stop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogbtnJogT_stop.UseVisualStyleBackColor = false;
            this.btnJogbtnJogT_stop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogbtnJogT_stop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogbtnJogT_left
            // 
            this.btnJogbtnJogT_left.BackColor = System.Drawing.Color.Yellow;
            this.btnJogbtnJogT_left.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogbtnJogT_left.ForeColor = System.Drawing.Color.Black;
            this.btnJogbtnJogT_left.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_xm;
            this.btnJogbtnJogT_left.Location = new System.Drawing.Point(1233, 672);
            this.btnJogbtnJogT_left.Name = "btnJogbtnJogT_left";
            this.btnJogbtnJogT_left.Size = new System.Drawing.Size(166, 133);
            this.btnJogbtnJogT_left.TabIndex = 89;
            this.btnJogbtnJogT_left.Text = "后退";
            this.btnJogbtnJogT_left.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogbtnJogT_left.UseVisualStyleBackColor = false;
            this.btnJogbtnJogT_left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogbtnJogT_left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // pictureVHL
            // 
            this.pictureVHL.BackColor = System.Drawing.Color.Transparent;
            this.pictureVHL.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.VHL_NoFOUP;
            this.pictureVHL.Location = new System.Drawing.Point(1005, 23);
            this.pictureVHL.Name = "pictureVHL";
            this.pictureVHL.Size = new System.Drawing.Size(469, 419);
            this.pictureVHL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureVHL.TabIndex = 88;
            this.pictureVHL.TabStop = false;
            // 
            // btnFrontSteerStatus
            // 
            this.btnFrontSteerStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnFrontSteerStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontSteerStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFrontSteerStatus.FlatAppearance.BorderSize = 0;
            this.btnFrontSteerStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontSteerStatus.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFrontSteerStatus.ForeColor = System.Drawing.Color.White;
            this.btnFrontSteerStatus.Location = new System.Drawing.Point(764, 37);
            this.btnFrontSteerStatus.Name = "btnFrontSteerStatus";
            this.btnFrontSteerStatus.Size = new System.Drawing.Size(294, 74);
            this.btnFrontSteerStatus.TabIndex = 107;
            this.btnFrontSteerStatus.Text = "前转向轮方向：左";
            this.btnFrontSteerStatus.UseVisualStyleBackColor = true;
            // 
            // btnRearSteerStatus
            // 
            this.btnRearSteerStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnRearSteerStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearSteerStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRearSteerStatus.FlatAppearance.BorderSize = 0;
            this.btnRearSteerStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearSteerStatus.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRearSteerStatus.ForeColor = System.Drawing.Color.White;
            this.btnRearSteerStatus.Location = new System.Drawing.Point(1442, 41);
            this.btnRearSteerStatus.Name = "btnRearSteerStatus";
            this.btnRearSteerStatus.Size = new System.Drawing.Size(294, 74);
            this.btnRearSteerStatus.TabIndex = 108;
            this.btnRearSteerStatus.Text = "后转向轮方向：左";
            this.btnRearSteerStatus.UseVisualStyleBackColor = true;
            // 
            // btnGripperStatus
            // 
            this.btnGripperStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnGripperStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGripperStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGripperStatus.FlatAppearance.BorderSize = 0;
            this.btnGripperStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGripperStatus.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGripperStatus.ForeColor = System.Drawing.Color.White;
            this.btnGripperStatus.Location = new System.Drawing.Point(764, 238);
            this.btnGripperStatus.Name = "btnGripperStatus";
            this.btnGripperStatus.Size = new System.Drawing.Size(294, 74);
            this.btnGripperStatus.TabIndex = 109;
            this.btnGripperStatus.Text = "夹爪状态：开";
            this.btnGripperStatus.UseVisualStyleBackColor = true;
            // 
            // btnAntiDropStatus
            // 
            this.btnAntiDropStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnAntiDropStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAntiDropStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAntiDropStatus.FlatAppearance.BorderSize = 0;
            this.btnAntiDropStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAntiDropStatus.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAntiDropStatus.ForeColor = System.Drawing.Color.White;
            this.btnAntiDropStatus.Location = new System.Drawing.Point(1448, 351);
            this.btnAntiDropStatus.Name = "btnAntiDropStatus";
            this.btnAntiDropStatus.Size = new System.Drawing.Size(294, 74);
            this.btnAntiDropStatus.TabIndex = 110;
            this.btnAntiDropStatus.Text = "防坠落装置：关";
            this.btnAntiDropStatus.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Location = new System.Drawing.Point(1051, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 35);
            this.label3.TabIndex = 103;
            this.label3.Text = "前";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Location = new System.Drawing.Point(1391, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 35);
            this.label4.TabIndex = 104;
            this.label4.Text = "后";
            // 
            // btnServoHome
            // 
            this.btnServoHome.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnServoHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnServoHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnServoHome.FlatAppearance.BorderSize = 0;
            this.btnServoHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServoHome.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnServoHome.ForeColor = System.Drawing.Color.White;
            this.btnServoHome.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.逆时针;
            this.btnServoHome.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnServoHome.Location = new System.Drawing.Point(145, 797);
            this.btnServoHome.Name = "btnServoHome";
            this.btnServoHome.Size = new System.Drawing.Size(222, 130);
            this.btnServoHome.TabIndex = 112;
            this.btnServoHome.Text = "伺服复位";
            this.btnServoHome.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnServoHome.UseVisualStyleBackColor = true;
            this.btnServoHome.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnServoHome.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // btnAlarmReset
            // 
            this.btnAlarmReset.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnAlarmReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAlarmReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAlarmReset.FlatAppearance.BorderSize = 0;
            this.btnAlarmReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlarmReset.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAlarmReset.ForeColor = System.Drawing.Color.White;
            this.btnAlarmReset.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.逆时针;
            this.btnAlarmReset.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAlarmReset.Location = new System.Drawing.Point(454, 797);
            this.btnAlarmReset.Name = "btnAlarmReset";
            this.btnAlarmReset.Size = new System.Drawing.Size(222, 130);
            this.btnAlarmReset.TabIndex = 113;
            this.btnAlarmReset.Text = "报警复位";
            this.btnAlarmReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAlarmReset.UseVisualStyleBackColor = true;
            this.btnAlarmReset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnAlarmReset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // servoStates1
            // 
            this.servoStates1.BackColor = System.Drawing.Color.Transparent;
            this.servoStates1.Location = new System.Drawing.Point(130, 81);
            this.servoStates1.Name = "servoStates1";
            this.servoStates1.Size = new System.Drawing.Size(452, 344);
            this.servoStates1.TabIndex = 114;
            // 
            // ManualCtrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ClientSize = new System.Drawing.Size(1920, 1031);
            this.ControlBox = false;
            this.Controls.Add(this.servoStates1);
            this.Controls.Add(this.btnAlarmReset);
            this.Controls.Add(this.btnServoHome);
            this.Controls.Add(this.btnAntiDropStatus);
            this.Controls.Add(this.btnGripperStatus);
            this.Controls.Add(this.btnRearSteerStatus);
            this.Controls.Add(this.btnFrontSteerStatus);
            this.Controls.Add(this.btnServoOff);
            this.Controls.Add(this.btnServoOn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnVolecityHigh);
            this.Controls.Add(this.btnVolecityMid);
            this.Controls.Add(this.btnVolecitySlow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnJogbtnJogT_right);
            this.Controls.Add(this.btnJogbtnJogT_stop);
            this.Controls.Add(this.btnJogbtnJogT_left);
            this.Controls.Add(this.pictureVHL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ManualCtrForm";
            this.Text = "ManualCtrForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureVHL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnServoOff;
        private System.Windows.Forms.Button btnServoOn;
        private System.Windows.Forms.Button btnVolecityHigh;
        private System.Windows.Forms.Button btnVolecityMid;
        private System.Windows.Forms.Button btnVolecitySlow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnJogbtnJogT_right;
        private System.Windows.Forms.Button btnJogbtnJogT_stop;
        private System.Windows.Forms.Button btnJogbtnJogT_left;
        private System.Windows.Forms.PictureBox pictureVHL;
        private System.Windows.Forms.Button btnFrontSteerStatus;
        private System.Windows.Forms.Button btnRearSteerStatus;
        private System.Windows.Forms.Button btnGripperStatus;
        private System.Windows.Forms.Button btnAntiDropStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnServoHome;
        private System.Windows.Forms.Button btnAlarmReset;
        private ServoStates servoStates1;
    }
}