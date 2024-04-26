namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Represents an <see cref="Action"/>, that uses the GW2 API to evaluates it's status.
    /// </summary>
    public abstract class ApiAction : Action
    {
        /// <summary>
        /// The Api endpoint the <see cref="Condition"/> needs to access information.
        /// </summary>
        public string Endpoint { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Endpoint);
        }
    }
}
