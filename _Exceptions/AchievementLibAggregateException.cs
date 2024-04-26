using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="AchievementLibException"/> that is thrown, when multiple 
    /// <see cref="AchievementLibException">AchievementLibExceptions</see> are combined 
    /// into a single <see cref="AchievementLibException"/>.
    /// </summary>
    public class AchievementLibAggregateException : AchievementLibException
    {
        /// <inheritdoc/>
        public AchievementLibAggregateException()
        {

        }

        /// <inheritdoc/>
        public AchievementLibAggregateException(string message)
            : base(message)
        {

        }

        /// <inheritdoc cref="AggregateException(string, Exception)"/>
        public AchievementLibAggregateException(string message, AchievementLibException inner)
            : base(message, inner)
        {

        }

        /// <inheritdoc cref="AggregateException(string, IEnumerable{Exception})"/>
        public AchievementLibAggregateException(string message, IEnumerable<AchievementLibException> innerExceptions)
            : base(message, innerExceptions == null ? null : (!innerExceptions.Any() ? null : new AggregateException(innerExceptions)))
        {
            
        }

        /// <inheritdoc cref="AggregateException(IEnumerable{Exception})"/>
        public AchievementLibAggregateException(IEnumerable<AchievementLibException> innerExceptions)
            : base("Multiple exceptions were thrown.", innerExceptions == null ? null : (!innerExceptions.Any() ? null : new AggregateException(innerExceptions)))
        {

        }

        /// <inheritdoc/>
        protected AchievementLibAggregateException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
