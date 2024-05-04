namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// The response layer, that the <see cref="ApiAction"/> uses for the output.
    /// </summary>
    public class Layer : IValidateable
    {
        /// <summary>
        /// The Key that is used on the <see cref="Layer"/>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The sublayer of the <see cref="Layer"/>. [Optional]
        /// </summary>
        public Layer SubLayer { get; set; }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Key)
                && (SubLayer == null || SubLayer.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    SubLayer?.Validate();
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
                $"\"Key\": {Key}, " +
                $"\"SubLayer\": {SubLayer}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
