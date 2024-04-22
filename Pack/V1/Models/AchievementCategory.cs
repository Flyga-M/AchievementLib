using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    public class AchievementCategory : IHierarchyObject, IValidateable
    {
        public string Id { get; set; }

        /// <summary>
        /// The name of the <see cref="AchievementCategory"/>.
        /// </summary>
        public Localizable Name { get; set; }

        /// <summary>
        /// The <see cref="AchievementCollection">AchievementCollections</see> in 
        /// the <see cref="AchievementCategory"/>.
        /// </summary>
        public IEnumerable<AchievementCollection> AchievementCollections { get; set; }

        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        [JsonIgnore]
        public IHierarchyObject[] Children => AchievementCollections.ToArray();

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && Name != null
                && Name.IsValid()
                && AchievementCollections != null
                && AchievementCollections.Any()
                && AchievementCollections.All(collection => collection.IsValid());
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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
