using System;
using System.Collections.Generic;
using System.Linq;
using Antix.Work.Sizing.Properties;

namespace Antix.Work.Sizing.Services.Models
{
    public static class ModelExtensions
    {
        public static IEnumerable<T> Add<T>(
            this IEnumerable<T> items, T item)
            where T : class
        {
            if (items == null) throw new ArgumentNullException("items");
            if (item == null) throw new ArgumentNullException("item");

            return items.Concat(new[] {item});
        }

        public static IEnumerable<T> AddByName<T>(
            this IEnumerable<T> items, T item)
            where T : class, IHasIdAndName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (item == null) throw new ArgumentNullException("item");

            var itemsArray = (items as T[] ?? items.ToArray())
                .NotById(item.Id)
                .ToArray();

            ProcessName(itemsArray, item);

            return itemsArray.Add(item);
        }

        public static void ProcessName<T>(
            this IEnumerable<T> items, T item)
            where T : class, IHasIdAndName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (item == null) throw new ArgumentNullException("item");

            if (string.IsNullOrWhiteSpace(item.Name))
                item.Name = Resources.InvalidMemberName;

            var itemsArray = items as T[] ?? items.ToArray();

            var name = item.Name;

            var count = 1;
            while (itemsArray.Any(i =>
                                  !CompareIds(i.Id, item.Id)
                                  && CompareNames(i.Name, name)))
            {
                name = string.Concat(item.Name, " (", ++count, ")");
            }

            item.Name = name;
        }

        public static IEnumerable<T> UpdateById<T>(
            this IEnumerable<T> items,
            string memberId,
            Action<T> action)
            where T : class, IHasIdAndName
        {
            var itemsArray = items as T[] ?? items.ToArray();
            var item = itemsArray.GetById(memberId);

            action(item);

            itemsArray.ProcessName(item);

            return itemsArray;
        }

        public static IEnumerable<T> NotById<T>(
            this IEnumerable<T> items,
            string id)
            where T : class, IHasId
        {
            if (items == null) throw new ArgumentNullException("items");

            return items.Where(
                i => !CompareIds(i.Id, id));
        }

        public static IEnumerable<T> NotByOwnerId<T>(
            this IEnumerable<T> items,
            string id)
            where T : class, IHasOwnerId
        {
            if (items == null) throw new ArgumentNullException("items");

            return items.Where(i => ! CompareIds(i.OwnerId, id));
        }

        public static IEnumerable<T> AddReplaceByOwnerId<T>(
            this IEnumerable<T> items,
            T item)
            where T : class, IHasOwnerId
        {
            if (items == null) throw new ArgumentNullException("items");

            return items
                .NotByOwnerId(item.OwnerId)
                .Add(item);
        }

        public static T GetByName<T>(
            this IEnumerable<T> items, string value)
            where T : class, IHasName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (value == null) throw new ArgumentNullException("value");

            return items.Single(
                i => i.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static T TryGetByName<T>(
            this IEnumerable<T> items, string value)
            where T : class, IHasName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (value == null) throw new ArgumentNullException("value");

            return items.SingleOrDefault(
                i => i.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public static T GetById<T>(
            this IEnumerable<T> items, string id)
            where T : class, IHasId
        {
            if (items == null) throw new ArgumentNullException("items");
            if (id == null) throw new ArgumentNullException("id");

            return items.Single(i => CompareIds(i.Id, id));
        }

        public static T TryGetById<T>(
            this IEnumerable<T> items, string id)
            where T : class, IHasId
        {
            if (items == null) throw new ArgumentNullException("items");
            if (id == null) throw new ArgumentNullException("id");

            return items.SingleOrDefault(i => CompareIds(i.Id, id));
        }

        public static bool ExistsById<T>(
            this IEnumerable<T> items, string value)
            where T : class, IHasId
        {
            if (items == null) throw new ArgumentNullException("items");
            if (value == null) throw new ArgumentNullException("value");

            return items.Any(i => CompareIds(i.Id, value));
        }

        public static bool ExistsByName<T>(
            this IEnumerable<T> items, string value)
            where T : class, IHasName
        {
            if (items == null) throw new ArgumentNullException("items");
            if (value == null) throw new ArgumentNullException("value");

            return items.Any(i => CompareNames(i.Name, value));
        }

        public static string TryGetNameById<T>(
            this IEnumerable<T> items, string id)
            where T : IHasIdAndName
        {
            return items
                .Where(m => CompareIds(m.Id, id))
                .Select(m => m.Name)
                .SingleOrDefault();
        }

        static readonly Func<string, string, bool> CompareIds =
            (value1, value2) => string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase);

        static readonly Func<string, string, bool> CompareNames =
            (value1, value2) => string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase);

        public static VoteResultModel[] GetVoteResults(
            this TeamModel team)
        {
            var votingMembersCount = team.Members.Count(m => !m.IsObserver);
            var votesCount = team.Story.Votes.Count();

            if (team.Story.VotingIsOpen
                && votingMembersCount != votesCount) return null;

            return (from v in team.Story.Votes
                    group v by v.Points
                    into g
                    orderby g.Key
                    select new VoteResultModel(
                        g.Key,
                        100*g.Count()/(decimal) votingMembersCount))
                .ToArray();
        }

        public static int GetMode(this IEnumerable<int> results)
        {
            return results.GroupBy(r => r)
                          .OrderByDescending(g => g.Count())
                          .Select(g => g.Key)
                          .FirstOrDefault();
        }

    }
}