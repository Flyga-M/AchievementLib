using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// A texture, that is either loaded or resolved.
    /// </summary>
    public abstract class LoadableOrResolvableTexture : ILoadableResource, IResolvableTexture
    {
        /// <summary>
        /// The loaded or resolved texture.
        /// </summary>
        /// <remarks>
        /// Might be <see langword="null"/>, if the texture was neither 
        /// loaded nor resolved.
        /// </remarks>
        public Texture2D Texture => LoadedTexture ?? ResolvedTexture;

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <summary>
        /// The loaded texture. Might be <see langword="null"/>.
        /// </summary>
        [JsonIgnore]
        public Texture2D LoadedTexture { get; protected set; }

        /// <inheritdoc/>
        public object LoadedResource => LoadedTexture;

        /// <inheritdoc/>
        public virtual bool IsLoaded => LoadedTexture != null;

        /// <inheritdoc/>
        public int AssetId { get; protected set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public Texture2D ResolvedTexture { get; protected set; }

        /// <inheritdoc/>
        public virtual bool IsResolved => ResolvedTexture != null;

        /// <inheritdoc/>
        public event EventHandler Resolved;

        /// <summary>
        /// Invokes the <see cref="Resolved"/> event.
        /// </summary>
        protected void OnResolved()
        {
            Resolved?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [JsonConstructor]
        public LoadableOrResolvableTexture(string path, int assetId)
        {
            Path = path;
            AssetId = assetId;
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            LoadedTexture?.Dispose();

            LoadedTexture = null;
        }

        /// <inheritdoc/>
        public abstract bool IsValid();

        /// <inheritdoc/>
        public virtual void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"{nameof(LoadableOrResolvableTexture)} {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public virtual void Load(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider)
        { }

        /// <inheritdoc/>
        public virtual Task LoadAsync(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual bool TryLoad(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, out PackResourceException exception)
        {
            exception = null;
            try
            {
                Load(resourceManager, graphicsDeviceProvider);
            }
            catch (Exception ex)
            {
                exception = new PackResourceException("Texture can't be loaded.", resourceManager.DataReader.GetPathRepresentation(Path), ex);
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public virtual async Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, IGraphicsDeviceProvider graphicsDeviceProvider, CancellationToken cancellationToken)
        {
            try
            {
                await LoadAsync(resourceManager, graphicsDeviceProvider, cancellationToken);
            }
            catch (Exception ex)
            {
                PackResourceException exception = new PackResourceException("Texture can't be loaded.", resourceManager.DataReader.GetPathRepresentation(Path), ex);
                return (false, exception);
            }

            return (true, null);
        }

        /// <inheritdoc/>
        public virtual void Resolve(IResolveContext context)
        {
            OnResolved();
        }

        /// <inheritdoc/>
        public virtual bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            exception = null;
            try
            {
                Resolve(context);
            }
            catch (Exception ex)
            {
                exception = new PackReferenceException("Texture can't be resolved.", AssetId.ToString(), ex);
                return false;
            }

            return true;
        }
    }
}
