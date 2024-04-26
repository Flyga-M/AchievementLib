namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Contains the comparison types for some <see cref="Action">Actions</see>.
    /// </summary>
    public enum Comparison
    {
        /// <summary>
        /// Equal (==)
        /// </summary>
        Equal,
        /// <summary>
        /// Not Equal (!=)
        /// </summary>
        NotEqual,
        /// <summary>
        /// Less than (&lt;)
        /// </summary>
        LessThan,
        /// <summary>
        /// Greater than (&gt;)
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Greater that or equal (&gt;=)
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// Less than or equal (&lt;=)
        /// </summary>
        LessThanOrEqual
    }
}
