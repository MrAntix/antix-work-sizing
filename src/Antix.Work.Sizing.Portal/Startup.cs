using System;
using System.Diagnostics;
using Antix.Logging;
using Antix.Work.Sizing.Portal.Hubs;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.InMemory;
using Microsoft.AspNet.SignalR;
using Owin;

namespace Antix.Work.Sizing.Portal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var logger = new TraceLogger();
            var teamDataService = new TeamDataService();
            var teamService = new TeamService(teamDataService, logger);

            GlobalHost.DependencyResolver
                      .Register(typeof (SizeHub), () => new SizeHub(teamService));


            app.MapSignalR(new HubConfiguration
                {
                    EnableDetailedErrors = true
                });
        }

        class TraceLogger : ILogAdapter
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
                    string.Format("{0}\r\n{1}", message, ex),
                    logLevel.ToString()
                    );
                Trace.Close();
            }
        }
    }
}