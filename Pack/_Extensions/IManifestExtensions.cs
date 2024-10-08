﻿using System;
using System.IO;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Provides extension methods for the <see cref="IManifest"/> interface.
    /// </summary>
    public static class IManifestExtensions
    {
        /// <summary>
        /// Returns the equivalent <see cref="SupportedPackVersions"/> value for the 
        /// <see cref="IManifest.PackVersion"/>, or null if the version is not supported.
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns>The equivalent <see cref="SupportedPackVersions"/> value for the 
        /// <see cref="IManifest.PackVersion"/>, or null if the version is not supported.</returns>
        public static SupportedPackVersions? GetSupportedPackVersion(this IManifest manifest)
        {
            if (!manifest.IsSupportedPackVersion())
            {
                return null;
            }

            return (SupportedPackVersions)manifest.PackVersion;
        }

        /// <summary>
        /// Determines whether the <see cref="IManifest.PackVersion"/> is supported by 
        /// this library.
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns>True, if the <see cref="IManifest.PackVersion"/> is defined by the 
        /// <see cref="SupportedPackVersions"/> enum.</returns>
        public static bool IsSupportedPackVersion(this IManifest manifest)
        {
            if (!Enum.IsDefined(typeof(SupportedPackVersions), manifest.PackVersion))
            {
                return false;
            }
            return true;
        }

        // copy from https://github.com/blish-hud/Blish-HUD/blob/dev/Blish%20HUD/GameServices/Modules/Manifest.cs
        /// <summary>
        /// Gets the detailed name of the achievement pack suitable for displaying in logs. 
        /// [Name] ([Namespace] v[Version])
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns>The detailed name of the achievement pack suitable for displaying in logs.</returns>
        public static string GetDetailedName(this IManifest manifest)
        {
            return $"{manifest.Name.GetLocalized("en")} ({manifest.Namespace}) v{manifest.Version}";
        }

        /// <summary>
        /// Attempts to retrieve the last modified date of the pack file.
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="lastUpdate"></param>
        /// <returns><see langword="true"/>, if the attempt was successfull. 
        /// Otherwise <see langword="false"/>.</returns>
        public static bool TryGetLastFileUpdate(this IManifest manifest, out DateTime lastUpdate)
        {
            lastUpdate = DateTime.MinValue;
            FileInfo fileInfo;

            try
            {
                fileInfo = new FileInfo(manifest.PackFilePath);
                lastUpdate = fileInfo.LastWriteTime;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
