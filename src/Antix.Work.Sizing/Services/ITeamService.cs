using System;
using System.Threading.Tasks;
using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services
{
    public interface ITeamService
    {
        Task<TeamModel> Connect(string teamId, TeamMemberModel teamMember);
        Task<TeamModel> TryDisconnect(string teamMemberId);

        Task<TeamModel> SetStory(string teamId, string memberId, string story);

        Task<TeamModel> OpenVoting(string teamId, string memberId);
        Task<TeamModel> CloseVoting(string teamId, string memberId);
        Task<TeamModel> Vote(string teamId, string memberId, int points);
        Task<TeamModel> ClearVotes(string teamId, string memberId);

        Task<TeamModel> TryUpdateMember(string teamId, string memberId, string targetMemberId,
                                        Action<TeamMemberModel> action);

    }
}