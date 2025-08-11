namespace Sineva.VHL.Library.Servo
{
    partial class ucConfigServo
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlServo = new System.Windows.Forms.TabControl();
            this.tabPageServo = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewServo = new System.Windows.Forms.TreeView();
            this.propertyGridServo = new System.Windows.Forms.PropertyGrid();
            this.tabPageMotion = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeViewMotion = new System.Windows.Forms.TreeView();
            this.propertyGridMotion = new System.Windows.Forms.PropertyGrid();
            this.tabPageAxisParameter = new System.Windows.Forms.TabPage();
            this.dgvAxes = new Sineva.VHL.Library.DoubleBufferedGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Add = new System.Windows.Forms.ToolStripMenuItem();
            this.Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlServo.SuspendLayout();
            this.tabPageServo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPageMotion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPageAxisParameter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAxes)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlServo
            // 
            this.tabControlServo.Controls.Add(this.tabPageServo);
            this.tabControlServo.Controls.Add(this.tabPageMotion);
            this.tabControlServo.Controls.Add(this.tabPageAxisParameter);
            this.tabControlServo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlServo.Location = new System.Drawing.Point(0, 0);
            this.tabControlServo.Name = "tabControlServo";
            this.tabControlServo.SelectedIndex = 0;
            this.tabControlServo.Size = new System.Drawing.Size(421, 425);
            this.tabControlServo.TabIndex = 1;
            // 
            // tabPageServo
            // 
            this.tabPageServo.Controls.Add(this.splitContainer1);
            this.tabPageServo.Location = new System.Drawing.Point(4, 22);
            this.tabPageServo.Name = "tabPageServo";
            this.tabPageServo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageServo.Size = new System.Drawing.Size(413, 399);
            this.tabPageServo.TabIndex = 0;
            this.tabPageServo.Text = "Servo Config";
            this.tabPageServo.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewServo);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGridServo);
            this.splitContainer1.Size = new System.Drawing.Size(407, 393);
            this.splitContainer1.SplitterDistance = 165;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeViewServo
            // 
            this.treeViewServo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewServo.Font = new System.Drawing.Font("Arial", 9F);
            this.treeViewServo.Location = new System.Drawing.Point(0, 0);
            this.treeViewServo.Name = "treeViewServo";
            this.treeViewServo.Size = new System.Drawing.Size(165, 393);
            this.treeViewServo.TabIndex = 0;
            this.treeViewServo.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewServo_NodeMouseClick);
            // 
            // propertyGridServo
            // 
            this.propertyGridServo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridServo.Font = new System.Drawing.Font("Arial", 9F);
            this.propertyGridServo.Location = new System.Drawing.Point(0, 0);
            this.propertyGridServo.Name = "propertyGridServo";
            this.propertyGridServo.Size = new System.Drawing.Size(238, 393);
            this.propertyGridServo.TabIndex = 0;
            // 
            // tabPageMotion
            // 
            this.tabPageMotion.Controls.Add(this.splitContainer2);
            this.tabPageMotion.Location = new System.Drawing.Point(4, 22);
            this.tabPageMotion.Name = "tabPageMotion";
            this.tabPageMotion.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMotion.Size = new System.Drawing.Size(413, 399);
            this.tabPageMotion.TabIndex = 1;
            this.tabPageMotion.Text = "Motion Config";
            this.tabPageMotion.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeViewMotion);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.propertyGridMotion);
            this.splitContainer2.Size = new System.Drawing.Size(407, 393);
            this.splitContainer2.SplitterDistance = 165;
            this.splitContainer2.TabIndex = 2;
            // 
            // treeViewMotion
            // 
            this.treeViewMotion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMotion.Font = new System.Drawing.Font("Arial", 9F);
            this.treeViewMotion.Location = new System.Drawing.Point(0, 0);
            this.treeViewMotion.Name = "treeViewMotion";
            this.treeViewMotion.Size = new System.Drawing.Size(165, 393);
            this.treeViewMotion.TabIndex = 0;
            this.treeViewMotion.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewMotion_NodeMouseClick);
            // 
            // propertyGridMotion
            // 
            this.propertyGridMotion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridMotion.Font = new System.Drawing.Font("Arial", 9F);
            this.propertyGridMotion.Location = new System.Drawing.Point(0, 0);
            this.propertyGridMotion.Name = "propertyGridMotion";
            this.propertyGridMotion.Size = new System.Drawing.Size(238, 393);
            this.propertyGridMotion.TabIndex = 0;
            // 
            // tabPageAxisParameter
            // 
            this.tabPageAxisParameter.Controls.Add(this.dgvAxes);
            this.tabPageAxisParameter.Location = new System.Drawing.Point(4, 22);
            this.tabPageAxisParameter.Name = "tabPageAxisParameter";
            this.tabPageAxisParameter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAxisParameter.Size = new System.Drawing.Size(413, 399);
            this.tabPageAxisParameter.TabIndex = 2;
            this.tabPageAxisParameter.Text = "Axis Parameter";
            this.tabPageAxisParameter.UseVisualStyleBackColor = true;
            // 
            // dgvAxes
            // 
            this.dgvAxes.AllowUserToAddRows = false;
            this.dgvAxes.AllowUserToDeleteRows = false;
            this.dgvAxes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvAxes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAxes.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgvAxes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAxes.Location = new System.Drawing.Point(3, 3);
            this.dgvAxes.Name = "dgvAxes";
            this.dgvAxes.RowTemplate.Height = 23;
            this.dgvAxes.Size = new System.Drawing.Size(407, 393);
            this.dgvAxes.TabIndex = 1;
            this.dgvAxes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAxes_KeyDown);
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
            // ucConfigServo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabControlServo);
            this.Name = "ucConfigServo";
            this.Size = new System.Drawing.Size(421, 425);
            this.tabControlServo.ResumeLayout(false);
            this.tabPageServo.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPageMotion.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPageAxisParameter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAxes)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlServo;
        private System.Windows.Forms.TabPage tabPageServo;
        private System.Windows.Forms.TabPage tabPageMotion;
        private System.Windows.Forms.TabPage tabPageAxisParameter;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeViewServo;
        private System.Windows.Forms.PropertyGrid propertyGridServo;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeViewMotion;
        private System.Windows.Forms.PropertyGrid propertyGridMotion;
        private DoubleBufferedGridView dgvAxes;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Add;
        private System.Windows.Forms.ToolStripMenuItem Remove;
    }
}
