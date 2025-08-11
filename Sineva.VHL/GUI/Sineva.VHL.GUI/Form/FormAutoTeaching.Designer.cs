namespace Sineva.VHL.GUI
{
    partial class FormAutoTeaching
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ucDevAutoTeaching1 = new Sineva.VHL.Device.ucDevAutoTeaching();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ucDevAutoTeaching1
            // 
            this.ucDevAutoTeaching1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.ucDevAutoTeaching1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucDevAutoTeaching1.Location = new System.Drawing.Point(0, 0);
            this.ucDevAutoTeaching1.Name = "ucDevAutoTeaching1";
            this.ucDevAutoTeaching1.Size = new System.Drawing.Size(474, 608);
            this.ucDevAutoTeaching1.TabIndex = 0;
            // 
            // FormAutoTeaching
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 608);
            this.Controls.Add(this.ucDevAutoTeaching1);
            this.Name = "FormAutoTeaching";
            this.Text = "FormAutoTeaching";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Device.ucDevAutoTeaching ucDevAutoTeaching1;
    }
}