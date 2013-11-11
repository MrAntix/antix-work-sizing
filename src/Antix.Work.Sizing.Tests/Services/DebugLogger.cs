using System;
using System.Diagnostics;
using Antix.Logging;

namespace Antix.Work.Sizing.Tests.Services
{
    public class DebugLogger:ILogAdapter
    {
        public void Log(
            LogLevel logLevel, 
            IFormatProvider formatProvider, 
            Func<LogMessageDelegate, string> getMessage, Exception ex)
        {
            var message = LoggerHelper
                .GetMessageFunc(formatProvider, getMessage)();

            Debug.Write(
                string.Format("{0}\r\n{1}", message, ex),
                logLevel.ToString()
                );
        }
    }
}