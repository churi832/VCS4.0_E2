namespace Sineva.VHL.GUI
{
    partial class FormDataDisplay
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
            this.zoomableChart1 = new Sineva.VHL.Library.ZoomableChart();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuHide = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // zoomableChart1
            // 
            this.zoomableChart1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zoomableChart1.ChartAreaBackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.zoomableChart1.ChartAreaBackgroundColor = System.Drawing.Color.Gainsboro;
            this.zoomableChart1.ChartAreaShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.zoomableChart1.CursorCount = 0;
            this.zoomableChart1.DefaultChartBackColor = System.Drawing.Color.FloralWhite;
            this.zoomableChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomableChart1.Font = new System.Drawing.Font("Arial", 9F);
            this.zoomableChart1.Location = new System.Drawing.Point(120, 0);
            this.zoomableChart1.Name = "zoomableChart1";
            this.zoomableChart1.ShowAxisLabel = true;
            this.zoomableChart1.ShowStatus = true;
            this.zoomableChart1.Size = new System.Drawing.Size(687, 176);
            this.zoomableChart1.TabIndex = 0;
            this.zoomableChart1.XLabelFormat = "F3";
            this.zoomableChart1.YLabelFormat = "F3";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuHide});
            this.contextMenu.Name = "contextMenuStrip";
            this.contextMenu.Size = new System.Drawing.Size(100, 26);
            // 
            // MenuHide
            // 
            this.MenuHide.Name = "MenuHide";
            this.MenuHide.Size = new System.Drawing.Size(99, 22);
            this.MenuHide.Text = "Hide";
            this.MenuHide.Click += new System.EventHandler(this.MenuHide_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(120, 176);
            this.checkedListBox1.TabIndex = 1;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // FormDistanceDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(807, 176);
            this.ControlBox = false;
            this.Controls.Add(this.zoomableChart1);
            this.Controls.Add(this.checkedListBox1);
            this.Name = "FormDistanceDisplay";
            this.Text = "Path Monitor";
            this.TopMost = true;
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Library.ZoomableChart zoomableChart1;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuHide;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}