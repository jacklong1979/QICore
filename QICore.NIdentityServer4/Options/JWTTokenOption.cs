using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.NIdentityServer4.Options
{
   
    /// <summary>
    /// 身份配置信息，生成Token所需要的信息
    /// </summary>
    public class JWTTokenOption
    {
        public string Audience { get; set; }
        public RsaSecurityKey RsaSecurityKey { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public string Issuer { get; set; }
        public string ClientId { get; set; }
        public int ExpiresIn { get; set; }
    }
}
