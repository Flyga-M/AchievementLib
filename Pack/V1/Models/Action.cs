using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

        /// <summary>
        /// A reference to the <see cref="Condition"/> that is holding this <see cref="Action"/>.
        /// </summary>
        [JsonIgnore]
        public Condition Parent { get; internal set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsFulfilled
        {
            get => _isFulfilled;
            set
            {
                if (_isFulfilled != value)
                {
                    OnIsFulFilledChanged(value);
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
                if (_freezeUpdates != value)
                {
                    FreezeUpdatesChanged?.Invoke(this, value);
                }
                _freezeUpdates = value;
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
                { "Parent Objective", Parent.Parent.GetFullName() }
            };

            return inner;
        }
    }
}
