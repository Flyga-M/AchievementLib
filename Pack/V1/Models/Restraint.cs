namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// A restraint that is imposed on an <see cref="ApiAction"/>, to filter it's response.
    /// </summary>
    public abstract class Restraint : IValidateable
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

        /// <inheritdoc/>
        public virtual bool IsValid()
        {
            return !string.IsNullOrEmpty(Key)
                && (OrRestraint == null || OrRestraint.IsValid())
                && (AndRestraint == null || AndRestraint.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public virtual void Validate()
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
                    throw new PackFormatException($"Filter {this} is invalid.", this.GetType(), ex);
                }
                
                throw new PackFormatException($"Filter {this} is invalid.", this.GetType());
            }
        }
    }
}
