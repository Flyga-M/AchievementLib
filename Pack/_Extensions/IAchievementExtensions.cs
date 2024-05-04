using System.Linq;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Extensions for the <see cref="IAchievement"/> interface.
    /// </summary>
    public static class IAchievementExtensions
    {
        /// <summary>
        /// Returns the maximum tier the <see cref="IAchievement"/> can reach. Tiers are 1-indexed.
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns>The maximum tier the <see cref="IAchievement"/> can reach.</returns>
        public static int GetMaxTier(this IAchievement achievement)
        {
            return achievement.Tiers.Count();
        }
    }
}
