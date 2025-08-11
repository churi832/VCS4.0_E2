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
    public class DeviceSettingAttribute : Attribute
    {
        #region Fileds
        private bool m_IsDeviceSettingType = false;
        private bool m_IsRuntimeSettingType = false; // Application Run일때만 Display하기 위해 정의
        #endregion

        #region Properties
        public bool IsDeviceSettingType
        {
            get { return m_IsDeviceSettingType; }
            set { m_IsDeviceSettingType = value; }
        }
        public bool IsRuntimeSettingType
        {
            get { return m_IsRuntimeSettingType; }
            set { m_IsRuntimeSettingType = value; }
        }
        #endregion

        public DeviceSettingAttribute(bool config_set, bool runtime_set = false)
        {
            m_IsDeviceSettingType = config_set;
            m_IsRuntimeSettingType = runtime_set;
        }
        public override bool IsDefaultAttribute()
        {
            return false;
        }
    }
}
