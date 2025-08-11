namespace Sineva.VHL.Data.Alarm
{
	partial class AlarmListView
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
			this.doubleBufferedGridView1 = new Sineva.VHL.Library.DoubleBufferedGridView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label23 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.doubleBufferedGridView1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
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
			this.doubleBufferedGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.doubleBufferedGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.doubleBufferedGridView1.Size = new System.Drawing.Size(174, 245);
			this.doubleBufferedGridView1.TabIndex = 7;
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(180, 281);
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
			this.label23.Size = new System.Drawing.Size(174, 30);
			this.label23.TabIndex = 17;
			this.label23.Text = "Alarm List";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AlarmListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "AlarmListView";
			this.Size = new System.Drawing.Size(180, 281);
			((System.ComponentModel.ISupportInitialize)(this.doubleBufferedGridView1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Sineva.VHL.Library.DoubleBufferedGridView doubleBufferedGridView1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label23;
	}
}
