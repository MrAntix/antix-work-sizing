using System;
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
        readonly IDemoService _demoService;

        public SizeHub(
            ITeamService teamService,
            Func<Func<TeamModel, Task>, IDemoService> getDemoService)
        {
            _teamService = teamService;
            _demoService = getDemoService(
                async team => await Clients
                                        .Group(team.Id)
                                        .teamUpdate(team.ToTeam()));
        }

        public async Task<Session> Connect(User user)
        {
            if (user.Id != Context.ConnectionId)
                await _teamService.TryDisconnect(user.TeamId, user.Id);

            var team = await _teamService
                                 .Connect(
                                     user.TeamId, user.ToTeamMemberModel(Context.ConnectionId));

            await Groups.Add(Context.ConnectionId, team.Id);

            await Clients
                .OthersInGroup(team.Id)
                .teamUpdate(team.ToTeam());

            return team.ToSession(Context.ConnectionId);
        }

        public override async Task OnDisconnected()
        {
            var team = await _teamService
                                 .TryDisconnect(Context.ConnectionId);

            if (team != null)
                await Clients
                    .OthersInGroup(team.Id)
                    .teamUpdate(team.ToTeam());

            await base.OnDisconnected();
        }

        public async Task<Session> UpdateCurrentUserName(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            var team = await _teamService
                                 .TryUpdateCurrentMember(
                                     Context.ConnectionId,
                                     m => m.Name = name.Trim());

            if (team == null) throw new TeamNotfoundException();

            await Clients
                .OthersInGroup(team.Id)
                .teamUpdate(team.ToTeam());

            return team.ToSession(Context.ConnectionId);
        }

        public async Task UpdateUserIsObserver(string name, bool value)
        {
            if (name == null) throw new ArgumentNullException("name");

            var team = await _teamService
                                 .TryUpdateMemberByName(
                                     Context.ConnectionId,
                                     name.Trim(),
                                     m => m.IsObserver = value);

            if (team == null) throw new TeamNotfoundException();

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task UpdateCurrentUserVote(int points)
        {
            var team = await _teamService
                                 .Vote(Context.ConnectionId, points);

            if (team == null) throw new TeamNotfoundException();

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task LockCurrentStory(string title)
        {
            if (title == null) throw new ArgumentNullException("title");

            var team = await _teamService
                                 .LockStory(Context.ConnectionId, title);

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task ReleaseCurrentStory()
        {
            var team = await _teamService
                                 .UnlockStory(Context.ConnectionId);

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task OpenVoting(VoteSchedule schedule)
        {

            var team
                = await _teamService
                            .OpenVoting(
                                Context.ConnectionId,
                                schedule.ToVoteScheduleModel()
                            );

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task CloseVoting()
        {
            var team = await _teamService
                                 .CloseVoting(Context.ConnectionId);

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task ClearVotes()
        {
            var team = await _teamService
                                 .ClearVotes(Context.ConnectionId);

            await Clients
                .Group(team.Id)
                .teamUpdate(team.ToTeam());
        }

        public async Task DemoStart()
        {
            await _demoService.Start(Context.ConnectionId);
        }

        public async Task DemoNext()
        {
            await _demoService.Next(Context.ConnectionId);
        }

        public async Task DemoCancel()
        {
            await _demoService.Cancel(Context.ConnectionId);
        }
    }
}