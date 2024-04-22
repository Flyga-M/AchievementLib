using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AchievementLib.Pack.V1.Models
{
    public class Localizable : IValidateable
    {
        /// <summary>
        /// The values accessible via their locale.
        /// </summary>
        public Dictionary<string, string> ByLocale { get; set; }

        /// <summary>
        /// Determines whether the <see cref="Localizable"/> has an entry for the 
        /// <paramref name="locale"/>.
        /// </summary>
        /// <param name="locale"></param>
        /// <returns>True, if <see cref="ByLocale"/> contains the key 
        /// <paramref name="locale"/>. Otherwise false.</returns>
        public bool HasLocale(string locale)
        {
            return ByLocale.ContainsKey(locale);
        }

        /// <summary>
        /// Returns the localized version of the <see cref="Localizable"/>.
        /// </summary>
        /// <param name="locale"></param>
        /// <returns>The localized version of the <see cref="Localizable"/>, or 
        /// the first element of <see cref="ByLocale"/> if the <paramref name="locale"/> is not found, or 
        /// <see cref="string.Empty"/>, if <see cref="ByLocale"/> is empty.</returns>
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

        /// <summary>
        /// Returns the localized version of the <see cref="Localizable"/> or the 
        /// localized version for the <paramref name="fallbackLocale"/>, if the 
        /// <see cref="Localizable"/> has no entry for the <paramref name="locale"/>.
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="fallbackLocale"></param>
        /// <returns>The localized version of the <see cref="Localizable"/> or the 
        /// localized version for the <paramref name="fallbackLocale"/>, if the 
        /// <see cref="Localizable"/> has no entry for the <paramref name="locale"/>, 
        /// or the first element of <see cref="ByLocale"/> if the 
        /// <paramref name="fallbackLocale"/> is not found, or <see cref="string.Empty"/>, 
        /// if <see cref="ByLocale"/> is empty.</returns>
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
