namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Compares the amount of elements in the api response to the <see cref="Value"/>, 
    /// after the <see cref="Restraint"/> is applied.
    /// </summary>
    public class ApiActionCountComparison : ApiAction
    {
        /// <summary>
        /// The value, that the count should be compared to.
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// The <see cref="Comparison"/> that evaluates the count. [Optional]
        /// </summary>
        public Comparison Comparison { get; set; }

        /// <summary>
        /// The <see cref="Restraint"/> that is applied to the api call before counting. 
        /// [Optional]
        /// </summary>
        public Restraint Restraint { get; set; }

        public override bool IsValid()
        {
            return Value.HasValue
                && Value >= 0
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
                    throw new PackFormatException($"ApiActionCountComparison {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"ApiActionCountComparison {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(ApiActionCountComparison)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"Value\": {Value}, " +
                $"\"Comparison\": {Comparison}, " +
                $"\"Restraint\": {Restraint}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
