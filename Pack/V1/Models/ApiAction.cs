namespace AchievementLib.Pack.V1.Models
{
    public abstract class ApiAction : Action
    {
        /// <summary>
        /// The Api endpoint the <see cref="Condition"/> needs to access information.
        /// </summary>
        public string Endpoint { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Endpoint);
        }
    }
}
