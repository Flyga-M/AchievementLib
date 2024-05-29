using System;

namespace AchievementLib.Reset
{
    /// <summary>
    /// Holds data on when a timer resets.
    /// </summary>
    public class ResetData
    {
        /// <summary>
        /// An unique identifier to distinguish <see cref="ResetData"/> from each other.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The <see cref="DateTime"/> of the first occurence of the reset.
        /// </summary>
        public virtual DateTime Start { get; }

        /// <summary>
        /// The <see cref="TimeSpan"/> interval of the occurences after the first occurence.
        /// </summary>
        public virtual TimeSpan Interval { get; }

        /// <summary>
        /// The next occurence of the reset.
        /// </summary>
        public DateTime NextOccurence
        {
            get
            {
                DateTime next = Start;

                while (!next.IsInTheFuture())
                {
                    next += Interval;
                }

                return next;
            }
        }

        /// <summary>
        /// The <see cref="TimeSpan"/> until the <see cref="NextOccurence"/>.
        /// </summary>
        public TimeSpan UntilNextOccurence
        {
            get
            {
                return NextOccurence - DateTime.UtcNow;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="interval"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty or whitespace, or if 
        /// <paramref name="interval"/> is less or equal to <see cref="TimeSpan.Zero"/>.</exception>
        public ResetData(string id, DateTime start, TimeSpan interval)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Start = start;
            Interval = interval;

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"{nameof(id)} can't be empty or whitespace.", nameof(id));
            }

            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(interval)} must be greater than zero.", nameof(interval));
            }
        }
    }
}
