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
            if (service == null) throw new ArgumentNullException("service");
            if (memberId == null) throw new ArgumentNullException("memberId");
            if (title == null) throw new ArgumentNullException("title");

            var teamId = await service.TryGetTeamIdByMemberId(memberId);
            if (teamId != null)
            {
                return await service.LockStory(
                    teamId, memberId,
                    title);
            }

            throw new TeamNotfoundException();
        }

        public static async Task<TeamModel> UnlockStory(
            this ITeamService service,
            string memberId)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (memberId == null) throw new ArgumentNullException("memberId");

            var teamId = await service.TryGetTeamIdByMemberId(memberId);
            if (teamId != null)
            {
                return await service.UnlockStory(
                    teamId, memberId);
            }

            throw new TeamNotfoundException();
        }
    }
}