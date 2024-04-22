using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AchievementLib
{
    public class AchievementLibAggregateException : AchievementLibException
    {
        public AchievementLibAggregateException()
        {

        }

        public AchievementLibAggregateException(string message)
            : base(message)
        {

        }

        public AchievementLibAggregateException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public AchievementLibAggregateException(string message, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions == null ? null : (!innerExceptions.Any() ? null : new AggregateException(innerExceptions)))
        {
            
        }

        public AchievementLibAggregateException(IEnumerable<Exception> innerExceptions)
            : base("Multiple exceptions were thrown.", innerExceptions == null ? null : (!innerExceptions.Any() ? null : new AggregateException(innerExceptions)))
        {

        }

        protected AchievementLibAggregateException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
