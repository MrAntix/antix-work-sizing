using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Services.InMemory
{
    public class TeamDataService : ITeamDataService
    {
        async Task<TeamModel> ITeamDataService.TryGetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            return MemoryCache.Default.Contains(id)
                       ? (TeamModel) MemoryCache.Default.Get(id)
                       : default(TeamModel);
        }

        async Task<TeamModel> ITeamDataService.Update(TeamModel data)
        {
            if (data.Id == null)
            {
                while (await Exists(
                    data.Id = Guid.NewGuid().ToString("N").Substring(0, 6)
                                 ))
                {
                }
            }

            AddOrUpdate(data.Id, data);

            return data;
        }

        async Task ITeamDataService.TryAddIndex(string indexKey, string value)
        {
            AddOrUpdate(indexKey, value);
        }

        async Task<string> ITeamDataService.TryGetIndex(string indexKey)
        {
            if (IsValidIndexKey(indexKey)
                && MemoryCache.Default.Contains(indexKey))
                return (string) MemoryCache.Default.Get(indexKey);

            return null;
        }

        async Task<string> ITeamDataService.TryRemoveIndex(string indexKey)
        {
            if (IsValidIndexKey(indexKey)
                && MemoryCache.Default.Contains(indexKey))
                return (string) MemoryCache.Default.Remove(indexKey);

            return null;
        }

        static void AddOrUpdate(string key, object value)
        {
            MemoryCache.Default.Set(
                key, value,
                DateTimeOffset.UtcNow.AddSeconds(InMemorySettings.Default.TeamTimeoutSeconds));
        }

        static bool IsValidIndexKey(string indexKey)
        {
            return !string.IsNullOrWhiteSpace(indexKey);
        }

        static async Task<bool> Exists(string id)
        {
            return !string.IsNullOrWhiteSpace(id)
                   && MemoryCache.Default.Contains(id);
        }
    }
}