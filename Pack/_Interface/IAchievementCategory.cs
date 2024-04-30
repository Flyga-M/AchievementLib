using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// A category that can contain multiple <see cref="IAchievementCollection">IAchievementCollections</see>.
    /// </summary>
    public interface IAchievementCategory : IHierarchyObject, IValidateable
    {
        /// <summary>
        /// The name of the <see cref="IAchievementCategory"/>.
        /// </summary>
        ILocalizable Name { get; }

        /// <summary>
        /// The <see cref="IAchievementCollection">IAchievementCollections</see> in 
        /// the <see cref="IAchievementCategory"/>.
        /// </summary>
        IEnumerable<IAchievementCollection> AchievementCollections { get; }
    }
}
