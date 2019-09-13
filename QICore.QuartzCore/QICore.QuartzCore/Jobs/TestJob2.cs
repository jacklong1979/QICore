using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QICore.QuartzCore.Jobs
{
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
}
