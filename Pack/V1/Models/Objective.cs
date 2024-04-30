﻿using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IObjective"/>
    /// This is the V1 implementation.
    /// </summary>
    public class Objective : IObjective, IResolvable
    {
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject[] Children => Array.Empty<IHierarchyObject>();

        [JsonIgnore]
        ILocalizable IObjective.Name => Name;

        [JsonIgnore]
        ILocalizable IObjective.Description => Description;

        [JsonIgnore]
        ICondition IObjective.Condition => Condition;

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsResolved => Condition.IsResolved;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc cref="Condition.Resolve(IResolveContext)"/>
        public void Resolve(IResolveContext context)
        {
            Condition.Resolve(context);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            return Condition.TryResolve(context, out exception);
        }
    }
}
