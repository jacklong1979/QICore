using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using QICore.OAuthResourceServer.Options;

namespace QICore.OAuthResourceServer
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

            #region 【方式】IdentityServer + API+Client演示客户端模式
            services.AddMvcCore().AddJsonFormatters();
            services.AddAuthentication((options) =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                //ClockSkew:允许的服务器时间偏移量,默认是5分钟，如果不设置，时间有效期间到了以后，5分钟之内还可以访问资源
                options.TokenValidationParameters = new TokenValidationParameters() { ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(0) };               
                options.RequireHttpsMetadata = false;
                options.Audience = authOption.Audience;//api范围
                options.Authority = authOption.Authority;//IdentityServer地址,发布token的服务地址

                });

            #endregion
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

            // app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
