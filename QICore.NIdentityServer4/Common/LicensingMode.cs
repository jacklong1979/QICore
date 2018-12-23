using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QICore.NIdentityServer4.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QICore.NIdentityServer4.Common
{
    /*
    客户端模式（Client Credentials）：和用户无关，用于应用程序与 API 资源的直接交互场景,经常运用于服务器对服务器中间通讯使用。
    密码模式（resource owner password credentials）：和用户有关，一般用于第三方登录。
    简化模式-With OpenID（implicit grant type）：仅限 OpenID 认证服务，用于第三方用户登录及获取用户信息，不包含授权。
    简化模式-With OpenID & OAuth（JS 客户端调用）：包含 OpenID 认证服务和 OAuth 授权，但只针对 JS 调用（URL 参数获取），一般用于前端或无线端。
    混合模式-With OpenID & OAuth（Hybrid Flow）：推荐使用，包含 OpenID 认证服务和 OAuth 授权，但针对的是后端服务调用。
    */
    /// <summary>
    /// 【授权模式】   
    /// </summary>
    public class LicensingMode
    {
        /// <summary>
        /// AccessToken的加密证书
        /// </summary>
        /// <returns></returns>
        private static RsaSecurityKey GetRsaSecurityKey(string rsaString = "")
        {
            if (string.IsNullOrEmpty(rsaString))
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(2048))
                {
                    //Console.WriteLine(Convert.ToBase64String(provider.ExportCspBlob(false)));   //PublicKey
                    rsaString = Convert.ToBase64String(provider.ExportCspBlob(true));
                    Console.WriteLine("证书："+rsaString);    //PrivateKey
                }
            }
            //配置AccessToken的加密证书
            var rsa = new RSACryptoServiceProvider();
            //从配置文件获取加密证书
            rsa.ImportCspBlob(Convert.FromBase64String(rsaString));
            return new RsaSecurityKey(rsa);
        }
        private static readonly IdentityOption _IdentityOption;

        public  LicensingMode(IOptions<IdentityOption> identityOption) 
        {
           
        }

        /// <summary>
        /// 【客户端模式】【密码模式】
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="identityOption">IdentityOption</param>
        public static void SetResourceOwnerPasswordAndClientCredentials(IServiceCollection services, IdentityOption identityOption)
        {
            #region 【客户端模式】【密码模式】
            /*
             * 【客户端模式】和用户无关，用于应用程序与 API 资源的直接交互场景,经常运用于服务器对服务器中间通讯使用。
             * 【密码模式】和用户有关，一般用于第三方登录
             */
            var apiResources = new List<ApiResource>
            {
                //给api资源定义Scopes 必须与 Client 的 AllowedScopes 对应上，不然显示 invalid_scope
                new ApiResource(identityOption.Scope,identityOption.Scope)
            };
            Collection<Secret> clientSecrets = new Collection<Secret>() { new Secret(identityOption.Secret.Sha256()) };
            services.AddIdentityServer()
                .AddSigningCredential(GetRsaSecurityKey())
               //.AddDeveloperSigningCredential()//设置RSA的加密证书（注意：默认是使用临时证书的，就是AddTemporarySigningCredential()，无论如何不应该使用临时证书，因为每次重启授权服务，就会重新生成新的临时证书），RSA加密证书长度要2048以上，否则服务运行会抛异常
               .AddInMemoryApiResources(apiResources)//添加api资源
               .AddInMemoryClients(new List<Client> {
                   new Client{
                        ClientId =identityOption.ClientId,
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                        ClientSecrets =clientSecrets,
                        AccessTokenLifetime = identityOption.ExpiresIn,
                        AllowedScopes = { identityOption.Scope }
                   }
                  }
                )
               .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
            #endregion
        }
        public static void SetOIDC(IServiceCollection services, IdentityOption identityOption)
        {
            /* 简单来说：OIDC 是OpenID Connect的简称，OIDC=(Identity, Authentication) + OAuth 2.0。它在OAuth2上构建了一个身份层，是一个基于OAuth2协议的身份认证标准协议
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
            var apiResources = new List<ApiResource>
            {
                //给api资源定义Scopes 必须与 Client 的 AllowedScopes 对应上，不然显示 invalid_scope
                new ApiResource(identityOption.Scope,identityOption.Scope)
            };
            Collection<Secret> clientSecrets = new Collection<Secret>() { new Secret(identityOption.Secret.Sha256()) };
            services.AddIdentityServer()
                .AddSigningCredential(GetRsaSecurityKey())
               //.AddDeveloperSigningCredential()//设置RSA的加密证书（注意：默认是使用临时证书的，就是AddTemporarySigningCredential()，无论如何不应该使用临时证书，因为每次重启授权服务，就会重新生成新的临时证书），RSA加密证书长度要2048以上，否则服务运行会抛异常
               .AddInMemoryApiResources(apiResources)//添加api资源
               .AddInMemoryClients(new List<Client> {
                   new Client{
                        ClientId =identityOption.ClientId,
                        AllowedGrantTypes = GrantTypes.Implicit,
                        ClientSecrets =clientSecrets,
                        AccessTokenLifetime = identityOption.ExpiresIn,
                        ClientName =identityOption.ClientName,
                        // where to redirect to after login
                        RedirectUris = { identityOption.RedirectUri},//登录成功后定向地址
                        // where to redirect to after logout
                        PostLogoutRedirectUris = {identityOption.LogoutRedirectUri },
                        AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile
                        }
                   }
                  }
                )
               .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

        }
        /// <summary>
        /// 密码模式（resource owner password credentials）：和用户有关，一般用于第三方登录。
        /// </summary>
        /// <returns></returns>
        public static Client GetResourceOwnerPassword()
        {
            var client = new Client
            {
                ClientId = _IdentityOption.ClientId,//注意：客户端不能包含重复ID
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AccessTokenType = AccessTokenType.Jwt,
                AccessTokenLifetime = _IdentityOption.ExpiresIn,
                IdentityTokenLifetime = _IdentityOption.ExpiresIn,
                UpdateAccessTokenClaimsOnRefresh = true,
                SlidingRefreshTokenLifetime = _IdentityOption.ExpiresIn,
                AllowOfflineAccess = true,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,//默认状态，RefreshToken只能使用一次，使用一次之后旧的就不能使用了，只能使用新的RefreshToken
                AlwaysSendClientClaims = true,
                Enabled = true,
                ClientSecrets =
                    {
                        new Secret(_IdentityOption.Secret.Sha256())
                    },
                AllowedScopes = { IdentityServerConstants.StandardScopes.OfflineAccess, _IdentityOption.Scope }
                //ClientId = "pwdClient",
                //AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,//Resource Owner Password模式需要对账号密码进行验证（如果是client credentials模式则不需要对账号密码验证了）：
                //ClientSecrets ={new Secret(secretString.Sha256())},                  
                //AllowedScopes =
                //{
                //    "UserApi"
                //    //如果想带有RefreshToken，那么必须设置：StandardScopes.OfflineAccess
                //    //如果是Client Credentials模式不支持RefreshToken的，就不需要设置OfflineAccess
                //    //StandardScopes.OfflineAccess
                //}
                // //AccessTokenLifetime = 3600, //AccessToken的过期时间， in seconds (defaults to 3600 seconds / 1 hour)
                ////AbsoluteRefreshTokenLifetime = 60, //RefreshToken的最大过期时间，in seconds. Defaults to 2592000 seconds / 30 day
                ////RefreshTokenUsage = TokenUsage.OneTimeOnly,   //默认状态，RefreshToken只能使用一次，使用一次之后旧的就不能使用了，只能使用新的RefreshToken
                ////RefreshTokenUsage = TokenUsage.ReUse,   //可重复使用RefreshToken，RefreshToken，当然过期了就不能使用了
            };
            return client;
        }
    }
}
