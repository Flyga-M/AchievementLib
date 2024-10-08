﻿using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack
{
    /// <summary>
    /// A <see cref="IManifest"/> that includes only the fields of the <see cref="IManifest"/>. 
    /// Is used for deserialization.
    /// </summary>
    public class MinimalManifest : IManifest
    {
        /// <inheritdoc/>
        public Version Version { get; set; }

        /// <inheritdoc/>
        public int PackVersion { get; set; }

        /// <inheritdoc/>
        public string Author { get; set; }

        /// <inheritdoc/>
        public string Namespace { get; set; }

        /// <inheritdoc/>
        public V1.Models.Localizable Name { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        ILocalizable IManifest.Name => Name;

        /// <inheritdoc/>
        public V1.Models.Localizable Description { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        ILocalizable IManifest.Description => Description;

        /// <inheritdoc/>
        [JsonIgnore]
        public string PackFilePath { get; set; }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return PackVersion > 0
                && Version != null
                && !string.IsNullOrWhiteSpace(Author)
                && !string.IsNullOrWhiteSpace(Namespace)
                && Name != null
                && Name.IsValid()
                && Description != null
                && Description.IsValid();
        }

        /// <inheritdoc/>
        /// <exception cref="PackManifestException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Name?.Validate();
                    Description?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackManifestException("Manifest is not valid. Name is invalid.", Namespace, ex);
                }

                throw new PackManifestException("PackVersion, Version, Author, Namespace or Name missing.");
            }
        }
    }
}
