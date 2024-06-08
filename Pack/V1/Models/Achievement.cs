using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using AchievementLib.Pack.PersistantData;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IAchievement"/>
    /// This is the V1 implementation.
    /// </summary>
    [Store]
    public class Achievement : IAchievement, ILoadable, IResolvable
    {
        private bool _isFulfilled = false;
        private bool _freezeUpdates = false;

        private int _currentTier = 1;
        private int _currentObjectives = 0;
        private bool _isUnlocked = false;

        private int _repeatedAmount = 0;
        private DateTime _lastCompletion = DateTime.MinValue;

        /// <inheritdoc/>
        public event EventHandler Resolved;

        /// <inheritdoc/>
        public event EventHandler<bool> FreezeUpdatesChanged;

        /// <inheritdoc/>
        public event EventHandler<bool> FulfilledChanged;

        /// <inheritdoc/>
        [StorageProperty(IsPrimaryKey = true)]
        public string Id { get; }

        /// <inheritdoc cref="IAchievement.Name"/>
        public Localizable Name { get; }

        /// <inheritdoc cref="IAchievement.Description"/>
        public Localizable Description { get; }

        /// <inheritdoc cref="IAchievement.LockedDescription"/>
        public Localizable LockedDescription { get; }

        /// <summary>
        /// The <see cref="LoadableTexture"/> to the icon that is displayed for 
        /// the <see cref="Achievement"/>. [Optional]
        /// </summary>
        public LoadableTexture Icon { get; }

        /// <summary>
        /// The <see cref="ResolvableHierarchyReference">ResolvableHierarchyReferences</see> of the 
        /// <see cref="Achievement">Achievements</see> that need to be 
        /// completed, before this <see cref="Achievement"/> is available. 
        /// [Optional]
        /// </summary>
        public IEnumerable<ResolvableHierarchyReference> Prerequesites { get; }

        /// <inheritdoc cref="IAchievement.Tiers"/>
        public IEnumerable<int> Tiers { get; }

        /// <inheritdoc cref="IAchievement.Objectives"/>
        public List<Objective> Objectives { get; }

        /// <inheritdoc cref="IAchievement.IsRepeatable"/>
        public bool IsRepeatable { get; }

        /// <inheritdoc cref="IAchievement.IsHidden"/>
        public bool IsHidden { get; }

        /// <inheritdoc cref="IAchievement.ResetType"/>
        public ResetType ResetType { get; }

        /// <summary>
        /// Instantiates an <see cref="Achievement"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="lockedDescription"></param>
        /// <param name="icon"></param>
        /// <param name="prerequesites"></param>
        /// <param name="tiers"></param>
        /// <param name="objectives"></param>
        /// <param name="isRepeatable"></param>
        /// <param name="isHidden"></param>
        /// <param name="resetType"></param>
        [JsonConstructor]
        public Achievement (string id, Localizable name, Localizable description, Localizable lockedDescription, LoadableTexture icon, IEnumerable<ResolvableHierarchyReference> prerequesites, IEnumerable<int> tiers, IEnumerable<Objective> objectives, bool isRepeatable, bool isHidden, ResetType resetType)
        {
            Id = id;
            Name = name;
            Description = description;
            LockedDescription = lockedDescription;
            Icon = icon;
            Prerequesites = prerequesites;
            Tiers = tiers;
            Objectives = new List<Objective>();
            IsRepeatable = isRepeatable;
            IsHidden = isHidden;
            ResetType = resetType;

            if (objectives != null)
            {
                // TODO: should throw if this returns false
                TryAddObjectives(objectives);
            }

            if (Prerequesites != null)
            {
                foreach(ResolvableHierarchyReference prerequesite in Prerequesites)
                {
                    if (prerequesite.IsResolved)
                    {
                        OnPrerequesiteResolved(prerequesite, null);
                    }

                    prerequesite.Resolved += OnPrerequesiteResolved;
                }
            }
        }

        private bool TryAddObjective(Objective objective)
        {
            if (objective == null)
            {
                return false;
            }

            if (Objectives.Contains(objective))
            {
                return false;
            }

            objective.FulfilledChanged += OnObjectiveFulfillmentStatusChanged;
            objective.Parent = this;

            Objectives.Add(objective);

            RecalculateCurrentObjectives();
            return true;
        }

        private bool TryAddObjectives(IEnumerable<Objective> objectives)
        {
            if (objectives == null)
            {
                return false;
            }

            return objectives.All(objective => TryAddObjective(objective));
        }

        private void OnPrerequesiteResolved(object prerequesite, EventArgs _)
        {
            if (!(prerequesite is ResolvableHierarchyReference reference))
            {
                throw new AchievementLibInternalException("OnPrerequesiteResolved called with a sender, that can't " +
                    $"be unboxed as ResolvableHierarchyReference. Given type: {prerequesite.GetType()}.");
            }

            if (!(reference.Reference is IAchievement achievement))
            {
                throw new AchievementLibInternalException("OnPrerequesiteResolved called with a sender, that can't " +
                    $"be resolved as IAchievement. Given type: {reference.Reference.GetType()}.");
            }

            if (!achievement.IsFulfilled)
            {
                FreezeUpdates = true;
            }

            achievement.FulfilledChanged += OnPrerequesiteFulfillmentStatusChanged;
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public int CurrentTier
        {
            get => _currentTier;
            private set
            {
                _currentTier = value;
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public int CurrentObjectives
        {
            get => _currentObjectives;
            private set
            {
                int oldValue = _currentObjectives;
                
                _currentObjectives = value;

                if (_currentObjectives != oldValue)
                {
                    RecalculateCurrentTier();
                }
            }
        }

        private void RecalculateCurrentTier()
        {
            for (int i=0; i < Tiers.Count(); i++)
            {
                int upperBound = Tiers.ElementAt(i);

                if (CurrentObjectives < upperBound)
                {
                    CurrentTier = i + 1;
                    return;
                }
            }

            CurrentTier = this.GetMaxTier();
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsUnlocked
        {
            get
            {
                if (Prerequesites == null || !Prerequesites.Any())
                {
                    return true;
                }
                return _isUnlocked;
            }
            private set
            {
                if (value && !_isUnlocked)
                {
                    FreezeUpdates = false;
                }

                if (!value && _isUnlocked)
                {
                    FreezeUpdates = true;
                }

                _isUnlocked = value;
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsVisible
        {
            get
            {
                if (!IsHidden)
                {
                    return true;
                }
                
                if (Prerequesites == null || !Prerequesites.Any())
                {
                    return true;
                }

                return IsUnlocked;
            }
        }

        private void OnPrerequesiteFulfillmentStatusChanged(object _, bool _1)
        {
            RecalculateLockedStatus();
        }

        private void RecalculateLockedStatus()
        {
            if (Prerequesites == null || !Prerequesites.Any())
            {
                IsUnlocked = true;
                return;
            }

            IsUnlocked = ((IAchievement)this).Prerequesites.All(achievement => achievement.IsFulfilled);
        }

        private void OnObjectiveFulfillmentStatusChanged(object _, bool _1)
        {
            RecalculateCurrentObjectives();
        }

        private void RecalculateCurrentObjectives()
        {
            int currentObjectives = 0;
            foreach (Objective objective in Objectives)
            {
                currentObjectives += objective.CurrentAmount;
            }
            CurrentObjectives = currentObjectives;

            if (CurrentObjectives >= MaxObjectives)
            {
                IsFulfilled = true;
            }
        }

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

        /// <summary>
        /// The maximum the <see cref="CurrentObjectives"/> can reach.
        /// </summary>
        [JsonIgnore]
        public int MaxObjectives
        {
            get
            {
                if (Tiers == null || !Tiers.Any())
                {
                    return 0;
                }

                return Tiers.Last();
            }
        }

        private void OnIsFulfilledChanged(bool isFulfilled)
        {
            if (!FreezeUpdates)
            {
                FulfilledChanged?.Invoke(this, isFulfilled);
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        [StorageProperty]
        public bool IsFulfilled
        {
            get => _isFulfilled;
            set
            {
                if (_isFulfilled == value)
                {
                    return;
                }

                _isFulfilled = value;

                if (value)
                {
                    RepeatedAmount++;
                    LastCompletion = DateTime.UtcNow;
                }

                OnIsFulfilledChanged(value);
                Storage.TryStoreProperty(this, nameof(IsFulfilled));

                if (value)
                {
                    FreezeUpdates = true;
                    if (IsRepeatable)
                    {
                        ResetProgress();
                    }
                }
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        [StorageProperty]
        public int RepeatedAmount
        {
            get => _repeatedAmount;
            private set
            {
                _repeatedAmount = value;
                Storage.TryStoreProperty(this, nameof(RepeatedAmount));
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        [StorageProperty]
        public DateTime LastCompletion
        {
            get => _lastCompletion;
            private set
            {
                _lastCompletion = value;
                Storage.TryStoreProperty(this, nameof(LastCompletion));
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool FreezeUpdates
        {
            get => _freezeUpdates;
            set
            {
                if (_freezeUpdates != value)
                {
                    FreezeUpdatesChanged?.Invoke(this, value);
                }
                _freezeUpdates = value;
                foreach (Objective objective in Objectives)
                {
                    objective.FreezeUpdates = value;
                }
            }
        }

        /// <inheritdoc/>
        public bool ResetProgress()
        {
            if (this.ResetType == ResetType.Permanent && this.IsRepeatable == false)
            {
                return false;
            }

            // will also unfreeze all objectives
            FreezeUpdates = false;

            foreach (Objective objective in Objectives)
            {
                // TODO: this should probably call a ResetProgress() method on Objective
                objective.CurrentAmount = 0;
                objective.IsFulfilled = false;
            }

            IsFulfilled = false;

            return true;
        }

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
            if (Objectives != null)
            {
                foreach (Objective objective in Objectives)
                {
                    objective.FulfilledChanged -= OnObjectiveFulfillmentStatusChanged;
                }
            }

            if (Prerequesites != null)
            {
                foreach (ResolvableHierarchyReference prerequesite in Prerequesites)
                {
                    prerequesite.Resolved -= OnPrerequesiteResolved;
                }
                foreach (IAchievement achievement in ((IAchievement)this).Prerequesites)
                {
                    achievement.FulfilledChanged -= OnPrerequesiteFulfillmentStatusChanged;
                }
            }

            Resolved = null;
            FulfilledChanged = null;

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

            Resolved?.Invoke(this, null);
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

            if (!exceptions.Any())
            {
                Resolved?.Invoke(this, null);
            }

            return !exceptions.Any();
        }

        /// <inheritdoc/>
        public bool TryAddChild(IHierarchyObject child)
        {
            if (!(child is Objective objective))
            {
                return false;
            }

            return TryAddObjective(objective);
        }
    }
}
