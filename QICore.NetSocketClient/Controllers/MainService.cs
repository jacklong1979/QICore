using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.NetSocketClient.Common;
using QICore.NetSocketClient.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.NetSocketClient.Controllers
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
                var host = "127.0.0.1";
                var port = 3400;
                Task.Run(async () =>
                {
                    SocketClient.Listen();
                    //await TcpSocketClient.StartClientAsync(host, port);
                });
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(DateTime.Now + " StopAsync");
            throw new NotImplementedException();
        }
    }
}
