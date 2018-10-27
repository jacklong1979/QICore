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

            TcpServer.StartServer("127.0.0.1", 3400);
            TcpServer.Listen(); //开始聆听
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
