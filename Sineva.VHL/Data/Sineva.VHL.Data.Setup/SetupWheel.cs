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
    public partial class SetupWheel
    {
        #region Fields
        private double m_StraightMaxAcc = 3000.0f;
        private double m_CurveMaxAcc = 2000;
        private double m_JunctionMaxAcc = 2000.0f;
        private double m_BranchMaxAcc = 2000.0f;

        private double m_StraightMaxDec = 3000.0f;
        private double m_CurveMaxDec = 2000;
        private double m_JunctionMaxDec = 2000.0f;
        private double m_BranchMaxDec = 2000.0f;

        private double m_StraightJerk = 3000.0f;
        private double m_CurveJerk = 2000;
        private double m_JunctionJerk = 2000.0f;
        private double m_BranchJerk = 2000.0f;

        private double m_StraightSpeed = 1000;
        private double m_CurveSpeed = 700;
        private double m_BranchSpeed = 700;
        private double m_JunctionSpeed = 700;
        private double m_BranchStraightSpeed = 700;
        private double m_JunctionStraightSpeed = 700;
        private double m_SBranchSpeed = 350;
        private double m_SJunctionSpeed = 350;

        private double m_InRange = 1.0f;
        private double m_InRangeMakePathStartPosition = 2.0f; // 한바퀴 돌지 않고 재자리 동작 범위
        private double m_InRangePortFindPosition = 2.0f;
        private double m_BCRScanPathDistance = 350.0f;
        private double m_BCRScanPathVelocity = 350.0f;
        private double m_CornerAdjustDistanceBefore = 100.0f;
        private double m_CornerAdjustDistanceAfter = 100.0f;

        private double m_VehicleMoveStartEnableCollisionDistance = 4000.0f;
        private double m_RescheduleDistanceBCREncorder = 500.0f;
        private int m_StopPositionOffset = -200; // -200 전에 멈춘다. MTL/SPL/JCS/AutoDoor에서 멈춰야 할 경우 

        private double m_OverrideStopDistance = 1000.0f;// Motor 잔여거리와 충돌 거리간 간격 비교. override 적용 유무 판단.
        private double m_OverrideLimitDistance = 300.0f; 
        private double m_OverrideAcceleration = 3000.0f;
        private double m_OverrideDeceleration = 3000.0f;
        #endregion

        #region Properties
        [Category("Junction"), DisplayName("Max ACC")]
        public double JunctionMaxAcc
        {
            get { return m_JunctionMaxAcc; }
            set { m_JunctionMaxAcc = value; }
        }
        [Category("Junction"), DisplayName("Max DEC")]
        public double JunctionMaxDec
        {
            get { return m_JunctionMaxDec; }
            set { m_JunctionMaxDec = value; }
        }
        [Category("Junction"), DisplayName("Jerk")]
        public double JunctionJerk
        {
            get { return m_JunctionJerk; }
            set { m_JunctionJerk = value; }
        }
        [Category("Junction"), DisplayName("Speed")]
        public double JunctionSpeed
        {
            get { return m_JunctionSpeed; }
            set { m_JunctionSpeed = value; }
        }
        [Category("Junction"), DisplayName("SJunctionSpeed")]
        public double SJunctionSpeed
        {
            get { return m_SJunctionSpeed; }
            set { m_SJunctionSpeed = value; }
        }

        [Category("Branch"), DisplayName("Max ACC")]
        public double BranchMaxAcc
        {
            get { return m_BranchMaxAcc; }
            set { m_BranchMaxAcc = value; }
        }
        [Category("Branch"), DisplayName("Max DEC")]
        public double BranchMaxDec
        {
            get { return m_BranchMaxDec; }
            set { m_BranchMaxDec = value; }
        }
        [Category("Branch"), DisplayName("Jerk")]
        public double BranchJerk
        {
            get { return m_BranchJerk; }
            set { m_BranchJerk = value; }
        }
        [Category("Branch"), DisplayName("Speed")]
        public double BranchSpeed
        {
            get { return m_BranchSpeed; }
            set { m_BranchSpeed = value; }
        }
        [Category("Branch"), DisplayName("SBranchSpeed")]
        public double SBranchSpeed
        {
            get { return m_SBranchSpeed; }
            set { m_SBranchSpeed = value; }
        }

        [Category("Curve"), DisplayName("Max ACC")]
        public double CurveMaxAcc
        {
            get { return m_CurveMaxAcc; }
            set { m_CurveMaxAcc = value; }
        }
        [Category("Curve"), DisplayName("Max DEC")]
        public double CurveMaxDec
        {
            get { return m_CurveMaxDec; }
            set { m_CurveMaxDec = value; }
        }
        [Category("Curve"), DisplayName("Jerk")]
        public double CurveJerk
        {
            get { return m_CurveJerk; }
            set { m_CurveJerk = value; }
        }
        [Category("Curve"), DisplayName("Speed")]
        public double CurveSpeed
        {
            get { return m_CurveSpeed; }
            set { m_CurveSpeed = value; }
        }

        [Category("Straight"), DisplayName("Max ACC")]
        public double StraightMaxAcc
        {
            get { return m_StraightMaxAcc; }
            set { m_StraightMaxAcc = value; }
        }
        [Category("Straight"), DisplayName("Max DEC")]
        public double StraightMaxDec
        {
            get { return m_StraightMaxDec; }
            set { m_StraightMaxDec = value; }
        }
        [Category("Straight"), DisplayName("Jerk")]
        public double StraightJerk
        {
            get { return m_StraightJerk; }
            set { m_StraightJerk = value; }
        }
        [Category("Straight"), DisplayName("Speed")]
        public double StraightSpeed
        {
            get { return m_StraightSpeed; }
            set { m_StraightSpeed = value; }
        }
        [Category("Straight"), DisplayName("BranchSpeed")]
        public double BranchStraightSpeed
        {
            get { return m_BranchStraightSpeed; }
            set { m_BranchStraightSpeed = value; }
        }
        [Category("Straight"), DisplayName("JunctionSpeed")]
        public double JunctionStraightSpeed
        {
            get { return m_JunctionStraightSpeed; }
            set { m_JunctionStraightSpeed = value; }
        }

        [Category("BCRScanParameter"), DisplayName("InRange_StartPosition")]
        public double InRangeMakePathStartPosition
        {
            get { return m_InRangeMakePathStartPosition; }
            set { m_InRangeMakePathStartPosition = value; }
        }
        [Category("BCRScanParameter"), DisplayName("InRange_PortFindPosition")]
        public double InRangePortFindPosition
        {
            get { return m_InRangePortFindPosition; }
            set { m_InRangePortFindPosition = value; }
        }
        [Category("BCRScanParameter"), DisplayName("Range")]
        public double InRange
        {
            get { return m_InRange; }
            set { m_InRange = value; }
        }
        [Category("BCRScanParameter"), DisplayName("Distance")]
        public double BCRScanPathDistance 
        {
            get { return m_BCRScanPathDistance; }
            set { m_BCRScanPathDistance = value; }
        }
        [Category("BCRScanParameter"), DisplayName("Velocity")]
        public double BCRScanPathVelocity 
        {
            get { return m_BCRScanPathVelocity; }
            set { m_BCRScanPathVelocity = value; }
        }
        [Category("AdjustDistance"), DisplayName("Before Curve")]
        public double CornerAdjustDistanceBefore
        {
            get { return m_CornerAdjustDistanceBefore; }
            set { m_CornerAdjustDistanceBefore = value; }
        }
        [Category("AdjustDistance"), DisplayName("After Curve")]
        public double CornerAdjustDistanceAfter
        {
            get { return m_CornerAdjustDistanceAfter; }
            set { m_CornerAdjustDistanceAfter = value; }
        }
        [Category("SeqVehicleMove"), DisplayName("Move Enable Collision Distance")]
        public double VehicleMoveStartEnableCollisionDistance
        {
            get { return m_VehicleMoveStartEnableCollisionDistance; }
            set { m_VehicleMoveStartEnableCollisionDistance = value; }
        }
        [Category("SeqVehicleMove"), DisplayName("Rechedule Interlock, distance difference between BCR and Encorder")]
        public double RescheduleDistanceBCREncorder
        {
            get { return m_RescheduleDistanceBCREncorder; }
            set { m_RescheduleDistanceBCREncorder = value; }
        }
        [Category("JCS"), DisplayName("Stop Position Offset(mm) _ minus value")]
        public int StopPositionOffset
        {
            get { return m_StopPositionOffset; }
            set { m_StopPositionOffset = value; }
        }

        [Category("Override"), DisplayName("Stop Distance")]
        public double OverrideStopDistance
        {
            get { return m_OverrideStopDistance; }
            set { m_OverrideStopDistance = value; }
        }
        [Category("Override"), DisplayName("Limit Distance")]
        public double OverrideLimitDistance
        {
            get { return m_OverrideLimitDistance; }
            set { m_OverrideLimitDistance = value; }
        }
        [Category("Override"), DisplayName("Acceleration")]
        public double OverrideAcceleration
        {
            get { return m_OverrideAcceleration; }
            set { m_OverrideAcceleration = value; }
        }
        [Category("Override"), DisplayName("Deceleration")]
        public double OverrideDeceleration
        {
            get { return m_OverrideDeceleration; }
            set { m_OverrideDeceleration = value; }
        }
        #endregion

        #region Constructor
        public SetupWheel()
        {

        }
        #endregion

        #region Methods
        #endregion
    }

}
