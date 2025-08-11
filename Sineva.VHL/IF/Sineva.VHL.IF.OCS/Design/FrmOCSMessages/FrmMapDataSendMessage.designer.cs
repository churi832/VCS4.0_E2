
namespace Sineva.VHL.IF.OCS
{
    partial class FrmMapDataSendMessage
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbPrimaryDataType = new System.Windows.Forms.Label();
            this.tbDataVersion = new System.Windows.Forms.TextBox();
            this.lbDataVersion = new System.Windows.Forms.Label();
            this.tbPrimaryVHL = new System.Windows.Forms.TextBox();
            this.lbPrimaryVHL = new System.Windows.Forms.Label();
            this.lbSecondaryVHL = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbSentTime = new System.Windows.Forms.TextBox();
            this.lbAcknowedge = new System.Windows.Forms.Label();
            this.lbSentTime = new System.Windows.Forms.Label();
            this.lbSecondaryDataType = new System.Windows.Forms.Label();
            this.tbSecondaryVHL = new System.Windows.Forms.TextBox();
            this.btnSendSecondaryMessage = new Sineva.VHL.Library.IButton();
            this.tbPrimaryDataType = new System.Windows.Forms.TextBox();
            this.tbReceivedTime = new System.Windows.Forms.TextBox();
            this.lbReceivedTime = new System.Windows.Forms.Label();
            this.tbFileStream = new System.Windows.Forms.TextBox();
            this.lbFileStream = new System.Windows.Forms.Label();
            this.cbSecondaryDataType = new System.Windows.Forms.ComboBox();
            this.cbAcknowledge = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbFileStream);
            this.groupBox1.Controls.Add(this.lbFileStream);
            this.groupBox1.Controls.Add(this.tbReceivedTime);
            this.groupBox1.Controls.Add(this.lbReceivedTime);
            this.groupBox1.Controls.Add(this.tbPrimaryDataType);
            this.groupBox1.Controls.Add(this.lbPrimaryDataType);
            this.groupBox1.Controls.Add(this.tbDataVersion);
            this.groupBox1.Controls.Add(this.lbDataVersion);
            this.groupBox1.Controls.Add(this.tbPrimaryVHL);
            this.groupBox1.Controls.Add(this.lbPrimaryVHL);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 268);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Message";
            // 
            // lbPrimaryDataType
            // 
            this.lbPrimaryDataType.AutoSize = true;
            this.lbPrimaryDataType.Location = new System.Drawing.Point(27, 80);
            this.lbPrimaryDataType.Name = "lbPrimaryDataType";
            this.lbPrimaryDataType.Size = new System.Drawing.Size(65, 12);
            this.lbPrimaryDataType.TabIndex = 15;
            this.lbPrimaryDataType.Text = "Data Type:";
            // 
            // tbDataVersion
            // 
            this.tbDataVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataVersion.Location = new System.Drawing.Point(146, 122);
            this.tbDataVersion.Name = "tbDataVersion";
            this.tbDataVersion.Size = new System.Drawing.Size(121, 21);
            this.tbDataVersion.TabIndex = 24;
            // 
            // lbDataVersion
            // 
            this.lbDataVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDataVersion.AutoSize = true;
            this.lbDataVersion.Location = new System.Drawing.Point(27, 126);
            this.lbDataVersion.Name = "lbDataVersion";
            this.lbDataVersion.Size = new System.Drawing.Size(83, 12);
            this.lbDataVersion.TabIndex = 23;
            this.lbDataVersion.Text = "Data Version:";
            // 
            // tbPrimaryVHL
            // 
            this.tbPrimaryVHL.Location = new System.Drawing.Point(146, 31);
            this.tbPrimaryVHL.Name = "tbPrimaryVHL";
            this.tbPrimaryVHL.Size = new System.Drawing.Size(121, 21);
            this.tbPrimaryVHL.TabIndex = 5;
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
            this.groupBox2.Controls.Add(this.cbAcknowledge);
            this.groupBox2.Controls.Add(this.cbSecondaryDataType);
            this.groupBox2.Controls.Add(this.tbSentTime);
            this.groupBox2.Controls.Add(this.lbAcknowedge);
            this.groupBox2.Controls.Add(this.lbSentTime);
            this.groupBox2.Controls.Add(this.lbSecondaryDataType);
            this.groupBox2.Controls.Add(this.tbSecondaryVHL);
            this.groupBox2.Controls.Add(this.lbSecondaryVHL);
            this.groupBox2.Location = new System.Drawing.Point(328, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 218);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Message";
            // 
            // tbSentTime
            // 
            this.tbSentTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSentTime.Location = new System.Drawing.Point(193, 169);
            this.tbSentTime.Name = "tbSentTime";
            this.tbSentTime.Size = new System.Drawing.Size(121, 21);
            this.tbSentTime.TabIndex = 14;
            // 
            // lbAcknowedge
            // 
            this.lbAcknowedge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAcknowedge.AutoSize = true;
            this.lbAcknowedge.Location = new System.Drawing.Point(47, 126);
            this.lbAcknowedge.Name = "lbAcknowedge";
            this.lbAcknowedge.Size = new System.Drawing.Size(77, 12);
            this.lbAcknowedge.TabIndex = 19;
            this.lbAcknowedge.Text = "Acknowledge:";
            // 
            // lbSentTime
            // 
            this.lbSentTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSentTime.AutoSize = true;
            this.lbSentTime.Location = new System.Drawing.Point(47, 172);
            this.lbSentTime.Name = "lbSentTime";
            this.lbSentTime.Size = new System.Drawing.Size(65, 12);
            this.lbSentTime.TabIndex = 6;
            this.lbSentTime.Text = "Sent Time:";
            // 
            // lbSecondaryDataType
            // 
            this.lbSecondaryDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryDataType.AutoSize = true;
            this.lbSecondaryDataType.Location = new System.Drawing.Point(47, 80);
            this.lbSecondaryDataType.Name = "lbSecondaryDataType";
            this.lbSecondaryDataType.Size = new System.Drawing.Size(65, 12);
            this.lbSecondaryDataType.TabIndex = 17;
            this.lbSecondaryDataType.Text = "Data Type:";
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
            this.btnSendSecondaryMessage.Location = new System.Drawing.Point(465, 296);
            this.btnSendSecondaryMessage.Name = "btnSendSecondaryMessage";
            this.btnSendSecondaryMessage.OverImage = null;
            this.btnSendSecondaryMessage.Size = new System.Drawing.Size(157, 23);
            this.btnSendSecondaryMessage.TabIndex = 20;
            this.btnSendSecondaryMessage.Text = "Send Secondary Message";
            this.btnSendSecondaryMessage.UpImage = null;
            this.btnSendSecondaryMessage.UseOneImage = false;
            this.btnSendSecondaryMessage.UseVisualStyleBackColor = true;
            // 
            // tbPrimaryDataType
            // 
            this.tbPrimaryDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPrimaryDataType.Location = new System.Drawing.Point(146, 77);
            this.tbPrimaryDataType.Name = "tbPrimaryDataType";
            this.tbPrimaryDataType.Size = new System.Drawing.Size(121, 21);
            this.tbPrimaryDataType.TabIndex = 25;
            // 
            // tbReceivedTime
            // 
            this.tbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReceivedTime.Location = new System.Drawing.Point(146, 222);
            this.tbReceivedTime.Name = "tbReceivedTime";
            this.tbReceivedTime.Size = new System.Drawing.Size(121, 21);
            this.tbReceivedTime.TabIndex = 27;
            // 
            // lbReceivedTime
            // 
            this.lbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbReceivedTime.AutoSize = true;
            this.lbReceivedTime.Location = new System.Drawing.Point(15, 225);
            this.lbReceivedTime.Name = "lbReceivedTime";
            this.lbReceivedTime.Size = new System.Drawing.Size(89, 12);
            this.lbReceivedTime.TabIndex = 26;
            this.lbReceivedTime.Text = "Received Time:";
            // 
            // tbFileStream
            // 
            this.tbFileStream.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFileStream.Location = new System.Drawing.Point(146, 169);
            this.tbFileStream.Name = "tbFileStream";
            this.tbFileStream.Size = new System.Drawing.Size(121, 21);
            this.tbFileStream.TabIndex = 29;
            // 
            // lbFileStream
            // 
            this.lbFileStream.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFileStream.AutoSize = true;
            this.lbFileStream.Location = new System.Drawing.Point(27, 173);
            this.lbFileStream.Name = "lbFileStream";
            this.lbFileStream.Size = new System.Drawing.Size(77, 12);
            this.lbFileStream.TabIndex = 28;
            this.lbFileStream.Text = "File Stream:";
            // 
            // cbSecondaryDataType
            // 
            this.cbSecondaryDataType.FormattingEnabled = true;
            this.cbSecondaryDataType.Location = new System.Drawing.Point(193, 77);
            this.cbSecondaryDataType.Name = "cbSecondaryDataType";
            this.cbSecondaryDataType.Size = new System.Drawing.Size(121, 20);
            this.cbSecondaryDataType.TabIndex = 20;
            // 
            // cbAcknowledge
            // 
            this.cbAcknowledge.FormattingEnabled = true;
            this.cbAcknowledge.Location = new System.Drawing.Point(193, 123);
            this.cbAcknowledge.Name = "cbAcknowledge";
            this.cbAcknowledge.Size = new System.Drawing.Size(121, 20);
            this.cbAcknowledge.TabIndex = 21;
            // 
            // FrmMapDataSendMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(737, 348);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSendSecondaryMessage);
            this.Name = "FrmMapDataSendMessage";
            this.Text = "FrmMapDataSendMessage";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbPrimaryDataType;
        private System.Windows.Forms.TextBox tbDataVersion;
        private System.Windows.Forms.Label lbDataVersion;
        private System.Windows.Forms.TextBox tbPrimaryVHL;
        private System.Windows.Forms.Label lbPrimaryVHL;
        private System.Windows.Forms.Label lbSecondaryVHL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSentTime;
        private System.Windows.Forms.Label lbAcknowedge;
        private System.Windows.Forms.Label lbSentTime;
        private System.Windows.Forms.Label lbSecondaryDataType;
        private System.Windows.Forms.TextBox tbSecondaryVHL;
        private Library.IButton btnSendSecondaryMessage;
        private System.Windows.Forms.TextBox tbFileStream;
        private System.Windows.Forms.Label lbFileStream;
        private System.Windows.Forms.TextBox tbReceivedTime;
        private System.Windows.Forms.Label lbReceivedTime;
        private System.Windows.Forms.TextBox tbPrimaryDataType;
        private System.Windows.Forms.ComboBox cbAcknowledge;
        private System.Windows.Forms.ComboBox cbSecondaryDataType;
    }
}