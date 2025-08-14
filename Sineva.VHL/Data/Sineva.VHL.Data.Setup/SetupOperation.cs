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
    public partial class SetupOperation
    {
        #region Fields
        private Use m_AntiDropUse = Use.Use;
        private Use m_RFIdUse = Use.Use;
        private Use m_AutoDoor1Use = Use.Use;
        private Use m_AutoDoor2Use = Use.Use;
        private Use m_OutRiggerUse = Use.Use;
        private OBSType m_OBSType = OBSType.SOS;
        private double m_LinkSearchRange = 400.0f;
        private double m_LinkSearchDiffRange = 200.0f;
        private double m_AutoDoor1CheckStartDistance = 8000.0f;
        private double m_AutoDoor2CheckStartDistance = 8000.0f;
        private double m_TorqueLimitApplyMargin_BeforeCurve = 0.0f;
        private double m_TorqueLimitApplyMargin_AfterCurve = 200.0f;
        private Use m_SlientStopAlarmUse = Use.Use;
        private Use m_BcrScanTwoStepUse = Use.Use;
        private Use m_NetworkControlUse = Use.NoUse;
        private int m_SilentStopWaitTime = 5;
        private Use m_Early_Motion_Use = Use.NoUse;
        private Use m_Continuous_Motion_Use = Use.NoUse;
        private double m_Early_Motion_Range = 500.0f;
        #endregion

        #region Properties
        [Category("AntiDrop"), DisplayName("Anti Drop Usage(Use)")]
        public Use AntiDropUse
        {
            get { return m_AntiDropUse; }
            set { m_AntiDropUse = value; }
        }
        [Category("RFID"), DisplayName("RF ID Usage(Use)")]
        public Use RFIdUse
        {
            get { return m_RFIdUse; }
            set { m_RFIdUse = value; }
        }
        [Category("AutoDoor"), DisplayName("AutoDoor1 Usage(Use)")]
        public Use AutoDoor1Use
        {
            get { return m_AutoDoor1Use; }
            set { m_AutoDoor1Use = value; }
        }
        [Category("AutoDoor"), DisplayName("AutoDoor2 Usage(Use)")]
        public Use AutoDoor2Use
        {
            get { return m_AutoDoor2Use; }
            set { m_AutoDoor2Use = value; }
        }
        [Category("OutRigger"), DisplayName("OutRigger Usage(Use)")]
        public Use OutRiggerUse
        {
            get { return m_OutRiggerUse; }
            set { m_OutRiggerUse = value; }
        }
        [Category("OBS"), DisplayName("OBS Type Select")]
        public OBSType OBSType
        {
            get { return m_OBSType; }
            set { m_OBSType = value; }
        }
        [Category("LinkSearch"), DisplayName("Similar Bcr Find Range")]
        public double LinkSearchRange 
        {
            get { return m_LinkSearchRange; }
            set { m_LinkSearchRange = value; }
        }
        [Category("LinkSearch"), DisplayName("Difference Bcr Range of between left and right")]
        public double LinkSearchDiffRange 
        {
            get { return m_LinkSearchDiffRange; }
            set { m_LinkSearchDiffRange = value; }
        }
        [Category("AutoDoor"), DisplayName("Interface Start Distance from Auto Door Node")]
        public double AutoDoor1CheckStartDistance 
        {
            get { return m_AutoDoor1CheckStartDistance; }
            set { m_AutoDoor1CheckStartDistance = value; }
        }
        [Category("AutoDoor"), DisplayName("Interface Start Distance from Auto Door Node")]
        public double AutoDoor2CheckStartDistance
        {
            get { return m_AutoDoor2CheckStartDistance; }
            set { m_AutoDoor2CheckStartDistance = value; }
        }
        [Category("TorqueLimit"), DisplayName("Torque Limit Apply Margin Before Curve")]
        public double TorqueLimitApplyMargin_BeforeCurve
        {
            get { return m_TorqueLimitApplyMargin_BeforeCurve; }
            set { m_TorqueLimitApplyMargin_BeforeCurve = value; }
        }
        [Category("TorqueLimit"), DisplayName("Torque Limit Apply Margin After Curve")]
        public double TorqueLimitApplyMargin_AfterCurve
        {
            get { return m_TorqueLimitApplyMargin_AfterCurve; }
            set { m_TorqueLimitApplyMargin_AfterCurve = value; }
        }
        [Category("Vehicle"), DisplayName("Vehicle Silent Stop Alarm Usage(Use)")]
        public Use SlientStopAlarmUse
        {
            get { return m_SlientStopAlarmUse; }
            set { m_SlientStopAlarmUse = value; }
        }
        [Category("Vehicle"), DisplayName("BCR Scan Two Step Usage(Use)")]
        public Use BcrScanTwoStepUse
        {
            get { return m_BcrScanTwoStepUse; }
            set { m_BcrScanTwoStepUse = value; }
        }
        [Category("Vehicle"), DisplayName("Network Reconnection Retry Usage(Use)")]
        public Use NetworkControlUse
        {
            get { return m_NetworkControlUse; }
            set { m_NetworkControlUse = value; }
        }
        [Category("Vehicle"), DisplayName("Silent Stop Wait Time")]
        public int SilentStopWaitTime
        {
            get { return m_SilentStopWaitTime; }
            set { m_SilentStopWaitTime = value; }
        }
        [Category("Transfer Motion"), DisplayName("Early Motion Usage(Use)")]
        public Use Early_Motion_Use
        {
            get { return m_Early_Motion_Use; }
            set { m_Early_Motion_Use = value; }
        }
        [Category("Transfer Motion"), DisplayName("Early Motion Range")]
        public double Early_Motion_Range
        {
            get { return m_Early_Motion_Range; }
            set { m_Early_Motion_Range = value; }
        }
        [Category("Transfer Motion"), DisplayName("Continuous Motion Usage(Use)")]
        public Use Continuous_Motion_Use
        {
            get { return m_Continuous_Motion_Use; }
            set { m_Continuous_Motion_Use = value; }
        }
        #endregion

        #region Constructor
        public SetupOperation()
        {

        }
        #endregion

        #region Methods
        #endregion

    }
}
