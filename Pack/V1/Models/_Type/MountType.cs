namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Type of mount. Corresponds to the Mumble API MumbleContext.mountIndex.
    /// </summary>
    /// <remarks>
    /// https://wiki.guildwars2.com/wiki/API:MumbleLink
    /// https://github.com/Archomeda/Gw2Sharp/blob/master/Gw2Sharp/Models/MountType.cs
    /// </remarks>
    public enum MountType
    {
        /// <summary>
        /// No mount.
        /// </summary>
        None,
        /// <summary>
        /// The jackal mount.
        /// </summary>
        Jackal,
        /// <summary>
        /// The griffon mount.
        /// </summary>
        Griffon,
        /// <summary>
        /// The springer mount.
        /// </summary>
        Springer,
        /// <summary>
        /// The skimmer mount.
        /// </summary>
        Skimmer,
        /// <summary>
        /// The raptor mount.
        /// </summary>
        Raptor,
        /// <summary>
        /// The roller beetle mount.
        /// </summary>
        RollerBeetle,
        /// <summary>
        /// The warclaw mount.
        /// </summary>
        Warclaw,
        /// <summary>
        /// The skyscale mount.
        /// </summary>
        Skyscale,
        /// <summary>
        /// The skiff.
        /// </summary>
        Skiff,
        /// <summary>
        /// The siege turtle mount.
        /// </summary>
        SiegeTurtle
    }
}
