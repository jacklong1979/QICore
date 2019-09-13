using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.QuartzCore.Jobs
{
    public class Test3Job : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => {
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine($"{DateTime.Now}:Job3[{++Program.index}]");
                    Thread.Sleep(3000);
                }
                // _scheduler.Shutdown(true);
            });
        }
    }
}
