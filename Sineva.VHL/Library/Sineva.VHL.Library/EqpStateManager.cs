/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public class EqpStateManager
    {
        public readonly static EqpStateManager Instance = new EqpStateManager();

        #region Fields
        private HostControlMode m_HostMode = HostControlMode.Offline;
        private OperateMode m_OpMode = OperateMode.None;
        private EqpRunMode m_RunMode = EqpRunMode.None;
        private EqpState m_State = EqpState.Idle;

        private bool m_EqpAutoRunInitReq = false;
        private bool m_EqpRecoveryInitReq = false; //Auto Mode에서 초기화 할때 사용하자...
        private bool m_EqpInitReq = false;
        private bool m_EqpInitComp = false;
        private bool m_EqpForcedInitComp = false;
        private bool m_EqpAutoStartChangeToStop = false;
        private bool m_EqpInitStateReset = false;

        private List<XInitTag> m_EqpInitStates = new List<XInitTag>();
        private bool m_EqpInitStateChanged = false;
        #endregion

        #region Properties
        public HostControlMode HostMode
        {
            get { return m_HostMode; }
            set { m_HostMode = value; }
        }
        public OperateMode OpMode
        {
            get { return m_OpMode; }
        }
        public EqpRunMode RunMode
        {
            get { return m_RunMode; }
        }
        public EqpState State
        {
            get { return m_State; }
        }
        public List<XInitTag> EqpInitStates
        {
            get { return m_EqpInitStates; }
            set { m_EqpInitStates = value; }
        }
        public bool EqpRecoveryInitReq
        {
            get { return m_EqpRecoveryInitReq; }
            set { m_EqpRecoveryInitReq = value; }
        }
        public bool EqpAutoRunInitReq
        {
            get { return m_EqpAutoRunInitReq; }
            set { m_EqpAutoRunInitReq = value; }
        }
        public bool EqpInitReq
        {
            get { return m_EqpInitReq; }
            set { m_EqpInitReq = value; }
        }
        public bool EqpInitIng
        {
            get
            {
                if (m_EqpInitComp && !m_EqpInitStateReset) return false;

                bool initIng = false;
                foreach (XInitTag item in m_EqpInitStates)
                {
                    initIng |= item.State == InitState.Init | item.State == InitState.Fail;
                }

                return initIng;
            }
        }
        public bool EqpInitComp
        {
            get
            {
                if (m_EqpInitComp && !m_EqpInitStateReset) return true;

                bool initComp = true;
                foreach (XInitTag item in m_EqpInitStates)
                {
                    if (EqpForcedInitComp) item.State = InitState.Comp;
                    initComp &= item.State == InitState.Skip | item.State == InitState.Comp;
                }
                if (EqpForcedInitComp) EqpForcedInitComp = false;
                m_EqpInitComp = initComp;
                m_EqpInitStateReset = false; //한번 처리되었으면 false
                return initComp;
            }
        }
        public bool EqpAutoStartChangeToStop
        {
            get { return m_EqpAutoStartChangeToStop; }
            set { m_EqpAutoStartChangeToStop = value; }
        }
        public bool EqpForcedInitComp
        {
            get { return m_EqpForcedInitComp; }
            set { m_EqpForcedInitComp = value; }
        }
        #endregion

        #region Constructor
        public EqpStateManager() 
        {
        }
        #endregion

        #region Methods
        public void SetOpMode(OperateMode opMode)
        {
            if (m_OpMode != opMode)
            {
                EventHandlerManager.Instance.InvokeOperateModeChanged(opMode);
                SequenceLog.WriteLog(string.Format("[SetOpMode] Operator Mode {0} -> {1}", m_OpMode, opMode));
                m_OpMode = opMode;
            }
        }
        public void SetRunMode(EqpRunMode runMode)
        {
            if (m_RunMode != runMode)
            {
                EventHandlerManager.Instance.InvokeRunModeChanged(runMode);
                SequenceLog.WriteLog(string.Format("[SetRunMode] Change Run Mode {0} -> {1}", m_RunMode, runMode));
                m_RunMode = runMode;
            }
        }
        public void SetState(EqpState state)
        {
            m_State = state;
        }
        #endregion

        #region Methods - Init
        public void ClearInitItem()
        {
            m_EqpInitStates.Clear();
        }
        public bool RegisterInitItem(XInitTag tag)
        {
            if (string.IsNullOrEmpty(tag.ItemName))
                return false;

            foreach (XInitTag item in m_EqpInitStates)
            {
                if (item.ItemName == tag.ItemName)
                {
                    return false;
                }
            }

            m_EqpInitStates.Add(tag);
            m_EqpInitStateChanged = true;
            return true;
        }
        public void SetInitState(XInitTag tag)
        {
            foreach (XInitTag item in m_EqpInitStates)
            {
                if (item.ItemName == tag.ItemName)
                {
                    item.State = tag.State;
                    item.CheckStatus = tag.CheckStatus;
                    m_EqpInitStateChanged = true;
                }
            }
        }
        public void ResetInitState()
        {
            m_EqpInitStateReset = true;
            foreach (XInitTag item in m_EqpInitStates)
            {
                item.State = InitState.Noop;
                item.CheckStatus = InitCheckState.NotReady;
            }
        }
        public XInitTag[] GetEqpInitState(out bool changed)
        {
            changed = m_EqpInitStateChanged;
            m_EqpInitStateChanged = false;

            return m_EqpInitStates.ToArray();
        }

        public void SetOpCallMessage(OperatorCallKind kind, string message)
        {
            EventHandlerManager.Instance.InvokeOperatorCalled(kind, message);
        }
        public void SetOpCallCommand(string message, string[] commands, bool show)
        {
            EventHandlerManager.Instance.InvokeOperatorCallCommand(message, commands, show);
        }
        public void SetOpCallEmoSafty(bool show)
        {
            EventHandlerManager.Instance.InvokeEmoSaftyWindowActivate(show);
        }
        #endregion
    }
}
