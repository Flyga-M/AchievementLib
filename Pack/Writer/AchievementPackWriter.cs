﻿using Newtonsoft.Json;
using System;
using System.IO;
using PositionEvents.Area.JSON;

namespace AchievementLib.Pack.Writer
{

    /// <summary>
    /// Provides methods to serialize <see cref="IAchievementData"/> to json.
    /// </summary>
    public static class AchievementPackWriter
    {
        /// <summary>
        /// Serializes the given <paramref name="data"/> to the <paramref name="target"/> 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="TAchievementData"></typeparam>
        /// <param name="data"></param>
        /// <param name="target"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SerializeToJson<TAchievementData>(TAchievementData data, Stream target, JsonSerializerSettings settings) where TAchievementData : IAchievementData
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

        /// <summary>
        /// Serializes the given <paramref name="data"/> to json.
        /// </summary>
        /// <typeparam name="TAchievementData"></typeparam>
        /// <param name="data"></param>
        /// <param name="settings"></param>
        /// <returns>The json representation of the given <paramref name="data"/>.</returns>
        public static string SerializeToJson<TAchievementData>(TAchievementData data, JsonSerializerSettings settings) where TAchievementData : IAchievementData
        {
            return JsonConvert.SerializeObject(data, settings);
        }

        /// <summary>
        /// Serializes the given <paramref name="data"/> to json.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="actionConverter"></param>
        /// <param name="areaConverter"></param>
        /// <returns>The json representation of the given <paramref name="data"/>.</returns>
        public static string SerializeV1ToJson(V1.Models.AchievementData data, V1.JSON.ActionConverter actionConverter = null, BoundingObjectConverter areaConverter = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters =
                {
                    actionConverter ?? V1.JSON.ActionConverter.Default,
                    areaConverter ?? BoundingObjectConverter.Default
                }
            };

            return SerializeToJson(data, settings);
        }

        /// <summary>
        /// Serializes the given <paramref name="data"/> to the <paramref name="target"/> 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="target"></param>
        /// <param name="actionConverter"></param>
        /// <param name="areaConverter"></param>
        public static void SerializeV1ToJson(V1.Models.AchievementData data, Stream target, V1.JSON.ActionConverter actionConverter = null, BoundingObjectConverter areaConverter = null)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters =
                {
                    actionConverter ?? V1.JSON.ActionConverter.Default,
                    areaConverter ?? BoundingObjectConverter.Default
                }
            };

            SerializeToJson(data, target, settings);
        }
    }
}
