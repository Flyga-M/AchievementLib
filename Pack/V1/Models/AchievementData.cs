using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    public class AchievementData : IAchievementData
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// The <see cref="AchievementCategory">IAchievementCategories</see> in 
        /// the <see cref="AchievementData"/>.
        /// </summary>
        public IEnumerable<AchievementCategory> AchievementCategories { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject[] Children => AchievementCategories.ToArray();

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && AchievementCategories != null
                && AchievementCategories.Any()
                && AchievementCategories.All(category => category.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    if (AchievementCategories != null)
                    {
                        foreach (AchievementCategory category in AchievementCategories)
                        {
                            category.Validate();
                        }
                    }
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"AchievementData {this.GetFullName()} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"AchievementData {this.GetFullName()} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(AchievementData)}: {{ " +
                $"\"Id\": {Id}, " +
                $"\"AchievementCategories\": {{ {(AchievementCategories == null ? "" : string.Join(", ", AchievementCategories))} }}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
