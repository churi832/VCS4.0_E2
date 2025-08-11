
namespace Sineva.VHL.IF.OCS
{
    partial class FrmPathRequestMessage
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
            this.lbPrimaryDataType = new System.Windows.Forms.Label();
            this.lbPrimaryVHL = new System.Windows.Forms.Label();
            this.tbAcknowledge = new System.Windows.Forms.TextBox();
            this.tbSecondaryDataType = new System.Windows.Forms.TextBox();
            this.tbPathList = new System.Windows.Forms.TextBox();
            this.lbPathList = new System.Windows.Forms.Label();
            this.tbReceivedTime = new System.Windows.Forms.TextBox();
            this.lbReceivedTime = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbPrimaryVHL = new System.Windows.Forms.TextBox();
            this.lbAcknowledge = new System.Windows.Forms.Label();
            this.lbSecondaryDataType = new System.Windows.Forms.Label();
            this.lbSecondaryVHL = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbSecondaryVHL = new System.Windows.Forms.TextBox();
            this.btnSendSecondaryMessage = new Sineva.VHL.Library.IButton();
            this.tbSentTime = new System.Windows.Forms.TextBox();
            this.lbSentTime = new System.Windows.Forms.Label();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.lbSource = new System.Windows.Forms.Label();
            this.tbDestnation = new System.Windows.Forms.TextBox();
            this.lbDestnation = new System.Windows.Forms.Label();
            this.cbPrimaryDataType = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbPrimaryDataType
            // 
            this.lbPrimaryDataType.AutoSize = true;
            this.lbPrimaryDataType.Location = new System.Drawing.Point(27, 79);
            this.lbPrimaryDataType.Name = "lbPrimaryDataType";
            this.lbPrimaryDataType.Size = new System.Drawing.Size(65, 12);
            this.lbPrimaryDataType.TabIndex = 6;
            this.lbPrimaryDataType.Text = "Data Type:";
            // 
            // lbPrimaryVHL
            // 
            this.lbPrimaryVHL.AutoSize = true;
            this.lbPrimaryVHL.Location = new System.Drawing.Point(27, 34);
            this.lbPrimaryVHL.Name = "lbPrimaryVHL";
            this.lbPrimaryVHL.Size = new System.Drawing.Size(95, 12);
            this.lbPrimaryVHL.TabIndex = 4;
            this.lbPrimaryVHL.Text = "Vehicle Number:";
            // 
            // tbAcknowledge
            // 
            this.tbAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAcknowledge.Location = new System.Drawing.Point(193, 120);
            this.tbAcknowledge.Name = "tbAcknowledge";
            this.tbAcknowledge.Size = new System.Drawing.Size(121, 21);
            this.tbAcknowledge.TabIndex = 26;
            // 
            // tbSecondaryDataType
            // 
            this.tbSecondaryDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSecondaryDataType.Location = new System.Drawing.Point(193, 76);
            this.tbSecondaryDataType.Name = "tbSecondaryDataType";
            this.tbSecondaryDataType.Size = new System.Drawing.Size(121, 21);
            this.tbSecondaryDataType.TabIndex = 25;
            // 
            // tbPathList
            // 
            this.tbPathList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPathList.Location = new System.Drawing.Point(193, 209);
            this.tbPathList.Name = "tbPathList";
            this.tbPathList.Size = new System.Drawing.Size(121, 21);
            this.tbPathList.TabIndex = 24;
            // 
            // lbPathList
            // 
            this.lbPathList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPathList.AutoSize = true;
            this.lbPathList.Location = new System.Drawing.Point(47, 212);
            this.lbPathList.Name = "lbPathList";
            this.lbPathList.Size = new System.Drawing.Size(65, 12);
            this.lbPathList.TabIndex = 23;
            this.lbPathList.Text = "Path List:";
            // 
            // tbReceivedTime
            // 
            this.tbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReceivedTime.Location = new System.Drawing.Point(193, 164);
            this.tbReceivedTime.Name = "tbReceivedTime";
            this.tbReceivedTime.Size = new System.Drawing.Size(121, 21);
            this.tbReceivedTime.TabIndex = 22;
            // 
            // lbReceivedTime
            // 
            this.lbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbReceivedTime.AutoSize = true;
            this.lbReceivedTime.Location = new System.Drawing.Point(47, 167);
            this.lbReceivedTime.Name = "lbReceivedTime";
            this.lbReceivedTime.Size = new System.Drawing.Size(89, 12);
            this.lbReceivedTime.TabIndex = 21;
            this.lbReceivedTime.Text = "Received Time:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbPrimaryDataType);
            this.groupBox1.Controls.Add(this.tbDestnation);
            this.groupBox1.Controls.Add(this.lbDestnation);
            this.groupBox1.Controls.Add(this.tbSource);
            this.groupBox1.Controls.Add(this.lbSource);
            this.groupBox1.Controls.Add(this.tbSentTime);
            this.groupBox1.Controls.Add(this.lbSentTime);
            this.groupBox1.Controls.Add(this.lbPrimaryDataType);
            this.groupBox1.Controls.Add(this.tbPrimaryVHL);
            this.groupBox1.Controls.Add(this.lbPrimaryVHL);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 245);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Message";
            // 
            // tbPrimaryVHL
            // 
            this.tbPrimaryVHL.Location = new System.Drawing.Point(146, 31);
            this.tbPrimaryVHL.Name = "tbPrimaryVHL";
            this.tbPrimaryVHL.Size = new System.Drawing.Size(121, 21);
            this.tbPrimaryVHL.TabIndex = 5;
            // 
            // lbAcknowledge
            // 
            this.lbAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAcknowledge.AutoSize = true;
            this.lbAcknowledge.Location = new System.Drawing.Point(47, 123);
            this.lbAcknowledge.Name = "lbAcknowledge";
            this.lbAcknowledge.Size = new System.Drawing.Size(77, 12);
            this.lbAcknowledge.TabIndex = 19;
            this.lbAcknowledge.Text = "Acknowledge:";
            // 
            // lbSecondaryDataType
            // 
            this.lbSecondaryDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryDataType.AutoSize = true;
            this.lbSecondaryDataType.Location = new System.Drawing.Point(47, 79);
            this.lbSecondaryDataType.Name = "lbSecondaryDataType";
            this.lbSecondaryDataType.Size = new System.Drawing.Size(65, 12);
            this.lbSecondaryDataType.TabIndex = 17;
            this.lbSecondaryDataType.Text = "Data Type:";
            // 
            // lbSecondaryVHL
            // 
            this.lbSecondaryVHL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryVHL.AutoSize = true;
            this.lbSecondaryVHL.Location = new System.Drawing.Point(47, 34);
            this.lbSecondaryVHL.Name = "lbSecondaryVHL";
            this.lbSecondaryVHL.Size = new System.Drawing.Size(95, 12);
            this.lbSecondaryVHL.TabIndex = 15;
            this.lbSecondaryVHL.Text = "Vehicle Number:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tbAcknowledge);
            this.groupBox2.Controls.Add(this.tbSecondaryDataType);
            this.groupBox2.Controls.Add(this.tbPathList);
            this.groupBox2.Controls.Add(this.lbPathList);
            this.groupBox2.Controls.Add(this.tbReceivedTime);
            this.groupBox2.Controls.Add(this.lbReceivedTime);
            this.groupBox2.Controls.Add(this.lbAcknowledge);
            this.groupBox2.Controls.Add(this.lbSecondaryDataType);
            this.groupBox2.Controls.Add(this.tbSecondaryVHL);
            this.groupBox2.Controls.Add(this.lbSecondaryVHL);
            this.groupBox2.Location = new System.Drawing.Point(345, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 245);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Message";
            // 
            // tbSecondaryVHL
            // 
            this.tbSecondaryVHL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSecondaryVHL.Location = new System.Drawing.Point(193, 31);
            this.tbSecondaryVHL.Name = "tbSecondaryVHL";
            this.tbSecondaryVHL.Size = new System.Drawing.Size(121, 21);
            this.tbSecondaryVHL.TabIndex = 16;
            // 
            // btnSendSecondaryMessage
            // 
            this.btnSendSecondaryMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendSecondaryMessage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSendSecondaryMessage.BgDefault = null;
            this.btnSendSecondaryMessage.BgDisable = null;
            this.btnSendSecondaryMessage.BgOver = null;
            this.btnSendSecondaryMessage.BgPushed = null;
            this.btnSendSecondaryMessage.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnSendSecondaryMessage.ConnectedLabel = null;
            this.btnSendSecondaryMessage.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnSendSecondaryMessage.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnSendSecondaryMessage.DefaultImage = null;
            this.btnSendSecondaryMessage.Description = null;
            this.btnSendSecondaryMessage.DownImage = null;
            this.btnSendSecondaryMessage.Location = new System.Drawing.Point(462, 284);
            this.btnSendSecondaryMessage.Name = "btnSendSecondaryMessage";
            this.btnSendSecondaryMessage.OverImage = null;
            this.btnSendSecondaryMessage.Size = new System.Drawing.Size(157, 23);
            this.btnSendSecondaryMessage.TabIndex = 13;
            this.btnSendSecondaryMessage.Text = "Send Secondary Message";
            this.btnSendSecondaryMessage.UpImage = null;
            this.btnSendSecondaryMessage.UseOneImage = false;
            this.btnSendSecondaryMessage.UseVisualStyleBackColor = true;
            // 
            // tbSentTime
            // 
            this.tbSentTime.Location = new System.Drawing.Point(146, 120);
            this.tbSentTime.Name = "tbSentTime";
            this.tbSentTime.Size = new System.Drawing.Size(121, 21);
            this.tbSentTime.TabIndex = 16;
            // 
            // lbSentTime
            // 
            this.lbSentTime.AutoSize = true;
            this.lbSentTime.Location = new System.Drawing.Point(27, 123);
            this.lbSentTime.Name = "lbSentTime";
            this.lbSentTime.Size = new System.Drawing.Size(65, 12);
            this.lbSentTime.TabIndex = 15;
            this.lbSentTime.Text = "Sent Time:";
            // 
            // tbSource
            // 
            this.tbSource.Location = new System.Drawing.Point(146, 164);
            this.tbSource.Name = "tbSource";
            this.tbSource.Size = new System.Drawing.Size(121, 21);
            this.tbSource.TabIndex = 18;
            // 
            // lbSource
            // 
            this.lbSource.AutoSize = true;
            this.lbSource.Location = new System.Drawing.Point(27, 167);
            this.lbSource.Name = "lbSource";
            this.lbSource.Size = new System.Drawing.Size(47, 12);
            this.lbSource.TabIndex = 17;
            this.lbSource.Text = "Source:";
            // 
            // tbDestnation
            // 
            this.tbDestnation.Location = new System.Drawing.Point(146, 209);
            this.tbDestnation.Name = "tbDestnation";
            this.tbDestnation.Size = new System.Drawing.Size(121, 21);
            this.tbDestnation.TabIndex = 20;
            // 
            // lbDestnation
            // 
            this.lbDestnation.AutoSize = true;
            this.lbDestnation.Location = new System.Drawing.Point(27, 212);
            this.lbDestnation.Name = "lbDestnation";
            this.lbDestnation.Size = new System.Drawing.Size(71, 12);
            this.lbDestnation.TabIndex = 19;
            this.lbDestnation.Text = "Destnation:";
            // 
            // cbPrimaryDataType
            // 
            this.cbPrimaryDataType.FormattingEnabled = true;
            this.cbPrimaryDataType.Location = new System.Drawing.Point(146, 76);
            this.cbPrimaryDataType.Name = "cbPrimaryDataType";
            this.cbPrimaryDataType.Size = new System.Drawing.Size(121, 20);
            this.cbPrimaryDataType.TabIndex = 21;
            // 
            // FrmPathRequestMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(806, 326);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSendSecondaryMessage);
            this.Name = "FrmPathRequestMessage";
            this.Text = "FrmPathRequestMessage";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbPrimaryDataType;
        private System.Windows.Forms.Label lbPrimaryVHL;
        private System.Windows.Forms.TextBox tbAcknowledge;
        private System.Windows.Forms.TextBox tbSecondaryDataType;
        private System.Windows.Forms.TextBox tbPathList;
        private System.Windows.Forms.Label lbPathList;
        private System.Windows.Forms.TextBox tbReceivedTime;
        private System.Windows.Forms.Label lbReceivedTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbPrimaryDataType;
        private System.Windows.Forms.TextBox tbDestnation;
        private System.Windows.Forms.Label lbDestnation;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.Label lbSource;
        private System.Windows.Forms.TextBox tbSentTime;
        private System.Windows.Forms.Label lbSentTime;
        private System.Windows.Forms.TextBox tbPrimaryVHL;
        private System.Windows.Forms.Label lbAcknowledge;
        private System.Windows.Forms.Label lbSecondaryDataType;
        private System.Windows.Forms.Label lbSecondaryVHL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSecondaryVHL;
        private Library.IButton btnSendSecondaryMessage;
    }
}