using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_FDCItem : DataItem
    {
        #region Fields
        private string m_EQPID;
        private string m_Time;
        private string m_ValueString;


        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public string EQPID
        {
            get { return m_EQPID; }
            set { m_EQPID = value; }
        }

        [DatabaseSettingAttribute(true)]
        public string ValueString
        {
            get { return m_ValueString; }
            set { m_ValueString = value; }
        }

        [DatabaseSettingAttribute(true)]
        public string Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        #endregion

        #region Constructors
        public DataItem_FDCItem()
        {
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_FDCItem source)
        {
            try
            {
                this.m_EQPID = source.EQPID;
                this.m_ValueString = source.ValueString;
                this.m_Time = source.Time;
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        public DataItem_FDCItem GetCopyOrNull()
        {
            DataItem_FDCItem component = null;
            try
            {
                component = (DataItem_FDCItem)base.MemberwiseClone();
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return component;
        }
        public bool CompareWith(DataItem_FDCItem target)
        {
            bool result = false;
            try
            {
                result = (this.m_EQPID == target.EQPID);
                result &= (this.m_ValueString == target.ValueString);
                result &= (this.m_Time == target.Time);
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.ToString());
                ExceptionLog.WriteLog(err.ToString());
            }
            return result;
        }
        #endregion
    }
}
