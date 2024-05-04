namespace AchievementLib.Pack
{
    /// <summary>
    /// A condition, that needs to be fulfilled for the <see cref="IObjective"/> to be considered completed.
    /// </summary>
    public interface ICondition : IValidateable, IFulfillable
    {
        /// <summary>
        /// The <see cref="IAction"/> carrying the data associated with the 
        /// <see cref="ICondition"/>.
        /// </summary>
        IAction Action { get; }
    }
}
