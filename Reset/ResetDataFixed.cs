using System;

namespace AchievementLib.Reset
{
    /// <summary>
    /// <inheritdoc/> 
    /// Implements fixed days or months to achieve timers like "the first day of every month".
    /// </summary>
    public class ResetDataFixed : ResetData
    {
        /// <summary>
        /// Determines whether the day component of the <see cref="Start"/> remains fixed.
        /// </summary>
        /// <remarks>
        /// This may be used to achieve something like "the first of the month".
        /// </remarks>
        public bool FixDay { get; }

        /// <summary>
        /// Determines whether the month component of the <see cref="Start"/> remains fixed.
        /// </summary>
        /// <remarks>
        /// This may be used to achieve something like "every day in february". Or "every second of april" if used in conjunction 
        /// with <see cref="FixDay"/>.
        /// </remarks>
        public bool FixMonth { get; }

        /// <inheritdoc/>
        public override DateTime Start
        {
            get
            {
                DateTime start = base.Start;

                DateTime now = DateTime.UtcNow;
                int day = start.Day;
                int month = start.Month;
                int year = now.Year;

                if (!FixDay)
                {
                    day = now.Day;
                }

                if (!FixMonth)
                {
                    month = now.Month;
                }

                return new DateTime(year, month, day, now.Hour, now.Minute, now.Second);
            }
        }

        /// <inheritdoc/>
        public override TimeSpan Interval
        {
            get
            {
                DateTime start = Start;
                DateTime next = GetNextOccurence();

                return next-start;
            }
        }

        /// <inheritdoc/>
        public ResetDataFixed(string id, DateTime start, bool fixDay, bool fixMonth) : base(id, start, TimeSpan.MaxValue) // just any data, since it will be overwritten anyway
        {
            FixDay = fixDay;
            FixMonth = fixMonth;
        }

        private DateTime GetNextOccurence()
        {
            DateTime start = Start;

            if (!FixDay)
            {
                return start.AddDays(1);
            }

            if (!FixMonth)
            {
                return start.AddMonths(1);
            }

            return start.AddYears(1);
        }
    }
}
