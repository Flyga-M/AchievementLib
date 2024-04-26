using Newtonsoft.Json;
using System;
using System.IO;
using PositionEvents.Area.JSON;

namespace AchievementLib.Pack.Reader
{
    /// <summary>
    /// Provides methods to deserialize a <see cref="IAchievementData"/> to json.
    /// </summary>
    public static class AchievementPackReader
    {
        /// <summary>
        /// Deserializes the given <paramref name="jsonStream"/> to a 
        /// <typeparamref name="TAchievementData"/> object.
        /// </summary>
        /// <typeparam name="TAchievementData"></typeparam>
        /// <param name="jsonStream"></param>
        /// <param name="settings"></param>
        /// <returns>The deserialized <typeparamref name="TAchievementData"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static TAchievementData DeserializeFromJson<TAchievementData>(Stream jsonStream, JsonSerializerSettings settings) where TAchievementData : IAchievementData
        {
            if (jsonStream == null)
            {
                throw new ArgumentNullException(nameof(jsonStream));
            }

            string jsonContents = "";

            using (StreamReader reader = new StreamReader(jsonStream))
            {
                jsonContents = reader.ReadToEnd();
            }

            return DeserializeFromJson<TAchievementData>(jsonContents, settings);
        }

        /// <summary>
        /// Deserializes the given <paramref name="jsonContents"/> to a 
        /// <typeparamref name="TAchievementData"/> object.
        /// </summary>
        /// <typeparam name="TAchievementData"></typeparam>
        /// <param name="jsonContents"></param>
        /// <param name="settings"></param>
        /// <returns>The deserialized <typeparamref name="TAchievementData"/>.</returns>
        private static TAchievementData DeserializeFromJson<TAchievementData>(string jsonContents, JsonSerializerSettings settings) where TAchievementData : IAchievementData
        {
            return JsonConvert.DeserializeObject<TAchievementData>(jsonContents, settings);
        }

        /// <summary>
        /// Deserializes the given <paramref name="jsonStream"/> to a 
        /// <see cref="V1.Models.AchievementData"/> object.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="actionConverter"></param>
        /// <param name="areaConverter"></param>
        /// <returns>The deserialized <see cref="V1.Models.AchievementData"/>.</returns>
        public static V1.Models.AchievementData DeserializeV1FromJson(Stream jsonStream, V1.JSON.ActionConverter actionConverter = null, BoundingObjectConverter areaConverter = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters =
                {
                    actionConverter ?? V1.JSON.ActionConverter.Default,
                    areaConverter ?? BoundingObjectConverter.Default
                }
            };

            return DeserializeFromJson<V1.Models.AchievementData>(jsonStream, settings);
        }

        /// <summary>
        /// Deserializes the given <paramref name="jsonContents"/> to a 
        /// <see cref="V1.Models.AchievementData"/> object.
        /// </summary>
        /// <param name="jsonContents"></param>
        /// <param name="actionConverter"></param>
        /// <param name="areaConverter"></param>
        /// <returns>The deserialized <see cref="V1.Models.AchievementData"/>.</returns>
        public static V1.Models.AchievementData DeserializeV1FromJson(string jsonContents, V1.JSON.ActionConverter actionConverter = null, BoundingObjectConverter areaConverter = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters =
                {
                    actionConverter ?? V1.JSON.ActionConverter.Default,
                    areaConverter ?? BoundingObjectConverter.Default
                }
            };

            return DeserializeFromJson<V1.Models.AchievementData>(jsonContents, settings);
        }
    }
}
