using System.Diagnostics;

namespace Antix.Work.Sizing.Services.Models
{
    [DebuggerDisplay("{Points} ({Percentage}%)")]
    public class VoteResultModel
    {
        readonly int _points;
        readonly decimal _percentage;

        public VoteResultModel(int points, decimal percentage)
        {
            _percentage = percentage;
            _points = points;
        }

        public int Points
        {
            get { return _points; }
        }

        public decimal Percentage
        {
            get { return _percentage; }
        }
    }
}