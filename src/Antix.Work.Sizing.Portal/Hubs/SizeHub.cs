using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antix.Work.Sizing.Portal.Models;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;
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

        public async Task<User> Connect(User user)
        {
            var team = await _teamService.Connect(
                user.TeamId, new TeamMemberModel
                    {
                        Id = Context.ConnectionId,
                        Name = user.Name,
                        IsObserver = user.IsObserver
                    });

            var teamMember = team.Members.ById(Context.ConnectionId);

            await Groups.Add(Context.ConnectionId, team.Id);

            await Clients
                .OthersInGroup(team.Id)
                .teamUpdate(MapToClient(team));

            user.Name = teamMember.Name;
            user.TeamId = team.Id;

            return user;
        }

        static Team MapToClient(TeamModel item)
        {
            return new Team
                {
                    Id = item.Id,
                    Members = MapToClient(item.Members)
                };
        }

        static User MapToClient(TeamMemberModel item)
        {
            return new User
                {
                };
        }

        static User[] MapToClient(IEnumerable<TeamMemberModel> items)
        {
            return items.Select(MapToClient).ToArray();
        }
    }
}