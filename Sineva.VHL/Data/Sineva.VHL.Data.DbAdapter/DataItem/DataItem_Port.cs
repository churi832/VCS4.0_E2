using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;
using System.ComponentModel;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_Port : DataItem
    {
        #region Fields
        private bool m_PIOUsed;
        private int m_State;                  //PortStatus
        private bool m_PortProhibition;
        private bool m_OffsetUsed;
        private PortType m_PortType;               //PortType
        private int m_PortID;
        private int m_LinkID;
        private int m_NodeID;
        private double m_SlidePosition;
        private double m_RotatePosition;
        private double m_HoistPosition;
        private double m_BeforeHoistPosition;
        private double m_UnloadSlidePosition;
        private double m_UnloadRotatePosition;
        private double m_UnloadHoistPosition;
        private double m_BeforeUnloadHoistPosition;
        private int m_PIOCS = 0;
        private int m_PIOID = 0;
        private int m_PIOCH = 0;
        private double m_BarcodeLeft = 0;
        private double m_BarcodeRight = 0;
        private int m_PBSSelectNo = 0;
        private bool m_PBSUsed = false;
        private double m_DriveLeftOffset = 0.0f;
        private double m_DriveRightOffset = 0.0f;
        private double m_SlideOffset = 0.0f;
        private double m_HoistOffset = 0.0f;
        private double m_RotateOffset = 0.0f;
        private enProfileExistPosition m_ProfileExistPosition = 0;
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true), DisplayName("A.PIOUsed")]
        public bool PIOUsed
        {
            get { return m_PIOUsed; }
            set { m_PIOUsed = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("B.State")]
        public int State
        {
            get { return m_State; }
            set { m_State = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("C.PortProhibition")]
        public bool PortProhibition
        {
            get { return m_PortProhibition; }
            set { m_PortProhibition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("D.OffsetUsed")]
        public bool OffsetUsed
        {
            get { return m_OffsetUsed; }
            set { m_OffsetUsed = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("E.PortType")]
        public PortType PortType
        {
            get { return m_PortType; }
            set { m_PortType = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("F.PortID")]
        public int PortID
        {
            get { return m_PortID; }
            set { m_PortID = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("G.LinkID")]
        public int LinkID
        {
            get { return m_LinkID; }
            set { m_LinkID = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("H.NodeID")]
        public int NodeID
        {
            get { return m_NodeID; }
            set { m_NodeID = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("I.PIOID")]
        public int PIOID
        {
            get { return m_PIOID; }
            set { m_PIOID = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("J.PIOCH")]
        public int PIOCH
        {
            get { return m_PIOCH; }
            set { m_PIOCH = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("K.PIOCS")]
        public int PIOCS
        {
            get { return m_PIOCS; }
            set { m_PIOCS = value; }
        }        
        [DatabaseSettingAttribute(true), DisplayName("L.SlidePosition")]
        public double SlidePosition
        {
            get { return m_SlidePosition; }
            set { m_SlidePosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("M.RotatePosition")]
        public double RotatePosition
        {
            get { return m_RotatePosition; }
            set { m_RotatePosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("N.BeforeHoistPosition")]
        public double BeforeHoistPosition
        {
            get { return m_BeforeHoistPosition; }
            set { m_BeforeHoistPosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("N.HoistPosition")]
        public double HoistPosition
        {
            get { return m_HoistPosition; }
            set { m_HoistPosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("O.UnloadSlidePosition")]
        public double UnloadSlidePosition
        {
            get { return m_UnloadSlidePosition; }
            set { m_UnloadSlidePosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("P.UnloadRotatePosition")]
        public double UnloadRotatePosition
        {
            get { return m_UnloadRotatePosition; }
            set { m_UnloadRotatePosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("Q.BeforeUnloadHoistPosition")]
        public double BeforeUnloadHoistPosition
        {
            get { return m_BeforeUnloadHoistPosition; }
            set { m_BeforeUnloadHoistPosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("Q.UnloadHoistPosition")]
        public double UnloadHoistPosition
        {
            get { return m_UnloadHoistPosition; }
            set { m_UnloadHoistPosition = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("R.BarcodeLeft")]
        public double BarcodeLeft
        {
            get { return m_BarcodeLeft; }
            set { m_BarcodeLeft = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("S.BarcodeRight")]
        public double BarcodeRight
        {
            get { return m_BarcodeRight; }
            set { m_BarcodeRight = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("T.PBSSelectNo")]
        public int PBSSelectNo
        {
            get { return m_PBSSelectNo; }
            set { m_PBSSelectNo = value; }

        }
        [DatabaseSettingAttribute(true), DisplayName("U.PBSUsed")]
        public bool PBSUsed
        {
            get { return m_PBSUsed; }
            set { m_PBSUsed = value; }

        }
        [DatabaseSettingAttribute(true), DisplayName("X.DriveLeftOffset")]
        public double DriveLeftOffset
        {
            get { return m_DriveLeftOffset; }
            set { m_DriveLeftOffset = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("X.DriveRightOffset")]
        public double DriveRightOffset
        {
            get { return m_DriveRightOffset; }
            set { m_DriveRightOffset = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("X.SlideOffset")]
        public double SlideOffset
        {
            get { return m_SlideOffset; }
            set { m_SlideOffset = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("X.HoistOffset")]
        public double HoistOffset
        {
            get { return m_HoistOffset; }
            set { m_HoistOffset = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("X.RotateOffset")]
        public double RotateOffset
        {
            get { return m_RotateOffset; }
            set { m_RotateOffset = value; }
        }
        [DatabaseSettingAttribute(true), DisplayName("V.PBSUsed")]
        public enProfileExistPosition ProfileExistPosition
        {
            get { return m_ProfileExistPosition; }
            set { m_ProfileExistPosition = value; }
        }
        
        #endregion

        #region Constructor
        public DataItem_Port()
        {

        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_Port source)
        {
            try
            {
                this.m_PIOUsed = source.PIOUsed;
                this.m_State = source.State;
                this.m_PortProhibition = source.PortProhibition;
                this.m_OffsetUsed = source.OffsetUsed;
                this.m_PortType = source.PortType;
                this.m_PortID = source.PortID;
                this.m_LinkID = source.LinkID;
                this.m_NodeID = source.NodeID;
                this.m_SlidePosition = source.SlidePosition;
                this.m_RotatePosition = source.RotatePosition;
                this.m_HoistPosition = source.HoistPosition;
                this.m_BeforeHoistPosition = source.BeforeHoistPosition;
                this.m_UnloadSlidePosition = source.UnloadSlidePosition;
                this.m_UnloadRotatePosition = source.UnloadRotatePosition;
                this.m_UnloadHoistPosition = source.UnloadHoistPosition;
                this.m_BeforeUnloadHoistPosition = source.BeforeUnloadHoistPosition;
                this.m_PIOID = source.PIOID;
                this.m_PIOCH = source.PIOCH;
                this.m_PIOCS = source.PIOCS;
                this.m_BarcodeLeft = source.BarcodeLeft;
                this.m_BarcodeRight = source.BarcodeRight;
                this.m_PBSSelectNo = source.PBSSelectNo;
                this.m_PBSUsed = source.PBSUsed;
                this.m_ProfileExistPosition = source.ProfileExistPosition;
            }
            catch
            {
            }
        }
        public void SetCopyTeachingData(DataItem_Port source)
        {
            try
            {
                this.m_PIOUsed = source.PIOUsed;
                //this.m_State = source.State;
                this.m_PortProhibition = source.PortProhibition;
                this.m_OffsetUsed = source.OffsetUsed;
                this.m_PortType = source.PortType;
                this.m_PortID = source.PortID;
                this.m_LinkID = source.LinkID;
                this.m_NodeID = source.NodeID;
                this.m_SlidePosition = source.SlidePosition;
                this.m_RotatePosition = source.RotatePosition;
                this.m_HoistPosition = source.HoistPosition;
                this.m_BeforeHoistPosition = source.BeforeHoistPosition;
                this.m_UnloadSlidePosition = source.UnloadSlidePosition;
                this.m_UnloadRotatePosition = source.UnloadRotatePosition;
                this.m_UnloadHoistPosition = source.UnloadHoistPosition;
                this.m_BeforeUnloadHoistPosition = source.BeforeUnloadHoistPosition;
                this.m_PIOID = source.PIOID;
                this.m_PIOCH = source.PIOCH;
                this.m_PIOCS = source.PIOCS;
                this.m_BarcodeLeft = source.BarcodeLeft;
                this.m_BarcodeRight = source.BarcodeRight;
                this.m_PBSSelectNo = source.PBSSelectNo;
                this.m_PBSUsed = source.PBSUsed;
                this.m_ProfileExistPosition = source.ProfileExistPosition;
            }
            catch
            {
            }
        }
        public DataItem_Port GetCopyOrNull()
        {
            try
            {
                return (DataItem_Port)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }

        public bool CompareWith(DataItem_Port target)
        {
            try
            {
                bool result = true;
                result &= (this.m_PIOUsed == target.PIOUsed);
                result &= (this.m_PIOID == target.PIOID);
                result &= (this.m_PIOCH == target.PIOCH);
                result &= (this.m_PIOCS == target.PIOCS);
                result &= (this.m_State == target.State);
                result &= (this.m_PortProhibition == target.PortProhibition);
                result &= (this.m_OffsetUsed == target.OffsetUsed);
                result &= (this.m_PortType == target.PortType);
                result &= (this.m_PortID == target.PortID);
                result &= (this.m_LinkID == target.LinkID);
                result &= (this.m_NodeID == target.NodeID);
                result &= (this.m_SlidePosition == target.SlidePosition);
                result &= (this.m_RotatePosition == target.RotatePosition);
                result &= (this.m_HoistPosition == target.HoistPosition);
                result &= (this.m_BeforeHoistPosition == target.BeforeHoistPosition);
                result &= (this.m_UnloadSlidePosition == target.UnloadSlidePosition);
                result &= (this.m_UnloadRotatePosition == target.UnloadRotatePosition);
                result &= (this.m_UnloadHoistPosition == target.UnloadHoistPosition);
                result &= (this.m_BeforeUnloadHoistPosition == target.BeforeUnloadHoistPosition);
                result &= (this.m_BarcodeLeft == target.BarcodeLeft);
                result &= (this.m_BarcodeRight == target.BarcodeRight);
                result &= (this.m_PBSSelectNo == target.PBSSelectNo);
                result &= (this.m_PBSUsed == target.PBSUsed);
                result &= (this.m_ProfileExistPosition == target.ProfileExistPosition);
                return result;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region override
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", m_PortID, m_LinkID, m_NodeID);
        }
        #endregion
    }
}
