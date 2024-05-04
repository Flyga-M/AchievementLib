namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// This <see cref="RestraintSubLevel"/> is used to filter a deeper level of the api 
    /// response. e.g. Characters[id].Crafting[0].Discipline
    /// </summary>
    public class RestraintSubLevel : Restraint
    {
        /// <summary>
        /// The inner <see cref="Restraint"/> that filters the api response.
        /// </summary>
        public Restraint Restraint { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            return (Restraint != null && Restraint.IsValid());
        }

        /// <inheritdoc/>
        public override void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    base.Validate();
                    
                    Restraint?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"RestraintSubLevel {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"RestraintSubLevel {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {this.GetType()}: {{ " +
                $"\"OrRestraint\": {OrRestraint}, " +
                $"\"AndRestraint\": {AndRestraint}, " +
                $"\"Key\": {Key}, " +
                $"\"Filter\": {Restraint}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
