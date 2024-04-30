using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// A collection of multiple <see cref="IAchievement">IAchievements</see>.
    /// </summary>
    public interface IAchievementCollection : IHierarchyObject, IValidateable
    {
        /// <summary>
        /// The name of the <see cref="IAchievementCollection"/>.
        /// </summary>
        ILocalizable Name { get; }

        /// <summary>
        /// The <see cref="IAchievement">IAchievements</see> in the 
        /// <see cref="IAchievementCollection"/>.
        /// </summary>
        IEnumerable<IAchievement> Achievements { get; }

        /// <summary>
        /// The icon that is displayed for 
        /// the <see cref="IAchievementCollection"/>. [Optional]
        /// </summary>
        Texture2D Icon { get; }
    }
}
