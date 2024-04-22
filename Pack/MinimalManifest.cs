using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    public class MinimalManifest : IManifest
    {
        public Version Version { get; set; }

        public int PackVersion { get; set; }

        public string Namespace { get; set; }

        [JsonIgnore]
        public string PackFilePath { get; set; }

        public bool IsValid()
        {
            return PackVersion > 0
                && Version != null
                && !string.IsNullOrWhiteSpace(Namespace);
        }

        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackManifestException("PackVersion, Version or Namespace missing.");
            }
        }
    }
}
