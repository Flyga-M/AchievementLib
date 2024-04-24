using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    public class Localizable : ILocalizable, IValidateable
    {
        /// <summary>
        /// The values accessible via their locale.
        /// </summary>
        public Dictionary<string, string> ByLocale { get; set; }

        /// <inheritdoc/>
        public bool HasLocale(string locale)
        {
            return ByLocale.ContainsKey(locale);
        }

        /// <inheritdoc/>
        public string GetLocalized(string locale)
        {
            if (!ByLocale.Any())
            {
                return string.Empty;
            }

            if (ByLocale.ContainsKey(locale))
            {
                return ByLocale[locale];
            }

            return ByLocale.First().Value;
        }

        /// <inheritdoc/>
        public string GetLocalized(string locale, string fallbackLocale)
        {
            if (HasLocale(locale))
            {
                return GetLocalized(locale);
            }

            return GetLocalized(fallbackLocale);
        }

        public bool IsValid()
        {
            return ByLocale != null && ByLocale.Any();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"Localizable {this} is invalid.", this.GetType());
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(Localizable)}: {{ " +
                $"\"ByLocale\": {{ {(ByLocale == null ? "" : string.Join(", ", ByLocale))} }}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
