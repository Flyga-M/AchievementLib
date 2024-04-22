using AchievementLib.Pack.Content;
using AchievementLib.Pack.Reader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AchievementLib.Pack
{
    public class AchievementPackInitiator : IDisposable
    {   
        // TODO: maybe add option to delete pack here?
        
        private readonly string _watchPath;

        private readonly SafeList<JsonConverter> _customConverters = new SafeList<JsonConverter>();

        private readonly SafeList<IManifest> _manifests = new SafeList<IManifest>();
        private readonly SafeList<IAchievementPackManager> _packs = new SafeList<IAchievementPackManager>();

        public IAchievementPackManager[] Packs => _packs.ToArray();

        /// <summary>
        /// <paramref name="customConverters"/> currently only respects custom 
        /// <see cref="V1.JSON.ActionConverter">ActionConverters</see>.
        /// </summary>
        /// <param name="watchPath"></param>
        /// <param name="customConverters"></param>
        public AchievementPackInitiator(string watchPath, IEnumerable<JsonConverter> customConverters = null)
        {
            _watchPath = watchPath;

            if (customConverters == null)
            {
                customConverters = Array.Empty<JsonConverter>();
            }

            _customConverters = new SafeList<JsonConverter>(customConverters);
        }

        private TConverter TryGetCustomConverter<TConverter>() where TConverter : JsonConverter
        {
            return (TConverter)_customConverters.Where(converter => converter.GetType() == typeof(TConverter)).FirstOrDefault();
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
                            newPack = V1.AchievementPackManager.FromArchivedMarkerPack(v1Manifest.PackFilePath, v1Manifest, TryGetCustomConverter<V1.JSON.ActionConverter>());
                        }
                        catch (FileNotFoundException ex)
                        {
                            throw new PackException("Attempted to " +
                                $"load an invalid Achievement Pack ({manifest.PackFilePath}): Path is " +
                                $"not available anymore.", ex);
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

        public void Dispose()
        {
            UnregisterPacks();
        }
    }
}
