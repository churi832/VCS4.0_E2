
namespace Sineva.VHL.IF.OCS
{
    partial class FrmCommandSendMessage
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
            this.tbDestinationID = new System.Windows.Forms.TextBox();
            this.lbDestinationID = new System.Windows.Forms.Label();
            this.tbSourceID = new System.Windows.Forms.TextBox();
            this.lbSourceID = new System.Windows.Forms.Label();
            this.tbDataSendInterval = new System.Windows.Forms.TextBox();
            this.lbDataSendInterval = new System.Windows.Forms.Label();
            this.lbPrimaryCommand = new System.Windows.Forms.Label();
            this.tbPrimaryVHL = new System.Windows.Forms.TextBox();
            this.lbPrimaryVHL = new System.Windows.Forms.Label();
            this.tbPrimaryCommand = new System.Windows.Forms.TextBox();
            this.tbCommandID = new System.Windows.Forms.TextBox();
            this.lbCommandID = new System.Windows.Forms.Label();
            this.tbCarrierID = new System.Windows.Forms.TextBox();
            this.lbCarrierID = new System.Windows.Forms.Label();
            this.tbNodes = new System.Windows.Forms.TextBox();
            this.lbNodes = new System.Windows.Forms.Label();
            this.tbNodeCount = new System.Windows.Forms.TextBox();
            this.lbNodeCount = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbSecondaryCommand = new System.Windows.Forms.Label();
            this.tbSecondaryVHL = new System.Windows.Forms.TextBox();
            this.lbSecondaryVHL = new System.Windows.Forms.Label();
            this.cbSecondaryCommand = new System.Windows.Forms.ComboBox();
            this.cbAcknowledge = new System.Windows.Forms.ComboBox();
            this.lbAcknowledge = new System.Windows.Forms.Label();
            this.tbSentTime = new System.Windows.Forms.TextBox();
            this.lbSentTime = new System.Windows.Forms.Label();
            this.btnSendSecondaryMessage = new Sineva.VHL.Library.IButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbNodes);
            this.groupBox1.Controls.Add(this.lbNodes);
            this.groupBox1.Controls.Add(this.tbNodeCount);
            this.groupBox1.Controls.Add(this.lbNodeCount);
            this.groupBox1.Controls.Add(this.tbCarrierID);
            this.groupBox1.Controls.Add(this.lbCarrierID);
            this.groupBox1.Controls.Add(this.tbCommandID);
            this.groupBox1.Controls.Add(this.lbCommandID);
            this.groupBox1.Controls.Add(this.tbPrimaryCommand);
            this.groupBox1.Controls.Add(this.tbDestinationID);
            this.groupBox1.Controls.Add(this.lbDestinationID);
            this.groupBox1.Controls.Add(this.tbSourceID);
            this.groupBox1.Controls.Add(this.lbSourceID);
            this.groupBox1.Controls.Add(this.tbDataSendInterval);
            this.groupBox1.Controls.Add(this.lbDataSendInterval);
            this.groupBox1.Controls.Add(this.lbPrimaryCommand);
            this.groupBox1.Controls.Add(this.tbPrimaryVHL);
            this.groupBox1.Controls.Add(this.lbPrimaryVHL);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(558, 245);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Primary Message";
            // 
            // tbDestinationID
            // 
            this.tbDestinationID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDestinationID.Location = new System.Drawing.Point(416, 71);
            this.tbDestinationID.Name = "tbDestinationID";
            this.tbDestinationID.Size = new System.Drawing.Size(121, 21);
            this.tbDestinationID.TabIndex = 13;
            // 
            // lbDestinationID
            // 
            this.lbDestinationID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDestinationID.AutoSize = true;
            this.lbDestinationID.Location = new System.Drawing.Point(297, 74);
            this.lbDestinationID.Name = "lbDestinationID";
            this.lbDestinationID.Size = new System.Drawing.Size(95, 12);
            this.lbDestinationID.TabIndex = 12;
            this.lbDestinationID.Text = "Destination ID:";
            // 
            // tbSourceID
            // 
            this.tbSourceID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSourceID.Location = new System.Drawing.Point(416, 31);
            this.tbSourceID.Name = "tbSourceID";
            this.tbSourceID.Size = new System.Drawing.Size(121, 21);
            this.tbSourceID.TabIndex = 11;
            // 
            // lbSourceID
            // 
            this.lbSourceID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSourceID.AutoSize = true;
            this.lbSourceID.Location = new System.Drawing.Point(297, 34);
            this.lbSourceID.Name = "lbSourceID";
            this.lbSourceID.Size = new System.Drawing.Size(65, 12);
            this.lbSourceID.TabIndex = 10;
            this.lbSourceID.Text = "Source ID:";
            // 
            // tbDataSendInterval
            // 
            this.tbDataSendInterval.Location = new System.Drawing.Point(146, 120);
            this.tbDataSendInterval.Name = "tbDataSendInterval";
            this.tbDataSendInterval.Size = new System.Drawing.Size(121, 21);
            this.tbDataSendInterval.TabIndex = 9;
            // 
            // lbDataSendInterval
            // 
            this.lbDataSendInterval.AutoSize = true;
            this.lbDataSendInterval.Location = new System.Drawing.Point(27, 123);
            this.lbDataSendInterval.Name = "lbDataSendInterval";
            this.lbDataSendInterval.Size = new System.Drawing.Size(119, 12);
            this.lbDataSendInterval.TabIndex = 8;
            this.lbDataSendInterval.Text = "Data Send Interval:";
            // 
            // lbPrimaryCommand
            // 
            this.lbPrimaryCommand.AutoSize = true;
            this.lbPrimaryCommand.Location = new System.Drawing.Point(27, 79);
            this.lbPrimaryCommand.Name = "lbPrimaryCommand";
            this.lbPrimaryCommand.Size = new System.Drawing.Size(53, 12);
            this.lbPrimaryCommand.TabIndex = 6;
            this.lbPrimaryCommand.Text = "Command:";
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
            // tbPrimaryCommand
            // 
            this.tbPrimaryCommand.Location = new System.Drawing.Point(146, 76);
            this.tbPrimaryCommand.Name = "tbPrimaryCommand";
            this.tbPrimaryCommand.Size = new System.Drawing.Size(121, 21);
            this.tbPrimaryCommand.TabIndex = 14;
            // 
            // tbCommandID
            // 
            this.tbCommandID.Location = new System.Drawing.Point(146, 164);
            this.tbCommandID.Name = "tbCommandID";
            this.tbCommandID.Size = new System.Drawing.Size(121, 21);
            this.tbCommandID.TabIndex = 16;
            // 
            // lbCommandID
            // 
            this.lbCommandID.AutoSize = true;
            this.lbCommandID.Location = new System.Drawing.Point(27, 167);
            this.lbCommandID.Name = "lbCommandID";
            this.lbCommandID.Size = new System.Drawing.Size(71, 12);
            this.lbCommandID.TabIndex = 15;
            this.lbCommandID.Text = "Command ID:";
            // 
            // tbCarrierID
            // 
            this.tbCarrierID.Location = new System.Drawing.Point(146, 206);
            this.tbCarrierID.Name = "tbCarrierID";
            this.tbCarrierID.Size = new System.Drawing.Size(121, 21);
            this.tbCarrierID.TabIndex = 18;
            // 
            // lbCarrierID
            // 
            this.lbCarrierID.AutoSize = true;
            this.lbCarrierID.Location = new System.Drawing.Point(27, 209);
            this.lbCarrierID.Name = "lbCarrierID";
            this.lbCarrierID.Size = new System.Drawing.Size(71, 12);
            this.lbCarrierID.TabIndex = 17;
            this.lbCarrierID.Text = "Carrier ID:";
            // 
            // tbNodes
            // 
            this.tbNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNodes.Location = new System.Drawing.Point(416, 160);
            this.tbNodes.Multiline = true;
            this.tbNodes.Name = "tbNodes";
            this.tbNodes.Size = new System.Drawing.Size(121, 67);
            this.tbNodes.TabIndex = 22;
            // 
            // lbNodes
            // 
            this.lbNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbNodes.AutoSize = true;
            this.lbNodes.Location = new System.Drawing.Point(297, 163);
            this.lbNodes.Name = "lbNodes";
            this.lbNodes.Size = new System.Drawing.Size(41, 12);
            this.lbNodes.TabIndex = 21;
            this.lbNodes.Text = "Nodes:";
            // 
            // tbNodeCount
            // 
            this.tbNodeCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbNodeCount.Location = new System.Drawing.Point(416, 120);
            this.tbNodeCount.Name = "tbNodeCount";
            this.tbNodeCount.Size = new System.Drawing.Size(121, 21);
            this.tbNodeCount.TabIndex = 20;
            // 
            // lbNodeCount
            // 
            this.lbNodeCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbNodeCount.AutoSize = true;
            this.lbNodeCount.Location = new System.Drawing.Point(297, 123);
            this.lbNodeCount.Name = "lbNodeCount";
            this.lbNodeCount.Size = new System.Drawing.Size(71, 12);
            this.lbNodeCount.TabIndex = 19;
            this.lbNodeCount.Text = "Node Count:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tbSentTime);
            this.groupBox2.Controls.Add(this.lbSentTime);
            this.groupBox2.Controls.Add(this.cbAcknowledge);
            this.groupBox2.Controls.Add(this.lbAcknowledge);
            this.groupBox2.Controls.Add(this.cbSecondaryCommand);
            this.groupBox2.Controls.Add(this.lbSecondaryCommand);
            this.groupBox2.Controls.Add(this.tbSecondaryVHL);
            this.groupBox2.Controls.Add(this.lbSecondaryVHL);
            this.groupBox2.Location = new System.Drawing.Point(596, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(390, 194);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Message";
            // 
            // lbSecondaryCommand
            // 
            this.lbSecondaryCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSecondaryCommand.AutoSize = true;
            this.lbSecondaryCommand.Location = new System.Drawing.Point(74, 79);
            this.lbSecondaryCommand.Name = "lbSecondaryCommand";
            this.lbSecondaryCommand.Size = new System.Drawing.Size(53, 12);
            this.lbSecondaryCommand.TabIndex = 17;
            this.lbSecondaryCommand.Text = "Command:";
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
            // cbSecondaryCommand
            // 
            this.cbSecondaryCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSecondaryCommand.FormattingEnabled = true;
            this.cbSecondaryCommand.Location = new System.Drawing.Point(193, 76);
            this.cbSecondaryCommand.Name = "cbSecondaryCommand";
            this.cbSecondaryCommand.Size = new System.Drawing.Size(121, 20);
            this.cbSecondaryCommand.TabIndex = 18;
            // 
            // cbAcknowledge
            // 
            this.cbAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAcknowledge.FormattingEnabled = true;
            this.cbAcknowledge.Location = new System.Drawing.Point(193, 120);
            this.cbAcknowledge.Name = "cbAcknowledge";
            this.cbAcknowledge.Size = new System.Drawing.Size(121, 20);
            this.cbAcknowledge.TabIndex = 20;
            // 
            // lbAcknowledge
            // 
            this.lbAcknowledge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAcknowledge.AutoSize = true;
            this.lbAcknowledge.Location = new System.Drawing.Point(74, 123);
            this.lbAcknowledge.Name = "lbAcknowledge";
            this.lbAcknowledge.Size = new System.Drawing.Size(77, 12);
            this.lbAcknowledge.TabIndex = 19;
            this.lbAcknowledge.Text = "Acknowledge:";
            // 
            // tbSentTime
            // 
            this.tbSentTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSentTime.Location = new System.Drawing.Point(193, 164);
            this.tbSentTime.Name = "tbSentTime";
            this.tbSentTime.Size = new System.Drawing.Size(121, 21);
            this.tbSentTime.TabIndex = 22;
            // 
            // lbSentTime
            // 
            this.lbSentTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSentTime.AutoSize = true;
            this.lbSentTime.Location = new System.Drawing.Point(74, 167);
            this.lbSentTime.Name = "lbSentTime";
            this.lbSentTime.Size = new System.Drawing.Size(65, 12);
            this.lbSentTime.TabIndex = 21;
            this.lbSentTime.Text = "Sent Time:";
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
            this.btnSendSecondaryMessage.Location = new System.Drawing.Point(715, 234);
            this.btnSendSecondaryMessage.Name = "btnSendSecondaryMessage";
            this.btnSendSecondaryMessage.OverImage = null;
            this.btnSendSecondaryMessage.Size = new System.Drawing.Size(157, 23);
            this.btnSendSecondaryMessage.TabIndex = 7;
            this.btnSendSecondaryMessage.Text = "Send Secondary Message";
            this.btnSendSecondaryMessage.UpImage = null;
            this.btnSendSecondaryMessage.UseOneImage = false;
            this.btnSendSecondaryMessage.UseVisualStyleBackColor = true;
            // 
            // FrmCommandSendMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(998, 269);
            this.Controls.Add(this.btnSendSecondaryMessage);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmCommandSendMessage";
            this.Text = "FrmCommandSendMessage";
            this.Load += new System.EventHandler(this.FrmCommandSendMessage_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbNodes;
        private System.Windows.Forms.Label lbNodes;
        private System.Windows.Forms.TextBox tbNodeCount;
        private System.Windows.Forms.Label lbNodeCount;
        private System.Windows.Forms.TextBox tbCarrierID;
        private System.Windows.Forms.Label lbCarrierID;
        private System.Windows.Forms.TextBox tbCommandID;
        private System.Windows.Forms.Label lbCommandID;
        private System.Windows.Forms.TextBox tbPrimaryCommand;
        private System.Windows.Forms.TextBox tbDestinationID;
        private System.Windows.Forms.Label lbDestinationID;
        private System.Windows.Forms.TextBox tbSourceID;
        private System.Windows.Forms.Label lbSourceID;
        private System.Windows.Forms.TextBox tbDataSendInterval;
        private System.Windows.Forms.Label lbDataSendInterval;
        private System.Windows.Forms.Label lbPrimaryCommand;
        private System.Windows.Forms.TextBox tbPrimaryVHL;
        private System.Windows.Forms.Label lbPrimaryVHL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSentTime;
        private System.Windows.Forms.Label lbSentTime;
        private System.Windows.Forms.ComboBox cbAcknowledge;
        private System.Windows.Forms.Label lbAcknowledge;
        private System.Windows.Forms.ComboBox cbSecondaryCommand;
        private System.Windows.Forms.Label lbSecondaryCommand;
        private System.Windows.Forms.TextBox tbSecondaryVHL;
        private System.Windows.Forms.Label lbSecondaryVHL;
        private Library.IButton btnSendSecondaryMessage;
    }
}