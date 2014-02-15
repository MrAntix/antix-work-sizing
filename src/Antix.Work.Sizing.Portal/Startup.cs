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
            var teamDataService = new TeamDataService();
            var teamService = new TeamService(teamDataService, ToTrace);

            GlobalHost.DependencyResolver
                      .Register(typeof (SizeHub),
                                () => new SizeHub(
                                          teamService,
                                          broadcast => new DemoService(teamDataService, null, broadcast))
                );

            app.MapSignalR(new HubConfiguration
                {
                    EnableDetailedErrors = true
                });
        }

        static readonly Log.Delegate ToTrace
            = l => (ex, f, a) =>
                {
                    if (Trace.Listeners.Count == 0) return;

                    // if (l <= Log.Level.Information) return;

                    var m = string.Format(f, a);

                    Trace.Write(
                        string.Format("{0} {1}\r\n{2}", DateTimeOffset.UtcNow, m, ex),
                        l.ToString()
                        );
                    Trace.Close();
                };
    }
}