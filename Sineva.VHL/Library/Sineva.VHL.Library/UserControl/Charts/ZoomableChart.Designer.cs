namespace Sineva.VHL.Library
{
    partial class ZoomableChart
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemUnZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUnZoomAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemAddCursor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemClearCursor = new System.Windows.Forms.ToolStripMenuItem();
            this.lblXyPos = new System.Windows.Forms.Label();
            this.cbColorPicker = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblCursor = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.ScrollBar.Enabled = false;
            chartArea1.AxisY.IsReversed = false;
            chartArea1.AxisY.ScrollBar.Enabled = false;
            chartArea1.BackColor = System.Drawing.Color.Gainsboro;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.Name = "ChartArea1";
            chartArea1.ShadowOffset = 4;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(3, 3);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(762, 576);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            this.chart1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseClick);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseMove);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemUnZoom,
            this.toolStripMenuItemUnZoomAll,
            this.toolStripSeparator1,
            this.toolStripMenuItemAddCursor,
            this.toolStripMenuItemClearCursor});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(146, 98);
            // 
            // toolStripMenuItemUnZoom
            // 
            this.toolStripMenuItemUnZoom.Name = "toolStripMenuItemUnZoom";
            this.toolStripMenuItemUnZoom.Size = new System.Drawing.Size(145, 22);
            this.toolStripMenuItemUnZoom.Text = "Un Zoom";
            this.toolStripMenuItemUnZoom.Click += new System.EventHandler(this.toolStripMenuItemUnZoom_Click);
            // 
            // toolStripMenuItemUnZoomAll
            // 
            this.toolStripMenuItemUnZoomAll.Name = "toolStripMenuItemUnZoomAll";
            this.toolStripMenuItemUnZoomAll.Size = new System.Drawing.Size(145, 22);
            this.toolStripMenuItemUnZoomAll.Text = "Reset Zoom";
            this.toolStripMenuItemUnZoomAll.Click += new System.EventHandler(this.toolStripMenuItemUnZoomAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(142, 6);
            // 
            // toolStripMenuItemAddCursor
            // 
            this.toolStripMenuItemAddCursor.Name = "toolStripMenuItemAddCursor";
            this.toolStripMenuItemAddCursor.Size = new System.Drawing.Size(145, 22);
            this.toolStripMenuItemAddCursor.Text = "Add Cursor";
            this.toolStripMenuItemAddCursor.Click += new System.EventHandler(this.toolStripMenuItemAddCursor_Click);
            // 
            // toolStripMenuItemClearCursor
            // 
            this.toolStripMenuItemClearCursor.Name = "toolStripMenuItemClearCursor";
            this.toolStripMenuItemClearCursor.Size = new System.Drawing.Size(145, 22);
            this.toolStripMenuItemClearCursor.Text = "Clear Cursors";
            this.toolStripMenuItemClearCursor.Click += new System.EventHandler(this.toolStripMenuItemClearCursor_Click);
            // 
            // lblXyPos
            // 
            this.lblXyPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblXyPos.Location = new System.Drawing.Point(3, 0);
            this.lblXyPos.Name = "lblXyPos";
            this.lblXyPos.Size = new System.Drawing.Size(154, 25);
            this.lblXyPos.TabIndex = 3;
            this.lblXyPos.Text = "(0.0000, 0.0000)";
            this.lblXyPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbColorPicker
            // 
            this.cbColorPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbColorPicker.FormattingEnabled = true;
            this.cbColorPicker.Location = new System.Drawing.Point(663, 0);
            this.cbColorPicker.Margin = new System.Windows.Forms.Padding(0);
            this.cbColorPicker.Name = "cbColorPicker";
            this.cbColorPicker.Size = new System.Drawing.Size(99, 23);
            this.cbColorPicker.TabIndex = 4;
            this.cbColorPicker.SelectedIndexChanged += new System.EventHandler(this.cbColorPicker_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chart1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(768, 613);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.tableLayoutPanel2.Controls.Add(this.lblXyPos, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbColorPicker, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblCursor, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 585);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(762, 25);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // lblCursor
            // 
            this.lblCursor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCursor.AutoSize = true;
            this.lblCursor.Location = new System.Drawing.Point(163, 0);
            this.lblCursor.Name = "lblCursor";
            this.lblCursor.Size = new System.Drawing.Size(41, 25);
            this.lblCursor.TabIndex = 5;
            this.lblCursor.Text = "label1";
            this.lblCursor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ZoomableChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.Name = "ZoomableChart";
            this.Size = new System.Drawing.Size(768, 613);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUnZoom;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUnZoomAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddCursor;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearCursor;
        private System.Windows.Forms.Label lblXyPos;
        private System.Windows.Forms.ComboBox cbColorPicker;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblCursor;

    }
}
