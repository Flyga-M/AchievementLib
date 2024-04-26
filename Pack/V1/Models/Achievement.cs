using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// An Achievement that can contain multiple objectives.
    /// </summary>
    public class Achievement : IHierarchyObject, ILoadable, IValidateable
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The name of the <see cref="Achievement"/>.
        /// </summary>
        public Localizable Name { get; set; }

        /// <summary>
        /// The description for this <see cref="Achievement"/>.
        /// </summary>
        public Localizable Description { get; set; }

        /// <summary>
        /// The description for this <see cref="Achievement"/> before it is unlocked. 
        /// [Optional] if <see cref="Prerequesites"/> are empty or null.
        /// </summary>
        public Localizable LockedDescription { get; set; }

        /// <summary>
        /// The <see cref="LoadableTexture"/> to the icon that is displayed for 
        /// the <see cref="Achievement"/>. [Optional]
        /// </summary>
        public LoadableTexture Icon { get; set; }

        /// <summary>
        /// The IDs of the<see cref="Achievement">Achievements</see> that need to be 
        /// completed, before this <see cref="Achievement"/> is available. 
        /// [Optional]
        /// </summary>
        public IEnumerable<string> Prerequesites { get; set; }

        /// <summary>
        /// The tiers in which this <see cref="Achievement"/> can be completed. 
        /// The values describe how many objectives are needed to complete 
        /// the tier.
        /// </summary>
        public IEnumerable<int> Tiers { get; set; }

        /// <summary>
        /// All <see cref="Objective">Objectives</see> that are part of this 
        /// <see cref="Achievement"/>.
        /// </summary>
        public IEnumerable<Objective> Objectives { get; set; }

        /// <summary>
        /// Determines whether the <see cref="Achievement"/> can be repeated 
        /// multiple times. [Optional]
        /// </summary>
        public bool IsRepeatable { get; set; }

        /// <summary>
        /// Determines whether an <see cref="Achievement"/> is not visible, until its 
        /// <see cref="Prerequesites"/> are completed. [Optional]
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Determines at which rate the <see cref="Achievement"/> resets, if at all. 
        /// [Optional]
        /// </summary>
        public ResetType ResetType { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsLoaded => Icon?.IsLoaded ?? true;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject[] Children => Objectives.ToArray();

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && Name != null
                && Name.IsValid()
                && Description != null
                && Description.IsValid()
                && (Prerequesites == null || !Prerequesites.Any()
                    || (LockedDescription != null && LockedDescription.IsValid()))
                && (Icon == null || Icon.IsValid())
                && Tiers != null
                && Tiers.Any()
                && Objectives != null
                && Objectives.Any()
                && Objectives.All(objective => objective.IsValid());
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
                    LockedDescription?.Validate();
                    Icon?.Validate();
                    if (Objectives != null)
                    {
                        foreach (Objective objective in Objectives)
                        {
                            objective.Validate();
                        }
                    }
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"Achievement {this.GetFullName()} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"Achievement {this.GetFullName()} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public void Load(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            Icon?.Load(resourceManager, graphicsDevice);
        }

        /// <inheritdoc/>
        public async Task LoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            await Icon?.LoadAsync(resourceManager, graphicsDevice, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/> Also true, if the optional resource is null and does 
        /// not need to be loaded.</returns>
        public bool TryLoad(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, out PackResourceException exception)
        {
            exception = null;
            if (Icon == null)
            {
                return true;
            }

            return Icon.TryLoad(resourceManager, graphicsDevice, out exception);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/> Also true, if the optional resource is null and does 
        /// not need to be loaded.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            if (Icon == null)
            {
                return (true, null);
            }
            return await Icon.TryLoadAsync(resourceManager, graphicsDevice, cancellationToken);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Icon?.Dispose();

            this.DisposeChildren();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(Achievement)}: {{ " +
                $"\"Id\": {Id}, " +
                $"\"Name\": {Name}, " +
                $"\"Description\": {Description}, " +
                $"\"LockedDescription\": {LockedDescription}, " +
                $"\"Icon\": {Icon}, " +
                $"\"Prerequesites\": {{ {(Prerequesites == null ? "" : string.Join(", ", Prerequesites))} }}, " +
                $"\"Tiers\": {{ {(Tiers == null ? "" : string.Join(", ", Tiers))} }}, " +
                $"\"Objectives\": {{ {(Objectives == null ? "" : string.Join(", ", Objectives))} }}, " +
                $"\"IsRepeatable\": {IsRepeatable}, " +
                $"\"IsHidden\": {IsHidden}, " +
                $"\"ResetType\": {ResetType}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
