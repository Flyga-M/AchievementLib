using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// The action, that determines how a condition can be resolved.
    /// </summary>
    public abstract class Action : IAction
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsFulfilled { get; set; } = false;

        /// <inheritdoc/>
        public abstract bool IsValid();

        /// <inheritdoc/>
        public abstract void Validate();
    }
}
