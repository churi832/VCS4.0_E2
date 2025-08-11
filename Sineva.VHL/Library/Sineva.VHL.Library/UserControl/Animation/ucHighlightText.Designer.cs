namespace Sineva.VHL.Library
{
    partial class ucHighlightText
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbNo = new System.Windows.Forms.Label();
            this.lbText = new System.Windows.Forms.Label();
            this.cbDone = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.cbDone)).BeginInit();
            this.SuspendLayout();
            // 
            // lbNo
            // 
            this.lbNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbNo.Location = new System.Drawing.Point(0, 0);
            this.lbNo.Margin = new System.Windows.Forms.Padding(3);
            this.lbNo.Name = "lbNo";
            this.lbNo.Size = new System.Drawing.Size(30, 34);
            this.lbNo.TabIndex = 0;
            this.lbNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbText
            // 
            this.lbText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbText.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbText.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lbText.Location = new System.Drawing.Point(30, 0);
            this.lbText.Margin = new System.Windows.Forms.Padding(3);
            this.lbText.Name = "lbText";
            this.lbText.Size = new System.Drawing.Size(234, 34);
            this.lbText.TabIndex = 1;
            this.lbText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LbText_MouseClick);
            this.lbText.MouseLeave += new System.EventHandler(this.LbText_MouseLeave);
            this.lbText.MouseHover += new System.EventHandler(this.LbText_MouseHover);
            this.lbText.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LbText_MouseMove);
            // 
            // cbDone
            // 
            this.cbDone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cbDone.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbDone.Image = Properties.Resources.icon_4;
            this.cbDone.InitialImage = null;
            this.cbDone.Location = new System.Drawing.Point(264, 0);
            this.cbDone.Name = "cbDone";
            this.cbDone.Size = new System.Drawing.Size(35, 34);
            this.cbDone.TabIndex = 2;
            this.cbDone.TabStop = false;
            // 
            // ucHighlightText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.cbDone);
            this.Controls.Add(this.lbText);
            this.Controls.Add(this.lbNo);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ucHighlightText";
            this.Size = new System.Drawing.Size(299, 34);
            ((System.ComponentModel.ISupportInitialize)(this.cbDone)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbNo;
        private System.Windows.Forms.Label lbText;
        private System.Windows.Forms.PictureBox cbDone;
    }
}
