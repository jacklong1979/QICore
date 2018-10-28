using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QICore.NetSocketServer.Common;

namespace QICore.NetSocketServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //启动服务器
            Console.Title = "通信服务端Server";//设置窗口标题
            TcpServer.StartServer("127.0.0.1", 3400);
            TcpServer.ListenAsync(); //开始监听
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
              // .AddJsonFile("host.json", optional: true)
               .Build();

            var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseConfiguration(config)
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseStartup<Startup>()
                        .Build();
            host.Run();
            // BuildWebHost(args).Run();
           
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
