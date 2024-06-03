using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Extensions for the <see cref="ICondition"/> interface.
    /// </summary>
    public static class IConditionExtensions
    {
        /// <summary>
        /// Returns the <see cref="IAction"/>s associated with the <see cref="ICondition"/>.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>The <see cref="IAction"/>s associated with the <see cref="ICondition"/>.</returns>
        public static IAction[] GetActions(this ICondition condition)
        {
            List<IAction> actions = new List<IAction>
            {
                condition.Action
            };

            if (condition.OrCondition != null)
            {
                actions.AddRange(condition.OrCondition.GetActions());
            }

            if (condition.AndCondition != null)
            {
                actions.AddRange(condition.AndCondition.GetActions());
            }

            return actions.ToArray();
        }
    }
}
