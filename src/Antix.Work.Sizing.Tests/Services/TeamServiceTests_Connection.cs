using System.Linq;
using System.Threading.Tasks;

using Antix.Work.Sizing.Services.Models;

using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceTests_Connection : TeamServiceTests
    {
        [Fact]
        public async Task connect_adds_user_and_team_if_none_exists()
        {
            var user = new TeamMemberModel();

            var teamService =
                new TeamServiceBuilder()
                    .Build();

            var result = await teamService.Connect(TeamId, user);

            Assert.Equal(1, result.Members.Count());
        }

        [Fact]
        public async Task disconnect_removes_the_user_and_users_votes_and_owner()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel()
                        },
                    Story = new StoryModel
                        {
                            OwnerId = MemberId,
                            Votes = new[]
                                {
                                    new VoteModel {OwnerId = MemberId},
                                    new VoteModel()
                                }
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            var result = await teamService.TryDisconnect(TeamId, MemberId);

            Assert.Equal(1, result.Members.Count());
            Assert.Null(result.Story.OwnerId);
            Assert.Equal(1, result.Story.Votes.Count());
        }

        [Fact]
        public async Task disconnect_does_not_clear_owner()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel()
                        },
                    Story = new StoryModel
                        {
                            OwnerId = "Other",
                            Votes = new[]
                                {
                                    new VoteModel {OwnerId = MemberId},
                                    new VoteModel()
                                }
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            var result = await teamService.TryDisconnect(TeamId, MemberId);

            Assert.NotNull(result.Story.OwnerId);
        }
    }
}