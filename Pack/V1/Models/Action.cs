﻿using Newtonsoft.Json;
using System;

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
    }
}
