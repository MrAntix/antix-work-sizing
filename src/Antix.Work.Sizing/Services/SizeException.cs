using System;
using System.Runtime.Serialization;

namespace Antix.Work.Sizing.Services
{
    [Serializable]
    public class SizeException : Exception
    {
        public SizeException(string format, params object[] args) :
            base(string.Format(format, args))
        {
        }

        protected SizeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}