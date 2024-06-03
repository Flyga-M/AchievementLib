using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Net;

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
        /// The cosine similarity that the viewing vector and the vector pointing to the <see cref="Target"/> 
        /// must at least have to still be considered looking at the target. Must be bigger 
        /// than -1 (player can be looking in any direction) and less than 1 (player must 
        /// look exactly at the <see cref="Target"/>).
        /// </summary>
        public float CosineSimilarityTolerance { get; set; } = 1;

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return Target.HasValue
                && MapId > 0
                && CosineSimilarityTolerance > -1
                && CosineSimilarityTolerance < 1;
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

        /// <inheritdoc/>
        protected override Dictionary<string, object> InnerToString()
        {
            Dictionary<string, object> inner = base.InnerToString();

            inner.Add($"{nameof(Target)}", Target);
            inner.Add($"{nameof(MapId)}", MapId);
            inner.Add($"{nameof(CosineSimilarityTolerance)}", CosineSimilarityTolerance);

            return inner;
        }
    }
}
