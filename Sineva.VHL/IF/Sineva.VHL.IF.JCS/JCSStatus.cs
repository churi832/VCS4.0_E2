using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.IF.JCS
{
    [Serializable]
    public class JCSStatus
    {
        #region Fields
        private bool m_Connected = false;

        private bool m_ConnectError = false;
        private string m_CommMessage = string.Empty;
        private bool m_StatusReportOk = false;
        private bool m_StatusReportStart = false;
        private List<int> m_JunctionAreaList = new List<int>();

        private List<JunctionControl> m_JunctionControl = new List<JunctionControl>();
        #endregion

        #region JCS Status Report Variable
        private int m_JunctionNumber = 0;
        private int m_CurrentNode = 0;
        private int m_CurrentLink = 0;
        private double m_CurrentPositionOfLink = 0.0;
        private bool m_InRailState = false;
        private bool m_WorkingState = false;
        private bool m_PauseState = false;
        private bool m_DownState = false;
        #endregion

        #region Event
        public event DelVoid_ObjectObject delMessageReceived;
        public event DelVoid_ObjectObject delMessageSent;
        public event DelVoid_Object delSocketClose;
        #endregion

        #region Properties
        public bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }

        public bool ConnectError
        {
            get { return m_ConnectError; }
            set { m_ConnectError = value; }
        }
        public string CommMessage
        {
            get { return m_CommMessage;}
            set { m_CommMessage = value; }
        }
        public bool StatusReportOk
        {
            get { return m_StatusReportOk; }
            set { m_StatusReportOk = value; }
        }
        public bool StatusReportStart
        {
            get { return m_StatusReportStart; }
            set { m_StatusReportStart = value; }
        }
        public List<int> JunctionAreaList
        {
            get { return m_JunctionAreaList; }
            set { m_JunctionAreaList = value; }
        }
        public List<JunctionControl> JunctionControl
        {
            get { return m_JunctionControl; }
            set { m_JunctionControl = value; }
        }
        #endregion

        #region Properties - JCS Status Report
        public int JunctionNumber { get => m_JunctionNumber; set => m_JunctionNumber = value; }
        public int CurrentNode { get => m_CurrentNode; set => m_CurrentNode = value; }
        public int CurrentLink { get => m_CurrentLink; set => m_CurrentLink = value; }
        public double CurrentPositionOfLink { get => m_CurrentPositionOfLink; set => m_CurrentPositionOfLink = value; }
        public bool InRailState { get => m_InRailState; set => m_InRailState = value; }
        public bool WorkingState { get => m_WorkingState; set => m_WorkingState = value; }
        public bool PauseState { get => m_PauseState; set => m_PauseState = value; }
        public bool DownState { get => m_DownState; set => m_DownState = value; }
        #endregion

        #region Events
        #endregion

        #region Constructors
        public JCSStatus()
        {
        }
        #endregion

        #region Methods - status report
        public JCSIF_SR GetStatusSendMessage()
        {
            try
            {
                JCSIF_SR message = new JCSIF_SR()
                {
                    JunctionNumber = JunctionNumber,
                    CurrentNode = CurrentNode,
                    CurrentLink = CurrentLink,
                    PositionInLink = CurrentPositionOfLink,
                    InRailState = InRailState,
                    WorkingState = WorkingState,
                    PauseState = PauseState,
                    DownState = DownState,
                };
                
                return message;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Methods
        public void FireMessageReceived(object obj1, object obj2)
        {
            if (delMessageReceived != null)
                delMessageReceived?.Invoke(obj1, obj2);
        }
        public void FireMessageSent(object obj1, object obj2)
        {
            if (delMessageSent != null)
                delMessageSent?.Invoke(obj1, obj2);
        }
        public void FireSocketClose(object obj)
        {
            if (delSocketClose != null)
                delSocketClose?.Invoke(obj);
        }
        #endregion
    }

    [Serializable]
    public class JunctionControl
    {
        public int index { get; set; }
        public int NodeId { get; set; }
        public int LinkId { get; set; }
        public bool JcsStop { get; set; }
        public double CheckDistance { get; set; }
        public bool RequestDone { get; set; }
        public bool PermitDone { get; set; }
        public bool IsJcsArea { get; set; }
        public JunctionControl() 
        { 
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", NodeId, LinkId, JcsStop, CheckDistance, RequestDone, PermitDone);
        }
    }
}
