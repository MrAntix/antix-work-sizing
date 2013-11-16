using System.Linq;
using System.Threading.Tasks;
using Antix.Work.Sizing.Properties;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;
using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceTests_Members : TeamServiceTests
    {
        [Fact]
        public async Task member_changes_name()
        {
            const string newName = "NEW NAME";
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

            var result = await teamService
                                   .TryUpdateMember(
                                       TeamId, MemberId,
                                       MemberId, m => m.Name = newName);

            Assert.Equal(newName, result.Members.Single().Name);
        }

        [Fact]
        public async Task member_changes_name_to_existing()
        {
            const string newName = "NEW NAME";
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = "OTHER", Name = newName}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            var result = await teamService
                                   .TryUpdateMember(
                                       TeamId, MemberId,
                                       MemberId, m => m.Name = newName);

            Assert.Equal(newName + " (2)", result.Members.GetById(MemberId).Name);
        }

        [Fact]
        public async Task member_changes_name_to_invalid()
        {
            const string newName = " ";
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            var result = await teamService
                                   .TryUpdateMember(
                                       TeamId, MemberId,
                                       MemberId, m => m.Name = newName);

            Assert.Equal(Resources.InvalidMemberName, result.Members.GetById(MemberId).Name);
        }

        [Fact]
        public async Task member_cannot_changes_other()
        {
            const string otherMemberId = "OTHER";
            const string newName = "XXX";
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = otherMemberId}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await AssertEx.Throws<RequiresOwnerPermissionException>(
                async () => await teamService
                                      .TryUpdateMember(
                                          TeamId, otherMemberId,
                                          MemberId, m => m.Name = newName)
                );
        }
        
        [Fact]
        public async Task owner_can_changes_other()
        {
            const string otherMemberId = "OTHER";
            const string newName = "XXX";
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = otherMemberId}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            await AssertEx.DoesNotThrow(
                async () => await teamService
                                      .TryUpdateMember(
                                          TeamId, MemberId,
                                          otherMemberId, m => m.Name = newName)
                );
        }

        [Fact]
        public async Task member_to_observer()
        {
            const string targetMemberId = "OTHER";
            var team = new TeamModel
                {
                    Id = TeamId,
                    Story = new StoryModel {OwnerId = MemberId},
                    Members = new[]
                        {
                            new TeamMemberModel {Id = MemberId},
                            new TeamMemberModel {Id = targetMemberId}
                        }
                };

            var teamService =
                new TeamServiceBuilder()
                    .With(team)
                    .Build();

            var result = await teamService
                                   .TryUpdateMember(
                                       TeamId, MemberId,
                                       targetMemberId, m => m.IsObserver = true);

            Assert.True(result.Members.GetById(targetMemberId).IsObserver);
        }
    }
}