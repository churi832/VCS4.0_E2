namespace Sineva.VHL.Library.IO
{
    partial class ucLed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucLed));
            this.pbLamp = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLamp)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLamp
            // 
            this.pbLamp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbLamp.BackgroundImage")));
            this.pbLamp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbLamp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLamp.InitialImage = ((System.Drawing.Image)(resources.GetObject("pbLamp.InitialImage")));
            this.pbLamp.Location = new System.Drawing.Point(0, 0);
            this.pbLamp.Margin = new System.Windows.Forms.Padding(0);
            this.pbLamp.Name = "pbLamp";
            this.pbLamp.Size = new System.Drawing.Size(25, 25);
            this.pbLamp.TabIndex = 0;
            this.pbLamp.TabStop = false;
            this.pbLamp.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbLamp_MouseDoubleClick);
            // 
            // ucLedO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pbLamp);
            this.Name = "ucLedO";
            this.Size = new System.Drawing.Size(25, 25);
            ((System.ComponentModel.ISupportInitialize)(this.pbLamp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLamp;
    }
}
