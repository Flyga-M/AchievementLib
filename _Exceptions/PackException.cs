using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    public class PackException : AchievementLibException
    {
        public PackException()
        {

        }

        public PackException(string message)
            : base(message)
        {

        }

        public PackException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected PackException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
