namespace AchievementLib.Pack.V1.Models
{
    // TODO: maybe support choice between ALL and ANY mode.
    
    /// <summary>
    /// Compares the value from the given <see cref="Key"/> of all elements in 
    /// the api response at the <see cref="ApiAction.ResultLayer"/>, after the 
    /// <see cref="ApiAction.Filter"/> is applied, to the <see cref="Value"/>.
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

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Key)
                && !string.IsNullOrWhiteSpace(Value)
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
                    base.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"{this.GetType()} {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"{this.GetType()} {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {this.GetType()}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"ResultLayer\": {ResultLayer}, " +
                $"\"Filter\": {Filter}, " +
                $"\"Key\": {Key}, " +
                $"\"Value\": {Value}, " +
                $"\"Comparison\": {Comparison}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
