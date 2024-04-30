using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    /// <summary>
    /// A condition, that needs to be fulfilled for the <see cref="IObjective"/> to be considered completed.
    /// </summary>
    public interface ICondition : IValidateable
    {
        /// <summary>
        /// The <see cref="IAction"/> carrying the data associated with the 
        /// <see cref="ICondition"/>.
        /// </summary>
        IAction Action { get; }

        /// <summary>
        /// Determines whether the <see cref="ICondition"/> is fulfilled.
        /// </summary>
        [JsonIgnore]
        bool IsFulfilled { get; }

        /// <summary>
        /// Determines whether the <see cref="ICondition"/> is fulfilled. Might throw Exceptions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>True, if the <see cref="ICondition"/> is fulfilled. Otherwise false.</returns>
        bool Check(IActionCheckContext context);

        /// <summary>
        /// Attempts to determine, whether the <see cref="ICondition"/> is fulfilled.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isFulfilled"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <see cref="ICondition"/> is eligible to be checked. Otherwise false.</returns>
        bool TryCheck(IActionCheckContext context, out bool isFulfilled, out PackSolveException exception);
    }
}
