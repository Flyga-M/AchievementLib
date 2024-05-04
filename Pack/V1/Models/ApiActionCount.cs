namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Counts the amount of element in the api response, after the 
    /// <see cref="Filter"/> is applied.
    /// </summary>
    public class ApiActionCount : ApiAction
    {
        /// <summary>
        /// The <see cref="Restraint"/> that is applied to the api call before counting. 
        /// [Optional]
        /// </summary>
        public Restraint Filter { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return (Filter == null || Filter.IsValid())
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
                    Filter?.Validate();
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
                $"\"Filter\": {Filter}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
