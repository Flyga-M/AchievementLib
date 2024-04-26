namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Copies the value from the given <see cref="Key"/> of the first element in 
    /// the api response, after the <see cref="Restraint"/> is applied.
    /// </summary>
    public class ApiActionCopy : ApiAction
    {
        /// <summary>
        /// The <see cref="Restraint"/> that is applied to the api call. [Optional]
        /// </summary>
        public Restraint Restraint { get; set; }

        /// <summary>
        /// The key whose value should be copied.
        /// </summary>
        public string Key { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return (Restraint == null || Restraint.IsValid())
                && !string.IsNullOrWhiteSpace(Key)
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
                    throw new PackFormatException($"ApiActionCopy {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"ApiActionCopy {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(ApiActionCopy)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"Key\": {Key}, " +
                $"\"Restraint\": {Restraint}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
