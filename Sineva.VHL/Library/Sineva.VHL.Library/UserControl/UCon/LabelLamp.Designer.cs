namespace Sineva.VHL.Library
{
    partial class LabelLamp
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
            this.pbLamp = new System.Windows.Forms.PictureBox();
            this.lblText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbLamp)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pbLamp.BackgroundImage = Properties.Resources.diode;
            this.pbLamp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbLamp.Dock = System.Windows.Forms.DockStyle.Left;
            this.pbLamp.Location = new System.Drawing.Point(0, 0);
            this.pbLamp.Margin = new System.Windows.Forms.Padding(0);
            this.pbLamp.Name = "pictureBox1";
            this.pbLamp.Size = new System.Drawing.Size(15, 15);
            this.pbLamp.TabIndex = 2;
            this.pbLamp.TabStop = false;
            // 
            // label1
            // 
            this.lblText.BackColor = System.Drawing.Color.Transparent;
            this.lblText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblText.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblText.Location = new System.Drawing.Point(15, 0);
            this.lblText.Margin = new System.Windows.Forms.Padding(0);
            this.lblText.Name = "label1";
            this.lblText.Size = new System.Drawing.Size(85, 15);
            this.lblText.TabIndex = 3;
            this.lblText.Text = "label1";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelLamp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.pbLamp);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "LabelLamp";
            this.Size = new System.Drawing.Size(100, 15);
            this.SizeChanged += new System.EventHandler(this.LabelLamp_SizeChanged);
            this.ParentChanged += new System.EventHandler(this.LabelLamp_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pbLamp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLamp;
        private System.Windows.Forms.Label lblText;
    }
}
