namespace Sineva.VHL.Device
{
    partial class ucConfigDevices
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
			if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucConfigDevices));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripIO = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.IoAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.IoChange = new System.Windows.Forms.ToolStripMenuItem();
            this.IoRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTeachingPoint = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TeachingDataAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.VelocityDataAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTemperature = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TemperatureAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripFlowmeter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FlowmeterAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripGapSensor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.GapSensorAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripLoadcell = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LoadcellAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripPowermeter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.PowermeterAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripConstantPressure = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ConstantPressureAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripIO.SuspendLayout();
            this.contextMenuStripTeachingPoint.SuspendLayout();
            this.contextMenuStripTemperature.SuspendLayout();
            this.contextMenuStripFlowmeter.SuspendLayout();
            this.contextMenuStripGapSensor.SuspendLayout();
            this.contextMenuStripLoadcell.SuspendLayout();
            this.contextMenuStripPowermeter.SuspendLayout();
            this.contextMenuStripConstantPressure.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(452, 440);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(200, 440);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1475141789_tick.png");
            this.imageList1.Images.SetKeyName(1, "1475141816_cross.png");
            // 
            // contextMenuStripIO
            // 
            this.contextMenuStripIO.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IoAdd,
            this.IoChange,
            this.IoRemove});
            this.contextMenuStripIO.Name = "contextMenuStrip";
            this.contextMenuStripIO.Size = new System.Drawing.Size(132, 70);
            // 
            // IoAdd
            // 
            this.IoAdd.Name = "IoAdd";
            this.IoAdd.Size = new System.Drawing.Size(131, 22);
            this.IoAdd.Text = "Io Add";
            this.IoAdd.Click += new System.EventHandler(this.IoAdd_Click);
            // 
            // IoChange
            // 
            this.IoChange.Name = "IoChange";
            this.IoChange.Size = new System.Drawing.Size(131, 22);
            this.IoChange.Text = "Io Change";
            this.IoChange.Click += new System.EventHandler(this.IoChange_Click);
            // 
            // IoRemove
            // 
            this.IoRemove.Name = "IoRemove";
            this.IoRemove.Size = new System.Drawing.Size(131, 22);
            this.IoRemove.Text = "Io Remove";
            this.IoRemove.Click += new System.EventHandler(this.IoRemove_Click);
            // 
            // contextMenuStripTeachingPoint
            // 
            this.contextMenuStripTeachingPoint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TeachingDataAdd,
            this.VelocityDataAdd});
            this.contextMenuStripTeachingPoint.Name = "contextMenuStrip";
            this.contextMenuStripTeachingPoint.Size = new System.Drawing.Size(196, 48);
            // 
            // TeachingDataAdd
            // 
            this.TeachingDataAdd.Name = "TeachingDataAdd";
            this.TeachingDataAdd.Size = new System.Drawing.Size(195, 22);
            this.TeachingDataAdd.Text = "Teaching Point Add";
            this.TeachingDataAdd.Click += new System.EventHandler(this.TeachingDataAdd_Click);
            // 
            // VelocityDataAdd
            // 
            this.VelocityDataAdd.Name = "VelocityDataAdd";
            this.VelocityDataAdd.Size = new System.Drawing.Size(195, 22);
            this.VelocityDataAdd.Text = "Teaching Velocity Add";
            this.VelocityDataAdd.Click += new System.EventHandler(this.VelocityDataAdd_Click);
            // 
            // contextMenuStripTemperature
            // 
            this.contextMenuStripTemperature.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TemperatureAdd});
            this.contextMenuStripTemperature.Name = "contextMenuStrip";
            this.contextMenuStripTemperature.Size = new System.Drawing.Size(168, 26);
            // 
            // TemperatureAdd
            // 
            this.TemperatureAdd.Name = "TemperatureAdd";
            this.TemperatureAdd.Size = new System.Drawing.Size(167, 22);
            this.TemperatureAdd.Text = "Temperature Add";
            // 
            // contextMenuStripFlowmeter
            // 
            this.contextMenuStripFlowmeter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FlowmeterAdd});
            this.contextMenuStripFlowmeter.Name = "contextMenuStrip";
            this.contextMenuStripFlowmeter.Size = new System.Drawing.Size(157, 26);
            // 
            // FlowmeterAdd
            // 
            this.FlowmeterAdd.Name = "FlowmeterAdd";
            this.FlowmeterAdd.Size = new System.Drawing.Size(156, 22);
            this.FlowmeterAdd.Text = "Flowmeter Add";
            // 
            // contextMenuStripGapSensor
            // 
            this.contextMenuStripGapSensor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GapSensorAdd});
            this.contextMenuStripGapSensor.Name = "contextMenuStrip";
            this.contextMenuStripGapSensor.Size = new System.Drawing.Size(162, 26);
            // 
            // GapSensorAdd
            // 
            this.GapSensorAdd.Name = "GapSensorAdd";
            this.GapSensorAdd.Size = new System.Drawing.Size(161, 22);
            this.GapSensorAdd.Text = "Gap Sensor Add";
            // 
            // contextMenuStripLoadcell
            // 
            this.contextMenuStripLoadcell.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadcellAdd});
            this.contextMenuStripLoadcell.Name = "contextMenuStrip";
            this.contextMenuStripLoadcell.Size = new System.Drawing.Size(145, 26);
            // 
            // LoadcellAdd
            // 
            this.LoadcellAdd.Name = "LoadcellAdd";
            this.LoadcellAdd.Size = new System.Drawing.Size(144, 22);
            this.LoadcellAdd.Text = "Loadcell Add";
            // 
            // contextMenuStripPowermeter
            // 
            this.contextMenuStripPowermeter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PowermeterAdd});
            this.contextMenuStripPowermeter.Name = "contextMenuStrip";
            this.contextMenuStripPowermeter.Size = new System.Drawing.Size(165, 26);
            // 
            // PowermeterAdd
            // 
            this.PowermeterAdd.Name = "PowermeterAdd";
            this.PowermeterAdd.Size = new System.Drawing.Size(164, 22);
            this.PowermeterAdd.Text = "Powermeter Add";
            // 
            // contextMenuStripConstantPressure
            // 
            this.contextMenuStripConstantPressure.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConstantPressureAdd});
            this.contextMenuStripConstantPressure.Name = "contextMenuStrip";
            this.contextMenuStripConstantPressure.Size = new System.Drawing.Size(197, 26);
            // 
            // ConstantPressureAdd
            // 
            this.ConstantPressureAdd.Name = "ConstantPressureAdd";
            this.ConstantPressureAdd.Size = new System.Drawing.Size(196, 22);
            this.ConstantPressureAdd.Text = "Constant Pressure Add";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(248, 440);
            this.propertyGrid1.TabIndex = 1;
            // 
            // ucConfigDevices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.Name = "ucConfigDevices";
            this.Size = new System.Drawing.Size(452, 440);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripIO.ResumeLayout(false);
            this.contextMenuStripTeachingPoint.ResumeLayout(false);
            this.contextMenuStripTemperature.ResumeLayout(false);
            this.contextMenuStripFlowmeter.ResumeLayout(false);
            this.contextMenuStripGapSensor.ResumeLayout(false);
            this.contextMenuStripLoadcell.ResumeLayout(false);
            this.contextMenuStripPowermeter.ResumeLayout(false);
            this.contextMenuStripConstantPressure.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripIO;
        private System.Windows.Forms.ToolStripMenuItem IoAdd;
        private System.Windows.Forms.ToolStripMenuItem IoChange;
        private System.Windows.Forms.ToolStripMenuItem IoRemove;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTeachingPoint;
        private System.Windows.Forms.ToolStripMenuItem TeachingDataAdd;
        private System.Windows.Forms.ToolStripMenuItem VelocityDataAdd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTemperature;
        private System.Windows.Forms.ToolStripMenuItem TemperatureAdd;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFlowmeter;
        private System.Windows.Forms.ToolStripMenuItem FlowmeterAdd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGapSensor;
        private System.Windows.Forms.ToolStripMenuItem GapSensorAdd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLoadcell;
        private System.Windows.Forms.ToolStripMenuItem LoadcellAdd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPowermeter;
        private System.Windows.Forms.ToolStripMenuItem PowermeterAdd;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripConstantPressure;
        private System.Windows.Forms.ToolStripMenuItem ConstantPressureAdd;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
