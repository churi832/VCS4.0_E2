using Sineva.VHL.GUI.TouchPad.Properties;
using System;

namespace Sineva.VHL.GUI.TouchPad
{
    partial class MainForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timerDateUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnReady = new System.Windows.Forms.Button();
            this.btnFrontAntiDropStatus = new System.Windows.Forms.Button();
            this.btnGripperStatus = new System.Windows.Forms.Button();
            this.btnRearSteerStatus = new System.Windows.Forms.Button();
            this.btnFrontSteerStatus = new System.Windows.Forms.Button();
            this.btnEqpState = new System.Windows.Forms.Button();
            this.btnMode = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnVolecityHigh = new System.Windows.Forms.Button();
            this.btnVolecityMid = new System.Windows.Forms.Button();
            this.btnVolecitySlow = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnJogRotateMinus = new System.Windows.Forms.Button();
            this.btnJogHoistPlus = new System.Windows.Forms.Button();
            this.btnJogRotatePlus = new System.Windows.Forms.Button();
            this.btnJogSlideMinus = new System.Windows.Forms.Button();
            this.btnJogHoistMinus = new System.Windows.Forms.Button();
            this.btnJogSlidePlus = new System.Windows.Forms.Button();
            this.btnJogTransferPlus = new System.Windows.Forms.Button();
            this.btnJogStop = new System.Windows.Forms.Button();
            this.btnJogTransferMiuns = new System.Windows.Forms.Button();
            this.pictureVHL = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.timerStatesUpdate = new System.Windows.Forms.Timer(this.components);
            this.lbConnectionStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRearAntiDropStatus = new System.Windows.Forms.Button();
            this.servoStates1 = new Sineva.VHL.GUI.TouchPad.ServoStates();
            this.ucTitle1 = new Sineva.VHL.GUI.TouchPad.ucTitle();
            ((System.ComponentModel.ISupportInitialize)(this.pictureVHL)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(40, 46);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timerDateUpdate
            // 
            this.timerDateUpdate.Enabled = true;
            this.timerDateUpdate.Interval = 1000;
            this.timerDateUpdate.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnAbort
            // 
            this.btnAbort.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnAbort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAbort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbort.FlatAppearance.BorderSize = 0;
            this.btnAbort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbort.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAbort.ForeColor = System.Drawing.Color.White;
            this.btnAbort.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_abort_default;
            this.btnAbort.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAbort.Location = new System.Drawing.Point(291, 690);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(167, 104);
            this.btnAbort.TabIndex = 138;
            this.btnAbort.Text = "终止";
            this.btnAbort.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnAbort_MouseClick);
            this.btnAbort.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnAbort.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // btnReady
            // 
            this.btnReady.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnReady.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReady.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReady.FlatAppearance.BorderSize = 0;
            this.btnReady.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReady.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReady.ForeColor = System.Drawing.Color.White;
            this.btnReady.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_ready_default;
            this.btnReady.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReady.Location = new System.Drawing.Point(72, 690);
            this.btnReady.Name = "btnReady";
            this.btnReady.Size = new System.Drawing.Size(167, 104);
            this.btnReady.TabIndex = 137;
            this.btnReady.Text = "初始化";
            this.btnReady.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReady.UseVisualStyleBackColor = true;
            this.btnReady.Click += new System.EventHandler(this.btnReady_Click);
            this.btnReady.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnReady.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // btnFrontAntiDropStatus
            // 
            this.btnFrontAntiDropStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnFrontAntiDropStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontAntiDropStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFrontAntiDropStatus.FlatAppearance.BorderSize = 0;
            this.btnFrontAntiDropStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontAntiDropStatus.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFrontAntiDropStatus.ForeColor = System.Drawing.Color.White;
            this.btnFrontAntiDropStatus.Location = new System.Drawing.Point(1120, 312);
            this.btnFrontAntiDropStatus.Name = "btnFrontAntiDropStatus";
            this.btnFrontAntiDropStatus.Size = new System.Drawing.Size(122, 51);
            this.btnFrontAntiDropStatus.TabIndex = 136;
            this.btnFrontAntiDropStatus.Tag = "control";
            this.btnFrontAntiDropStatus.Text = "UnLock";
            this.btnFrontAntiDropStatus.UseVisualStyleBackColor = true;
            this.btnFrontAntiDropStatus.TextChanged += new System.EventHandler(this.btnAntiDropStatus_TextChanged);
            // 
            // btnGripperStatus
            // 
            this.btnGripperStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnGripperStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGripperStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGripperStatus.FlatAppearance.BorderSize = 0;
            this.btnGripperStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGripperStatus.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGripperStatus.ForeColor = System.Drawing.Color.White;
            this.btnGripperStatus.Location = new System.Drawing.Point(1120, 239);
            this.btnGripperStatus.Name = "btnGripperStatus";
            this.btnGripperStatus.Size = new System.Drawing.Size(122, 51);
            this.btnGripperStatus.TabIndex = 135;
            this.btnGripperStatus.Tag = "control";
            this.btnGripperStatus.Text = "开";
            this.btnGripperStatus.UseVisualStyleBackColor = true;
            this.btnGripperStatus.TextChanged += new System.EventHandler(this.btnGripperStatus_TextChanged);
            // 
            // btnRearSteerStatus
            // 
            this.btnRearSteerStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnRearSteerStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearSteerStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRearSteerStatus.FlatAppearance.BorderSize = 0;
            this.btnRearSteerStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearSteerStatus.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRearSteerStatus.ForeColor = System.Drawing.Color.White;
            this.btnRearSteerStatus.Location = new System.Drawing.Point(1120, 166);
            this.btnRearSteerStatus.Name = "btnRearSteerStatus";
            this.btnRearSteerStatus.Size = new System.Drawing.Size(122, 51);
            this.btnRearSteerStatus.TabIndex = 134;
            this.btnRearSteerStatus.Tag = "control";
            this.btnRearSteerStatus.Text = "左";
            this.btnRearSteerStatus.UseVisualStyleBackColor = true;
            this.btnRearSteerStatus.TextChanged += new System.EventHandler(this.btnSteerStatus_TextChanged);
            this.btnRearSteerStatus.Click += new System.EventHandler(this.btnRearSteerStatus_Click);
            // 
            // btnFrontSteerStatus
            // 
            this.btnFrontSteerStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnFrontSteerStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFrontSteerStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFrontSteerStatus.FlatAppearance.BorderSize = 0;
            this.btnFrontSteerStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFrontSteerStatus.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFrontSteerStatus.ForeColor = System.Drawing.Color.White;
            this.btnFrontSteerStatus.Location = new System.Drawing.Point(1120, 93);
            this.btnFrontSteerStatus.Name = "btnFrontSteerStatus";
            this.btnFrontSteerStatus.Size = new System.Drawing.Size(122, 51);
            this.btnFrontSteerStatus.TabIndex = 133;
            this.btnFrontSteerStatus.Tag = "control";
            this.btnFrontSteerStatus.Text = "左";
            this.btnFrontSteerStatus.UseVisualStyleBackColor = true;
            this.btnFrontSteerStatus.TextChanged += new System.EventHandler(this.btnSteerStatus_TextChanged);
            this.btnFrontSteerStatus.Click += new System.EventHandler(this.btnFrontSteerStatus_Click);
            // 
            // btnEqpState
            // 
            this.btnEqpState.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnEqpState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEqpState.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEqpState.FlatAppearance.BorderSize = 0;
            this.btnEqpState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqpState.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnEqpState.ForeColor = System.Drawing.Color.White;
            this.btnEqpState.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_stop_default;
            this.btnEqpState.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEqpState.Location = new System.Drawing.Point(293, 507);
            this.btnEqpState.Name = "btnEqpState";
            this.btnEqpState.Size = new System.Drawing.Size(167, 104);
            this.btnEqpState.TabIndex = 132;
            this.btnEqpState.Text = "循环停止";
            this.btnEqpState.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEqpState.UseVisualStyleBackColor = true;
            this.btnEqpState.Click += new System.EventHandler(this.btnEqpState_Click);
            this.btnEqpState.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnEqpState.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // btnMode
            // 
            this.btnMode.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMode.FlatAppearance.BorderSize = 0;
            this.btnMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMode.ForeColor = System.Drawing.Color.White;
            this.btnMode.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.right_manual_default;
            this.btnMode.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMode.Location = new System.Drawing.Point(74, 507);
            this.btnMode.Name = "btnMode";
            this.btnMode.Size = new System.Drawing.Size(167, 104);
            this.btnMode.TabIndex = 131;
            this.btnMode.Text = "手动模式";
            this.btnMode.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnMode.UseVisualStyleBackColor = true;
            this.btnMode.Click += new System.EventHandler(this.btnMode_Click);
            this.btnMode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseDown);
            this.btnMode.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnServo_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Location = new System.Drawing.Point(862, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 35);
            this.label4.TabIndex = 130;
            this.label4.Text = "后";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Location = new System.Drawing.Point(541, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 35);
            this.label3.TabIndex = 129;
            this.label3.Text = "前";
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
            this.btnVolecityHigh.Location = new System.Drawing.Point(581, 720);
            this.btnVolecityHigh.Name = "btnVolecityHigh";
            this.btnVolecityHigh.Size = new System.Drawing.Size(177, 83);
            this.btnVolecityHigh.TabIndex = 128;
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
            this.btnVolecityMid.Location = new System.Drawing.Point(581, 624);
            this.btnVolecityMid.Name = "btnVolecityMid";
            this.btnVolecityMid.Size = new System.Drawing.Size(177, 83);
            this.btnVolecityMid.TabIndex = 127;
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
            this.btnVolecitySlow.Location = new System.Drawing.Point(581, 528);
            this.btnVolecitySlow.Name = "btnVolecitySlow";
            this.btnVolecitySlow.Size = new System.Drawing.Size(177, 83);
            this.btnVolecitySlow.TabIndex = 126;
            this.btnVolecitySlow.Text = "慢速";
            this.btnVolecitySlow.UseVisualStyleBackColor = true;
            this.btnVolecitySlow.Click += new System.EventHandler(this.btnVolecity_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(598, 471);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 46);
            this.label2.TabIndex = 125;
            this.label2.Text = "速度选择";
            this.label2.Click += new System.EventHandler(this.btnVolecity_Click);
            // 
            // btnJogRotateMinus
            // 
            this.btnJogRotateMinus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogRotateMinus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogRotateMinus.ForeColor = System.Drawing.Color.Black;
            this.btnJogRotateMinus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_ccwp;
            this.btnJogRotateMinus.Location = new System.Drawing.Point(1136, 462);
            this.btnJogRotateMinus.Name = "btnJogRotateMinus";
            this.btnJogRotateMinus.Size = new System.Drawing.Size(117, 102);
            this.btnJogRotateMinus.TabIndex = 124;
            this.btnJogRotateMinus.Tag = "control";
            this.btnJogRotateMinus.Text = "顺时针旋转";
            this.btnJogRotateMinus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogRotateMinus.UseVisualStyleBackColor = false;
            this.btnJogRotateMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogRotateMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogHoistPlus
            // 
            this.btnJogHoistPlus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogHoistPlus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogHoistPlus.ForeColor = System.Drawing.Color.Black;
            this.btnJogHoistPlus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_zp;
            this.btnJogHoistPlus.Location = new System.Drawing.Point(869, 700);
            this.btnJogHoistPlus.Name = "btnJogHoistPlus";
            this.btnJogHoistPlus.Size = new System.Drawing.Size(117, 102);
            this.btnJogHoistPlus.TabIndex = 123;
            this.btnJogHoistPlus.Tag = "control";
            this.btnJogHoistPlus.Text = "上升";
            this.btnJogHoistPlus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogHoistPlus.UseVisualStyleBackColor = false;
            this.btnJogHoistPlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogHoistPlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogRotatePlus
            // 
            this.btnJogRotatePlus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogRotatePlus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogRotatePlus.ForeColor = System.Drawing.Color.Black;
            this.btnJogRotatePlus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_ccwm;
            this.btnJogRotatePlus.Location = new System.Drawing.Point(869, 462);
            this.btnJogRotatePlus.Name = "btnJogRotatePlus";
            this.btnJogRotatePlus.Size = new System.Drawing.Size(117, 102);
            this.btnJogRotatePlus.TabIndex = 122;
            this.btnJogRotatePlus.Tag = "control";
            this.btnJogRotatePlus.Text = "逆时针旋转";
            this.btnJogRotatePlus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogRotatePlus.UseVisualStyleBackColor = false;
            this.btnJogRotatePlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogRotatePlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogSlideMinus
            // 
            this.btnJogSlideMinus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogSlideMinus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogSlideMinus.ForeColor = System.Drawing.Color.Black;
            this.btnJogSlideMinus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_ym;
            this.btnJogSlideMinus.Location = new System.Drawing.Point(1003, 700);
            this.btnJogSlideMinus.Name = "btnJogSlideMinus";
            this.btnJogSlideMinus.Size = new System.Drawing.Size(117, 102);
            this.btnJogSlideMinus.TabIndex = 121;
            this.btnJogSlideMinus.Tag = "control";
            this.btnJogSlideMinus.Text = "前右侧伸出";
            this.btnJogSlideMinus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogSlideMinus.UseVisualStyleBackColor = false;
            this.btnJogSlideMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogSlideMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogHoistMinus
            // 
            this.btnJogHoistMinus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogHoistMinus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogHoistMinus.ForeColor = System.Drawing.Color.Black;
            this.btnJogHoistMinus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_zm;
            this.btnJogHoistMinus.Location = new System.Drawing.Point(1136, 700);
            this.btnJogHoistMinus.Name = "btnJogHoistMinus";
            this.btnJogHoistMinus.Size = new System.Drawing.Size(117, 102);
            this.btnJogHoistMinus.TabIndex = 120;
            this.btnJogHoistMinus.Tag = "control";
            this.btnJogHoistMinus.Text = "下降";
            this.btnJogHoistMinus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogHoistMinus.UseVisualStyleBackColor = false;
            this.btnJogHoistMinus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogHoistMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogSlidePlus
            // 
            this.btnJogSlidePlus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogSlidePlus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogSlidePlus.ForeColor = System.Drawing.Color.Black;
            this.btnJogSlidePlus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_yp;
            this.btnJogSlidePlus.Location = new System.Drawing.Point(1003, 462);
            this.btnJogSlidePlus.Name = "btnJogSlidePlus";
            this.btnJogSlidePlus.Size = new System.Drawing.Size(117, 102);
            this.btnJogSlidePlus.TabIndex = 119;
            this.btnJogSlidePlus.Tag = "control";
            this.btnJogSlidePlus.Text = "前左侧伸出";
            this.btnJogSlidePlus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogSlidePlus.UseVisualStyleBackColor = false;
            this.btnJogSlidePlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogSlidePlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogTransferPlus
            // 
            this.btnJogTransferPlus.BackColor = System.Drawing.Color.Yellow;
            this.btnJogTransferPlus.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogTransferPlus.ForeColor = System.Drawing.Color.Black;
            this.btnJogTransferPlus.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_xp;
            this.btnJogTransferPlus.Location = new System.Drawing.Point(1136, 581);
            this.btnJogTransferPlus.Name = "btnJogTransferPlus";
            this.btnJogTransferPlus.Size = new System.Drawing.Size(117, 102);
            this.btnJogTransferPlus.TabIndex = 118;
            this.btnJogTransferPlus.Tag = "control";
            this.btnJogTransferPlus.Text = "前进";
            this.btnJogTransferPlus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogTransferPlus.UseVisualStyleBackColor = false;
            this.btnJogTransferPlus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogTransferPlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogStop
            // 
            this.btnJogStop.BackColor = System.Drawing.Color.Yellow;
            this.btnJogStop.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogStop.ForeColor = System.Drawing.Color.Black;
            this.btnJogStop.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_stop;
            this.btnJogStop.Location = new System.Drawing.Point(1003, 581);
            this.btnJogStop.Name = "btnJogStop";
            this.btnJogStop.Size = new System.Drawing.Size(117, 102);
            this.btnJogStop.TabIndex = 117;
            this.btnJogStop.Tag = "control";
            this.btnJogStop.Text = "停止";
            this.btnJogStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogStop.UseVisualStyleBackColor = false;
            this.btnJogStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogStop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // btnJogTransferMiuns
            // 
            this.btnJogTransferMiuns.BackColor = System.Drawing.Color.Yellow;
            this.btnJogTransferMiuns.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnJogTransferMiuns.ForeColor = System.Drawing.Color.Black;
            this.btnJogTransferMiuns.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.motor_jog_xm;
            this.btnJogTransferMiuns.Location = new System.Drawing.Point(869, 581);
            this.btnJogTransferMiuns.Name = "btnJogTransferMiuns";
            this.btnJogTransferMiuns.Size = new System.Drawing.Size(117, 102);
            this.btnJogTransferMiuns.TabIndex = 116;
            this.btnJogTransferMiuns.Tag = "control";
            this.btnJogTransferMiuns.Text = "后退";
            this.btnJogTransferMiuns.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJogTransferMiuns.UseVisualStyleBackColor = false;
            this.btnJogTransferMiuns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jog_MouseDown);
            this.btnJogTransferMiuns.MouseUp += new System.Windows.Forms.MouseEventHandler(this.jog_MouseUp);
            // 
            // pictureVHL
            // 
            this.pictureVHL.BackColor = System.Drawing.Color.Transparent;
            this.pictureVHL.Image = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.VHL_NoFOUP;
            this.pictureVHL.Location = new System.Drawing.Point(491, 62);
            this.pictureVHL.Name = "pictureVHL";
            this.pictureVHL.Size = new System.Drawing.Size(441, 378);
            this.pictureVHL.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureVHL.TabIndex = 115;
            this.pictureVHL.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(939, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 46);
            this.label5.TabIndex = 140;
            this.label5.Text = "前导向轮";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(939, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 46);
            this.label6.TabIndex = 141;
            this.label6.Text = "后导向轮";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(938, 239);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 46);
            this.label7.TabIndex = 142;
            this.label7.Text = "夹       爪";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(935, 312);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 46);
            this.label8.TabIndex = 143;
            this.label8.Text = "前防坠落";
            // 
            // lbConnectionStatus
            // 
            this.lbConnectionStatus.AutoSize = true;
            this.lbConnectionStatus.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbConnectionStatus.ForeColor = System.Drawing.Color.White;
            this.lbConnectionStatus.Location = new System.Drawing.Point(68, 69);
            this.lbConnectionStatus.Name = "lbConnectionStatus";
            this.lbConnectionStatus.Size = new System.Drawing.Size(74, 21);
            this.lbConnectionStatus.TabIndex = 145;
            this.lbConnectionStatus.Text = "连接状态";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(935, 385);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 46);
            this.label1.TabIndex = 149;
            this.label1.Text = "后防坠落";
            // 
            // btnRearAntiDropStatus
            // 
            this.btnRearAntiDropStatus.BackgroundImage = global::Sineva.VHL.GUI.TouchPad.Properties.Resources.buttonFace1;
            this.btnRearAntiDropStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRearAntiDropStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRearAntiDropStatus.FlatAppearance.BorderSize = 0;
            this.btnRearAntiDropStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRearAntiDropStatus.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRearAntiDropStatus.ForeColor = System.Drawing.Color.White;
            this.btnRearAntiDropStatus.Location = new System.Drawing.Point(1120, 385);
            this.btnRearAntiDropStatus.Name = "btnRearAntiDropStatus";
            this.btnRearAntiDropStatus.Size = new System.Drawing.Size(122, 51);
            this.btnRearAntiDropStatus.TabIndex = 148;
            this.btnRearAntiDropStatus.Tag = "control";
            this.btnRearAntiDropStatus.Text = "UnLock";
            this.btnRearAntiDropStatus.UseVisualStyleBackColor = true;
            this.btnRearAntiDropStatus.TextChanged += new System.EventHandler(this.btnAntiDropStatus_TextChanged);
            // 
            // servoStates1
            // 
            this.servoStates1.BackColor = System.Drawing.Color.Transparent;
            this.servoStates1.Location = new System.Drawing.Point(72, 131);
            this.servoStates1.Name = "servoStates1";
            this.servoStates1.Size = new System.Drawing.Size(435, 297);
            this.servoStates1.TabIndex = 147;
            // 
            // ucTitle1
            // 
            this.ucTitle1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ucTitle1.BackgroundImage")));
            this.ucTitle1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ucTitle1.Location = new System.Drawing.Point(0, -1);
            this.ucTitle1.Name = "ucTitle1";
            this.ucTitle1.Size = new System.Drawing.Size(1288, 57);
            this.ucTitle1.TabIndex = 146;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ClientSize = new System.Drawing.Size(1284, 834);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRearAntiDropStatus);
            this.Controls.Add(this.servoStates1);
            this.Controls.Add(this.ucTitle1);
            this.Controls.Add(this.lbConnectionStatus);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnReady);
            this.Controls.Add(this.btnFrontAntiDropStatus);
            this.Controls.Add(this.btnGripperStatus);
            this.Controls.Add(this.btnRearSteerStatus);
            this.Controls.Add(this.btnFrontSteerStatus);
            this.Controls.Add(this.btnEqpState);
            this.Controls.Add(this.btnMode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnVolecityHigh);
            this.Controls.Add(this.btnVolecityMid);
            this.Controls.Add(this.btnVolecitySlow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnJogRotateMinus);
            this.Controls.Add(this.btnJogHoistPlus);
            this.Controls.Add(this.btnJogRotatePlus);
            this.Controls.Add(this.btnJogSlideMinus);
            this.Controls.Add(this.btnJogHoistMinus);
            this.Controls.Add(this.btnJogSlidePlus);
            this.Controls.Add(this.btnJogTransferPlus);
            this.Controls.Add(this.btnJogStop);
            this.Controls.Add(this.btnJogTransferMiuns);
            this.Controls.Add(this.pictureVHL);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "control";
            this.Text = "VHL Touch GUI";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureVHL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer timerDateUpdate;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnReady;
        private System.Windows.Forms.Button btnFrontAntiDropStatus;
        private System.Windows.Forms.Button btnGripperStatus;
        private System.Windows.Forms.Button btnRearSteerStatus;
        private System.Windows.Forms.Button btnFrontSteerStatus;
        private System.Windows.Forms.Button btnEqpState;
        private System.Windows.Forms.Button btnMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnVolecityHigh;
        private System.Windows.Forms.Button btnVolecityMid;
        private System.Windows.Forms.Button btnVolecitySlow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnJogRotateMinus;
        private System.Windows.Forms.Button btnJogHoistPlus;
        private System.Windows.Forms.Button btnJogRotatePlus;
        private System.Windows.Forms.Button btnJogSlideMinus;
        private System.Windows.Forms.Button btnJogHoistMinus;
        private System.Windows.Forms.Button btnJogSlidePlus;
        private System.Windows.Forms.Button btnJogTransferPlus;
        private System.Windows.Forms.Button btnJogStop;
        private System.Windows.Forms.Button btnJogTransferMiuns;
        private System.Windows.Forms.PictureBox pictureVHL;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer timerStatesUpdate;
        private System.Windows.Forms.Label lbConnectionStatus;
        private ucTitle ucTitle1;
        private ServoStates servoStates1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRearAntiDropStatus;
    }
}

