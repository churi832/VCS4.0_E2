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
    public class SequenceState
    {
        #region Fields
        private int m_SequenceRemainCount = 0;
        private int m_SequenceCurrentStep = 0;
        private string m_SequenceStepName = string.Empty;
        private bool m_IsSequenceComplete = false;
        private bool m_IsSequenceBusy = false;
        private bool m_IsSequenceAlarm = false;
        private int m_SequenceAlarmId = 0;
        private bool m_IsExternalEncoderRun = false; // Run 중일 경우는 BCR 값을 가지고 완료 Check를 해야 하고 중복 Command를 내리면 않된다.
        private bool m_IsSequenceMoving = false; 
        #endregion

        #region Properties
        public int SequenceRemainCount { get => m_SequenceRemainCount; set => m_SequenceRemainCount = value; }
        public int SequenceCurrentStep { get => m_SequenceCurrentStep; set => m_SequenceCurrentStep = value; }
        public string SequenceStepName { get => m_SequenceStepName; set => m_SequenceStepName = value; }
        public bool IsSequenceComplete { get => m_IsSequenceComplete; set => m_IsSequenceComplete = value; }
        public bool IsSequenceBusy { get => m_IsSequenceBusy; set => m_IsSequenceBusy = value; }
        public bool IsSequenceAlarm { get => m_IsSequenceAlarm; set => m_IsSequenceAlarm = value; }
        public int SequenceAlarmId { get => m_SequenceAlarmId; set => m_SequenceAlarmId = value; }
        public bool IsExternalEncoderRun { get => m_IsExternalEncoderRun; set => m_IsExternalEncoderRun = value; }
        public bool IsSequenceMoving { get => m_IsSequenceMoving; set => m_IsSequenceMoving = value; }
        #endregion

        #region Constructor
        public SequenceState()
        {
        }
        #endregion
    }
}
