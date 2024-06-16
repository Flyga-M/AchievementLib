using System;
using System.Data.SQLite;
using System.IO;

namespace AchievementLib.Pack.PersistantData.SQLite
{
    /// <summary>
    /// Handles <see cref="SQLiteConnection"/>s.
    /// </summary>
    public static class ConnectionHandler
    {
        /// <summary>
        /// The directory that is used by the <see cref="DefaultConnection"/>.
        /// </summary>
        /// <remarks>
        /// To edit, use <see cref="Storage"/> class instead.
        /// </remarks>
        internal static string DefaultDirectory = string.Empty;

        /// <summary>
        /// The filename that is used by the <see cref="DefaultConnection"/>.
        /// </summary>
        /// <remarks>
        /// Should NOT include the extension. Set <see cref="DefaultFileExtension"/> instead. 
        /// To edit, use <see cref="Storage"/> class instead.
        /// </remarks>
        internal static string DefaultFileName = "default";

        /// <summary>
        /// The file extension that is used by the <see cref="DefaultConnection"/>.
        /// </summary>
        /// <remarks>
        /// To edit, use <see cref="Storage"/> class instead.
        /// </remarks>
        internal static string DefaultFileExtension = "sqlite";

        /// <summary>
        /// The version that is used by the <see cref="DefaultConnection"/>.
        /// </summary>
        public static int DefaultVersion = 3;

        /// <summary>
        /// Returns a <see cref="SQLiteConnection"/> with the <see cref="DefaultDirectory"/> and 
        /// <see cref="DefaultVersion"/>.
        /// </summary>
        /// <remarks>
        /// Make sure <see cref="DefaultDirectory"/> is valid, before using <see cref="DefaultConnection"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">If <see cref="DefaultDirectory"/> is null, empty or whitespace.</exception>
        public static SQLiteConnection DefaultConnection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DefaultDirectory))
                {
                    throw new InvalidOperationException($"Unable to return {nameof(DefaultConnection)}. " +
                        $"{nameof(DefaultDirectory)} can't be null, empty or whitespace.");
                }

                string path = DefaultDirectory;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = DefaultFileName;
                }
                else
                {
                    path = Path.Combine(path, DefaultFileName);
                }

                path += $".{DefaultFileExtension}";

                return GetConnection(path, DefaultVersion);
            }
        }

        /// <summary>
        /// Returns a <see cref="SQLiteConnection"/> with the <paramref name="path"/> as the data source and the 
        /// given <paramref name="version"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="SQLiteConnection"/> needs to be open and closed manually and properly disposed. 
        /// Will create an empty file with the given <paramref name="path"/>.
        /// </remarks>
        /// <param name="path"></param>
        /// <param name="version"></param>
        /// <returns>A <see cref="SQLiteConnection"/> with the <paramref name="path"/> as the data source and the 
        /// given <paramref name="version"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="path"/> is empty or whitespace.</exception>
        public static SQLiteConnection GetConnection(string path, int version)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"{nameof(path)} can't be empty or whitespace.", nameof(path));
            }

            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }

            return new SQLiteConnection(GetConnectionString(path, version));
        }

        /// <summary>
        /// Returns a <see cref="SQLiteConnection"/> with the <paramref name="path"/> as the data source and the 
        /// <see cref="DefaultVersion"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="SQLiteConnection"/> needs to be open and closed manually and properly disposed. 
        /// Will create an empty file with the given <paramref name="path"/>.
        /// </remarks>
        /// <param name="path"></param>
        /// <returns>A <see cref="SQLiteConnection"/> with the <paramref name="path"/> as the data source and the 
        /// <see cref="DefaultVersion"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="path"/> is empty or whitespace.</exception>
        public static SQLiteConnection GetConnection(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"{nameof(path)} can't be empty or whitespace.", nameof(path));
            }

            return GetConnection(path, DefaultVersion);
        }

        /// <summary>
        /// Returns a <see cref="SQLiteConnection"/> with the <see cref="DefaultDirectory"/>, <paramref name="subPath"/>, 
        /// <paramref name="fileName"/> and <paramref name="fileExtension"/> as the data source and the 
        /// <see cref="DefaultVersion"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="DefaultFileName"/> and <see cref="DefaultFileExtension"/> will be used, 
        /// if <paramref name="fileName"/> and <paramref name="fileExtension"/> are <see langword="null"/>, empty or 
        /// whitespace.
        /// The <see cref="SQLiteConnection"/> needs to be open and closed manually and properly disposed. 
        /// Will create an empty file at the created path.
        /// </remarks>
        /// <param name="subPath"></param>
        /// <param name="fileName"></param>
        /// <param name="fileExtension"></param>
        /// <returns>A <see cref="SQLiteConnection"/> with the <see cref="DefaultDirectory"/>, <paramref name="subPath"/>, 
        /// <paramref name="fileName"/> and <paramref name="fileExtension"/> as the data source and the 
        /// <see cref="DefaultVersion"/>.</returns>
        public static SQLiteConnection GetConnection(string subPath, string fileName, string fileExtension)
        {
            string path = DefaultDirectory ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(subPath))
            {
                path = Path.Combine(path, subPath);
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                path = Path.Combine(path, fileName);
            }
            else
            {
                path = Path.Combine(path, DefaultFileName);
            }

            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                path += $".{fileExtension}";
            }
            else
            {
                path += $".{DefaultFileExtension}";
            }

            return GetConnection(path, DefaultVersion);
        }

        /// <summary>
        /// Returns a <see cref="SQLiteConnection"/> with the <see cref="DefaultDirectory"/>, 
        /// <paramref name="fileName"/> and <paramref name="fileExtension"/> as the data source and the 
        /// <see cref="DefaultVersion"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="DefaultFileName"/> and <see cref="DefaultFileExtension"/> will be used, 
        /// if <paramref name="fileName"/> and <paramref name="fileExtension"/> are <see langword="null"/>, empty or 
        /// whitespace.
        /// The <see cref="SQLiteConnection"/> needs to be open and closed manually and properly disposed. 
        /// Will create an empty file at the created path.
        /// </remarks>
        /// <param name="fileName"></param>
        /// <param name="fileExtension"></param>
        /// <returns>A <see cref="SQLiteConnection"/> with the <see cref="DefaultDirectory"/>, 
        /// <paramref name="fileName"/> and <paramref name="fileExtension"/> as the data source and the 
        /// <see cref="DefaultVersion"/>.</returns>
        public static SQLiteConnection GetConnection(string fileName, string fileExtension)
        {
            return GetConnection(null, fileName, fileExtension);
        }

        private static string GetConnectionString(string path, int version)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"{nameof(path)} can't be empty or whitespace.", nameof(path));
            }

            return $"Data Source=\"{path}\";Version={version};";
        }
    }
}
