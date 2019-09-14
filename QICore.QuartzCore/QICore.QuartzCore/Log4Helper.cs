using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QICore.QuartzCore
{
    public class Log4Helper
    {
        /*
           loggerName:对应配置文件
           <logger name="RollingLogFileAppender">
          <level value="ALL" />
          <appender-ref ref="RollingFileDebug" />
          <appender-ref ref="RollingFileInfo" />
          <appender-ref ref="RollingFileWarn" />
          <appender-ref ref="RollingFileError" />
        </logger>
         */
        private static string loggerName = "RollingLogFileAppender";
        private static ILoggerRepository _loggerRepository;

        private static ILoggerRepository LoggerRepository
        {
            get
            {
                if (_loggerRepository != null)
                {
                    return _loggerRepository;
                }
                _loggerRepository = LogManager.CreateRepository(nameof(Log4Helper));
                XmlConfigurator.ConfigureAndWatch(_loggerRepository, new FileInfo("log4net.config"));
                return _loggerRepository;
            }
        }

        public static ILog GetLogger<T>(T t)
        {
            return LogManager.GetLogger(LoggerRepository.Name, t.GetType());
        }

        public static ILog GetLogger(object obj)
        {
            return LogManager.GetLogger(LoggerRepository.Name, loggerName);
        }

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(LoggerRepository.Name, loggerName);
         
        }

        public static ILog GetLogger()
        {
            return LogManager.GetLogger(LoggerRepository.Name, loggerName);
        }
    }
    public static class LogExtension
    {
        public static void Error(ILog logger,object message,bool showConsole=false)
        {
            logger.Error(message);
        }      
    }

}
