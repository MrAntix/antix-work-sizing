using System.Diagnostics;

namespace Antix.Work.Sizing.Services.Models
{
    [DebuggerDisplay("{Points} ({Percentage}%)")]
    public class VoteResultModel
    {
        public VoteResultModel(int points, int count)
        {
            Percentage = count;
            Points = points;
        }

        public int Points { get; private set; }
        public int Percentage { get; private set; }
    }
}