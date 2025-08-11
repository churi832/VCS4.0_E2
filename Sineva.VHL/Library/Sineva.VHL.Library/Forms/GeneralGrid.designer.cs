namespace Sineva.VHL.Library
{
	partial class GeneralGrid
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
			this.lblTitle = new System.Windows.Forms.Label();
			this.doubleBufferedGridView1 = new DoubleBufferedGridView();
			((System.ComponentModel.ISupportInitialize)(this.doubleBufferedGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblTitle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTitle.Location = new System.Drawing.Point(0, 0);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(382, 20);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Title :";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// doubleBufferedGridView1
			// 
			this.doubleBufferedGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.doubleBufferedGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.doubleBufferedGridView1.Location = new System.Drawing.Point(0, 23);
			this.doubleBufferedGridView1.Name = "doubleBufferedGridView1";
			this.doubleBufferedGridView1.RowTemplate.Height = 23;
			this.doubleBufferedGridView1.Size = new System.Drawing.Size(382, 304);
			this.doubleBufferedGridView1.TabIndex = 2;
			// 
			// GeneralGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.doubleBufferedGridView1);
			this.Controls.Add(this.lblTitle);
			this.Name = "GeneralGrid";
			this.Size = new System.Drawing.Size(382, 327);
			((System.ComponentModel.ISupportInitialize)(this.doubleBufferedGridView1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblTitle;
		private DoubleBufferedGridView doubleBufferedGridView1;
	}
}
