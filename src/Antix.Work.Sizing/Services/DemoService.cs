using System;
using System.Linq;
using System.Threading.Tasks;

using Antix.Logging;
using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services
{
    public class DemoService :
        IDemoService
    {
        readonly ITeamDataService _teamDataService;
        readonly Log.Delegate _logger;
        readonly Func<TeamModel, Task> _broadcast;

        string _bobId;
        string _garyId;

        public DemoService(
            ITeamDataService teamDataService,
            Log.Delegate logger,
            Func<TeamModel, Task> broadcast
            )
        {
            _teamDataService = teamDataService;
            _logger = logger;
            _broadcast = broadcast;
        }

        async Task IDemoService.Start(string memberId)
        {
            var team = await GetTeam(memberId);

            Schedule.Create(_logger)
                    .At(0, async () => _bobId = await AddUser(team, "Bob (developer)"), "Add Bob")
                    .At(500, async () => _bobId = await AddUser(team, "Edward (project manager)"), "Add Edward")
                    .At(1500, async () => _bobId = await AddUser(team, "Patrick (tester)"), "Add Patrick")
                    .At(2000, async () => _bobId = await AddUser(team, "Eugine (stakeholder)"), "Add Eugine")
                    .At(3500, async () => _garyId = await AddUser(team, "Gary (developer)"), "Add Gary");
        }

        async Task<string> AddUser(TeamModel team, string name)
        {
            var id = Guid.NewGuid().ToString();
            team.Members = team.Members.AddByName(new TeamMemberModel
                {
                    IsDemo = true,
                    Id = id,
                    Name = name
                }).ToArray();

            await _teamDataService.Update(team);

            await _broadcast(team);

            return id;
        }

        Task IDemoService.Next(string memberId)
        {
            throw new NotImplementedException();
        }

        Task IDemoService.Cancel(string memberId)
        {
            throw new NotImplementedException();
        }

        async Task<TeamModel> GetTeam(string memberId)
        {
            var teamId = await _teamDataService.TryGetIndex(memberId);
            return await _teamDataService.TryGetById(teamId);
        }
    }
}