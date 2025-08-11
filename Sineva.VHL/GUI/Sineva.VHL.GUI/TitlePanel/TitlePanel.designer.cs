namespace Sineva.VHL.GUI
{
    partial class TitlePanel
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
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitlePanel));
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelSub = new System.Windows.Forms.Panel();
            this.panel40 = new System.Windows.Forms.Panel();
            this.cbMessage = new System.Windows.Forms.ComboBox();
            this.panel49 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel39 = new System.Windows.Forms.Panel();
            this.panel32 = new System.Windows.Forms.Panel();
            this.lbEqpState = new Sineva.VHL.Library.RGBLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel33 = new System.Windows.Forms.Panel();
            this.panel34 = new System.Windows.Forms.Panel();
            this.panel38 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbEqpMode = new Sineva.VHL.Library.RGBLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel31 = new System.Windows.Forms.Panel();
            this.panel30 = new System.Windows.Forms.Panel();
            this.panel26 = new System.Windows.Forms.Panel();
            this.lbCommand = new Sineva.VHL.Library.ImageLabel();
            this.panel45 = new System.Windows.Forms.Panel();
            this.panel46 = new System.Windows.Forms.Panel();
            this.panel44 = new System.Windows.Forms.Panel();
            this.panel43 = new System.Windows.Forms.Panel();
            this.panel25 = new System.Windows.Forms.Panel();
            this.lbViewName = new Sineva.VHL.Library.ImageLabel();
            this.panel42 = new System.Windows.Forms.Panel();
            this.panel41 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel50 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lbOcsState = new Sineva.VHL.Library.RGBLabel();
            this.lbJcsState = new Sineva.VHL.Library.RGBLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel28 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panelModuleCheck = new System.Windows.Forms.Panel();
            this.panelModuleStatus = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.lbPathInfo = new Sineva.VHL.Library.ImageLabel();
            this.panel52 = new System.Windows.Forms.Panel();
            this.panel53 = new System.Windows.Forms.Panel();
            this.panel54 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel29 = new System.Windows.Forms.Panel();
            this.lbRightBcr = new Sineva.VHL.Library.ImageLabel();
            this.panel35 = new System.Windows.Forms.Panel();
            this.panel36 = new System.Windows.Forms.Panel();
            this.panel48 = new System.Windows.Forms.Panel();
            this.panel47 = new System.Windows.Forms.Panel();
            this.lbLoginInfo = new System.Windows.Forms.Label();
            this.panel24 = new System.Windows.Forms.Panel();
            this.lbLeftBcr = new Sineva.VHL.Library.ImageLabel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel27 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel23 = new System.Windows.Forms.Panel();
            this.panel21 = new System.Windows.Forms.Panel();
            this.logInView2 = new Sineva.VHL.Data.LogIn.LogInView();
            this.panel18 = new System.Windows.Forms.Panel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.panel22 = new System.Windows.Forms.Panel();
            this.panel20 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lbCurSystemTime = new System.Windows.Forms.Label();
            this.panel16 = new System.Windows.Forms.Panel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lbVehicleNumber = new Sineva.VHL.Library.ImageLabel();
            this.panel51 = new System.Windows.Forms.Panel();
            this.panelSub.SuspendLayout();
            this.panel40.SuspendLayout();
            this.panel32.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel26.SuspendLayout();
            this.panel25.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelModuleCheck.SuspendLayout();
            this.PanelMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel29.SuspendLayout();
            this.panel47.SuspendLayout();
            this.panel24.SuspendLayout();
            this.panel21.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // panelSub
            // 
            this.panelSub.BackColor = System.Drawing.Color.Transparent;
            this.panelSub.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.TitleSubBackGround2;
            this.panelSub.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelSub.Controls.Add(this.panel40);
            this.panelSub.Controls.Add(this.panel39);
            this.panelSub.Controls.Add(this.panel32);
            this.panelSub.Controls.Add(this.panel38);
            this.panelSub.Controls.Add(this.panel1);
            this.panelSub.Controls.Add(this.panel26);
            this.panelSub.Controls.Add(this.panel25);
            this.panelSub.Controls.Add(this.panel3);
            this.panelSub.Controls.Add(this.panelModuleCheck);
            this.panelSub.Controls.Add(this.panel8);
            this.panelSub.Controls.Add(this.panel12);
            this.panelSub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSub.Location = new System.Drawing.Point(0, 44);
            this.panelSub.Name = "panelSub";
            this.panelSub.Size = new System.Drawing.Size(1674, 56);
            this.panelSub.TabIndex = 22;
            // 
            // panel40
            // 
            this.panel40.BackColor = System.Drawing.Color.Transparent;
            this.panel40.Controls.Add(this.cbMessage);
            this.panel40.Controls.Add(this.panel49);
            this.panel40.Controls.Add(this.panel11);
            this.panel40.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel40.Location = new System.Drawing.Point(630, 0);
            this.panel40.Name = "panel40";
            this.panel40.Size = new System.Drawing.Size(527, 56);
            this.panel40.TabIndex = 36;
            // 
            // cbMessage
            // 
            this.cbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMessage.Font = new System.Drawing.Font("Arial", 11F);
            this.cbMessage.FormattingEnabled = true;
            this.cbMessage.Location = new System.Drawing.Point(0, 15);
            this.cbMessage.Name = "cbMessage";
            this.cbMessage.Size = new System.Drawing.Size(527, 25);
            this.cbMessage.TabIndex = 31;
            // 
            // panel49
            // 
            this.panel49.BackColor = System.Drawing.Color.Transparent;
            this.panel49.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel49.Location = new System.Drawing.Point(0, 46);
            this.panel49.Name = "panel49";
            this.panel49.Size = new System.Drawing.Size(527, 10);
            this.panel49.TabIndex = 34;
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.Transparent;
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(527, 15);
            this.panel11.TabIndex = 32;
            // 
            // panel39
            // 
            this.panel39.BackColor = System.Drawing.Color.Transparent;
            this.panel39.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel39.Location = new System.Drawing.Point(618, 0);
            this.panel39.Name = "panel39";
            this.panel39.Size = new System.Drawing.Size(12, 56);
            this.panel39.TabIndex = 34;
            // 
            // panel32
            // 
            this.panel32.BackColor = System.Drawing.Color.Transparent;
            this.panel32.Controls.Add(this.lbEqpState);
            this.panel32.Controls.Add(this.label3);
            this.panel32.Controls.Add(this.panel33);
            this.panel32.Controls.Add(this.panel34);
            this.panel32.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel32.Location = new System.Drawing.Point(481, 0);
            this.panel32.Name = "panel32";
            this.panel32.Size = new System.Drawing.Size(137, 56);
            this.panel32.TabIndex = 33;
            // 
            // lbEqpState
            // 
            this.lbEqpState.BackColor = System.Drawing.Color.Transparent;
            this.lbEqpState.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lbEqpState.BackgroundImage")));
            this.lbEqpState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbEqpState.BgSelect = Sineva.VHL.Library.enRGBSelect.Green;
            this.lbEqpState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbEqpState.LabelText = "RUN";
            this.lbEqpState.LableColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbEqpState.Location = new System.Drawing.Point(47, 10);
            this.lbEqpState.Name = "lbEqpState";
            this.lbEqpState.Size = new System.Drawing.Size(90, 36);
            this.lbEqpState.TabIndex = 34;
            this.lbEqpState.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(0, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 36);
            this.label3.TabIndex = 33;
            this.label3.Text = "EQP\r\nSTATE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel33
            // 
            this.panel33.BackColor = System.Drawing.Color.Transparent;
            this.panel33.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel33.Location = new System.Drawing.Point(0, 46);
            this.panel33.Name = "panel33";
            this.panel33.Size = new System.Drawing.Size(137, 10);
            this.panel33.TabIndex = 32;
            // 
            // panel34
            // 
            this.panel34.BackColor = System.Drawing.Color.Transparent;
            this.panel34.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel34.Location = new System.Drawing.Point(0, 0);
            this.panel34.Name = "panel34";
            this.panel34.Size = new System.Drawing.Size(137, 10);
            this.panel34.TabIndex = 30;
            // 
            // panel38
            // 
            this.panel38.BackColor = System.Drawing.Color.Transparent;
            this.panel38.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel38.Location = new System.Drawing.Point(469, 0);
            this.panel38.Name = "panel38";
            this.panel38.Size = new System.Drawing.Size(12, 56);
            this.panel38.TabIndex = 32;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.lbEqpMode);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.panel31);
            this.panel1.Controls.Add(this.panel30);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(332, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(137, 56);
            this.panel1.TabIndex = 29;
            // 
            // lbEqpMode
            // 
            this.lbEqpMode.BackColor = System.Drawing.Color.Transparent;
            this.lbEqpMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lbEqpMode.BackgroundImage")));
            this.lbEqpMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbEqpMode.BgSelect = Sineva.VHL.Library.enRGBSelect.Blue;
            this.lbEqpMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbEqpMode.LabelText = "AUTO";
            this.lbEqpMode.LableColor = System.Drawing.Color.White;
            this.lbEqpMode.Location = new System.Drawing.Point(47, 10);
            this.lbEqpMode.Name = "lbEqpMode";
            this.lbEqpMode.Size = new System.Drawing.Size(90, 36);
            this.lbEqpMode.TabIndex = 34;
            this.lbEqpMode.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 36);
            this.label2.TabIndex = 33;
            this.label2.Text = "EQP\r\nMODE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel31
            // 
            this.panel31.BackColor = System.Drawing.Color.Transparent;
            this.panel31.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel31.Location = new System.Drawing.Point(0, 46);
            this.panel31.Name = "panel31";
            this.panel31.Size = new System.Drawing.Size(137, 10);
            this.panel31.TabIndex = 32;
            // 
            // panel30
            // 
            this.panel30.BackColor = System.Drawing.Color.Transparent;
            this.panel30.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel30.Location = new System.Drawing.Point(0, 0);
            this.panel30.Name = "panel30";
            this.panel30.Size = new System.Drawing.Size(137, 10);
            this.panel30.TabIndex = 30;
            // 
            // panel26
            // 
            this.panel26.BackColor = System.Drawing.Color.Transparent;
            this.panel26.Controls.Add(this.lbCommand);
            this.panel26.Controls.Add(this.panel45);
            this.panel26.Controls.Add(this.panel46);
            this.panel26.Controls.Add(this.panel44);
            this.panel26.Controls.Add(this.panel43);
            this.panel26.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel26.ForeColor = System.Drawing.Color.Black;
            this.panel26.Location = new System.Drawing.Point(126, 0);
            this.panel26.Name = "panel26";
            this.panel26.Size = new System.Drawing.Size(206, 56);
            this.panel26.TabIndex = 28;
            // 
            // lbCommand
            // 
            this.lbCommand.AutoResizeTextSize = true;
            this.lbCommand.BackColor = System.Drawing.Color.Transparent;
            this.lbCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCommand.Font = new System.Drawing.Font("Arial", 9F);
            this.lbCommand.ForeColor = System.Drawing.Color.Transparent;
            this.lbCommand.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbCommand.ImageOfBackground = global::Sineva.VHL.GUI.Properties.Resources.RoundRectWhite;
            this.lbCommand.LabelText = "MODEL A-1";
            this.lbCommand.Location = new System.Drawing.Point(12, 10);
            this.lbCommand.Name = "lbCommand";
            this.lbCommand.Size = new System.Drawing.Size(182, 36);
            this.lbCommand.TabIndex = 50;
            this.lbCommand.TextColor = System.Drawing.Color.Black;
            this.lbCommand.TextFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCommand.TextFromRight = System.Windows.Forms.RightToLeft.No;
            // 
            // panel45
            // 
            this.panel45.BackColor = System.Drawing.Color.Transparent;
            this.panel45.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel45.Location = new System.Drawing.Point(12, 46);
            this.panel45.Name = "panel45";
            this.panel45.Size = new System.Drawing.Size(182, 10);
            this.panel45.TabIndex = 49;
            // 
            // panel46
            // 
            this.panel46.BackColor = System.Drawing.Color.Transparent;
            this.panel46.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel46.Location = new System.Drawing.Point(12, 0);
            this.panel46.Name = "panel46";
            this.panel46.Size = new System.Drawing.Size(182, 10);
            this.panel46.TabIndex = 48;
            // 
            // panel44
            // 
            this.panel44.BackColor = System.Drawing.Color.Transparent;
            this.panel44.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel44.Location = new System.Drawing.Point(0, 0);
            this.panel44.Name = "panel44";
            this.panel44.Size = new System.Drawing.Size(12, 56);
            this.panel44.TabIndex = 23;
            // 
            // panel43
            // 
            this.panel43.BackColor = System.Drawing.Color.Transparent;
            this.panel43.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel43.Location = new System.Drawing.Point(194, 0);
            this.panel43.Name = "panel43";
            this.panel43.Size = new System.Drawing.Size(12, 56);
            this.panel43.TabIndex = 22;
            // 
            // panel25
            // 
            this.panel25.BackColor = System.Drawing.Color.Transparent;
            this.panel25.Controls.Add(this.lbViewName);
            this.panel25.Controls.Add(this.panel42);
            this.panel25.Controls.Add(this.panel41);
            this.panel25.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel25.ForeColor = System.Drawing.Color.RoyalBlue;
            this.panel25.Location = new System.Drawing.Point(12, 0);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(114, 56);
            this.panel25.TabIndex = 27;
            // 
            // lbViewName
            // 
            this.lbViewName.AutoResizeTextSize = false;
            this.lbViewName.BackColor = System.Drawing.Color.Transparent;
            this.lbViewName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbViewName.Font = new System.Drawing.Font("Arial", 9F);
            this.lbViewName.ForeColor = System.Drawing.Color.Transparent;
            this.lbViewName.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbViewName.ImageOfBackground = global::Sineva.VHL.GUI.Properties.Resources.RoundRectWhite;
            this.lbViewName.LabelText = "JOBS";
            this.lbViewName.Location = new System.Drawing.Point(0, 10);
            this.lbViewName.Name = "lbViewName";
            this.lbViewName.Size = new System.Drawing.Size(114, 36);
            this.lbViewName.TabIndex = 0;
            this.lbViewName.TextColor = System.Drawing.Color.RoyalBlue;
            this.lbViewName.TextFont = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbViewName.TextFromRight = System.Windows.Forms.RightToLeft.No;
            // 
            // panel42
            // 
            this.panel42.BackColor = System.Drawing.Color.Transparent;
            this.panel42.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel42.Location = new System.Drawing.Point(0, 46);
            this.panel42.Name = "panel42";
            this.panel42.Size = new System.Drawing.Size(114, 10);
            this.panel42.TabIndex = 46;
            // 
            // panel41
            // 
            this.panel41.BackColor = System.Drawing.Color.Transparent;
            this.panel41.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel41.Location = new System.Drawing.Point(0, 0);
            this.panel41.Name = "panel41";
            this.panel41.Size = new System.Drawing.Size(114, 10);
            this.panel41.TabIndex = 45;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.panel50);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.lbOcsState);
            this.panel3.Controls.Add(this.lbJcsState);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.panel28);
            this.panel3.Controls.Add(this.panel9);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1157, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(322, 56);
            this.panel3.TabIndex = 26;
            // 
            // panel50
            // 
            this.panel50.BackColor = System.Drawing.Color.Transparent;
            this.panel50.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel50.Location = new System.Drawing.Point(160, 10);
            this.panel50.Name = "panel50";
            this.panel50.Size = new System.Drawing.Size(10, 36);
            this.panel50.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Right;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(172, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 36);
            this.label4.TabIndex = 36;
            this.label4.Text = "JCS\r\nSTATUS";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbOcsState
            // 
            this.lbOcsState.BackColor = System.Drawing.Color.Transparent;
            this.lbOcsState.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lbOcsState.BackgroundImage")));
            this.lbOcsState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbOcsState.BgSelect = Sineva.VHL.Library.enRGBSelect.Red;
            this.lbOcsState.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbOcsState.LabelText = "Not Ready";
            this.lbOcsState.LableColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbOcsState.Location = new System.Drawing.Point(65, 10);
            this.lbOcsState.Name = "lbOcsState";
            this.lbOcsState.Size = new System.Drawing.Size(95, 36);
            this.lbOcsState.TabIndex = 35;
            this.lbOcsState.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // lbJcsState
            // 
            this.lbJcsState.BackColor = System.Drawing.Color.Transparent;
            this.lbJcsState.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lbJcsState.BackgroundImage")));
            this.lbJcsState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbJcsState.BgSelect = Sineva.VHL.Library.enRGBSelect.Red;
            this.lbJcsState.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbJcsState.LabelText = "Not Ready";
            this.lbJcsState.LableColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbJcsState.Location = new System.Drawing.Point(227, 10);
            this.lbJcsState.Margin = new System.Windows.Forms.Padding(0);
            this.lbJcsState.Name = "lbJcsState";
            this.lbJcsState.Size = new System.Drawing.Size(95, 36);
            this.lbJcsState.TabIndex = 5;
            this.lbJcsState.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 36);
            this.label1.TabIndex = 36;
            this.label1.Text = "OCS\r\nSTATUS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel28
            // 
            this.panel28.BackColor = System.Drawing.Color.Transparent;
            this.panel28.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel28.Location = new System.Drawing.Point(10, 46);
            this.panel28.Name = "panel28";
            this.panel28.Size = new System.Drawing.Size(312, 10);
            this.panel28.TabIndex = 33;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.Transparent;
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 10);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(10, 46);
            this.panel9.TabIndex = 30;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Transparent;
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(322, 10);
            this.panel5.TabIndex = 29;
            // 
            // panelModuleCheck
            // 
            this.panelModuleCheck.BackColor = System.Drawing.Color.Transparent;
            this.panelModuleCheck.Controls.Add(this.panelModuleStatus);
            this.panelModuleCheck.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelModuleCheck.Location = new System.Drawing.Point(1479, 0);
            this.panelModuleCheck.Name = "panelModuleCheck";
            this.panelModuleCheck.Size = new System.Drawing.Size(181, 56);
            this.panelModuleCheck.TabIndex = 25;
            // 
            // panelModuleStatus
            // 
            this.panelModuleStatus.BackColor = System.Drawing.Color.Black;
            this.panelModuleStatus.ForeColor = System.Drawing.Color.White;
            this.panelModuleStatus.Location = new System.Drawing.Point(6, 15);
            this.panelModuleStatus.Name = "panelModuleStatus";
            this.panelModuleStatus.Size = new System.Drawing.Size(160, 28);
            this.panelModuleStatus.TabIndex = 55;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.Transparent;
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(1660, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(14, 56);
            this.panel8.TabIndex = 24;
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.Transparent;
            this.panel12.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel12.Location = new System.Drawing.Point(0, 0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(12, 56);
            this.panel12.TabIndex = 21;
            // 
            // PanelMain
            // 
            this.PanelMain.BackColor = System.Drawing.Color.Transparent;
            this.PanelMain.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.TitleMainBackGround;
            this.PanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PanelMain.Controls.Add(this.lbPathInfo);
            this.PanelMain.Controls.Add(this.panel52);
            this.PanelMain.Controls.Add(this.panel53);
            this.PanelMain.Controls.Add(this.panel54);
            this.PanelMain.Controls.Add(this.panel2);
            this.PanelMain.Controls.Add(this.panel47);
            this.PanelMain.Controls.Add(this.panel24);
            this.PanelMain.Controls.Add(this.panel13);
            this.PanelMain.Controls.Add(this.panel23);
            this.PanelMain.Controls.Add(this.panel21);
            this.PanelMain.Controls.Add(this.panel4);
            this.PanelMain.Controls.Add(this.panel6);
            this.PanelMain.Controls.Add(this.lbVehicleNumber);
            this.PanelMain.Controls.Add(this.panel51);
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(1674, 44);
            this.PanelMain.TabIndex = 21;
            // 
            // lbPathInfo
            // 
            this.lbPathInfo.AutoResizeTextSize = false;
            this.lbPathInfo.BackColor = System.Drawing.Color.Transparent;
            this.lbPathInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPathInfo.Font = new System.Drawing.Font("Arial", 9F);
            this.lbPathInfo.ForeColor = System.Drawing.Color.Transparent;
            this.lbPathInfo.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbPathInfo.ImageOfBackground = null;
            this.lbPathInfo.LabelText = "(L，R,  60)";
            this.lbPathInfo.Location = new System.Drawing.Point(429, 10);
            this.lbPathInfo.Name = "lbPathInfo";
            this.lbPathInfo.Size = new System.Drawing.Size(99, 24);
            this.lbPathInfo.TabIndex = 64;
            this.lbPathInfo.TextColor = System.Drawing.Color.Navy;
            this.lbPathInfo.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPathInfo.TextFromRight = System.Windows.Forms.RightToLeft.No;
            // 
            // panel52
            // 
            this.panel52.BackColor = System.Drawing.Color.Transparent;
            this.panel52.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel52.Location = new System.Drawing.Point(429, 34);
            this.panel52.Name = "panel52";
            this.panel52.Size = new System.Drawing.Size(99, 10);
            this.panel52.TabIndex = 63;
            // 
            // panel53
            // 
            this.panel53.BackColor = System.Drawing.Color.Transparent;
            this.panel53.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel53.Location = new System.Drawing.Point(429, 0);
            this.panel53.Name = "panel53";
            this.panel53.Size = new System.Drawing.Size(99, 10);
            this.panel53.TabIndex = 62;
            // 
            // panel54
            // 
            this.panel54.BackColor = System.Drawing.Color.Transparent;
            this.panel54.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel54.Location = new System.Drawing.Point(528, 0);
            this.panel54.Name = "panel54";
            this.panel54.Size = new System.Drawing.Size(660, 44);
            this.panel54.TabIndex = 61;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.panel29);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(303, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(126, 44);
            this.panel2.TabIndex = 51;
            // 
            // panel29
            // 
            this.panel29.BackColor = System.Drawing.Color.Transparent;
            this.panel29.Controls.Add(this.lbRightBcr);
            this.panel29.Controls.Add(this.panel35);
            this.panel29.Controls.Add(this.panel36);
            this.panel29.Controls.Add(this.panel48);
            this.panel29.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel29.Location = new System.Drawing.Point(0, 0);
            this.panel29.Name = "panel29";
            this.panel29.Size = new System.Drawing.Size(126, 44);
            this.panel29.TabIndex = 49;
            // 
            // lbRightBcr
            // 
            this.lbRightBcr.AutoResizeTextSize = false;
            this.lbRightBcr.BackColor = System.Drawing.Color.Transparent;
            this.lbRightBcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRightBcr.Font = new System.Drawing.Font("Arial", 9F);
            this.lbRightBcr.ForeColor = System.Drawing.Color.Transparent;
            this.lbRightBcr.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbRightBcr.ImageOfBackground = null;
            this.lbRightBcr.LabelText = "right bcr value";
            this.lbRightBcr.Location = new System.Drawing.Point(0, 10);
            this.lbRightBcr.Name = "lbRightBcr";
            this.lbRightBcr.Size = new System.Drawing.Size(114, 24);
            this.lbRightBcr.TabIndex = 54;
            this.lbRightBcr.TextColor = System.Drawing.Color.Navy;
            this.lbRightBcr.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRightBcr.TextFromRight = System.Windows.Forms.RightToLeft.No;
            this.lbRightBcr.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbRightBcr_MouseDoubleClick);
            // 
            // panel35
            // 
            this.panel35.BackColor = System.Drawing.Color.Transparent;
            this.panel35.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel35.Location = new System.Drawing.Point(0, 34);
            this.panel35.Name = "panel35";
            this.panel35.Size = new System.Drawing.Size(114, 10);
            this.panel35.TabIndex = 53;
            // 
            // panel36
            // 
            this.panel36.BackColor = System.Drawing.Color.Transparent;
            this.panel36.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel36.Location = new System.Drawing.Point(0, 0);
            this.panel36.Name = "panel36";
            this.panel36.Size = new System.Drawing.Size(114, 10);
            this.panel36.TabIndex = 52;
            // 
            // panel48
            // 
            this.panel48.BackColor = System.Drawing.Color.Transparent;
            this.panel48.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel48.Location = new System.Drawing.Point(114, 0);
            this.panel48.Name = "panel48";
            this.panel48.Size = new System.Drawing.Size(12, 44);
            this.panel48.TabIndex = 51;
            // 
            // panel47
            // 
            this.panel47.BackColor = System.Drawing.Color.Transparent;
            this.panel47.Controls.Add(this.lbLoginInfo);
            this.panel47.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel47.Location = new System.Drawing.Point(1188, 0);
            this.panel47.Name = "panel47";
            this.panel47.Size = new System.Drawing.Size(183, 44);
            this.panel47.TabIndex = 50;
            // 
            // lbLoginInfo
            // 
            this.lbLoginInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLoginInfo.Location = new System.Drawing.Point(0, 0);
            this.lbLoginInfo.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.lbLoginInfo.Name = "lbLoginInfo";
            this.lbLoginInfo.Size = new System.Drawing.Size(183, 44);
            this.lbLoginInfo.TabIndex = 0;
            this.lbLoginInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel24
            // 
            this.panel24.BackColor = System.Drawing.Color.Transparent;
            this.panel24.Controls.Add(this.lbLeftBcr);
            this.panel24.Controls.Add(this.panel7);
            this.panel24.Controls.Add(this.panel10);
            this.panel24.Controls.Add(this.panel27);
            this.panel24.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel24.Location = new System.Drawing.Point(177, 0);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(126, 44);
            this.panel24.TabIndex = 48;
            // 
            // lbLeftBcr
            // 
            this.lbLeftBcr.AutoResizeTextSize = false;
            this.lbLeftBcr.BackColor = System.Drawing.Color.Transparent;
            this.lbLeftBcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLeftBcr.Font = new System.Drawing.Font("Arial", 9F);
            this.lbLeftBcr.ForeColor = System.Drawing.Color.Transparent;
            this.lbLeftBcr.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbLeftBcr.ImageOfBackground = null;
            this.lbLeftBcr.LabelText = "left bcr value";
            this.lbLeftBcr.Location = new System.Drawing.Point(0, 10);
            this.lbLeftBcr.Name = "lbLeftBcr";
            this.lbLeftBcr.Size = new System.Drawing.Size(114, 24);
            this.lbLeftBcr.TabIndex = 54;
            this.lbLeftBcr.TextColor = System.Drawing.Color.Navy;
            this.lbLeftBcr.TextFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLeftBcr.TextFromRight = System.Windows.Forms.RightToLeft.No;
            this.lbLeftBcr.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbLeftBcr_MouseDoubleClick);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Transparent;
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 34);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(114, 10);
            this.panel7.TabIndex = 53;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.Transparent;
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(114, 10);
            this.panel10.TabIndex = 52;
            // 
            // panel27
            // 
            this.panel27.BackColor = System.Drawing.Color.Transparent;
            this.panel27.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel27.Location = new System.Drawing.Point(114, 0);
            this.panel27.Name = "panel27";
            this.panel27.Size = new System.Drawing.Size(12, 44);
            this.panel27.TabIndex = 51;
            // 
            // panel13
            // 
            this.panel13.BackColor = System.Drawing.Color.Transparent;
            this.panel13.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel13.Location = new System.Drawing.Point(163, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(14, 44);
            this.panel13.TabIndex = 45;
            // 
            // panel23
            // 
            this.panel23.BackColor = System.Drawing.Color.Transparent;
            this.panel23.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel23.Location = new System.Drawing.Point(1371, 0);
            this.panel23.Name = "panel23";
            this.panel23.Size = new System.Drawing.Size(14, 44);
            this.panel23.TabIndex = 44;
            // 
            // panel21
            // 
            this.panel21.BackColor = System.Drawing.Color.Transparent;
            this.panel21.Controls.Add(this.logInView2);
            this.panel21.Controls.Add(this.panel18);
            this.panel21.Controls.Add(this.panel19);
            this.panel21.Controls.Add(this.panel22);
            this.panel21.Controls.Add(this.panel20);
            this.panel21.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel21.Location = new System.Drawing.Point(1385, 0);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(100, 44);
            this.panel21.TabIndex = 40;
            // 
            // logInView2
            // 
            this.logInView2.BackColor = System.Drawing.Color.Transparent;
            this.logInView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logInView2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.logInView2.ForeColor = System.Drawing.Color.White;
            this.logInView2.Location = new System.Drawing.Point(6, 7);
            this.logInView2.Message = "LOGIN    ";
            this.logInView2.Name = "logInView2";
            this.logInView2.Size = new System.Drawing.Size(88, 30);
            this.logInView2.TabIndex = 42;
            // 
            // panel18
            // 
            this.panel18.BackColor = System.Drawing.Color.Transparent;
            this.panel18.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel18.Location = new System.Drawing.Point(6, 0);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(88, 7);
            this.panel18.TabIndex = 40;
            // 
            // panel19
            // 
            this.panel19.BackColor = System.Drawing.Color.Transparent;
            this.panel19.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel19.Location = new System.Drawing.Point(6, 37);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(88, 7);
            this.panel19.TabIndex = 39;
            // 
            // panel22
            // 
            this.panel22.BackColor = System.Drawing.Color.Transparent;
            this.panel22.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel22.Location = new System.Drawing.Point(0, 0);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(6, 44);
            this.panel22.TabIndex = 38;
            // 
            // panel20
            // 
            this.panel20.BackColor = System.Drawing.Color.Transparent;
            this.panel20.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel20.Location = new System.Drawing.Point(94, 0);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(6, 44);
            this.panel20.TabIndex = 36;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.lbCurSystemTime);
            this.panel4.Controls.Add(this.panel16);
            this.panel4.Controls.Add(this.panel17);
            this.panel4.Controls.Add(this.panel15);
            this.panel4.Controls.Add(this.panel14);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1485, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(175, 44);
            this.panel4.TabIndex = 36;
            // 
            // lbCurSystemTime
            // 
            this.lbCurSystemTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCurSystemTime.Font = new System.Drawing.Font("Arial", 9.75F);
            this.lbCurSystemTime.ForeColor = System.Drawing.Color.White;
            this.lbCurSystemTime.Image = global::Sineva.VHL.GUI.Properties.Resources.RoundRectBlue;
            this.lbCurSystemTime.Location = new System.Drawing.Point(6, 10);
            this.lbCurSystemTime.Name = "lbCurSystemTime";
            this.lbCurSystemTime.Size = new System.Drawing.Size(163, 24);
            this.lbCurSystemTime.TabIndex = 39;
            this.lbCurSystemTime.Text = "0000/00/00 00:00:00";
            this.lbCurSystemTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.Color.Transparent;
            this.panel16.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel16.Location = new System.Drawing.Point(6, 0);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(163, 10);
            this.panel16.TabIndex = 38;
            // 
            // panel17
            // 
            this.panel17.BackColor = System.Drawing.Color.Transparent;
            this.panel17.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel17.Location = new System.Drawing.Point(6, 34);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(163, 10);
            this.panel17.TabIndex = 37;
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.Color.Transparent;
            this.panel15.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(6, 44);
            this.panel15.TabIndex = 36;
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.Color.Transparent;
            this.panel14.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel14.Location = new System.Drawing.Point(169, 0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(6, 44);
            this.panel14.TabIndex = 31;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Transparent;
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(1660, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(14, 44);
            this.panel6.TabIndex = 28;
            // 
            // lbVehicleNumber
            // 
            this.lbVehicleNumber.AutoResizeTextSize = false;
            this.lbVehicleNumber.BackColor = System.Drawing.Color.Transparent;
            this.lbVehicleNumber.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbVehicleNumber.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVehicleNumber.ForeColor = System.Drawing.Color.Transparent;
            this.lbVehicleNumber.ImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lbVehicleNumber.ImageOfBackground = null;
            this.lbVehicleNumber.LabelText = "Number";
            this.lbVehicleNumber.Location = new System.Drawing.Point(16, 0);
            this.lbVehicleNumber.Name = "lbVehicleNumber";
            this.lbVehicleNumber.Size = new System.Drawing.Size(147, 44);
            this.lbVehicleNumber.TabIndex = 57;
            this.lbVehicleNumber.TextColor = System.Drawing.Color.Blue;
            this.lbVehicleNumber.TextFont = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVehicleNumber.TextFromRight = System.Windows.Forms.RightToLeft.No;
            // 
            // panel51
            // 
            this.panel51.BackColor = System.Drawing.Color.Transparent;
            this.panel51.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel51.Location = new System.Drawing.Point(0, 0);
            this.panel51.Name = "panel51";
            this.panel51.Size = new System.Drawing.Size(16, 44);
            this.panel51.TabIndex = 59;
            // 
            // TitlePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panelSub);
            this.Controls.Add(this.PanelMain);
            this.Name = "TitlePanel";
            this.Size = new System.Drawing.Size(1674, 100);
            this.panelSub.ResumeLayout(false);
            this.panel40.ResumeLayout(false);
            this.panel32.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel26.ResumeLayout(false);
            this.panel25.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panelModuleCheck.ResumeLayout(false);
            this.PanelMain.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel29.ResumeLayout(false);
            this.panel47.ResumeLayout(false);
            this.panel24.ResumeLayout(false);
            this.panel21.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel23;
        private System.Windows.Forms.Panel panel21;
        private Sineva.VHL.Data.LogIn.LogInView logInView2;
        private System.Windows.Forms.Panel panel18;
        private System.Windows.Forms.Panel panel19;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.Panel panel20;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Panel panel17;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel47;
        private System.Windows.Forms.Panel panelSub;
        private System.Windows.Forms.Panel panel40;
        private System.Windows.Forms.Panel panel39;
        private System.Windows.Forms.Panel panel32;
        private Sineva.VHL.Library.RGBLabel lbEqpState;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel33;
        private System.Windows.Forms.Panel panel34;
        private System.Windows.Forms.Panel panel38;
        private System.Windows.Forms.Panel panel1;
        private Sineva.VHL.Library.RGBLabel lbEqpMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel31;
        private System.Windows.Forms.Panel panel30;
        private System.Windows.Forms.Panel panel26;
        private System.Windows.Forms.Panel panel45;
        private System.Windows.Forms.Panel panel46;
        private System.Windows.Forms.Panel panel44;
        private System.Windows.Forms.Panel panel43;
        private System.Windows.Forms.Panel panel25;
        private System.Windows.Forms.Panel panel42;
        private System.Windows.Forms.Panel panel41;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox cbMessage;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panelModuleCheck;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel12;
        private Sineva.VHL.Library.ImageLabel lbViewName;
		private Sineva.VHL.Library.ImageLabel lbCommand;
        private System.Windows.Forms.Timer tmrUpdate;
		private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel27;
		private Sineva.VHL.Library.ImageLabel lbLeftBcr;
		private System.Windows.Forms.Label lbLoginInfo;
		private System.Windows.Forms.Label lbCurSystemTime;
        private System.Windows.Forms.Panel panelModuleStatus;
        private System.Windows.Forms.Panel panel11;
        private Library.RGBLabel lbJcsState;
        private System.Windows.Forms.Label label1;
        private Library.RGBLabel lbOcsState;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel29;
        private Library.ImageLabel lbRightBcr;
        private System.Windows.Forms.Panel panel35;
        private System.Windows.Forms.Panel panel36;
        private System.Windows.Forms.Panel panel48;
        private System.Windows.Forms.Panel panel49;
        private System.Windows.Forms.Panel panel50;
        private System.Windows.Forms.Panel panel28;
        private System.Windows.Forms.Panel panel9;
        private Library.ImageLabel lbVehicleNumber;
        private System.Windows.Forms.Panel panel51;
        private Library.ImageLabel lbPathInfo;
        private System.Windows.Forms.Panel panel52;
        private System.Windows.Forms.Panel panel53;
        private System.Windows.Forms.Panel panel54;
    }
}
