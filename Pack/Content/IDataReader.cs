using System;
using System.IO;
using System.Threading.Tasks;

namespace AchievementLib.Pack.Content
{
    // Slightly modified copy of TmfLib.Content.IDataReader https://github.com/dlamkins/TmfLib
    /// <summary>
    /// Provides methods to read from various file systems, endpoints, and formats.
    /// </summary>
    public interface IDataReader : IDisposable
    {
        /// <summary>
        /// Gets a new <see cref="IDataReader"/> with the root path set to the provided <paramref name="subPath"/>.
        /// </summary>
        /// <param name="subPath">A sub path within the context of the <see cref="IDataReader"/>.</param>
        IDataReader GetSubPath(string subPath);

        /// <summary>
        /// Returns a string representation of the current path the DataReader is reading from. This won't necessarily be a valid file path.
        /// </summary>
        /// <param name="relativeFilePath">If provided, the path to a file within the <see cref="IDataReader"/> will be returned.</param>
        string GetPathRepresentation(string relativeFilePath = null);

        /// <summary>
        /// Enumerates all available files. Files that have the extension <paramref name="fileExtension"/>
        /// will be passed to the provided <paramref name="loadFileFunc"/>.
        /// </summary>
        /// <param name="loadFileFunc">The method to call on all files within the context of the <see cref="IDataReader"/> that have the required file extension.</param>
        /// <param name="fileExtension">The file extension criteria. Should contain the '.' before the extension. If no fileExtension is provided, all files will meet the criteria.</param>
        /// <param name="progress"></param>
        Task LoadOnFileTypeAsync(Func<Stream, IDataReader, string, Task> loadFileFunc, string fileExtension = "", IProgress<string> progress = null);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="IDataReader"/>.</param>
        bool FileExists(string filePath);

        /// <summary>
        /// Retreives a file <see cref="Stream"/> of the file.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="IDataReader"/>.</param>
        /// <returns>
        /// A <see cref="Stream"/> that represents the file's content.
        /// If the file does not exist or cannot be read, null will be returned instead of a <see cref="Stream"/>.
        /// </returns>
        Stream GetFileStream(string filePath);

        /// <summary>
        /// Opens a file and returns the raw data in a byte array.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="IDataReader"/>.</param>
        /// <returns>
        /// A byte array of the file's data.
        /// If the file does not exist or cannot be read, null will be returned instead of byte[].
        /// </returns>
        byte[] GetFileBytes(string filePath);

        /// <summary>
        /// Asynchronously retreives a <see cref="Stream"/> of the file.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="IDataReader"/>.</param>
        /// <returns>
        /// A task that represents the file's opened <see cref="Stream"/>.
        /// If the file does not exist or cannot be read, the <see cref="Task"/> will result in null instead of a <see cref="Stream"/>.
        /// </returns>
        Task<Stream> GetFileStreamAsync(string filePath);

        /// <summary>
        /// Asynchronously opens a file and returns the raw data in a byte array.
        /// </summary>
        /// <param name="filePath">A path to a file within the context of the <see cref="IDataReader"/>.</param>
        /// <returns>
        /// A task that represents a byte array of the file's data.
        /// If the file does not exist or cannot be read, the <see cref="Task"/> will result in null instead of a byte[].
        /// </returns>
        Task<byte[]> GetFileBytesAsync(string filePath);

        void AttemptReleaseLocks();

    }
}
