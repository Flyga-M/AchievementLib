using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class that can be fulfilled.
    /// </summary>
    public interface IFulfillable
    {
        /// <summary>
        /// Triggers, when <see cref="IsFulfilled"/> changes.
        /// </summary>
        event EventHandler<bool> FulfilledChanged;

        /// <summary>
        /// Triggers, when <see cref="FreezeUpdates"/> changes.
        /// </summary>
        event EventHandler<bool> FreezeUpdatesChanged;

        /// <summary>
        /// Determines whether the <see cref="IFulfillable"/> is fulfilled.
        /// </summary>
        [JsonIgnore]
        bool IsFulfilled { get; set; }

        /// <summary>
        /// Will not trigger <see cref="FulfilledChanged"/> while this is set to true.
        /// </summary>
        [JsonIgnore]
        bool FreezeUpdates { get; set; }
    }
}
