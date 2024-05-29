using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Reset.Default
{
    /// <summary>
    /// <inheritdoc/> 
    /// Contains the default resets for Guild Wars 2.
    /// </summary>
    public class Gw2Resets : ResetManager
    {
        /// <summary>
        /// The daily reset. 00:00 UTC every day.
        /// </summary>
        public static readonly ResetData DailyReset = new ResetData("daily", DateTime.Today.AddDays(1), TimeSpan.FromDays(1));

        /// <summary>
        /// The weekly reset. 07:30 UTC every monday.
        /// </summary>
        public static readonly ResetData WeeklyReset = new ResetData("weekly", DateTimeUtil.GetTodayOrNextWeekday(DayOfWeek.Monday).AddHours(7).AddMinutes(30), TimeSpan.FromDays(7));

        /// <summary>
        /// The monthly reset. 00:00 UTC every first day of the month. (Does not exist in gw2)
        /// </summary>
        public static readonly ResetData MonthlyReset = new ResetDataFixed("monthly", DateTimeUtil.GetDayOfMonth(1), true, false);

        /// <inheritdoc/>
        public Gw2Resets() : base(new ResetData[] { DailyReset, WeeklyReset, MonthlyReset })
        {

        }
    }
}
