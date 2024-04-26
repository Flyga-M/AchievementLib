namespace AchievementLib.Pack
{
    /// <summary>
    /// Indicates the load state of an achievement pack.
    /// </summary>
    public enum PackLoadState
    {
        /// <summary>
        /// The achievement pack is not enabled and it's resources have not been loaded ( or already freed).
        /// </summary>
        Unloaded,
        /// <summary>
        /// The achievement pack has been enabled and is currently loading.
        /// </summary>
        Loading,
        /// <summary>
        /// The achievement pack is enabled and fully loaded (including it's resources)
        /// </summary>
        Loaded,
        /// <summary>
        /// The achievement pack has been disabled and is currently unloading and freeing it's resources.
        /// </summary>
        Unloading,
        /// <summary>
        /// A fatal error occured when loading or enabling the achievement pack. For more information 
        /// check <see cref="IAchievementPackManager.Report"/>.
        /// </summary>
        FatalError
    }
}
