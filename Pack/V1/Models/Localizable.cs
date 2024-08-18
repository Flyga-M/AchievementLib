using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.Models
{
    /// <inheritdoc cref="ILocalizable"/>
    public class Localizable : ILocalizable
    {
        private Dictionary<string, string> _byLocale;

        /// <summary>
        /// The values accessible via their locale. 
        /// [Optional]
        /// </summary>
        public Dictionary<string, string> ByLocale
        {
            get
            {
                if (_byLocale == null)
                {
                    _byLocale = new Dictionary<string, string>();
                }

                return _byLocale;
            }
            set => _byLocale = value;
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool AnyLocale => ByLocale.Any();

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

        /// <inheritdoc/>
        public string GetLocalized(string locale, IEnumerable<ILocalizable> references)
        {
            return GetLocalized(locale, references, locale);
        }

        /// <inheritdoc/>
        public string GetLocalized(string locale, IEnumerable<ILocalizable> references, string fallbackLocale)
        {
            if (HasLocale(locale))
            {
                return GetLocalized(locale);
            }

            foreach (ILocalizable reference in references)
            {
                if (!reference.HasLocale(locale))
                {
                    continue;
                }

                foreach (KeyValuePair<string, string> valueBylocale in ByLocale)
                {
                    if (!reference.HasLocale(valueBylocale.Key))
                    {
                        continue;
                    }

                    if (reference.GetLocalized(valueBylocale.Key) == GetLocalized(valueBylocale.Key))
                    {
                        return reference.GetLocalized(locale);
                    }
                }
            }

            return GetLocalized(fallbackLocale);
        }
        
        /// <inheritdoc/>
        public Dictionary<string, string> GetAll()
        {
            return new Dictionary<string, string>(ByLocale);
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return true;
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"Localizable {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(Localizable)}: {{ " +
                $"\"ByLocale\": {{ {(ByLocale == null ? "" : string.Join(", ", ByLocale))} }}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
