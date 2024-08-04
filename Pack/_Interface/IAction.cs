using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    /// <summary>
    /// The <see cref="IAction"/>, that determines how a <see cref="ICondition"/> can be resolved.
    /// </summary>
    public interface IAction : IValidateable, IFulfillable
    {
        /// <summary>
        /// The <see cref="IAchievementPackManager"/> that holds the <see cref="IAction"/>.
        /// </summary>
        [JsonIgnore]
        IAchievementPackManager Root { get; }

        /// <summary>
        /// A reference to the <see cref="ICondition"/> that is holding this <see cref="IAction"/>.
        /// </summary>
        [JsonIgnore]
        ICondition ParentCondition { get; }
    }
}
