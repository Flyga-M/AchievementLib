using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    public class AchievementLibInternalException : AchievementLibException
    {
        internal AchievementLibInternalException()
        {

        }

        internal AchievementLibInternalException(string message)
            : base(message)
        {

        }

        internal AchievementLibInternalException(string message, Exception inner)
            : base(message, inner)
        {

        }

        internal protected AchievementLibInternalException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
