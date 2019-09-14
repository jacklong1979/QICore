using log4net;
using QICore.QuartzCore.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QICore.QuartzCore
{
   public class QuartzHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(QuartzHelper));
        private static ISchedulerFactory _schedulerFactory;
        private static IScheduler scheduler;
        private static string threadCount = "20";//设置线程池个数为20 ,默认是10个  
        private static List<IScheduler> _listsche = new List<IScheduler>();
        /// <summary>
        /// 【新增作业】
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        public static async Task AddJob(string jobName = "", string jobGroup = "")
        {
            var pairs = new System.Collections.Specialized.NameValueCollection() { };
             pairs.Add("quartz.threadPool.ThreadCount", threadCount);   //设置线程池个数为20 ,默认是10个  
            _schedulerFactory = new StdSchedulerFactory(pairs);//将前面的配置加到Scheduler工厂中
            //1、通过调度工厂获得调度器
            scheduler = await _schedulerFactory.GetScheduler();
            //添加监听器到指定的trigger
            scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            //2、开启调度器
            await scheduler.Start();
            //3、创建一个触发器
            var trigger = TriggerBuilder.Create()             
                .WithIdentity($"触发器_{jobName}", $"组_{jobGroup}")
                            .StartNow()//设置任务开始时间
                           // .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())//每seconds秒执行一次
                            .Build();

            //4、创建任务(job)
            var jobDetail = JobBuilder.Create<TestJob>()
               // .UsingJobData("key1", "AAAAAAAAAAA")
               // .UsingJobData("key2", "BBBBBBBBBB")
                .WithIdentity($"{jobName}", $"{jobGroup}").Build();          
            await scheduler.ScheduleJob(jobDetail, trigger);
            var meta =await scheduler.GetMetaData();
            int threadPoolSize = meta.ThreadPoolSize;
            Console.WriteLine("线程池的个数为：{0}", threadPoolSize);

        }
        /// <summary>
        ///新增作业每隔{seconds}秒重复一次
        /// </summary>
        /// <typeparam name="T"></typeparam>       
        /// <param name="seconds">每隔{seconds}秒执行：</param>
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>
        /// <param name="map">参数</param>
        public static async Task AddJob(string jobName = "", string jobGroup = "",IDictionary<string,object> map=null)
        {
            var pairs = new System.Collections.Specialized.NameValueCollection() { };
            pairs.Add("quartz.threadPool.ThreadCount", threadCount);   //设置线程池个数为20 ,默认是10个  
            _schedulerFactory = new StdSchedulerFactory(pairs);//将前面的配置加到Scheduler工厂中
            //1、通过调度工厂获得调度器
            scheduler = await _schedulerFactory.GetScheduler();
            //添加监听器到指定的trigger
            scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            //2、开启调度器
            await scheduler.Start();
            //3、创建一个触发器
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"触发器_{jobName}", $"组_{jobGroup}")
                            .StartNow()//现在开始
                            //.WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())//每seconds秒执行一次
                            .Build();           
            //4、创建任务(job)
            if (map != null && map.Count > 0)
            {
                JobDataMap mapData = new JobDataMap(map);
                var jobDetail = JobBuilder.Create<TestJob>()
                     .UsingJobData(mapData)
                    .WithIdentity($"{jobName}", $"{jobGroup}").Build();
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            else
            { 

                var jobDetail = JobBuilder.Create<TestJob>()                   
                    .WithIdentity($"{jobName}", $"{jobGroup}").Build();
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            var meta = await scheduler.GetMetaData();
            int threadPoolSize = meta.ThreadPoolSize;
            Console.WriteLine("线程池的个数为：{0}", threadPoolSize);

        }
        /// <summary>
        ///新增作业， 设置【开始时间】【结束时间】每隔{seconds}秒重复一次
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startTime">开始时间</param>
        /// <param name="entTime">结束时间</param>
        /// <param name="seconds">每隔多少秒重复一次</param>
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>

        public static void AddJob<T>(DateTime startTime,DateTime entTime, int seconds,string jobName = "",string jobGroup = "") where T : IJob
        {
            jobName = string.IsNullOrEmpty(jobName) ? jobGroup : jobName;
            jobGroup = string.IsNullOrEmpty(jobGroup) ? jobName : jobGroup;
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            IJobDetail job = JobBuilder.Create<T>().Build();
            ITrigger trigger = null;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobGroup))
            {
                  trigger = TriggerBuilder.Create()
                   //.UsingJobData("key3", ++index)
                   .WithIdentity($"trigger_{jobName}", $"trigger_{jobGroup}")
                               .StartAt(startTime)//设置任务开始时间
                               .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())
                               .EndAt(entTime)
                               .ForJob(jobName, jobGroup)
                               .Build();
            }
            else
            {
                trigger = TriggerBuilder.Create()     
                              .StartAt(startTime)//设置任务开始时间
                              .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).RepeatForever())
                              .EndAt(entTime)
                             
                              .Build();
            }
            _listsche.Add(scheduler);
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
        /// <summary>
        ///新增作业每隔{seconds}秒重复一次
        /// </summary>
        /// <typeparam name="T"></typeparam>       
        /// <param name="seconds">每隔{seconds}秒执行：</param>
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>
        public static void AddJob<T>(int seconds, string jobName = "", string jobGroup = "") where T : IJob
        {
            jobName = string.IsNullOrEmpty(jobName) ? jobGroup : jobName;
            jobGroup = string.IsNullOrEmpty(jobGroup) ? jobName : jobGroup;
            //var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler =  factory.GetScheduler().Result;
            scheduler.Start();
            //scheduler =   _schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddSchedulerListener(new CustomSchedulerListener());
            scheduler.ListenerManager.AddTriggerListener(new CustomTriggerListener());
            scheduler.ListenerManager.AddJobListener(new CustomJobListener());
            IJobDetail job = JobBuilder.Create<T>().WithIdentity(jobName, jobName).Build();
            ITrigger trigger = null;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobGroup))
            {
                 trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity($"trigger_{jobName}", $"trigger_{jobGroup}")
                .StartAt(DateBuilder.FutureDate(seconds, IntervalUnit.Second)) //使用DateBuilder将来创建一个时间日期
                .ForJob(jobName, jobGroup) //通过JobKey识别作业
                .Build();
            }
            else
            {
                trigger = (ISimpleTrigger)TriggerBuilder.Create()                
                 .StartAt(DateBuilder.FutureDate(seconds, IntervalUnit.Second)) //使用DateBuilder将来创建一个时间日期                
                 .Build();
            }
            _listsche.Add(scheduler);
            scheduler.ScheduleJob(job, trigger);
           
        }
        /// <summary>
        ///新增作业
        /// </summary>
        /// <typeparam name="T"></typeparam>       
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>

        public static void AddJob<T>(string jobName = "", string jobGroup = "") where T : IJob
        {
            jobName = string.IsNullOrEmpty(jobName) ? jobGroup : jobName;
            jobGroup = string.IsNullOrEmpty(jobGroup) ? jobName : jobGroup;
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            IJobDetail job = JobBuilder.Create<T>().Build();
            ITrigger trigger = null;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobGroup))
            {
                trigger = (ISimpleTrigger)TriggerBuilder.Create()
               .WithIdentity($"trigger_{jobName}", $"trigger_{jobGroup}")
               .StartNow() 
               .ForJob(jobName, jobGroup) //通过JobKey识别作业
               .Build();
            }
            else
            {
                trigger = (ISimpleTrigger)TriggerBuilder.Create()
                 .StartNow() 
                 .Build();
            }
            _listsche.Add(scheduler);
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
        /// <summary>
        /// 新增作业，每隔{seconds}秒执行{times}次
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seconds">每隔{seconds}秒执行</param>
        /// <param name="times">秒执行{times}次</param>
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>
        public static void AddJob<T>(int seconds,int times,string jobName = "", string jobGroup = "") where T : IJob
        {
            jobName = string.IsNullOrEmpty(jobName) ? jobGroup : jobName;
            jobGroup = string.IsNullOrEmpty(jobGroup) ? jobName : jobGroup;
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            IJobDetail job = JobBuilder.Create<T>().Build();
            ITrigger trigger = null;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobGroup))
            {
                trigger = (ISimpleTrigger)TriggerBuilder.Create()
               .WithIdentity($"trigger_{jobName}", $"trigger_{jobGroup}")
               .StartNow()
               .WithSimpleSchedule(x => x.WithIntervalInSeconds(seconds).WithRepeatCount(times))
               .ForJob(jobName, jobGroup) //通过JobKey识别作业
               .Build();
            }
            else
            {
                trigger = (ISimpleTrigger)TriggerBuilder.Create()
                 .StartNow()
                 .Build();
            }
            _listsche.Add(scheduler);
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
        /// <summary>
        ///新增作业， 指定【开始时间】后，每隔{hours}小时重复一次
        /// </summary>
        /// <typeparam name="T"></typeparam>  
        /// <param name="startTime">指定开始时间                 </param>
        /// <param name="hours">将在未来hours小时内触发一次：</param>
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>
        public static void AddJob<T>(DateTime startTime,int hours, string jobName = "", string jobGroup = "") where T : IJob
        {
            jobName = string.IsNullOrEmpty(jobName) ? jobGroup : jobName;
            jobGroup = string.IsNullOrEmpty(jobGroup) ? jobName : jobGroup;
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            IJobDetail job = JobBuilder.Create<T>().Build();
            ITrigger trigger = null;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobGroup))
            {
                trigger = TriggerBuilder.Create()
                .WithIdentity($"trigger_{jobName}", $"trigger_{jobGroup}")// 由于未指定组，因此“trigger5”将位于默认分组中
                .StartAt(startTime)             
                .WithSimpleSchedule(x => x
                     .WithIntervalInHours(hours)//执行间隔多少小时
                     .RepeatForever())
                .Build();
            }
            else
            {
                trigger = TriggerBuilder.Create()
                .StartAt(startTime)
                .WithSimpleSchedule(x => x
                     .WithIntervalInHours(hours)//执行间隔多少小时
                     .RepeatForever())
                .Build();
            }
            _listsche.Add(scheduler);
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }
        /// <summary>
        /// 指定时间执行任务
        /// </summary>
        /// <typeparam name="T">任务类，必须实现IJob接口</typeparam>
        /// <param name="cronExpression">cron表达式，即指定时间点的表达式</param>
        /// <param name="jobName">作业名字</param>
        /// <param name="jobGroup">组名称</param>
        public static void AddJobByCron<T>(string cronExpression, string jobName = "", string jobGroup = "") where T : IJob
        {
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            jobName = string.IsNullOrEmpty(jobName) ? jobGroup : jobName;
            jobGroup = string.IsNullOrEmpty(jobGroup) ? jobName : jobGroup;
            IJobDetail job = JobBuilder.Create<T>().Build();

            ICronTrigger trigger;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrEmpty(jobGroup))
            {
                trigger = (ICronTrigger)TriggerBuilder.Create()
              .WithIdentity($"trigger_{jobName}", $"trigger_{jobGroup}")
               .StartNow()
               .WithSimpleSchedule()
               .ForJob(jobName, jobGroup) //通过JobKey识别作业
               .Build();
            }
            else
            {
               trigger = (ICronTrigger)TriggerBuilder.Create()            
              .WithCronSchedule(cronExpression)              
               .StartNow()             
               .Build();
            }
            _listsche.Add(scheduler);
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();

            //Thread.Sleep(TimeSpan.FromDays(2));
            //scheduler.Shutdown();
        }               

        /// <summary>
        /// 关闭所有工厂调度任务 
        /// </summary>
        public static void ShutdownJobs()
        {
            try
            {
                if (_listsche.Count > 0)
                {
                    foreach (var sched in _listsche)
                    {
                        if (!sched.IsShutdown)
                        {
                            logger.Info(string.Format("关闭任务调度：{0}", sched.SchedulerName));
                            sched.Shutdown();
                        }
                    }
                    _listsche.Clear();
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("关闭任务调度，出错：{0}", e), e);
            }
        }

    }
}
