using System.Threading.Tasks;
using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services
{
    public interface ITeamDataService
    {
        Task<TeamModel> TryGetById(string id);
        Task<TeamModel> Update(TeamModel team);

        Task TryAddIndex(string memberId, string id);
        Task<string> TryRemoveIndex(string memberId);
    }
}