using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Provides extension methods for the <see cref="SpecializationType"/> enum.
    /// </summary>
    public static class SpecializationTypeExtensions
    {
        private readonly static Dictionary<ProfessionType, ReadOnlyCollection<SpecializationType>> _specializationsByProfession = new Dictionary<ProfessionType, ReadOnlyCollection<SpecializationType>>()
        {
            { ProfessionType.Guardian, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Valor,
                    SpecializationType.Radiance,
                    SpecializationType.Dragonhunter, // elite
                    SpecializationType.Zeal,
                    SpecializationType.Virtues,
                    SpecializationType.Honor,
                    SpecializationType.Firebrand, // elite
                    SpecializationType.Willbender // elite
                })
            },
            { ProfessionType.Warrior, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Strength,
                    SpecializationType.Tactics,
                    SpecializationType.Berserker, // elite
                    SpecializationType.Defense,
                    SpecializationType.Arms,
                    SpecializationType.Discipline,
                    SpecializationType.Spellbreaker, // elite
                    SpecializationType.Bladesworn // elite
                })
            },
            { ProfessionType.Engineer, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Explosives,
                    SpecializationType.Tools,
                    SpecializationType.Alchemy,
                    SpecializationType.Firearms,
                    SpecializationType.Scrapper, // elite
                    SpecializationType.Inventions,
                    SpecializationType.Holosmith, // elite
                    SpecializationType.Mechanist // elite
                })
            },
            { ProfessionType.Ranger, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Druid, // elite
                    SpecializationType.Marksmanship,
                    SpecializationType.NatureMagic,
                    SpecializationType.Skirmishing,
                    SpecializationType.Beastmastery,
                    SpecializationType.WildernessSurvival,
                    SpecializationType.Soulbeast, // elite
                    SpecializationType.Untamed // elite
                })
            },
            { ProfessionType.Thief, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Daredevil, // elite
                    SpecializationType.ShadowArts,
                    SpecializationType.DeadlyArts,
                    SpecializationType.CriticalStrikes,
                    SpecializationType.Trickery,
                    SpecializationType.Acrobatics,
                    SpecializationType.Deadeye, // elite
                    SpecializationType.Specter // elite
                })
            },
            { ProfessionType.Elementalist, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Water,
                    SpecializationType.Earth,
                    SpecializationType.Fire,
                    SpecializationType.Arcane,
                    SpecializationType.Air,
                    SpecializationType.Tempest, // elite
                    SpecializationType.Weaver, // elite
                    SpecializationType.Catalyst // elite
                })
            },
            { ProfessionType.Mesmer, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Dueling,
                    SpecializationType.Domination,
                    SpecializationType.Inspiration,
                    SpecializationType.Illusions,
                    SpecializationType.Chronomancer, // elite
                    SpecializationType.Chaos,
                    SpecializationType.Mirage, // elite
                    SpecializationType.Virtuoso // elite
                })
            },
            { ProfessionType.Necromancer, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.DeathMagic,
                    SpecializationType.BloodMagic,
                    SpecializationType.Reaper, // elite
                    SpecializationType.Curses,
                    SpecializationType.SoulReaping,
                    SpecializationType.Spite,
                    SpecializationType.Scourge, // elite
                    SpecializationType.Harbinger // elite
                })
            },
            { ProfessionType.Revenant, new ReadOnlyCollection<SpecializationType> (new[]
                {
                    SpecializationType.Invocation,
                    SpecializationType.Retribution,
                    SpecializationType.Salvation,
                    SpecializationType.Corruption,
                    SpecializationType.Devastation,
                    SpecializationType.Herald, // elite
                    SpecializationType.Renegade, // elite
                    SpecializationType.Vindicator // elite
                })
            }
        };

        private readonly static SpecializationType[] _eliteSpecializations = new[]
        {
            SpecializationType.Druid,
            SpecializationType.Daredevil,
            SpecializationType.Berserker,
            SpecializationType.Dragonhunter,
            SpecializationType.Reaper,
            SpecializationType.Chronomancer,
            SpecializationType.Scrapper,
            SpecializationType.Tempest,
            SpecializationType.Herald,
            SpecializationType.Soulbeast,
            SpecializationType.Weaver,
            SpecializationType.Holosmith,
            SpecializationType.Deadeye,
            SpecializationType.Mirage,
            SpecializationType.Scourge,
            SpecializationType.Spellbreaker,
            SpecializationType.Firebrand,
            SpecializationType.Renegade,
            SpecializationType.Harbinger,
            SpecializationType.Willbender,
            SpecializationType.Virtues,
            SpecializationType.Catalyst,
            SpecializationType.Bladesworn,
            SpecializationType.Vindicator,
            SpecializationType.Mechanist,
            SpecializationType.Specter,
            SpecializationType.Untamed
        };

        /// <summary>
        /// Contains all the <see cref="SpecializationType"/>s, ordered by their <see cref="ProfessionType"/>s.
        /// </summary>
        public static ReadOnlyDictionary<ProfessionType, ReadOnlyCollection<SpecializationType>> SpecializationsByProfession = new ReadOnlyDictionary<ProfessionType, ReadOnlyCollection<SpecializationType>>(_specializationsByProfession);

        /// <summary>
        /// Contains all the <see cref="SpecializationType"/>s, that are elite specializations.
        /// </summary>
        public static ReadOnlyCollection<SpecializationType> EliteSpecializations = new ReadOnlyCollection<SpecializationType>(_eliteSpecializations);

        /// <summary>
        /// Returns the <see cref="ProfessionType"/> that corresponds to the given <see cref="SpecializationType"/>.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns>The <see cref="ProfessionType"/> that corresponds to the given <see cref="SpecializationType"/>.</returns>
        public static ProfessionType GetProfession(this SpecializationType spec)
        {
            if (spec == SpecializationType.None)
            {
                return ProfessionType.None;
            }

            return _specializationsByProfession.FirstOrDefault(keyValuePair => keyValuePair.Value.Contains(spec)).Key;
        }

        /// <summary>
        /// Determines whether the given <see cref="SpecializationType"/> is an elite specialization.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns><see langword="true"/>, if the <see cref="SpecializationType"/> is an elite 
        /// specialization.</returns>
        public static bool IsElite(this SpecializationType spec)
        {
            return _eliteSpecializations.Contains(spec);
        }
    }
}
