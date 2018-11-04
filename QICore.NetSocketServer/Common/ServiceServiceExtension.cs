using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QICore.NetSocketServer.Controllers;
using QICore.NetSocketServer.DaoInterface;
using QICore.NetSocketServer.DaoService;
using QICore.NetSocketServer.Models;

namespace QICore.NetSocketServer.Common
{
    public static class ServiceServiceExtension
    {
        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddServiceSingleton(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<ICacheService, MemoryCacheService>();            
            services.Configure<SocketSetting>(Configuration.GetSection("SocketServer"));
            //注册简单的定时任务执行
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MainService>();
        }

    }   
}
