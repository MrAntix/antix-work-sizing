using System;
using System.Threading.Tasks;

using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services
{
    public static class TeamServiceExtensions
    {
        public static async Task<TeamModel> TryDisconnect(
            this ITeamService service,
            string memberId)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (memberId == null) throw new ArgumentNullException("memberId");

            var teamId = await service.TryGetTeamIdByMemberId(memberId);
            if (teamId != null)
            {
                return await service.TryDisconnect(teamId, memberId);
            }

            return null;
        }

        public static async Task<TeamModel> TryUpdateCurrentMember(
            this ITeamService service,
            string memberId, Action<TeamMemberModel> action)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (memberId == null) throw new ArgumentNullException("memberId");
            if (action == null) throw new ArgumentNullException("action");

            var teamId = await service.TryGetTeamIdByMemberId(memberId);
            if (teamId != null)
            {
                return await service.TryUpdateMember(
                    teamId, memberId,
                    memberId,
                    action);
            }

            return null;
        }

        public static async Task<TeamModel> TryUpdateMemberByName(
            this ITeamService service,
            string memberId,
            string targetMemberName,
            Action<TeamMemberModel> action)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (memberId == null) throw new ArgumentNullException("memberId");
            if (targetMemberName == null) throw new ArgumentNullException("targetMemberName");
            if (action == null) throw new ArgumentNullException("action");

            var teamId = await service.TryGetTeamIdByMemberId(memberId);
            if (teamId != null)
            {
                return await service.TryUpdateMemberByName(
                    teamId, memberId,
                    targetMemberName,
                    action);
            }

            return null;
        }

        public static async Task<TeamModel> LockStory(
            this ITeamService service,
            string memberId, string title)
        {
            if (title == null) throw new ArgumentNullException("title");

            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service.LockStory(
                teamId, memberId,
                title);
        }

        public static async Task<TeamModel> UnlockStory(
            this ITeamService service,
            string memberId)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service.UnlockStory(
                teamId, memberId);
        }

        public static async Task<TeamModel> OpenVoting(
            this ITeamService service,
            string memberId)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service
                             .OpenVoting(teamId, memberId);
        }

        public static async Task<TeamModel> Vote(
            this ITeamService service,
            string memberId, int points)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service
                             .Vote(teamId, memberId, points);
        }

        public static async Task<TeamModel> CloseVoting(
            this ITeamService service,
            string memberId)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service
                             .CloseVoting(teamId, memberId);
        }

        public static async Task<TeamModel> ClearVotes(
            this ITeamService service,
            string memberId)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service
                             .ClearVotes(teamId, memberId);
        }

        public static async Task<TeamModel> DemoToggle(
            this ITeamService service,
            string memberId)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service
                             .DemoToggle(teamId, memberId);
        }

        public static async Task<TeamModel> DemoStage(
            this ITeamService service,
            string memberId, DemoStage stage)
        {
            var teamId = await service.GetTeamIdByMemberId(memberId);
            return await service
                             .DemoStage(teamId, memberId, stage);
        }

        public static async Task<string> GetTeamIdByMemberId(
            this ITeamService service,
            string memberId)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (memberId == null) throw new ArgumentNullException("memberId");

            var teamId = await service.TryGetTeamIdByMemberId(memberId);
            if (teamId != null)
            {
                return teamId;
            }

            throw new TeamNotfoundException();
        }
    }
}