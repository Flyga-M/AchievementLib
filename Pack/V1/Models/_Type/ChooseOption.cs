namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Determines which element in a range of api responses will be used by the <see cref="ApiAction"/>.
    /// </summary>
    public enum ChooseOption
    {
        /// <summary>
        /// The response itself.
        /// </summary>
        Self,
        /// <summary>
        /// The first element of the response.
        /// </summary>
        First,
        /// <summary>
        /// The last element of the response.
        /// </summary>
        Last,
        /// <summary>
        /// The element with the most items.
        /// </summary>
        Max,
        /// <summary>
        /// The element with the fewest items.
        /// </summary>
        Min,
        /// <summary>
        /// The items of all elements will be combined to a single list of items.
        /// </summary>
        AppendAll
    }
}
