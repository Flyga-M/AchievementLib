using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="ILoadableResource"/> 
    /// In this implementation the resource is a <see cref="Texture2D"/>.
    /// </summary>
    public class LoadableTexture : ILoadableResource
    {
        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public string ActualPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Path))
                {
                    return "";
                }

                return System.IO.Path.Combine(V1Constants.RESOURCE_DIRECTORY, Path);
            }
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsLoaded => LoadedTexture != null;

        /// <summary>
        /// The loaded texture. Might be null, if <see cref="ILoadable.IsLoaded"/> == false.
        /// </summary>
        [JsonIgnore]
        public Texture2D LoadedTexture { get; private set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public object LoadedResource => LoadedTexture;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackResourceException"></exception>
        public void Load(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }

            if (string.IsNullOrWhiteSpace(ActualPath))
            {
                throw new InvalidOperationException("Path must have a value.");
            }

            if (!resourceManager.ResourceExists(ActualPath))
            {
                throw new FileNotFoundException("Resource does not exist.", resourceManager.DataReader.GetPathRepresentation(ActualPath));
            }

            Texture2D loadedTexture;

            try
            {
                Stream fileStream = resourceManager.DataReader.GetFileStream(ActualPath);
                loadedTexture = Texture2D.FromStream(graphicsDevice, fileStream);
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new PackResourceException("Resource could not be loaded as " +
                    $"{nameof(Texture2D)}", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
            }

            if (loadedTexture == null)
            {
                throw new PackResourceException("Resource could not be loaded as " +
                    $"{nameof(Texture2D)}", resourceManager.DataReader.GetPathRepresentation(ActualPath));
            }

            LoadedTexture = loadedTexture;
        }

        /// <inheritdoc/>
        public bool TryLoad(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, out PackResourceException exception)
        {
            exception = null;
            
            try
            {
                Load(resourceManager, graphicsDevice);
            }
            catch (ArgumentNullException ex)
            {
                exception = new PackResourceException("resourceManager can't be null.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                exception = new PackResourceException("path can't be empty.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return false;
            }
            catch (FileNotFoundException ex)
            {
                exception = new PackResourceException("Resource file does not exist.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return false;
            }
            catch (PackResourceException ex)
            {
                exception = new PackResourceException("Resource could not be loaded.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PackResourceException"></exception>
        public async Task LoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            if (resourceManager == null)
            {
                throw new ArgumentNullException(nameof(resourceManager));
            }

            if (string.IsNullOrWhiteSpace(ActualPath))
            {
                throw new InvalidOperationException("Path must have a value.");
            }

            if (!resourceManager.ResourceExists(ActualPath))
            {
                throw new FileNotFoundException("Resource does not exist.", resourceManager.DataReader.GetPathRepresentation(ActualPath));
            }

            Texture2D loadedTexture;

            try
            {
                Stream fileStream = await resourceManager.LoadResourceStreamAsync(ActualPath);
                loadedTexture = Texture2D.FromStream(graphicsDevice, fileStream);
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new PackResourceException("Resource could not be loaded as " +
                    $"{nameof(Texture2D)}", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
            }

            if (loadedTexture == null)
            {
                throw new PackResourceException("Resource could not be loaded as " +
                    $"{nameof(Texture2D)}", resourceManager.DataReader.GetPathRepresentation(ActualPath));
            }

            LoadedTexture = loadedTexture;
        }

        /// <inheritdoc/>
        public async Task<(bool, PackResourceException)> TryLoadAsync(AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            try
            {
                await LoadAsync(resourceManager, graphicsDevice);
            }
            catch (ArgumentNullException ex)
            {
                PackResourceException exception = new PackResourceException("resourceManager can't be null.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return (false, exception);
            }
            catch (InvalidOperationException ex)
            {
                PackResourceException exception = new PackResourceException("path can't be empty.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return (false, exception);
            }
            catch (FileNotFoundException ex)
            {
                PackResourceException exception = new PackResourceException("Resource file does not exist.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return (false, exception);
            }
            catch (PackResourceException ex)
            {
                PackResourceException exception = new PackResourceException("Resource could not be loaded.", resourceManager.DataReader.GetPathRepresentation(ActualPath), ex);
                return (false, exception);
            }

            return (true, null);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            LoadedTexture?.Dispose();

            LoadedTexture = null;
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ActualPath);
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"LoadableTexture {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(LoadableTexture)}: {{ " +
                $"\"Path\": {Path}, " +
                $"\"ActualPath\": {ActualPath}, " +
                $"\"IsLoaded\": {IsLoaded}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
