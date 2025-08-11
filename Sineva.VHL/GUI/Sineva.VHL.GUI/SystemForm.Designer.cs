namespace Sineva.VHL.GUI
{
    partial class SystemForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripSystem = new System.Windows.Forms.ToolStrip();
            this.toolAddChart = new System.Windows.Forms.ToolStripButton();
            this.toolRemoveAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tabControlSystem = new Sineva.VHL.Library.FlatTabControl();
            this.tabPageDirectIO = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.viewIoEdit1 = new Sineva.VHL.Library.IO.ViewIoEdit();
            this.trendGraph1 = new Sineva.VHL.Library.IO.TrendGraph();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStripSystem.SuspendLayout();
            this.tabControlSystem.SuspendLayout();
            this.tabPageDirectIO.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tableLayoutPanel1.Controls.Add(this.toolStripSystem, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControlSystem, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1121, 658);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // toolStripSystem
            // 
            this.toolStripSystem.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.Command_bg;
            this.toolStripSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripSystem.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripSystem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAddChart,
            this.toolRemoveAll,
            this.toolStripSeparator1});
            this.toolStripSystem.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStripSystem.Location = new System.Drawing.Point(1024, 0);
            this.toolStripSystem.Name = "toolStripSystem";
            this.toolStripSystem.Size = new System.Drawing.Size(97, 658);
            this.toolStripSystem.TabIndex = 2;
            this.toolStripSystem.Text = "toolStrip1";
            // 
            // toolAddChart
            // 
            this.toolAddChart.Font = new System.Drawing.Font("Arial", 9F);
            this.toolAddChart.ForeColor = System.Drawing.Color.White;
            this.toolAddChart.Image = ((System.Drawing.Image)(resources.GetObject("toolAddChart.Image")));
            this.toolAddChart.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolAddChart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolAddChart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddChart.Name = "toolAddChart";
            this.toolAddChart.Size = new System.Drawing.Size(95, 82);
            this.toolAddChart.Text = "Add Trend";
            this.toolAddChart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolAddChart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolAddChart.Click += new System.EventHandler(this.toolAddChart_Click);
            // 
            // toolRemoveAll
            // 
            this.toolRemoveAll.ForeColor = System.Drawing.Color.White;
            this.toolRemoveAll.Image = ((System.Drawing.Image)(resources.GetObject("toolRemoveAll.Image")));
            this.toolRemoveAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolRemoveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRemoveAll.Name = "toolRemoveAll";
            this.toolRemoveAll.Size = new System.Drawing.Size(95, 82);
            this.toolRemoveAll.Text = "Remove All";
            this.toolRemoveAll.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolRemoveAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolRemoveAll.Click += new System.EventHandler(this.toolRemoveAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(95, 6);
            // 
            // tabControlSystem
            // 
            this.tabControlSystem.Controls.Add(this.tabPageDirectIO);
            this.tabControlSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSystem.ImageList = this.imageList1;
            this.tabControlSystem.Location = new System.Drawing.Point(3, 3);
            this.tabControlSystem.myBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(60)))), ((int)(((byte)(89)))));
            this.tabControlSystem.mySelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tabControlSystem.myTextColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabControlSystem.Name = "tabControlSystem";
            this.tabControlSystem.SelectedIndex = 0;
            this.tabControlSystem.Size = new System.Drawing.Size(1018, 652);
            this.tabControlSystem.TabIndex = 3;
            this.tabControlSystem.Tag = "SYSTEM";
            // 
            // tabPageDirectIO
            // 
            this.tabPageDirectIO.BackColor = System.Drawing.Color.Transparent;
            this.tabPageDirectIO.Controls.Add(this.tableLayoutPanel2);
            this.tabPageDirectIO.ImageIndex = 0;
            this.tabPageDirectIO.Location = new System.Drawing.Point(4, 26);
            this.tabPageDirectIO.Name = "tabPageDirectIO";
            this.tabPageDirectIO.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDirectIO.Size = new System.Drawing.Size(1010, 622);
            this.tabPageDirectIO.TabIndex = 0;
            this.tabPageDirectIO.Tag = "DIRECTIO";
            this.tabPageDirectIO.Text = "I/O";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.viewIoEdit1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.trendGraph1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1004, 616);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // viewIoEdit1
            // 
            this.viewIoEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewIoEdit1.Font = new System.Drawing.Font("Arial", 9F);
            this.viewIoEdit1.Location = new System.Drawing.Point(3, 3);
            this.viewIoEdit1.Name = "viewIoEdit1";
            this.viewIoEdit1.Size = new System.Drawing.Size(998, 360);
            this.viewIoEdit1.TabIndex = 0;
            // 
            // trendGraph1
            // 
            this.trendGraph1.Capacity = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.trendGraph1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            this.trendGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trendGraph1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.trendGraph1.LegendDockStyle = Sineva.VHL.Library.IO.TrendGraph.LEGEND_DOCK_STYLE.ChartArea1;
            this.trendGraph1.Location = new System.Drawing.Point(3, 369);
            this.trendGraph1.Name = "trendGraph1";
            this.trendGraph1.SamplingEnable = false;
            this.trendGraph1.SamplingInterval = 1000;
            this.trendGraph1.Size = new System.Drawing.Size(998, 244);
            this.trendGraph1.TabIndex = 1;
            this.trendGraph1.TestGraph = false;
            this.trendGraph1.UpdateDuringVisible = false;
            this.trendGraph1.UseSamplingPeriodSetting = true;
            this.trendGraph1.XAxisTitle = "";
            this.trendGraph1.YAxisTitle = "";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "system_io.ico");
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SystemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1121, 658);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SystemForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SystemForm";
            this.Load += new System.EventHandler(this.SystemForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStripSystem.ResumeLayout(false);
            this.toolStripSystem.PerformLayout();
            this.tabControlSystem.ResumeLayout(false);
            this.tabPageDirectIO.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStripSystem;
        private System.Windows.Forms.ToolStripButton toolAddChart;
        private System.Windows.Forms.ToolStripButton toolRemoveAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ImageList imageList1;
        private Library.FlatTabControl tabControlSystem;
        private System.Windows.Forms.TabPage tabPageDirectIO;
        private Library.IO.ViewIoEdit viewIoEdit1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Library.IO.TrendGraph trendGraph1;
        private System.Windows.Forms.Timer timer1;
    }
}