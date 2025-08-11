using System;
using System.ComponentModel;
using System.Reflection;

namespace Sineva.VHL.Library.Common
{
    [Serializable()]
    public class DataItem_Base
    {
        #region Fields
        protected int _No = 0;
        protected string _ID = string.Empty;
        #endregion

        #region Event
        //public delegate void PropertyUpdateNotifyHandler(object sender, object data);
        //public event PropertyUpdateNotifyHandler PropertyUpdateNotify;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public int No
        {
            get { return _No; }
            set { _No = value; }
        }
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        #endregion

        #region Constructors
        public DataItem_Base()
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

                if (fieldInfo == null) return false;

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
