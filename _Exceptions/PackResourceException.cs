using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    public class PackResourceException : PackException
    {
        public string ResourceName { get; }

        public override string Message
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ResourceName))
                {
                    return base.Message + $" ResourceName: {ResourceName}";
                }
                return base.Message;
            }
        }

        public PackResourceException()
        {

        }

        public PackResourceException(string message)
            : base(message)
        {

        }

        public PackResourceException(string message, string resourceName)
            : base(message)
        {
            ResourceName = resourceName;
            base.Data.Add("ResourceName", ResourceName);
        }

        public PackResourceException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public PackResourceException(string message, string resourceName, Exception inner)
            : base(message, inner)
        {
            ResourceName = resourceName;
            base.Data.Add("ResourceName", ResourceName);
        }

        protected PackResourceException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
