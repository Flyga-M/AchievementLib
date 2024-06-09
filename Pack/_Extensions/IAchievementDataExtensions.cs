using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Extensions for the <see cref="IAchievementData"/> interface.
    /// </summary>
    public static class IAchievementDataExtensions
    {
        /// <summary>
        /// Returns the <see cref="IAchievement"/>s associated with the <see cref="IAchievementData"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The <see cref="IAchievement"/>s associated with the <see cref="IAchievementData"/>.</returns>
        public static IAchievement[] GetAchievements(this IAchievementData data)
        {
            List<IAchievement> achievements = new List<IAchievement>();

            foreach (IAchievementCategory category in data.AchievementCategories)
            {
                achievements.AddRange(category.GetAchievements());
            }

            return achievements.ToArray();
        }
    }
}
