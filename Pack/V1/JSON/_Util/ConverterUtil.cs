using Newtonsoft.Json;
using PositionEvents.Area.JSON;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack.V1.JSON
{
    /// <summary>
    /// Provides utility functions for the <see cref="JsonConverter"/>.
    /// </summary>
    public static class ConverterUtil
    {
        /// <summary>
        /// Adds the default <see cref="JsonConverter">JsonConverters</see> to the list of <paramref name="customConverters"/>, 
        /// unless it already contains the relevant converters.
        /// </summary>
        /// <param name="customConverters"></param>
        /// <returns>A new <see cref="IList{JsonConverter}"/> that contains the <paramref name="customConverters"/> and 
        /// the relevant default converters.</returns>
        public static IList<JsonConverter> AddDefaultConvertersIfNecessary(IEnumerable<JsonConverter> customConverters)
        {
            List<JsonConverter> converters = new List<JsonConverter>();

            if (customConverters != null)
            {
                converters.AddRange(customConverters);
            }

            if (!converters.Any(converter => converter is ActionConverter))
            {
                converters.Add(ActionConverter.Default);
            }

            if (!converters.Any(converter => converter is RestraintConverter))
            {
                converters.Add(RestraintConverter.Default);
            }

            if (!converters.Any(converter => converter is ColorConverter))
            {
                converters.Add(ColorConverter.Default);
            }

            if (!converters.Any(converter => converter is TextureConverter))
            {
                converters.Add(TextureConverter.Default);
            }

            if (!converters.Any(converter => converter is BoundingObjectConverter))
            {
                converters.Add(BoundingObjectConverter.Default);
            }

            return converters;
        }
    }
}
