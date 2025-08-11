using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Setup
{
    [Serializable]
    public partial class SetupPIO
    {
        #region Fields
        private int m_PioConnectionRetryTimes = 3;

        // SPL PIO
        private int m_PioSplStartOnTimeOut = 30000;
        private int m_PioSplInsertReqOffTimeOut = 30000;
        private int m_PioSplInsertReadyTimeOut = 10000;
        private int m_PioSplExportReqOffTimeOut = 30000;
        private int m_PioSplExportReadyTimeOut = 10000;
        private int m_PioSplBusyOnTimeOut = 30000;

        private int m_PioMtlStartOnTimeOut = 60000;
        private int m_PioMtlInsertReqOffTimeOut = 30000;
        private int m_PioMtlInsertReadyTimeOut = 10000;
        private int m_PioMtlExportReqOffTimeOut = 10000;
        private int m_PioMtlExportReadyTimeOut = 60000;
        private int m_PioMtlBusyOnTimeOut = 60000;

        private int m_PioInterlockWaitTime = 5; // 5sec wait after move
        private int m_PioSTKReadyWaitTime = 20;

        private bool m_PioAllAutoUsing = true;
        #endregion

        #region Properties
        [Category("PIO"), DisplayName("PIO Connection Retry Time")]
        public int PioConnectionRetryTimes
        {
            get { return m_PioConnectionRetryTimes; }
            set { m_PioConnectionRetryTimes = value; }
        }

        [Category("PIO"), DisplayName("PIO SPL Start ON Timeout")]
        public int PioSplStartOnTimeOut
        {
            get { return m_PioSplStartOnTimeOut; }
            set { m_PioSplStartOnTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO SPL Insert Request OFF Timeout")]
        public int PioSplInsertReqOffTimeOut
        {
            get { return m_PioSplInsertReqOffTimeOut; }
            set { m_PioSplInsertReqOffTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO SPL Insert Ready ON Timeout")]
        public int PioSplInsertReadyTimeOut
        {
            get { return m_PioSplInsertReadyTimeOut; }
            set { m_PioSplInsertReadyTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO SPL Export Request OFF Timeout")]
        public int PioSplExportReqOffTimeOut
        {
            get { return m_PioSplExportReqOffTimeOut; }
            set { m_PioSplExportReqOffTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO SPL Busy ON Timeout")]
        public int PioSplBusyOnTimeOut
        {
            get { return m_PioSplBusyOnTimeOut; }
            set { m_PioSplBusyOnTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO SPL Export Ready ON Timeout")]
        public int PioSplExportReadyTimeOut
        {
            get { return m_PioSplExportReadyTimeOut; }
            set { m_PioSplExportReadyTimeOut = value; }
        }

        [Category("PIO"), DisplayName("PIO MTL Start ON Timeout")]
        public int PioMtlStartOnTimeOut
        {
            get { return m_PioMtlStartOnTimeOut; }
            set { m_PioMtlStartOnTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO MTL Insert Request OFF Timeout")]
        public int PioMtlInsertReqOffTimeOut
        {
            get { return m_PioMtlInsertReqOffTimeOut; }
            set { m_PioMtlInsertReqOffTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO MTL Insert Ready ON Timeout")]
        public int PioMtlInsertReadyTimeOut
        {
            get { return m_PioMtlInsertReadyTimeOut; }
            set { m_PioMtlInsertReadyTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO MTL Export Request OFF Timeout")]
        public int PioMtlExportReqOffTimeOut
        {
            get { return m_PioMtlExportReqOffTimeOut; }
            set { m_PioMtlExportReqOffTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO MTL Busy ON Timeout")]
        public int PioMtlBusyOnTimeOut
        {
            get { return m_PioMtlBusyOnTimeOut; }
            set { m_PioMtlBusyOnTimeOut = value; }
        }
        [Category("PIO"), DisplayName("PIO MTL Export Ready ON Timeout")]
        public int PioMtlExportReadyTimeOut
        {
            get { return m_PioMtlExportReadyTimeOut; }
            set { m_PioMtlExportReadyTimeOut = value; }
        }
        [Category("PIO"), DisplayName("ES Signal Wait Time")]
        public int PioInterlockWaitTime
        {
            get { return m_PioInterlockWaitTime; }
            set { m_PioInterlockWaitTime = value; }
        }
        [Category("PIO"), DisplayName("STK Ready Wait Time")]
        public int PioSTKReadyWaitTime
        {
            get { return m_PioSTKReadyWaitTime; }
            set { m_PioSTKReadyWaitTime = value; }
        }

        [Category("PIO"), DisplayName("PIO All Auto Using (EQ Port, STK Port)")]
        public bool PioAllAutoUsing
        {
            get { return m_PioAllAutoUsing; }
            set { m_PioAllAutoUsing = value; }
        }
        #endregion

        #region Constructor
        public SetupPIO()
        {

        }
        #endregion

        #region Methods
        #endregion
    }
}
