namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks, whether the <see cref="Value"/> exists in the api response.
    /// </summary>
    public class ApiActionContains : ApiAction
    {
        /// <summary>
        /// The value, that the response from the Api should contain.
        /// </summary>
        public string Value { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Value)
                && base.IsValid();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"ApiActionContains {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(ApiActionContains)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"Value\": {Value}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
