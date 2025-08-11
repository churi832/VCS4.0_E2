using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Data.DbAdapter;
using System.Windows.Forms;
using Sineva.VHL.IF.OCS;
using Sineva.VHL.Library;
using System.ComponentModel.Design;
using System.Reflection.Emit;
using System.Xml.Linq;
using System.ComponentModel;
using System.Drawing.Design;

namespace Sineva.VHL.Data.Process
{
    /// <summary>
    /// OCS에서 내려오면 명령을 넣어두자
    /// </summary>
    [Serializable]
    public class Command
    {
        #region Fields
        private bool m_IsValid = false; // 데이터 유효성 판단.
        private OCSCommand m_ProcessCommand = OCSCommand.None;
        private string m_CommandID = string.Empty;
        private string m_CassetteID = string.Empty;
        private int m_SourceID = 0;
        private int m_DestinationID = 0;
        private int m_StartNode = 0; // first Path의 ToNodeID
        private int m_EndNode = 0; // last Path의 FromNodeID
        private List<int> m_FullPathNodes = new List<int>(); // first Path FromNodeID ~ last Path ToNodeID
        private List<int> m_PathNodes = new List<int>(); // OCS에서 받은 Node List : first Path ToNodeID ~ last Path FromNodeID

        private enGoCommandType m_TypeOfDestination = enGoCommandType.None; //Go Command 구분( 1:By Location, 2:By Distance )
        private double m_TargetNodeToDistance = 0.0f;
        #endregion

        #region Properties
        public bool IsValid
        {
            get { return m_IsValid; }
            set { m_IsValid = value; }
        }
        public OCSCommand ProcessCommand
        {
            get { return m_ProcessCommand; }
            set { m_ProcessCommand = value; }
        }
        public string CommandID
        {
            get { return m_CommandID; }
            set { m_CommandID = value; }
        }
        public string CassetteID
        {
            get { return m_CassetteID; }
            set { m_CassetteID = value; }
        }
        [Editor(typeof(UIEditorPortSelect), typeof(UITypeEditor))]
        public int SourceID
        {
            get { return m_SourceID; }
            set { m_SourceID = value; }
        }
        [Editor(typeof(UIEditorPortSelect), typeof(UITypeEditor))]
        public int DestinationID
        {
            get { return m_DestinationID; }
            set { m_DestinationID = value; }
        }
        public int StartNode
        {
            get { return m_StartNode; }
            set { m_StartNode = value; }
        }
        public int EndNode
        {
            get { return m_EndNode; }
            set { m_EndNode = value; }
        }
        public List<int> FullPathNodes
        {
            get { return m_FullPathNodes; }
            set { m_FullPathNodes = value; }
        }
        public List<int> PathNodes
        {
            get { return m_PathNodes; }
            set { m_PathNodes = value; }
        }
        /// <summary>
        /// Go Command 구분( 1:By Location, 2:By Distance )
        /// </summary>
        public enGoCommandType TypeOfDestination
        {
            get { return m_TypeOfDestination; }
            set { m_TypeOfDestination = value; }
        }
        public double TargetNodeToDistance
        {
            get { return m_TargetNodeToDistance; }
            set { m_TargetNodeToDistance = value; }
        }
        #endregion

        #region Constructor
        public Command()
        {
        }
        #endregion

        #region Methods
        public void SetCopy(Command source)
        {
            try
            {
                this.m_ProcessCommand = source.ProcessCommand;
                this.m_CommandID = source.CommandID;
                this.m_CassetteID = source.CassetteID;
                this.m_SourceID = source.SourceID;
                this.m_DestinationID = source.DestinationID;
                this.m_StartNode = source.StartNode;
                this.m_EndNode = source.EndNode;
                this.m_FullPathNodes = source.FullPathNodes;
                this.m_PathNodes = source.PathNodes;
                this.m_TypeOfDestination = source.TypeOfDestination;
                this.m_TargetNodeToDistance = source.TargetNodeToDistance;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods - override
        public override string ToString()
        {
            return string.Format("{0} / {1} / {2} / {3} / {4} / {5}", m_CommandID, m_CassetteID, m_SourceID, m_DestinationID, m_TypeOfDestination, m_TargetNodeToDistance);
        }
        #endregion
    }
}
