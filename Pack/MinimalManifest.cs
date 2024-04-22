using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    public class MinimalManifest : IManifest
    {
        public int PackVersion { get; set; }

        [JsonIgnore]
        public string PackFilePath { get; set; }

        public bool IsValid()
        {
            return PackVersion > 0;
        }

        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackManifestException("PackVersion missing.");
            }
        }
    }
}
