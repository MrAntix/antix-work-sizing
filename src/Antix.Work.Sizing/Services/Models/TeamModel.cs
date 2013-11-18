using System.Diagnostics;

namespace Antix.Work.Sizing.Services.Models
{
    [DebuggerDisplay("{Id}")]
    public class TeamModel : IHasId
    {
        public TeamModel()
        {
            Members = new TeamMemberModel[] {};
            Story = new StoryModel();
        }

        public string Id { get; set; }

        public TeamMemberModel[] Members { get; set; }

        public StoryModel Story { get; set; }

        public bool InDemoMode { get; set; }
    }
}