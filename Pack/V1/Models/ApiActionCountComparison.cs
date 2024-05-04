namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Compares the amount of elements of the element determined by 
    /// <see cref="ChooseOption"/> in the api response to the <see cref="Value"/> 
    /// at the <see cref="ApiAction.ResultLayer"/>, after the <see cref="ApiAction.Filter"/> is applied.
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
        /// The element that will be chosen, if the api response contains more than 1 element. 
        /// [Optional]
        /// </summary>
        public ChooseOption ChooseOption { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return Value.HasValue
                && Value >= 0
                && (Filter == null || Filter.IsValid())
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
            return $"{{ {typeof(ApiActionCountComparison)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"ResultLayer\": {ResultLayer}, " +
                $"\"Filter\": {Filter}, " +
                $"\"Value\": {Value}, " +
                $"\"Comparison\": {Comparison}, " +
                $"\"ChooseOption\": {ChooseOption}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
