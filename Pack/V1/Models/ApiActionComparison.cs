namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Compares the value from the given <see cref="Key"/> of all elements in 
    /// the api response, after the <see cref="Restraint"/> is applied to the 
    /// <see cref="Value"/>.
    /// </summary>
    public class ApiActionComparison : ApiAction
    {
        /// <summary>
        /// The key of the value that should be compared.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value, that the api response should be compared to.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The <see cref="Comparison"/> that evaluates the values. [Optional]
        /// </summary>
        public Comparison Comparison { get; set; }

        /// <summary>
        /// The <see cref="Restraint"/> that is applied to the api call before comparison. 
        /// [Optional]
        /// </summary>
        public Restraint Restraint { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Key)
                && !string.IsNullOrWhiteSpace(Value)
                && (Restraint == null || Restraint.IsValid())
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
                try
                {
                    Restraint?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"ApiActionComparison {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"ApiActionComparison {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(ApiActionComparison)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"Key\": {Key}, " +
                $"\"Value\": {Value}, " +
                $"\"Comparison\": {Comparison}, " +
                $"\"Restraint\": {Restraint}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
