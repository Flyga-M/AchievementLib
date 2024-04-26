using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class that can be loaded.
    /// </summary>
    public interface ILoadable : IDisposable
    {
        /// <summary>
        /// True, if the <see cref="ILoadableResource"/> was successfully loaded. 
        /// Otherwise false.
        /// </summary>
        [JsonIgnore]
        bool IsLoaded { get; }

        /// <summary>
        /// Loads the <see cref="ILoadable"/>. Might throw Exceptions.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        void Load(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice);

        /// <summary>
        /// Attempts to load the <see cref="ILoadable"/>.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <see cref="ILoadable"/> was sucessfully loaded. Otherwise 
        /// false.</returns>
        bool TryLoad(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, out PackResourceException exception);

        /// <summary>
        /// Loads the <see cref="ILoadable"/> asynchronously. Might 
        /// throw Exceptions.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        Task LoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice);

        /// <summary>
        /// Attempts to load the <see cref="ILoadable"/> asynchronously.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <returns>True, if the <see cref="ILoadable"/> was sucessfully loaded. Otherwise 
        /// false. Might contain information on a <see cref="PackResourceException"/>.</returns>
        Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice);
    }
}
