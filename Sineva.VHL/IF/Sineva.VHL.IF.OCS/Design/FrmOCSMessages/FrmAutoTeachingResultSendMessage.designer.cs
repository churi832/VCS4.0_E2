
namespace Sineva.VHL.IF.OCS
{
    partial class FrmAutoTeachingResultSendMessage
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
            this.btnSendPrimaryMessage = new Sineva.VHL.Library.IButton();
            this.lbPrimaryResult = new System.Windows.Forms.Label();
            this.lbPrimaryPortID = new System.Windows.Forms.Label();
            this.tbPrimaryVHL = new System.Windows.Forms.TextBox();
            this.lbPrimaryVHL = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbSecondaryPortID = new System.Windows.Forms.TextBox();
            this.tbReceivedTime = new System.Windows.Forms.TextBox();
            this.cbSecondaryResult = new System.Windows.Forms.ComboBox();
            this.lbSecondaryResult = new System.Windows.Forms.Label();
            this.lbReceivedTime = new System.Windows.Forms.Label();
            this.lbSecondaryPortID = new System.Windows.Forms.Label();
            this.tbSecondaryVHL = new System.Windows.Forms.TextBox();
            this.lbSecondaryVHL = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbPrimaryResult = new System.Windows.Forms.ComboBox();
            this.tbPrimaryPortID = new System.Windows.Forms.TextBox();
            this.tbSentTime = new System.Windows.Forms.TextBox();
            this.lbSentTime = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            this.btnSendPrimaryMessage.Location = new System.Drawing.Point(85, 242);
            this.btnSendPrimaryMessage.Name = "btnSendPrimaryMessage";
            this.btnSendPrimaryMessage.OverImage = null;
            this.btnSendPrimaryMessage.Size = new System.Drawing.Size(157, 23);
            this.btnSendPrimaryMessage.TabIndex = 14;
            this.btnSendPrimaryMessage.Text = "Send Primary Message";
            this.btnSendPrimaryMessage.UpImage = null;
            this.btnSendPrimaryMessage.UseOneImage = false;
            this.btnSendPrimaryMessage.UseVisualStyleBackColor = true;
            // 
            // lbPrimaryResult
            // 
            this.lbPrimaryResult.AutoSize = true;
            this.lbPrimaryResult.Location = new System.Drawing.Point(25, 123);
            this.lbPrimaryResult.Name = "lbPrimaryResult";
            this.lbPrimaryResult.Size = new System.Drawing.Size(47, 12);
            this.lbPrimaryResult.TabIndex = 8;
            this.lbPrimaryResult.Text = "Result:";
            // 
            // lbPrimaryPortID
            // 
            this.lbPrimaryPortID.AutoSize = true;
            this.lbPrimaryPortID.Location = new System.Drawing.Point(25, 79);
            this.lbPrimaryPortID.Name = "lbPrimaryPortID";
            this.lbPrimaryPortID.Size = new System.Drawing.Size(53, 12);
            this.lbPrimaryPortID.TabIndex = 6;
            this.lbPrimaryPortID.Text = "Port ID:";
            // 
            // tbPrimaryVHL
            // 
            this.tbPrimaryVHL.Location = new System.Drawing.Point(145, 31);
            this.tbPrimaryVHL.Name = "tbPrimaryVHL";
            this.tbPrimaryVHL.Size = new System.Drawing.Size(121, 21);
            this.tbPrimaryVHL.TabIndex = 5;
            // 
            // lbPrimaryVHL
            // 
            this.lbPrimaryVHL.AutoSize = true;
            this.lbPrimaryVHL.Location = new System.Drawing.Point(25, 34);
            this.lbPrimaryVHL.Name = "lbPrimaryVHL";
            this.lbPrimaryVHL.Size = new System.Drawing.Size(95, 12);
            this.lbPrimaryVHL.TabIndex = 4;
            this.lbPrimaryVHL.Text = "Vehicle Number:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbSecondaryPortID);
            this.groupBox2.Controls.Add(this.tbReceivedTime);
            this.groupBox2.Controls.Add(this.cbSecondaryResult);
            this.groupBox2.Controls.Add(this.lbSecondaryResult);
            this.groupBox2.Controls.Add(this.lbReceivedTime);
            this.groupBox2.Controls.Add(this.lbSecondaryPortID);
            this.groupBox2.Controls.Add(this.tbSecondaryVHL);
            this.groupBox2.Controls.Add(this.lbSecondaryVHL);
            this.groupBox2.Location = new System.Drawing.Point(399, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 210);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Message";
            // 
            // tbSecondaryPortID
            // 
            this.tbSecondaryPortID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSecondaryPortID.Location = new System.Drawing.Point(193, 76);
            this.tbSecondaryPortID.Name = "tbSecondaryPortID";
            this.tbSecondaryPortID.Size = new System.Drawing.Size(121, 21);
            this.tbSecondaryPortID.TabIndex = 24;
            // 
            // tbReceivedTime
            // 
            this.tbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReceivedTime.Location = new System.Drawing.Point(193, 163);
            this.tbReceivedTime.Name = "tbReceivedTime";
            this.tbReceivedTime.Size = new System.Drawing.Size(121, 21);
            this.tbReceivedTime.TabIndex = 23;
            // 
            // cbSecondaryResult
            // 
            this.cbSecondaryResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSecondaryResult.FormattingEnabled = true;
            this.cbSecondaryResult.Location = new System.Drawing.Point(193, 120);
            this.cbSecondaryResult.Name = "cbSecondaryResult";
            this.cbSecondaryResult.Size = new System.Drawing.Size(121, 20);
            this.cbSecondaryResult.TabIndex = 22;
            // 
            // lbSecondaryResult
            // 
            this.lbSecondaryResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryResult.AutoSize = true;
            this.lbSecondaryResult.Location = new System.Drawing.Point(74, 123);
            this.lbSecondaryResult.Name = "lbSecondaryResult";
            this.lbSecondaryResult.Size = new System.Drawing.Size(47, 12);
            this.lbSecondaryResult.TabIndex = 21;
            this.lbSecondaryResult.Text = "Result:";
            // 
            // lbReceivedTime
            // 
            this.lbReceivedTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbReceivedTime.AutoSize = true;
            this.lbReceivedTime.Location = new System.Drawing.Point(74, 166);
            this.lbReceivedTime.Name = "lbReceivedTime";
            this.lbReceivedTime.Size = new System.Drawing.Size(89, 12);
            this.lbReceivedTime.TabIndex = 19;
            this.lbReceivedTime.Text = "Received Time:";
            // 
            // lbSecondaryPortID
            // 
            this.lbSecondaryPortID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryPortID.AutoSize = true;
            this.lbSecondaryPortID.Location = new System.Drawing.Point(74, 79);
            this.lbSecondaryPortID.Name = "lbSecondaryPortID";
            this.lbSecondaryPortID.Size = new System.Drawing.Size(53, 12);
            this.lbSecondaryPortID.TabIndex = 17;
            this.lbSecondaryPortID.Text = "Port ID:";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbPrimaryResult);
            this.groupBox1.Controls.Add(this.tbPrimaryPortID);
            this.groupBox1.Controls.Add(this.tbSentTime);
            this.groupBox1.Controls.Add(this.lbSentTime);
            this.groupBox1.Controls.Add(this.lbPrimaryResult);
            this.groupBox1.Controls.Add(this.lbPrimaryPortID);
            this.groupBox1.Controls.Add(this.tbPrimaryVHL);
            this.groupBox1.Controls.Add(this.lbPrimaryVHL);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 210);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Message";
            // 
            // cbPrimaryResult
            // 
            this.cbPrimaryResult.FormattingEnabled = true;
            this.cbPrimaryResult.Location = new System.Drawing.Point(146, 120);
            this.cbPrimaryResult.Name = "cbPrimaryResult";
            this.cbPrimaryResult.Size = new System.Drawing.Size(119, 20);
            this.cbPrimaryResult.TabIndex = 19;
            // 
            // tbPrimaryPortID
            // 
            this.tbPrimaryPortID.Location = new System.Drawing.Point(145, 76);
            this.tbPrimaryPortID.Name = "tbPrimaryPortID";
            this.tbPrimaryPortID.Size = new System.Drawing.Size(121, 21);
            this.tbPrimaryPortID.TabIndex = 14;
            // 
            // tbSentTime
            // 
            this.tbSentTime.Location = new System.Drawing.Point(145, 163);
            this.tbSentTime.Name = "tbSentTime";
            this.tbSentTime.Size = new System.Drawing.Size(121, 21);
            this.tbSentTime.TabIndex = 13;
            // 
            // lbSentTime
            // 
            this.lbSentTime.AutoSize = true;
            this.lbSentTime.Location = new System.Drawing.Point(25, 166);
            this.lbSentTime.Name = "lbSentTime";
            this.lbSentTime.Size = new System.Drawing.Size(65, 12);
            this.lbSentTime.TabIndex = 12;
            this.lbSentTime.Text = "Sent Time:";
            // 
            // FrmAutoTeachingResultSendMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(813, 288);
            this.Controls.Add(this.btnSendPrimaryMessage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmAutoTeachingResultSendMessage";
            this.Text = "FrmAutoTeachingResultSendMessage";
            this.Load += new System.EventHandler(this.FrmAutoTeachingResultSendMessage_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Library.IButton btnSendPrimaryMessage;
        private System.Windows.Forms.Label lbPrimaryResult;
        private System.Windows.Forms.Label lbPrimaryPortID;
        private System.Windows.Forms.TextBox tbPrimaryVHL;
        private System.Windows.Forms.Label lbPrimaryVHL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbReceivedTime;
        private System.Windows.Forms.Label lbSecondaryPortID;
        private System.Windows.Forms.TextBox tbSecondaryVHL;
        private System.Windows.Forms.Label lbSecondaryVHL;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbPrimaryPortID;
        private System.Windows.Forms.TextBox tbSentTime;
        private System.Windows.Forms.Label lbSentTime;
        private System.Windows.Forms.TextBox tbSecondaryPortID;
        private System.Windows.Forms.TextBox tbReceivedTime;
        private System.Windows.Forms.ComboBox cbSecondaryResult;
        private System.Windows.Forms.Label lbSecondaryResult;
        private System.Windows.Forms.ComboBox cbPrimaryResult;
    }
}