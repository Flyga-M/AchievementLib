using System;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Provides extension methods for the <see cref="ProfessionType"/> enum.
    /// </summary>
    public static class ProfessionTypeExtensions
    {
        /// <summary>
        /// Returns the <see cref="SpecializationType"/>s for the given <see cref="ProfessionType"/>.
        /// </summary>
        /// <param name="profession"></param>
        /// <returns>The <see cref="SpecializationType"/>s for the given <see cref="ProfessionType"/>.</returns>
        public static SpecializationType[] GetSpecializations(this ProfessionType profession)
        {
            if (profession == ProfessionType.None)
            {
                return Array.Empty<SpecializationType>();
            }

            return SpecializationTypeExtensions.SpecializationsByProfession[profession].ToArray();
        }

        /// <summary>
        /// Returns the elite <see cref="SpecializationType"/>s for the given <see cref="ProfessionType"/>.
        /// </summary>
        /// <param name="profession"></param>
        /// <returns>The elite <see cref="SpecializationType"/>s for the given <see cref="ProfessionType"/>.</returns>
        public static SpecializationType[] GetEliteSpecializations(this ProfessionType profession)
        {
            if (profession == ProfessionType.None)
            {
                return Array.Empty<SpecializationType>();
            }

            return SpecializationTypeExtensions.SpecializationsByProfession[profession].Where(spec => spec.IsElite()).ToArray();
        }
    }
}
