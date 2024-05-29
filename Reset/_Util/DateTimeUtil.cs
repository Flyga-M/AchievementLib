using System;

namespace AchievementLib.Reset
{
    /// <summary>
    /// Provides utility functions for the <see cref="DateTime"/> struct.
    /// </summary>
    public static class DateTimeUtil
    {
        /// <summary>
        /// Returns the next occurence of the <paramref name="weekday"/>, or <see cref="DateTime.Today"/>, if today is the 
        /// <paramref name="weekday"/>.
        /// </summary>
        /// <param name="weekday"></param>
        /// <returns>The next occurence of the <paramref name="weekday"/>, or <see cref="DateTime.Today"/>, if today is the 
        /// <paramref name="weekday"/>.</returns>
        public static DateTime GetTodayOrNextWeekday(DayOfWeek weekday)
        {
            DateTime today = DateTime.Today;
            if (today.DayOfWeek == weekday)
            {
                return today;
            }

            int daysUntilWeekday = ((int)weekday - (int)today.DayOfWeek + 7) % 7;
            DateTime next = today.AddDays(daysUntilWeekday);

            return next;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> for the current month, with the day component set to <paramref name="day"/>.
        /// </summary>
        /// <param name="day"></param>
        /// <returns>The <see cref="DateTime"/> for the current month, with the day component set to <paramref name="day"/>.</returns>
        public static DateTime GetDayOfMonth(int day)
        {
            DateTime today = DateTime.Today;

            return new DateTime(today.Year, today.Month, day);
        }
    }
}
