using System.IO;
using System.Threading.Tasks;


namespace AchievementLib.Pack
{
    // Full copy of TmfLib.IPackResourceManager https://github.com/dlamkins/TmfLib
    public interface IAchievementPackResourceManager
    {
        bool ResourceExists(string resourcePath);

        Task<byte[]> LoadResourceAsync(string resourcePath);

        Task<Stream> LoadResourceStreamAsync(string resourcePath);
    }
}
