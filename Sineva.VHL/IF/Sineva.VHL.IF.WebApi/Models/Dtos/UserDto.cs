using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models.Dtos
{
    public class UserDto : Page
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public int Level { get; set; } = -1;
        public string ClientIp { get; set; }
    }
}