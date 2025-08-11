using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    /// <summary>
    /// NodeList查询条件
    /// </summary>
    public class NodeListDto : Page
    {
        /// <summary>
        /// NodeID
        /// </summary>
        public int NodeID { get; set; } = -1;
        /// <summary>
        /// 是否启用
        /// </summary>
        public int UseFlag { get; set; } = -1;
        /// <summary>
        /// NodeType
        /// </summary>
        public int NodeType { get; set; } = -1;
    }

    /// <summary>
    /// Node查询条件
    /// </summary>
    public class NodeDto
    {
        public int NodeID { get; set; }

        public bool UseFlag { get; set; }

        public double LocationValue1 { get; set; }

        public double LocationValue2 { get; set; }

        public int Type { get; set; }

        public int CPSZoneID { get; set; }

        public int UBSLevel { get; set; }

        public int UBSCheckSensor { get; set; }

        public int JCSCheck { get; set; }
    }
}