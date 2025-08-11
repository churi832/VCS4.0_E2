using System;

namespace Sineva.OHT.Common
{
    public class ConfigAttribute : Attribute
    {
        public string SaveName { get; set; }
        public bool IsElementAttribute { get; set; } = false;
    }
}
