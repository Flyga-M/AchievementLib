using Newtonsoft.Json;
using System;
using System.IO;

namespace AchievementLib.Pack.Writer
{
    /// <summary>
    /// Provides methods to serialize a <see cref="IManifest"/> to json.
    /// </summary>
    public static class ManifestWriter
    {
        /// <summary>
        /// Serializes the given <paramref name="manifest"/> to the <paramref name="target"/> 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="TManifest"></typeparam>
        /// <param name="manifest"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SerializeToJson<TManifest>(TManifest manifest, Stream target) where TManifest : IManifest
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            string jsonContents = SerializeToJson(manifest) ?? throw new InvalidOperationException();

            using (StreamWriter writer = new StreamWriter(target))
            {
                writer.Write(jsonContents);
            }
        }

        /// <summary>
        /// Serializes the given <paramref name="manifest"/> to json.
        /// </summary>
        /// <typeparam name="TManifest"></typeparam>
        /// <param name="manifest"></param>
        /// <returns>The json representation of the given <paramref name="manifest"/>.</returns>
        public static string SerializeToJson<TManifest>(TManifest manifest) where TManifest : IManifest
        {
            return JsonConvert.SerializeObject(manifest);
        }
    }
}
