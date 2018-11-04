using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.NetSocketServer.Common;
using QICore.NetSocketServer.Models;

namespace QICore.NetSocketServer.Controllers
{
    public class BaseController<T> : Controller
    {
       
        /// <summary>
        /// Socket配置
        /// </summary>
        public SocketSetting SocketSetting;
        /// <summary>
        /// NLog日志
        /// </summary>
        public ILogger logger;
        public BaseController()
        {
            var httpContextAccessor = (IHttpContextAccessor)ServiceLocator.Instance.GetService(typeof(IHttpContextAccessor));
            SocketSetting = ((IOptions<SocketSetting>)httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IOptions<SocketSetting>))).Value;
            var loggerFactory = (ILoggerFactory)httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ILoggerFactory));
            logger = loggerFactory.CreateLogger<T>();
        }
    }
}
