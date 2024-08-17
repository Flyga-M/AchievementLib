using Newtonsoft.Json;

namespace AchievementLib.Pack.PersistantData
{
    /// <summary>
    /// An object, that can be retrieved via <see cref="Storage"/>.
    /// </summary>
    public interface IRetrievable
    {
        /// <summary>
        /// Determines whether the properties are currently being retrieved by the 
        /// <see cref="Storage"/>.
        /// </summary>
        /// <remarks>
        /// Is used to prevent (re)storing properties, directly after they have been retrieved.
        /// </remarks>
        [JsonIgnore]
        bool IsRetrieving { get; set; }
    }
}
