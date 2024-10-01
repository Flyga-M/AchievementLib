using Newtonsoft.Json;
using System.Collections.Generic;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether the player is on a certain map.
    /// </summary>
    public class MapAction : Action
    {
        /// <summary>
        /// The mapId, that is checked. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:2/maps 
        /// [Optional, if <see cref="MapType"/> is set]
        /// </summary>
        /// <remarks>
        /// Will solely check for <see cref="MapType"/>, if <see cref="MapId"/> is <see langword="null"/>.
        /// </remarks>
        public int? MapId { get; set; }

        /// <summary>
        /// The map type, that is checked. For reference take a look at 
        /// https://wiki.guildwars2.com/wiki/API:MumbleLink
        /// [Optional, if <see cref="MapId"/> is set]
        /// </summary>
        /// <remarks>
        /// Will solely check for <see cref="MapId"/>, if <see cref="MapType"/> is <see langword="null"/>.
        /// </remarks>
        public MapType? MapType { get; set; }

        /// <summary>
        /// Instantiates a <see cref="MapAction"/>.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="mapType"></param>
        [JsonConstructor]
        public MapAction(int? mapId, MapType? mapType)
        {
            this.MapId = mapId;
            this.MapType = mapType;
        }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            if (MapId.HasValue && MapId <= 0)
            {
                return false;
            }

            if (!MapId.HasValue && !MapType.HasValue)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"MapAction {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        protected override Dictionary<string, object> InnerToString()
        {
            Dictionary<string, object> inner = base.InnerToString();

            inner.Add($"{nameof(MapId)}", MapId);
            inner.Add($"{nameof(MapType)}", MapType);

            return inner;
        }
    }
}
