using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    public class VelocityLimitDto : Page
    {
        public int LinkID { get; set; } = -1;
        public int ToLinkID { get; set; } = -1;
        public int MaxVelocity { get; set; }
    }
    public class VelocityDto
    {
        public int ID { get; set; }
        public int LinkId { get; set; }
        public int ToLinkId { get; set; }
        public int MaxVelocity { get; set; }
        public double LeftBcrStart { get; set; }
        public double LeftBcrEnd { get; set; }
        public double RightBcrStart { get; set; }
        public double RightBcrEnd { get; set; }
    }
}