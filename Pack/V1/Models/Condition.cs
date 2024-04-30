using MonoGame.Extended.Collections;
using Newtonsoft.Json;
using SharpDX.DirectWrite;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="ICondition"/>
    /// This is the V1 implementation.
    /// </summary>
    public class Condition : ICondition, IResolvable, IDisposable
    {
        private bool _isFulfilled = false;
        private bool _freezeUpdates = false;

        /// <inheritdoc/>
        public event EventHandler Resolved;

        /// <inheritdoc/>
        public event EventHandler<bool> FulfilledChanged;

        /// <summary>
        /// If not null, an alternative <see cref="Condition"/> that may be satisfied 
        /// instead of this <see cref="Condition"/> to be true. Functions as an 
        /// OR-condition. [Optional]
        /// </summary>
        public Condition OrCondition { get; internal set; }

        /// <summary>
        /// If not null, an additional <see cref="Condition"/> that must be satisfied 
        /// with this <see cref="Condition"/> to be true. Functions as an 
        /// AND-condition. [Optional]
        /// </summary>
        public Condition AndCondition { get; internal set; }

        /// <summary>
        /// The <see cref="Action"/> carrying the data associated with the 
        /// <see cref="Condition"/>.
        /// </summary>
        public Action Action { get; internal set; }

        /// <inheritdoc/>
        [JsonIgnore]
        IAction ICondition.Action => Action;

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
                _freezeUpdates = value;
                Action.FreezeUpdates = value;
            }
        }

        private void OnChildFulfillmentStatusChanged(object _, bool _1)
        {
            RecalculateIsFulfilled();
        }

        private void RecalculateIsFulfilled()
        {
            IsFulfilled = (Action.IsFulfilled && (AndCondition == null || AndCondition.IsFulfilled))
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
