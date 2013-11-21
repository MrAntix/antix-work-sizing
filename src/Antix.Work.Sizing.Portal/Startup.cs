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
                      .Register(typeof (SizeHub),
                                () => new SizeHub(
                                          teamService,
                                          broadcast => new DemoService(teamDataService, logger, broadcast)));

            app.MapSignalR(new HubConfiguration
                {
                    EnableDetailedErrors = true
                });
        }
    }
}