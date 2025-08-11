/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public interface IUpdateUCon
    {
        void UpdateState();
        bool Initialize();
    }

    public static class UpdateUConFunc
    {
        public static void GetAllUpdateUCon(Control control, ref List<IUpdateUCon> ucons)
        {
            foreach (Control ctrl in control.Controls) 
            {
                IUpdateUCon ucon = ctrl as IUpdateUCon;
                if (ucon != null) { ucons.Add(ucon); }
                GetAllUpdateUCon(ctrl, ref ucons);
            }
        }
    }
}
