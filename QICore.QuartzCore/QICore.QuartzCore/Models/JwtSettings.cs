using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.QuartzCore.Models
{
    public class JwtSettings
    {
        //token是谁颁发的
        public string Issuer { get; set; }
        //token可以给哪些客户端使用
        public string Audience { get; set; }
        //加密的key
        public string SecretKey { get; set; }
        //过期时间（秒）
        public int ExpiresIn { get; set; }
        /// <summary>
        /// 到期时间（日期）
        /// </summary>
        public DateTime ExpiresTime { get { return DateTime.Now.AddSeconds(ExpiresIn); } }
    }
}
