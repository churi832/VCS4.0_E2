using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.ReturnModels
{
    public class FrontDetectFilterRtn : ReturnPage
    {
        public List<FrontDetectListData> FrontDetectListDatas { get; set; }
    }
    public class FrontDetectListData
    {
        public int ID { get; set; }

        public int SensorLevel { get; set; }
        public int Area { get; set; }

        public int LinkId { get; set; }

        public bool UseFlag { get; set; }
        public int LeftBcrStart { get; set; }

        public int LeftBcrEnd { get; set; }

        public int RightBcrStart { get; set; }
        public int RightBcrEnd { get; set; }
    }
}