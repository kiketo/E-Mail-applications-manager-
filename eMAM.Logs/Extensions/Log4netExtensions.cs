﻿using log4net.Appender;
using Microsoft.Extensions.Logging;

namespace eMAM.Logs.Extensions
{
    public static class Log4netExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, IAppender[] appenders)
        {
            factory.AddProvider(new Log4NetProvider(appenders));
            return factory;
        }
    }
}
