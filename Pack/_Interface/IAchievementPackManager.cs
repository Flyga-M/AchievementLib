using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Manages the state and contents of an Achievement Pack.
    /// </summary>
    public interface IAchievementPackManager : IDisposable, IHierarchyObject
    {
        /// <summary>
        /// The <see cref="IManifest"/>, that contains information on the 
        /// <see cref="IManifest.PackVersion"/>.
        /// </summary>
        IManifest Manifest { get; }

        /// <summary>
        /// Fires, once the pack was successfully enabled and loaded via 
        /// <see cref="Enable(GraphicsDevice, IResolveContext, out Task)"/>.
        /// </summary>
        event EventHandler PackLoaded;

        /// <summary>
        /// Fires, once the pack was successfully disabled and unloaded via
        /// <see cref="Disable"/>.
        /// </summary>
        event EventHandler PackUnloaded;

        /// <summary>
        /// Fires every time the <see cref="State"/> changes.
        /// </summary>
        event EventHandler<PackLoadState> PackLoadStateChanged;

        /// <summary>
        /// Fires, if an error occurs during Enabling"/>.
        /// </summary>
        event EventHandler<AchievementLibException> PackError;

        /// <summary>
        /// The <see cref="PackLoadState"/> of the <see cref="IAchievementPackManager"/>.
        /// </summary>
        PackLoadState State { get; }

        /// <summary>
        /// The <see cref="IPackLoadReport"/> of the Achievement Pack. Contains 
        /// information on the (un)successfully loading of the Achievement Pack.
        /// </summary>
        IPackLoadReport Report { get; }

        /// <summary>
        /// The <see cref="IAchievementCategory"/>ies of the Achievement Pack. The seperate files are combined where 
        /// namespaces overlap.
        /// </summary>
        IAchievementCategory[] Categories { get; }

        /// <summary>
        /// Attempts to enable the <see cref="IAchievementPackManager"/> and load it's data. 
        /// The data is loaded asynchronously and is not available 
        /// directly after <see cref="Enable(GraphicsDevice, IResolveContext, out Task)"/> was called. Listen to 
        /// <see cref="PackLoaded"/> and <see cref="PackError"/> to make sure, the 
        /// data is available.
        /// </summary>
        /// <remarks>
        /// The <paramref name="loadingTask"/> should be awaited before disposing the 
        /// <paramref name="graphicsDevice"/> or its context.
        /// </remarks>
        /// <param name="graphicsDevice"></param>
        /// <param name="loadingTask"></param>
        /// <param name="resolveContext"></param>
        /// <returns>True, if the <see cref="IAchievementPackManager"/> is eligible to be enabled. 
        /// Otherwise false.</returns>
        bool Enable(GraphicsDevice graphicsDevice, IResolveContext resolveContext, out Task loadingTask);

        /// <summary>
        /// Attempts to disable the <see cref="IAchievementPackManager"/> and free it's resources by 
        /// disposing of any <see cref="IDisposable"/> children.
        /// </summary>
        /// <param name="forceDisable"></param>
        /// <returns>True, if the <see cref="IAchievementPackManager"/> is eligible to be disabled. 
        /// Otherwise false.</returns>
        bool Disable(bool forceDisable);
    }
}
