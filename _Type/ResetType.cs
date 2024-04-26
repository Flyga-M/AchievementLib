namespace AchievementLib
{
    /// <summary>
    /// The timespan in which an achievement resets.
    /// </summary>
    public enum ResetType
    {
        /// <summary>
        /// The achievement will never reset.
        /// </summary>
        Permanent,
        /// <summary>
        /// The achievement resets once every day.
        /// </summary>
        Daily,
        /// <summary>
        /// The achievement resets once every week.
        /// </summary>
        Weekly,
        /// <summary>
        /// The achievement resets once every month.
        /// </summary>
        Monthly
    }
}
