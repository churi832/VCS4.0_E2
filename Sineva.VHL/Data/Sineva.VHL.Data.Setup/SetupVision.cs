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
    public partial class SetupVision
    {
        #region Fields
        private Use m_AutoTeachingUse = Use.NoUse;
        private double m_SensorSearchDistance = -100.0f;
        private double m_SensorOffsetDistanceLOHB = 81.09f;
        private double m_SensorOffsetDistanceROHB = 81.09f;
        private double m_SensorOffsetDistancePORT = 26.4f;

        private double m_VisionSpecX = 1.0f;
        private double m_VisionSpecY = 1.0f;
        private double m_VisionSpecT = 1.0f;

        private double m_AutoTeachingRotateMoveRatio = 1;
        private double m_AutoTeachingSlideMoveRatio = 1;
        private double m_AutoTeachingHoistMoveOffsetPORT = 45.4f;
        private double m_AutoTeachingHoistMoveOffsetLOHB = -73.0f;
        private double m_AutoTeachingHoistMoveOffsetROHB = -73.0f;
        #endregion

        #region Properties
        [Category("AutoTeaching"), DisplayName("Use/NoUse")]
        public Use AutoTeachingUse { get => m_AutoTeachingUse; set => m_AutoTeachingUse = value; }
        [Category("AutoTeaching"), DisplayName("dX Error Spec")]
        public double VisionSpecX { get => m_VisionSpecX; set => m_VisionSpecX = value; }
        [Category("AutoTeaching"), DisplayName("dY Error Spec")]
        public double VisionSpecY { get => m_VisionSpecY; set => m_VisionSpecY = value; }
        [Category("AutoTeaching"), DisplayName("dT Error Spec")]
        public double VisionSpecT { get => m_VisionSpecT; set => m_VisionSpecT = value; }
        [Category("AutoTeaching"), DisplayName("Rotate Moving Ratio")]
        public double AutoTeachingRotateMoveRatio { get => m_AutoTeachingRotateMoveRatio; set => m_AutoTeachingRotateMoveRatio = value; }
        [Category("AutoTeaching"), DisplayName("Slide Moving Ratio")]
        public double AutoTeachingSlideMoveRatio { get => m_AutoTeachingSlideMoveRatio; set => m_AutoTeachingSlideMoveRatio = value; }

        [Category("AutoTeaching"), DisplayName("Hoist Moving Offset(Port)")]
        public double AutoTeachingHoistMoveOffsetPORT { get => m_AutoTeachingHoistMoveOffsetPORT; set => m_AutoTeachingHoistMoveOffsetPORT = value; }
        [Category("AutoTeaching"), DisplayName("Hoist Moving Offset(Left OHB)")]
        public double AutoTeachingHoistMoveOffsetLOHB { get => m_AutoTeachingHoistMoveOffsetLOHB; set => m_AutoTeachingHoistMoveOffsetLOHB = value; }
        [Category("AutoTeaching"), DisplayName("Hoist Moving Offset(Right OHB)")]
        public double AutoTeachingHoistMoveOffsetROHB { get => m_AutoTeachingHoistMoveOffsetROHB; set => m_AutoTeachingHoistMoveOffsetROHB = value; }
        [Category("AutoTeaching"), DisplayName("Sensor Search Distance")]
        public double SensorSearchDistance { get => m_SensorSearchDistance; set => m_SensorSearchDistance = value; }
        [Category("AutoTeaching"), DisplayName("Sensor Offset Distance(PORT)")]
        public double SensorOffsetDistancePORT { get => m_SensorOffsetDistancePORT; set => m_SensorOffsetDistancePORT = value; }
        [Category("AutoTeaching"), DisplayName("Sensor Offset Distance(Left OHB )")]
        public double SensorOffsetDistanceLOHB { get => m_SensorOffsetDistanceLOHB; set => m_SensorOffsetDistanceLOHB = value; }
        [Category("AutoTeaching"), DisplayName("Sensor Offset Distance(Right OHB)")]
        public double SensorOffsetDistanceROHB { get => m_SensorOffsetDistanceROHB; set => m_SensorOffsetDistanceROHB = value; }
        #endregion

        #region Constructor
        public SetupVision()
        {

        }

        #endregion

        #region Methods
        #endregion

    }
}
