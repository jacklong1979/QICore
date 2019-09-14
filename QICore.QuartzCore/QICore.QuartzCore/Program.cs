using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QICore.QuartzCore.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.QuartzCore
{
    class Program
    {
        /*
         示例：

        "0 0 0 1 1 ?”               每年元旦1月1日 0 点触发

        "0 15 10 * * ? *"         每天上午10:15触发  
        "0 15 10 * * ? 2005"   2005年的每天上午10:15触发

        "0 0-5 14 * * ?"          每天下午2点到下午2:05期间的每1分钟触发  
        "0 10,44 14 ? 3 WED"  每年三月的星期三的下午2:10和2:44触发  
        "0 15 10 ? * MON-FRI" 周一至周五的上午10:15触发

        "0 15 10 ? * 6#3"        每月的第三个星期五上午10:15触发

         */
        public static int index = 0;
        private static ISchedulerFactory _schedulerFactory;
        private static IScheduler _scheduler;

        static async Task Main(string[] args)
        {
            BaseIoc.InitIoc();//初始化容器           
            //var builder = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //IConfigurationRoot configuration = builder.Build();

            var services = new ServiceCollection().AddLogging().BuildServiceProvider();
            services.GetService<ILoggerFactory>().AddConsole(LogLevel.Debug);

            ////注入
            ////  services.AddTransient<IMemcachedClient, MemcachedClient>();
            ////services.AddLogging();
            ////构建容器
            ////  IServiceProvider serviceProvider = services.BuildServiceProvider();
            var logger = services.GetService<ILoggerFactory>().CreateLogger<Program>();
            //logger.LogInformation("LogInformationLogInformationLogInformationLogInformation");
            //logger.LogWarning("LogWarningLogWarningLogWarningLogWarningLogWarning");
            //logger.LogError("LogErrorLogErrorLogErrorLogError");
            //logger.LogDebug("DebugDebugDebug");
            ////解析

            //var currTime = DateTime.Now;
            //Console.WriteLine($"当前时间：{currTime}");
           // await NewMethod(currTime);
            try
            {
                for (var i = 0; i < 10; i++)
                {
                    await QuartzHelper.AddJob($"深圳{i}", "广东省");
                }             
                //QuartzHelper.AddJob<TestJob>(3, "作业1", "中国");
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
            // QuartzHelper.AddJob<Test2Job>(5, "作业2", "中国");
            // QuartzHelper.AddJob<Test3Job>(7, "作业3", "中国");
            Console.WriteLine("Hello World!");
            Console.Read();
        }

        private static async Task NewMethod(DateTime currTime)
        {
            _schedulerFactory = new StdSchedulerFactory();
            //1、通过调度工厂获得调度器
            _scheduler = await _schedulerFactory.GetScheduler();
            //添加监听器到指定的trigger

            _scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            _scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            _scheduler.ListenerManager.AddJobListener(new CustomJobListener());

            ////添加监听器到指定分类的所有监听器。
            //scheduler.ListenerManager.AddTriggerListener(myJobListener, GroupMatcher<TriggerKey>.GroupEquals("myJobGroup"));

            ////添加监听器到指定分类的所有监听器。
            //scheduler.ListenerManager.AddTriggerListener(myJobListener, GroupMatcher<TriggerKey>.GroupEquals("myJobGroup"));

            ////添加监听器到指定的2个分组。
            //scheduler.ListenerManager.AddTriggerListener(myJobListener, GroupMatcher<TriggerKey>.GroupEquals("myJobGroup"), GroupMatcher<TriggerKey>.GroupEquals("myJobGroup2"));

            ////添加监听器到所有的触发器上。
            //scheduler.ListenerManager.AddTriggerListener(myJobListener, GroupMatcher<TriggerKey>.AnyGroup());


            //2、开启调度器
            await _scheduler.Start();
            //3、创建一个触发器
            var startTime = currTime.AddSeconds(5);
            var trigger = TriggerBuilder.Create()
                .UsingJobData("key3", ++index)
                .WithIdentity("执行表达式", "大数据")
                            .StartAt(startTime)//设置任务开始时间
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())//每两秒执行一次
                            .Build();

            var trigger2 = TriggerBuilder.Create()
               .WithIdentity("手环", "大数据")
                           .StartAt(startTime)//设置任务开始时间
                           .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).RepeatForever())//每两秒执行一次
                           .Build();

            //每个2秒执行一次
            string cron = "*/2 * * * * ?";
            var cronTrigger = TriggerBuilder.Create()
                            .WithCronSchedule(cron)
                            .Build();

            Console.WriteLine($"开始时间：{startTime}");
            //4、创建任务(job)
            var jobDetail = JobBuilder.Create<TestJob>()
                .UsingJobData("key1", "AAAAAAAAAAA")
                .UsingJobData("key2", "BBBBBBBBBB")
                .WithIdentity("job", "group").Build();
            var jobDetail2 = JobBuilder.Create<Test2Job>().WithIdentity("job2", "group").Build();
            var jobDetailCron = JobBuilder.Create<Test3Job>().WithIdentity("job3", "group").Build();
            //5、将触发器和任务器绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);
            // await _scheduler.ScheduleJob(jobDetail2, trigger2);
            // await _scheduler.ScheduleJob(jobDetailCron, cronTrigger);
        }
        #region 多任务
        public static void StartJobs<TJob>() where TJob : IJob
        {
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;

            var job = JobBuilder.Create<TJob>()
                .WithIdentity("jobs")
                .Build();

            var trigger1 = TriggerBuilder.Create()
                .WithIdentity("job.trigger1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(5)).RepeatForever())
                .ForJob(job)
                .Build();

            var trigger2 = TriggerBuilder.Create()
                .WithIdentity("job.trigger2")
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(11)).RepeatForever())
                .ForJob(job)
                .Build();

            var dictionary = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
            {
                {job, new HashSet<ITrigger> {trigger1, trigger2}}
            };
            scheduler.ScheduleJobs(dictionary, true);
            scheduler.Start();
        }
        #endregion
       
        
    }
}
