using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sineva.VHL.Data.Alarm;
using System.IO;

namespace Sineva.VHL.Device.ServoControl
{
    #region XmlInclude
    [XmlInclude(typeof(AxisTag))]
    [XmlInclude(typeof(ServoUnitTag))]
    #endregion

    [Serializable]
    public class _DevServoUnit : _Device
    {
        private const string DevName = "DevServoUnit";

        #region Fields
        private ServoUnitTag m_ServoTag = null;
        private List<_DevAxis> m_DevAxes = null;
        #endregion

        #region Properties
        public ServoUnitTag ServoTag
        {
            get { return m_ServoTag; }
            set { m_ServoTag = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public List<_DevAxis> DevAxes
        {
            get { return m_DevAxes; }
            set { m_DevAxes = value; }
        }
        #endregion

        #region Constructor
        public _DevServoUnit()
        {
            if (!Initialized)
            {
                this.MyName = DevName;
            }
        }
        public _DevServoUnit(ServoUnitTag tag)
        {
            if (!Initialized)
            {
                m_ServoTag = tag;
                this.MyName = m_ServoTag.ServoName;
            }
        }
        #endregion

        #region Override
        public override bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            // 신규 Device 생성 시, _Device.Initialize() 내용 복사 후 붙여넣어서 사용하시오
            if (name != "") this.ParentName = name;
            //if (read_xml) ReadXml();

            //////////////////////////////////////////////////////////////////////////////
            #region 1. 이미 초기화 완료된 상태인지 확인
            if (Initialized)
            {
                if (false)
                {
                    // 초기화된 상태에서도 변경이 가능한 항목을 추가
                }
                return true;
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 2. 필수 I/O 할당 여부 확인
            bool ok = true;
            //ok &= new object() != null;
            //ok &= m_SubDevice.Initiated;
            ok &= m_ServoTag != null;
            ok &= m_ServoTag.GetServoUnit() != null;
            if (!ok)
            {
                ExceptionLog.WriteLog(string.Format("Initialize Fail : Indispensable DevServoUnit is not assigned({0})", name));
                return false;
            }
            this.MyName = m_ServoTag.ServoName;
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 3. Alarm Item 생성
            //AlarmExample = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            //if(Condition) AlarmConditionable = AlarmCreator.NewAlarm(ALARM_NAME, ALARM_LEVEL, ALARM_CODE);
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////
            #region 4. Device Variable 초기화
            //m_Variable = false;
            m_DevAxes = new List<_DevAxis>();
            foreach (_Axis axis in m_ServoTag.GetServoUnit().Axes)
            {
                _DevAxis devAxis = new _DevAxis(m_ServoTag, new AxisTag(axis.AxisName, axis.AxisId));
                devAxis.IsValid = true;
                m_DevAxes.Add(devAxis);
            }
            foreach (_DevAxis dev in m_DevAxes)
            {
                dev.Initialize(MyName, false);
            }
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 6. Initialize 마무으리
            Initialized = true;
            Initialized &= ok;
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            return Initialized;
        }
        #endregion

        #region Methods
        public ServoUnit GetServoUnit()
        {
            if (m_ServoTag == null) return null;
            return m_ServoTag.GetServoUnit();
        }
        public string GetName()
        {
            if (m_ServoTag == null) return string.Empty;
            if (m_ServoTag.GetServoUnit() == null) return string.Empty;
            return GetServoUnit().ServoName;
        }
        public int GetId()
        {
            if (ServoTag == null) return -1;
            if (m_ServoTag.GetServoUnit() == null) return -1;
            return GetServoUnit().ServoId;
        }
        #endregion
    }
}
