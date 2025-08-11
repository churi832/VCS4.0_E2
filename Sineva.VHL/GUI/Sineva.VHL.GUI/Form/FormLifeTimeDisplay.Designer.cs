namespace Sineva.VHL.GUI
{
    partial class FormLifeTimeDisplay
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
            this.ucLifeTime1 = new Sineva.VHL.GUI.ucLifeTime();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ucLifeTime1
            // 
            this.ucLifeTime1.Location = new System.Drawing.Point(2, 0);
            this.ucLifeTime1.Name = "ucLifeTime1";
            this.ucLifeTime1.Size = new System.Drawing.Size(1170, 640);
            this.ucLifeTime1.TabIndex = 0;
            // 
            // FormLifeTimeDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 641);
            this.Controls.Add(this.ucLifeTime1);
            this.Name = "FormLifeTimeDisplay";
            this.Text = "FormLifeTimeDisplay";
            this.ResumeLayout(false);

        }

        #endregion

        private ucLifeTime ucLifeTime1;
        private System.Windows.Forms.Timer timer1;
    }
}