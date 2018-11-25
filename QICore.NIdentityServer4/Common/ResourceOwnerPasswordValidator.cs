using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.NIdentityServer4.Common
{
    #region 用户Resource Owner Password模式需要对账号密码进行验证
    /// <summary>
    /// 用户Resource Owner Password模式需要对账号密码进行验证
    /// 使用ResourceOwnerPasswordValidator的作用，就是自定义用户登录的用户名密码判断，而不是使用 IdentityServer4 的TestUser。，如果有的TestUser，则ResourceOwnerPasswordValidator不起作用
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            Console.WriteLine($"Password模式 用户名：{context.UserName},密码：{context.Password }");
            if (context.UserName == "admin" && context.Password == "123")
            {
                //验证成功
                //使用subject可用于在资源服务器区分用户身份等等
                //获取：资源服务器通过User.Claims.Where(l => l.Type == "sub").FirstOrDefault();获取
                context.Result = new GrantValidationResult(subject: "admin", authenticationMethod: "custom");
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid username and password");
            }
            return Task.FromResult(0);
        }
    }
    #endregion
}
