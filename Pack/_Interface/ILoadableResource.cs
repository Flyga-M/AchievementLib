using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    public interface ILoadableResource : ILoadable, IValidateable
    {
        /// <summary>
        /// The relative file path of the <see cref="ILoadableResource"/>.
        /// </summary>
        string Path { get; set; }

        [JsonIgnore]
        /// <summary>
        /// The loaded resource. Might be null, if <see cref="IsLoaded"/> == false.
        /// </summary>
        object LoadedResource { get; }
    }
}
