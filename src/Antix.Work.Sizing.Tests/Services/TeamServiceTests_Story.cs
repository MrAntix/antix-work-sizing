using System.Threading.Tasks;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;
using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceTests_Story : TeamServiceTests
    {
        [Fact]
        public async Task set_story_takes_ownership()
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

            var result = await teamService.SetStory(TeamId, MemberId, StoryTitle);

            Assert.Equal(StoryTitle, result.Story.Title);
            Assert.Equal(MemberId, result.Story.OwnerId);
        }

        [Fact]
        public async Task set_story_throws_team_not_found()
        {
            var teamService =
                new TeamServiceBuilder()
                    .Build();

            await AssertEx.Throws<TeamNotfoundException>(
                async () => await teamService.SetStory(TeamId, MemberId, StoryTitle));
        }

        [Fact]
        public async Task set_story_throws_member_not_found()
        {
            var team = new TeamModel {Id = TeamId};
            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await AssertEx.Throws<TeamMemberNotFoundException>(
                async () => await teamService.SetStory(TeamId, MemberId, StoryTitle));
        }

        [Fact]
        public async Task set_story_throws_not_owner()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = "OTHER"},
                    Members = new[] {new TeamMemberModel {Id = MemberId}}
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await AssertEx.Throws<RequiresOwnerPermissionException>(
                async () => await teamService.SetStory(TeamId, MemberId, StoryTitle));
        }
    }
}