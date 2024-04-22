using System.Threading.Tasks;
using System.IO;
using AchievementLib.Pack.Content;

namespace AchievementLib.Pack
{
    // Full copy of TmfLib.PackResourceManager https://github.com/dlamkins/TmfLib
    public class AchievementPackResourceManager : IAchievementPackResourceManager
    {

        public IDataReader DataReader { get; }

        internal AchievementPackResourceManager(IDataReader dataReader)
        {
            this.DataReader = dataReader;
        }

        public bool ResourceExists(string resourcePath)
        {
            return this.DataReader.FileExists(resourcePath);
        }

        public async Task<byte[]> LoadResourceAsync(string resourcePath)
        {
            return await this.DataReader.GetFileBytesAsync(resourcePath);
        }

        public async Task<Stream> LoadResourceStreamAsync(string resourcePath)
        {
            return await this.DataReader.GetFileStreamAsync(resourcePath);
        }

    }
}
