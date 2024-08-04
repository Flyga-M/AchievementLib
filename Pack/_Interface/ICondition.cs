namespace AchievementLib.Pack
{
    /// <summary>
    /// A condition, that needs to be fulfilled for the <see cref="IObjective"/> to be considered completed.
    /// </summary>
    public interface ICondition : IValidateable, IFulfillable
    {
        /// <summary>
        /// The <see cref="IAchievementPackManager"/> that holds the <see cref="ICondition"/>.
        /// </summary>
        IAchievementPackManager Root { get; }

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

        /// <summary>
        ///  A reference to the <see cref="ICondition"/> that may be holding this <see cref="ICondition"/>. 
        ///  Or <see langword="null"/>.
        /// </summary>
        ICondition ParentCondition { get; }

        /// <summary>
        ///  A reference to the <see cref="IAchievement"/> that may be holding this <see cref="ICondition"/>, either 
        ///  directly as a <see cref="IAchievement.ResetCondition"/>, or indirectly via a <see cref="IObjective"/>.
        /// </summary>
        IAchievement ParentAchievement{ get; }

        /// <summary>
        /// A reference to the <see cref="IObjective"/> that is holding this <see cref="Condition"/>.
        /// </summary>
        /// <remarks>
        /// Might be <see langword="null"/>, if the <see cref="ICondition"/> is a <see cref="IAchievement.ResetCondition"/>.
        /// </remarks>
        IObjective ParentObjective { get; }
    }
}
