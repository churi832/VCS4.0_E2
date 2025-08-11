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
    public sealed class CustomAttribute : Attribute
    {
        #region Fields
        private bool m_IsSequencial = false;
        private string m_ItemName;
        private Type m_ItemType = null;
        #endregion

        #region Properties
        public string ItemName
        {
            get { return m_ItemName; }
        }
        public Type ItemType
        {
            get { return m_ItemType; }
        }
        public bool IsSequencial
        {
            get { return m_IsSequencial; }
        }
        #endregion

        #region Constructor
        public CustomAttribute(string name, Type type)
        {
            m_ItemName = name;
            m_ItemType = type;
        }

        public CustomAttribute(string name, Type type, bool sequencial)
            : this(name, type)
        {
            m_IsSequencial = sequencial;
        }
        #endregion

        public override bool IsDefaultAttribute()
        {
            return false;
        }
    }
}
