namespace Antix.Work.Sizing.Portal.Models
{
    public class User
    {
        public string Name { get; set; }

        public string TeamId { get; set; }
        public bool IsObserver { get; set; }
    }

    public class Team
    {
        public string Id { get; set; }
        public User[] Members { get; set; }
    }
}