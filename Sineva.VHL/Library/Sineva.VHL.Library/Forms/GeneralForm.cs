/*///////////////////////////////////////////////////////////////
// Copyright	: (주)KPS, www.kpscorp.co.kr
// Version		: V1.0
// Programmer	: dspcrassus
// Date			: 2014-12-04
// Description	: 창닫기 단축키(Ctrl+F4) 로 인한 MDI Child Form Closing 방지
 * 
///////////////////////////////////////////////////////////////*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public class GeneralForm : Form
    {
        public GeneralForm()
        {
			if (XFunc.IsRunTime())
			{
				FormClosing += IcsForm_FormClosing;
			}
        }

        void IcsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(IsMdiChild && e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}
