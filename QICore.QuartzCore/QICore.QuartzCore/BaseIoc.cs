using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QICore.QuartzCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QICore.QuartzCore
{
   public class BaseIoc
    {
        /// <summary>
        /// IOC容器
        /// </summary>
        private static IServiceCollection serviceCollection { get; } = new ServiceCollection();

        /// <summary>
        /// 初始化IOC容器
        /// </summary>
        public static void InitIoc()
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            var audience = configuration["JwtSettings:Audience"];
            var identityConn = configuration.GetConnectionString("Connection");//读取数据库连接字符串
                                                                               //db
                                                                               // serviceCollection.AddTransient(_ => new PostgreDbContext(identityConn));
                                                                               //log
            serviceCollection.AddOptions(); //注入IOptions<T>，才可以在DI容器中获取IOptions<T>
            serviceCollection.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            //var jwtSettings = new JwtSettings();
            //configuration.Bind("JwtSettings", jwtSettings);
            serviceCollection.AddLogging(configure =>
            {
                configure.AddConfiguration(configuration.GetSection("Logging"));//读取配置的日志配置
                configure.AddConsole();
            });
            //config
            serviceCollection.AddSingleton<IConfiguration>(configuration);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return serviceCollection.BuildServiceProvider().GetService<T>();
        }
    }
}
