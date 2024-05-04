namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Determines how many elements need to be compared against.
    /// </summary>
    public enum ComparisonTarget
    {
        /// <summary>
        /// All elements in the api response.
        /// </summary>
        All,
        /// <summary>
        /// At least one element in the api response.
        /// </summary>
        Any
    }
}
