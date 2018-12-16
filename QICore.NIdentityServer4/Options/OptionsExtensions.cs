using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QICore.NIdentityServer4.Options
{
    /// <summary>
    /// 配置选择项扩展(所有的配置都应在这个方法中处理)
    /// </summary>
    public static partial class OptionsExtensions
    {
        /// <summary>
        /// 增加配置选项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <remarks>所有的配置都应在这个方法中处理</remarks>
        public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // 添加服务设置实例配置
            services.Configure<IdentityOption>(configuration.GetSection("IdentityOption"));
            var symmetricKeyAsBase64 = configuration["IdentityOption:Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            IdentityOption identityOption = new IdentityOption
            {
                Secret = configuration["IdentityOption:Secret"], //密钥
                Issuer = configuration["IdentityOption:Issuer"], //发行者
                Audience = configuration["IdentityOption:Audience"], //令牌的观众
                TokenType = configuration["IdentityOption:TokenType"], //表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
                Scope = configuration["IdentityOption:Scope"], //表示权限范围，如果与客户端申请的范围一致，此项可省略
                Subject = configuration["IdentityOption:Subject"], //主题
                ExpiresIn = Convert.ToInt32(configuration["IdentityOption:ExpiresIn"]), //表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
                ClientId = configuration["IdentityOption:ClientId"], //表示客户端的ID，必选项
                ResponseType = configuration["IdentityOption:ResponseType"], //表示授权类型，必选项，此处的值固定为"code"
                RedirectUri = configuration["IdentityOption:RedirectUri"],
                State = configuration["IdentityOption:State"], //表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值。
                SigningCredentials = signingCredentials
            };
            services.AddSingleton(identityOption);
        }
    }
}
