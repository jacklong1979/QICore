using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.QuartzCore
{
    #region IJobListener
    public class CustomJobListener : IJobListener
    {

        private readonly ILog logger = Log4Helper.GetLogger(typeof(CustomJobListener));
        public string Name => "CustomJobListener";
        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
                 logger.Info($"IJobListener [1]【Job 执行被否决】 {context.JobDetail.Description}");
            });
        }
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            var jobName = ((Quartz.Impl.Triggers.AbstractTrigger)((Quartz.Impl.JobExecutionContextImpl)context).Trigger).JobName;
            await Task.Run(() => {
                 logger.Info($"IJobListener [2]【Job 正在执行...】 {jobName}");
            });
        }
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken)
        {
            var jobName = ((Quartz.Impl.Triggers.AbstractTrigger)((Quartz.Impl.JobExecutionContextImpl)context).Trigger).JobName;

            await Task.Run(() => {
                 logger.Info($"IJobListener [3]【Job 已执行完成】 {jobName}");
            });
        }
    }
    #endregion
    #region ITriggerListener
    public class CustomTriggerListener : ITriggerListener
    {
        private readonly ILog logger = Log4Helper.GetLogger(typeof(CustomJobListener));
        public string Name => "CustomTriggerListener";

        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken)
        {
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                 logger.Info($"ITriggerListener [4]【触发完成】 {triggerTemp.FullJobName}");
            });
        }

        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken)
        {
            // (1)Trigger被激发 它关联的job即将被运行
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                 logger.Info($"ITriggerListener [5]【触发执行中】 {triggerTemp.FullJobName}");
            });
        }
        /**
    *  当Trigger错过被激发时执行,比如当前时间有很多触发器都需要执行，但是线程池中的有效线程都在工作，
    *  那么有的触发器就有可能超时，错过这一轮的触发。
    * Called by the Scheduler when a Trigger has misfired.
    */
        public async Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken)
        {
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                 logger.Info($"ITriggerListener [6]【不起作用】 {triggerTemp.FullJobName}");
            });
        }

        /// <summary>
        /// 要不要放弃job
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken)
        {
            //Trigger被激发 它关联的job即将被运行,先执行(1)，在执行(2) 如果返回TRUE 那么任务job会被终止
            var triggerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            await Task.Run(() =>
            {
                 logger.Info($"ITriggerListener [7]【终止作业执行】 {triggerTemp.FullJobName}");
            });
            return false;//false 才能继续执行
        }
    }
    #endregion
    #region ISchedulerListener
    public class CustomSchedulerListener : ISchedulerListener
    {
        private readonly ILog logger = Log4Helper.GetLogger(typeof(CustomJobListener));
        public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken)
        {
            var job = (Quartz.Impl.JobDetailImpl)jobDetail;
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [8]【增加Job】 {job.FullName}");
            });
        }

        public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [9]【删除Job】 {jobKey?.ToString()}");
            });
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Warn($"ISchedulerListener [10]【中断Job】 {jobKey?.ToString()}");
            });
        }

        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Warn($"ISchedulerListener [11]【暂停Job】 {jobKey?.ToString()}");
            });
        }

        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [12]【恢复ob】 {jobKey?.ToString()}");
            });
        }

        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken)
        {
            var trigerTemp = ((Quartz.Impl.Triggers.AbstractTrigger)trigger);
            return Task.Run(() =>
            {
                logger.Info($"ISchedulerListener [13]【计划Job】 {trigerTemp.FullJobName}({trigerTemp.FullName})");
            });
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [14]【暂停Job组】 {jobGroup}");
            });
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [15]【恢复Job组】 { jobGroup}");
            });
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [16]【未计划的Job】 { triggerKey.Name}");
            });
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Error($"ISchedulerListener [17]【调度程序错误】 { msg}");
            });
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [18]【调度待机模式】 ");
            });
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [19]【调度关闭】 ");
            });
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [20]【调度停止】 ");
            });
        }

        public Task SchedulerStarted(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [21]【调度已经开始】 ");
            });
        }

        public Task SchedulerStarting(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [22]【调度正在开始.....】 ");
            });
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [23]【调度正在清理数据.....】 ");
            });
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [24]【调度正在清理数据.....】 ");
            });
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [25]【TriggerPaused】 ");
            });
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [26]【TriggerResumed】 ");
            });
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [27]【TriggersPaused】 ");
            });
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                 logger.Info($"ISchedulerListener [28]【TriggersResumed】 ");
            });
        }
    }
    #endregion
}
