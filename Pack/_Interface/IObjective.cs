using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    /// <summary>
    /// An objective that is part of an <see cref="IAchievement"/>. Are named bits in the gw2 API V2.
    /// </summary>
    public interface IObjective : IHierarchyObject, IValidateable, IFulfillable
    {
        /// <summary>
        /// Fires, when the <see cref="CurrentAmount"/> changes.
        /// </summary>
        event EventHandler<int> CurrentAmountChanged;

        /// <summary>
        /// The name of the <see cref="IObjective"/>.
        /// </summary>
        ILocalizable Name { get; }

        /// <summary>
        /// The description for this <see cref="IObjective"/>.
        /// </summary>
        ILocalizable Description { get; }

        /// <summary>
        /// The amount of how much this <see cref="IObjective"/> can contribute to the 
        /// <see cref="IAchievement"/>.
        /// </summary>
        int MaxAmount { get; }

        /// <summary>
        /// The <see cref="ICondition"/> that needs to be satified for the 
        /// <see cref="IObjective"/> to be complete.
        /// </summary>
        ICondition Condition { get; }

        /// <summary>
        /// The amount of how much this <see cref="IObjective"/> currently contributes 
        /// to the <see cref="IAchievement"/>. Should only be used by objectives that grant partial 
        /// completion progress. If an objective <see cref="IFulfillable.IsFulfilled">IsFulfilled</see>, 
        /// <see cref="CurrentAmount"/> == <see cref="MaxAmount"/>.
        /// </summary>
        [JsonIgnore]
        int CurrentAmount { get; set; }
    }
}
