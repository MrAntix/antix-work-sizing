using System.Collections.Generic;
using System.Linq;

using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Portal.Models
{
    public static class MappingExtensions
    {
        public static Team ToTeam(
            this TeamModel model)
        {
            var results = model.GetVoteResults().ToStoryPointResults();
            var result = results.OrderByDescending(r => r.Percentage).Select(r=>r.Value).FirstOrDefault();

            return new Team
                {
                    Id = model.Id,
                    Users = model.Members.ToTeamMembers(),
                    CurrentStory = model.Story.ToStory(model.Members),
                    CurrentStoryOwner = model.Members.TryGetNameById(model.Story.OwnerId),
                    CurrentStoryResult = result,
                    CurrentStoryResults = results
                };
        }

        public static User ToUser(
            this TeamModel model, string userId)
        {
            return model.Members.GetById(userId).ToUser(model.Id);
        }

        public static User ToUser(
            this TeamMemberModel model, string teamId)
        {
            return new User
                {
                    Id = model.Id,
                    Name = model.Name,
                    TeamId = teamId,
                    IsObserver = model.IsObserver
                };
        }

        public static TeamMemberModel ToTeamMemberModel(
            this User model, string id)
        {
            return new TeamMemberModel
                {
                    Id = id,
                    Name = model.Name,
                    IsObserver = model.IsObserver
                };
        }

        public static Session ToSession(this TeamModel team, string memberId)
        {
            return new Session
                {
                    Team = team.ToTeam(),
                    User = team.ToUser(memberId)
                };
        }

        public static TeamMember[] ToTeamMembers(this IEnumerable<TeamMemberModel> models)
        {
            return models.Select(ToTeamMember).ToArray();
        }

        public static TeamMember ToTeamMember(this TeamMemberModel model)
        {
            return new TeamMember
                {
                    Name = model.Name,
                    IsObserver = model.IsObserver
                };
        }

        public static Story ToStory(
            this StoryModel model,
            IEnumerable<TeamMemberModel> members)
        {
            return new Story
                {
                    Title = model.Title,
                    VotingOpen = model.VotingIsOpen,
                    Points = ToStoryPoints(model.Votes, members)
                };
        }

        public static StoryPoints[] ToStoryPoints(
            this IEnumerable<VoteModel> models,
            IEnumerable<TeamMemberModel> members)
        {
            return models.Select(m => ToStoryPoints(m, members)).ToArray();
        }

        public static StoryPoints ToStoryPoints(
            this VoteModel model,
            IEnumerable<TeamMemberModel> members)
        {
            return new StoryPoints
                {
                    Name = members.TryGetNameById(model.OwnerId),
                    Value = model.Points
                };
        }

        public static StoryPointsResults[] ToStoryPointResults(this IEnumerable<VoteResultModel> models)
        {
            return models == null
                       ? new StoryPointsResults[] {}
                       : models.Select(ToStoryPointResults).ToArray();
        }

        public static StoryPointsResults ToStoryPointResults(this VoteResultModel model)
        {
            return new StoryPointsResults
                {
                    Value = model.Points,
                    Percentage = model.Percentage
                };
        }
    }
}