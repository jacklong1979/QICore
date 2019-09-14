using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.QuartzCore.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.QuartzCore.Jobs
{
    [DisallowConcurrentExecution]//禁止并发执行
    public class TestJob : IJob
    {
        //private readonly Microsoft.Extensions.Logging.ILogger _logger;

        //public TestJob(ILoggerFactory loggerFactory)
        //{
        //    _logger = loggerFactory.CreateLogger<TestJob>();
        //}
        private readonly ILog logger;//= LogManager.GetLogger(typeof(TestJob));
        private readonly ILogger<TestJob> log;
        private readonly IOptions<JwtSettings> _settings;
        //public TestJob(IOptions<JwtSettings> setting)
        //{
        //    _settings = setting;
        //}
        public TestJob()
        {
           
            logger = Log4Helper.GetLogger(typeof(TestJob));
            log = BaseIoc.GetService<ILoggerFactory>().CreateLogger<TestJob>();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //ILoggerRepository repository = LogManager.CreateRepository("TestJob");
                //XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                //ILog log = LogManager.GetLogger(repository.Name, "TestJob");
                var methodName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
                logger.Info("InfoInfoInfoInfoInfoInfoInfoInfoInfoInfoInfoInfoInfo");
                logger.Warn("WarnWarnWarnWarnWarnWarnWarnWarnWarnWarnWarnWarnWarnWarnWarn");
                logger.Error("ErrorErrorErrorErrorErrorErrorErrorErrorErrorErrorErrorErrorError");
                logger.Debug("DebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebug");
                //log.LogInformation("aaaaaaaaaaaaaaaaa");
                //log.LogWarning("bbbbbbbbbbbbbbbbbbbbb");
                //log.LogError("cccccccccccccccccccccccccc");
                //log.LogDebug("dddddddddddddddddd");
                var jobData = context.JobDetail.JobDataMap;//获取Job中的参数
                var triggerData = context.Trigger.JobDataMap;//获取Trigger中的参数
                                                             //  var data = context.MergedJobDataMap;//获取Job和Trigger中合并的参数
                var value1 = jobData.GetString("key1");
                var value2 = jobData.GetString("key2");//
                var value3=triggerData.GetInt("key3");
                // var value3 = data.GetString("key2");

                await Task.Run(() =>
                {
                    logger.Info($"参数key1:{value1}");                  
                    //Thread.Sleep(5000);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
