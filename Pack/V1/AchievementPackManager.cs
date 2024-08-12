using AchievementLib.Pack.Content;
using AchievementLib.Pack.V1.Models;
using AchievementLib.Pack.Reader;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;
using AchievementLib.Pack.PersistantData;
using System.Data.SQLite;

namespace AchievementLib.Pack.V1
{
    /// <summary>
    /// <inheritdoc cref="IAchievementPackManager"/>
    /// This is the implementation of the V1 AchievementPackManager.
    /// </summary>
    [Store]
    public class AchievementPackManager : IAchievementPackManager
    {
        private readonly IDataReader _dataReader;

        private Manifest _manifest;

        private IEnumerable<JsonConverter> _customConverters;

        private AchievementData[] _data;

        private PackLoadState _state = PackLoadState.Unloaded;
        private object _packStateLock = new object();

        private bool _isEnabled;

        private PackLoadReport _report;

        private CancellationTokenSource _cancellationSourceEnable = new CancellationTokenSource();

        /// <summary>
        /// Fires, once the pack was successfully enabled and loaded via 
        /// <see cref="Enable(GraphicsDevice, IResolveContext, out Task)"/>.
        /// </summary>
        public event EventHandler PackLoaded;

        /// <summary>
        /// Fires, once the pack was successfully disabled and unloaded via
        /// <see cref="Disable"/>.
        /// </summary>
        public event EventHandler PackUnloaded;

        /// <summary>
        /// Fires every time the <see cref="State"/> changes.
        /// </summary>
        public event EventHandler<PackLoadState> PackLoadStateChanged;

        /// <summary>
        /// Fires, if an error occurs during <see cref="Enable(GraphicsDevice, IResolveContext, out Task)"/>.
        /// </summary>
        public event EventHandler<AchievementLibException> PackError;

        private void OnPackLoaded()
        {
            PackLoaded?.Invoke(this, null);
        }

        private void OnPackUnloaded()
        {
            PackUnloaded?.Invoke(this, null);
        }

        private void OnPackLoadStateChanged(PackLoadState state)
        {
            PackLoadStateChanged?.Invoke(this, state);
            if (state == PackLoadState.Unloaded)
            {
                OnPackUnloaded();
            }
            else if (state == PackLoadState.Loaded)
            {
                OnPackLoaded();
            }
        }

        private void OnPackError(AchievementLibException ex)
        {
            IsEnabled = false;

            PackError?.Invoke(this, ex);
        }

        /// <summary>
        /// The <see cref="PackLoadState"/> of the <see cref="AchievementPackManager"/>.
        /// </summary>
        public PackLoadState State
        {
            get
            {
                if (_packStateLock == null) // the state should still be retrievable for logging purposes, after the manager was disposed. 
                {
                    return PackLoadState.Unloaded;
                }

                lock (_packStateLock)
                {
                    return _state;
                }
            }
            private set
            {
                lock (_packStateLock)
                {
                    PackLoadState oldValue = _state;
                    _state = value;

                    if (oldValue != value)
                    {
                        OnPackLoadStateChanged(value);
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="PackLoadReport"/> of the Achievement Pack. Contains 
        /// information on the (un)successfully loading of the Achievement Pack.
        /// </summary>
        public PackLoadReport Report => _report;

        IPackLoadReport IAchievementPackManager.Report => Report;

        /// <summary>
        /// The file name of the Achievement Pack without the .zip extension.
        /// </summary>
        public string FileName => Path.GetFileNameWithoutExtension(_dataReader.GetPathRepresentation());

        /// <summary>
        /// The <see cref="Models.Manifest"/> of the Achievement Pack.
        /// </summary>
        public Manifest Manifest => _manifest;

        IManifest IAchievementPackManager.Manifest => _manifest;

        /// <summary>
        /// The raw <see cref="AchievementData"/> of the Achievement Pack. Every entry 
        /// represents a seperate file in the Achievement Pack. Might have overlapping 
        /// namespaces.
        /// </summary>
        public AchievementData[] Data => _data;

        /// <inheritdoc/>
        public IAchievementCategory[] Categories
        {
            get
            {
                CombineData();

                return _data?.SelectMany(data => data.AchievementCategories).ToArray() ?? Array.Empty<IAchievementCategory>();
            }
        }

        /// <summary>
        /// The resource manager of the <see cref="AchievementPackManager"/>.
        /// </summary>
        public AchievementPackResourceManager ResourceManager { get; private set; }

        /// <inheritdoc/>
        public string Id => Manifest?.Namespace;

        /// <inheritdoc/>
        [StorageProperty(IsPrimaryKey = true, ColumnName = "Id", DoNotRetrieve = true)]
        public string FullId => this.GetFullName();

        /// <inheritdoc/>
        public IHierarchyObject Parent
        {
            get
            {
                return null;
            }
            set
            {
                if (value != null)
                {
                    throw new ArgumentException($"Parent of {nameof(AchievementPackManager)} " +
                        $"must be null.", nameof(value));
                }
            }
        }

        /// <inheritdoc/>
        public IHierarchyObject[] Children => Data;

        /// <inheritdoc/>
        [StorageProperty]
        public bool IsEnabled
        {
            get => _isEnabled;
            private set
            {
                bool oldValue = _isEnabled;
                _isEnabled = value;
                if (oldValue != _isEnabled)
                {
                    Storage.TryStoreProperty(this, nameof(IsEnabled));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="manifest"></param>
        /// <param name="customConverters"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AchievementPackManager(IDataReader dataReader, Manifest manifest, IEnumerable<JsonConverter> customConverters)
        {   
            _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));

            _customConverters = customConverters;

            ResourceManager = new AchievementPackResourceManager(dataReader);

            _report = new PackLoadReport();

            _data = Array.Empty<AchievementData>();
        }

        private bool TryAddAchievementData(AchievementData data)
        {
            if (data == null)
            {
                return false;
            }

            if (_data.Contains(data))
            {
                return false;
            }

            data.Parent = this;

            List<AchievementData> newData = new List<AchievementData>(_data)
            {
                data
            };

            _data = newData.ToArray();

            return true;
        }

        private bool TryAddAchievementData(IEnumerable<AchievementData> data)
        {
            if (data == null)
            {
                return false;
            }

            List<AchievementData> newData = new List<AchievementData>(_data);

            foreach (AchievementData date in data)
            {
                if (newData.Contains(date))
                {
                    return false;
                }

                date.Parent = this;
                newData.Add(date);
            }

            _data = newData.ToArray();
            return true;
        }

        /// <summary>
        /// Creates an <see cref="AchievementPackManager"/> from a .zip archive file.
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="manifest"></param>
        /// <param name="customConverters"></param>
        /// <returns>The corresponding <see cref="AchievementPackManager"/>.</returns>
        public static AchievementPackManager FromArchivedPack(string archivePath, Manifest manifest, IEnumerable<JsonConverter> customConverters) => new AchievementPackManager(new ZipArchiveReader(archivePath), manifest, customConverters);

        /// <summary>
        /// Attempts to enable the Achievement Pack and load it's data into 
        /// <see cref="Data"/>. The data is loaded asynchronously and is not available 
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
        /// <returns>True, if the Achievement Pack is eligible to be enabled. 
        /// Otherwise false.</returns>
        public bool Enable(GraphicsDevice graphicsDevice, IResolveContext resolveContext, out Task loadingTask)
        {
            loadingTask = null;
            if (State != PackLoadState.Unloaded)
            {
                return false;
            }

            IsEnabled = true;

            State = PackLoadState.Loading;

            loadingTask = Load(graphicsDevice, resolveContext, _cancellationSourceEnable.Token);
            
            return true;
        }

        private void OnLoadCancelled()
        {
            // Currently NOOP
        }

        /// <exception cref="OperationCanceledException"></exception>
        private async Task Load(GraphicsDevice graphicsDevice, IResolveContext resolveContext, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                OnLoadCancelled();
                return;
            }

            bool achievementSuccess;
            PackFormatException ex;

            try
            {
                (achievementSuccess, ex) = await TryLoadAchievements(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                OnLoadCancelled();
                return;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                OnLoadCancelled();
                return;
            }

            if (!achievementSuccess)
            {
                _report.FaultyData = true;
                _report.Exception = ex;
                State = PackLoadState.FatalError;

                OnPackError(ex);
                return;
            }

            _report.FaultyData = false;

            if (cancellationToken.IsCancellationRequested)
            {
                OnLoadCancelled();
                return;
            }

            if (!TryResolveReferences(resolveContext, out PackReferenceException referenceException))
            {
                _report.FaultyReferences = true;
                _report.Exception = referenceException;
                State = PackLoadState.FatalError;

                OnPackError(referenceException);
                return;
            }

            _report.FaultyReferences = false;

            if (cancellationToken.IsCancellationRequested)
            {
                OnLoadCancelled();
                return;
            }

            bool resourceSuccess;
            PackResourceException[] exceptions;

            try
            {
                (resourceSuccess, exceptions) = await TryLoadResourcesAsync(graphicsDevice, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                OnLoadCancelled();
                return;
            }

            if (!resourceSuccess)
            {
                _report.FaultyResources = true;
                _report.Exception = new AchievementLibAggregateException("Some resources could not be loaded.", exceptions);

                // not a fatal error
            }
            else
            {
                _report.FaultyResources = false;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                OnLoadCancelled();
                return;
            }
            else
            {
                State = PackLoadState.Loaded;
            }

            ReleaseLocks();
        }

        /// <exception cref="PackReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void ResolveReferences(IResolveContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!this.TryResolveChildren(context, out PackReferenceException[] packExceptions))
            {
                throw new PackReferenceException($"Unable to resolve references for pack " +
                    $"{this.Manifest.GetDetailedName()}", new AchievementLibAggregateException(packExceptions));
            }
        }

        private bool TryResolveReferences(IResolveContext context, out PackReferenceException exception)
        {
            try
            {
                ResolveReferences(context);
            }
            catch (PackReferenceException ex)
            {
                exception = ex;
                return false;
            }
            catch (ArgumentNullException ex)
            {
                exception = new PackReferenceException($"Resolving of references failed for pack " +
                    $"{this.Manifest.GetDetailedName()}. An internal exception occured.",
                    new AchievementLibInternalException("context must be set to a valid object.", ex));
                return false;
            }
            catch (Exception ex)
            {
                exception = new PackReferenceException($"Resolving of references failed for pack " +
                    $"{this.Manifest.GetDetailedName()}.", ex);
                return false;
            }

            exception = null;
            return true;
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="AggregateException"></exception>
        private void LoadResources(GraphicsDevice graphicsDevice)
        {
            if (State != PackLoadState.Loaded)
            {
                throw new InvalidOperationException("pack must be successfully loaded " +
                    "before resources can be loaded.");
            }

            if (!this.TryLoadChildrensResources(ResourceManager, graphicsDevice, out PackResourceException[] exceptions))
            {
                throw new AggregateException(exceptions);
            }
        }

        private bool TryLoadResources(GraphicsDevice graphicsDevice, out PackResourceException[] exceptions)
        {
            try
            {
                LoadResources(graphicsDevice);
            }
            catch (InvalidOperationException ex)
            {
                exceptions = new PackResourceException[] { new PackResourceException("pack was not loaded " +
                    "before attempting to load resources.", ex) };
                return false;
            }
            catch (AggregateException ex)
            {
                List<PackResourceException> allExceptions = new List<PackResourceException>();

                foreach (Exception exception in ex.InnerExceptions)
                {
                    if (exception is PackResourceException resourceException)
                    {
                        allExceptions.Add(resourceException);
                    }
                }

                exceptions = allExceptions.ToArray();
                return false;
            }

            exceptions = Array.Empty<PackResourceException>();
            return true;
        }

        /// <summary>
        /// Throws, if at least one resource was not loaded successfully.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        private async Task LoadResourcesAsync(GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool success = false;
            PackResourceException[] exceptions = Array.Empty<PackResourceException>();

            try
            {
                (success, exceptions) = await this.TryLoadChildrensResourcesAsync(ResourceManager, graphicsDevice, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            

            if (!success)
            {
                throw new AggregateException(exceptions);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Returns false, if at least one resource was not loaded successfully. Will 
        /// strill try to attempt to load the other resources.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException"></exception>
        private async Task<(bool, PackResourceException[])> TryLoadResourcesAsync(GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                await LoadResourcesAsync(graphicsDevice, cancellationToken);
            }
            catch (AggregateException ex)
            {
                List<PackResourceException> allExceptions = new List<PackResourceException>();

                foreach (Exception exception in ex.InnerExceptions)
                {
                    if (exception is PackResourceException resourceException)
                    {
                        allExceptions.Add(resourceException);
                    }
                }

                PackResourceException[] exceptions = allExceptions.ToArray();
                return (false, exceptions);
            }
            catch (OperationCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            return (true, Array.Empty<PackResourceException>());
        }

        private bool CombineData()
        {
            if (_data == null || _data.Length == 0)
            {
                return true;
            }

            return this.TryCombineChildren();
        }

        /// <inheritdoc/>
        public bool Disable(bool forceDisable)
        {
            if (State == PackLoadState.Unloading
                || State == PackLoadState.Unloaded
                || State == PackLoadState.FatalError)
            {
                return false;
            }
            
            if (!forceDisable
                && State == PackLoadState.Loading)
                
            {
                return false;
            }

            if (forceDisable)
            {
                _cancellationSourceEnable.Cancel();
                _cancellationSourceEnable = new CancellationTokenSource();
            }

            State = PackLoadState.Unloading;

            this.DisposeChildren();

            _data = null;
            _report.FaultyData = null;
            _report.FaultyReferences = null;
            _report.FaultyResources = null;
            _report.Exception = null;

            State = PackLoadState.Unloaded;

            IsEnabled = false;

            return true;
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PackFormatException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        private async Task LoadAchievements(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_dataReader == null)
            {
                throw new InvalidOperationException("_dataReader must be != null.");
            }
            
            _data = Array.Empty<AchievementData>();

            List<AchievementData> data = new List<AchievementData>();

            List<(Stream, IDataReader, string)> candidates = new List<(Stream fileStream, IDataReader dataReader, string fileName)>();

            cancellationToken.ThrowIfCancellationRequested();

            await _dataReader.LoadOnFileTypeAsync((fileStream, dataReader, fileName) =>
            {
                candidates.Add((fileStream, dataReader, fileName));
                return Task.CompletedTask;
            }, ".json");

            cancellationToken.ThrowIfCancellationRequested();

            foreach ((Stream fileStream, IDataReader dataReader, string fileName) in candidates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (fileName == Path.GetFileNameWithoutExtension(Constants.MANIFEST_NAME))
                {
                    // ignore manifest here
                    fileStream?.Dispose();
                    continue;
                }

                AchievementData achievementData;

                try
                {
                    achievementData = AchievementPackReader.
                    DeserializeV1FromJson(fileStream, _customConverters);
                }
                catch (JsonSerializationException ex)
                {
                    throw new PackFormatException("Attempted to " +
                    $"load an invalid Achievement Pack ({FileName}): {fileName} " +
                    "appears to be malformed.", ex);
                }
                catch (NotImplementedException ex)
                {
                    throw new PackFormatException("Attempted to " +
                   $"load an invalid Achievement Pack ({FileName}): {fileName} " +
                   "appears to be malformed. The given type indicator is not currently supported.", ex);
                }
                finally
                {
                    fileStream.Dispose();
                }


                if (achievementData == null)
                {
                    throw new PackFormatException("Attempted to " +
                    $"load an invalid Achievement Pack ({FileName}): {fileName} " +
                    "appears to be malformed.");
                }

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    achievementData.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException("Attempted to " +
                    $"load an invalid Achievement Pack ({FileName}): {fileName} " +
                    "appears to be malformed.", ex);
                }

                achievementData.Parent = this;

                // TODO: currently always uses the default connection. Might change that later to allow for more customization.
                RetrieveStoredAchievements(null, achievementData.GetAchievements().Select(achievement => (Achievement)achievement));

                data.Add(achievementData);
            }

            // TODO: should throw if this returns false
            TryAddAchievementData(data);

            // TODO: should throw if this returns false
            CombineData();

            cancellationToken.ThrowIfCancellationRequested();
        }

        private void RetrieveStoredAchievements(SQLiteConnection connection, IEnumerable<Achievement> achievements)
        {
            foreach(Achievement achievement in achievements)
            {
                RetrieveStoredAchievement(connection, achievement);
            }
        }

        private void RetrieveStoredAchievement(SQLiteConnection connection, Achievement achievement)
        {
            if (!Storage.TryRetrieve(connection, achievement, out _))
            {
                return; // No exception here, because the exception will be available through Storage.ExceptionOccured.
            }

            foreach (Objective objective in achievement.Objectives)
            {
                Storage.TryRetrieve(connection, objective, out _);
            }
        }

        /// <exception cref="OperationCanceledException"></exception>
        private async Task<(bool, PackFormatException)> TryLoadAchievements(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_dataReader == null)
            {
                return (false, null);
            }

            try
            {
                await LoadAchievements(cancellationToken);
            }
            catch (PackFormatException ex)
            {
                return (false, ex);
            }
            catch (OperationCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            return (true, null);
        }

        public void ReleaseLocks()
        {
            _dataReader?.AttemptReleaseLocks();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.DisposeChildren();
            
            _manifest = null;

            _customConverters = null;

            _data = null;

            _packStateLock = null;
            _report = null;

            PackLoaded = null;
            PackUnloaded = null;
            PackLoadStateChanged = null;

            ResourceManager = null;

            ReleaseLocks();
            _dataReader?.Dispose();
            _cancellationSourceEnable?.Dispose();
        }

        /// <inheritdoc/>
        public bool TryAddChild(IHierarchyObject child)
        {
            if (!(child is AchievementData data))
            {
                return false;
            }

            return TryAddAchievementData(data);
        }
    }
}
