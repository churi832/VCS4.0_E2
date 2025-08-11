using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Data.Setup;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Data.Process
{
    [Serializable]
    public class Path : DataItem_Link
    {
        #region Fields - Path Command
        private bool m_Certain = false; // 확실한 경우 PathMap가 있는 경우.. 그렇지 않으면 Database에 설정된 것만 사용해도 되는 경우
        private int m_Index = 0;
        private int m_FromLinkID = 0; // 0일 경우 시작 link
        private int m_ToLinkID = 0; // 0일 경우 마지막 link
        private bool m_BcrScanUse = false;
        private bool m_JcsPermit = false;
        private bool m_AutoDoorPermit = false;
        private bool m_MotionRun = false; // Motion Command를 Setting 했을 경우. InRange 관찰 대상...
        private double m_LinkVelocity = 0.0f;
        #endregion

        #region Fields - Path Monitor
        private double m_CurrentPositionOfLink; // Begin <-> Current
        private double m_RunPositionOfLink; // Start <-> Current
        private double m_RemainDistanceOfLink;  // Current <-> End
        private enMotionProc m_MotionProc = enMotionProc.none; // ready -> wait -> inProc -> Finished

        private enSteerDirection m_SteerDirection;
        private enBcrCheckDirection m_BcrDirection;
        private double m_RunDistance = 0.0f;
        private double m_TotalDistance = 0.0f; // 누적거리...+=RunDistance
        private double m_MotorTargetPosition = 0.0f; // Path의 Target위치를 지났는데도 Next Path Find를 못하는 경우 경로 이탈로 생각하자~~~
        private double m_CurrentMotorPositionOfLink;
        #endregion

        #region Properties
        public bool Certain
        {
            get { return m_Certain;}
            set { m_Certain = value;}
        }
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }
        public int FromLinkID
        {
            get { return m_FromLinkID; }
            set { m_FromLinkID = value; }
        }
        public int ToLinkID
        {
            get { return m_ToLinkID; }
            set { m_ToLinkID = value; }
        }
        public bool BcrScanUse
        {
            get { return m_BcrScanUse; }
            set { m_BcrScanUse = value; }
        }
        public bool JcsPermit
        {
            get { return m_JcsPermit; }
            set { m_JcsPermit = value; }
        }
        public bool AutoDoorPermit
        {
            get { return m_AutoDoorPermit; }
            set { m_AutoDoorPermit = value; }
        }
        public bool MotionRun
        {
            get { return m_MotionRun; }
            set { m_MotionRun = value; }
        }
        public double LinkVelocity
        {
            get { return m_LinkVelocity; }
            set { m_LinkVelocity = value; }
        }
        public enSteerDirection SteerDirection 
        {
            get { return m_SteerDirection; }
            set { m_SteerDirection = value; }
        }
        public enBcrCheckDirection BcrDirection
        {
            get { return m_BcrDirection; }
            set { m_BcrDirection = value; }
        }
        public double CurrentPositionOfLink 
        {
            get { return m_CurrentPositionOfLink; }
            set { m_CurrentPositionOfLink = value; }
        }
        public double RunPositionOfLink
        {
            get { return m_RunPositionOfLink; }
            set { m_RunPositionOfLink = value; }
        }        
        public double RemainDistanceOfLink 
        {
            get { return m_RemainDistanceOfLink; }
            set { m_RemainDistanceOfLink = value; }
        }
        public enMotionProc MotionProc
        {
            get { return m_MotionProc; }
            set { m_MotionProc = value; }
        }
        public double RunDistance
        {
            get 
            {
                double val = m_RunDistance;
                if (val == 0.0f) val = base.Distance; // Run Distance 설정이 않되었을 경우...
                return val; 
            }
            set { m_RunDistance = value; }
        }
        public double TotalDistance
        {
            get { return m_TotalDistance; }
            set { m_TotalDistance = value; }
        }
        public double MotorTargetPosition
        {
            get { return m_MotorTargetPosition; }
            set { m_MotorTargetPosition = value; }
        }
        public double CurrentMotorPositionOfLink
        {
            get { return m_CurrentMotorPositionOfLink; }
            set { m_CurrentMotorPositionOfLink = value; }
        }
        #endregion

        #region Constructor
        public Path()
        { 
        }
        public Path(bool certain, int index, DataItem_Link link)
        {
            SetCopy(link);
            m_Certain = certain;
            m_Index = index;
            m_RunDistance = link.Distance;
            m_SteerDirection = link.SteerDirectionValue;
            m_BcrDirection = link.BCRMatchType == "L" ? enBcrCheckDirection.Left :
                link.BCRMatchType == "R" ? enBcrCheckDirection.Right : enBcrCheckDirection.Both;

            m_BcrScanUse = false;
            m_JcsPermit = false;
            m_AutoDoorPermit = false;
            m_MotionRun = false;
            m_LinkVelocity = link.Velocity;
            m_RemainDistanceOfLink = m_RunDistance;
        }
        #endregion

        #region Methods
        public bool IsSame(Path other)
        {
            bool rv = true;
            rv &= this.Certain == other.Certain;
            rv &= this.Index == other.Index;
            rv &= this.LinkID == other.LinkID;
            rv &= this.FromNodeID == other.FromNodeID;
            rv &= this.ToNodeID == other.ToNodeID;
            rv &= this.FromLinkID == other.FromLinkID;
            rv &= this.ToLinkID == other.ToLinkID;
            rv &= this.TotalDistance == other.TotalDistance;
            rv &= this.RunDistance == other.RunDistance;
            rv &= this.SteerDirection == other.SteerDirection;
            rv &= this.BcrDirection == other.BcrDirection;
            rv &= this.BcrScanUse == other.BcrScanUse;
            rv &= this.LeftBCRStart == other.LeftBCRStart;
            rv &= this.RightBCRStart == other.RightBCRStart;
            rv &= this.LeftBCRTarget == other.LeftBCRTarget;
            rv &= this.RightBCRTarget == other.RightBCRTarget;
            return rv;
        }
        // 현위치가 어디에 가깞냐 ?
        // 가까운쪽 선택
        public DataItem_Node IsNearNode(double LeftBcr, double RightBcr)
        {
            try
            {
                if (this.FromNodeID == 0 || this.ToNodeID == 0) return null;

                double stop_range = Math.Abs(SetupManager.Instance.SetupWheel.StopPositionOffset) + 15.0f;
                DataItem_Node fromNode = DatabaseHandler.Instance.GetNodeDataOrNull(this.FromNodeID);
                DataItem_Node toNode = DatabaseHandler.Instance.GetNodeDataOrNull(this.ToNodeID);
                double fromBCR = m_BcrDirection == enBcrCheckDirection.Right ? fromNode.LocationValue2 : fromNode.LocationValue1;
                double endBCR = m_BcrDirection == enBcrCheckDirection.Right ? fromNode.LocationValue2 + this.Distance : fromNode.LocationValue1 + this.Distance;
                double toBCR = m_BcrDirection == enBcrCheckDirection.Right ? toNode.LocationValue2 : toNode.LocationValue1;
                double curBcr = m_BcrDirection == enBcrCheckDirection.Right ? RightBcr : LeftBcr;
                bool endSelect = true;
                endSelect &= Math.Abs(curBcr - fromBCR) > Math.Abs(curBcr - endBCR);
                endSelect &= Math.Abs(curBcr - endBCR) < stop_range;
                bool toSelect = true;
                toSelect &= Math.Abs(curBcr - fromBCR) > Math.Abs(curBcr - toBCR);
                toSelect &= Math.Abs(curBcr - toBCR) < stop_range;
                if (endSelect || toSelect) return toNode;
                else return fromNode;
            }
            catch
            {
                return null;
            }
        }
        public DataItem_Node IsFromNode()
        {
            DataItem_Node fromNode = DatabaseHandler.Instance.GetNodeDataOrNull(this.FromNodeID);
            return fromNode;
        }
        public DataItem_Node IsToNode()
        {
            DataItem_Node toNode = DatabaseHandler.Instance.GetNodeDataOrNull(this.ToNodeID);
            return toNode;
        }
        #endregion

        #region override
        public override string ToString()
        {
            return string.Format("from:{0}~to:{1},{2},{3},{4},{5},{6},{7},{8}, {9}", FromNodeID, ToNodeID, LinkID, Type, Distance, Velocity, BcrDirection, SteerDirection, Index, MotionProc);
        }
        #endregion
    }
}
