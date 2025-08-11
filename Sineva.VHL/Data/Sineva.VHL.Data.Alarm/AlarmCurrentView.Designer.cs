namespace Sineva.VHL.Data.Alarm
{
	partial class AlarmCurrentView
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
            this.dataGridViewCurrentAlarms = new Library.DoubleBufferedGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurrentAlarms)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewCurrentAlarms
            // 
            this.dataGridViewCurrentAlarms.AllowUserToAddRows = false;
            this.dataGridViewCurrentAlarms.AllowUserToOrderColumns = true;
            this.dataGridViewCurrentAlarms.AllowUserToResizeColumns = false;
            this.dataGridViewCurrentAlarms.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewCurrentAlarms.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewCurrentAlarms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewCurrentAlarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewCurrentAlarms.BackgroundColor = System.Drawing.Color.Linen;
            this.dataGridViewCurrentAlarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCurrentAlarms.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewCurrentAlarms.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewCurrentAlarms.Name = "dataGridViewCurrentAlarms";
            this.dataGridViewCurrentAlarms.RowTemplate.Height = 23;
            this.dataGridViewCurrentAlarms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCurrentAlarms.Size = new System.Drawing.Size(150, 150);
            this.dataGridViewCurrentAlarms.TabIndex = 10;
            this.dataGridViewCurrentAlarms.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCurrentAlarms_CellContentDoubleClick);
            // 
            // AlarmCurrentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.dataGridViewCurrentAlarms);
            this.Name = "AlarmCurrentView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurrentAlarms)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private Library.DoubleBufferedGridView dataGridViewCurrentAlarms;

    }
}
