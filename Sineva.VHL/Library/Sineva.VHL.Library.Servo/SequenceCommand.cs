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
    public class SequenceCommand
    {
        #region Fields
        private enMotionMoveMethod m_MoveMethod = enMotionMoveMethod.Relative;
        private enMotionBufferMode m_BufferMode = enMotionBufferMode.BlendingLow;
        private List<MotionProfile> m_MotionProfiles = new List<MotionProfile>();
        private MotionSensor m_PositionSensorInfo = new MotionSensor();
        private bool m_ExternalEncoderAbort = false; // 만일 BCR Scan 중일때 멈추고 다음 명령을 내려야 하는 경우가 발생하면 abort 후 처리해야지 ~~~.
        #endregion

        #region Properties
        public int MotionProfileCount { get { return m_MotionProfiles.Count; } }
        public enMotionMoveMethod MoveMethod { get => m_MoveMethod; set => m_MoveMethod = value; }
        public enMotionBufferMode BufferMode { get => m_BufferMode; set => m_BufferMode = value; }
        public List<MotionProfile> MotionProfiles { get => m_MotionProfiles; set => m_MotionProfiles = value; }
        public MotionSensor PositionSensorInfo { get => m_PositionSensorInfo; set => m_PositionSensorInfo = value; }
        public bool ExternalEncoderAbort { get => m_ExternalEncoderAbort; set => m_ExternalEncoderAbort = value; }
        #endregion

        public SequenceCommand() 
        {

        }

        #region
        public string GetCommandLog()
        {
            string msg = string.Empty;
            msg += string.Format("\tMoveMethod : {0}\r\n", m_MoveMethod);
            msg += string.Format("\tBufferMode : {0}\r\n", m_BufferMode);
            msg += string.Format("\tBCR Scan Control MP : {0}\r\n", m_PositionSensorInfo.ControlMp);
            msg += string.Format("\tBCR Scan Sensor Use : {0}\r\n", m_PositionSensorInfo.SensorUse);
            msg += string.Format("\tBCR Scan SlaveNo : {0}\r\n", m_PositionSensorInfo.SlaveNo);
            msg += string.Format("\tBCR Scan In Range : {0}\r\n", m_PositionSensorInfo.SensorPositionSetRange);
            msg += string.Format("\tBCR Scan Distance : {0}\r\n", m_PositionSensorInfo.SensorScanDistance);
            msg += string.Format("\tBCR Scan Velocity : {0}\r\n", m_PositionSensorInfo.SensorScanVelocity);
            msg += string.Format("\tBCR Scan Acceleration : {0}\r\n", m_PositionSensorInfo.SensorScanAcceleration);
            msg += string.Format("\tBCR Scan Deceleration : {0}\r\n", m_PositionSensorInfo.SensorScanDeceleration);
            msg += string.Format("\tBCR Scan Jerk : {0}\r\n", m_PositionSensorInfo.SensorScanJerk);
            msg += string.Format("\tProfiles Info : \r\n", m_BufferMode);
            int index = 0;
            foreach (MotionProfile motion in m_MotionProfiles) msg += string.Format("\t\tStep[{0}] : {1}\r\n", index++, motion.ToString());
            return msg;
        }
        #endregion
    }
}
