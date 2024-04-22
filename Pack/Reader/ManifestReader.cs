using Newtonsoft.Json;
using System;
using System.IO;

namespace AchievementLib.Pack.Reader
{
    public static class ManifestReader
    {
        public static TManifest DeserializeFromJson<TManifest>(Stream jsonStream)
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

        public static TManifest DeserializeFromJson<TManifest>(string jsonContents)
        {
            return JsonConvert.DeserializeObject<TManifest>(jsonContents);
        }
    }
}
