using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="PackException"/> that is thrown, when an error with a 
    /// resource arises.
    /// </summary>
    [Serializable]
    public class PackResourceException : PackException
    {
        /// <summary>
        /// The name of the affected resource. Is not always set.
        /// </summary>
        public string ResourceName { get; }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ResourceName))
                {
                    return base.Message + $" ResourceName: {ResourceName}";
                }
                return base.Message;
            }
        }

        /// <inheritdoc/>
        public PackResourceException()
        {

        }

        /// <inheritdoc/>
        public PackResourceException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackResourceException"/> with the 
        /// given <paramref name="message"/> and <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="resourceName"></param>
        public PackResourceException(string message, string resourceName)
            : base(message)
        {
            ResourceName = resourceName;
            base.Data.Add("ResourceName", ResourceName);
        }

        /// <inheritdoc/>
        public PackResourceException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackResourceException"/> with the 
        /// given <paramref name="message"/>, <paramref name="resourceName"/> and a reference to 
        /// the inner <see cref="Exception"/>, that triggered this <see cref="PackResourceException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="resourceName"></param>
        /// <param name="inner"></param>
        public PackResourceException(string message, string resourceName, Exception inner)
            : base(message, inner)
        {
            ResourceName = resourceName;
            base.Data.Add("ResourceName", ResourceName);
        }

        /// <inheritdoc/>
        protected PackResourceException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
