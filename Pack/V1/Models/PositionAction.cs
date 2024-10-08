﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether the player is at a specified <see cref="Position"/> for a given 
    /// map.
    /// </summary>
    public class PositionAction : Action, IValidateable
    {
        /// <summary>
        /// The position that is checked against the player position.
        /// </summary>
        public Vector3? Position { get; set; }

        /// <summary>
        /// The mapId, for which the position is checked. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:2/maps
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// The amount of how much the player position can deviate from the 
        /// <see cref="Position"/> on every axis and still be considered on point.
        /// </summary>
        public float Tolerance { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return Position.HasValue
                && MapId > 0
                && Tolerance > 0;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"PositionAction {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        protected override Dictionary<string, object> InnerToString()
        {
            Dictionary<string, object> inner = base.InnerToString();

            inner.Add($"{nameof(Position)}", Position);
            inner.Add($"{nameof(MapId)}", MapId);
            inner.Add($"{nameof(Tolerance)}", Tolerance);

            return inner;
        }
    }
}
