namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Character's race. Corresponds to the Mumble API identity.race.
    /// </summary>
    /// <remarks>
    /// https://github.com/Archomeda/Gw2Sharp/blob/master/Gw2Sharp/Models/RaceType.cs
    /// https://wiki.guildwars2.com/wiki/API:MumbleLink
    /// </remarks>
    public enum RaceType
    {
        /// <summary>
        /// Asura
        /// </summary>
        Asura,
        /// <summary>
        /// Charr
        /// </summary>
        Charr,
        /// <summary>
        /// Human
        /// </summary>
        Human,
        /// <summary>
        /// Norn
        /// </summary>
        Norn,
        /// <summary>
        /// Sylvari
        /// </summary>
        Sylvari
    }
}
