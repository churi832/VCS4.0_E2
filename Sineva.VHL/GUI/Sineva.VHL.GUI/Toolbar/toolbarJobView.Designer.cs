namespace Sineva.VHL.GUI
{
    partial class toolbarJobView
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(toolbarJobView));
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonReady = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAuto = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonManual = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPause = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAbort = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = global::Sineva.VHL.GUI.Properties.Resources.Command_bg;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonReady,
            this.toolStripSeparator1,
            this.toolStripButtonAuto,
            this.toolStripButtonManual,
            this.toolStripSeparator2,
            this.toolStripButtonStart,
            this.toolStripButtonPause,
            this.toolStripButtonAbort,
            this.toolStripSeparator3});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(95, 850);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStripJobs";
            // 
            // toolStripButtonReady
            // 
            this.toolStripButtonReady.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonReady.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonReady.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReady.Image")));
            this.toolStripButtonReady.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButtonReady.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonReady.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReady.Name = "toolStripButtonReady";
            this.toolStripButtonReady.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonReady.Text = "READY";
            this.toolStripButtonReady.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolStripButtonReady.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonReady.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(93, 6);
            this.toolStripSeparator1.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButtonAuto
            // 
            this.toolStripButtonAuto.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonAuto.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonAuto.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAuto.Image")));
            this.toolStripButtonAuto.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonAuto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAuto.Name = "toolStripButtonAuto";
            this.toolStripButtonAuto.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonAuto.Text = "AUTO";
            this.toolStripButtonAuto.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonAuto.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButtonManual
            // 
            this.toolStripButtonManual.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonManual.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonManual.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonManual.Image")));
            this.toolStripButtonManual.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonManual.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonManual.Name = "toolStripButtonManual";
            this.toolStripButtonManual.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonManual.Text = "MANUAL";
            this.toolStripButtonManual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonManual.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(93, 6);
            this.toolStripSeparator2.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonStart.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStart.Image")));
            this.toolStripButtonStart.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonStart.Text = "CYCLE START";
            this.toolStripButtonStart.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButtonPause
            // 
            this.toolStripButtonPause.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonPause.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonPause.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPause.Image")));
            this.toolStripButtonPause.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPause.Name = "toolStripButtonPause";
            this.toolStripButtonPause.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonPause.Text = "PAUSE";
            this.toolStripButtonPause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonPause.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButtonAbort
            // 
            this.toolStripButtonAbort.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripButtonAbort.ForeColor = System.Drawing.Color.White;
            this.toolStripButtonAbort.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAbort.Image")));
            this.toolStripButtonAbort.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolStripButtonAbort.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonAbort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbort.Name = "toolStripButtonAbort";
            this.toolStripButtonAbort.Size = new System.Drawing.Size(93, 82);
            this.toolStripButtonAbort.Text = "ABORT";
            this.toolStripButtonAbort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButtonAbort.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(93, 6);
            // 
            // toolbarJobView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.toolStrip1);
            this.Name = "toolbarJobView";
            this.Size = new System.Drawing.Size(95, 850);
            this.Tag = "JOB";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonReady;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAuto;
        private System.Windows.Forms.ToolStripButton toolStripButtonManual;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private System.Windows.Forms.ToolStripButton toolStripButtonPause;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbort;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
