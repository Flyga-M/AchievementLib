namespace AchievementLib.Pack
{
    /// <summary>
    /// Extensions for the <see cref="IObjective"/> interface.
    /// </summary>
    public static class IObjectiveExtensions
    {
        /// <summary>
        /// Returns the <see cref="IAction"/>s associated with the <see cref="IObjective"/>.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns>The <see cref="IAction"/>s associated with the <see cref="IObjective"/>.</returns>
        public static IAction[] GetActions(this IObjective objective)
        {
            return objective.Condition.GetActions();
        }
    }
}
