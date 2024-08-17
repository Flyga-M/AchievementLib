using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Threading;
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
        /// <param name="graphicsDeviceProvider"></param>
        void Load(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider);

        /// <summary>
        /// Attempts to load the <see cref="ILoadable"/>.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDeviceProvider"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <see cref="ILoadable"/> was sucessfully loaded. Otherwise 
        /// false.</returns>
        bool TryLoad(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, out PackResourceException exception);

        /// <summary>
        /// Loads the <see cref="ILoadable"/> asynchronously. Might 
        /// throw Exceptions.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDeviceProvider"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="OperationCanceledException"></exception>
        Task LoadAsync(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, CancellationToken cancellationToken);

        /// <summary>
        /// Attempts to load the <see cref="ILoadable"/> asynchronously.
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDeviceProvider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True, if the <see cref="ILoadable"/> was sucessfully loaded. Otherwise 
        /// false. Might contain information on a <see cref="PackResourceException"/>.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, CancellationToken cancellationToken);
    }
}
