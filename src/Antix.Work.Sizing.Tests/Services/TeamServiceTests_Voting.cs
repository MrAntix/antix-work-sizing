using System.Linq;
using System.Threading.Tasks;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;
using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceTests_Voting : TeamServiceTests
    {
        [Fact]
        public async Task owner_opens_voting()
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

            var result = await teamService.OpenVoting(TeamId, MemberId);

            Assert.True(result.Story.VotingIsOpen);
        }

        [Fact]
        public async Task owner_closes_voting()
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

            var result = await teamService.CloseVoting(TeamId, MemberId);

            Assert.False(result.Story.VotingIsOpen);
        }

        [Fact]
        public async Task member_votes()
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

            await teamService.OpenVoting(TeamId, MemberId);

            var result = await teamService.Vote(TeamId, MemberId, 1);

            Assert.Equal(1, result.Story.Votes.Single().Points);
        }

        [Fact]
        public async Task member_votes_throws_if_not_open()
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

            await AssertEx.Throws<VotingIsClosedException>(
                async () => await teamService.Vote(TeamId, MemberId, 1));
        }

        [Fact]
        public async Task result_available_when_all_votes_are_closed()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = "OTHER"}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await teamService.OpenVoting(TeamId, MemberId);

            var result = await teamService.Vote(TeamId, MemberId, 1);

            Assert.Null(result.GetVoteResults());

            await teamService.CloseVoting(TeamId, MemberId);

            var votes = result.GetVoteResults();
            Assert.NotNull(votes);
        }

        [Fact]
        public async Task result_available_when_all_votes_are_in()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = "OTHER"}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await teamService.OpenVoting(TeamId, MemberId);

            await teamService.Vote(TeamId, "OTHER", 1);
            var result = await teamService.Vote(TeamId, MemberId, 1);
            var votes = result.GetVoteResults();

            Assert.NotNull(votes);
        }

        [Fact]
        public async Task clear_all_votes()
        {
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = "OTHER"}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await teamService.OpenVoting(TeamId, MemberId);

            await teamService.Vote(TeamId, "OTHER", 1);
            await teamService.Vote(TeamId, MemberId, 1);

            var result = await teamService.ClearVotes(TeamId, MemberId);

            Assert.Equal(0, result.Story.Votes.Count());
            Assert.False(result.Story.VotingIsOpen);
        }
    }
}