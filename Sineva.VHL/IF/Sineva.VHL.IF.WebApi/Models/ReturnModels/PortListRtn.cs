using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.ReturnModels
{
    public class PortListRtn : ReturnPage
    {
        public List<PortListData> PortListDatas { get; set; }
    }
    public class PortListData
    {
        public int PortID { get; set; }

        public int LinkID { get; set; }
        public int NodeID { get; set; }

        public int PortType { get; set; }

        public double BeforeHoistPosition { get; set; }
        public double HoistPosition { get; set; }
        public double SlidePosition { get; set; }
        public double RotatePosition { get; set; }

        public double BeforeUnloadHoistPosition { get; set; }

        public double UnloadHoistPosition { get; set; }

        public double UnloadSlidePosition { get; set; }

        public double UnloadRotatePosition { get; set; }

        public double BarcodeLeft { get; set; }

        public double BarcodeRight { get; set; }

        public int PIOID { get; set; }

        public int PIOCH { get; set; }
        public int PIOCS { get; set; }

        public bool PIOUsed { get; set; }

        public bool PortProhibition { get; set; }

        public bool OffsetUsed { get; set; }
        public bool PBSUsed { get; set; }
        public int PBSSelectNo { get; set; }
        public int ProfileExistPosition { get; set; }

    }
}