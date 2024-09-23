using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a reference to an asset texture.
    /// </summary>
    public interface IResolvableTexture : IResolvable
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        int AssetId { get; }

        /// <summary>
        /// The resolved texture. Might be <see langword="null"/> if 
        /// <see cref="IResolvable.IsResolved"/> is <see langword="false"/>.
        /// </summary>
        [JsonIgnore]
        Texture2D ResolvedTexture { get; }
    }
}
