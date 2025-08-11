/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Xml.Serialization;

namespace Sineva.VHL.Library
{
    [Serializable]
    public class XSeqCmd
    {
        #region Fields
        private string m_CmdTag = string.Empty;
        bool m_Start = false;
        bool m_Ing = false;
        bool m_End = false;
        bool m_Fail = false;
        bool m_Abort = false;
        bool m_Pause = false;
        #endregion

        #region Constructor
        public XSeqCmd()
        {
            Reset();
        }
        public XSeqCmd(string tag)
            : this()
        {
            m_CmdTag = tag;
        }
        #endregion

        #region Properties
        public string CmdTag
        {
            get { return m_CmdTag; }
            set { m_CmdTag = value; }
        }
        [XmlIgnore]
        public bool Start
        {
            get { return m_Start; }
            set
            {
                if (value)
                {
                    if (Ing || Fail || Abort) { }
                    else
                    {
                        m_Start = true;
                        m_Ing = true;
                        m_End = false;
                        m_Fail = false;
                        m_Abort = false;
                        m_Pause = false;
                    }
                }
                else
                {
                    m_Start = false;
                }
            }
        }
        [XmlIgnore]
        public bool End
        {
            get { return m_End; }
            set
            {
                m_End = value;
                if (value) m_Ing = false;
            }
        }
        [XmlIgnore]
        public bool Ing
        {
            get { return m_Ing; }
            set { m_Ing = value; }
        }
        [XmlIgnore]
        public bool Fail
        {
            get { return m_Fail; }
            set { m_Fail = value; }
        }
        [XmlIgnore]
        public bool Abort
        {
            get { return m_Abort; }
            set
            {
                m_Abort = value;
                if (value) m_Ing = false;
            }
        }
        [XmlIgnore]
        public bool Pause
        {
            get { return m_Pause; }
            set { m_Pause = value; }
        }
        #endregion

        #region Methods
        public void Reset()
        {
            m_Start = false;
            m_Ing = false;
            m_End = false;
            m_Fail = false;
            m_Abort = false;
            m_Pause = false;
        }
        #endregion

        public override string ToString()
        {
            return CmdTag;
        }

    }
}
