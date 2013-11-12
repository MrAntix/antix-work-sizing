using System.Threading.Tasks;
using Antix.Work.Sizing.Portal.Models;
using Antix.Work.Sizing.Services;
using Microsoft.AspNet.SignalR;

namespace Antix.Work.Sizing.Portal.Hubs
{
    public class SizeHub : Hub
    {
        readonly ITeamService _teamService;

        public SizeHub(
            ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<Session> Connect(User user)
        {
            if (user.Id != Context.ConnectionId)
                await _teamService.TryDisconnect(user.Id);

            var team = await _teamService
                                 .Connect(
                                     user.TeamId, Map.ToServer(user, Context.ConnectionId));

            await Groups.Add(Context.ConnectionId, team.Id);

            await Clients
                .OthersInGroup(team.Id)
                .teamUpdate(Map.ToClient(team));

            return Map.ToClient(team, Context.ConnectionId);
        }

        public override async Task OnDisconnected()
        {
            var team = await _teamService
                                 .TryDisconnect(Context.ConnectionId);
            if (team != null)
                await Clients
                    .OthersInGroup(team.Id)
                    .teamUpdate(Map.ToClient(team));

            await base.OnDisconnected();
        }
    }
}