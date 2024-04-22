using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    public interface IManifest : IValidateable
    {
        /// <summary>
        /// The PackVersion, that the Achievement Pack adheres to.
        /// </summary>
        int PackVersion { get; set; }

        [JsonIgnore]
        string PackFilePath { get; set; }
    }
}
