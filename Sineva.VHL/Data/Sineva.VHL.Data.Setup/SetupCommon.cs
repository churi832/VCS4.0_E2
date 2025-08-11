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
    public partial class SetupCommon
    {
        #region Fields
        private string m_VHLVersion = "1.0";

        private Use m_CpuTempWarningUse = Use.NoUse;
        private float m_CpuTempLimit = 90.0f;
        private float m_CpuMemoryClearLimit = 100.0f;
        private int m_CpuMemoryCheckTime = 60 * 60 * 1000; //1hour

        private Use m_TorqueLogSaveUse = Use.Use;
        private bool m_ServoMoveAlert = false;
        private Use m_MelodyUse = Use.Use;
        #endregion

        #region Properties
        [Category("EQP"), DisplayName("1. EQP Generation")]
        public string VHLVersion
        {
            get { return m_VHLVersion; } 
            set { m_VHLVersion = value; }
        }
        [Category("PC"), DisplayName("1. Cpu Temperature Warning(Use)")]
        public Use CpuTempWarningUse
        {
            get { return m_CpuTempWarningUse; }
            set { m_CpuTempWarningUse = value; }
        }
        [Category("PC"), DisplayName("2. Cpu Temperature Limit(℃)")]
        public float CpuTempLimit
        {
            get { return m_CpuTempLimit; }
            set { m_CpuTempLimit = value; }
        }
        [Category("PC"), DisplayName("3. Cpu Memory Clear Limit(MB)")]
        public float CpuMemoryClearLimit
        {
            get { return m_CpuMemoryClearLimit; }
            set { m_CpuMemoryClearLimit = value; }
        }
        [Category("PC"), DisplayName("4. Cpu Memory Clear Check Time(sec)")]
        public int CpuMemoryCheckTime
        {
            get { return m_CpuMemoryCheckTime; }
            set { m_CpuMemoryCheckTime = value; }
        }

        [Category("LOG"), DisplayName("5. Torque Log Save Usage")]
        public Use TorqueLogSaveUse
        {
            get { return m_TorqueLogSaveUse; }
            set { m_TorqueLogSaveUse = value; }
        }
        [Category("EQP"), DisplayName("6. Servo Move Alert")]
        public bool ServoMoveAlert 
        {
            get { return m_ServoMoveAlert; }
            set { m_ServoMoveAlert = value; }
        }
        [Category("EQP"), DisplayName("7. Melody Use")]

        public Use MelodyUse
        {
            get { return m_MelodyUse; }
            set { m_MelodyUse = value; }
        }
        #endregion

        #region Constructor
        public SetupCommon()
        {

        }
        #endregion

        #region Methods
        #endregion
    }
}
