using System.Diagnostics;

namespace Antix.Work.Sizing.Services.Models
{
    [DebuggerDisplay("{OwnerId} {Points}")]
    public class VoteModel : IHasOwnerId
    {
        public int Points { get; set; }
        public string OwnerId { get; set; }
    }
}