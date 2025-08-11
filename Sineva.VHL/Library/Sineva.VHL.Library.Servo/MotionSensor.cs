using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.Servo
{
    [Editor(typeof(UIEditorPropertyEdit), typeof(UITypeEditor))]
    [Serializable]
    public class MotionSensor
    {
        private bool m_ControlMp = true;
        private UInt32 m_SensorUse = 0;
        private UInt32 m_SlaveNo = 9; // Left : 9, Right : 10
        private UInt16 m_Offset = 0;
        private UInt16 m_Size = 4;
        private float m_SensorTargetValue = 0.0f;
        private float m_SensorPositionSetRange = 0.5f;
        private float m_SensorPulseToUnit = 0.1f;
        private float m_SensorScanDistance = 0.0f;
        private float m_SensorScanVelocity = 2000.0f;
        private float m_SensorScanAcceleration = 2800.0f;
        private float m_SensorScanDeceleration = 2800.0f;
        private float m_SensorScanJerk = 2200.0f;
        #region Properties
        public bool ControlMp { get { return m_ControlMp; } set { m_ControlMp = value; } } //Motion PLC에서 Control할건지 상위 PC에서 할건지 결정 
        public UInt32 SensorUse { get {return m_SensorUse; } set { m_SensorUse = value; } } // 위치 센서 모니터링 사용 유무를 설정한다.
        public UInt32 SlaveNo { get {return m_SlaveNo; } set { m_SlaveNo = value; } } //위치 센서 입력 모듈의 Slave 번호를 설정합니다. 9번 or 10 번
        public UInt16 Offset { get {return m_Offset; } set { m_Offset = value; } } // 0x2013,  Position 센서 모듈의 Position Data 입력 위치를 설정합니다.
        public UInt16 Size { get {return m_Size; } set { m_Size = value; } } // 4Byte, Position 센서 모듈의 Position Data 크기를 설정합니다.
        public float SensorTargetValue { get {return m_SensorTargetValue; } set { m_SensorTargetValue = value; } } //이송 완료 위치에서의 위치센서값을 입력합니다.
        public float SensorPositionSetRange { get {return m_SensorPositionSetRange; } set { m_SensorPositionSetRange = value; } } //정지 위치 판정 거리를 입력 합니다. 
        public float SensorPulseToUnit { get {return m_SensorPulseToUnit; } set { m_SensorPulseToUnit = value; } } // Position 센서 1Count 에 해당하는 위치 값을 입력합니다.
        public float SensorScanDistance { get {return m_SensorScanDistance; } set { m_SensorScanDistance = value; } } // Position 센서를 이용 감속 운전시 감속 거리를 입력합니다
        public float SensorScanVelocity { get { return m_SensorScanVelocity; } set { m_SensorScanVelocity = value; } } // Position 센서를 이용 감속 운전시 속도를 입력합니다
        public float SensorScanAcceleration { get { return m_SensorScanAcceleration; } set { m_SensorScanAcceleration = value; } } // 정위치 정지를 위한 가속도
        public float SensorScanDeceleration { get { return m_SensorScanDeceleration; } set { m_SensorScanDeceleration = value; } } // 정위치 정지를 위한 감속도
        public float SensorScanJerk { get { return m_SensorScanJerk; } set { m_SensorScanJerk = value; } } // 정위치 정지를 위한 Jerk
        #endregion
        public MotionSensor()
        {

        }
    }
}
