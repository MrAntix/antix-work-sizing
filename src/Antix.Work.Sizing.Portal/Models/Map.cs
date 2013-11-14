using System.Collections.Generic;
using System.Linq;
using Antix.Work.Sizing.Services.Models;

namespace Antix.Work.Sizing.Portal.Models
{
    public static class Map
    {
        public static Session ToClient(TeamModel model, string userId)
        {
            return new Session
            {
                User = ToClient(model.Members.ById(userId), model.Id),
                Team = ToClient(model)
            };
        }

        public static Team ToClient(TeamModel model)
        {
            return new Team
                {
                    Id = model.Id,
                    Users = ToClient(model.Members),
                    CurrentStory = ToClient(model.Story),
                    CurrentStoryOwner = model.Members
                        .Where(m=>m.Id==model.Story.OwnerId)
                        .Select(m=>m.Name)
                        .SingleOrDefault()
                };
        }

        public static User ToClient(TeamMemberModel model, string teamId)
        {
            return new User
            {
                Id = model.Id,
                Name = model.Name,
                TeamId = teamId,
                IsObserver = model.IsObserver
            };
        }

        public static TeamMember ToClient(TeamMemberModel model)
        {
            return new TeamMember
            {
                Name = model.Name,
                IsObserver = model.IsObserver
            };
        }

        public static TeamMember[] ToClient(IEnumerable<TeamMemberModel> models)
        {
            return models.Select(ToClient).ToArray();
        }

        public static Story ToClient(StoryModel model)
        {
            return new Story
                {
                    Title = model.Title,
                    VotingOpen = model.VotingIsOpen,
                    Points = ToClient(model.Votes)
                };
        }

        public static StoryPoints[] ToClient(IEnumerable<VoteModel> models)
        {
            return models.Select(ToClient).ToArray();
        }

        public static StoryPoints ToClient(VoteModel model)
        {
            return new StoryPoints
                {
                    Name = model.OwnerId,
                    Value = model.Points
                };
        }

        public static TeamMemberModel ToServer(User model, string id)
        {
            return new TeamMemberModel
                {
                    Id = id,
                    Name = model.Name,
                    IsObserver = model.IsObserver
                };
        }
    }
}