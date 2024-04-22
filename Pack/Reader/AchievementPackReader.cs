using Newtonsoft.Json;
using System;
using System.IO;
using PositionEvents.Area.JSON;

namespace AchievementLib.Pack.Reader
{
    public static class AchievementPackReader
    {
        private static TAchievementData DeserializeFromJson<TAchievementData>(Stream jsonStream, JsonSerializerSettings settings)
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

        private static TAchievementData DeserializeFromJson<TAchievementData>(string jsonContents, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<TAchievementData>(jsonContents, settings);
        }

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
