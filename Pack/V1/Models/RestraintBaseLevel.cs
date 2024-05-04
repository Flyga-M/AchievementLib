namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class RestraintBaseLevel : Restraint
    {
        /// <summary>
        /// The value of the <see cref="Restraint"/>.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The <see cref="Comparison"/> that evaluates the values. [Optional]
        /// </summary>
        public Comparison Comparison { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }
            
            return !string.IsNullOrEmpty(Value);
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
                    throw new PackFormatException($"RestraintBaseLevel {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"RestraintBaseLevel {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {this.GetType()}: {{ " +
                $"\"OrRestraint\": {OrRestraint}, " +
                $"\"AndRestraint\": {AndRestraint}, " +
                $"\"Key\": {Key}, " +
                $"\"Value\": {Value}" +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
