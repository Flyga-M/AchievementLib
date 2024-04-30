using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a reference to a different object in the hierarchy tree.
    /// </summary>
    public interface IResolvableHierarchyReference : IResolvable
    {
        /// <summary>
        /// The full id of the element inside the hierarchy tree.
        /// </summary>
        string ReferenceId { get; }

        /// <summary>
        /// The resolved reference. Might be null if <see cref="IResolvable.IsResolved"/> == false.
        /// </summary>
        [JsonIgnore]
        IHierarchyObject Reference { get; set; }
    }
}
