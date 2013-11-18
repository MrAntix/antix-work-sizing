using System;
using System.Threading.Tasks;

using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services
{
    public interface ITeamService
    {
        Task<TeamModel> Connect(string teamId, TeamMemberModel teamMember);
        Task<TeamModel> TryDisconnect(string teamId, string teamMemberId);

        Task<TeamModel> TryGetTeamByMemberId(string memberId);
        Task<string> TryGetTeamIdByMemberId(string memberId);

        Task<TeamModel> LockStory(string teamId, string memberId, string story);
        Task<TeamModel> UnlockStory(string teamId, string memberId);

        Task<TeamModel> OpenVoting(string teamId, string memberId);
        Task<TeamModel> CloseVoting(string teamId, string memberId);
        Task<TeamModel> Vote(string teamId, string memberId, int points);
        Task<TeamModel> ClearVotes(string teamId, string memberId);

        Task<TeamModel> TryUpdateMember(
            string teamId, string memberId,
            string targetMemberId, Action<TeamMemberModel> action);

        Task<TeamModel> TryUpdateMemberByName(
            string teamId, string memberId,
            string targetMemberName, Action<TeamMemberModel> action);

        Task<TeamModel> DemoToggle(string teamId, string memberId);
        Task<TeamModel> DemoStage(string teamId, string memberId, DemoStage stage);
    }
}