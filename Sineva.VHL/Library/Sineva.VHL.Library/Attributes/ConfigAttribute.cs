/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library
{
    public class ConfigAttribute : Attribute
    {
        public string SaveName { get; set; }
        public bool IsElementAttribute { get; set; } = false;
    }
}
