using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    public interface IManifest : IValidateable
    {
        /// <summary>
        /// The version of the Achievement Pack. Used to check for 
        /// newer versions.
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// The PackVersion, that the Achievement Pack adheres to.
        /// </summary>
        int PackVersion { get; set; }

        /// <summary>
        /// The unique namespace of the Achievement Pack.
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// The (preferably unique) name of the Achievement Pack.
        /// </summary>
        ILocalizable Name { get; }

        [JsonIgnore]
        string PackFilePath { get; set; }
    }
}
