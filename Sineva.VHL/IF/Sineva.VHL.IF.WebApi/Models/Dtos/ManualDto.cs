using Sineva.VHL.Library.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    public class ManualDto
    {
        public int OperateMode { get; set; }
    }
    public class RunModeDto
    {
        public int RunMode { get; set; }
    }
    public class ManualActionDto
    {
        public int WebDevice { get; set; }
        public int WebAction { get; set; }
        public double WebVelocity { get; set; }
        public double WebDistance { get; set; }
    }
    public class OffsetUpdateDto
    {
        public int PortID { get; set; }
        public int WebDevice { get; set; }
        public int WebAction { get; set; }
        public double DriveLeftOffset { get; set; }
        public double DriveRightOffset { get; set; }
        public double HoistOffset { get; set; }
        public double SlideOffset { get; set; }
        public double RotateOffset { get; set; }
    }

    public class CommandDto
    {
        public bool IsValid { get; set; }

        public int CommandType { get; set; }
        public string CommandID { get; set; }
        public string CassetteID { get; set; }
        public int SourceID { get; set; }
        public int DestinationID { get; set; }
        public int TypeOfDestination { get; set; }
        public double TargetNodeToDistance { get; set; }
        public int TotalCount { get; set; }
        public int WaitTime { get; set; }
    }
    public class CarrierStateDto
    {
        public bool Install { get; set; }
    }
}