using System.Threading.Tasks;

using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;

using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceTests_Story : TeamServiceTests
    {
        [Fact]
        public async Task lock_story_takes_ownership()
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

            var result = await teamService.LockStory(TeamId, MemberId, StoryTitle);

            Assert.Equal(StoryTitle, result.Story.Title);
            Assert.Equal(MemberId, result.Story.OwnerId);
        }

        [Fact]
        public async Task lock_story_throws_team_not_found()
        {
            var teamService =
                new TeamServiceBuilder()
                    .Build();

            await AssertEx.Throws<TeamNotfoundException>(
                async () => await teamService.LockStory(TeamId, MemberId, StoryTitle));
        }

        [Fact]
        public async Task lock_story_throws_member_not_found()
        {
            var team = new TeamModel {Id = TeamId};
            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await AssertEx.Throws<TeamMemberNotFoundException>(
                async () => await teamService.LockStory(TeamId, MemberId, StoryTitle));
        }

        [Fact]
        public async Task lock_story_throws_not_owner()
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
                async () => await teamService.LockStory(TeamId, MemberId, StoryTitle));
        }

        [Fact]
        public async Task unlock_story_clears_the_owner()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[] {new TeamMemberModel {Id = MemberId}}
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            team = await teamService.UnlockStory(TeamId, MemberId);

            Assert.Null(team.Story.OwnerId);
        }

        [Fact]
        public async Task unlock_story_throws_not_owner()
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
                async () => await teamService.UnlockStory(TeamId, MemberId));
        }
    }
}