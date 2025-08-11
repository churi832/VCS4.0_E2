namespace Sineva.VHL.GUI
{
    partial class DatabaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseForm));
            this.tabControlDatabase = new Sineva.VHL.Library.FlatTabControl();
            this.tabPageDatabase = new System.Windows.Forms.TabPage();
            this.databaseView1 = new Sineva.VHL.Data.DbAdapter.DatabaseView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControlDatabase.SuspendLayout();
            this.tabPageDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlDatabase
            // 
            this.tabControlDatabase.Controls.Add(this.tabPageDatabase);
            this.tabControlDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDatabase.ImageList = this.imageList1;
            this.tabControlDatabase.Location = new System.Drawing.Point(0, 0);
            this.tabControlDatabase.myBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tabControlDatabase.mySelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabControlDatabase.myTextColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabControlDatabase.Name = "tabControlDatabase";
            this.tabControlDatabase.SelectedIndex = 0;
            this.tabControlDatabase.Size = new System.Drawing.Size(1916, 842);
            this.tabControlDatabase.TabIndex = 0;
            this.tabControlDatabase.Tag = "Database";
            // 
            // tabPageDatabase
            // 
            this.tabPageDatabase.BackColor = System.Drawing.Color.Transparent;
            this.tabPageDatabase.Controls.Add(this.databaseView1);
            this.tabPageDatabase.ImageIndex = 1;
            this.tabPageDatabase.Location = new System.Drawing.Point(4, 26);
            this.tabPageDatabase.Name = "tabPageDatabase";
            this.tabPageDatabase.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDatabase.Size = new System.Drawing.Size(1908, 812);
            this.tabPageDatabase.TabIndex = 0;
            this.tabPageDatabase.Tag = "Database";
            this.tabPageDatabase.Text = "Database";
            // 
            // databaseView1
            // 
            this.databaseView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseView1.Location = new System.Drawing.Point(3, 3);
            this.databaseView1.Name = "databaseView1";
            this.databaseView1.Size = new System.Drawing.Size(1902, 806);
            this.databaseView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "motor_servo.ico");
            this.imageList1.Images.SetKeyName(1, "motor_maxon.ico");
            // 
            // DatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1916, 842);
            this.ControlBox = false;
            this.Controls.Add(this.tabControlDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "DatabaseForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DatabaseForm";
            this.Load += new System.EventHandler(this.DatabaseForm_Load);
            this.tabControlDatabase.ResumeLayout(false);
            this.tabPageDatabase.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Library.FlatTabControl tabControlDatabase;
        private System.Windows.Forms.TabPage tabPageDatabase;
        private System.Windows.Forms.ImageList imageList1;
        private Data.DbAdapter.DatabaseView databaseView1;
    }
}