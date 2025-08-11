using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.ReturnModels
{
    public class NodeListRtn : ReturnPage
    {
        public List<NodeListData> NodeListDatas { get; set; }
    }
    public class NodeListData
    {
        public int NodeID { get; set; }

        public bool UseFlag { get; set; }
        public int LocationValue1 { get; set; }
        public int LocationValue2 { get; set; }
        public int NodeType { get; set; }
        public int UBSLevel { get; set; }
        public int UBSCheckSensor { get; set; }
        public int JCSCheck { get; set; }
    }
}