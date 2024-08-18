using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents the Manifest data of an Achievement Pack.
    /// </summary>
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
        /// The author of the Achievement Pack.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// The unique namespace of the Achievement Pack.
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// The (preferably unique) name of the Achievement Pack.
        /// </summary>
        ILocalizable Name { get; }

        /// <summary>
        /// The description of the Achievement Pack.
        /// </summary>
        ILocalizable Description { get; }

        /// <summary>
        /// The file path of the achievement pack.
        /// </summary>
        [JsonIgnore]
        string PackFilePath { get; set; }
    }
}
