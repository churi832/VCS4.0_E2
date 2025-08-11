using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI
{
    public partial class FormLifeTimeDisplay : Form
    {
        public FormLifeTimeDisplay()
        {
            InitializeComponent();

            ucLifeTime1.Initialize();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                ucLifeTime1.UpdateState();
            }
        }
    }
}
