using Microsoft.Xna.Framework;
using PositionEvents.Area;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether the player is inside a specified <see cref="IBoundingObject"/> for a given 
    /// map.
    /// </summary>
    public class PositionAreaAction : Action, IValidateable
    {
        /// <summary>
        /// The <see cref="IBoundingObject"/> that is checked against the player position.
        /// </summary>
        public IBoundingObject Area { get; set; }

        /// <summary>
        /// The mapId, for which the position is checked. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:2/maps
        /// </summary>
        public int MapId { get; set; }

        public override bool IsValid()
        {
            return Area != null
                && MapId > 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"PositionAreaAction {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(PositionAreaAction)}: {{ " +
                $"\"Area\": {Area}, " +
                $"\"MapId\": {MapId}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
