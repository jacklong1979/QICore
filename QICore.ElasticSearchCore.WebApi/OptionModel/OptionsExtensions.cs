using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.OptionModel
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
            ////将appsettings.json中的 ConnectionStrings 部分文件读取到 ConnectionStrings 中，这是给其他地方用的
            //services.Configure<ConnectionStringOption>(Configuration.GetSection("ConnectionStrings"));
            ////如果初始化的时候我们就需要用，使用Bind的方式读取配置
            ////将配置绑定到 ConnectionStrings 实例中
            //var connectionStringOption = new ConnectionStringOption();
            //Configuration.Bind("ConnectionStrings", connectionStringOption);
            services.Configure<ConnectionStringOption>(configuration.GetSection("ConnectionStrings"));
        }
    }
}
