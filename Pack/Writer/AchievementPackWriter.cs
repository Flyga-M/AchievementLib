using Newtonsoft.Json;
using System;
using System.IO;

namespace AchievementLib.Pack.Writer
{
    public class AchievementPackWriter
    {
        public static void SerializeToJson<TAchievementData>(TAchievementData data, Stream target, JsonSerializerSettings settings)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            string jsonContents = SerializeToJson(data, settings) ?? throw new InvalidOperationException();

            using (StreamWriter writer = new StreamWriter(target))
            {
                writer.Write(jsonContents);
            }
        }

        public static string SerializeToJson<TAchievementData>(TAchievementData manifest, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(manifest, settings);
        }

        public static string SerializeV1ToJson(V1.Models.AchievementData data, V1.JSON.ActionConverter actionConverter = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = { actionConverter ?? V1.JSON.ActionConverter.Default }
            };

            return SerializeToJson(data, settings);
        }

        public static void SerializeV1ToJson(V1.Models.AchievementData data, Stream target, V1.JSON.ActionConverter actionConverter = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = { actionConverter ?? V1.JSON.ActionConverter.Default }
            };

            SerializeToJson(data, target, settings);
        }
    }
}
