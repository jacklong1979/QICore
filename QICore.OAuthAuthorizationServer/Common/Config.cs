using IdentityServer4;
using IdentityServer4.Models;
using QICore.OAuthAuthorizationServer.Options;
using System.Collections.Generic;

public class Config
{
    //public static IEnumerable<IdentityResource> GetIdentityResourceResources()
    //{
    //    return new List<IdentityResource>
    //        {
    //            new IdentityResources.OpenId(), //必须要添加，否则报无效的scope错误
    //            new IdentityResources.Profile()
    //        };
    //}
    // scopes define the API resources in your system
    /// <summary>
    /// 定义授权范围
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static IEnumerable<ApiResource> GetApiResources(AuthOption option)
    {
        var apiList = new List<ApiResource>();
        if (option != null && option.ClientCredentials != null && option.ClientCredentials.Count > 0)
        {
            foreach (var o in option.ClientCredentials)
            {
                var api = new ApiResource(o.Scope, o.Description);
                apiList.Add(api);
            }
        }
        if (option != null && option.ResourceOwnerPassword != null && option.ResourceOwnerPassword.Count > 0)
        {
            foreach (var o in option.ResourceOwnerPassword)
            {
                var api = new ApiResource(o.Scope, o.Description);
                apiList.Add(api);
            }
        }
        return apiList;
        //return new List<ApiResource>
        //    {
        //        new ApiResource("api1", "My API"),
        //        new ApiResource("api2", "My API2")
        //    };
        //return new List<Scope>
        //   {
        //                new Scope
        //       {
        //                        Name = "api1",
        //           Description = "My API",
        //       },
        //        //如果想带有RefreshToken，那么必须设置：StandardScopes.OfflineAccess
        //       // StandardScopes.OfflineAccess,
        //    };
    }

    // clients want to access resources (aka scopes)
    /// <summary>
    /// 定义客户端与授权范围
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static IEnumerable<Client> GetClients(AuthOption option)
    {
        // client credentials client
        var clientList = new List<Client>();
        if (option != null && option.ClientCredentials != null && option.ClientCredentials.Count > 0)
        {
            foreach (var o in option.ClientCredentials)
            {
                var clientSecrets = new Secret[] { new Secret(option.Secret.Sha256()) };
                var scopes = new string[] { o.Scope }; ;
                var client = new Client();
                client.ClientId = o.ClientId;
                client.AllowedGrantTypes = GrantTypes.ClientCredentials;
                client.ClientSecrets = clientSecrets;
                client.AllowedScopes = scopes;
                client.AccessTokenLifetime = option.ExpiresIn;
                clientList.Add(client);
            }
        }
        if (option != null && option.ResourceOwnerPassword != null && option.ResourceOwnerPassword.Count > 0)
        {
            foreach (var o in option.ResourceOwnerPassword)
            {
                var clientSecrets = new Secret[] { new Secret(option.Secret.Sha256()) };
                var scopes = new string[] { o.Scope }; ;
                var client = new Client();
                client.ClientId = o.ClientId;
                client.AllowedGrantTypes = GrantTypes.ResourceOwnerPassword;
                client.ClientSecrets = clientSecrets;
                client.AllowedScopes = scopes;
                client.AccessTokenLifetime = option.ExpiresIn;
                clientList.Add(client);
            }
        }
        return clientList;


        //return new List<Client>
        //    {
        //        new Client
        //        {
        //            ClientId = "client1",
        //            AllowedGrantTypes = GrantTypes.ClientCredentials,

        //            ClientSecrets =
        //            {
        //                new Secret("secret".Sha256())
        //            },
        //            //下面一句必须与：GetIdentityResourceResources同时出现
        //          //  AllowedScopes = { "api1",IdentityServerConstants.StandardScopes.OpenId, //必须要添加，否则报forbidden错误
        //          //IdentityServerConstants.StandardScopes.Profile},
        //              AllowedScopes = { "api1","api2" },
        //            AccessTokenLifetime = option.ExpiresIn
        //            // AllowedScopes =
        //            //{
        //            //    "api1","api2"
        //            //    //如果想带有RefreshToken，那么必须设置：StandardScopes.OfflineAccess
        //            //    //如果是Client Credentials模式不支持RefreshToken的，就不需要设置OfflineAccess
        //            //    StandardScopes.OfflineAccess
        //            //}
        //             //AccessTokenLifetime = 3600, //AccessToken的过期时间， in seconds (defaults to 3600 seconds / 1 hour)
        //            //AbsoluteRefreshTokenLifetime = 60, //RefreshToken的最大过期时间，in seconds. Defaults to 2592000 seconds / 30 day
        //            //RefreshTokenUsage = TokenUsage.OneTimeOnly,   //默认状态，RefreshToken只能使用一次，使用一次之后旧的就不能使用了，只能使用新的RefreshToken
        //            //RefreshTokenUsage = TokenUsage.ReUse,   //可重复使用RefreshToken，RefreshToken，当然过期了就不能使用了

        //        },

        //        // resource owner password grant client
        //        new Client
        //        {
        //            ClientId = "client2",
        //            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

        //            ClientSecrets =
        //            {
        //                new Secret("secret".Sha256())
        //            },
        //          //  AllowedScopes = { "api1",IdentityServerConstants.StandardScopes.OpenId, //必须要添加，否则报forbidden错误
        //          //IdentityServerConstants.StandardScopes.Profile }

        //              AllowedScopes = { "api1" },
        //                AccessTokenLifetime = option.ExpiresIn
        //        }
        //    };
    }
}