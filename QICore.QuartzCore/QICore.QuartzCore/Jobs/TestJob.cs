using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QICore.QuartzCore.Jobs
{    public class TestJob : IJob
    {
        //private readonly Microsoft.Extensions.Logging.ILogger _logger;

        //public TestJob(ILoggerFactory loggerFactory)
        //{
        //    _logger = loggerFactory.CreateLogger<TestJob>();
        //}
      
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                ILoggerRepository repository = LogManager.CreateRepository("TestJob");
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                ILog log = LogManager.GetLogger(repository.Name, "TestJob");

                log.Info("TestJobTestJob");
                log.Warn("TestJobTestJob");
                log.Error("TestJobTestJob");
                log.Debug("TestJobTestJob");
               
                var jobData = context.JobDetail.JobDataMap;//获取Job中的参数
                var triggerData = context.Trigger.JobDataMap;//获取Trigger中的参数
                                                             //  var data = context.MergedJobDataMap;//获取Job和Trigger中合并的参数
                var value1 = jobData.GetString("key1");
                var value2 = jobData.GetString("key2");//
                var value3=triggerData.GetInt("key3");
                // var value3 = data.GetString("key2");

                await Task.Run(() =>
                {
                  
                    Console.WriteLine($"{DateTime.Now}:任务执行了Job中的参数key1:{value1}");
                    Console.WriteLine($"{DateTime.Now}:任务执行了Job中的参数key2:{value2}");
                    Console.WriteLine($"{DateTime.Now}:任务执行了Trigger中的参数key3:{value3}");
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
