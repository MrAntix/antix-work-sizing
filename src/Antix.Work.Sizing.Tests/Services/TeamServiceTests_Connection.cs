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
        public async Task disconnect_removes_the_user_and_users_votes()
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
            Assert.Equal(1, result.Story.Votes.Count());
        }
    }
}