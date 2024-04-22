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

        [JsonIgnore]
        /// <summary>
        /// The parent of the <see cref="IHierarchyObject"/>.
        /// </summary>
        IHierarchyObject Parent { get; set; }

        [JsonIgnore]
        /// <summary>
        /// The children of the <see cref="IHierarchyObject"/>.
        /// </summary>
        IHierarchyObject[] Children { get; }
    }
}
