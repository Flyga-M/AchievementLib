using AchievementLib.Pack.Content;
using AchievementLib.Pack.V1.Models;
using AchievementLib.Pack.Reader;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AchievementLib.Pack.V1.JSON;
using Newtonsoft.Json;
using PositionEvents.Area.JSON;

namespace AchievementLib.Pack.V1
{
    public class AchievementPackManager : IHierarchyObject, IAchievementPackManager
    {
        private readonly IDataReader _dataReader;

        private Manifest _manifest;

        private ActionConverter _actionConverter;
        private BoundingObjectConverter _areaConverter;

        private AchievementData[] _data;

        private PackLoadState _state = PackLoadState.Unloaded;
        private object _packStateLock = new object();

        private PackLoadReport _report;

        /// <summary>
        /// Fires, once the pack was successfully enabled and loaded via 
        /// <see cref="Enable(bool)"/>.
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
        /// Fires, if an error occurs during <see cref="Enable(bool)"/>.
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
            PackError?.Invoke(this, ex);
        }

        /// <summary>
        /// The <see cref="PackLoadState"/> of the Achievement Pack.
        /// </summary>
        public PackLoadState State
        {
            get
            {
                lock(_packStateLock)
                {
                    return _state;
                }
            }
            private set
            {
                lock (_packStateLock)
                {
                    if (_state != value)
                    {
                        OnPackLoadStateChanged(value);
                    }
                    _state = value;
                }
            }
        }

        /// <summary>
        /// The <see cref="PackLoadReport"/> of the Achievement Pack. Contains 
        /// information on the (un)successfully loading of the Achievement Pack.
        /// </summary>
        public PackLoadReport Report => _report;

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

        /// <summary>
        /// The resource manager of the <see cref="AchievementPackManager"/>.
        /// </summary>
        public AchievementPackResourceManager ResourceManager { get; private set; }

        public string Id => Manifest?.Namespace;

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

        public IHierarchyObject[] Children => Data;

        public AchievementPackManager(IDataReader dataReader, Manifest manifest, ActionConverter actionConverter = null, BoundingObjectConverter areaConverter = null)
        {   
            _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));

            _actionConverter = actionConverter;
            _areaConverter = areaConverter;

            ResourceManager = new AchievementPackResourceManager(dataReader);

            _report = new PackLoadReport();

            _data = Array.Empty<AchievementData>();
        }

        public static AchievementPackManager FromArchivedMarkerPack(string archivePath, Manifest manifest, ActionConverter actionConverter = null, BoundingObjectConverter areaConverter = null) => new AchievementPackManager(new ZipArchiveReader(archivePath), manifest, actionConverter, areaConverter);

        /// <summary>
        /// Attempts to enable the Achievement Pack and load it's data into 
        /// <see cref="Data"/>. The data is loaded asynchronously and is not available 
        /// directly after <see cref="Enable(bool)"/> was called. Listen to 
        /// <see cref="PackLoaded"/> and <see cref="PackError"/> to make sure, the 
        /// data is available.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns>True, if the Achievement Pack is eligible to be enabled. 
        /// Otherwise false.</returns>
        public bool Enable(GraphicsDevice graphicsDevice, out Task task)
        {
            task = null;
            if (State == PackLoadState.FatalError
                || State == PackLoadState.Unloading
                || State == PackLoadState.Loading
                || State == PackLoadState.Loaded)
            {
                return false;
            }

            State = PackLoadState.Loading;

            task = Load(graphicsDevice);
            
            return true;
        }

        private async Task Load(GraphicsDevice graphicsDevice)
        {   
            (bool success, PackFormatException ex) = await TryLoadAchievements();

            if (!success)
            {
                _report.FaultyData = true;
                _report.Exception = ex;
                State = PackLoadState.FatalError;

                OnPackError(ex);
                return;
            }

            _report.FaultyData = false;

            (bool resourceEval, PackResourceException[] exceptions) = await TryLoadResourcesAsync(graphicsDevice);
            if (!resourceEval)
            {
                _report.FaultyResources = true;
                _report.Exception = new AchievementLibAggregateException("Some resources could not be loaded.", exceptions);

                // not a fatal error
            }
            else
            {
                _report.FaultyResources = false;
            }

            State = PackLoadState.Loaded;
        }

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
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        private async Task LoadResourcesAsync(GraphicsDevice graphicsDevice)
        {
            (bool eval, PackResourceException[] exceptions) = await this.TryLoadChildrensResourcesAsync(ResourceManager, graphicsDevice);

            if (!eval)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Returns false, if at least one resource was not loaded successfully. Will 
        /// strill try to attempt to load the other resources.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        private async Task<(bool, PackResourceException[])> TryLoadResourcesAsync(GraphicsDevice graphicsDevice)
        {
            try
            {
                await LoadResourcesAsync(graphicsDevice);
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

            return (true, Array.Empty<PackResourceException>());
        }

        public bool Disable()
        {
            if (State == PackLoadState.Unloading
                || State == PackLoadState.Loading
                || State == PackLoadState.Unloaded)
            {
                return false;
            }

            State = PackLoadState.Unloading;

            this.DisposeChildren();

            _data = null;
            _report.FaultyData = null;
            _report.FaultyResources = null;
            _report.Exception = null;

            State = PackLoadState.Unloaded;

            return true;
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PackFormatException"></exception>
        private async Task LoadAchievements()
        {   
            if (_dataReader == null)
            {
                throw new InvalidOperationException("_dataReader must be != null.");
            }
            
            _data = Array.Empty<AchievementData>();

            List<AchievementData> data = new List<AchievementData>();

            List<(Stream, IDataReader, string)> candidates = new List<(Stream fileStream, IDataReader dataReader, string fileName)>();

            await _dataReader.LoadOnFileTypeAsync((fileStream, dataReader, fileName) =>
            {
                candidates.Add((fileStream, dataReader, fileName));
                return Task.CompletedTask;
            }, ".json");

            foreach ((Stream fileStream, IDataReader dataReader, string fileName) in candidates)
            {
                if (fileName == Path.GetFileNameWithoutExtension(Constants.MANIFEST_NAME))
                {
                    // ignore manifest here
                    continue;
                }

                AchievementData achievementData;

                try
                {
                    achievementData = AchievementPackReader.
                    DeserializeV1FromJson(fileStream, _actionConverter, _areaConverter);
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
                

                if (achievementData == null)
                {
                    throw new PackFormatException("Attempted to " +
                    $"load an invalid Achievement Pack ({FileName}): {fileName} " +
                    "appears to be malformed.");
                }

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

                achievementData.AssertHierarchy(this);

                data.Add(achievementData);
            }

            _data = data.ToArray();
        }

        private async Task<(bool, PackFormatException)> TryLoadAchievements()
        {
            if (_dataReader == null)
            {
                return (false, null);
            }

            try
            {
                await LoadAchievements();
            }
            catch (PackFormatException ex)
            {
                return (false, ex);
            }

            return (true, null);
        }

        public void Dispose()
        {
            this.DisposeChildren();
            
            _manifest = null;

            _actionConverter = null;
            _areaConverter = null;

            _data = null;

            _packStateLock = null;
            _report = null;

            PackLoaded = null;
            PackUnloaded = null;
            PackLoadStateChanged = null;

            ResourceManager = null;
            
            _dataReader?.Dispose();
        }
    }
}
