using System.IO;
using System.Threading.Tasks;


namespace AchievementLib.Pack
{
    // Full copy of TmfLib.IPackResourceManager https://github.com/dlamkins/TmfLib
    /// <summary>
    /// Loads resources for an Achievement Pack
    /// </summary>
    public interface IAchievementPackResourceManager
    {
        /// <summary>
        /// Determines whether a resource at the given <paramref name="resourcePath"/> exists.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns>True, if the resource exists. Otherwise false.</returns>
        bool ResourceExists(string resourcePath);

        /// <summary>
        /// Loads the resource at the <paramref name="resourcePath"/> asynchronously.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        Task<byte[]> LoadResourceAsync(string resourcePath);

        /// <summary>
        /// Loads the resource at the <paramref name="resourcePath"/> asynchronously.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns>The <see cref="Stream"/> associated with the resource.</returns>
        Task<Stream> LoadResourceStreamAsync(string resourcePath);
    }
}
