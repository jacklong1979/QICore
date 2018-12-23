using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QICore.OAuthAuthorizationServer.Common;
using QICore.OAuthAuthorizationServer.Options;
namespace QICore.OAuthAuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions(Configuration);//配置选择项扩展(所有的配置都应在这个方法中处理)
                                               //实体配置
            var provider = new ServiceCollection().AddOptions()
                        .Configure<AuthOption>(Configuration.GetSection("AuthOption"))
                        .BuildServiceProvider();
            var authOption = provider.GetService<IOptions<AuthOption>>().Value;


            services.AddIdentityServer()
                  .AddDeveloperSigningCredential()
                  //  .AddInMemoryIdentityResources(Config.GetIdentityResourceResources())
                  .AddInMemoryApiResources(Config.GetApiResources(authOption))
                  .AddInMemoryClients(Config.GetClients(authOption))
                  .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                  .AddProfileService<ProfileService>();
            services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //使用identityserver中间件
            app.UseIdentityServer();         
            //  app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
