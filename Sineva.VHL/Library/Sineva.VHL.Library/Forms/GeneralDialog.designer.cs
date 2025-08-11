namespace Sineva.VHL.Library
{
    partial class GeneralDialog
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
            if(disposing && (components != null))
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
            this.panelTitle = new System.Windows.Forms.Panel();
            this.pbButtonClose = new System.Windows.Forms.PictureBox();
            this.pbButtonMinimize = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pbButtonLock = new System.Windows.Forms.PictureBox();
            this.panelTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbButtonClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbButtonMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbButtonLock)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTitle
            // 
            this.panelTitle.Controls.Add(this.pbButtonClose);
            this.panelTitle.Controls.Add(this.pbButtonMinimize);
            this.panelTitle.Controls.Add(this.lblTitle);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Margin = new System.Windows.Forms.Padding(0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(800, 24);
            this.panelTitle.TabIndex = 1;
            // 
            // pbButtonClose
            // 
            this.pbButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbButtonClose.BackColor = System.Drawing.Color.Transparent;
            this.pbButtonClose.BackgroundImage = global::Sineva.VHL.Library.Properties.Resources.chars001_X;
            this.pbButtonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbButtonClose.Location = new System.Drawing.Point(777, 2);
            this.pbButtonClose.Name = "pbButtonClose";
            this.pbButtonClose.Size = new System.Drawing.Size(20, 20);
            this.pbButtonClose.TabIndex = 3;
            this.pbButtonClose.TabStop = false;
            // 
            // pbButtonMinimize
            // 
            this.pbButtonMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbButtonMinimize.BackColor = System.Drawing.Color.Transparent;
            this.pbButtonMinimize.BackgroundImage = global::Sineva.VHL.Library.Properties.Resources.chars002_underbar;
            this.pbButtonMinimize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbButtonMinimize.Location = new System.Drawing.Point(751, 2);
            this.pbButtonMinimize.Name = "pbButtonMinimize";
            this.pbButtonMinimize.Size = new System.Drawing.Size(20, 20);
            this.pbButtonMinimize.TabIndex = 2;
            this.pbButtonMinimize.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(800, 24);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "TITLE";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Title_MouseDown);
            this.lblTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Title_MouseMove);
            this.lblTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Title_MouseUp);
            // 
            // pbButtonLock
            // 
            this.pbButtonLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbButtonLock.BackColor = System.Drawing.Color.GreenYellow;
            this.pbButtonLock.BackgroundImage = global::Sineva.VHL.Library.Properties.Resources.i001_Lock;
            this.pbButtonLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbButtonLock.Location = new System.Drawing.Point(768, 418);
            this.pbButtonLock.Name = "pbButtonLock";
            this.pbButtonLock.Size = new System.Drawing.Size(30, 30);
            this.pbButtonLock.TabIndex = 4;
            this.pbButtonLock.TabStop = false;
            // 
            // IcsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.pbButtonLock);
            this.Controls.Add(this.panelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IcsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IcsDialog";
            this.Activated += new System.EventHandler(this.Dialog_Activated);
            this.Deactivate += new System.EventHandler(this.Dialog_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Dialog_FormClosing);
            this.Shown += new System.EventHandler(this.IcsDialog_Shown);
            this.SizeChanged += new System.EventHandler(this.Dialog_SizeChanged);
            this.panelTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbButtonClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbButtonMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbButtonLock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pbButtonClose;
        private System.Windows.Forms.PictureBox pbButtonMinimize;
        private System.Windows.Forms.PictureBox pbButtonLock;
    }
}