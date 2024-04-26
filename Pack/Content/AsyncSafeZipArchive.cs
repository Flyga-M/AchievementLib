using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementLib.Pack.Content
{
    // Full copy of TmfLib.Content.AsyncSafeZipArchive https://github.com/dlamkins/TmfLib
    /// <summary>
    /// Manages access to a .zip archive.
    /// </summary>
    public sealed class AsyncSafeZipArchive
    {

        private readonly ConcurrentBag<ZipArchive> _availableArchives = new ConcurrentBag<ZipArchive>();
        private readonly Dictionary<string, int> _entryLookup = new Dictionary<string, int>();

        private int _generation = 0;

        private readonly string _archivePath;

        /// <summary>
        /// The filepaths of the entries in the .zip archive.
        /// </summary>
        public IEnumerable<string> Entries => _entryLookup.Keys;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public AsyncSafeZipArchive(string filePath)
        {
            _archivePath = filePath;

            InitMetadata();
        }

        private void InitMetadata()
        {
            var (archive, generation) = GetArchive();

            for (int i = 0; i < archive.Entries.Count; i++)
            {
                string filePath = GetUniformFilePath(archive.Entries[i].FullName);

                // We have to check because zips are case sensitive and technically
                // can have duplicate entries that vary only by capitalization.
                if (!_entryLookup.ContainsKey(filePath))
                {
                    _entryLookup.Add(filePath, i);
                }
            }

            ReturnArchive(archive, generation);
        }

        private string GetUniformFilePath(string filePath)
        {
            return filePath.Replace(@"\", "/").Replace("//", "/").ToLowerInvariant().Trim().TrimStart('.');
        }

        /// <summary>
        /// Determines whether a file exists inside the .zip archive.
        /// </summary>
        /// <param name="filePath">The relative filepath inside the .zip archive.</param>
        /// <returns>True, if the <paramref name="filePath"/> exists. Otherwise false.</returns>
        public bool FileExists(string filePath)
        {
            return _entryLookup.ContainsKey(GetUniformFilePath(filePath));
        }

        private ZipArchiveEntry GetArchiveEntry(ZipArchive archive, string filePath)
        {
            return _entryLookup.TryGetValue(GetUniformFilePath(filePath), out int index)
                       ? archive.Entries[index]
                       : null;
        }

        private (ZipArchive ZipArchive, int Generation) GetArchive()
        {
            return _availableArchives.TryTake(out var archive)
                       ? (archive, _generation)
                       : (ZipFile.OpenRead(_archivePath), _generation);
        }

        private void ReturnArchive(ZipArchive archive, int generation)
        {
            if (generation == _generation)
            {
                _availableArchives.Add(archive);
            }
            else
            {
                archive.Dispose();
            }
        }

        /// <summary>
        /// Asynchronously retreives a <see cref="Stream"/> of the file.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="AsyncSafeZipArchive"/>.</param>
        /// <returns>
        /// A task that represents the file's opened <see cref="Stream"/>.
        /// If the file does not exist or cannot be read, the <see cref="Task"/> will result in null instead of a <see cref="Stream"/>.
        /// </returns>
        public async Task<Stream> GetFileStreamAsync(string filePath)
        {
            var (archive, generation) = GetArchive();

            try
            {
                ZipArchiveEntry fileEntry;

                if ((fileEntry = this.GetArchiveEntry(archive, filePath)) != null)
                {
                    var memStream = new MemoryStream();

                    using (var entryStream = fileEntry.Open())
                    {
                        await entryStream.CopyToAsync(memStream);
                    }

                    memStream.Position = 0;

                    return memStream;
                }
            }
            finally
            {
                ReturnArchive(archive, generation);
            }

            return null;
        }

        /// <summary>
        /// Retreives a file <see cref="Stream"/> of the file.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="AsyncSafeZipArchive"/>.</param>
        /// <returns>
        /// A <see cref="Stream"/> that represents the file's content.
        /// If the file does not exist or cannot be read, null will be returned instead of a <see cref="Stream"/>.
        /// </returns>
        public Stream GetFileStream(string filePath)
        {
            var (archive, generation) = GetArchive();

            try
            {
                ZipArchiveEntry fileEntry;

                if ((fileEntry = this.GetArchiveEntry(archive, filePath)) != null)
                {
                    var memStream = new MemoryStream();

                    using (var entryStream = fileEntry.Open())
                    {
                        entryStream.CopyTo(memStream);
                    }

                    memStream.Position = 0;

                    return memStream;
                }
            }
            finally
            {
                ReturnArchive(archive, generation);
            }

            return null;
        }

        /// <summary>
        /// Asynchronously opens a file and returns the raw data in a byte array.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="AsyncSafeZipArchive"/>.</param>
        /// <returns>
        /// A task that represents a byte array of the file's data.
        /// If the file does not exist or cannot be read, the <see cref="Task"/> will result in null instead of a byte[].
        /// </returns>
        public async Task<byte[]> GetFileBytesAsync(string filePath)
        {
            // We know GetFileStream returns a MemoryStream, so we don't check
            using (var fileStream = await GetFileStreamAsync(filePath) as MemoryStream)
            {
                if (fileStream != null)
                {
                    return fileStream.ToArray();
                }
            }

            return null;
        }

        /// <summary>
        /// Opens a file and returns the raw data in a byte array.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="AsyncSafeZipArchive"/>.</param>
        /// <returns>
        /// A byte array of the file's data.
        /// If the file does not exist or cannot be read, null will be returned instead of byte[].
        /// </returns>
        public byte[] GetFileBytes(string filePath)
        {
            // We know GetFileStream returns a MemoryStream, so we don't check
            using (var fileStream = GetFileStream(filePath) as MemoryStream)
            {
                if (fileStream != null)
                {
                    return fileStream.ToArray();
                }
            }

            return null;
        }

        public void AttemptReleaseLocks()
        {
            Interlocked.Increment(ref _generation);

            while (_availableArchives.TryTake(out var archive))
            {
                archive.Dispose();
            }
        }

    }
}
