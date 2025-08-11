using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class LoadingWnd : Form
    {
        public LoadingWnd()
        {
            InitializeComponent();
        }

        public void CenterParentFrm(FileExplore frmParent, int count)
        {
            int nX, nY;

            nX = (int)((frmParent.Width - this.Width) / 2) + frmParent.Location.X; // +frmParent.MdiParent.Location.X;
            nY = (int)((frmParent.Height - this.Height) / 2) + frmParent.Location.Y; // +frmParent.MdiParent.Location.Y;

            this.Location = new Point(nX, nY);
            this.progressBar1.Maximum = count;
        }

        public void LoadingImage()
        {
            if (progressBar1.Value < progressBar1.Maximum)
                progressBar1.Value = progressBar1.Value + 1;
            else
                progressBar1.Value = 0;

            progressBar1.Update();
        }

    }
}
