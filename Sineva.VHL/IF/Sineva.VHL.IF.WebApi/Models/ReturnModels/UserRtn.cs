using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.ReturnModels
{
    public class UserRtn : ReturnPage
    {
        public List<User> UserListDatas { get; set; }
    }
    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        ///// <summary>
        ///// 密码
        ///// </summary>
        //public string Password { get; set; }
        /// <summary>
        /// 权限等级
        /// </summary>
        public int Level { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }

    }
}