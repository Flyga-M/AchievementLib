using System.Threading.Tasks;
using System.IO;
using AchievementLib.Pack.Content;

namespace AchievementLib.Pack
{
    // Full copy of TmfLib.PackResourceManager https://github.com/dlamkins/TmfLib
    /// <inheritdoc/>
    public class AchievementPackResourceManager : IAchievementPackResourceManager
    {
        /// <summary>
        /// The <see cref="IDataReader"/> of the <see cref="AchievementPackResourceManager"/>.
        /// </summary>
        public IDataReader DataReader { get; }

        internal AchievementPackResourceManager(IDataReader dataReader)
        {
            this.DataReader = dataReader;
        }

        /// <inheritdoc/>
        public bool ResourceExists(string resourcePath)
        {
            return this.DataReader.FileExists(resourcePath);
        }

        /// <inheritdoc/>
        public async Task<byte[]> LoadResourceAsync(string resourcePath)
        {
            return await this.DataReader.GetFileBytesAsync(resourcePath);
        }

        /// <inheritdoc/>
        public async Task<Stream> LoadResourceStreamAsync(string resourcePath)
        {
            return await this.DataReader.GetFileStreamAsync(resourcePath);
        }

    }
}
