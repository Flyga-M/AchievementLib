using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// The action, that determines how a condition can be resolved.
    /// </summary>
    public abstract class Action : IAction
    {
        private bool _isFulfilled = false;
        private bool _freezeUpdates = false;

        private int _minimumDuration;
        private Timer _durationTimer;

        /// <inheritdoc cref="IAction.MinimumDuration"/>
        public int MinimumDuration
        {
            get => _minimumDuration;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _minimumDuration = value;
            }
        }

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

                if (oldValue == value)
                {
                    return;
                }

                if (value && _durationTimer != null)
                {
                    // prevent resetting of timer, if IsFulfilled is repeatedly set to true, while
                    // _isFulfilled has not been yet updated by the timer
                    return;
                }

                if (!value)
                {
                    // cancel timer
                    _durationTimer?.Dispose();
                    _durationTimer = null;
                }

                if (value && MinimumDuration > 0)
                {
                    // just as a precaution, _durationTimer should be null at this point
                    _durationTimer?.Dispose();
                    _durationTimer = new Timer(Fulfill, null, MinimumDuration, Timeout.Infinite);

                    // setting _isFulfilled to true will be handled by TimerCallback Fulfill
                    // that's also where the timer will be disposed and set to null again
                    return;
                }

                _isFulfilled = value;
                OnIsFulFilledChanged(value);
            }
        }

        private void Fulfill(object _)
        {
            if (IsFulfilled == true)
            {
                return;
            }

            _durationTimer?.Dispose();
            _durationTimer = null;

            _isFulfilled = true;
            OnIsFulFilledChanged(true);
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

        /// <summary>
        /// Resets the current progress of the <see cref="Action"/>.
        /// </summary>
        public void ResetProgress()
        {
            FreezeUpdates = false;
            IsFulfilled = false;
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
