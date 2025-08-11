using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    public class FrontDetectFilterDto : Page
    {
        /// <summary>
        /// LinkID
        /// </summary>
        public int LinkID { get; set; } = -1;
        /// <summary>
        /// SensorLevel
        /// </summary>
        public int SensorLevel { get; set; } = 0;
        /// <summary>
        /// UseFlag
        /// </summary>
        public int UseFlag { get; set; } = -1;
    }
    public class FrontDto
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