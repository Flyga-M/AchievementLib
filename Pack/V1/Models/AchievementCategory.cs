using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IAchievementCategory"/>
    /// This is the V1 implementation.
    /// </summary>
    public class AchievementCategory : IAchievementCategory
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc cref="IAchievementCategory.Name"/>
        public Localizable Name { get; set; }

        /// <inheritdoc/>
        ILocalizable IAchievementCategory.Name => Name;

        /// <summary>
        /// <inheritdoc cref="IAchievementCategory.AchievementCollections"/>
        /// </summary>
        public IEnumerable<AchievementCollection> AchievementCollections { get; set; }

        /// <inheritdoc/>
        IEnumerable<IAchievementCollection> IAchievementCategory.AchievementCollections => AchievementCollections;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject[] Children => AchievementCollections.ToArray();

        /// <summary>
        /// Instantiates an <see cref="AchievementCategory"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="achievementCollections"></param>
        [JsonConstructor]
        public AchievementCategory(string id, Localizable name, IEnumerable<AchievementCollection> achievementCollections)
        {
            Id = id;
            Name = name;
            AchievementCollections = achievementCollections;

            if (AchievementCollections != null)
            {
                foreach (AchievementCollection collection in AchievementCollections)
                {
                    collection.Parent = this;
                }
            }
        }



        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && Name != null
                && Name.IsValid()
                && AchievementCollections != null
                && AchievementCollections.Any()
                && AchievementCollections.All(collection => collection.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Name?.Validate();
                    if (AchievementCollections != null)
                    {
                        foreach (AchievementCollection collection in AchievementCollections)
                        {
                            collection.Validate();
                        }
                    }
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"AchievementCategory {this.GetFullName()} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"AchievementCategory {this.GetFullName()} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(AchievementCategory)}: {{ " +
                $"\"Id\": {Id}, " +
                $"\"Name\": {Name}, " +
                $"\"AchievementCollections\": {{ {(AchievementCollections == null ? "" : string.Join(", ", AchievementCollections))} }}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
