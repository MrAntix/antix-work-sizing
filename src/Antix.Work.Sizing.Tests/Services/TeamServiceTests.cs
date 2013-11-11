using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antix.Work.Sizing.Services;
using Moq;
using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceTests
    {
        [Fact]
        public async Task connect_adds_user_and_team_if_none_exists()
        {
            var user = new TeamMemberModel();

            var teamService =
                new TeamServiceBuilder()
                    .Build();

            var result = await teamService.Connect(user, "TEAM");

            Assert.Equal(1, result.Members.Count());
        }

        [Fact]
        public async Task disconnect_removes_the_user_and_users_votes()
        {
            var team = new TeamModel
                {
                    Id = "TEAM",
                    Members = new[]
                        {
                            new TeamMemberModel {Id = "MEMBER"},
                            new TeamMemberModel()
                        },
                    StoryVotes = new[]
                        {
                            new StoryVoteModel {OwnerId = "MEMBER"},
                            new StoryVoteModel()
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            var result = await teamService.TryDisconnect("MEMBER", "TEAM");

            Assert.Equal(1, result.Members.Count());
            Assert.Equal(1, result.StoryVotes.Count());
        }
    }

    public class TeamServiceBuilder
    {
        readonly IList<TeamModel> _teams = new List<TeamModel>();

        public TeamServiceBuilder With(TeamModel team)
        {
            _teams.Add(team);
            return this;
        }

        public ITeamService Build()
        {
            var dataServiceMock = new Mock<ITeamDataService>();
            dataServiceMock
                .Setup(o => o.TryGetById(It.IsAny<string>()))
                .Returns((string id) => Task.FromResult(_teams.ByIdOrDefault(id)));
            dataServiceMock
                .Setup(o => o.Create())
                .Returns(new TeamModel
                    {
                        Id = Guid.NewGuid().ToString()
                    });

            return new TeamService(dataServiceMock.Object);
        }
    }
}