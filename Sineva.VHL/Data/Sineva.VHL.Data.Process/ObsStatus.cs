using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Process
{
    [Serializable]
    public class ObsStatus
    {
        #region Fields
        private double m_CollisionMaxDistance = 0.0f;
        private double m_CollisionMinDistance = 0.0f;
        private double m_CollisionDistance = 0.0f;
        private double m_CollisionPosition = 0.0f;
        private bool m_IsOverrideResetting = false;
        private double m_OverrideRatio = 0.9999f;
        private double m_MxpOverrideRatio = 0.9999f;
        private double m_OverrideTime = 0.0f;
        private enFrontDetectState m_ObsUpperSensorState = enFrontDetectState.enNone;
        private enFrontDetectState m_ObsLowerSensorState = enFrontDetectState.enNone;
        private int m_ObsUpperAreaValue = 1;
        private int m_ObsLowerAreaValue = 1;
        private bool m_OverrideDontControl = false;

        private int m_IoModuleSlaveNo = 0;
        private int m_IoChannelOffset = 0;
        private int m_IoChannelStartAddress = 0;
        #endregion

        #region Properties
        public double CollisionMaxDistance
        {
            get { return m_CollisionMaxDistance; }
            set { m_CollisionMaxDistance = value; }
        }
        public double CollisionMinDistance
        {
            get { return m_CollisionMinDistance; }
            set { m_CollisionMinDistance = value; }
        }
        public double CollisionDistance 
        {
            get { return m_CollisionDistance; }
            set { m_CollisionDistance = value; }
        }
        public double CollisionPosition 
        {
            get { return m_CollisionPosition; }
            set { m_CollisionPosition = value; }
        }
        public bool IsOverrideResetting 
        {
            get { return m_IsOverrideResetting; }
            set { m_IsOverrideResetting = value; }
        }
        public double OverrideRatio 
        {
            get { return m_OverrideRatio; }
            set { m_OverrideRatio = value; }
        }
        public double MxpOverrideRatio
        {
            get { return m_MxpOverrideRatio; }
            set { m_MxpOverrideRatio = value; }
        }        
        public double OverrideTime 
        {
            get { return m_OverrideTime; }
            set { m_OverrideTime = value; }
        }
        public enFrontDetectState ObsUpperSensorState 
        {
            get { return m_ObsUpperSensorState; }
            set { m_ObsUpperSensorState = value; }
        }
        public enFrontDetectState ObsLowerSensorState 
        {
            get { return m_ObsLowerSensorState; }
            set { m_ObsLowerSensorState = value; }
        }
        public int ObsUpperAreaValue 
        {
            get { return m_ObsUpperAreaValue; }
            set { m_ObsUpperAreaValue = value; }
        }
        public int ObsLowerAreaValue 
        {
            get { return m_ObsLowerAreaValue; }
            set { m_ObsLowerAreaValue = value; }
        }
        public bool OverrideDontControl
        {
            get { return m_OverrideDontControl; }
            set { m_OverrideDontControl = value; }
        }
        public int IoModuleSlaveNo
        {
            get { return m_IoModuleSlaveNo; }
            set { m_IoModuleSlaveNo = value; }
        }
        public int IoChannelOffset
        {
            get { return m_IoChannelOffset; }
            set { m_IoChannelOffset = value; }
        }
        public int IoChannelStartAddress
        {
            get { return m_IoChannelStartAddress; }
            set { m_IoChannelStartAddress = value; }
        }
        #endregion

        public ObsStatus() 
        {
        }

    }
}
