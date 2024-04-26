namespace AchievementLib.Pack
{
    /// <summary>
    /// Contains data on the loading success of an achievement pack.
    /// </summary>
    public interface IPackLoadReport
    {
        /// <summary>
        /// Might be null, if the data was not yet loaded.
        /// </summary>
        bool? FaultyData { get; }

        /// <summary>
        /// Might be null, if the data was not yet loaded, or faulty. Does not determine 
        /// <see cref="Success"/>.
        /// </summary>
        bool? FaultyResources { get; }

        /// <summary>
        /// Will still be true, if some resources are faulty.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// An exception that occured during the loading of the achievement pack, or null if 
        /// no exception occured (yet).
        /// </summary>
        AchievementLibException Exception { get; }
    }
}
