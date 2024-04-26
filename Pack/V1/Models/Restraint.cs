﻿namespace AchievementLib.Pack.V1.Models
{
    // TODO: maybe implement ComparisonType, so not only "equals" can be supported
    
    /// <summary>
    /// A restraint that is imposed on an <see cref="ApiAction"/>, to filter it's response.
    /// </summary>
    public class Restraint : IValidateable
    {
        /// <summary>
        /// If not null, an alternative <see cref="Restraint"/> that may be satisfied 
        /// instead of this <see cref="Restraint"/> to be true. Functions as an 
        /// OR-condition. [Optional]
        /// </summary>
        public Restraint OrRestraint { get; set; }

        /// <summary>
        /// If not null, an additional <see cref="Restraint"/> that must be satisfied 
        /// with this <see cref="Restraint"/> to be true. Functions as an 
        /// AND-condition. [Optional]
        /// </summary>
        public Restraint AndRestraint { get; set; }

        /// <summary>
        /// The key of the <see cref="Restraint"/>.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the <see cref="Restraint"/>.
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key)
                && !string.IsNullOrEmpty(Value)
                && (OrRestraint == null || OrRestraint.IsValid())
                && (AndRestraint == null || AndRestraint.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    OrRestraint?.Validate();
                    AndRestraint?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"Restraint {this} is invalid.", this.GetType(), ex);
                }
                
                throw new PackFormatException($"Restraint {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(Restraint)}: {{ " +
                $"\"OrRestraint\": {OrRestraint}, " +
                $"\"AndRestraint\": {AndRestraint}, " +
                $"\"Key\": {Key}, " +
                $"\"Value\": {Value}" +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
