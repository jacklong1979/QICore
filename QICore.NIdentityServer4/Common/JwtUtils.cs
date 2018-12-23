using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QICore.NIdentityServer4.Options;

namespace QICore.NIdentityServer4.Common
{
    public class JwtUtils: Controller
    {
        /// <summary>
        /// 生成一个新的 Token
        /// </summary>
        /// <param name="option">身份配置信息，生成Token所需要的信息</param>
        /// <param name="claims">用户信息实体</param>      
        /// <returns></returns>
        private JsonResult CreateToken(IdentityOption option, Claim[] claims)
        {
            #region Claim
           // var claims = new Claim[]
           //{

           //     new Claim(JwtRegisteredClaimNames.Aud, _options.Audience),
           //     new Claim(JwtRegisteredClaimNames.Sid, username),
           //     new Claim(JwtRegisteredClaimNames.Sub, username),
           //     new Claim(JwtRegisteredClaimNames.Exp, _options.ExpiresIn.ToString()),
           //     new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString()),
           //     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
           //     new Claim(JwtRegisteredClaimNames.Iat, _options.Issued.ToString(), ClaimValueTypes.Integer64),//发行时间
           //     //用户名
           //     new Claim("UserName","中国人民"),
           //     //角色
           //     new Claim("Role","我是角色"),
           //     new Claim("Country","中国"),
           //     new Claim("Expired",_options.ExpiresIn.ToString()),
           //     new Claim("Mobile","13556891160")
           //};
            #endregion
            var jwt = new JwtSecurityToken(
               issuer: option.Issuer, // 发行者(颁发机构)
               audience: option.Audience,//订阅人 ， 令牌的观众(颁发给谁)
               claims: claims,
               notBefore: DateTime.UtcNow,
               expires: option.ExpiresTime,
               signingCredentials: option.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new
            {
                access_token = encodedJwt,
                token_type = option.TokenType,
                expires_in = option.ExpiresIn, //过期时间(秒)
                expires_time = option.ExpiresTime.ToString("yyyy-MM-dd HH:mm:ss"), //过期时间（日期）
                claims = claims
            };
            return Json(token);
        }
    }
}
