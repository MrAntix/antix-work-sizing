//  by Anthony J. Johnston, antix.co.uk

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Antix.Work.Sizing.Services
{
    public class TeamService :
        ITeamService
    {
        readonly ITeamDataService _dataService;

        public TeamService(ITeamDataService dataService)
        {
            _dataService = dataService;
        }

        async Task<TeamModel> ITeamService
            .Connect(TeamMemberModel teamMember, string teamId)
        {
            if (teamMember == null) throw new ArgumentNullException("teamMember");
            if (teamId == null) throw new ArgumentNullException("teamId");

            var team = await _dataService.TryGetById(teamId)
                       ?? _dataService.Create();

            team.Members = team.Members.Add(teamMember);

            return team;
        }

        async Task<TeamModel> ITeamService
            .TryDisconnect(string teamMemberId, string teamId)
        {
            if (teamMemberId == null) throw new ArgumentNullException("teamMemberId");
            if (teamId == null) throw new ArgumentNullException("teamId");

            var team = await _dataService.TryGetById(teamId);
            if (team == null) return null;

            team.Members = team.Members.NotById(teamMemberId);
            team.StoryVotes = team.StoryVotes.NotByOwnerId(teamMemberId);

            return team;
        }
    }

    public interface ITeamService
    {
        Task<TeamModel> Connect(TeamMemberModel teamMember, string teamId);
        Task<TeamModel> TryDisconnect(string teamMemberId, string teamId);
    }

    public interface ITeamDataService
    {
        Task<TeamModel> TryGetById(string teamId);
        TeamModel Create();
    }

    public class TeamModel : IHasId
    {
        public TeamModel()
        {
            Members = new TeamMemberModel[] {};
            StoryVotes = new StoryVoteModel[] {};
        }

        public string Id { get; set; }

        public IEnumerable<TeamMemberModel> Members { get; set; }

        public string Story { get; set; }
        public string StoryOwnerId { get; set; }
        public IEnumerable<StoryVoteModel> StoryVotes { get; set; }
    }

    public class TeamMemberModel : IHasId, IHasName
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool IsObserver { get; set; }
    }

    public class StoryVoteModel : IHasOwnerId
    {
        public int Points { get; set; }
        public string OwnerId { get; set; }
    }

    public interface IHasName
    {
        string Name { get; set; }
    }

    public interface IHasId
    {
        string Id { get; }
    }

    public interface IHasOwnerId
    {
        string OwnerId { get; }
    }

    public static class ModelExtensions
    {
        public static IEnumerable<T> Add<T>(
            this IEnumerable<T> items, T item)
            where T : class, IHasName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (item == null) throw new ArgumentNullException("item");

            if (string.IsNullOrWhiteSpace(item.Name))
                item.Name = "Shy Member";

            var itemsArray = items as T[] ?? items.ToArray();

            var name = item.Name;

            var count = 1;
            while (itemsArray.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                name = string.Concat(item.Name, " (", ++count, ")");
            }

            return itemsArray.Concat(new[] {item});
        }

        public static IEnumerable<T> NotById<T>(
            this IEnumerable<T> items,
            string id)
            where T : IHasId
        {
            if (items == null) throw new ArgumentNullException("items");

            return items.Where(
                i => !i.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<T> NotByOwnerId<T>(
            this IEnumerable<T> items,
            string id)
            where T : IHasOwnerId
        {
            if (items == null) throw new ArgumentNullException("items");

            return items.Where(
                i => !i.OwnerId.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static T ByNameOrDefault<T>(
            this IEnumerable<T> items, string value)
            where T : class, IHasName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (value == null) throw new ArgumentNullException("value");

            return items.SingleOrDefault(
                i => i.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static T ByIdOrDefault<T>(
            this IEnumerable<T> items, string value)
            where T : class, IHasId
        {
            if (items == null) throw new ArgumentNullException("items");
            if (value == null) throw new ArgumentNullException("value");

            return items.SingleOrDefault(i => i.Id.Equals(value));
        }
    }
}