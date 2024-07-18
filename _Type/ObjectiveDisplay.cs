using AchievementLib.Pack;

namespace AchievementLib
{
    /// <summary>
    /// Determines how the <see cref="IObjective"/>s of an <see cref="IAchievement"/> are displayed.
    /// </summary>
    public enum ObjectiveDisplay
    {
        /// <summary>
        /// No <see cref="IObjective"/>s will be displayed.
        /// </summary>
        None,
        /// <summary>
        /// The <see cref="IObjective"/>s will be displayed as a checklist
        /// </summary>
        Checklist
    }
}
