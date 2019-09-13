using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            //配置文件
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);     
            IConfigurationRoot configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();
            //注入
          //  services.AddTransient<IMemcachedClient, MemcachedClient>();
            //构建容器
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            //解析
           // var memcachedClient = serviceProvider.GetService<IMemcachedClient>();

            var currTime = DateTime.Now;
            Console.WriteLine($"当前时间：{currTime}");
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
                .UsingJobData("key2", index)
                .WithIdentity("执行表达式","大数据")
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
                            .WithCronSchedule("*/1 * * * * ?")
                            .Build();

            Console.WriteLine($"开始时间：{startTime}");
            //4、创建任务(job)
            var jobDetail = JobBuilder.Create<TestJob>()
              //  .UsingJobData("key1","AAAAAAAAAAA")
              //  .UsingJobData("key2", "BBBBBBBBBB")
                .WithIdentity("job", "group").Build();
            var jobDetail2 = JobBuilder.Create<Test2Job>().WithIdentity("job2", "group").Build();
            var jobDetailCron = JobBuilder.Create<Test3Job>().WithIdentity("job3", "group").Build();
            //5、将触发器和任务器绑定到调度器中
          //  await _scheduler.ScheduleJob(jobDetail, trigger);
          //  await _scheduler.ScheduleJob(jobDetail2, trigger2);
            await _scheduler.ScheduleJob(jobDetailCron, cronTrigger);
            Console.WriteLine("Hello World!");
            Console.Read();
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
    #region IJob
    public class TestJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
           // var jobData = context.JobDetail.JobDataMap;//获取Job中的参数
           // var triggerData = context.Trigger.JobDataMap;//获取Trigger中的参数
                                                         //  var data = context.MergedJobDataMap;//获取Job和Trigger中合并的参数
            var value1 = "AAA";// jobData.GetString("key1");
            var value2 = "BBBB";// triggerData.GetString("key2");
           // var value3 = data.GetString("key2");

            await Task.Run(() => {
                Console.WriteLine($"{DateTime.Now}:任务执行了Job中的参数key1:{value1}");
                Console.WriteLine($"{DateTime.Now}:任务执行了Trigger中的参数key2:{value2}");
            });
        }
    }
    public class Test2Job : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => {
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine($"{DateTime.Now}:Job2[{++Program.index}]");
                }
                // _scheduler.Shutdown(true);
            });
        }
    }
    public class Test3Job : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => {
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine($"{DateTime.Now}:Job3[{++Program.index}]");
                    Thread.Sleep(1000);
                }
                // _scheduler.Shutdown(true);
            });
        }
    }
    #endregion
    #region IJobListener
    public class CustomJobListener : IJobListener
    {
        public string Name => "CustomJobListener";
        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(() => {
                Console.WriteLine($"IJobListener [1]【Job 执行被否决】 {context.JobDetail.Description}");
            });
        }
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobName = ((Quartz.Impl.Triggers.AbstractTrigger)((Quartz.Impl.JobExecutionContextImpl)context).Trigger).JobName;
            await Task.Run(() => {
                Console.WriteLine($"IJobListener [2]【Job 正在执行...】 {jobName}");
            });
        }
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobName = ((Quartz.Impl.Triggers.AbstractTrigger)((Quartz.Impl.JobExecutionContextImpl)context).Trigger).JobName;

            await Task.Run(() => {
                Console.WriteLine($"IJobListener [3]【Job 已执行完成】 {jobName}");
            });
        }
    }
    #endregion
    #region ITriggerListener
    public class CustomTriggerListener : ITriggerListener
    {
        public string Name => "CustomTriggerListener";

        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                Console.WriteLine($"ITriggerListener [4]【触发完成】 {triggerTemp.FullJobName}");
            });
        }

        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // (1)Trigger被激发 它关联的job即将被运行
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                Console.WriteLine($"ITriggerListener [5]【触发执行中】 {triggerTemp.FullJobName}");
            });
        }
        /**
    *  当Trigger错过被激发时执行,比如当前时间有很多触发器都需要执行，但是线程池中的有效线程都在工作，
    *  那么有的触发器就有可能超时，错过这一轮的触发。
    * Called by the Scheduler when a Trigger has misfired.
    */
        public async Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                Console.WriteLine($"ITriggerListener [6]【不起作用】 {triggerTemp.FullJobName}");
            });
        }

        /// <summary>
        /// 要不要放弃job
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Trigger被激发 它关联的job即将被运行,先执行(1)，在执行(2) 如果返回TRUE 那么任务job会被终止
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                Console.WriteLine($"ITriggerListener [7]【终止作业执行】 {triggerTemp.FullJobName}");
            });
            return false;//false才能继续执行
        }
    }
    #endregion
    #region ISchedulerListener
    public class CustomSchedulerListener : ISchedulerListener
    {
        public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [8]【增加Job】 {jobDetail.Key}");
            });
        }

        public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [9]【删除Job】 {jobKey.Name}");
            });
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [10]【中断Job】 {jobKey.Name}");
            });
        }

        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [11]【暂停Job】 {jobKey.Name}");
            });
        }

        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [12]【恢复ob】 {jobKey.Name}");
            });
        }

        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            var trigerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [13]【计划Job】 {trigerTemp.FullJobName}({trigerTemp.FullName})");
            });
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [14]【暂停Job组】 {jobGroup}");
            });
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [15]【恢复Job组】 { jobGroup}");
            });
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [16]【未计划的Job】 { triggerKey.Name}");
            });
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [17]【调度程序错误】 { msg}");
            });
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [18]【调度待机模式】 ");
            });
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [19]【调度关闭】 ");
            });
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [20]【调度停止】 ");
            });
        }

        public Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [21]【调度已经开始】 ");
            });
        }

        public Task SchedulerStarting(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [22]【调度正在开始.....】 ");
            });
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [23]【调度正在清理数据.....】 ");
            });
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [24]【调度正在清理数据.....】 ");
            });
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [25]【TriggerPaused】 ");
            });
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [26]【TriggerResumed】 ");
            });
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [27]【TriggersPaused】 ");
            });
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"ISchedulerListener [28]【TriggersResumed】 ");
            });
        }
    }
    #endregion

}
