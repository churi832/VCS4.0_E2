namespace Sineva.VHL.Data.LogIn
{
	partial class LogInForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogInForm));
            this.tbPass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.linkLabelNewAccount = new System.Windows.Forms.LinkLabel();
            this.linkLabelDeleteAccount = new System.Windows.Forms.LinkLabel();
            this.linkLabelChangePassword = new System.Windows.Forms.LinkLabel();
            this.tbNewPass = new System.Windows.Forms.TextBox();
            this.lblNewPass = new System.Windows.Forms.Label();
            this.tbConfirmPass = new System.Windows.Forms.TextBox();
            this.lblConfirmPass = new System.Windows.Forms.Label();
            this.tbId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbLevel = new System.Windows.Forms.ComboBox();
            this.lblUserLevel = new System.Windows.Forms.Label();
            this.btnLogOut = new Sineva.VHL.Library.IButton();
            this.btnLogIn = new Sineva.VHL.Library.IButton();
            this.btnDo = new Sineva.VHL.Library.IButton();
            this.SuspendLayout();
            // 
            // tbPass
            // 
            this.tbPass.Location = new System.Drawing.Point(137, 65);
            this.tbPass.Name = "tbPass";
            this.tbPass.PasswordChar = '*';
            this.tbPass.Size = new System.Drawing.Size(121, 21);
            this.tbPass.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "ID List";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(10, 27);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(121, 172);
            this.listBox1.TabIndex = 4;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // linkLabelNewAccount
            // 
            this.linkLabelNewAccount.AutoSize = true;
            this.linkLabelNewAccount.Location = new System.Drawing.Point(269, 78);
            this.linkLabelNewAccount.Name = "linkLabelNewAccount";
            this.linkLabelNewAccount.Size = new System.Drawing.Size(81, 12);
            this.linkLabelNewAccount.TabIndex = 5;
            this.linkLabelNewAccount.TabStop = true;
            this.linkLabelNewAccount.Text = "New Account";
            this.linkLabelNewAccount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelNewAccount_LinkClicked);
            // 
            // linkLabelDeleteAccount
            // 
            this.linkLabelDeleteAccount.AutoSize = true;
            this.linkLabelDeleteAccount.Location = new System.Drawing.Point(269, 104);
            this.linkLabelDeleteAccount.Name = "linkLabelDeleteAccount";
            this.linkLabelDeleteAccount.Size = new System.Drawing.Size(90, 12);
            this.linkLabelDeleteAccount.TabIndex = 5;
            this.linkLabelDeleteAccount.TabStop = true;
            this.linkLabelDeleteAccount.Text = "Delete Account";
            this.linkLabelDeleteAccount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelDeleteAccount_LinkClicked);
            // 
            // linkLabelChangePassword
            // 
            this.linkLabelChangePassword.AutoSize = true;
            this.linkLabelChangePassword.Location = new System.Drawing.Point(269, 130);
            this.linkLabelChangePassword.Name = "linkLabelChangePassword";
            this.linkLabelChangePassword.Size = new System.Drawing.Size(110, 12);
            this.linkLabelChangePassword.TabIndex = 5;
            this.linkLabelChangePassword.TabStop = true;
            this.linkLabelChangePassword.Text = "Change Properties";
            this.linkLabelChangePassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChangePassword_LinkClicked);
            // 
            // tbNewPass
            // 
            this.tbNewPass.Location = new System.Drawing.Point(137, 142);
            this.tbNewPass.Name = "tbNewPass";
            this.tbNewPass.PasswordChar = '*';
            this.tbNewPass.Size = new System.Drawing.Size(121, 21);
            this.tbNewPass.TabIndex = 1;
            // 
            // lblNewPass
            // 
            this.lblNewPass.AutoSize = true;
            this.lblNewPass.Location = new System.Drawing.Point(137, 127);
            this.lblNewPass.Name = "lblNewPass";
            this.lblNewPass.Size = new System.Drawing.Size(92, 12);
            this.lblNewPass.TabIndex = 3;
            this.lblNewPass.Text = "New Password";
            // 
            // tbConfirmPass
            // 
            this.tbConfirmPass.Location = new System.Drawing.Point(137, 180);
            this.tbConfirmPass.Name = "tbConfirmPass";
            this.tbConfirmPass.PasswordChar = '*';
            this.tbConfirmPass.Size = new System.Drawing.Size(121, 21);
            this.tbConfirmPass.TabIndex = 1;
            // 
            // lblConfirmPass
            // 
            this.lblConfirmPass.AutoSize = true;
            this.lblConfirmPass.Location = new System.Drawing.Point(137, 165);
            this.lblConfirmPass.Name = "lblConfirmPass";
            this.lblConfirmPass.Size = new System.Drawing.Size(110, 12);
            this.lblConfirmPass.TabIndex = 3;
            this.lblConfirmPass.Text = "Confirm Password";
            // 
            // tbId
            // 
            this.tbId.Location = new System.Drawing.Point(137, 27);
            this.tbId.Name = "tbId";
            this.tbId.Size = new System.Drawing.Size(121, 21);
            this.tbId.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(137, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "ID";
            // 
            // cbLevel
            // 
            this.cbLevel.FormattingEnabled = true;
            this.cbLevel.Location = new System.Drawing.Point(137, 104);
            this.cbLevel.Name = "cbLevel";
            this.cbLevel.Size = new System.Drawing.Size(121, 20);
            this.cbLevel.TabIndex = 6;
            // 
            // lblUserLevel
            // 
            this.lblUserLevel.AutoSize = true;
            this.lblUserLevel.Location = new System.Drawing.Point(137, 88);
            this.lblUserLevel.Name = "lblUserLevel";
            this.lblUserLevel.Size = new System.Drawing.Size(65, 12);
            this.lblUserLevel.TabIndex = 3;
            this.lblUserLevel.Text = "User Level";
            // 
            // btnLogOut
            // 
            this.btnLogOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLogOut.BackgroundImage")));
            this.btnLogOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLogOut.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnLogOut.BgDefault")));
            this.btnLogOut.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnLogOut.BgDisable")));
            this.btnLogOut.BgOver = ((System.Drawing.Image)(resources.GetObject("btnLogOut.BgOver")));
            this.btnLogOut.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnLogOut.BgPushed")));
            this.btnLogOut.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnLogOut.ConnectedLabel = null;
            this.btnLogOut.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnLogOut.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnLogOut.DefaultImage = null;
            this.btnLogOut.Description = null;
            this.btnLogOut.DownImage = null;
            this.btnLogOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogOut.ForeColor = System.Drawing.Color.White;
            this.btnLogOut.Location = new System.Drawing.Point(270, 20);
            this.btnLogOut.Name = "btnLogOut";
            this.btnLogOut.OverImage = null;
            this.btnLogOut.Size = new System.Drawing.Size(93, 38);
            this.btnLogOut.TabIndex = 2;
            this.btnLogOut.Text = "Log Out";
            this.btnLogOut.UpImage = null;
            this.btnLogOut.UseOneImage = true;
            this.btnLogOut.UseVisualStyleBackColor = true;
            this.btnLogOut.Click += new System.EventHandler(this.btnLogOut_Click);
            // 
            // btnLogIn
            // 
            this.btnLogIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLogIn.BackgroundImage")));
            this.btnLogIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLogIn.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnLogIn.BgDefault")));
            this.btnLogIn.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnLogIn.BgDisable")));
            this.btnLogIn.BgOver = ((System.Drawing.Image)(resources.GetObject("btnLogIn.BgOver")));
            this.btnLogIn.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnLogIn.BgPushed")));
            this.btnLogIn.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnLogIn.ConnectedLabel = null;
            this.btnLogIn.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnLogIn.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnLogIn.DefaultImage = null;
            this.btnLogIn.Description = null;
            this.btnLogIn.DownImage = null;
            this.btnLogIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogIn.ForeColor = System.Drawing.Color.White;
            this.btnLogIn.Location = new System.Drawing.Point(270, 20);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.OverImage = null;
            this.btnLogIn.Size = new System.Drawing.Size(93, 38);
            this.btnLogIn.TabIndex = 2;
            this.btnLogIn.Text = "Log In";
            this.btnLogIn.UpImage = null;
            this.btnLogIn.UseOneImage = false;
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // btnDo
            // 
            this.btnDo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDo.BackgroundImage")));
            this.btnDo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDo.BgDefault = ((System.Drawing.Image)(resources.GetObject("btnDo.BgDefault")));
            this.btnDo.BgDisable = ((System.Drawing.Image)(resources.GetObject("btnDo.BgDisable")));
            this.btnDo.BgOver = ((System.Drawing.Image)(resources.GetObject("btnDo.BgOver")));
            this.btnDo.BgPushed = ((System.Drawing.Image)(resources.GetObject("btnDo.BgPushed")));
            this.btnDo.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnDo.ConnectedLabel = null;
            this.btnDo.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnDo.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnDo.DefaultImage = null;
            this.btnDo.Description = null;
            this.btnDo.DownImage = null;
            this.btnDo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDo.ForeColor = System.Drawing.Color.White;
            this.btnDo.Location = new System.Drawing.Point(271, 160);
            this.btnDo.Name = "btnDo";
            this.btnDo.OverImage = null;
            this.btnDo.Size = new System.Drawing.Size(93, 38);
            this.btnDo.TabIndex = 2;
            this.btnDo.Text = "OK";
            this.btnDo.UpImage = null;
            this.btnDo.UseOneImage = true;
            this.btnDo.UseVisualStyleBackColor = true;
            this.btnDo.Click += new System.EventHandler(this.btnDo_Click);
            // 
            // LogInForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(373, 213);
            this.Controls.Add(this.cbLevel);
            this.Controls.Add(this.linkLabelDeleteAccount);
            this.Controls.Add(this.linkLabelChangePassword);
            this.Controls.Add(this.linkLabelNewAccount);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblConfirmPass);
            this.Controls.Add(this.lblNewPass);
            this.Controls.Add(this.lblUserLevel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLogOut);
            this.Controls.Add(this.btnLogIn);
            this.Controls.Add(this.btnDo);
            this.Controls.Add(this.tbId);
            this.Controls.Add(this.tbConfirmPass);
            this.Controls.Add(this.tbNewPass);
            this.Controls.Add(this.tbPass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogInForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Account";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbPass;
		private Sineva.VHL.Library.IButton btnDo;
		private Sineva.VHL.Library.IButton btnLogIn;
		private Sineva.VHL.Library.IButton btnLogOut;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.LinkLabel linkLabelNewAccount;
		private System.Windows.Forms.LinkLabel linkLabelDeleteAccount;
		private System.Windows.Forms.LinkLabel linkLabelChangePassword;
		private System.Windows.Forms.TextBox tbNewPass;
		private System.Windows.Forms.Label lblNewPass;
		private System.Windows.Forms.TextBox tbConfirmPass;
		private System.Windows.Forms.Label lblConfirmPass;
		private System.Windows.Forms.TextBox tbId;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbLevel;
        private System.Windows.Forms.Label lblUserLevel;
	}
}