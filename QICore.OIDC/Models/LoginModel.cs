using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.OIDC.Models
{
    public class LoginModel
    {
        //用户名
        [Required]
        public string User { get; set; }
        //密码
        [Required]
        public string Password { get; set; }
    }
}
