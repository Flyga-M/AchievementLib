using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class that contains the data of an Achievement Pack.
    /// </summary>
    public interface IAchievementData : IHierarchyObject, IValidateable
    {
        /// <summary>
        /// The <see cref="IAchievementCategory">IAchievementCategories</see> in 
        /// the <see cref="IAchievementData"/>.
        /// </summary>
        IEnumerable<IAchievementCategory> AchievementCategories { get; }
    }
}
