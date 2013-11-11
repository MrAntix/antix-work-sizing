using System;
using System.Linq;
using System.Threading.Tasks;
using Antix.Logging;
using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services
{
    public class TeamService :
        ITeamService
    {
        readonly ITeamDataService _dataService;
        readonly ILogAdapter _logger;

        public TeamService(
            ITeamDataService dataService, 
            ILogAdapter logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        async Task<TeamModel> ITeamService
            .Connect(string teamId, TeamMemberModel member)
        {
            if (member == null) throw new ArgumentNullException("member");
            if (teamId == null) throw new ArgumentNullException("teamId");

            var team = await _dataService.TryGetById(teamId)
                       ?? new TeamModel();

            team.Members = team.Members
                               .AddByName(member)
                               .ToArray();

            _logger.Information(m=>m("Connected '{0}'", member));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService
            .TryDisconnect(string teamId, string memberId)
        {
            if (memberId == null) throw new ArgumentNullException("memberId");
            if (teamId == null) throw new ArgumentNullException("teamId");

            var team = await _dataService.TryGetById(teamId);
            if (team == null) return null;

            team.Members = team.Members
                               .NotById(memberId)
                               .ToArray();

            team.Story.Votes = team.Story.Votes
                                   .NotByOwnerId(memberId)
                                   .ToArray();

            _logger.Information(m => m("Disconnected '{0}'", memberId));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService
            .SetStory(string teamId, string memberId, string title)
        {
            var team = await GetTeam(teamId);

            AssertIsStoryOwner(team, memberId);

            team.Story.OwnerId = memberId;
            team.Story.Title = title;

            _logger.Information(m => m("{0} set title '{1}'", memberId, title));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService
            .OpenVoting(string teamId, string memberId)
        {
            var team = await GetTeam(teamId);

            AssertIsStoryOwner(team, memberId);

            team.Story.VotingIsOpen = true;

            _logger.Information(m => m("{0} opened voting", memberId));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService
            .CloseVoting(string teamId, string memberId)
        {
            var team = await GetTeam(teamId);

            AssertIsStoryOwner(team, memberId);

            team.Story.VotingIsOpen = false;

            _logger.Information(m => m("{0} closed voting", memberId));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService.Vote(
            string teamId, string memberId, int points)
        {
            var team = await GetTeam(teamId);

            AssertIsMember(team, memberId);

            if (!team.Story.VotingIsOpen)
                throw new VotingIsClosedException();

            team.Story.Votes = team.Story.Votes
                                   .AddReplaceByOwnerId(
                                       new VoteModel
                                           {
                                               OwnerId = memberId,
                                               Points = points
                                           })
                                   .ToArray();

            _logger.Information(m => m("{0} voted {1} points", memberId, points));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService
            .ClearVotes(string teamId, string memberId)
        {
            var team = await GetTeam(teamId);

            AssertIsMember(team, memberId);

            team.Story.Votes = new VoteModel[] {};

            _logger.Information(m => m("{0} cleared votes", memberId));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> ITeamService
            .TryUpdateMember(
            string teamId, string memberId,
            string targetMemberId, Action<TeamMemberModel> action)
        {
            var team = await GetTeam(teamId);

            if (memberId == targetMemberId)
                AssertIsMember(team, memberId);
            else
                AssertIsStoryOwner(team, memberId);

            team.Members = team.Members
                               .UpdateById(targetMemberId, action)
                               .ToArray();

            _logger.Information(m => m("{0} updated {1}", memberId, targetMemberId));

            return await _dataService.Update(team);
        }

        async Task<TeamModel> GetTeam(string teamId)
        {
            if (teamId == null) throw new ArgumentNullException("teamId");

            var team = await _dataService.TryGetById(teamId);
            if (team == null)
                throw new TeamNotfoundException(teamId);

            return team;
        }

        static void AssertIsMember(TeamModel team, string memberId)
        {
            if (memberId == null) throw new ArgumentNullException("memberId");

            if (!team.Members.ExistsById(memberId))
                throw new TeamMemberNotFoundException(memberId, team.Id);
        }

        static void AssertIsStoryOwner(TeamModel team, string memberId)
        {
            AssertIsMember(team, memberId);

            if (team.Story.OwnerId != null
                && !team.Story.OwnerId.Equals(memberId))
                throw new RequiresOwnerPermissionException(memberId, team.Story.OwnerId);
        }
    }
}