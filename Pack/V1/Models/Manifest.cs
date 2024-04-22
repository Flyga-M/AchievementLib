using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    public class Manifest : IManifest
    {
        /// <summary>
        /// The version of the Achievement Pack. Used to check for 
        /// newer versions.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// The PackVersion, that the Achievement Pack adheres to. 
        /// Has to be 1 for this iteration of the Achievement Pack.
        /// </summary>
        public int PackVersion { get; set; }

        /// <summary>
        /// The author of the Achievement Pack.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The name of the Achievement Pack.
        /// </summary>
        public Localizable Name { get; set; }

        /// <summary>
        /// The description of the Achievement Pack.
        /// </summary>
        public Localizable Description { get; set; }

        /// <summary>
        /// The unique namespace of the Achievement Pack.
        /// </summary>
        public string Namespace { get; set; }

        [JsonIgnore]
        public string PackFilePath { get; set; }

        public bool IsValid()
        {
            return Version != null
                && PackVersion == 1
                && !string.IsNullOrWhiteSpace(Author)
                && Name != null
                && Name.IsValid()
                && Description != null
                && Description.IsValid()
                && !string.IsNullOrWhiteSpace(Namespace);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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
                    throw new PackManifestException("Manifest is not valid.", Namespace, ex);
                }
                throw new PackManifestException("Manifest is not valid", Namespace);
            }
        }

        public override string ToString()
        {
            return $"{{ {typeof(Manifest)}: {{ " +
                $"\"Version\": {Version}, " +
                $"\"PackVersion\": {PackVersion}, " +
                $"\"Author\": {Author}, " +
                $"\"Name\": {Name}, " +
                $"\"Description\": {Description}, " +
                $"\"Namespace\": {Namespace}, " +
                $"\"PackFilePath\": {PackFilePath}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
