using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models
{
    /// <summary>
    /// 分页
    /// </summary>
    public class Page
    {
        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 页数
        /// </summary>
        public int PageNo { get; set; } = 1;
    }
}