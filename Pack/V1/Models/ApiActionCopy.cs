namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Copies the value from the given <see cref="Key"/> of the element determined by 
    /// <see cref="ChooseOption"/> in the api response at the <see cref="ApiAction.ResultLayer"/>, 
    /// after the <see cref="ApiAction.Filter"/> is applied.
    /// </summary>
    public class ApiActionCopy : ApiAction
    {
        /// <summary>
        /// The key whose value should be copied.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The element that will be chosen, if the api response contains more than 1 element. 
        /// [Optional]
        /// </summary>
        public ChooseOption ChooseOption { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Key)
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
                $"\"ChooseOption\": {ChooseOption}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
