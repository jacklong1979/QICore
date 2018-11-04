using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.NetSocketServer.Common;
using QICore.NetSocketServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.NetSocketServer.Controllers
{
    public class MainService : Controller, Microsoft.Extensions.Hosting.IHostedService
    {
        /// <summary>
        /// Socket配置
        /// </summary>
        public SocketSetting SocketSetting;
        /// <summary>
        /// 日志
        /// </summary>
        public ILogger logger;
        public MainService( IOptions<SocketSetting> socket, ILoggerFactory loggerFactory)
        {          
            SocketSetting = socket.Value;
            logger = loggerFactory.CreateLogger<MainService>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(" **************** MainService 已启动 **************** ");
            return Task.Run(() =>
            {
                TcpServer.StartServer("127.0.0.1", 3400);
                TcpServer.ListenAsync(); //开始监听
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(DateTime.Now + " StopAsync");
            throw new NotImplementedException();
        }
    }
}
