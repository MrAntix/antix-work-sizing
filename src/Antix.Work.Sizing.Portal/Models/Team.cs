namespace Antix.Work.Sizing.Portal.Models
{
    public class Team
    {
        public string Id { get; set; }
        public TeamMember[] Users { get; set; }
        public Story CurrentStory { get; set; }
        public string CurrentStoryOwner { get; set; }
    }
}