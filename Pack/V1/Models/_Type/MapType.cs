namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Type of map. Corresponds to the Mumble API MumbleContext.mapType.
    /// </summary>
    /// <remarks>
    /// https://wiki.guildwars2.com/wiki/API:MumbleLink
    /// https://github.com/Archomeda/Gw2Sharp/blob/master/Gw2Sharp/Models/MapType.cs
    /// </remarks>
    public enum MapType
    {
        /// <summary>
        /// e.g., when logging in while in a PvP match
        /// </summary>
        Redirect = 0,
        /// <summary>
        /// Character creation
        /// </summary>
        CharacterCreation = 1,
        /// <summary>
        /// PvP maps (also activities)
        /// </summary>
        CompetitivePvp = 2,
        /// <summary>
        /// GvG maps. [GW2Sharp: Unused]
        /// </summary>
        Gvg = 3,
        /// <summary>
        /// Instanced content. e.g. Home instances, Personal story instances, Guild halls, 
        /// Dungeons, Fractals of the Mists, the private variants of Strike Missions, 
        /// Dragon Response Missions or Dragonstorm
        /// </summary>
        Instance = 4,
        /// <summary>
        /// Public maps, e.g. open world.
        /// </summary>
        Public = 5,
        /// <summary>
        /// Tournaments. [GW2Sharp: Probably unused]
        /// </summary>
        Tournament = 6,
        /// <summary>
        /// Tutorial maps.
        /// </summary>
        Tutorial = 7,
        /// <summary>
        /// User tournaments. [GW2Sharp: Probably unused]
        /// </summary>
        UserTournament = 8,
        /// <summary>
        /// Eternal Battlegrounds (WvW).
        /// </summary>
        EternalBattlegrounds = 9,
        /// <summary>
        /// Blue Borderlands (WvW).
        /// </summary>
        BlueBorderlands = 10,
        /// <summary>
        /// Green Borderlands (WvW).
        /// </summary>
        GreenBorderlands = 11,
        /// <summary>
        /// Red Borderlands (WvW).
        /// </summary>
        RedBorderlands = 12,
        /// <summary>
        /// [GW2Sharp: Fortune's Vale. Unused]
        /// </summary>
        WvWReward = 13,
        /// <summary>
        /// Obsidian Sanctum (WvW).
        /// </summary>
        ObsidianSanctum = 14,
        /// <summary>
        /// Edge of the Mists (WvW).
        /// </summary>
        EdgeOfTheMists = 15,
        /// <summary>
        /// [Wiki: (e.g. Mistlock Sanctuary or the public variants of Strike Missions, 
        ///       Dragon Response Missions, Dragonstorm or Convergences]
        /// [GW2Sharp: Mini public map type, e.g. Dry Top, the Silverwastes and Mistlock Sanctuary]
        /// </summary>
        PublicMini = 16,
        /// <summary>
        /// ?
        /// </summary>
        BigBattle = 17,
        /// <summary>
        /// Armistice Bastion (WvW).
        /// </summary>
        ArmsticeBastion = 18
    }
}
