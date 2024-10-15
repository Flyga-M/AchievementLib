using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="ITier"/>
    /// This is the V1 implementation.
    /// </summary>
    public class Tier : ITier
    {
        /// <inheritdoc/>
        public int Count { get; }

        /// <inheritdoc/>
        public int Points { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="points"></param>
        [JsonConstructor]
        public Tier(int count, int points)
        {
            Count = count;
            Points = points;
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return Count > 0 && Points >= 0;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"Tier {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ \"Count\": {Count}, \"Points\": {Points} }}";
        }
    }
}
