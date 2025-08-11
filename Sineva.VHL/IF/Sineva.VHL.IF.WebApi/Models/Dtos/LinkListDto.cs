using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    /// <summary>
    /// LinkList查询条件
    /// </summary>
    public class LinkListDto : Page
    {
        /// <summary>
        /// LinkID
        /// </summary>
        public int LinkID { get; set; } = -1;
        /// <summary>
        /// 是否启用
        /// </summary>
        public int UseFlag { get; set; } = -1;
        /// <summary>
        /// FromNode
        /// </summary>
        public int FromNode { get; set; } = -1;
        /// <summary>
        /// ToNode
        /// </summary>
        public int ToNode { get; set; } = -1;
        /// <summary>
        /// LinkType
        /// </summary>
        public int LinkType { get; set; } = -1;
    }
    /// <summary>
    /// Link添加、修改条件
    /// </summary>
    public class LinkDto
    {
        public int LinkID { get; set; }

        public bool UseFlag { get; set; }

        public int FromNodeID { get; set; }

        public int ToNodeID { get; set; }

        public int Type { get; set; }

        public int SteerDirectionValue { get; set; }

        public string BCRMatchType { get; set; }
        public int SteerChangeLeftBCR { get; set; }

        public int SteerChangeRightBCR { get; set; }

        public double Time { get; set; }

        public int Weight { get; set; }


        public double Distance { get; set; }

        public double Velocity { get; set; }

        public double Acceleration { get; set; }
        public double Deceleration { get; set; }

        public double Jerk { get; set; }
        public int UBSLevel0 { get; set; }

        public int UBSLevel1 { get; set; }

        public bool RouteChangeCheck { get; set; }

        public double SteerGuideLengthFromNode { get; set; }

        public double SteerGuideLengthToNode { get; set; }
        public bool JCSAreaFlag { get; set; }

        public int FromExtensionDistance { get; set; }
        public int ToExtensionDistance { get; set; }
    }
}