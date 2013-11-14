namespace Antix.Work.Sizing.Portal.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string TeamId { get; set; }
        public bool IsObserver { get; set; }
    }
}