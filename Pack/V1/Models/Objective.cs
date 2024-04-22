using Newtonsoft.Json;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AchievementLib.Pack.V1.Models
{
    public class Objective : IHierarchyObject, IValidateable
    {
        public string Id { get; set; }
        
        /// <summary>
        /// The name of the <see cref="Objective"/>.
        /// </summary>
        public Localizable Name { get; set; }

        /// <summary>
        /// The description of the <see cref="Objective"/>.
        /// </summary>
        public Localizable Description { get; set; }

        /// <summary>
        /// The amount of how much this <see cref="Objective"/> can contribute to the 
        /// <see cref="Achievement"/>.
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// The <see cref="Condition"/> that needs to be satified for the 
        /// <see cref="Objective"/> to be complete.
        /// </summary>
        public Condition Condition { get; set; }

        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        [JsonIgnore]
        public IHierarchyObject[] Children => Array.Empty<IHierarchyObject>();

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Id)
                && Name != null
                && Name.IsValid()
                && Description != null
                && Description.IsValid()
                && MaxAmount > 0
                && Condition != null
                && Condition.IsValid();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Name?.Validate();
                    Description?.Validate();
                    Condition?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"Objective {this.GetFullName()} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"Objective {this.GetFullName()} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(Objective)}: {{ " +
                $"\"Id\": {Id}, " +
                $"\"Name\": {Name}, " +
                $"\"Description\": {Description}, " +
                $"\"MaxAmount\": {MaxAmount}, " +
                $"\"Condition\": {Condition}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
