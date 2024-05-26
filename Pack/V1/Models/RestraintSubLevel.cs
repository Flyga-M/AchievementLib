using Newtonsoft.Json;
using System;

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
        /// The inner <see cref="Models.Restraint"/> that filters the api response. The <see cref="Restraint.AndRestraint"/> and 
        /// <see cref="Restraint.OrRestraint"/> of the <see cref="Restraint"/> will be ignored. Use top level 
        /// <see cref="Restraint.AndRestraint"/> and <see cref="Restraint.OrRestraint"/> instead.
        /// </summary>
        public Restraint Restraint { get; set; }

        /// <summary>
        /// The value of the last <see cref="RestraintBaseLevel"/> in the inner <see cref="Restraint"/> chain.
        /// </summary>
        [JsonIgnore]
        public string Value
        {
            get
            {
                if (Restraint is RestraintBaseLevel restraintBase)
                {
                    return restraintBase.Value;
                }

                if (Restraint is RestraintSubLevel restraintSub)
                {
                    return restraintSub.Value;
                }

                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The <see cref="Comparison"/> of the last <see cref="RestraintBaseLevel"/> in the inner 
        /// <see cref="Restraint"/> chain.
        /// </summary>
        [JsonIgnore]
        public Comparison Comparison
        {
            get
            {
                if (Restraint is RestraintBaseLevel restraintBase)
                {
                    return restraintBase.Comparison;
                }

                if (Restraint is RestraintSubLevel restraintSub)
                {
                    return restraintSub.Comparison;
                }

                throw new NotImplementedException();
            }
        }

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
