using System;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Antix.Logging;
using Antix.Work.Sizing.Portal.Hubs;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;
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

        public class TraceLogger : ILogAdapter
        {
            public void Log(
                LogLevel logLevel,
                IFormatProvider formatProvider,
                Func<LogMessageDelegate, string> getMessage,
                Exception ex)
            {
                if (Trace.Listeners.Count == 0) return;

                if (logLevel <= LogLevel.Information) return;

                var message = LoggerHelper
                    .GetMessageFunc(formatProvider, getMessage)();

                Trace.Write(
                    string.Format("{0}\r\n{1}", message, ex),
                    logLevel.ToString()
                    );
                Trace.Close();
            }
        }

        public class TeamDataService : ITeamDataService
        {
            public async Task<TeamModel> TryGetById(string id)
            {
                if (string.IsNullOrWhiteSpace(id)) return null;

                return MemoryCache.Default.Contains(id)
                           ? (TeamModel) MemoryCache.Default.Get(id)
                           : default(TeamModel);
            }

            public async Task<TeamModel> Update(TeamModel data)
            {
                if (data.Id == null)
                {
                    while (await Exists(
                        data.Id = Guid.NewGuid().ToString("N").Substring(0, 6)
                                     ))
                    {
                    }
                }

                MemoryCache.Default.Add(
                    data.Id, data, DateTimeOffset.UtcNow.AddSeconds(1440));

                return data;
            }

            static async Task<bool> Exists(string id)
            {
                return !string.IsNullOrWhiteSpace(id)
                       && MemoryCache.Default.Contains(id);
            }
        }
    }
}