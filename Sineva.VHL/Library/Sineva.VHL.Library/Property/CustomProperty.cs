/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.16 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;

namespace Sineva.VHL.Library
{
    public sealed class CustomProperty
    {
        #region Field
        private string m_Name = string.Empty;
        private string m_Value = string.Empty;
        private Type m_ValueType = null;
        #endregion

        #region Property
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public string Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        public Type ValueType
        {
            get { return m_ValueType; }
            set { m_ValueType = value; }
        }
        #endregion

        #region Method
        public bool GetValue(out object value)
        {
            value = null;
            try
            {
                if (m_ValueType == null) return false;
                else if (m_ValueType == typeof(Boolean) || m_ValueType == typeof(bool)) value = Convert.ToBoolean(m_Value);
                else if (m_ValueType == typeof(UInt16) || m_ValueType == typeof(ushort)) value = Convert.ToUInt16(m_Value);
                else if (m_ValueType == typeof(UInt32) || m_ValueType == typeof(uint)) value = Convert.ToUInt32(m_Value);
                else if (m_ValueType == typeof(UInt64) || m_ValueType == typeof(ulong)) value = Convert.ToUInt64(m_Value);
                else if (m_ValueType == typeof(Int16) || m_ValueType == typeof(short)) value = Convert.ToInt16(m_Value);
                else if (m_ValueType == typeof(Int32) || m_ValueType == typeof(int)) value = Convert.ToInt32(m_Value);
                else if (m_ValueType == typeof(Int64) || m_ValueType == typeof(long)) value = Convert.ToInt64(m_Value);
                else if (m_ValueType == typeof(Single) || m_ValueType == typeof(float)) value = Convert.ToSingle(m_Value);
                else if (m_ValueType == typeof(Double) || m_ValueType == typeof(double)) value = Convert.ToDouble(m_Value);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
