using AchievementLib.Pack.Content;
using AchievementLib.Pack.Reader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Handles the registering of all the Achievement Packs in the watch path.
    /// </summary>
    public class AchievementPackInitiator : IDisposable
    {   
        private readonly string _watchPath;

        private readonly IEnumerable<JsonConverter> _customConverters;

        private readonly SafeList<IManifest> _manifests = new SafeList<IManifest>();
        private readonly SafeList<IAchievementPackManager> _packs = new SafeList<IAchievementPackManager>();

        /// <summary>
        /// Contains all packs from the watch path, that did not fail to load after 
        /// <see cref="LoadWatchPath"/> was called.
        /// </summary>
        public IAchievementPackManager[] Packs => _packs.ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="watchPath"></param>
        /// <param name="customConverters"></param>
        public AchievementPackInitiator(string watchPath, IEnumerable<JsonConverter> customConverters = null)
        {
            _watchPath = watchPath;

            _customConverters = customConverters;
        }

        /// <summary>
        /// Attempts to returns the registered pack with the given <paramref name="namespace"/>.
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="exception"></param>
        /// <param name="manager"></param>
        /// <returns>True, if a pack with the <paramref name="namespace"/> exists and can be retrieved. 
        /// Otherwise false.</returns>
        public bool TryGetPack(string @namespace, out PackException exception, out IAchievementPackManager manager)
        {
            exception = null;

            manager = _packs.Where(pack => pack.Manifest.Namespace == @namespace).FirstOrDefault();

            if (manager == null)
            {
                exception = new PackException($"no pack with the given namespace \"{@namespace}\" was registered.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a pack with the <paramref name="namespace"/> is already registered.
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public bool IsRegistered(string @namespace)
        {
            return _manifests.Any(manifest => manifest.Namespace == @namespace);
        }

        /// <summary>
        /// Determines whether a pack with the same namespace as the <paramref name="manager"/> is already registered.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool IsRegistered(IAchievementPackManager manager)
        {
            return IsRegistered(manager.Manifest.Namespace);
        }

        /// <summary>
        /// Registers Manifests and Achievement Packs from the 
        /// watch path.
        /// </summary>
        /// <returns><see cref="PackException">PackExceptions</see> that occur during 
        /// initalization.</returns>
        public PackException[] LoadWatchPath()
        {
            UnregisterPacks();

            List<PackException> exceptions = new List<PackException>();

            exceptions.AddRange(LoadManifestsFromWatchPath());

            exceptions.AddRange(RegisterPacksFromLoadedManifests());

            return exceptions.ToArray();
        }

        /// <summary>
        /// Attempts to manually add a pack to the <see cref="AchievementPackInitiator"/>.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <paramref name="manager"/> was successfully registered. Otherwise false.</returns>
        public bool TryRegisterPack(IAchievementPackManager manager, out PackException exception)
        {
            exception = null;

            if (manager == null)
            {
                exception = new PackException("manager can't be null.", new ArgumentNullException(nameof(manager)));
                return false;
            }

            if (IsRegistered(manager))
            {
                TryGetPack(manager.Manifest.Namespace, out PackException _, out IAchievementPackManager existingPack);
                exception = new PackException($"can't register pack ({manager.Manifest.GetDetailedName()}), " +
                    $"because a pack with the same namespace ({existingPack.Manifest.GetDetailedName()}) is " +
                    "already registered.");
                return false;
            }

            if (_packs.Contains(manager))
            {
                exception = new PackException("can't register pack, that is already registered.");
                return false;
            }

            if (_manifests.Contains(manager.Manifest))
            {
                exception = new PackException("can't register pack, whose manifest is already registered.");
                return false;
            }

            _packs.Add(manager);
            _manifests.Add(manager.Manifest);
            return true;
        }

        /// <summary>
        /// Attempts to manually add a pack to the <see cref="AchievementPackInitiator"/>.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="exception"></param>
        /// <param name="manager"></param>
        /// <returns>True, if the pack was successfully registered from the <paramref name="filePath"/>. 
        /// Otherwise false.</returns>
        public bool TryRegisterPack(string filePath, out PackException exception, out IAchievementPackManager manager)
        {
            exception = null;
            manager = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                exception = new PackException("filepath can't be empty.");
                return false;
            }

            if (!File.Exists(filePath))
            {
                exception = new PackException($"Can't register pack from a file that does not exist: {filePath}", 
                    new FileNotFoundException("File not found.", filePath));
                return false;
            }

            IManifest manifest;

            using (IDataReader reader = new ZipArchiveReader(filePath))
            {
                if (!TryLoadManifest(reader, out PackManifestException manifestException, out manifest))
                {
                    exception = new PackException("An exception occured while trying to load the manifest.",
                        manifestException);
                    return false;
                }
            }

            if (!TryRegisterPackFromLoadedManifest(manifest, out PackException packException, out manager))
            {
                exception = new PackException("An exception occured while trying to register the pack from the " +
                    "loaded manifest.", packException);
                return false;
            }

            return TryRegisterPack(manager, out exception);
        }

        /// <summary>
        /// Attempts to delete the achievement pack with the <paramref name="namespace"/> from disk. Disables the 
        /// pack beforehand. Will NOT dispose the corresponding <see cref="IAchievementPackManager"/>, that needs 
        /// to be done manually.
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="exception"></param>
        /// <param name="manager"></param>
        /// <returns>True, if the pack was successfully deleted. Otherwise false.</returns>
        public bool TryDeletePack(string @namespace, out PackException exception, out IAchievementPackManager manager)
        {
            if (!TryGetPack(@namespace, out exception, out manager))
            {
                return false;
            }

            return TryDeletePack(manager, out exception);
        }

        /// <summary>
        /// Attempts to delete the achievement pack from disk. Disables the pack beforehand. 
        /// Will NOT dispose of the <paramref name="manager"/>, that needs to be done manually.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the achievement pack was successfully deleted. Otherwise false.</returns>
        public bool TryDeletePack(IAchievementPackManager manager, out PackException exception)
        {
            if (manager == null)
            {
                exception = new PackException("manager can't be null.", new ArgumentNullException(nameof(manager)));
                return false;
            }
            
            string filePath = manager.Manifest.PackFilePath;

            if (!DeletePack(filePath, out exception))
            {
                return false;
            }

            _packs.Remove(manager);
            _manifests.Remove(manager.Manifest);

            manager.Disable(true);

            return true;
        }

        // is not prefixed with "Try" because of the public TryDeletePack with the same parameter types.
        private bool DeletePack(string filePath, out PackException exception)
        {
            exception = null;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                exception = new PackException("filepath can't be empty.");
                return false;
            }

            if (!File.Exists(filePath))
            {
                exception = new PackException($"Can't delete pack for a file that does not exist: {filePath}",
                    new FileNotFoundException("File not found.", filePath));
                return false;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                exception = new PackException("An exception occured while trying to delete pack.", ex);
                return false;
            }

            return true;
        }

        private PackManifestException[] LoadManifestsFromWatchPath()
        {
            IEnumerable<string> zipPackFiles = Directory.GetFiles(_watchPath, "*.zip", SearchOption.AllDirectories);

            List<PackManifestException> exceptions = new List<PackManifestException>();

            foreach (string packArchive in zipPackFiles)
            {
                IDataReader reader = new ZipArchiveReader(packArchive);

                if (!TryLoadManifest(reader, out PackManifestException ex, out IManifest manifest))
                {
                    exceptions.Add(ex);
                    reader.Dispose();
                    continue;
                }

                reader.Dispose();

                _manifests.Add(manifest);
            }

            return exceptions.ToArray();
        }

        private bool TryLoadManifest(IDataReader dataReader, out PackManifestException exception, out IManifest manifest)
        {
            exception = null;
            manifest = null;

            try
            {
                manifest = LoadManifest(dataReader);
            }
            catch (AchievementLibInternalException ex)
            {
                exception = new PackManifestException("an internal exception occured.", ex);
                return false;
            }
            catch (PackManifestException ex)
            {
                exception = ex;
                return false;
            }

            return true;
        }

        /// <exception cref="AchievementLibInternalException"></exception>
        /// <exception cref="PackManifestException"></exception>
        private IManifest LoadManifest(IDataReader dataReader)
        {
            string fileName = Path.GetFileNameWithoutExtension(dataReader.GetPathRepresentation());

            if (dataReader == null)
            {
                throw new AchievementLibInternalException("dataReader is null.", new ArgumentNullException(nameof(dataReader)));
            }

            if (!dataReader.FileExists(Constants.MANIFEST_NAME))
            {
                throw new PackManifestException("Attempted to load an invalid " +
                    $"Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} is missing.");
            }

            IManifest moduleManifest;

            try
            {
                Stream fileStream = dataReader.GetFileStream(Constants.MANIFEST_NAME);
                moduleManifest = ManifestReader.DeserializeFromJson<MinimalManifest>(fileStream);
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new PackManifestException("Attempted to load an invalid " +
                    $"Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} appears to be " +
                    $"malformed.", ex);
            }

            if (moduleManifest == null)
            {
                throw new PackManifestException("Attempted to " +
                $"load an invalid Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} " +
                "appears to be malformed.");
            }

            if (!moduleManifest.IsSupportedPackVersion())
            {
                throw new PackManifestException("Attempted to " +
                $"load an invalid Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} " +
                $"packVersion {moduleManifest.PackVersion} is not supported.");
            }

            SupportedPackVersions? packVersion = moduleManifest.GetSupportedPackVersion();

            if (!packVersion.HasValue)
            {
                throw new AchievementLibInternalException("Attempted to " +
                $"load an Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} " +
                $"packVersion {moduleManifest.PackVersion} should be supported, but " +
                $"can not be cast as SupportedPackVersion.");
            }

            switch(packVersion.Value)
            {
                case (SupportedPackVersions.V1):
                    {
                        try
                        {
                            Stream fileStream = dataReader.GetFileStream(Constants.MANIFEST_NAME);
                            moduleManifest = ManifestReader.DeserializeFromJson<V1.Models.Manifest>(fileStream);
                            fileStream.Dispose();

                            moduleManifest.Validate();
                        }
                        catch (Exception ex)
                        {
                            throw new PackManifestException("Attempted to load an invalid " +
                                $"Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} appears to be " +
                                $"malformed.", ex);
                        }

                        break;
                    }
                default:
                    {
                        throw new AchievementLibInternalException("supported pack version " +
                            "not implemented.", new NotImplementedException());
                    }
            }

            if (moduleManifest == null)
            {
                throw new PackManifestException("Attempted to " +
                $"load an invalid Achievement Pack ({fileName}): {Constants.MANIFEST_NAME} " +
                "appears to be malformed.");
            }

            moduleManifest.PackFilePath = dataReader.GetPathRepresentation();

            return moduleManifest;
        }

        private PackException[] RegisterPacksFromLoadedManifests()
        {
            List<PackException> exceptions = new List<PackException>();

            foreach (IManifest manifest in _manifests)
            {
                if (!TryRegisterPackFromLoadedManifest(manifest, out PackException ex, out IAchievementPackManager pack))
                {
                    exceptions.Add(ex);
                    continue;
                }

                _packs.Add(pack);
            }

            return exceptions.ToArray();
        }

        private bool TryRegisterPackFromLoadedManifest(IManifest manifest, out PackException exception, out IAchievementPackManager pack)
        {
            exception = null;
            pack = null;

            try
            {
                pack = RegisterPackFromLoadedManifest(manifest);
            }
            catch (AchievementLibInternalException ex)
            {
                exception = new PackException("An internal exception occured.", ex);
                return false;
            }
            catch (PackException ex)
            {
                exception = ex;
                return false;
            }

            return true;
        }

        /// <exception cref="AchievementLibInternalException"></exception>
        /// <exception cref="PackException"></exception>
        private IAchievementPackManager RegisterPackFromLoadedManifest(IManifest manifest)
        {
            if (manifest == null)
            {
                throw new AchievementLibInternalException("manifest is null.", new ArgumentNullException(nameof(manifest)));
            }

            SupportedPackVersions? packVersion = manifest.GetSupportedPackVersion();

            if (!packVersion.HasValue)
            {
                throw new AchievementLibInternalException("Attempted to " +
                $"load an Achievement Pack ({manifest.PackFilePath}): {Constants.MANIFEST_NAME} " +
                $"packVersion {manifest.PackVersion} is not supported.");
            }

            IAchievementPackManager newPack;

            switch (packVersion.Value)
            {
                case (SupportedPackVersions.V1):
                    {
                        if (!(manifest is V1.Models.Manifest v1Manifest))
                        {
                            throw new AchievementLibInternalException("Attempted to " +
                                $"load an Achievement Pack ({manifest.PackFilePath}): {Constants.MANIFEST_NAME} " +
                                $"packVersion {manifest.PackVersion} should be supported, " +
                                $"but an error occured while unboxing as {typeof(V1.Models.Manifest)}.");
                        }
                        
                        try
                        {
                            newPack = V1.AchievementPackManager.FromArchivedPack(v1Manifest.PackFilePath, v1Manifest, _customConverters);
                        }
                        catch (FileNotFoundException ex)
                        {
                            throw new PackException("Attempted to " +
                                $"load an invalid Achievement Pack ({manifest.PackFilePath}): Path is " +
                                $"not available anymore.", ex);
                        }
                        catch (Exception ex)
                        {
                            throw new PackException("Attempted to " +
                                $"load an invalid Achievement Pack ({manifest.PackFilePath}): Uncaught exception " +
                                $"occured.", ex);
                        }

                        break;
                    }
                default:
                    {
                        throw new AchievementLibInternalException("supported PackVersion " +
                            $"{packVersion.Value} not implemented.", new NotImplementedException());
                    }
            }

            if (newPack == null)
            {
                throw new PackException("Unable to load pack from loaded " +
                    $"manifest. {manifest.PackFilePath}");
            }

            return newPack;
        }

        private void UnregisterPacks()
        {
            foreach (IAchievementPackManager pack in _packs)
            {
                pack.Dispose();
            }

            _manifests.Clear();
            _packs.Clear();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            UnregisterPacks();
        }
    }
}
