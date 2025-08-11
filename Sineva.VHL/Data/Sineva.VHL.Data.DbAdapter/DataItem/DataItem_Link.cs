using Sineva.VHL.Library;
using System;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_Link : DataItem
    {
        #region Fields
        private int m_LinkID = 0;
        private bool m_UseFlag = false;
        private int m_FromNodeID = 0;
        private int m_ToNodeID = 0;
        private LinkType m_Type = LinkType.Straight;
        private enSteerDirection m_SteerDirectionValue = enSteerDirection.DontCare;
        private string m_BCRMatchType = string.Empty;
        private int m_SteerChangeLeftBCR = 0;
        private int m_SteerChangeRightBCR = 0;
        private double m_Time = 0;
        private int m_Weight = 0;

        private double m_Distance = 0;
        private double m_Velocity = 0.0f;
        private double m_Acceleration = 0.0f;
        private double m_Deceleration = 0.0f;
        private double m_Jerk = 0.0f;

        private int m_UBSLevel0 = 0;
        private int m_UBSLevel1 = 0;

        private bool m_RouteChangeCheck = false;
        private double m_SteerGuideLengthFromNode = 0;
        private double m_SteerGuideLengthToNode = 0;
        private bool m_JCSAreaFlag = false;

        private int m_TorqueLimit = 300; // DB DELETE....240501
        private int m_UBSOnlyUsingSensorNo = 0; // DB DELETE...240501
        private double m_LeftBCRBegin = 0; // Link 시작노드 위치
        private double m_RightBCRBegin = 0;
        private double m_LeftBCREnd = 0; // Link 끝노드 위치
        private double m_RightBCREnd = 0;
        private double m_LeftBCRStart = 0; // Link 내에서 출발 위치가 필요...
        private double m_RightBCRStart = 0;
        private double m_LeftBCRTarget = 0; // Link 내에서 멈추는 위치가 필요...
        private double m_RightBCRTarget = 0;
        private int m_FromExtensionDistance = 0; // +- 값을 설정가능
        private int m_ToExtensionDistance = 0;
        #endregion

        #region Properties - Database
        [DatabaseSettingAttribute(true)]
        public int LinkID
        {
            get { return m_LinkID; }
            set { m_LinkID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public bool UseFlag
        {
            get { return m_UseFlag; }
            set { m_UseFlag = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int FromNodeID
        {
            get { return m_FromNodeID; }
            set { m_FromNodeID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int ToNodeID
        {
            get { return m_ToNodeID; }
            set { m_ToNodeID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public LinkType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        [DatabaseSettingAttribute(true)]
        public enSteerDirection SteerDirectionValue
        {
            get { return m_SteerDirectionValue; }
            set { m_SteerDirectionValue = value; }
        }
        [DatabaseSettingAttribute(true)]
        public string BCRMatchType
        {
            get { return m_BCRMatchType; }
            set { m_BCRMatchType = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int SteerChangeLeftBCR
        {
            get { return m_SteerChangeLeftBCR; }
            set { m_SteerChangeLeftBCR = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int SteerChangeRightBCR
        {
            get { return m_SteerChangeRightBCR; }
            set { m_SteerChangeRightBCR = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }

        [DatabaseSettingAttribute(true)]
        public double Distance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double Acceleration
        {
            get { return m_Acceleration; }
            set { m_Acceleration = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double Deceleration
        {
            get { return m_Deceleration; }
            set { m_Deceleration = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double Jerk
        {
            get { return m_Jerk; }
            set { m_Jerk = value; }
        }

        [DatabaseSettingAttribute(true)]
        public int UBSLevel0
        {
            get { return m_UBSLevel0; }
            set { m_UBSLevel0 = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int UBSLevel1
        {
            get { return m_UBSLevel1; }
            set { m_UBSLevel1 = value; }
        }
        [DatabaseSettingAttribute(true)]
        public bool RouteChangeCheck
        {
            get { return m_RouteChangeCheck; }
            set { m_RouteChangeCheck = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double SteerGuideLengthFromNode
        {
            get { return m_SteerGuideLengthFromNode; }
            set { m_SteerGuideLengthFromNode = value; }
        }
        [DatabaseSettingAttribute(true)]
        public double SteerGuideLengthToNode
        {
            get { return m_SteerGuideLengthToNode; }
            set { m_SteerGuideLengthToNode = value; }
        }
        [DatabaseSettingAttribute(true)]
        public bool JCSAreaFlag
        {
            get { return m_JCSAreaFlag; }
            set { m_JCSAreaFlag = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int FromExtensionDistance
        {
            get { return m_FromExtensionDistance; }
            set { m_FromExtensionDistance = value; }
        }
        [DatabaseSettingAttribute(true)]
        public int ToExtensionDistance
        {
            get { return m_ToExtensionDistance; }
            set { m_ToExtensionDistance = value; }
        }
        #endregion

        #region Properties - 
        [DatabaseSettingAttribute(false)]
        public double TotalTimeWeight
        {
            get { return m_Weight + m_Time; }
        }
        [DatabaseSettingAttribute(false)]
        public int TorqueLimit
        {
            get { return m_TorqueLimit; }
            set { m_TorqueLimit = value; }
        }
        [DatabaseSettingAttribute(false)]
        public int UBSOnlyUsingSensorNo
        {
            get { return m_UBSOnlyUsingSensorNo; }
            set { m_UBSOnlyUsingSensorNo = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double LeftBCRBegin
        {
            get { return m_LeftBCRBegin; }
            set { m_LeftBCRBegin = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double RightBCRBegin
        {
            get { return m_RightBCRBegin; }
            set { m_RightBCRBegin = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double LeftBCREnd
        {
            get { return m_LeftBCREnd; }
            set { m_LeftBCREnd = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double RightBCREnd
        {
            get { return m_RightBCREnd; }
            set { m_RightBCREnd = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double LeftBCRStart
        {
            get { return m_LeftBCRStart; }
            set { m_LeftBCRStart = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double RightBCRStart
        {
            get { return m_RightBCRStart; }
            set { m_RightBCRStart = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double LeftBCRTarget
        {
            get { return m_LeftBCRTarget; }
            set { m_LeftBCRTarget = value; }
        }
        [DatabaseSettingAttribute(false)]
        public double RightBCRTarget
        {
            get { return m_RightBCRTarget; }
            set { m_RightBCRTarget = value; }
        }
        #endregion

        #region Constructors
        public DataItem_Link()
        {
        }
        public DataItem_Link(int id, bool useFlag, int fromNodeID, int toNodeID, LinkType type, string bcrMatch, enSteerDirection steerDirection, int steerChangeLeftBCR, int steerChangeRightBCR, double time, int weight,
            double distance, double velocity, double acceleration, double decceleration, double jerk,
            int ubslevel0 = 0, int ubslevel1 = 0, bool routeChangeCheck = false, double steerGuideLenghtFromNode = 0, double steerGuideLenghtToNode = 0, bool jcsAreaFlag = false, int fromDistance = 0, int toDistance = 0)
        {
            try
            {
                this.m_LinkID = id;
                this.m_UseFlag = useFlag;
                this.m_FromNodeID = fromNodeID;
                this.m_ToNodeID = toNodeID;
                this.m_Type = type;
                this.m_BCRMatchType = bcrMatch; // HJYOU 2023/03/20 Database BCRMatchType 추가
                this.m_SteerDirectionValue = steerDirection;
                this.m_SteerChangeLeftBCR = steerChangeLeftBCR;
                this.m_SteerChangeRightBCR = steerChangeRightBCR;
                this.m_Time = time;
                this.m_Weight = weight;

                this.m_Distance = distance;
                this.m_Velocity = velocity;
                this.m_Acceleration = acceleration;
                this.m_Deceleration = decceleration;
                this.m_Jerk = jerk;

                this.m_UBSLevel0 = ubslevel0;
                this.m_UBSLevel1 = ubslevel1;
                this.m_RouteChangeCheck = routeChangeCheck;
                this.m_SteerGuideLengthFromNode = steerGuideLenghtFromNode;
                this.m_SteerGuideLengthToNode = steerGuideLenghtToNode;
                this.m_JCSAreaFlag = jcsAreaFlag;
                this.m_FromExtensionDistance = fromDistance;
                this.m_ToExtensionDistance = toDistance;
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_Link source)
        {
            try
            {
                this.m_LinkID = source.LinkID;
                this.m_UseFlag = source.UseFlag;
                this.m_FromNodeID = source.FromNodeID;
                this.m_ToNodeID = source.ToNodeID;
                this.m_Type = source.Type;
                this.m_BCRMatchType = source.BCRMatchType;
                this.m_SteerDirectionValue = source.SteerDirectionValue;
                this.m_SteerChangeLeftBCR = source.SteerChangeLeftBCR;
                this.m_SteerChangeRightBCR = source.SteerChangeRightBCR;
                this.m_Time = source.Time;
                this.m_Weight = source.Weight;

                this.m_Distance = source.Distance;
                this.m_Velocity = source.Velocity;
                this.m_Acceleration = source.Acceleration;
                this.m_Deceleration = source.Deceleration;
                this.m_Jerk = source.Jerk;

                this.m_UBSLevel0 = source.UBSLevel0;
                this.m_UBSLevel1 = source.UBSLevel1;
                this.m_RouteChangeCheck = source.RouteChangeCheck;
                this.m_SteerGuideLengthFromNode = source.SteerGuideLengthFromNode;
                this.m_SteerGuideLengthToNode = source.SteerGuideLengthToNode;
                this.m_JCSAreaFlag = source.JCSAreaFlag;
                this.m_FromExtensionDistance = source.FromExtensionDistance;
                this.m_ToExtensionDistance = source.ToExtensionDistance;

                this.m_TorqueLimit = source.TorqueLimit;
                this.m_UBSOnlyUsingSensorNo = source.UBSOnlyUsingSensorNo;
                this.m_LeftBCRBegin = source.LeftBCRBegin;
                this.m_RightBCRBegin = source.RightBCRBegin;
                this.m_LeftBCREnd = source.LeftBCREnd;
                this.m_RightBCREnd = source.RightBCREnd;

                this.LeftBCRStart = source.LeftBCRStart; // Link 내에서 출발 위치가 필요...
                this.RightBCRStart = source.RightBCRStart;
                this.LeftBCRTarget = source.LeftBCRTarget; // Link 내에서 멈추는 위치가 필요...
                this.RightBCRTarget = source.RightBCRTarget;
            }
            catch
            {
            }
        }
        public DataItem_Link GetCopyOrNull()
        {
            try
            {
                return (DataItem_Link)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public bool CompareWith(DataItem_Link target, ref string msg)
        {
            try
            {
                bool result = true;

                result &= (this.m_LinkID == target.LinkID); if (LinkID != target.LinkID) msg += "LinkID,";
                result &= (this.m_UseFlag == target.UseFlag); if (UseFlag != target.UseFlag) msg += "LiUseFlagnkID,";
                result &= (this.m_FromNodeID == target.FromNodeID); if (FromNodeID != target.FromNodeID) msg += "FromNodeID,";
                result &= (this.m_ToNodeID == target.ToNodeID); if (ToNodeID != target.ToNodeID) msg += "ToNodeID,";
                result &= (this.m_Type == target.Type); if (Type != target.Type) msg += "Type,";
                result &= (this.m_BCRMatchType == target.BCRMatchType); if (BCRMatchType != target.BCRMatchType) msg += "BCRMatchType,";
                result &= (this.m_SteerDirectionValue == target.SteerDirectionValue); if (SteerDirectionValue != target.SteerDirectionValue) msg += "SteerDirectionValue,";
                result &= (this.m_SteerChangeLeftBCR == target.SteerChangeLeftBCR); if (SteerChangeLeftBCR != target.SteerChangeLeftBCR) msg += "SteerChangeLeftBCR,";
                result &= (this.m_SteerChangeRightBCR == target.SteerChangeRightBCR); if (SteerChangeRightBCR != target.SteerChangeRightBCR) msg += "SteerChangeRightBCR,";
                result &= (this.m_Time == target.Time); if (Time != target.Time) msg += "Time,";
                result &= (this.m_Weight == target.Weight); if (Weight != target.Weight) msg += "Weight,";

                result &= (this.m_Distance == target.Distance); if (Distance != target.Distance) msg += "Distance,";
                result &= (this.m_Velocity == target.Velocity); if (Velocity != target.Velocity) msg += "Velocity,";
                result &= (this.m_Acceleration == target.Acceleration); if (Acceleration != target.Acceleration) msg += "Acceleration,";
                result &= (this.m_Deceleration == target.Deceleration); if (Deceleration != target.Deceleration) msg += "Deceleration,";
                result &= (this.m_Jerk == target.Jerk); if (Jerk != target.Jerk) msg += "Jerk,";

                result &= (this.m_UBSLevel0 == target.UBSLevel0); if (UBSLevel0 != target.UBSLevel0) msg += "UBSLevel0,";
                result &= (this.m_UBSLevel1 == target.UBSLevel1); if (UBSLevel1 != target.UBSLevel1) msg += "UBSLevel1,";
                result &= (this.m_RouteChangeCheck == target.RouteChangeCheck); if (RouteChangeCheck != target.RouteChangeCheck) msg += "RouteChangeCheck,";
                result &= (this.m_SteerGuideLengthFromNode == target.SteerGuideLengthFromNode); if (SteerGuideLengthFromNode != target.SteerGuideLengthFromNode) msg += "SteerGuideLengthFromNode,";
                result &= (this.m_SteerGuideLengthToNode == target.SteerGuideLengthToNode); if (SteerGuideLengthToNode != target.SteerGuideLengthToNode) msg += "SteerGuideLengthToNode,";
                result &= (this.m_JCSAreaFlag == target.JCSAreaFlag); if (JCSAreaFlag != target.JCSAreaFlag) msg += "JCSAreaFlag,";
                result &= (this.m_FromExtensionDistance == target.FromExtensionDistance); if (FromExtensionDistance != target.FromExtensionDistance) msg += "FromExtensionDistance,";
                result &= (this.m_ToExtensionDistance == target.ToExtensionDistance); if (ToExtensionDistance != target.ToExtensionDistance) msg += "ToExtensionDistance,";

                result &= (this.m_TorqueLimit == target.TorqueLimit); if (TorqueLimit != target.TorqueLimit) msg += "TorqueLimit,";
                result &= (this.m_UBSOnlyUsingSensorNo == target.UBSOnlyUsingSensorNo); if (UBSOnlyUsingSensorNo != target.UBSOnlyUsingSensorNo) msg += "UBSOnlyUsingSensorNo,";
                result &= (this.m_LeftBCRBegin == target.LeftBCRBegin); if (LeftBCRBegin != target.LeftBCRBegin) msg += "LeftBCRBegin,";
                result &= (this.m_RightBCRBegin == target.RightBCRBegin); if (RightBCRBegin != target.RightBCRBegin) msg += "RightBCRBegin,";
                result &= (this.m_LeftBCREnd == target.LeftBCREnd); if (LeftBCREnd != target.LeftBCREnd) msg += "LeftBCREnd,";
                result &= (this.m_RightBCREnd == target.RightBCREnd); if (RightBCREnd != target.RightBCREnd) msg += "RightBCREnd,";
                return result;
            }
            catch
            {
                return false;
            }
        }
        public bool IsCorner()
        {
            bool corner = false;
            corner |= this.Type == LinkType.LeftCurve ? true : false;
            corner |= this.Type == LinkType.RightCurve ? true : false;
            corner |= this.Type == LinkType.LeftBranch ? true : false;
            corner |= this.Type == LinkType.RightBranch ? true : false;
            corner |= this.Type == LinkType.LeftSBranch ? true : false;
            corner |= this.Type == LinkType.RightSBranch ? true : false;
            corner |= this.Type == LinkType.LeftJunction ? true : false;
            corner |= this.Type == LinkType.RightJunction ? true : false;
            corner |= this.Type == LinkType.LeftSJunction ? true : false;
            corner |= this.Type == LinkType.RightSJunction ? true : false;
            corner |= this.Type == LinkType.LeftCompositedSCurveBranch ? true : false;
            corner |= this.Type == LinkType.RightCompositedSCurveBranch ? true : false;
            corner |= this.Type == LinkType.LeftCompositedSCurveJunction ? true : false;
            corner |= this.Type == LinkType.RightCompositedSCurveJunction ? true : false;
            return corner;
        }
        public bool IsBranchJunctionStraight()
        {
            bool bjStraight = false;
            bjStraight |= this.Type == LinkType.LeftBranchStraight;
            bjStraight |= this.Type == LinkType.RightBranchStraight;
            bjStraight |= this.Type == LinkType.LeftJunctionStraight;
            bjStraight |= this.Type == LinkType.RightJunctionStraight;
            return bjStraight;
        }
        public bool IsSameType(DataItem_Link prev)
        {
            bool rv = false;
            rv |= prev.Type == this.Type ? true : false;

            bool prev_straight = false;
            prev_straight |= prev.Type == LinkType.Straight ? true : false;
            prev_straight |= prev.Type == LinkType.LeftBranchStraight ? true : false;
            prev_straight |= prev.Type == LinkType.RightBranchStraight ? true : false;
            prev_straight |= prev.Type == LinkType.LeftJunctionStraight ? true : false;
            prev_straight |= prev.Type == LinkType.RightJunctionStraight ? true : false;
            prev_straight |= prev.Type == LinkType.JunctionStraight ? true : false;
            prev_straight |= prev.Type == LinkType.SideStraight ? true : false;
            prev_straight |= prev.Type == LinkType.SideLeftJunctionStraight ? true : false;
            prev_straight |= prev.Type == LinkType.SideRightJunctionStraight ? true : false;
            bool cur_straight = false;
            cur_straight |= this.Type == LinkType.Straight ? true : false;
            cur_straight |= this.Type == LinkType.LeftBranchStraight ? true : false;
            cur_straight |= this.Type == LinkType.RightBranchStraight ? true : false;
            cur_straight |= this.Type == LinkType.LeftJunctionStraight ? true : false;
            cur_straight |= this.Type == LinkType.RightJunctionStraight ? true : false;
            cur_straight |= this.Type == LinkType.JunctionStraight ? true : false;
            cur_straight |= this.Type == LinkType.SideStraight ? true : false;
            cur_straight |= this.Type == LinkType.SideLeftJunctionStraight ? true : false;
            cur_straight |= this.Type == LinkType.SideRightJunctionStraight ? true : false;
            rv |= prev_straight && cur_straight ? true : false;

            bool prev_left_curve = false;
            prev_left_curve |= prev.Type == LinkType.LeftCurve ? true : false;
            prev_left_curve |= prev.Type == LinkType.LeftBranch ? true : false;
            prev_left_curve |= prev.Type == LinkType.LeftSBranch ? true : false;
            prev_left_curve |= prev.Type == LinkType.LeftJunction ? true : false;
            prev_left_curve |= prev.Type == LinkType.LeftSJunction ? true : false;
            bool cur_left_curve = false;
            cur_left_curve |= this.Type == LinkType.LeftCurve ? true : false;
            cur_left_curve |= this.Type == LinkType.LeftBranch ? true : false;
            cur_left_curve |= this.Type == LinkType.LeftSBranch ? true : false;
            cur_left_curve |= this.Type == LinkType.LeftJunction ? true : false;
            cur_left_curve |= this.Type == LinkType.LeftSJunction ? true : false;
            rv |= prev_left_curve && cur_left_curve ? true : false;

            bool prev_right_curve = false;
            prev_right_curve |= prev.Type == LinkType.RightCurve ? true : false;
            prev_right_curve |= prev.Type == LinkType.RightBranch ? true : false;
            prev_right_curve |= prev.Type == LinkType.RightSBranch ? true : false;
            prev_right_curve |= prev.Type == LinkType.RightJunction ? true : false;
            prev_right_curve |= prev.Type == LinkType.RightSJunction ? true : false;
            bool cur_right_curve = false;
            cur_right_curve |= this.Type == LinkType.RightCurve ? true : false;
            cur_right_curve |= this.Type == LinkType.RightBranch ? true : false;
            cur_right_curve |= this.Type == LinkType.RightSBranch ? true : false;
            cur_right_curve |= this.Type == LinkType.RightJunction ? true : false;
            cur_right_curve |= this.Type == LinkType.RightSJunction ? true : false;
            rv |= prev_right_curve && cur_right_curve ? true : false;

            return rv;
        }
        #endregion

        #region override
        public override string ToString()
        {
            return string.Format("from:{0}~to:{1},{2},{3},{4},{5},{6}", m_FromNodeID, m_ToNodeID, m_LinkID, m_Type, m_Distance, m_Velocity, m_BCRMatchType);
        }
        #endregion
    }
}
