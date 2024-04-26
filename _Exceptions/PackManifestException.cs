using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="PackException"/> that is thrown when the loaded manifest of an 
    /// AchievementPack has incomplete or incorrect information.
    /// </summary>
    [Serializable]
    public class PackManifestException : PackException
    {
        /// <summary>
        /// The namespace of the manifest, that triggered the <see cref="PackManifestException"/>.
        /// </summary>
        public string PackNamespace { get; }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PackNamespace))
                {
                    return base.Message + $", PackNamespace: {PackNamespace}";
                }
                return base.Message;
            }
        }

        /// <inheritdoc/>
        public PackManifestException()
        {

        }

        /// <inheritdoc/>
        public PackManifestException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackResourceException"/> with the 
        /// given <paramref name="message"/> and <paramref name="packNamespace"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="packNamespace"></param>
        public PackManifestException(string message, string packNamespace)
            : base (message)
        {
            PackNamespace = packNamespace;
            base.Data.Add("PackNamespace", PackNamespace);
        }

        /// <inheritdoc/>
        public PackManifestException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackResourceException"/> with the 
        /// given <paramref name="message"/>, <paramref name="packNamespace"/> and a reference to 
        /// the inner <see cref="Exception"/>, that triggered this <see cref="PackManifestException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="packNamespace"></param>
        /// <param name="inner"></param>
        public PackManifestException(string message, string packNamespace, Exception inner)
            : base (message, inner)
        {
            PackNamespace = packNamespace;
            base.Data.Add("PackNamespace", PackNamespace);
        }

        /// <inheritdoc/>
        protected PackManifestException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {
            
        }
    }
}
