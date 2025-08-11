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
using System.Reflection;

namespace Sineva.VHL.Library
{
    [AttributeUsage(AttributeTargets.All)]
    public class DatabaseSettingAttribute : Attribute
    {
        #region Fileds
        private bool m_IsSettingType = false;
        #endregion

        #region Properties
        public bool IsSettingType
        {
            get { return m_IsSettingType; }
            set { m_IsSettingType = value; }
        }
        #endregion

        public DatabaseSettingAttribute(bool config_set)
        {
            m_IsSettingType = config_set;
        }
        public override bool IsDefaultAttribute()
        {
            return false;
        }
    }
}
