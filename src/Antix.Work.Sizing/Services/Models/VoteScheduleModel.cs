using System.Diagnostics;

namespace Antix.Work.Sizing.Services.Models
{
    [DebuggerDisplay("{Seconds}s {Percent}%")]
    public class VoteScheduleModel
    {
        readonly decimal _percent;
        readonly int _seconds;

        public VoteScheduleModel(decimal percent, int seconds)
        {
            _percent = percent;
            _seconds = seconds;
        }

        public decimal Percent
        {
            get { return _percent; }
        }

        public int Seconds
        {
            get { return _seconds; }
        }
    }
}