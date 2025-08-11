using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    /// <summary>
    /// PortList查询条件
    /// </summary>
    public class PortListDto : Page
    {
        /// <summary>
        /// PortID
        /// </summary>
        public int PortID { get; set; } = -1;
        /// <summary>
        /// LinkID
        /// </summary>
        public int LinkID { get; set; } = -1;
        /// <summary>
        /// NodeID
        /// </summary>
        public int NodeID { get; set; } = -1;
        /// <summary>
        /// PortType
        /// </summary>
        public int PortType { get; set; } = -1;

        /// <summary>
        /// PIOUse
        /// </summary>
        public int PIOUse { get; set; } = -1;

        /// <summary>
        /// PBSUse
        /// </summary>
        public int PBSUse { get; set; } = -1;
    }

    public class PortDto
    {
        public bool PIOUsed { get; set; }
        public int State { get; set; }
        public bool PortProhibition { get; set; }
        public bool OffsetUsed { get; set; }
        public int PortType { get; set; }
        public int PortID { get; set; }
        public int LinkID { get; set; }
        public int NodeID { get; set; }
        public int PIOID { get; set; }
        public int PIOCH { get; set; }
        public int PIOCS { get; set; }
        public double SlidePosition { get; set; }
        public double RotatePosition { get; set; }
        public double BeforeHoistPosition { get; set; }
        public double HoistPosition { get; set; }
        public double UnloadSlidePosition { get; set; }
        public double UnloadRotatePosition { get; set; }
        public double BeforeUnloadHoistPosition { get; set; }
        public double UnloadHoistPosition { get; set; }
        public double BarcodeLeft { get; set; }
        public double BarcodeRight { get; set; }
        public int PBSSelectNo { get; set; }
        public bool PBSUsed { get; set; }
        public double DriveLeftOffset { get; set; }
        public double DriveRightOffset { get; set; }
        public double SlideOffset { get; set; }
        public double HoistOffset { get; set; }
        public double RotateOffset { get; set; }
        public int ProfileExistPosition { get; set; }
    }
    public class TeachingDto
    {
        public bool IsLoadPosition { get; set; }
        public int PortType { get; set; }
        public int PortID { get; set; }
        public double SlidePosition { get; set; }
        public double RotatePosition { get; set; }
        public double BeforeHoistPosition { get; set; }
        public double HoistPosition { get; set; }
        public double UnloadSlidePosition { get; set; }
        public double UnloadRotatePosition { get; set; }
        public double BeforeUnloadHoistPosition { get; set; }
        public double UnloadHoistPosition { get; set; }
        public double BarcodeLeft { get; set; }
        public double BarcodeRight { get; set; }
    }
}