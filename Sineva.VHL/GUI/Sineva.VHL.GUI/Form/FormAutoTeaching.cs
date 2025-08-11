using Sineva.VHL.Library;
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
    public partial class FormAutoTeaching : Form
    {
        public FormAutoTeaching()
        {
            InitializeComponent();
            this.ucDevAutoTeaching1.Initialize();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.ucDevAutoTeaching1.UpdateState();
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
    }
}
