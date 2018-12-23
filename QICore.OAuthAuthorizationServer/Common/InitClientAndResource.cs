using IdentityServer4.Models;
using Microsoft.Extensions.Options;
using QICore.OAuthAuthorizationServer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.OAuthAuthorizationServer.Common
{
    /// <summary>
    /// 初始化Client和Resource
    /// </summary>
    public class InitClientAndResource
    {
        #region 加载配置文件信息
        private AuthOption _tokenConfig { get; set; }
        public  InitClientAndResource(IOptions<AuthOption> settings)
        {
            _tokenConfig = settings.Value;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
           {
               new ApiResource("inventoryapi", "this is inventory api"),
               new ApiResource("orderapi", "this is order api"),
               new ApiResource("productapi", "this is product api")
           };
        }
        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
           {
               new Client
               {
                   ClientId = "inventory",
                   AllowedGrantTypes = GrantTypes.ClientCredentials,

                   ClientSecrets =
                   {
                       new Secret("lkc311@163.comLONGKC".Sha256())
                   },

                   AllowedScopes = { "inventoryapi" }
               },
                new Client
               {
                   ClientId = "order",
                   AllowedGrantTypes = GrantTypes.ClientCredentials,

                   ClientSecrets =
                   {
                       new Secret("lkc311@163.comLONGKC".Sha256())
                   },

                   AllowedScopes = { "orderapi" }
               },
                new Client
               {
                   ClientId = "product",
                   AllowedGrantTypes = GrantTypes.ClientCredentials,

                   ClientSecrets =
                   {
                       new Secret("lkc311@163.comLONGKC".Sha256())
                   },

                   AllowedScopes = { "productapi" }
               }
           };
        }
    }
}
