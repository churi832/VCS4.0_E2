using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.ReturnModels
{
    public class AlarmHistoryRtn : ReturnPage
    {
        public List<AlarmHistoryData> AlarmHistoryDatas { get; set; }
    }
    public class AlarmHistoryData
    {
        public DateTime OccurredTime { get; set; }

        public DateTime? ClearedTime { get; set; }

        public int ID { get; set; }

        public string LocationID { get; set; }

        public int Code { get; set; }
        public int Level { get; set; }

        public string Unit { get; set; }
        public string Description { get; set; }

        public string Comment { get; set; }
    }
}