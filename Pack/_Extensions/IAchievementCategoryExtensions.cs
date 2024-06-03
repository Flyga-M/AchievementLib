using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Extensions for the <see cref="IAchievementCategory"/> interface.
    /// </summary>
    public static class IAchievementCategoryExtensions
    {
        /// <summary>
        /// Returns the <see cref="IAction"/>s associated with the <see cref="IAchievementCategory"/>.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>The <see cref="IAction"/>s associated with the <see cref="IAchievementCategory"/>.</returns>
        public static IAction[] GetActions(this IAchievementCategory category)
        {
            List<IAction> actions = new List<IAction>();

            foreach (IAchievementCollection collection in category.AchievementCollections)
            {
                actions.AddRange(collection.GetActions());
            }

            return actions.ToArray();
        }
    }
}
