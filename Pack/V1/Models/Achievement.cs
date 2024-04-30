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
    /// <inheritdoc cref="IAchievement"/>
    /// This is the V1 implementation.
    /// </summary>
    public class Achievement : IAchievement, ILoadable, IResolvable
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc cref="IAchievement.Name"/>
        public Localizable Name { get; set; }

        /// <inheritdoc cref="IAchievement.Description"/>
        public Localizable Description { get; set; }

        /// <inheritdoc cref="IAchievement.LockedDescription"/>
        public Localizable LockedDescription { get; set; }

        /// <summary>
        /// The <see cref="LoadableTexture"/> to the icon that is displayed for 
        /// the <see cref="Achievement"/>. [Optional]
        /// </summary>
        public LoadableTexture Icon { get; set; }

        /// <summary>
        /// The <see cref="ResolvableHierarchyReference">ResolvableHierarchyReferences</see> of the 
        /// <see cref="Achievement">Achievements</see> that need to be 
        /// completed, before this <see cref="Achievement"/> is available. 
        /// [Optional]
        /// </summary>
        public IEnumerable<ResolvableHierarchyReference> Prerequesites { get; set; }

        /// <inheritdoc cref="IAchievement.Tiers"/>
        public IEnumerable<int> Tiers { get; set; }

        /// <inheritdoc cref="IAchievement.Objectives"/>
        public IEnumerable<Objective> Objectives { get; set; }

        /// <inheritdoc cref="IAchievement.IsRepeatable"/>
        public bool IsRepeatable { get; set; }

        /// <inheritdoc cref="IAchievement.IsHidden"/>
        public bool IsHidden { get; set; }

        /// <inheritdoc cref="IAchievement.ResetType"/>
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

        /// <summary>
        /// <inheritdoc/>
        /// Will only be true, if all <see cref="Prerequesites"/> and <see cref="Objectives"/> 
        /// have been successfully resolved.
        /// </summary>
        [JsonIgnore]
        public bool IsResolved
        {
            get
            {
                return (!Prerequesites.Any() || Prerequesites.All(achievement => achievement.IsResolved))
                    && (!Objectives.Any() || Objectives.All(objective => objective.IsResolved));
            }
        }

        [JsonIgnore]
        ILocalizable IAchievement.Name => Name;

        [JsonIgnore]
        ILocalizable IAchievement.Description => Description;

        [JsonIgnore]
        ILocalizable IAchievement.LockedDescription => LockedDescription;

        [JsonIgnore]
        Texture2D IAchievement.Icon => Icon.LoadedTexture;

        [JsonIgnore]
        IEnumerable<IAchievement> IAchievement.Prerequesites => Prerequesites.Select(resolvable => (IAchievement)resolvable.Reference);

        [JsonIgnore]
        IEnumerable<IObjective> IAchievement.Objectives => Objectives;

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="PackReferenceException"></exception>
        /// <inheritdoc cref="ResolvableHierarchyReference.Resolve(IResolveContext)"/>
        public void Resolve(IResolveContext context)
        {
            foreach(ResolvableHierarchyReference achievement in Prerequesites)
            {
                achievement.Resolve(context);
                if (!(achievement.Reference is IAchievement referencedAchievement))
                {
                    throw new PackReferenceException("Reference in prerequesites must be to another IAchievement. " +
                        $"Referenced type: {achievement.Reference.GetType()}.");
                }
            }

            foreach (Objective objective in Objectives)
            {
                objective.Resolve(context);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns><inheritdoc/> Will only return true, if all <see cref="Prerequesites"/> and <see cref="Objectives"/> 
        /// have been successfully resolved.</returns>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            List<PackReferenceException> exceptions = new List<PackReferenceException>();

            foreach (ResolvableHierarchyReference achievement in Prerequesites)
            {
                if (!achievement.TryResolve(context, out PackReferenceException achievementException))
                {
                    exceptions.Add(achievementException);
                    continue;
                }

                if (!(achievement.Reference is IAchievement))
                {
                    exceptions.Add(new PackReferenceException("Reference in prerequesites must be to another IAchievement. " +
                        $"Referenced type: {achievement.Reference.GetType()}."));
                }
            }

            foreach (Objective objective in Objectives)
            {
                if (!objective.TryResolve(context, out PackReferenceException objectiveException))
                {
                    exceptions.Add(objectiveException);
                }
            }

            exception = exceptions.FirstOrDefault();

            if (exceptions.Count > 1)
            {
                exception = new PackReferenceException("Multiple exceptions occured when attempting to " +
                    "resolve the Prerequesites.", new AchievementLibAggregateException(exceptions));
            }

            return !exceptions.Any();
        }
    }
}
