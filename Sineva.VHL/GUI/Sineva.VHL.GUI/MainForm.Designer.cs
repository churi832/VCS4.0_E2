namespace Sineva.VHL.GUI
{
    partial class MainForm
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.panelNavi = new System.Windows.Forms.Panel();
            this.btnEms = new Sineva.VHL.Library.IButton();
            this.btnExit = new Sineva.VHL.Library.IButton();
            this.btnNaviDatabase = new Sineva.VHL.Library.IButton();
            this.btnNaviAlarm = new Sineva.VHL.Library.IButton();
            this.btnNaviSystem = new Sineva.VHL.Library.IButton();
            this.btnNaviMotor = new Sineva.VHL.Library.IButton();
            this.btnNaviSetup = new Sineva.VHL.Library.IButton();
            this.btnNaviJobs = new Sineva.VHL.Library.IButton();
            this.titlePanel1 = new Sineva.VHL.GUI.TitlePanel();
            this.panelNavi.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // panelNavi
            // 
            this.panelNavi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(67)))), ((int)(((byte)(74)))));
            this.panelNavi.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.navi_Panel_bg;
            this.panelNavi.Controls.Add(this.btnEms);
            this.panelNavi.Controls.Add(this.btnExit);
            this.panelNavi.Controls.Add(this.btnNaviDatabase);
            this.panelNavi.Controls.Add(this.btnNaviAlarm);
            this.panelNavi.Controls.Add(this.btnNaviSystem);
            this.panelNavi.Controls.Add(this.btnNaviMotor);
            this.panelNavi.Controls.Add(this.btnNaviSetup);
            this.panelNavi.Controls.Add(this.btnNaviJobs);
            this.panelNavi.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelNavi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(67)))), ((int)(((byte)(74)))));
            this.panelNavi.Location = new System.Drawing.Point(0, 950);
            this.panelNavi.Name = "panelNavi";
            this.panelNavi.Size = new System.Drawing.Size(1904, 94);
            this.panelNavi.TabIndex = 3;
            // 
            // btnEms
            // 
            this.btnEms.BackColor = System.Drawing.Color.Transparent;
            this.btnEms.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEms.BgDefault = null;
            this.btnEms.BgDisable = null;
            this.btnEms.BgOver = null;
            this.btnEms.BgPushed = null;
            this.btnEms.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnEms.ConnectedLabel = null;
            this.btnEms.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnEms.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnEms.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_emoD;
            this.btnEms.Description = "EMO";
            this.btnEms.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_emo_click;
            this.btnEms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEms.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_emoD;
            this.btnEms.Location = new System.Drawing.Point(1139, -4);
            this.btnEms.Name = "btnEms";
            this.btnEms.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_emo_over;
            this.btnEms.Size = new System.Drawing.Size(101, 98);
            this.btnEms.TabIndex = 8;
            this.btnEms.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEms.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_emoD;
            this.btnEms.UseOneImage = false;
            this.btnEms.UseVisualStyleBackColor = false;
            this.btnEms.Click += new System.EventHandler(this.btnEms_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExit.BgDefault = null;
            this.btnExit.BgDisable = null;
            this.btnExit.BgOver = null;
            this.btnExit.BgPushed = null;
            this.btnExit.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnExit.ConnectedLabel = null;
            this.btnExit.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnExit.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnExit.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_exitD;
            this.btnExit.Description = "EXIT";
            this.btnExit.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_exit_click;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_exitD;
            this.btnExit.Location = new System.Drawing.Point(1803, -4);
            this.btnExit.Name = "btnExit";
            this.btnExit.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_exit_over;
            this.btnExit.Size = new System.Drawing.Size(101, 98);
            this.btnExit.TabIndex = 7;
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExit.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_exitD;
            this.btnExit.UseOneImage = false;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnNaviDatabase
            // 
            this.btnNaviDatabase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaviDatabase.BgDefault = null;
            this.btnNaviDatabase.BgDisable = null;
            this.btnNaviDatabase.BgOver = null;
            this.btnNaviDatabase.BgPushed = null;
            this.btnNaviDatabase.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnNaviDatabase.ConnectedLabel = null;
            this.btnNaviDatabase.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnNaviDatabase.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnNaviDatabase.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_recipeD;
            this.btnNaviDatabase.Description = "ALARM";
            this.btnNaviDatabase.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_ricipe_click;
            this.btnNaviDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaviDatabase.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_recipeD;
            this.btnNaviDatabase.Location = new System.Drawing.Point(505, -4);
            this.btnNaviDatabase.Name = "btnNaviDatabase";
            this.btnNaviDatabase.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_ricipe_over;
            this.btnNaviDatabase.Size = new System.Drawing.Size(101, 98);
            this.btnNaviDatabase.TabIndex = 6;
            this.btnNaviDatabase.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNaviDatabase.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_recipeD;
            this.btnNaviDatabase.UseOneImage = false;
            this.btnNaviDatabase.UseVisualStyleBackColor = false;
            this.btnNaviDatabase.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // btnNaviAlarm
            // 
            this.btnNaviAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaviAlarm.BgDefault = null;
            this.btnNaviAlarm.BgDisable = null;
            this.btnNaviAlarm.BgOver = null;
            this.btnNaviAlarm.BgPushed = null;
            this.btnNaviAlarm.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnNaviAlarm.ConnectedLabel = null;
            this.btnNaviAlarm.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnNaviAlarm.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnNaviAlarm.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_alarmD;
            this.btnNaviAlarm.Description = "ALARM";
            this.btnNaviAlarm.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_alarm_click;
            this.btnNaviAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaviAlarm.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_alarmD;
            this.btnNaviAlarm.Location = new System.Drawing.Point(404, -4);
            this.btnNaviAlarm.Name = "btnNaviAlarm";
            this.btnNaviAlarm.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_alarm_over;
            this.btnNaviAlarm.Size = new System.Drawing.Size(101, 98);
            this.btnNaviAlarm.TabIndex = 6;
            this.btnNaviAlarm.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNaviAlarm.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_alarmD;
            this.btnNaviAlarm.UseOneImage = false;
            this.btnNaviAlarm.UseVisualStyleBackColor = false;
            this.btnNaviAlarm.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // btnNaviSystem
            // 
            this.btnNaviSystem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaviSystem.BgDefault = null;
            this.btnNaviSystem.BgDisable = null;
            this.btnNaviSystem.BgOver = null;
            this.btnNaviSystem.BgPushed = null;
            this.btnNaviSystem.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnNaviSystem.ConnectedLabel = null;
            this.btnNaviSystem.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnNaviSystem.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnNaviSystem.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_systemD;
            this.btnNaviSystem.Description = "SYSTEM";
            this.btnNaviSystem.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_systemP;
            this.btnNaviSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaviSystem.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_systemD;
            this.btnNaviSystem.Location = new System.Drawing.Point(303, -4);
            this.btnNaviSystem.Name = "btnNaviSystem";
            this.btnNaviSystem.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_systemO;
            this.btnNaviSystem.Size = new System.Drawing.Size(101, 98);
            this.btnNaviSystem.TabIndex = 5;
            this.btnNaviSystem.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNaviSystem.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_systemD;
            this.btnNaviSystem.UseOneImage = false;
            this.btnNaviSystem.UseVisualStyleBackColor = false;
            this.btnNaviSystem.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // btnNaviMotor
            // 
            this.btnNaviMotor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaviMotor.BgDefault = null;
            this.btnNaviMotor.BgDisable = null;
            this.btnNaviMotor.BgOver = null;
            this.btnNaviMotor.BgPushed = null;
            this.btnNaviMotor.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnNaviMotor.ConnectedLabel = null;
            this.btnNaviMotor.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnNaviMotor.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnNaviMotor.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_motorD;
            this.btnNaviMotor.Description = "MOTORS";
            this.btnNaviMotor.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_motorP;
            this.btnNaviMotor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaviMotor.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_motorD;
            this.btnNaviMotor.Location = new System.Drawing.Point(202, -4);
            this.btnNaviMotor.Name = "btnNaviMotor";
            this.btnNaviMotor.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_motor_over;
            this.btnNaviMotor.Size = new System.Drawing.Size(101, 98);
            this.btnNaviMotor.TabIndex = 4;
            this.btnNaviMotor.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNaviMotor.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_motorD;
            this.btnNaviMotor.UseOneImage = false;
            this.btnNaviMotor.UseVisualStyleBackColor = false;
            this.btnNaviMotor.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // btnNaviSetup
            // 
            this.btnNaviSetup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaviSetup.BgDefault = null;
            this.btnNaviSetup.BgDisable = null;
            this.btnNaviSetup.BgOver = null;
            this.btnNaviSetup.BgPushed = null;
            this.btnNaviSetup.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnNaviSetup.ConnectedLabel = null;
            this.btnNaviSetup.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnNaviSetup.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnNaviSetup.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_setupD;
            this.btnNaviSetup.Description = "SETUP";
            this.btnNaviSetup.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_setupP;
            this.btnNaviSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaviSetup.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_setupD;
            this.btnNaviSetup.Location = new System.Drawing.Point(101, -4);
            this.btnNaviSetup.Name = "btnNaviSetup";
            this.btnNaviSetup.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_setup_over;
            this.btnNaviSetup.Size = new System.Drawing.Size(101, 98);
            this.btnNaviSetup.TabIndex = 1;
            this.btnNaviSetup.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNaviSetup.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_setupD;
            this.btnNaviSetup.UseOneImage = false;
            this.btnNaviSetup.UseVisualStyleBackColor = false;
            this.btnNaviSetup.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // btnNaviJobs
            // 
            this.btnNaviJobs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaviJobs.BgDefault = null;
            this.btnNaviJobs.BgDisable = null;
            this.btnNaviJobs.BgOver = null;
            this.btnNaviJobs.BgPushed = null;
            this.btnNaviJobs.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnNaviJobs.ConnectedLabel = null;
            this.btnNaviJobs.ConnectedLableOffColor = System.Drawing.Color.White;
            this.btnNaviJobs.ConnectedLableOnColor = System.Drawing.SystemColors.Highlight;
            this.btnNaviJobs.DefaultImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_jobsD;
            this.btnNaviJobs.Description = "JOBS";
            this.btnNaviJobs.DownImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_jobsP;
            this.btnNaviJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaviJobs.Image = global::Sineva.VHL.GUI.Properties.Resources.bottom_jobsD;
            this.btnNaviJobs.Location = new System.Drawing.Point(0, -4);
            this.btnNaviJobs.Name = "btnNaviJobs";
            this.btnNaviJobs.OverImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_jobs_over;
            this.btnNaviJobs.Size = new System.Drawing.Size(101, 98);
            this.btnNaviJobs.TabIndex = 0;
            this.btnNaviJobs.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNaviJobs.UpImage = global::Sineva.VHL.GUI.Properties.Resources.bottom_jobsD;
            this.btnNaviJobs.UseOneImage = false;
            this.btnNaviJobs.UseVisualStyleBackColor = false;
            this.btnNaviJobs.Click += new System.EventHandler(this.btnNavi_Click);
            // 
            // titlePanel1
            // 
            this.titlePanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel1.Location = new System.Drawing.Point(0, 0);
            this.titlePanel1.Name = "titlePanel1";
            this.titlePanel1.Size = new System.Drawing.Size(1904, 100);
            this.titlePanel1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1904, 1044);
            this.ControlBox = false;
            this.Controls.Add(this.panelNavi);
            this.Controls.Add(this.titlePanel1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VHL Software";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelNavi.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerUpdate;
        private TitlePanel titlePanel1;
        private System.Windows.Forms.Panel panelNavi;
        private Library.IButton btnNaviSetup;
        private Library.IButton btnNaviJobs;
        private Library.IButton btnNaviAlarm;
        private Library.IButton btnNaviSystem;
        private Library.IButton btnNaviMotor;
        private Library.IButton btnExit;
        private Library.IButton btnEms;
        private Library.IButton btnNaviDatabase;
    }
}

