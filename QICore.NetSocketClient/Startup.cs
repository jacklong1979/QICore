using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using QICore.NetSocketClient.Common;

namespace QICore.NetSocketClient
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
            services.AddLogging();// 添加日志服务
            services.AddMvc();
            services.AddOptions();
            //services.Configure<PushServerOption>(Configuration.GetSection("PushServer"));
            services.AddMemoryCache();
            services.AddServiceSingleton(Configuration);
            #region 设置允许跨域请求
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("AllowAllOrigin", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod().AllowCredentials();
                });

            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();//添加日志中间件
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ServiceLocator.Instance = app.ApplicationServices;
            #region 设置允许跨域请求
            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
            #endregion
            app.UseMvc();
        }
    }
}
