using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// The action, that determines how a condition can be resolved.
    /// </summary>
    public abstract class Action : IAction
    {
        private bool _isFulfilled = false;
        private bool _freezeUpdates = false;

        /// <inheritdoc/>
        public event EventHandler<bool> FreezeUpdatesChanged;

        /// <inheritdoc/>
        public event EventHandler<bool> FulfilledChanged;
        
        private void OnIsFulFilledChanged(bool isFulfilled)
        {
            if (!FreezeUpdates)
            {
                FulfilledChanged?.Invoke(this, isFulfilled);
            }
        }

        /// <inheritdoc cref="IAction.Root"/>
        [JsonIgnore]
        public AchievementPackManager Root => ParentCondition?.Root;

        /// <inheritdoc/>
        [JsonIgnore]
        IAchievementPackManager IAction.Root => Root;

        /// <inheritdoc cref="IAction.ParentCondition"/>
        [JsonIgnore]
        public Condition ParentCondition { get; internal set; }

        /// <inheritdoc/>
        [JsonIgnore]
        ICondition IAction.ParentCondition => ParentCondition;

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsFulfilled
        {
            get => _isFulfilled;
            set
            {   
                bool oldValue = _isFulfilled;
                _isFulfilled = value;

                if (oldValue != value)
                {
                    OnIsFulFilledChanged(value);
                }
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
                    FreezeUpdatesChanged?.Invoke(this, value);
                }
            }
        }

        /// <inheritdoc/>
        public abstract bool IsValid();

        /// <inheritdoc/>
        public abstract void Validate();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {this.GetType()}: {{ " +
                $"{FormatInnerToString()}" +
                $" }}, Valid?: {IsValid()} }}";
        }

        private string FormatInnerToString()
        {
            Dictionary<string, object> inner = InnerToString();

            if (inner == null || !inner.Any())
            {
                return string.Empty;
            }

            return string.Join(", ", inner.Select(kvp => $"\"{kvp.Key}\": {kvp.Value}"));
        }

        /// <summary>
        /// Adds additional information to the <see cref="ToString"/> method.
        /// </summary>
        protected virtual Dictionary<string, object> InnerToString()
        {
            Dictionary<string, object> inner = new Dictionary<string, object>()
            {
                { "Parent Objective", ParentCondition?.ParentObjective?.GetFullName() }
            };

            return inner;
        }
    }
}
