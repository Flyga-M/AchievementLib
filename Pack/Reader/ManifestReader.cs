using Newtonsoft.Json;
using System;
using System.IO;

namespace AchievementLib.Pack.Reader
{
    /// <summary>
    /// Provides methods to deserialize a <see cref="IManifest"/> to json.
    /// </summary>
    public static class ManifestReader
    {
        /// <summary>
        /// Deserializes the given <paramref name="jsonStream"/> to a 
        /// <typeparamref name="TManifest"/> object.
        /// </summary>
        /// <typeparam name="TManifest"></typeparam>
        /// <param name="jsonStream"></param>
        /// <returns>The deserialized <typeparamref name="TManifest"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TManifest DeserializeFromJson<TManifest>(Stream jsonStream) where TManifest : IManifest
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

            return DeserializeFromJson<TManifest>(jsonContents);
        }

        /// <summary>
        /// Deserializes the given <paramref name="jsonContents"/> to a 
        /// <typeparamref name="TManifest"/> object.
        /// </summary>
        /// <typeparam name="TManifest"></typeparam>
        /// <param name="jsonContents"></param>
        /// <returns>The deserialized <typeparamref name="TManifest"/>.</returns>
        public static TManifest DeserializeFromJson<TManifest>(string jsonContents) where TManifest : IManifest
        {
            return JsonConvert.DeserializeObject<TManifest>(jsonContents);
        }
    }
}
