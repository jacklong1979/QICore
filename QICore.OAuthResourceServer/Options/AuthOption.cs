using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.OAuthResourceServer.Options
{
    /*
    用户信息（由一组Cliams构成以及其他辅助的Cliams）的JWT格式的数据结构。ID Token的主要构成部分如下（使用OAuth2流程的OIDC）。
    iss = Issuer Identifier：必须。提供认证信息者的唯一标识。一般是一个https的url（不包含querystring和fragment部分）。
    sub = Subject Identifier：必须。iss提供的EU的标识，在iss范围内唯一。它会被RP用来标识唯一的用户。最长为255个ASCII个字符。
    aud = Audience(s)：必须。标识ID Token的受众。必须包含OAuth2的client_id。
    exp = Expiration time：必须。过期时间，超过此时间的ID Token会作废不再被验证通过。
    iat = Issued At Time：必须。JWT的构建的时间。
    auth_time = AuthenticationTime：EU完成认证的时间。如果RP发送AuthN请求的时候携带max_age的参数，则此Claim是必须的。
    nonce：RP发送请求的时候提供的随机字符串，用来减缓重放攻击，也可以来关联ID Token和RP本身的Session信息。
    acr = Authentication Context Class Reference：可选。表示一个认证上下文引用值，可以用来标识认证上下文类。
    amr = Authentication Methods References：可选。表示一组认证方法。
    azp = Authorized party：可选。结合aud使用。只有在被认证的一方和受众（aud）不一致时才使用此值，一般情况下很少使用。
    */
    /// <summary>
    /// 身份配置信息，生成Token所需要的信息
    /// </summary>
    public class AuthOption
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 发行者(颁发机构),token 的发布地址
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// 令牌的观众(颁发给谁)，token 服务器的授权范围 Scope
        /// </summary>
        public string Audience { get; set; }
             
    }
  
}
