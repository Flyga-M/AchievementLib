namespace AchievementLib.Pack
{
    /// <summary>
    /// A tier of an <see cref="IAchievement"/> that describes the requirement and reward for reaching 
    /// it.
    /// </summary>
    public interface ITier : IValidateable
    {
        /// <summary>
        /// The <see cref="IAchievement.CurrentObjectives"/> that needs to be completed 
        /// to reach the <see cref="ITier"/>.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The amount of rewarded points, if the <see cref="ITier"/> is reached.
        /// </summary>
        int Points { get; }
    }
}
