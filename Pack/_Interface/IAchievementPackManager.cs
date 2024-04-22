using System;

namespace AchievementLib.Pack
{
    public interface IAchievementPackManager : IDisposable
    {
        /// <summary>
        /// The <see cref="IManifest"/>, that contains information on the 
        /// <see cref="IManifest.PackVersion"/>.
        /// </summary>
        IManifest Manifest { get; }
    }
}
