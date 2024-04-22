using Newtonsoft.Json;
using System;
using System.IO;

namespace AchievementLib.Pack.Writer
{
    public static class ManifestWriter
    {
        public static void SerializeToJson<TManifest>(TManifest manifest, Stream target)
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

        public static string SerializeToJson<TManifest>(TManifest manifest)
        {
            return JsonConvert.SerializeObject(manifest);
        }
    }
}
