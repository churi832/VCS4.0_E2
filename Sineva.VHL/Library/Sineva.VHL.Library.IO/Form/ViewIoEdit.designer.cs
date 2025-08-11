namespace Sineva.VHL.Library.IO
{
	partial class ViewIoEdit
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textFilter = new System.Windows.Forms.TextBox();
            this.cbFilterOn = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDi = new System.Windows.Forms.TabPage();
            this.dataGridViewDi = new System.Windows.Forms.DataGridView();
            this.tabPageDo = new System.Windows.Forms.TabPage();
            this.dataGridViewDo = new System.Windows.Forms.DataGridView();
            this.tabPageAi = new System.Windows.Forms.TabPage();
            this.dataGridViewAi = new System.Windows.Forms.DataGridView();
            this.tabPageAo = new System.Windows.Forms.TabPage();
            this.dataGridViewAo = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabPageDi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDi)).BeginInit();
            this.tabPageDo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDo)).BeginInit();
            this.tabPageAi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAi)).BeginInit();
            this.tabPageAo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAo)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textFilter
            // 
            this.textFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textFilter.Location = new System.Drawing.Point(3, 1);
            this.textFilter.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.textFilter.Name = "textFilter";
            this.textFilter.Size = new System.Drawing.Size(397, 21);
            this.textFilter.TabIndex = 0;
            this.textFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textFilter_KeyDown);
            // 
            // cbFilterOn
            // 
            this.cbFilterOn.AutoSize = true;
            this.cbFilterOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFilterOn.Location = new System.Drawing.Point(406, 3);
            this.cbFilterOn.Name = "cbFilterOn";
            this.cbFilterOn.Size = new System.Drawing.Size(398, 18);
            this.cbFilterOn.TabIndex = 1;
            this.cbFilterOn.Text = "Filter";
            this.cbFilterOn.UseVisualStyleBackColor = true;
            this.cbFilterOn.CheckedChanged += new System.EventHandler(this.cbFilterOn_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageDi);
            this.tabControl1.Controls.Add(this.tabPageDo);
            this.tabControl1.Controls.Add(this.tabPageAi);
            this.tabControl1.Controls.Add(this.tabPageAo);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 33);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(807, 363);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPageDi
            // 
            this.tabPageDi.Controls.Add(this.dataGridViewDi);
            this.tabPageDi.Location = new System.Drawing.Point(4, 24);
            this.tabPageDi.Name = "tabPageDi";
            this.tabPageDi.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDi.Size = new System.Drawing.Size(799, 335);
            this.tabPageDi.TabIndex = 0;
            this.tabPageDi.Text = "Digital Input";
            this.tabPageDi.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDi
            // 
            this.dataGridViewDi.AllowUserToAddRows = false;
            this.dataGridViewDi.AllowUserToDeleteRows = false;
            this.dataGridViewDi.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F);
            this.dataGridViewDi.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewDi.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewDi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDi.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewDi.Name = "dataGridViewDi";
            this.dataGridViewDi.RowHeadersVisible = false;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F);
            this.dataGridViewDi.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewDi.RowTemplate.Height = 23;
            this.dataGridViewDi.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewDi.Size = new System.Drawing.Size(793, 329);
            this.dataGridViewDi.TabIndex = 0;
            this.dataGridViewDi.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewDi_DataError);
            // 
            // tabPageDo
            // 
            this.tabPageDo.Controls.Add(this.dataGridViewDo);
            this.tabPageDo.Location = new System.Drawing.Point(4, 24);
            this.tabPageDo.Name = "tabPageDo";
            this.tabPageDo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDo.Size = new System.Drawing.Size(799, 335);
            this.tabPageDo.TabIndex = 1;
            this.tabPageDo.Text = "Digital Output";
            this.tabPageDo.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDo
            // 
            this.dataGridViewDo.AllowUserToAddRows = false;
            this.dataGridViewDo.AllowUserToDeleteRows = false;
            this.dataGridViewDo.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            this.dataGridViewDo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewDo.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewDo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDo.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewDo.Name = "dataGridViewDo";
            this.dataGridViewDo.RowHeadersVisible = false;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.dataGridViewDo.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewDo.RowTemplate.Height = 23;
            this.dataGridViewDo.Size = new System.Drawing.Size(793, 329);
            this.dataGridViewDo.TabIndex = 0;
            // 
            // tabPageAi
            // 
            this.tabPageAi.Controls.Add(this.dataGridViewAi);
            this.tabPageAi.Location = new System.Drawing.Point(4, 24);
            this.tabPageAi.Name = "tabPageAi";
            this.tabPageAi.Size = new System.Drawing.Size(799, 335);
            this.tabPageAi.TabIndex = 2;
            this.tabPageAi.Text = "Analog Input";
            this.tabPageAi.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAi
            // 
            this.dataGridViewAi.AllowUserToAddRows = false;
            this.dataGridViewAi.AllowUserToDeleteRows = false;
            this.dataGridViewAi.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            this.dataGridViewAi.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewAi.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewAi.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAi.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAi.Name = "dataGridViewAi";
            this.dataGridViewAi.RowHeadersVisible = false;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 9F);
            this.dataGridViewAi.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewAi.RowTemplate.Height = 23;
            this.dataGridViewAi.Size = new System.Drawing.Size(799, 335);
            this.dataGridViewAi.TabIndex = 0;
            // 
            // tabPageAo
            // 
            this.tabPageAo.Controls.Add(this.dataGridViewAo);
            this.tabPageAo.Location = new System.Drawing.Point(4, 24);
            this.tabPageAo.Name = "tabPageAo";
            this.tabPageAo.Size = new System.Drawing.Size(799, 335);
            this.tabPageAo.TabIndex = 3;
            this.tabPageAo.Text = "Analog Output";
            this.tabPageAo.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAo
            // 
            this.dataGridViewAo.AllowUserToAddRows = false;
            this.dataGridViewAo.AllowUserToDeleteRows = false;
            this.dataGridViewAo.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            this.dataGridViewAo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewAo.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewAo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAo.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAo.Name = "dataGridViewAo";
            this.dataGridViewAo.RowHeadersVisible = false;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 9F);
            this.dataGridViewAo.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewAo.RowTemplate.Height = 23;
            this.dataGridViewAo.Size = new System.Drawing.Size(799, 335);
            this.dataGridViewAo.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(859, 445);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(206)))), ((int)(((byte)(214)))));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(23, 23);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(813, 399);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.textFilter, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.cbFilterOn, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(807, 24);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // ViewIoEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.Name = "ViewIoEdit";
            this.Size = new System.Drawing.Size(859, 445);
            this.Load += new System.EventHandler(this.ViewIoEdit_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageDi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDi)).EndInit();
            this.tabPageDo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDo)).EndInit();
            this.tabPageAi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAi)).EndInit();
            this.tabPageAo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAo)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textFilter;
		private System.Windows.Forms.CheckBox cbFilterOn;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageDi;
		private System.Windows.Forms.TabPage tabPageDo;
		private System.Windows.Forms.TabPage tabPageAi;
		private System.Windows.Forms.TabPage tabPageAo;
		private System.Windows.Forms.DataGridView dataGridViewDi;
		private System.Windows.Forms.DataGridView dataGridViewDo;
		private System.Windows.Forms.DataGridView dataGridViewAi;
		private System.Windows.Forms.DataGridView dataGridViewAo;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
	}
}
