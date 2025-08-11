using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.Setup
{
    [Serializable]
    public partial class SetupHoist
    {
        #region Fields
        private Use m_FoupGripperUse = Use.Use;
        private bool m_HoistTwoStepDown = false;
        private double m_HoistSensorSearchSpeed = 10.0f;
        private double m_HoistSensorSearchAcc = 10.0f;
        private double m_HoistSensorSearchDec = 10.0f;
        private double m_HoistSensorSearchJerk = 20.0f;
        private double m_HoistSensorDetectUpRangeLimitOHB = 10.0f; // Down Pos 기준 Foup 감지될수 있는 높이. UP<->Limit 사이값
        private double m_HoistSensorDetectUpRangeLimitPort = 20.0f; // Down Pos 기준 Foup 감지될수 있는 높이. UP<->Limit 사이값
        private double m_HoistSensorDetectDownRangeLimitOHB = -5.0f; // Down Pos 기준 Foup 감지될수 있는 높이. UP<->Limit 사이값
        private double m_HoistSensorDetectDownRangeLimitPort = -5.0f; // Down Pos 기준 Foup 감지될수 있는 높이. UP<->Limit 사이값
        private double m_HoistSensorDetectMoveDistance = -3.0f; // up sensor detect => -3mm down
        private Use m_HoistHomeSensorDetectOffset = Use.NoUse;
        private double m_HoistTorqueLimit = 100.0f;
        private double m_OriginOnDetectErrorRange = 3.0f;
        #endregion

        #region Properties
        [Category("AntiDrop"), DisplayName("Foup Gripper Usage(Use)")]
        public Use FoupGripperUse
        {
            get { return m_FoupGripperUse; }
            set { m_FoupGripperUse = value; }
        }
        [Category("Hoist"), DisplayName("Hoist Two Step Down [BeforeDown->Down]")]
        public bool HoistTwoStepDown
        {
            get { return m_HoistTwoStepDown; }
            set { m_HoistTwoStepDown = value; }
        }
        [Category("Hoist"), DisplayName("HoistSensorSearchSpeed")]
        public double HoistSensorSearchSpeed { get => m_HoistSensorSearchSpeed; set => m_HoistSensorSearchSpeed = value; }
        [Category("Hoist"), DisplayName("HoistSensorSearchAcceleration")]
        public double HoistSensorSearchAcc { get => m_HoistSensorSearchAcc; set => m_HoistSensorSearchAcc = value; }
        [Category("Hoist"), DisplayName("HoistSensorSearchDeceleration")]
        public double HoistSensorSearchDec { get => m_HoistSensorSearchDec; set => m_HoistSensorSearchDec = value; }
        [Category("Hoist"), DisplayName("HoistSensorSearchJerk")]
        public double HoistSensorSearchJerk { get => m_HoistSensorSearchJerk; set => m_HoistSensorSearchJerk = value; }
        [Category("Hoist"), DisplayName("HoistSensorDetectUpRangeLimitOHB")]
        public double HoistSensorDetectUpRangeLimitOHB { get => m_HoistSensorDetectUpRangeLimitOHB; set => m_HoistSensorDetectUpRangeLimitOHB = value; }
        [Category("Hoist"), DisplayName("HoistSensorDetectUpRangeLimitPORT")]
        public double HoistSensorDetectUpRangeLimitPort { get => m_HoistSensorDetectUpRangeLimitPort; set => m_HoistSensorDetectUpRangeLimitPort = value; }
        [Category("Hoist"), DisplayName("HoistSensorDetectDownRangeLimitOHB")]
        public double HoistSensorDetectDownRangeLimitOHB { get => m_HoistSensorDetectDownRangeLimitOHB; set => m_HoistSensorDetectDownRangeLimitOHB = value; }
        [Category("Hoist"), DisplayName("HoistSensorDetectDownRangeLimitPORT")]
        public double HoistSensorDetectDownRangeLimitPort { get => m_HoistSensorDetectDownRangeLimitPort; set => m_HoistSensorDetectDownRangeLimitPort = value; }
        [Category("Hoist"), DisplayName("HoistSensorDetectMoveDistance")]
        public double HoistSensorDetectMoveDistance { get => m_HoistSensorDetectMoveDistance; set => m_HoistSensorDetectMoveDistance = value; }
        [Category("Hoist"), DisplayName("HoistHomeSensorDetectedOffset(Used : Add Transfer Hoist Down Position)")]
        public Use HoistHomeSensorDetectOffset
        {
            get { return m_HoistHomeSensorDetectOffset; }
            set { m_HoistHomeSensorDetectOffset = value; }
        }
        [Category("Hoist"), DisplayName("Hoist Torque Limit(Move to Before Down(after Gripper Close))")]
        public double HoistTorqueLimit { get => m_HoistTorqueLimit; set => m_HoistTorqueLimit = value; }
        [Category("Hoist"), DisplayName("Origin On Deteect Error Range")]
        public double OriginOnDetectErrorRange { get => m_OriginOnDetectErrorRange; set => m_OriginOnDetectErrorRange = value; }
        
        #endregion

        #region Constructor
        public SetupHoist()
        {

        }
        #endregion

        #region Methods
        #endregion

    }
}
