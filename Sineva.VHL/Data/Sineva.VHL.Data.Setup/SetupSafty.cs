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
    public partial class SetupSafty
    {
        #region Fields
        private Use m_PBSSensorUse = Use.Use;
        private int m_PBSSensorTimeOver = 5000;

        private Use m_STBDoubleCheckUse = Use.Use;
        private Use m_OBSUpperSensorUse = Use.Use;
        private Use m_OBSLowerSensorUse = Use.Use;
        private Use m_FoupCoverExistCheckUse = Use.Use; //
        private Use m_FoupCoverOpenCheckUse = Use.Use;  // Hoist
        private Use m_BeltCutCheckUse = Use.Use;
        private Use m_SwingSensorCheckUse = Use.Use;
        private Use m_BumpCollisionCheckUse = Use.Use;

        private int m_SteerOffTime = 500;
        private Use m_CheckFoupAfterGripOpen = Use.Use; //DEMO Line은 감지거리가 길어서 Open해도 감지됨.. DEMO Line만 사용할 것.

        private double m_FrontProfileDistance = 250.0f;
        private double m_RearProfileDistance = -300.0f;
        private double m_FrontSideProfileUnSafeDistance = 230.0f;
        private double m_RearSideProfileUnSafeDistance = -280.0f;

        private Use m_CpsVoltageCheckUse = Use.Use;
        private int m_CpsVoltageLow = 300; // 300 Volt
        private int m_CpsVoltageKeepTime = 1000; // msec

        //2024.12.31 Foup Cover
        private int m_CoverNotDetectCheckTime = 3000; // msec

        private int m_OBSAbnormalReleaseWaitTime = 2;
        #endregion

        #region Properties
        [Category("PBS"), DisplayName("Look Down Sensor Usage")]
        public Use PBSSensorUse 
        {
            get { return m_PBSSensorUse; }
            set { m_PBSSensorUse = value; }
        }
        [Category("PBS"), DisplayName("Look Down Sensor ON TimeOver(ms)")]
        public int PBSSensorTimeOver 
        {
            get { return m_PBSSensorTimeOver; }
            set { m_PBSSensorTimeOver = value; }
        }
        [Category("StorageCheck"), DisplayName("Foup Double Storage Check Usage")]
        public Use STBDoubleCheckUse 
        {
            get { return m_STBDoubleCheckUse; }
            set { m_STBDoubleCheckUse = value; }
        }
        [Category("OBS"), DisplayName("Obstacle Upper Sensor Usage")]
        public Use OBSUpperSensorUse 
        {
            get { return m_OBSUpperSensorUse; }
            set { m_OBSUpperSensorUse = value; }
        }
        [Category("OBS"), DisplayName("Obstacle Lower Sensor Usage")]
        public Use OBSLowerSensorUse 
        {
            get { return m_OBSLowerSensorUse; }
            set { m_OBSLowerSensorUse = value; }
        }
        [Category("FoupCover"), DisplayName("Foup Cover Exist Check Usage(Hoist)")]
        public Use FoupCoverExistCheckUse 
        {
            get { return m_FoupCoverExistCheckUse; }
            set { m_FoupCoverExistCheckUse = value; }
        }
        [Category("FoupCover"), DisplayName("Foup Cover Open Check Usage")]
        public Use FoupCoverOpenCheckUse 
        {
            get { return m_FoupCoverOpenCheckUse; }
            set { m_FoupCoverOpenCheckUse = value; }
        }
        [Category("HoistBelt"), DisplayName("Hoist Belt Cut Check Usage")]
        public Use BeltCutCheckUse
        {
            get { return m_BeltCutCheckUse; }
            set { m_BeltCutCheckUse = value; }
        }
        [Category("HoistSwing"), DisplayName("Hoist Swing Sensor Check Usage")]
        public Use SwingSensorCheckUse
        {
            get { return m_SwingSensorCheckUse; }
            set { m_SwingSensorCheckUse = value; }
        }
        [Category("Bump Collisition"), DisplayName("Bump Collision Sensor Check Usage")]
        public Use BumpCollisionCheckUse
        {
            get { return m_BumpCollisionCheckUse; }
            set { m_BumpCollisionCheckUse = value; }
        }        

        [Category("Steer"), DisplayName("Output OFF Delay Time after Steer Change(ms)")]
        public int SteerOffTime 
        {
            get { return m_SteerOffTime; }
            set { m_SteerOffTime = value; }
        }
        [Category("FoupDetect"), DisplayName("Use of the Foup detection sensor after Grip Open")]
        public Use CheckFoupAfterGripOpen //DEMO Line은 감지거리가 길어서 Open해도 감지됨.. DEMO Line만 사용할 것.
        {
            get { return m_CheckFoupAfterGripOpen; }
            set { m_CheckFoupAfterGripOpen = value; }
        }
        [Category("OHB Profile"), DisplayName("Front Side OHB Profile Distance")]
        public double FrontProfileDistance
        {
            get { return m_FrontProfileDistance; }
            set { m_FrontProfileDistance = value; }
        }
        [Category("OHB Profile"), DisplayName("Rear Side OHB Profile Distance")]
        public double RearProfileDistance
        {
            get { return m_RearProfileDistance; }
            set { m_RearProfileDistance = value; }
        }
        [Category("OHB Profile"), DisplayName("Front Side Unsafe Distance of OHB Profile")]
        public double FrontSideProfileUnSafeDistance
        {
            get { return m_FrontSideProfileUnSafeDistance; }
            set { m_FrontSideProfileUnSafeDistance = value; }
        }
        [Category("OHB Profile"), DisplayName("Rear Side Unsafe Distance of OHB Profile")]
        public double RearSideProfileUnSafeDistance
        {
            get { return m_RearSideProfileUnSafeDistance; }
            set { m_RearSideProfileUnSafeDistance = value; }
        }
        [Category("CPS"), DisplayName("Low Voltage Interlock Usage")]
        public Use CpsVoltageCheckUse
        {
            get { return m_CpsVoltageCheckUse; }
            set { m_CpsVoltageCheckUse = value; }
        }
        [Category("CPS"), DisplayName("Low Voltage Setting Value")]
        public int CpsVoltageLow
        {
            get { return m_CpsVoltageLow; }
            set { m_CpsVoltageLow = value; }
        }
        [Category("CPS"), DisplayName("Low Voltage Keep Time")]
        public int CpsVoltageKeepTime
        {
            get { return m_CpsVoltageKeepTime; }
            set { m_CpsVoltageKeepTime = value; }
        }
        [Category("FoupCover"), DisplayName("Foup Cover Exist Check Time(ms)")]
        public int CoverNotDetectCheckTime
        {
            get { return m_CoverNotDetectCheckTime; }
            set { m_CoverNotDetectCheckTime = value; }
        }
        [Category("OBS"), DisplayName("Abnormal Override Stop Release Time(sec)")]
        public int OBSAbnormalReleaseWaitTime
        {
            get { return m_OBSAbnormalReleaseWaitTime; }
            set { m_OBSAbnormalReleaseWaitTime = value; }
        }        
        #endregion

        #region Constructor
        public SetupSafty()
        {

        }

        #endregion

        #region Methods
        #endregion

    }
}
