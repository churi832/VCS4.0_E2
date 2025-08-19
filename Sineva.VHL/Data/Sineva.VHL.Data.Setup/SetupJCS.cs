using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Setup
{
    [Serializable]
    public partial class SetupJCS
    {
        #region Fields
        //private Use m_JCSUse = Use.Use;
        private bool m_ContinueMode = true;
        private int m_PermitWaitTime = 5000;
        private int m_MessageRepayTimeOut = 60000;
        private int m_StatusReportIntervalTime = 200;
        private int m_NetworkRestartFailCount = 5;
        private int m_PermitRetryCount = 18;
        #endregion

        #region Properties
        //[Category("JCS"), DisplayName("Using")]
        //public Use JCSUse 
        //{
        //    get { return m_JCSUse; }
        //    set { m_JCSUse = value; }
        //}
        [Category("JCS"), DisplayName("Pass Request during Moving")]
        public bool ContinueMode
        {
            get { return m_ContinueMode; }
            set { m_ContinueMode = value; }
        }
        [Category("JCS"), DisplayName("Permit Wait Time(ms)")]
        public int PermitWaitTime 
        {
            get { return m_PermitWaitTime; }
            set { m_PermitWaitTime = value; }
        }
        [Category("JCS"), DisplayName("Message Reply Timeout(ms)")]
        public int MessageRepayTimeOut
        {
            get { return m_MessageRepayTimeOut; }
            set { m_MessageRepayTimeOut = value; }
        }
        
        [Category("JCS"), DisplayName("Status Report Interval Time(ms)")]
        public int StatusReportIntervalTime 
        {
            get { return m_StatusReportIntervalTime; }
            set 
            {
                if (value < 100) value = 100;
                m_StatusReportIntervalTime = value; 
            }
        }
        [Category("JCS"), DisplayName("Network Restart Connection Fail Count")]
        public int NetworkRestartFailCount
        {
            get { return m_NetworkRestartFailCount; }
            set { m_NetworkRestartFailCount = value; }
        }
        [Category("JCS"), DisplayName("Permit Retry Count(Alarm Occurred)")]
        public int PermitRetryCount
        {
            get { return m_PermitRetryCount; }
            set { m_PermitRetryCount = value; }
        }
        #endregion

        #region Constructor
        public SetupJCS()
        {

        }
        #endregion

        #region Methods
        #endregion
    }
}
