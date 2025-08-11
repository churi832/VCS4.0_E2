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
    public partial class SetupOCS
    {
        #region Fields
        private int m_StatusReportIntervalTime = 200;
        private double m_CancelAbortEnableDistance = 6000.0f;
        private int m_VehicleRemoveMTLNodeId = 81;
        private int m_NetworkRestartFailCount = 5;
        private Use m_UseFDCReport = Use.NoUse;
        #endregion

        #region Properties
        [Category("OCS"), DisplayName("Status Report Interval Time(ms)")]
        public int StatusReportIntervalTime 
        {
            get { return m_StatusReportIntervalTime; }
            set 
            {
                if (value < 100) value = 100;
                m_StatusReportIntervalTime = value; 
            }
        }
        [Category("OCS"), DisplayName("Cancel/Abort Enable Remain Distance(mm)")]
        public double CancelAbortEnableDistance
        {
            get { return m_CancelAbortEnableDistance; }
            set { m_CancelAbortEnableDistance = value; }
        }
        [Category("OCS"), DisplayName("Vehicle Remove MTL Node ID")]
        public int VehicleRemoveMTLNodeId
        {
            get { return m_VehicleRemoveMTLNodeId; }
            set { m_VehicleRemoveMTLNodeId = value; }
        }
        [Category("OCS"), DisplayName("Network Restart Connection Fail Count")]
        public int NetworkRestartFailCount
        {
            get { return m_NetworkRestartFailCount; }
            set { m_NetworkRestartFailCount = value; }
        }
        [Category("FDC"), DisplayName("Use FDC Report")]
        public Use UseFDCReport
        {
            get { return m_UseFDCReport; }
            set { m_UseFDCReport = value; }
        }
        #endregion

        #region Constructor
        public SetupOCS()
        {

        }
        #endregion

        #region Methods
        #endregion
    }
}
