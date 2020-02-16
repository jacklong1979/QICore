using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.NSignalR.Common;

namespace QICore.NSignalR
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
            services.AddSingleton<SignalRModel>(provider =>
            {
                return new SignalRModel();
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddCors(op =>
            {
                op.AddPolicy("default", set =>
                {
                    set.SetIsOriginAllowed(origin => true)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
                });
            });
            services.AddMvc();
            services.AddSignalR();
            #region 配置authorrize登录验证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/api/values/Login";
                options.Cookie.Name = "AspnetcoreSessionId";
                options.Cookie.Path = "/"; //cookie所在的目录，asp.net默认为"/"，就是根目录
                options.Cookie.HttpOnly = true; //设置了HttpOnly属性，js脚本将无法读取到cookie信息，能有效的防止XSS攻击窃取cookie内容，增加cookie的安全性
                options.Cookie.Expiration = new TimeSpan(8, 0, 0); //cookie过期时间
                options.ExpireTimeSpan = new TimeSpan(8, 0, 0);
            });
            #endregion
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("default");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //验证中间件
            //该中间件一定要放在app.UseMvc前面，否则HttpContext.User.Identity.IsAuthenticated一直为false。
            app.UseAuthentication();

            app.UseSignalR(routes => { routes.MapHub<WeChatHub>("/api/hub"); });
            app.UseMvc();
        }
    }
}
