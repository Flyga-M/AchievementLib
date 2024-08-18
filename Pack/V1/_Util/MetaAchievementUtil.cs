using AchievementLib.Pack.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1
{
    /// <summary>
    /// Provides utility functions to generate <see cref="Objective"/>s from 
    /// <see cref="ResolvableHierarchyReference"/>s. This is useful for generating 
    /// Meta Achievements for a list of achievements.
    /// </summary>
    public static class MetaAchievementUtil
    {
        private static int _fallbackCounter = 0;

        /// <summary>
        /// Returns an array of <see cref="Objective"/>s, that contain <see cref="Condition"/>s 
        /// with <see cref="AchievementAction"/>s referencing the <paramref name="achievements"/>.
        /// </summary>
        /// <param name="achievements"></param>
        /// <returns></returns>
        public static Objective[] GetMetaObjectives(IEnumerable<ResolvableHierarchyReference> achievements)
        {
            List<Objective> result = new List<Objective>();

            foreach (ResolvableHierarchyReference achievement in achievements)
            {
                result.Add(GetMetaObjective(achievement));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns an <see cref="Objective"/> that contains a <see cref="Condition"/> with 
        /// an <see cref="AchievementAction"/> referencing the given 
        /// <paramref name="achievement"/>.
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns>An <see cref="Objective"/> that contains a <see cref="Condition"/> with 
        /// an <see cref="AchievementAction"/> referencing the given 
        /// <paramref name="achievement"/>.</returns>
        public static Objective GetMetaObjective(ResolvableHierarchyReference achievement)
        {
            return new Objective(
                GetIdFromReferenceId(achievement.ReferenceId),
                new Localizable(),
                new Localizable(),
                1,
                new Condition(
                    null,
                    null,
                    new AchievementAction(achievement)
                )
            );
        }

        private static string GetIdFromReferenceId(string referenceId)
        {
            return referenceId.Split('.').LastOrDefault() ?? $"MISSING_ID{_fallbackCounter++}";
        }
    }
}
