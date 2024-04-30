using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="PackException"/> that is thrown, when an error with a 
    /// solvable condiiton or action arises.
    /// </summary>
    [Serializable]
    public class PackSolveException : PackException
    {
        /// <inheritdoc/>
        public PackSolveException()
        {

        }

        /// <inheritdoc/>
        public PackSolveException(string message)
            : base(message)
        {

        }

        /// <inheritdoc/>
        public PackSolveException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <inheritdoc/>
        protected PackSolveException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
