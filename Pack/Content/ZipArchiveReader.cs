using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AchievementLib.Pack.Content
{
    // Slightly modified copy of TmfLib.Content.ZipArchiveReader https://github.com/dlamkins/TmfLib
    /// <summary>
    /// Manages access to the content of a .zip archive.
    /// </summary>
    public sealed class ZipArchiveReader : IDataReader
    {
        private readonly AsyncSafeZipArchive _archive;

        private readonly string _archivePath;
        private readonly string _subPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="subPath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public ZipArchiveReader(string archivePath, string subPath = "")
        {
            if (!File.Exists((archivePath)))
                throw new FileNotFoundException("Archive path not found.", archivePath);

            _archivePath = archivePath;
            _subPath = subPath;

            _archive = new AsyncSafeZipArchive(archivePath);
        }

        /// <inheritdoc/>
        public IDataReader GetSubPath(string subPath)
        {
            return new ZipArchiveReader(_archivePath, Path.Combine(subPath));
        }

        /// <inheritdoc/>
        public string GetPathRepresentation(string relativeFilePath = null)
        {
            if (relativeFilePath == null)
            {
                return _archivePath;
            }
            return $"{_archivePath}[{Path.GetFileName(Path.Combine(_subPath, relativeFilePath ?? string.Empty))}]";
        }

        /// <inheritdoc/>
        public async Task LoadOnFileTypeAsync(Func<Stream, IDataReader, string, Task> loadFileFunc, string fileExtension = "", IProgress<string> progress = null)
        {
            var validEntries = _archive.Entries.Where(e => e.EndsWith(fileExtension.ToLowerInvariant())).ToList();

            foreach (var entry in validEntries)
            {
                progress?.Report($"Loading {entry}...");
                await loadFileFunc.Invoke(await this.GetFileStreamAsync(entry), this, Path.GetFileNameWithoutExtension(entry));
            }
        }

        /// <inheritdoc/>
        public bool FileExists(string filePath)
        {
            return _archive.FileExists(filePath);
        }

        /// <inheritdoc/>
        public Stream GetFileStream(string filePath) => _archive.GetFileStream(filePath);

        /// <inheritdoc/>
        public Task<Stream> GetFileStreamAsync(string filePath) => _archive.GetFileStreamAsync(filePath);

        /// <inheritdoc/>
        public byte[] GetFileBytes(string filePath) => _archive.GetFileBytes(filePath);

        /// <inheritdoc/>
        public Task<byte[]> GetFileBytesAsync(string filePath) => _archive.GetFileBytesAsync(filePath);

        /// <inheritdoc/>
        public void AttemptReleaseLocks() => _archive.AttemptReleaseLocks();

        /// <inheritdoc/>
        public void Dispose()
        {
            AttemptReleaseLocks();
        }

    }
}
