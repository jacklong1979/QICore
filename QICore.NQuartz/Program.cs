using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QICore.NQuartz.Dao;
using QICore.NQuartz.IDao;
using System;
using NLog;
using QICore.NCommon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using NLog.Extensions.Logging;
using NLog.Web;

namespace QICore.NQuartz
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            #region 微软自己的依赖注入
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()//日志
                .AddSingleton<IQuartzCommon, QuartzCommon>()//
                .BuildServiceProvider();
            #endregion
            #region 配置控制台输出日志，日志输出的级别
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(Microsoft.Extensions.Logging.LogLevel.Debug);//定义日志输出的级别
            #endregion
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogInformation("Quarz 已启动");
            var quartz = serviceProvider.GetService<IQuartzCommon>();
            quartz.GetName();
            Console.WriteLine("魂牵梦萦!");
            logger.LogDebug("LogDebug 已启动");
            logger.LogError("LogError 已启动");
            logger.LogWarning("LogWarning 已启动");
            Console.WriteLine("Hello World!");
            Logger logger2 = LogManager.GetCurrentClassLogger();
            logger2.Trace("输出一条记录信息成功！");//最常见的记录信息，一般用于普通输出 
            logger2.Debug("输出一条Debug信息成功！"); //同样是记录信息，不过出现的频率要比Trace少一些，一般用来调试程序 
            logger2.Info("输出一条消息类型信息成功！");//信息类型的消息 
            logger2.Warn("输出一条警告信息成功");//警告信息，一般用于比较重要的场合 
            logger2.Error("输出一条错误信息成功！");//错误信息
            logger2.Fatal("输出一条致命信息成功！");//致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。
           
            Console.ReadKey();
        }
    }
   
}
