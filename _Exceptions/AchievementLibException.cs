using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    public class AchievementLibException : Exception
    {
        public AchievementLibException()
        {

        }

        public AchievementLibException(string message)
            : base(message)
        {

        }

        public AchievementLibException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected AchievementLibException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
