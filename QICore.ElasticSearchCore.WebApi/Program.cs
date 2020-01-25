using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace QICore.ElasticSearchCore.WebApi
{
    /// <summary>
    /// 主类.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 主函数.
        /// </summary>
        /// <param name="args">输入参数.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 缩主启动.
        /// </summary>
        /// <param name="args">输入参数.</param>
        /// <returns>返回IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
