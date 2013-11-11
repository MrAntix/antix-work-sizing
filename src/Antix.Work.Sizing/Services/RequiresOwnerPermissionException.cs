using System;
using System.Runtime.Serialization;
using Antix.Work.Sizing.Properties;

namespace Antix.Work.Sizing.Services
{
    [Serializable]
    public class RequiresOwnerPermissionException : SizeException
    {
        public RequiresOwnerPermissionException(string memberId, string ownerId) :
            base(Resources.RequiresOwnerPermissionException, memberId, ownerId)
        {
        }

        protected RequiresOwnerPermissionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}