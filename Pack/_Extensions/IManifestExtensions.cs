using System;

namespace AchievementLib.Pack
{
    public static class IManifestExtensions
    {
        public static SupportedPackVersions? GetSupportedPackVersion(this IManifest manifest)
        {
            if (!manifest.IsSupportedPackVersion())
            {
                return null;
            }

            return (SupportedPackVersions)manifest.PackVersion;
        }

        public static bool IsSupportedPackVersion(this IManifest manifest)
        {
            if (!Enum.IsDefined(typeof(SupportedPackVersions), manifest.PackVersion))
            {
                return false;
            }
            return true;
        }
    }
}
