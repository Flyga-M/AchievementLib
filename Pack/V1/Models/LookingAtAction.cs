using Microsoft.Xna.Framework;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether the player camera is looking at a specified <see cref="Target"/> for 
    /// a given map.
    /// </summary>
    public class LookingAtAction : Action
    {
        /// <summary>
        /// The position the player should be looking at.
        /// </summary>
        public Vector3? Target {  get; set; }

        /// <summary>
        /// The mapId, for which the target is checked. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:2/maps
        /// </summary>
        public int MapId { get; set; } = -1;

        /// <summary>
        /// The angle that the viewing vector may deviate from the target to still be 
        /// considered looking at the target.
        /// </summary>
        public float ToleranceAngle { get; set; }

        public override bool IsValid()
        {
            return Target.HasValue
                && MapId > 0
                && ToleranceAngle > 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"LookingAtAction {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(LookingAtAction)}: {{ " +
                $"\"Target\": {Target}, " +
                $"\"MapId\": {MapId}, " +
                $"\"ToleranceAngle\": {ToleranceAngle}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
