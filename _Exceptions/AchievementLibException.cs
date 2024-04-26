using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// A general <see cref="Exception"/> that is thrown inside this library. Every other 
    /// exception inherits from the <see cref="AchievementLibException"/>.
    /// </summary>
    public class AchievementLibException : Exception
    {
        /// <inheritdoc/>
        public AchievementLibException()
        {

        }

        /// <inheritdoc/>
        public AchievementLibException(string message)
            : base(message)
        {

        }

        /// <inheritdoc/>
        public AchievementLibException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <inheritdoc/>
        protected AchievementLibException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
