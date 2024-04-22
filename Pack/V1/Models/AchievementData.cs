using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AchievementLib.Pack.V1.Models
{
    public class AchievementData : IHierarchyObject, IValidateable
    {
        public string Id { get; set; }

        /// <summary>
        /// The <see cref="AchievementCategory">IAchievementCategories</see> in 
        /// the <see cref="AchievementData"/>.
        /// </summary>
        public IEnumerable<AchievementCategory> AchievementCategories { get; set; }

        [JsonIgnore]
        public IHierarchyObject Parent { get; set; } = null;

        [JsonIgnore]
        public IHierarchyObject[] Children => AchievementCategories.ToArray();

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && AchievementCategories != null
                && AchievementCategories.Any()
                && AchievementCategories.All(category => category.IsValid());
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

        public override string ToString()
        {
            return $"{{ {typeof(AchievementData)}: {{ " +
                $"\"Id\": {Id}, " +
                $"\"AchievementCategories\": {{ {(AchievementCategories == null ? "" : string.Join(", ", AchievementCategories))} }}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
