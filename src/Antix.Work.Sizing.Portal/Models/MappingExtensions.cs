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
            return new Team
                {
                    Id = model.Id,
                    Users = model.Members.ToTeamMembers(),
                    CurrentStory = ToStory(model.Story),
                    CurrentStoryOwner = model.Members
                                             .Where(m => m.Id == model.Story.OwnerId)
                                             .Select(m => m.Name)
                                             .SingleOrDefault()
                };
        }

        public static User ToUser(
            this TeamModel model, string userId)
        {
            return ToUser(model.Members.ById(userId), model.Id);
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

        static TeamMember ToTeamMember(this TeamMemberModel model)
        {
            return new TeamMember
                {
                    Name = model.Name,
                    IsObserver = model.IsObserver
                };
        }

        public static Story ToStory(this StoryModel model)
        {
            return new Story
                {
                    Title = model.Title,
                    VotingOpen = model.VotingIsOpen,
                    Points = StoryPoints(model.Votes)
                };
        }

        public static StoryPoints[] StoryPoints(this IEnumerable<VoteModel> models)
        {
            return models.Select(StoryPoints).ToArray();
        }

        public static StoryPoints StoryPoints(this VoteModel model)
        {
            return new StoryPoints
                {
                    Name = model.OwnerId,
                    Value = model.Points
                };
        }
    }
}