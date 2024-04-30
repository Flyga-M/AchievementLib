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
        bool IsFulfilled { get; }

        /// <summary>
        /// Determines whether the <see cref="IAction"/> is fulfilled. Might throw Exceptions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>True, if the <see cref="IAction"/> is fulfilled. Otherwise false.</returns>
        bool Check(IActionCheckContext context);

        /// <summary>
        /// Attempts to determine, whether the <see cref="IAction"/> is fulfilled.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isFulfilled"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <see cref="IAction"/> is eligible to be checked. Otherwise false.</returns>
        bool TryCheck(IActionCheckContext context, out bool isFulfilled, out PackSolveException exception);
    }
}
