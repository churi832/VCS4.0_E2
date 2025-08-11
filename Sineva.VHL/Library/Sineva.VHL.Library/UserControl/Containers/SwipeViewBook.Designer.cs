namespace Sineva.VHL.Library
{
    partial class SwipeViewBook
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelPages = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblPageCurrent = new System.Windows.Forms.Label();
            this.lblPageTotal = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbViewPageList = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.touchLabel1 = new TouchLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelPages, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(347, 364);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelPages
            // 
            this.panelPages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPages.Location = new System.Drawing.Point(1, 23);
            this.panelPages.Margin = new System.Windows.Forms.Padding(1);
            this.panelPages.Name = "panelPages";
            this.panelPages.Size = new System.Drawing.Size(345, 315);
            this.panelPages.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 20);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblPageCurrent);
            this.splitContainer1.Panel1.Controls.Add(this.lblPageTotal);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1MinSize = 82;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cbViewPageList);
            this.splitContainer1.Size = new System.Drawing.Size(345, 20);
            this.splitContainer1.SplitterDistance = 82;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // lblPageCurrent
            // 
            this.lblPageCurrent.Location = new System.Drawing.Point(4, 4);
            this.lblPageCurrent.Name = "lblPageCurrent";
            this.lblPageCurrent.Size = new System.Drawing.Size(32, 12);
            this.lblPageCurrent.TabIndex = 1;
            this.lblPageCurrent.Text = "0";
            this.lblPageCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPageTotal
            // 
            this.lblPageTotal.Location = new System.Drawing.Point(48, 4);
            this.lblPageTotal.Name = "lblPageTotal";
            this.lblPageTotal.Size = new System.Drawing.Size(32, 12);
            this.lblPageTotal.TabIndex = 1;
            this.lblPageTotal.Text = "0";
            this.lblPageTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(36, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "/";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbViewPageList
            // 
            this.cbViewPageList.BackColor = System.Drawing.SystemColors.Window;
            this.cbViewPageList.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbViewPageList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbViewPageList.FormattingEnabled = true;
            this.cbViewPageList.Location = new System.Drawing.Point(34, 0);
            this.cbViewPageList.Name = "cbViewPageList";
            this.cbViewPageList.Size = new System.Drawing.Size(228, 20);
            this.cbViewPageList.TabIndex = 0;
            this.cbViewPageList.SelectionChangeCommitted += new System.EventHandler(this.CbViewPageList_SelectionChangeCommitted);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.touchLabel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 340);
            this.panel2.Margin = new System.Windows.Forms.Padding(1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(345, 23);
            this.panel2.TabIndex = 1;
            // 
            // touchLabel1
            // 
            this.touchLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.touchLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.touchLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.touchLabel1.Location = new System.Drawing.Point(0, 0);
            this.touchLabel1.Name = "touchLabel1";
            this.touchLabel1.Size = new System.Drawing.Size(345, 23);
            this.touchLabel1.TabIndex = 0;
            this.touchLabel1.Text = "Slide Area";
            this.touchLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.touchLabel1.UserBackColorDefault = System.Drawing.SystemColors.Control;
            this.touchLabel1.UserBackColorMouseDown = System.Drawing.Color.LightBlue;
            this.touchLabel1.UserBackColorMouseEnter = System.Drawing.Color.YellowGreen;
            this.touchLabel1.UserForeColorDefault = System.Drawing.SystemColors.ControlText;
            this.touchLabel1.UserForeColorMouseDown = System.Drawing.Color.White;
            this.touchLabel1.UserForeColorMouseEnter = System.Drawing.Color.Blue;
            // 
            // SwipeViewContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SwipeViewContainer";
            this.Size = new System.Drawing.Size(347, 364);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelPages;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbViewPageList;
        private System.Windows.Forms.Panel panel2;
        private TouchLabel touchLabel1;
        private System.Windows.Forms.Label lblPageCurrent;
        private System.Windows.Forms.Label lblPageTotal;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
    }
}
