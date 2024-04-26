namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// The action, that determines how a condition can be resolved.
    /// </summary>
    public abstract class Action : IValidateable
    {
        /// <inheritdoc/>
        public abstract bool IsValid();

        /// <inheritdoc/>
        public abstract void Validate();
    }
}
