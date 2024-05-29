using System;

namespace AchievementLib.Reset
{
    /// <summary>
    /// Provides extension methods for the <see cref="DateTime"/> struct.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Determines whether the <paramref name="dateTime"/> is in the future.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns><see langword="true"/>, if the <paramref name="dateTime"/> is in the future. 
        /// Otherwise <see langword="false"/>.</returns>
        public static bool IsInTheFuture(this DateTime dateTime)
        {
            return dateTime - DateTime.UtcNow > TimeSpan.Zero;
        }
    }
}
