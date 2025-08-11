/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.12 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sineva.VHL.Library
{
    public class GeneralObject
    {
        #region enum
        private enum GenType
        {
            Unknown,
            Method,
            Property,
        }
        #endregion
        #region Field
        private bool m_Initialized = false;

        private GenType m_GenType = GenType.Unknown;
        private string m_Name = string.Empty;
        private string m_Description = string.Empty;
        private object m_GenInstance = null;
        private PropertyInfo m_GenProperty = null;
        private MethodInfo m_GenMethod = null;

        private string m_Value = string.Empty;
        private object[] m_GenParams = null;
        private Random m_Rand = null;
        private int m_ScanInterval = 100;
        #endregion

        #region Property
        public string Name
        {
            get { return m_Name; }
        }
        public string Description
        {
            get { return m_Description; }
        }
        public object LinkItem
        {
            get { return m_GenInstance; }
        }
        public Type InstanceType
        {
            get
            {
                if (m_GenInstance == null)
                    return null;
                return m_GenInstance.GetType();
            }
        }
        public bool HasMethod { get { return m_GenMethod != null ? true : false; } }
        public bool HasProperty { get { return m_GenProperty != null ? true : false; } }
        public string Value { get { return m_Value; } }
        #endregion

        #region Constructor
        public GeneralObject(string name, string description)
        {
            this.m_Name = name;
            this.m_Description = description;
        }
        public GeneralObject(string name, string description, object instance, string targetName, int scan_interval, params object[] param) 
            : this(name, description) 
        {
            if (!m_Initialized) 
            {
                try
                {
                    if (instance != null && !string.IsNullOrEmpty(targetName))
                    {
                        this.m_GenInstance = instance;
                        List<Type> types = new List<Type>();
                        if (param != null)
                        {
                            for (int i = 0; i < param.Length; i++) { types.Add(param[i].GetType()); }
                        }
                        this.m_GenParams = param;
                        if (types.Count > 0) this.m_GenMethod = m_GenInstance.GetType().GetMethod(targetName, types.ToArray());
                        else this.m_GenMethod = m_GenInstance.GetType().GetMethod(targetName);
                        this.m_GenProperty = m_GenInstance.GetType().GetProperty(targetName);

                        if (this.m_GenMethod != null) this.m_GenType = GenType.Method;
                        else if (this.m_GenProperty != null) this.m_GenType = GenType.Property;
                    }

                    bool ok = true;
                    ok &= this.m_GenInstance != null;
                    ok &= this.m_GenType != GenType.Unknown;
                    if (ok)
                    {
                        m_ScanInterval = scan_interval;
                        m_Initialized = true;
                        // General Object 직접 Update 하자........
                        System.Threading.Thread threadUpdate = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadCallback));
                        threadUpdate.IsBackground = true;
                        threadUpdate.Start();
                    }
                }
                catch(Exception e) 
                {
                    m_Initialized= false;
                }
            }
        }
        #endregion

        #region Methods
        private void ThreadCallback()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(m_ScanInterval);
                this.UpdateValue();
            }
        }

        private bool GetValue(out object output)
        {
            if (!m_Initialized) { output = null;  return false; }

            bool rv = true;
            try
            {
                switch (m_GenType)
                {
                    case GenType.Method:
                        output = m_GenMethod.Invoke(m_GenInstance, m_GenParams);
                        break;
                    case GenType.Property:
                        output = m_GenProperty.GetValue(m_GenInstance);
                        break;
                    default: rv = false; output = null;  break;
                }
            }
            catch { output = null; rv = false; }
            return rv;
        }
        public void UpdateValue()
        {
            if (!m_Initialized) return;
            string rv = string.Empty;
            try
            {
                object value;
                if (GetValue(out value)) rv = value.ToString();
                else rv = string.Empty;
            }
            catch(Exception e)
            {
                rv = string.Empty;
            }
            m_Value = rv;
        }
        #endregion

        #region Override
        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, this.Description);
        }
        #endregion
    }
}
