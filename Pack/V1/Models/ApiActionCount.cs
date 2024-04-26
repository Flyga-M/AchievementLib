namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Counts the amount of element in the api response, after the 
    /// <see cref="Restraint"/> is applied.
    /// </summary>
    public class ApiActionCount : ApiAction
    {
        /// <summary>
        /// The <see cref="Restraint"/> that is applied to the api call before counting. 
        /// [Optional]
        /// </summary>
        public Restraint Restraint { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return (Restraint == null || Restraint.IsValid())
                && base.IsValid();
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Restraint?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"ApiActionCount {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"ApiActionCount {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(ApiActionCount)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"Restraint\": {Restraint}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
