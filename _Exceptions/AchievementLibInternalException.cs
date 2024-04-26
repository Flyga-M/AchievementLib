using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="AchievementLibException"/> that is thrown, when an internal error occurs 
    /// in this library. If this <see cref="AchievementLibException"/> get's ever thrown, 
    /// there is an error in the internal logic of this library. Please report the issue to me!
    /// </summary>
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

        internal AchievementLibInternalException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
