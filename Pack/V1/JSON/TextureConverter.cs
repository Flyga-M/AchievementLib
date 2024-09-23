using AchievementLib.Pack.V1.Models;
using PositionEvents.Area.JSON;
using System;
using System.Collections.Generic;

namespace AchievementLib.Pack.V1.JSON
{
    /// <summary>
    /// A converter to (de-)serialize the base V1 LoadableOrResolvableTextures in this library.
    /// </summary>
    public class TextureConverter : BasicConverter<LoadableOrResolvableTexture>
    {
        /// <summary>
        /// The default <see cref="TextureConverter"/> that only containes all the V1 LoadableOrResolvableTextures 
        /// that are part of this library.
        /// </summary>
        public static ReadOnlyBasicConverter<LoadableOrResolvableTexture> Default = new ReadOnlyBasicConverter<LoadableOrResolvableTexture>(new TextureConverter());

        /// <inheritdoc/>
        public TextureConverter() : base(GetSubTypes())
        {
            // NOOP
        }

        private static Dictionary<string, Type> GetSubTypes()
        {
            return new Dictionary<string, Type>()
            {
                { "file", typeof(LoadableTexture) },
                { "asset", typeof(ResolvableTexture) }
            };
        }
    }
}
