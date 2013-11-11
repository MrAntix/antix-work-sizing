using System;
using System.Runtime.Serialization;
using Antix.Work.Sizing.Properties;

namespace Antix.Work.Sizing.Services
{
    [Serializable]
    public class TeamMemberNotFoundException : SizeException
    {
        public TeamMemberNotFoundException(string memberId, string teamId) :
            base(Resources.TeamMemberNotFoundException, memberId, teamId)
        {
        }

        protected TeamMemberNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}