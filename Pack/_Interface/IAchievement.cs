using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// An Achievement that can contain multiple <see cref="IObjective">IObjectives</see>.
    /// </summary>
    public interface IAchievement : IHierarchyObject, IValidateable, IFulfillable
    {
        /// <summary>
        /// The name of the <see cref="IAchievement"/>.
        /// </summary>
        ILocalizable Name { get; }

        /// <summary>
        /// The description for this <see cref="IAchievement"/>.
        /// </summary>
        ILocalizable Description { get; }

        /// <summary>
        /// The description for this <see cref="IAchievement"/> before it is unlocked. 
        /// [Optional] if <see cref="Prerequesites"/> are empty or null.
        /// </summary>
        ILocalizable LockedDescription { get; }

        /// <summary>
        /// The icon that is displayed for 
        /// the <see cref="IAchievement"/>. [Optional]
        /// </summary>
        Texture2D Icon { get; }

        /// <summary>
        /// The <see cref="IAchievement">IAchievements</see> that need to be 
        /// completed, before this <see cref="IAchievement"/> is available. 
        /// [Optional]
        /// </summary>
        IEnumerable<IAchievement> Prerequesites { get; }

        /// <summary>
        /// The tiers in which this <see cref="IAchievement"/> can be completed. 
        /// The values describe how many <see cref="Objectives"/> (in total) are needed to complete 
        /// the tier.
        /// </summary>
        IEnumerable<int> Tiers { get; }

        /// <summary>
        /// All <see cref="IObjective">IObjectives</see> that are part of this 
        /// <see cref="IAchievement"/>.
        /// </summary>
        IEnumerable<IObjective> Objectives { get; }

        /// <summary>
        /// Determines whether the <see cref="IAchievement"/> can be repeated 
        /// multiple times. [Optional]
        /// </summary>
        bool IsRepeatable { get; }

        /// <summary>
        /// Determines whether an <see cref="IAchievement"/> is not visible, until its 
        /// <see cref="Prerequesites"/> are completed. [Optional]
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Determines at which rate the <see cref="IAchievement"/> resets, if at all. 
        /// [Optional]
        /// </summary>
        ResetType ResetType { get; }

        /// <summary>
        /// The current tier, that the <see cref="IAchievement"/> is completing. 1-indexed.
        /// </summary>
        [JsonIgnore]
        int CurrentTier { get; }

        /// <summary>
        /// The current achievement progress.
        /// </summary>
        [JsonIgnore]
        int CurrentObjectives { get; }

        /// <summary>
        /// Determines whether the <see cref="Prerequesites"/> are met, if there are any.
        /// </summary>
        [JsonIgnore]
        bool IsUnlocked { get; }

        /// <summary>
        /// Determines whether the <see cref="IAchievement"/> is currently hidden (<see cref="IsHidden"/> is 
        /// <see langword="true"/> and the <see cref="Prerequesites"/> are not met), or visible.
        /// </summary>
        [JsonIgnore]
        bool IsVisible { get; }
    }
}
