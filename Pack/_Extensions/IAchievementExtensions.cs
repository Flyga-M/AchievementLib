using System.Collections.Generic;
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

        /// <summary>
        /// Returns the <see cref="IAction"/>s associated with the <see cref="IAchievement"/>.
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns>The <see cref="IAction"/>s associated with the <see cref="IAchievement"/>.</returns>
        public static IAction[] GetActions(this IAchievement achievement)
        {
            List<IAction> actions = new List<IAction>();
            
            foreach(IObjective objective in achievement.Objectives)
            {
                actions.AddRange(objective.GetActions());
            }

            if (achievement.ResetCondition != null)
            {
                actions.AddRange(achievement.ResetCondition.GetActions());
            }

            return actions.ToArray();
        }
    }
}
