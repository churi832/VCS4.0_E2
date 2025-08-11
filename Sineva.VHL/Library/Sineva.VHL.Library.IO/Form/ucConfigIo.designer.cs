namespace Sineva.VHL.Library.IO
{
    partial class ucConfigIo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucConfigIo));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Add = new System.Windows.Forms.ToolStripMenuItem();
            this.Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageIoConfig = new System.Windows.Forms.TabPage();
            this.tabPageIoEdit = new System.Windows.Forms.TabPage();
            this.viewIoEdit1 = new Sineva.VHL.Library.IO.ViewIoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageIoConfig.SuspendLayout();
            this.tabPageIoEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(479, 446);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 3;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList1;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(181, 446);
            this.treeView.TabIndex = 0;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "addin.ico");
            this.imageList1.Images.SetKeyName(1, "MxComponent.bmp");
            this.imageList1.Images.SetKeyName(2, "BitInOut.bmp");
            this.imageList1.Images.SetKeyName(3, "BitIn.bmp");
            this.imageList1.Images.SetKeyName(4, "BitOut.bmp");
            this.imageList1.Images.SetKeyName(5, "WordIn.bmp");
            this.imageList1.Images.SetKeyName(6, "WordOut.bmp");
            this.imageList1.Images.SetKeyName(7, "DI.bmp");
            this.imageList1.Images.SetKeyName(8, "DO.bmp");
            this.imageList1.Images.SetKeyName(9, "AI.bmp");
            this.imageList1.Images.SetKeyName(10, "AO.bmp");
            this.imageList1.Images.SetKeyName(11, "16-NeXT98 Batch File.ico");
            this.imageList1.Images.SetKeyName(12, "1Power.ico");
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(294, 446);
            this.propertyGrid.TabIndex = 0;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Add,
            this.Remove});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(118, 48);
            // 
            // Add
            // 
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(117, 22);
            this.Add.Text = "Add";
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Remove
            // 
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(117, 22);
            this.Remove.Text = "Remove";
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageIoConfig);
            this.tabControl.Controls.Add(this.tabPageIoEdit);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(493, 478);
            this.tabControl.TabIndex = 5;
            // 
            // tabPageIoConfig
            // 
            this.tabPageIoConfig.Controls.Add(this.splitContainer1);
            this.tabPageIoConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageIoConfig.Name = "tabPageIoConfig";
            this.tabPageIoConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIoConfig.Size = new System.Drawing.Size(485, 452);
            this.tabPageIoConfig.TabIndex = 0;
            this.tabPageIoConfig.Text = "I/O Config";
            this.tabPageIoConfig.UseVisualStyleBackColor = true;
            // 
            // tabPageIoEdit
            // 
            this.tabPageIoEdit.Controls.Add(this.viewIoEdit1);
            this.tabPageIoEdit.Location = new System.Drawing.Point(4, 22);
            this.tabPageIoEdit.Name = "tabPageIoEdit";
            this.tabPageIoEdit.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIoEdit.Size = new System.Drawing.Size(485, 452);
            this.tabPageIoEdit.TabIndex = 1;
            this.tabPageIoEdit.Text = "I/O Edit";
            this.tabPageIoEdit.UseVisualStyleBackColor = true;
            // 
            // viewIoEdit1
            // 
            this.viewIoEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewIoEdit1.Font = new System.Drawing.Font("Arial", 9F);
            this.viewIoEdit1.Location = new System.Drawing.Point(3, 3);
            this.viewIoEdit1.Name = "viewIoEdit1";
            this.viewIoEdit1.Size = new System.Drawing.Size(479, 446);
            this.viewIoEdit1.TabIndex = 0;
            // 
            // ucConfigIo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabControl);
            this.Name = "ucConfigIo";
            this.Size = new System.Drawing.Size(493, 478);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageIoConfig.ResumeLayout(false);
            this.tabPageIoEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Add;
        private System.Windows.Forms.ToolStripMenuItem Remove;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageIoConfig;
        private System.Windows.Forms.TabPage tabPageIoEdit;
        private ViewIoEdit viewIoEdit1;
    }
}
