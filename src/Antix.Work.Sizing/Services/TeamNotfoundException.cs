using System;
using System.Runtime.Serialization;

using Antix.Work.Sizing.Properties;

namespace Antix.Work.Sizing.Services
{
    [Serializable]
    public class TeamNotfoundException : SizeException
    {
        public TeamNotfoundException(string teamId) :
            base(Resources.TeamNotfoundException, teamId)
        {
        }

        protected TeamNotfoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}