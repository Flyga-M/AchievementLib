namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks, whether the <see cref="Value"/> exists in the api response at the <see cref="ApiAction.ResultLayer"/>, 
    /// after the <see cref="ApiAction.Filter"/> is applied.
    /// </summary>
    public class ApiActionContains : ApiAction
    {
        /// <summary>
        /// The value, that the response from the Api should contain.
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Value)
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
                $"\"Value\": {Value}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
