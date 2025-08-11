/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library
{
    [Serializable]
    public class _Device
    {
        #region Fields
        private bool m_Initialized;
        private int m_DeviceId = 0;
        private string m_MyName;
        private string m_ParentName;
        private bool m_IsValid = false;

        private DateTime m_DeviceStartTime = DateTime.Now;
        private bool m_SaveCurState = false;
        #endregion

        #region Properties
        [Category("!Setting Device"), Description("Device Validation"), DeviceSetting(true)]
        public int DeviceId
        {
            get { return m_DeviceId; }
            set { m_DeviceId = value; }
        }
        [Category("!Setting Device"), Description("Device Name"), DeviceSetting(true)]
        public string MyName
        {
            get { return m_MyName; }
            set { m_MyName = value; }
        }
        [ReadOnly(true)]
        [Category("!Setting Device"), Description("Device Parent Name"), DeviceSetting(true)]
        public string ParentName
        {
            get { return m_ParentName; }
            set { m_ParentName = value; }
        }
        [Browsable(false), XmlIgnore()]
        public bool Initialized
        {
            get { return m_Initialized; }
            set { m_Initialized = value; }
        }
        [Category("!Setting Device"), Description("Device IsValid"), DeviceSetting(true)]
        public bool IsValid
        {
            get { return m_IsValid; }
            set { m_IsValid = value; }
        }
        [Category("!LifeTime Manager"), DisplayName("a.Start Time"), Description("Device Life Time Start"), DeviceSetting(false, true)]
        public DateTime DeviceStartTime
        {
            get { return m_DeviceStartTime; }
            set { m_DeviceStartTime = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public bool SaveCurState
        {
            get { return m_SaveCurState; }
            set { m_SaveCurState = value; }
        }
        #endregion

        #region Constructor
        #endregion

        #region Method
        public override string ToString()
        {
            string value = string.Format("{0}.{1}", this.ParentName, this.MyName);
            return value;
        }

        public virtual void SeqAbort()
        {
        }
        /// <summary>
        /// read_xml : 상위 device에서 xml을 읽어 들였을 경우(instance 중복 생성방지)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="read_xml"></param>
        /// <returns></returns>
        public virtual bool Initialize(string name = "", bool read_xml = true, bool heavy_alarm = true)
        {
            // 신규 Device 생성 시, _Device.Initialize() 내용 복사 후 붙여넣어서 사용하시오
            if (name != "") ParentName = name;
            if (read_xml) ReadXml();

            //////////////////////////////////////////////////////////////////////////////
            #region 1. 이미 초기화 완료된 상태인지 확인
            if (m_Initialized)
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
            if (!ok)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, string.Format("Initialize Fail : Indispensable I/O is not assigned({0})", name));
                return false;
            }
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
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 5. Device Sequence 생성
            //SeqExample = new SeqExample(this);
            #endregion
            //////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////
            #region 6. Initialize 마무으리
            m_Initialized = true;
            m_Initialized &= ok;
            #endregion
            //////////////////////////////////////////////////////////////////////////////

            return m_Initialized;
        }

        public virtual TimeSpan GetLifeTime()
        {
            if (!IsValid) return TimeSpan.Zero;
            TimeSpan rv = DateTime.Now - DeviceStartTime;
            return rv;
        }

        public virtual bool ReadXml()
        {
            bool rv = true;

            return rv;
        }

        public virtual void WriteXml()
        {

        }
        #endregion
    }
}
