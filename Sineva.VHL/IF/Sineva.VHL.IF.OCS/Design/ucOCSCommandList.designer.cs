
namespace Sineva.VHL.IF.OCS
{
    partial class ucOCSCommandList
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.headerPanel1 = new Sineva.VHL.Library.HeaderPanel();
            this.dgOCSCommand = new System.Windows.Forms.DataGridView();
            this.headerPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOCSCommand)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel1
            // 
            this.headerPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headerPanel1.Controls.Add(this.dgOCSCommand);
            this.headerPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel1.Font = new System.Drawing.Font("Arial", 9F);
            this.headerPanel1.FriendControl = null;
            this.headerPanel1.Location = new System.Drawing.Point(0, 0);
            this.headerPanel1.Name = "headerPanel1";
            this.headerPanel1.Size = new System.Drawing.Size(378, 435);
            this.headerPanel1.TabIndex = 0;
            this.headerPanel1.Title = "OCS Command List";
            this.headerPanel1.TitleBorderStyle = System.Windows.Forms.BorderStyle.None;
            this.headerPanel1.TitleColor = System.Drawing.SystemColors.ActiveCaption;
            this.headerPanel1.TitleTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.headerPanel1.TitleTextColor = System.Drawing.SystemColors.ControlText;
            // 
            // dgOCSCommand
            // 
            this.dgOCSCommand.AllowUserToAddRows = false;
            this.dgOCSCommand.AllowUserToDeleteRows = false;
            this.dgOCSCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgOCSCommand.Location = new System.Drawing.Point(3, 21);
            this.dgOCSCommand.Name = "dgOCSCommand";
            this.dgOCSCommand.RowTemplate.Height = 23;
            this.dgOCSCommand.Size = new System.Drawing.Size(371, 409);
            this.dgOCSCommand.TabIndex = 2;
            this.dgOCSCommand.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgOCSCommand_CellFormatting);
            this.dgOCSCommand.SelectionChanged += new System.EventHandler(this.dgOCSCommand_SelectionChanged);
            // 
            // OCSCommandList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.headerPanel1);
            this.Name = "OCSCommandList";
            this.Size = new System.Drawing.Size(378, 435);
            this.Load += new System.EventHandler(this.OCSCommandList_Load);
            this.headerPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOCSCommand)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Library.HeaderPanel headerPanel1;
        private System.Windows.Forms.DataGridView dgOCSCommand;
    }
}
