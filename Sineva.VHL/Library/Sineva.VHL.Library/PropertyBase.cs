using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Sineva.VHL.Library
{
    public class PropertyBase
    {
        #region Fields
        protected int m_No = 0;
        protected string m_ID = string.Empty;
        #endregion

        #region Properties
        public int No
        {
            get { return m_No; }
            set { m_No = value; }
        }
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public PropertyBase()
        {

        }
        #endregion

        #region Methods
        protected bool PropertyUpdate(string propertyName, object value, bool valueCheck = true)
        {
            try
            {
                bool result = true;

                FieldInfo fieldInfo = this.GetType().GetField("_" + propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    return false;
                }

                if (valueCheck)
                {
                    if (fieldInfo.GetValue((object)this) == null || fieldInfo.GetValue((object)this).Equals(value))
                    {
                        return false;
                    }
                }

                fieldInfo.SetValue((object)this, value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
