namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Character's profession. Corresponds to the Mumble API identity.profession.
    /// </summary>
    /// <remarks>
    /// https://wiki.guildwars2.com/wiki/API:MumbleLink
    /// https://github.com/Archomeda/Gw2Sharp/blob/master/Gw2Sharp/Models/ProfessionType.cs
    /// </remarks>
    public enum ProfessionType
    {
        /// <summary>
        /// [Unused] Just added it, because profession is 1-indexed.
        /// </summary>
        None,
        /// <summary>
        /// Guardian.
        /// </summary>
        Guardian,
        /// <summary>
        /// Warrior.
        /// </summary>
        Warrior,
        /// <summary>
        /// Engineer.
        /// </summary>
        Engineer,
        /// <summary>
        /// Ranger.
        /// </summary>
        Ranger,
        /// <summary>
        /// Thief.
        /// </summary>
        Thief,
        /// <summary>
        /// Elementalist.
        /// </summary>
        Elementalist,
        /// <summary>
        /// Mesmer.
        /// </summary>
        Mesmer,
        /// <summary>
        /// Necromancer.
        /// </summary>
        Necromancer,
        /// <summary>
        /// Revenant.
        /// </summary>
        Revenant
    }
}
