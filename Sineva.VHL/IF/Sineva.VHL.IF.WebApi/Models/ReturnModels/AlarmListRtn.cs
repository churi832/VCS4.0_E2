using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.ReturnModels
{
    public class AlarmListRtn : ReturnPage
    {
        public List<AlarmListData> AlarmListDatas { get; set; }
    }
    public class AlarmListData
    {
        public int ID { get; set; }

        public int Code { get; set; }
        public int Level { get; set; }

        public string Unit { get; set; }
        public string Description { get; set; }

        public string Comment { get; set; }
    }
}