using System;
using System.Runtime.Serialization;

using Antix.Work.Sizing.Properties;

namespace Antix.Work.Sizing.Services
{
    [Serializable]
    public class VotingIsClosedException : SizeException
    {
        public VotingIsClosedException() :
            base(Resources.VotingIsClosedException)
        {
        }

        protected VotingIsClosedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}