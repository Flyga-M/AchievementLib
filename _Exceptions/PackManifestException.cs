using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    [Serializable]
    /// <summary>
    /// The exception that is thrown when the loaded manifest of an 
    /// AchievementPack has incomplete or incorrect information.
    /// </summary>
    public class PackManifestException : PackException
    {
        public string PackNamespace { get; }

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

        public PackManifestException()
        {

        }

        public PackManifestException(string message)
            : base(message)
        {

        }

        public PackManifestException(string message, string packNamespace)
            : base (message)
        {
            PackNamespace = packNamespace;
            base.Data.Add("PackNamespace", PackNamespace);
        }

        public PackManifestException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public PackManifestException(string message, string packNamespace, Exception inner)
            : base (message, inner)
        {
            PackNamespace = packNamespace;
            base.Data.Add("PackNamespace", PackNamespace);
        }

        protected PackManifestException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {
            
        }
    }
}
