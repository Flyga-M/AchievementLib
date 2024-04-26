using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    public interface IHierarchyObject
    {
        /// <summary>
        /// A unique (inside the current hierarchy layer) ID for the 
        /// <see cref="IHierarchyObject"/>.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The parent of the <see cref="IHierarchyObject"/>.
        /// </summary>
        [JsonIgnore]
        IHierarchyObject Parent { get; set; }

        /// <summary>
        /// The children of the <see cref="IHierarchyObject"/>.
        /// </summary>
        [JsonIgnore]
        IHierarchyObject[] Children { get; }
    }
}
