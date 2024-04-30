using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    /// <summary>
    /// The <see cref="IAction"/>, that determines how a <see cref="ICondition"/> can be resolved.
    /// </summary>
    public interface IAction : IValidateable
    {
        /// <summary>
        /// Determines whether the <see cref="IAction"/> is fulfilled.
        /// </summary>
        [JsonIgnore]
        bool IsFulfilled { get; set; }
    }
}
