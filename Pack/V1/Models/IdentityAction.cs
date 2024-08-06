using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks the profession, specialization, race and commander tag status.
    /// </summary>
    /// <remarks>
    /// Currently allows values on the enums that are outside the defined range, just to make it more robust 
    /// against possible changes. 
    /// Users of this library have to handle more rigid validation themselves.
    /// </remarks>
    public class IdentityAction : Action
    {
        /// <summary>
        /// The profession of the current character. For reference take a look at 
        /// https://api.guildwars2.com/v2/professions [Optional, if any other property is set]
        /// </summary>
        public ProfessionType? Profession {  get; set; }

        /// <summary>
        /// The 3rd specialization of the current character. For reference take a look at 
        /// https://api.guildwars2.com/v2/specializations [Optional if any other property is set]
        /// </summary>
        public SpecializationType? Specialization { get; set; }

        /// <summary>
        /// The race of the current character. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:MumbleLink [Optional if any other property is set]
        /// </summary>
        /// <remarks>
        /// Mumble order does not match API order at endpoint https://api.guildwars2.com/v2/races!
        /// </remarks>
        public RaceType? Race { get; set; }

        /// <summary>
        /// Whether the character has an active commander tag. 
        /// [Optional if any other property is set]
        /// </summary>
        public bool? ActiveCommanderTag { get; set; }

        /// <summary>
        /// Instantiates a <see cref="IdentityAction"/>.
        /// </summary>
        /// <param name="profession"></param>
        /// <param name="specialization"></param>
        /// <param name="race"></param>
        /// <param name="activeCommanderTag"></param>
        [JsonConstructor]
        public IdentityAction(ProfessionType? profession, SpecializationType? specialization, RaceType? race, bool? activeCommanderTag)
        {
            Profession = profession;
            Specialization = specialization;
            Race = race;
            ActiveCommanderTag = activeCommanderTag;
        }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            if (!Profession.HasValue
                && !Specialization.HasValue
                && !Race.HasValue
                && !ActiveCommanderTag.HasValue)
            {
                return false;
            }

            if (Profession == ProfessionType.None
                || Specialization == SpecializationType.None)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"IdentityAction {this} is invalid.", this.GetType());
            }
        }
    }
}
