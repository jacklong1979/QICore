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

        public RsaSecurityKey RsaSecurityKey { get; set; }      
        /// <summary>
        /// 表示用于生成数字签名的加密密钥和安全算法。
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
        /// <summary>
        /// 表示客户端的ID，必选项
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 发行者(颁发机构)
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 令牌的观众(颁发给谁)
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 到期时间(秒)
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
