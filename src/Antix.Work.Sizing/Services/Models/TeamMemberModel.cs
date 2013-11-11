namespace Antix.Work.Sizing.Services.Models
{
    public class TeamMemberModel : IHasIdAndName
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool IsObserver { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}: {1} {2}",
                Id, Name, IsObserver ? "[observer]" : string.Empty);
        }
    }
}