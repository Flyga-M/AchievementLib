using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IAchievementData"/> 
    /// This is the V1 implementation.
    /// </summary>
    public class AchievementData : IAchievementData
    {
        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc cref="IAchievementData.AchievementCategories"/>
        public List<AchievementCategory> AchievementCategories { get; }

        /// <inheritdoc/>
        IEnumerable<IAchievementCategory> IAchievementData.AchievementCategories => AchievementCategories;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject[] Children => AchievementCategories.ToArray();

        /// <summary>
        /// Instantiates an <see cref="AchievementData"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="achievementCategories"></param>
        [JsonConstructor]
        public AchievementData(string id, IEnumerable<AchievementCategory> achievementCategories)
        {
            Id = id;
            AchievementCategories = new List<AchievementCategory>();

            if (achievementCategories != null)
            {
                // TODO: should throw if this returns false
                TryAddAchievementCategories(achievementCategories);
            }
        }

        private bool TryAddAchievementCategory(AchievementCategory category)
        {
            if (category == null)
            {
                return false;
            }

            if (AchievementCategories.Contains(category))
            {
                return false;
            }

            category.Parent = this;

            AchievementCategories.Add(category);
            return true;
        }

        private bool TryAddAchievementCategories(IEnumerable<AchievementCategory> categories)
        {
            if (categories == null)
            {
                return false;
            }

            return categories.All(category => TryAddAchievementCategory(category));
        }

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

        /// <inheritdoc/>
        public bool TryAddChild(IHierarchyObject child)
        {
            if (!(child is AchievementCategory category))
            {
                return false;
            }

            return TryAddAchievementCategory(category);
        }
    }
}
