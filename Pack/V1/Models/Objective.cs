using AchievementLib.Pack.PersistantData;
using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IObjective"/>
    /// This is the V1 implementation.
    /// </summary>
    [Store]
    public class Objective : IObjective, IResolvable, IDisposable
    {
        private Condition _condition;
        
        private bool _isFulfilled = false;
        private bool _freezeUpdates = false;
        private int _currentAmount = 0;

        /// <inheritdoc/>
        public event EventHandler Resolved;

        /// <inheritdoc/>
        public event EventHandler<bool> FreezeUpdatesChanged;

        /// <inheritdoc/>
        public event EventHandler<bool> FulfilledChanged;

        /// <inheritdoc/>
        public event EventHandler<int> CurrentAmountChanged;

        /// <inheritdoc/>
        public string Id { get; set; }

        [StorageProperty(IsPrimaryKey = true, ColumnName = "Id", DoNotRetrieve = true)]
        private string FullId => this.GetFullName();
        
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
        public Condition Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                if (_condition != null)
                {
                    _condition.ParentObjective = this;
                }
            }
        }

        /// <summary>
        /// Instantiates an <see cref="Objective"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="maxAmount"></param>
        /// <param name="condition"></param>
        [JsonConstructor]
        public Objective(string id, Localizable name, Localizable description, int maxAmount, Condition condition)
        {
            Id = id;
            Name = name;
            Description = description;
            MaxAmount = maxAmount;
            Condition = condition;

            if (Condition != null)
            {
                Condition.FulfilledChanged += OnChildFulfillmentStatusChanged;
            }
        }

        private void OnChildFulfillmentStatusChanged(object _, bool isFullfilled)
        {
            IsFulfilled = isFullfilled;
        }

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

        private void OnIsFulfilledChanged(bool isFulfilled, bool ignoreFreeze = false)
        {
            if (!FreezeUpdates || ignoreFreeze)
            {
                CurrentAmountChanged?.Invoke(this, CurrentAmount);
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

                Storage.TryStoreProperty(this, nameof(IsFulfilled));

                bool freezeUpdates = FreezeUpdates;

                if (value)
                {
                    FreezeUpdates = true;
                }

                // must only be called after FreezeUpdates was changed, because this may trigger an update chain
                // -> achievement completed -> reset achievement -> reset objectives -> unfreeze updates
                // and "FreezeUpdates = true" would overwrite this unfreeze.
                OnIsFulfilledChanged(value, ignoreFreeze: !freezeUpdates);
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool FreezeUpdates
        {
            get => _freezeUpdates;
            set
            {
                if (_freezeUpdates == value)
                {
                    return;
                }

                _freezeUpdates = value;

                if (Condition != null)
                {
                    Condition.FreezeUpdates = value;
                }

                FreezeUpdatesChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        [StorageProperty]
        public int CurrentAmount
        {
            get
            {
                if (IsFulfilled)
                {
                    return MaxAmount;
                }
                return _currentAmount;
            }
            set
            {
                int oldValue = _currentAmount;
                
                if (value >= MaxAmount)
                {
                    _currentAmount = MaxAmount;
                    Storage.TryStoreProperty(this, nameof(CurrentAmount));
                    IsFulfilled = true;
                    // no need to invoke CurrentAmountChanged, because it will be invoked by IsFulfilled
                    return;
                }
                _currentAmount = value;
                IsFulfilled = false; // needs to be set before storing, or the getter returns MaxAmount
                bool eval = Storage.TryStoreProperty(this, nameof(CurrentAmount));

                if (oldValue != value)
                {
                    CurrentAmountChanged?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// The id of the parent <see cref="Achievement"/>. Only used for <see cref="Storage"/> purposes.
        /// </summary>
        [JsonIgnore]
        [StorageProperty(DoNotRetrieve = true)]
        public string AchievementId => Parent.GetFullName();

        /// <summary>
        /// Resets the current progress of the <see cref="Objective"/>.
        /// </summary>
        public void ResetProgress()
        {
            FreezeUpdates = false;
            CurrentAmount = 0;

            Condition?.ResetProgress();

            IsFulfilled = false;
        }

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
            Resolved?.Invoke(this, null);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            bool success = Condition.TryResolve(context, out exception);
            
            if(success)
            {
                Resolved?.Invoke(this, null);
            }
            return success;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Also disposes the <see cref="Condition"/>.
        /// </summary>
        public void Dispose()
        {
            if (Condition != null)
            {
                Condition.FulfilledChanged -= OnChildFulfillmentStatusChanged;
                Condition.Dispose();
            }
        }

        /// <inheritdoc/> 
        /// Will always return false as <see cref="Objective"/>s have no children. 
        public bool TryAddChild(IHierarchyObject child)
        {
            return false;
        }
    }
}
