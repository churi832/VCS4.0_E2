namespace Sineva.VHL.Data.DbAdapter
{
	partial class DatabaseView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label23 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbSelect = new System.Windows.Forms.Label();
            this.treeView = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbUpdateTeachingSamePortType = new System.Windows.Forms.CheckBox();
            this.lbProcessCount = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnImportOffset = new System.Windows.Forms.Button();
            this.cbSelectedItemUpdate = new System.Windows.Forms.CheckBox();
            this.doubleBufferedGridView1 = new Sineva.VHL.Library.DoubleBufferedGridView();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleBufferedGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(206)))), ((int)(((byte)(214)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label23, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.doubleBufferedGridView1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(951, 653);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label23.Font = new System.Drawing.Font("굴림", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label23.ForeColor = System.Drawing.Color.Black;
            this.label23.Location = new System.Drawing.Point(3, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(945, 30);
            this.label23.TabIndex = 17;
            this.label23.Text = "Database List";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1281, 653);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.TabIndex = 9;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(206)))), ((int)(((byte)(214)))));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lbSelect, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.treeView, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(326, 653);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // lbSelect
            // 
            this.lbSelect.BackColor = System.Drawing.Color.Transparent;
            this.lbSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelect.Font = new System.Drawing.Font("굴림", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSelect.ForeColor = System.Drawing.Color.Black;
            this.lbSelect.Location = new System.Drawing.Point(3, 0);
            this.lbSelect.Name = "lbSelect";
            this.lbSelect.Size = new System.Drawing.Size(320, 30);
            this.lbSelect.TabIndex = 14;
            this.lbSelect.Text = "Select Category";
            this.lbSelect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(3, 33);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(320, 467);
            this.treeView.TabIndex = 0;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbSelectedItemUpdate);
            this.panel1.Controls.Add(this.cbUpdateTeachingSamePortType);
            this.panel1.Controls.Add(this.lbProcessCount);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnImportOffset);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 506);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 144);
            this.panel1.TabIndex = 15;
            // 
            // cbUpdateTeachingSamePortType
            // 
            this.cbUpdateTeachingSamePortType.AutoSize = true;
            this.cbUpdateTeachingSamePortType.Location = new System.Drawing.Point(17, 109);
            this.cbUpdateTeachingSamePortType.Name = "cbUpdateTeachingSamePortType";
            this.cbUpdateTeachingSamePortType.Size = new System.Drawing.Size(283, 16);
            this.cbUpdateTeachingSamePortType.TabIndex = 2;
            this.cbUpdateTeachingSamePortType.Text = "Update Teaching Offset of the Same PortType";
            this.cbUpdateTeachingSamePortType.UseVisualStyleBackColor = true;
            // 
            // lbProcessCount
            // 
            this.lbProcessCount.Location = new System.Drawing.Point(230, 44);
            this.lbProcessCount.Name = "lbProcessCount";
            this.lbProcessCount.Size = new System.Drawing.Size(75, 31);
            this.lbProcessCount.TabIndex = 1;
            this.lbProcessCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(130, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 60);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Database Update";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnImportOffset
            // 
            this.btnImportOffset.Location = new System.Drawing.Point(17, 19);
            this.btnImportOffset.Name = "btnImportOffset";
            this.btnImportOffset.Size = new System.Drawing.Size(96, 60);
            this.btnImportOffset.TabIndex = 0;
            this.btnImportOffset.Text = "Import Offset";
            this.btnImportOffset.UseVisualStyleBackColor = true;
            this.btnImportOffset.Click += new System.EventHandler(this.btnImportOffset_Click);
            // 
            // cbSelectedItemUpdate
            // 
            this.cbSelectedItemUpdate.AutoSize = true;
            this.cbSelectedItemUpdate.Location = new System.Drawing.Point(17, 87);
            this.cbSelectedItemUpdate.Name = "cbSelectedItemUpdate";
            this.cbSelectedItemUpdate.Size = new System.Drawing.Size(144, 16);
            this.cbSelectedItemUpdate.TabIndex = 3;
            this.cbSelectedItemUpdate.Text = "Update Selected Item";
            this.cbSelectedItemUpdate.UseVisualStyleBackColor = true;
            // 
            // doubleBufferedGridView1
            // 
            this.doubleBufferedGridView1.AllowUserToAddRows = false;
            this.doubleBufferedGridView1.AllowUserToDeleteRows = false;
            this.doubleBufferedGridView1.AllowUserToResizeRows = false;
            this.doubleBufferedGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.doubleBufferedGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferedGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.doubleBufferedGridView1.Location = new System.Drawing.Point(3, 33);
            this.doubleBufferedGridView1.Name = "doubleBufferedGridView1";
            this.doubleBufferedGridView1.ReadOnly = true;
            this.doubleBufferedGridView1.RowHeadersVisible = false;
            this.doubleBufferedGridView1.RowTemplate.Height = 23;
            this.doubleBufferedGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.doubleBufferedGridView1.Size = new System.Drawing.Size(945, 617);
            this.doubleBufferedGridView1.TabIndex = 7;
            this.doubleBufferedGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.doubleBufferedGridView1_CellContentClick);
            // 
            // DatabaseView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DatabaseView";
            this.Size = new System.Drawing.Size(1281, 653);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleBufferedGridView1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Sineva.VHL.Library.DoubleBufferedGridView doubleBufferedGridView1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label23;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lbSelect;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnImportOffset;
        private System.Windows.Forms.Label lbProcessCount;
        private System.Windows.Forms.CheckBox cbUpdateTeachingSamePortType;
        private System.Windows.Forms.CheckBox cbSelectedItemUpdate;
    }
}
