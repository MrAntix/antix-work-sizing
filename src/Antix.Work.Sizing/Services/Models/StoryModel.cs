using System.Diagnostics;

namespace Antix.Work.Sizing.Services.Models
{
    [DebuggerDisplay("{Title} (Owner: {OwnerId})")]
    public class StoryModel
    {
        public StoryModel()
        {
            Votes = new VoteModel[] {};
        }

        public string Title { get; set; }
        public string OwnerId { get; set; }
        public bool VotingIsOpen { get; set; }
        public VoteModel[] Votes { get; set; }

        public VoteScheduleModel VoteSchedule { get; set; }
    }
}