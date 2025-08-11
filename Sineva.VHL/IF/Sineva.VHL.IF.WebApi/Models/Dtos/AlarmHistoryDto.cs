using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    public class AlarmHistoryDto : Page
    {
        public DateTime? OccurredTimeStart { get; set; }

        public DateTime? OccurredTimeEnd { get; set; }

        public int ErrorID { get; set; } = -1;

        public int ErrorLevel { get; set; } = -1;

        public string Unit { get; set; }
        public string Description { get; set; }

        public string Comment { get; set; }
    }
}