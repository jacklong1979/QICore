using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.NQuartz.IDao
{
    public class QuartzJob: IJob
    {
        /// <summary>
        /// 作业的入口
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            Console.Out.WriteLineAsync("作业......");
            return Task.CompletedTask;
        }
    }
}
