namespace Antix.Work.Sizing.Portal.Models
{
    public class Story
    {
        public string Title { get; set; }

        public bool VotingOpen { get; set; }
        public StoryPoints[] Points { get; set; }
    }
}