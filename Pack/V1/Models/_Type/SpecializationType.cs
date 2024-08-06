namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Character's third specialization. Corresponds to the Mumble API identity.spec.
    /// </summary>
    /// <remarks>
    /// https://wiki.guildwars2.com/wiki/API:MumbleLink
    /// https://api.guildwars2.com/v2/specializations
    /// </remarks>
    public enum SpecializationType
    {
        /// <summary>
        /// [Unused] Just added it, because specialization is 1-indexed.
        /// </summary>
        None,
        /// <summary>
        /// Mesmer (not elite)
        /// </summary>
        Dueling,
        /// <summary>
        /// Necromancer (not elite)
        /// </summary>
        DeathMagic,
        /// <summary>
        /// Revenant (not elite)
        /// </summary>
        Invocation,
        /// <summary>
        /// Warrior (not elite)
        /// </summary>
        Strength,
        /// <summary>
        /// Ranger (elite)
        /// </summary>
        Druid,
        /// <summary>
        /// Engineer (not elite)
        /// </summary>
        Explosives,
        /// <summary>
        /// Thief (elite)
        /// </summary>
        Daredevil,
        /// <summary>
        /// Ranger (not elite)
        /// </summary>
        Marksmanship,
        /// <summary>
        /// Revenant (not elite)
        /// </summary>
        Retribution,
        /// <summary>
        /// Mesmer (not elite)
        /// </summary>
        Domination,
        /// <summary>
        /// Warrior (not elite)
        /// </summary>
        Tactics,
        /// <summary>
        /// Revenant (not elite)
        /// </summary>
        Salvation,
        /// <summary>
        /// Guardian (not elite)
        /// </summary>
        Valor,
        /// <summary>
        /// Revenant (not elite)
        /// </summary>
        Corruption,
        /// <summary>
        /// Revenant (not elite)
        /// </summary>
        Devastation,
        /// <summary>
        /// Guardian (not elite)
        /// </summary>
        Radiance,
        /// <summary>
        /// Elementalist (not elite)
        /// </summary>
        Water,
        /// <summary>
        /// Warrior (elite)
        /// </summary>
        Berserker,
        /// <summary>
        /// Necromancer (not elite)
        /// </summary>
        BloodMagic,
        /// <summary>
        /// Thief (not elite)
        /// </summary>
        ShadowArts,
        /// <summary>
        /// Engineer (not elite)
        /// </summary>
        Tools,
        /// <summary>
        /// Warrior (not elite)
        /// </summary>
        Defense,
        /// <summary>
        /// Mesmer (not elite)
        /// </summary>
        Inspiration,
        /// <summary>
        /// Mesmer (not elite)
        /// </summary>
        Illusions,
        /// <summary>
        /// Ranger (not elite)
        /// </summary>
        NatureMagic,
        /// <summary>
        /// Elementalist (not elite)
        /// </summary>
        Earth,
        /// <summary>
        /// Guardian (elite)
        /// </summary>
        Dragonhunter,
        /// <summary>
        /// Thief (not elite)
        /// </summary>
        DeadlyArts,
        /// <summary>
        /// Engineer (not elite)
        /// </summary>
        Alchemy,
        /// <summary>
        /// Ranger (not elite)
        /// </summary>
        Skirmishing,
        /// <summary>
        /// Elementalist (not elite)
        /// </summary>
        Fire,
        /// <summary>
        /// Ranger (not elite)
        /// </summary>
        Beastmastery,
        /// <summary>
        /// Ranger (not elite)
        /// </summary>
        WildernessSurvival,
        /// <summary>
        /// Necromancer (elite)
        /// </summary>
        Reaper,
        /// <summary>
        /// Thief (not elite)
        /// </summary>
        CriticalStrikes,
        /// <summary>
        /// Warrior (not elite)
        /// </summary>
        Arms,
        /// <summary>
        /// Elementalist (not elite)
        /// </summary>
        Arcane,
        /// <summary>
        /// Engineer (not elite)
        /// </summary>
        Firearms,
        /// <summary>
        /// Necromancer (not elite)
        /// </summary>
        Curses,
        /// <summary>
        /// Mesmer (elite)
        /// </summary>
        Chronomancer,
        /// <summary>
        /// Elementalist (not elite)
        /// </summary>
        Air,
        /// <summary>
        /// Guardian (not elite)
        /// </summary>
        Zeal,
        /// <summary>
        /// Engineer (elite)
        /// </summary>
        Scrapper,
        /// <summary>
        /// Thief (not elite)
        /// </summary>
        Trickery,
        /// <summary>
        /// Mesmer (not elite)
        /// </summary>
        Chaos,
        /// <summary>
        /// Guardian (not elite)
        /// </summary>
        Virtues,
        /// <summary>
        /// Engineer (not elite)
        /// </summary>
        Inventions,
        /// <summary>
        /// Elementalist (elite)
        /// </summary>
        Tempest,
        /// <summary>
        /// Guardian (not elite)
        /// </summary>
        Honor,
        /// <summary>
        /// Necromancer (not elite)
        /// </summary>
        SoulReaping,
        /// <summary>
        /// Warrior (not elite)
        /// </summary>
        Discipline,
        /// <summary>
        /// Revenant (elite)
        /// </summary>
        Herald,
        /// <summary>
        /// Necromancer (not elite)
        /// </summary>
        Spite,
        /// <summary>
        /// Thief (not elite)
        /// </summary>
        Acrobatics,
        /// <summary>
        /// Ranger (elite)
        /// </summary>
        Soulbeast,
        /// <summary>
        /// Elementalist (elite)
        /// </summary>
        Weaver,
        /// <summary>
        /// Engineer (elite)
        /// </summary>
        Holosmith,
        /// <summary>
        /// Thief (elite)
        /// </summary>
        Deadeye,
        /// <summary>
        /// Mesmer (elite)
        /// </summary>
        Mirage,
        /// <summary>
        /// Necromancer (elite)
        /// </summary>
        Scourge,
        /// <summary>
        /// Warrior (elite)
        /// </summary>
        Spellbreaker,
        /// <summary>
        /// Guardian (elite)
        /// </summary>
        Firebrand,
        /// <summary>
        /// Revenant (elite)
        /// </summary>
        Renegade,
        /// <summary>
        /// Necromancer (elite)
        /// </summary>
        Harbinger,
        /// <summary>
        /// Guardian (elite)
        /// </summary>
        Willbender,
        /// <summary>
        /// Mesmer (elite)
        /// </summary>
        Virtuoso,
        /// <summary>
        /// Elementalist (elite)
        /// </summary>
        Catalyst,
        /// <summary>
        /// Warrior (elite)
        /// </summary>
        Bladesworn,
        /// <summary>
        /// Revenant (elite)
        /// </summary>
        Vindicator,
        /// <summary>
        /// Engineer (elite)
        /// </summary>
        Mechanist,
        /// <summary>
        /// Thief (elite)
        /// </summary>
        Specter,
        /// <summary>
        /// Ranger (elite)
        /// </summary>
        Untamed
    }
}
