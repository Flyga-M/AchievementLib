using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="ICondition"/>
    /// This is the V1 implementation.
    /// </summary>
    public class Condition : ICondition, IResolvable, IDisposable
    {
        private Condition _orCondition;
        private Condition _andCondition;
        private Action _action;

        private Objective _parentObjective;
        private Achievement _parentAchievement;

        private bool _isFulfilled = false;
        private bool _freezeUpdates = false;

        /// <inheritdoc/>
        public event EventHandler Resolved;

        /// <inheritdoc/>
        public event EventHandler<bool> FreezeUpdatesChanged;

        /// <inheritdoc/>
        public event EventHandler<bool> FulfilledChanged;

        /// <inheritdoc cref="ICondition.OrCondition"/>
        public Condition OrCondition
        {
            get => _orCondition;
            set
            {
                _orCondition = value;
                if (value != null)
                {
                    _orCondition.ParentCondition = this;
                }
            }
        }

        /// <inheritdoc cref="ICondition.AndCondition"/>
        public Condition AndCondition
        {
            get => _andCondition;
            set
            {
                _andCondition = value;
                if (value != null)
                {
                    _andCondition.ParentCondition = this;
                }
            }
        }

        /// <inheritdoc cref="ICondition.Action"/>
        public Action Action
        {
            get => _action;
            set
            {
                _action = value;
                if (value != null)
                {
                    _action.ParentCondition = this;
                }
            }
        }

        /// <inheritdoc cref="ICondition.Root"/>
        [JsonIgnore]
        public AchievementPackManager Root => ParentAchievement?.GetRoot() as AchievementPackManager;

        /// <inheritdoc/>
        [JsonIgnore]
        IAchievementPackManager ICondition.Root => Root;

        /// <inheritdoc/>
        [JsonIgnore]
        IAction ICondition.Action => Action;

        /// <inheritdoc/>
        [JsonIgnore]
        ICondition ICondition.OrCondition => OrCondition;

        /// <inheritdoc/>
        [JsonIgnore]
        ICondition ICondition.AndCondition => AndCondition;

        /// <inheritdoc/>
        [JsonIgnore]
        IObjective ICondition.ParentObjective => ParentObjective;

        /// <inheritdoc/>
        [JsonIgnore]
        ICondition ICondition.ParentCondition => ParentCondition;

        /// <inheritdoc/>
        [JsonIgnore]
        IAchievement ICondition.ParentAchievement => ParentAchievement;

        /// <inheritdoc cref="ICondition.ParentObjective"/>
        [JsonIgnore]
        public Objective ParentObjective
        {
            get
            {
                if (ParentCondition != null)
                {
                    return ParentCondition.ParentObjective;
                }
                return _parentObjective;
            }
            internal set => _parentObjective = value;
        }

        /// <inheritdoc cref="ICondition.ParentAchievement"/>
        [JsonIgnore]
        public Achievement ParentAchievement
        {
            get
            {
                if (ParentCondition != null)
                {
                    return ParentCondition.ParentAchievement;
                }
                if (ParentObjective != null)
                {
                    return ParentObjective.Parent as Achievement;
                }

                return _parentAchievement;
            }
            internal set => _parentAchievement = value;
        }

        /// <inheritdoc cref="ICondition.ParentCondition"/>
        [JsonIgnore]
        public Condition ParentCondition { get; internal set; }

        /// <summary>
        /// Determines whether this condition and it's additional conditions are fulfilled, regardless of 
        /// the <see cref="OrCondition"/>.
        /// </summary>
        private bool LocalIsFulfilled => Action.IsFulfilled && (AndCondition == null || AndCondition.IsFulfilled);

        private void OnIsFulfilledChanged(bool isFulfilled)
        {
            if (!FreezeUpdates)
            {
                FulfilledChanged?.Invoke(this, isFulfilled);
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsFulfilled
        {
            get => _isFulfilled;
            set
            {
                if (_isFulfilled != value)
                {
                    OnIsFulfilledChanged(value);
                }

                _isFulfilled = value;
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool FreezeUpdates
        {
            get => _freezeUpdates;
            set
            {
                bool oldValue = _freezeUpdates;
                _freezeUpdates = value;

                if (oldValue != value)
                {
                    if (value)
                    {
                        OnConditionFreeze();
                    }
                    else
                    {
                        OnConditionUnFreeze();
                    }

                    FreezeUpdatesChanged?.Invoke(this, value);
                }
            }
        }

        private void OnConditionFreeze()
        {
            Action?.FreezeUpdates();
            OrCondition?.FreezeUpdates();
            AndCondition?.FreezeUpdates();
        }

        private void OnConditionUnFreeze()
        {
            Action?.UnfreezeUpdates();

            RecalculateChildFreeze();
            RecalculateIsFulfilled();
        }

        private void RecalculateChildFreeze()
        {
            if (FreezeUpdates)
            {
                OnConditionFreeze();
                return;
            }
            
            if (LocalIsFulfilled)
            {
                OrCondition?.FreezeUpdates();
            }
            else
            {
                OrCondition?.UnfreezeUpdates();
            }

            if (!Action.IsFulfilled)
            {
                AndCondition?.FreezeUpdates();
            }
            else
            {
                AndCondition?.UnfreezeUpdates();
            }
        }

        private void OnChildFulfillmentStatusChanged(object _, bool _1)
        {
            RecalculateChildFreeze();
            RecalculateIsFulfilled();
        }

        private void RecalculateIsFulfilled()
        {
            IsFulfilled = LocalIsFulfilled
                || (OrCondition != null && OrCondition.IsFulfilled);
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsResolved
        {
            get
            {
                if (Action is IResolvable resolvable)
                {
                    return resolvable.IsResolved;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Instantiates a <see cref="Condition"/>.
        /// </summary>
        /// <param name="orCondition"></param>
        /// <param name="andCondition"></param>
        /// <param name="action"></param>
        [JsonConstructor]
        public Condition(Condition orCondition, Condition andCondition, Action action)
        {
            OrCondition = orCondition;
            AndCondition = andCondition;
            Action = action;

            if (Action != null)
            {
                Action.FulfilledChanged += OnChildFulfillmentStatusChanged;
            }
            if (OrCondition != null)
            {
                OrCondition.FulfilledChanged += OnChildFulfillmentStatusChanged;
            }
            if (AndCondition != null)
            {
                AndCondition.FulfilledChanged += OnChildFulfillmentStatusChanged;
            }
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return Action != null
                && Action.IsValid()
                && (OrCondition == null || OrCondition.IsValid())
                && (AndCondition == null || AndCondition.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Action?.Validate();
                    OrCondition?.Validate();
                    AndCondition?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"Condition {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"Condition {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(Condition)}: {{ " +
                $"\"OrCondition\": {OrCondition}, " +
                $"\"AndCondition\": {AndCondition}, " +
                $"\"Action\": {Action}, " +
                $" }}, Valid?: {IsValid()} }}";
        }

        /// <inheritdoc/>
        public void Resolve(IResolveContext context)
        {
            if (Action is IResolvable resolvable)
            {
                resolvable.Resolve(context);
            }

            Resolved?.Invoke(this, null);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            if (Action is IResolvable resolvable)
            {
                return resolvable.TryResolve(context, out exception);
            }

            Resolved?.Invoke(this, null);

            exception = null;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Also disposes <see cref="OrCondition"/>, <see cref="AndCondition"/> and <see cref="Action"/> (if applicable).
        /// </summary>
        public void Dispose()
        {
            if (Action != null)
            {
                Action.FulfilledChanged -= OnChildFulfillmentStatusChanged;
                if (Action is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            if (OrCondition != null)
            {
                OrCondition.FulfilledChanged -= OnChildFulfillmentStatusChanged;
                OrCondition.Dispose();
            }
            if (AndCondition != null)
            {
                AndCondition.FulfilledChanged -= OnChildFulfillmentStatusChanged;
                AndCondition.Dispose();
            }
        }
    }
}
