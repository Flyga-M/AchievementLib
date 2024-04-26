using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="AchievementLibException"/> that is thrown, when an error arises during 
    /// reading, writing or loading an achievement pack.
    /// </summary>
    public class PackException : AchievementLibException
    {
        /// <inheritdoc/>
        public PackException()
        {

        }

        /// <inheritdoc/>
        public PackException(string message)
            : base(message)
        {

        }

        /// <inheritdoc/>
        public PackException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <inheritdoc/>
        protected PackException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
