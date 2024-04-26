using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class, that contains a resource that can be loaded.
    /// </summary>
    public interface ILoadableResource : ILoadable, IValidateable
    {
        /// <summary>
        /// The relative file path of the <see cref="ILoadableResource"/>.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The loaded resource. Might be null, if <see cref="ILoadable.IsLoaded"/> == false.
        /// </summary>
        [JsonIgnore]
        object LoadedResource { get; }
    }
}
