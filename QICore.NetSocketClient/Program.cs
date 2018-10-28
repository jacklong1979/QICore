using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QICore.NetSocketClient.Common;

namespace QICore.NetSocketClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "通信客户端Client";//设置窗口标题
            var host = "127.0.0.1";
            var port = 3400;

            Task.Run(async () =>
            {
                await TcpSocketClient.StartClientAsync(host, port);
            });


            //for (int batchIndex = 0; batchIndex < 60; batchIndex++)
            //{
            //    Task.Run(async () =>
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            await TcpSocketClient.StartClientAsync(host, port);
            //        }
            //    });
            //}
            Console.WriteLine("Hello World!");
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
