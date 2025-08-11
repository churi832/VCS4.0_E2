/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/

namespace Sineva.VHL.Library
{
    public enum enSeqState
    {
        READY,
        RUN,
        ALARM,
        STOP,
    }

    public class XInitTag
    {
        #region Fields
        private string m_ItemName;
        private InitState m_State;
        private string m_CheckStatus;
        #endregion

        public static string PropertyName = "ItemName";
        public static string PropertyState = "State";
        public static string PropertyCheck = "CheckStatus";

        #region Properties
        public string ItemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; }
        }
        public InitState State
        {
            get { return m_State; }
            set { m_State = value; }
        }
        public string CheckStatus
        {
            get { return m_CheckStatus; }
            set { m_CheckStatus = value; }
        }
        #endregion


        #region Constructor
        public XInitTag()
        {
            m_ItemName = "NONE";
            m_State = InitState.Noop;
            m_CheckStatus = InitCheckState.NotReady;
        }
        public XInitTag(string name)
        {
            m_ItemName = name;
            m_State = InitState.Noop;
            m_CheckStatus = InitCheckState.NotReady;
        }
        #endregion

    }
}
