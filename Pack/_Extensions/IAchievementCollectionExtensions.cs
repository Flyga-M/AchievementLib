using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Extensions for the <see cref="IAchievementCollection"/> interface.
    /// </summary>
    public static class IAchievementCollectionExtensions
    {
        /// <summary>
        /// Returns the <see cref="IAction"/>s associated with the <see cref="IAchievementCollection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>The <see cref="IAction"/>s associated with the <see cref="IAchievementCollection"/>.</returns>
        public static IAction[] GetActions(this IAchievementCollection collection)
        {
            List<IAction> actions = new List<IAction>();

            foreach (IAchievement achievement in collection.Achievements)
            {
                actions.AddRange(achievement.GetActions());
            }

            return actions.ToArray();
        }
    }
}
