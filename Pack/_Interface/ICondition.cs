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

        /// <summary>
        /// If not <see langword="null"/>, an alternative <see cref="ICondition"/> that may be satisfied 
        /// instead of this <see cref="ICondition"/> to be <see langword="true"/>. Functions as an 
        /// OR-condition. [Optional]
        /// </summary>
        ICondition OrCondition { get; }

        /// <summary>
        /// If not <see langword="null"/>, an additional <see cref="ICondition"/> that must be satisfied 
        /// with this <see cref="ICondition"/> to be <see langword="true"/>. Functions as an 
        /// AND-condition. [Optional]
        /// </summary>
        ICondition AndCondition { get; }
    }
}
