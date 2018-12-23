using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using QICore.NIdentityServer4.Common;
using QICore.NIdentityServer4.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QICore.NIdentityServer4.Extensions
{
    /// <summary>
    ///  注入服务扩展类
    /// </summary>
    public static class ServiceExtension
    {
       

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="Configuration">IConfiguration</param>
        public static void AddServiceSingleton(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<IdentityOption>(Configuration.GetSection("IdentityOption"));
            //var identityConfigurationSection = Configuration.GetSection("IdentityOption");
            // 添加服务设置实例配置
             var identity = Configuration.GetSection("IdentityOption");            
            #region 【读取配置】
            var symmetricKeyAsBase64 = Configuration["IdentityOption:Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            IdentityOption identityOption = new IdentityOption
            {
                Secret = Configuration["IdentityOption:Secret"], //密钥
                Issuer = Configuration["IdentityOption:Issuer"], //发行者
                Audience = Configuration["IdentityOption:Audience"], //令牌的观众
                TokenType = Configuration["IdentityOption:TokenType"], //表示令牌类型，该值大小写不敏感，必选项，可以是bearer类型或mac类型。
                Scope = Configuration["IdentityOption:Scope"], //表示权限范围，如果与客户端申请的范围一致，此项可省略
                Subject = Configuration["IdentityOption:Subject"], //主题
                ExpiresIn = Convert.ToInt32(Configuration["IdentityOption:ExpiresIn"]), //表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
                ClientId = Configuration["IdentityOption:ClientId"], //表示客户端的ID，必选项
                ResponseType = Configuration["IdentityOption:ResponseType"], //表示授权类型，必选项，此处的值固定为"code"
                RedirectUri = Configuration["IdentityOption:RedirectUri"],
                State = Configuration["IdentityOption:State"], //表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值。
                SigningCredentials = signingCredentials
            };
            #endregion
            #region 【客户端模式】【密码模式】
            LicensingMode.SetResourceOwnerPasswordAndClientCredentials(services, identityOption);
            #endregion
            #region JWT JwtRegisteredClaimNames 方式 直接读取配置文件信息，初始化Token 需要验证的信息,如果不同在一台服务，则产生的Token与验证的Token的服务器验证信息与产生的信息要一致
            var jwtKeyAsBase64 = Configuration["JWTTokenOption:Secret"];
            var jwtKeyByteArray = Encoding.ASCII.GetBytes(jwtKeyAsBase64);
            var jwtSigningKey = new SymmetricSecurityKey(jwtKeyByteArray);
            var jwtSigningCredentials = new SigningCredentials(jwtSigningKey, SecurityAlgorithms.RsaSha256Signature);          
            JWTTokenOption jwtOption = new JWTTokenOption
            {
              
                Issuer = Configuration["JWTTokenOption:Issuer"], //发行者
                Audience = Configuration["JWTTokenOption:Audience"], //令牌的观众
                ExpiresIn = Convert.ToInt32(Configuration["JWTTokenOption:ExpiresIn"]), //表示过期时间，单位为秒。如果省略该参数，必须其他方式设置过期时间。
                ClientId = Configuration["JWTTokenOption:ClientId"], //表示客户端的ID，必选项               
                SigningCredentials = jwtSigningCredentials
            };
            // 从文件读取密钥
            string keyDir = PlatformServices.Default.Application.ApplicationBasePath;
            if (RSAUtils.TryGetKeyParameters(keyDir, false, out RSAParameters keyParams) == false)
            {
                keyParams = RSAUtils.GenerateAndSaveKey(keyDir);
            }
            jwtOption.RsaSecurityKey = new RsaSecurityKey(keyParams);
            // 添加到 IoC 容器
            // services.SigningCredentials(_tokenOptions);

          
            var tokenValidationParameters = new TokenValidationParameters
            {
                #region 下面三个参数是必须
                // 签名秘钥
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtSigningKey,
                // 发行者(颁发机构)
                ValidateIssuer = true,
                ValidIssuer = jwtOption.Issuer,
                // 令牌的观众(颁发给谁)
                ValidateAudience = true,
                ValidAudience = jwtOption.Audience,
                #endregion
                // 是否验证Token有效期
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero //ClockSkew:允许的服务器时间偏移量,默认是5分钟，如果不设置，时间有效期间到了以后，5分钟之内还可以访问资源
                /***********************************TokenValidationParameters的参数默认值***********************************/
                // RequireSignedTokens = true,
                // SaveSigninToken = false,
                // ValidateActor = false,
                // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                // ValidateAudience = true,
                // ValidateIssuer = true, 
                // ValidateIssuerSigningKey = false,
                // 是否要求Token的Claims中必须包含Expires
                // RequireExpirationTime = true,
                // 允许的服务器时间偏移量
                // ClockSkew = TimeSpan.FromSeconds(300),//TimeSpan.Zero
                // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                // ValidateLifetime = true
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //不使用https
                //o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = tokenValidationParameters;
            });
            #endregion

            #region 【密码模式 OIDC】和用户有关，一般用于第三方登录
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            // .AddCookie()
            // .AddOpenIdConnect(o =>
            // {
            //     o.ClientId = "oidc.hybrid";
            //     o.ClientSecret = "secret";

            //    // 若不设置Authority，就必须指定MetadataAddress
            //    o.Authority = "https://oidc.faasx.com/";
            //    // 默认为Authority+".well-known/openid-configuration"
            //    //o.MetadataAddress = "https://oidc.faasx.com/.well-known/openid-configuration";
            //    o.RequireHttpsMetadata = false;

            //    // 使用混合流
            //    o.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            //    // 是否将Tokens保存到AuthenticationProperties中
            //    o.SaveTokens = true;
            //    // 是否从UserInfoEndpoint获取Claims
            //    o.GetClaimsFromUserInfoEndpoint = true;
            //    // 在本示例中，使用的是IdentityServer，而它的ClaimType使用的是JwtClaimTypes。
            //    o.TokenValidationParameters.NameClaimType = "name"; //JwtClaimTypes.Name;

            //    // 以下参数均有对应的默认值，通常无需设置。
            //    //o.CallbackPath = new PathString("/signin-oidc");
            //    //o.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
            //    //o.RemoteSignOutPath = new PathString("/signout-oidc");
            //    //o.Scope.Add("openid");
            //    //o.Scope.Add("profile");
            //    //o.ResponseMode = OpenIdConnectResponseMode.FormPost; 

            //    /***********************************相关事件***********************************/
            //    // 未授权时，重定向到OIDC服务器时触发
            //    //o.Events.OnRedirectToIdentityProvider = context => Task.CompletedTask;

            //    // 获取到授权码时触发
            //    //o.Events.OnAuthorizationCodeReceived = context => Task.CompletedTask;
            //    // 接收到OIDC服务器返回的认证信息（包含Code, ID Token等）时触发
            //    //o.Events.OnMessageReceived = context => Task.CompletedTask;
            //    // 接收到TokenEndpoint返回的信息时触发
            //    //o.Events.OnTokenResponseReceived = context => Task.CompletedTask;
            //    // 验证Token时触发
            //    //o.Events.OnTokenValidated = context => Task.CompletedTask;
            //    // 接收到UserInfoEndpoint返回的信息时触发
            //    //o.Events.OnUserInformationReceived = context => Task.CompletedTask;
            //    // 出现异常时触发
            //    //o.Events.OnAuthenticationFailed = context => Task.CompletedTask;

            //    // 退出时，重定向到OIDC服务器时触发
            //    //o.Events.OnRedirectToIdentityProviderForSignOut = context => Task.CompletedTask;
            //    // OIDC服务器退出后，服务端回调时触发
            //    //o.Events.OnRemoteSignOut = context => Task.CompletedTask;
            //    // OIDC服务器退出后，客户端重定向时触发
            //    //o.Events.OnSignedOutCallbackRedirect = context => Task.CompletedTask;

            //});

            #endregion
            //注册简单的定时任务执行
            //services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MainService>();
        }
    }
}
