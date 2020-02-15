using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace QICore.ElasticSearchCore.WebApi
{
    /// <summary>
    /// ����.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// ������.
        /// </summary>
        /// <param name="args">�������.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// ��������.
        /// </summary>
        /// <param name="args">�������.</param>
        /// <returns>����IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
