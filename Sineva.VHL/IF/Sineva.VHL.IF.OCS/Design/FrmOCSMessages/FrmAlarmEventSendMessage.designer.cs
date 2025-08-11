
namespace Sineva.VHL.IF.OCS
{
    partial class FrmAlarmEventSendMessage
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
            this.lbAlarmType = new System.Windows.Forms.Label();
            this.lbReceivedTime = new System.Windows.Forms.Label();
            this.lbAcknowledge = new System.Windows.Forms.Label();
            this.tbSecondaryVHL = new System.Windows.Forms.TextBox();
            this.tbAlarmID = new System.Windows.Forms.TextBox();
            this.tbSentTime = new System.Windows.Forms.TextBox();
            this.lbSentTime = new System.Windows.Forms.Label();
            this.tbAlarmCode = new System.Windows.Forms.TextBox();
            this.lbAlarmCode = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbReceivedTime = new System.Windows.Forms.TextBox();
            this.tbAcknowledge = new System.Windows.Forms.TextBox();
            this.lbSecondaryVHL = new System.Windows.Forms.Label();
            this.lbAlarmStatus = new System.Windows.Forms.Label();
            this.lbAlarmID = new System.Windows.Forms.Label();
            this.tbPrimaryVHL = new System.Windows.Forms.TextBox();
            this.lbPrimaryVHL = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAlarmType = new System.Windows.Forms.ComboBox();
            this.cbAlarmStatus = new System.Windows.Forms.ComboBox();
            this.btnSendPrimaryMessage = new Sineva.VHL.Library.IButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbAlarmType
            // 
            this.lbAlarmType.AutoSize = true;
            this.lbAlarmType.Location = new System.Drawing.Point(327, 34);
            this.lbAlarmType.Name = "lbAlarmType";
            this.lbAlarmType.Size = new System.Drawing.Size(71, 12);
            this.lbAlarmType.TabIndex = 15;
            this.lbAlarmType.Text = "Alarm Type:";
            // 
            // lbReceivedTime
            // 
            this.lbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbReceivedTime.AutoSize = true;
            this.lbReceivedTime.Location = new System.Drawing.Point(74, 123);
            this.lbReceivedTime.Name = "lbReceivedTime";
            this.lbReceivedTime.Size = new System.Drawing.Size(89, 12);
            this.lbReceivedTime.TabIndex = 19;
            this.lbReceivedTime.Text = "Received Time:";
            // 
            // lbAcknowledge
            // 
            this.lbAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAcknowledge.AutoSize = true;
            this.lbAcknowledge.Location = new System.Drawing.Point(74, 79);
            this.lbAcknowledge.Name = "lbAcknowledge";
            this.lbAcknowledge.Size = new System.Drawing.Size(77, 12);
            this.lbAcknowledge.TabIndex = 17;
            this.lbAcknowledge.Text = "Acknowledge:";
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
            // tbAlarmID
            // 
            this.tbAlarmID.Location = new System.Drawing.Point(146, 76);
            this.tbAlarmID.Name = "tbAlarmID";
            this.tbAlarmID.Size = new System.Drawing.Size(121, 21);
            this.tbAlarmID.TabIndex = 14;
            // 
            // tbSentTime
            // 
            this.tbSentTime.Location = new System.Drawing.Point(446, 116);
            this.tbSentTime.Name = "tbSentTime";
            this.tbSentTime.Size = new System.Drawing.Size(121, 21);
            this.tbSentTime.TabIndex = 13;
            // 
            // lbSentTime
            // 
            this.lbSentTime.AutoSize = true;
            this.lbSentTime.Location = new System.Drawing.Point(327, 119);
            this.lbSentTime.Name = "lbSentTime";
            this.lbSentTime.Size = new System.Drawing.Size(65, 12);
            this.lbSentTime.TabIndex = 12;
            this.lbSentTime.Text = "Sent Time:";
            // 
            // tbAlarmCode
            // 
            this.tbAlarmCode.Location = new System.Drawing.Point(446, 76);
            this.tbAlarmCode.Name = "tbAlarmCode";
            this.tbAlarmCode.Size = new System.Drawing.Size(121, 21);
            this.tbAlarmCode.TabIndex = 11;
            // 
            // lbAlarmCode
            // 
            this.lbAlarmCode.AutoSize = true;
            this.lbAlarmCode.Location = new System.Drawing.Point(327, 79);
            this.lbAlarmCode.Name = "lbAlarmCode";
            this.lbAlarmCode.Size = new System.Drawing.Size(71, 12);
            this.lbAlarmCode.TabIndex = 10;
            this.lbAlarmCode.Text = "Alarm Code:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tbReceivedTime);
            this.groupBox2.Controls.Add(this.tbAcknowledge);
            this.groupBox2.Controls.Add(this.lbReceivedTime);
            this.groupBox2.Controls.Add(this.lbAcknowledge);
            this.groupBox2.Controls.Add(this.tbSecondaryVHL);
            this.groupBox2.Controls.Add(this.lbSecondaryVHL);
            this.groupBox2.Location = new System.Drawing.Point(613, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 170);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Message";
            // 
            // tbReceivedTime
            // 
            this.tbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReceivedTime.Location = new System.Drawing.Point(193, 116);
            this.tbReceivedTime.Name = "tbReceivedTime";
            this.tbReceivedTime.Size = new System.Drawing.Size(121, 21);
            this.tbReceivedTime.TabIndex = 21;
            // 
            // tbAcknowledge
            // 
            this.tbAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAcknowledge.Location = new System.Drawing.Point(193, 76);
            this.tbAcknowledge.Name = "tbAcknowledge";
            this.tbAcknowledge.Size = new System.Drawing.Size(121, 21);
            this.tbAcknowledge.TabIndex = 20;
            // 
            // lbSecondaryVHL
            // 
            this.lbSecondaryVHL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryVHL.AutoSize = true;
            this.lbSecondaryVHL.Location = new System.Drawing.Point(74, 34);
            this.lbSecondaryVHL.Name = "lbSecondaryVHL";
            this.lbSecondaryVHL.Size = new System.Drawing.Size(95, 12);
            this.lbSecondaryVHL.TabIndex = 15;
            this.lbSecondaryVHL.Text = "Vehicle Number:";
            // 
            // lbAlarmStatus
            // 
            this.lbAlarmStatus.AutoSize = true;
            this.lbAlarmStatus.Location = new System.Drawing.Point(27, 123);
            this.lbAlarmStatus.Name = "lbAlarmStatus";
            this.lbAlarmStatus.Size = new System.Drawing.Size(83, 12);
            this.lbAlarmStatus.TabIndex = 8;
            this.lbAlarmStatus.Text = "Alarm Status:";
            // 
            // lbAlarmID
            // 
            this.lbAlarmID.AutoSize = true;
            this.lbAlarmID.Location = new System.Drawing.Point(27, 79);
            this.lbAlarmID.Name = "lbAlarmID";
            this.lbAlarmID.Size = new System.Drawing.Size(59, 12);
            this.lbAlarmID.TabIndex = 6;
            this.lbAlarmID.Text = "Alarm ID:";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAlarmType);
            this.groupBox1.Controls.Add(this.cbAlarmStatus);
            this.groupBox1.Controls.Add(this.lbAlarmType);
            this.groupBox1.Controls.Add(this.tbAlarmID);
            this.groupBox1.Controls.Add(this.tbSentTime);
            this.groupBox1.Controls.Add(this.lbSentTime);
            this.groupBox1.Controls.Add(this.tbAlarmCode);
            this.groupBox1.Controls.Add(this.lbAlarmCode);
            this.groupBox1.Controls.Add(this.lbAlarmStatus);
            this.groupBox1.Controls.Add(this.lbAlarmID);
            this.groupBox1.Controls.Add(this.tbPrimaryVHL);
            this.groupBox1.Controls.Add(this.lbPrimaryVHL);
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(575, 170);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Message";
            // 
            // cbAlarmType
            // 
            this.cbAlarmType.FormattingEnabled = true;
            this.cbAlarmType.Location = new System.Drawing.Point(446, 31);
            this.cbAlarmType.Name = "cbAlarmType";
            this.cbAlarmType.Size = new System.Drawing.Size(121, 20);
            this.cbAlarmType.TabIndex = 18;
            // 
            // cbAlarmStatus
            // 
            this.cbAlarmStatus.FormattingEnabled = true;
            this.cbAlarmStatus.Location = new System.Drawing.Point(146, 120);
            this.cbAlarmStatus.Name = "cbAlarmStatus";
            this.cbAlarmStatus.Size = new System.Drawing.Size(121, 20);
            this.cbAlarmStatus.TabIndex = 17;
            // 
            // btnSendPrimaryMessage
            // 
            this.btnSendPrimaryMessage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSendPrimaryMessage.BgDefault = null;
            this.btnSendPrimaryMessage.BgDisable = null;
            this.btnSendPrimaryMessage.BgOver = null;
            this.btnSendPrimaryMessage.BgPushed = null;
            this.btnSendPrimaryMessage.ButtonType = Sineva.VHL.Library.IButton.enButtonType.Normal;
            this.btnSendPrimaryMessage.ConnectedLabel = null;
            this.btnSendPrimaryMessage.ConnectedLableOffColor = System.Drawing.Color.Empty;
            this.btnSendPrimaryMessage.ConnectedLableOnColor = System.Drawing.Color.Empty;
            this.btnSendPrimaryMessage.DefaultImage = null;
            this.btnSendPrimaryMessage.Description = null;
            this.btnSendPrimaryMessage.DownImage = null;
            this.btnSendPrimaryMessage.Location = new System.Drawing.Point(143, 212);
            this.btnSendPrimaryMessage.Name = "btnSendPrimaryMessage";
            this.btnSendPrimaryMessage.OverImage = null;
            this.btnSendPrimaryMessage.Size = new System.Drawing.Size(157, 23);
            this.btnSendPrimaryMessage.TabIndex = 11;
            this.btnSendPrimaryMessage.Text = "Send Primary Message";
            this.btnSendPrimaryMessage.UpImage = null;
            this.btnSendPrimaryMessage.UseOneImage = false;
            this.btnSendPrimaryMessage.UseVisualStyleBackColor = true;
            // 
            // FrmAlarmEventSendMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1015, 263);
            this.Controls.Add(this.btnSendPrimaryMessage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmAlarmEventSendMessage";
            this.Text = "FrmAlarmEventSendMessage";
            this.Load += new System.EventHandler(this.FrmAlarmEventSendMessage_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbAlarmType;
        private System.Windows.Forms.Label lbReceivedTime;
        private System.Windows.Forms.Label lbAcknowledge;
        private System.Windows.Forms.TextBox tbSecondaryVHL;
        private System.Windows.Forms.TextBox tbAlarmID;
        private System.Windows.Forms.TextBox tbSentTime;
        private System.Windows.Forms.Label lbSentTime;
        private System.Windows.Forms.TextBox tbAlarmCode;
        private System.Windows.Forms.Label lbAlarmCode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbSecondaryVHL;
        private System.Windows.Forms.Label lbAlarmStatus;
        private System.Windows.Forms.Label lbAlarmID;
        private System.Windows.Forms.TextBox tbPrimaryVHL;
        private System.Windows.Forms.Label lbPrimaryVHL;
        private System.Windows.Forms.GroupBox groupBox1;
        private Library.IButton btnSendPrimaryMessage;
        private System.Windows.Forms.ComboBox cbAlarmType;
        private System.Windows.Forms.ComboBox cbAlarmStatus;
        private System.Windows.Forms.TextBox tbReceivedTime;
        private System.Windows.Forms.TextBox tbAcknowledge;
    }
}