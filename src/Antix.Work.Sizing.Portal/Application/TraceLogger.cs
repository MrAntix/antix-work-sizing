using System;
using System.Diagnostics;

using Antix.Logging;

namespace Antix.Work.Sizing.Portal
{
    public class TraceLogger : ILogAdapter
    {
        public void Log(
            LogLevel logLevel,
            IFormatProvider formatProvider,
            Func<LogMessageDelegate, string> getMessage,
            Exception ex)
        {
            if (Trace.Listeners.Count == 0) return;

            // if (logLevel <= LogLevel.Information) return;

            var message = LoggerHelper
                .GetMessageFunc(formatProvider, getMessage)();

            Trace.Write(
                string.Format("{0} {1}\r\n{2}", DateTimeOffset.UtcNow, message, ex),
                logLevel.ToString()
                );
            Trace.Close();
        }
    }
}