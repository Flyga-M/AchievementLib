using System;
using System.Runtime.Serialization;

namespace AchievementLib
{

    /// <summary>
    /// The <see cref="PackException"/> that is thrown when the loaded AchievementPack 
    /// has incomplete information.
    /// </summary>
    [Serializable]
    public class PackFormatException : PackException
    {
        /// <summary>
        /// The <see cref="Type"/> of the achievement pack element that triggered the 
        /// <see cref="PackFormatException"/>.
        /// </summary>
        public Type PackElement { get; }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (PackElement != null)
                {
                    return base.Message + $" PackElement: {PackElement}";
                }
                return base.Message;
            }
        }

        /// <inheritdoc/>
        public PackFormatException()
        {

        }

        /// <inheritdoc/>
        public PackFormatException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackFormatException"/> with the 
        /// given <paramref name="message"/> and <paramref name="packElement"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="packElement"></param>
        public PackFormatException(string message, Type packElement)
            : base(message)
        {
            PackElement = packElement;
            base.Data.Add("PackElement", PackElement);
        }

        /// <inheritdoc/>
        public PackFormatException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackResourceException"/> with the 
        /// given <paramref name="message"/>, <paramref name="packElement"/> and a reference to 
        /// the inner <see cref="Exception"/>, that triggered this <see cref="PackFormatException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="packElement"></param>
        /// <param name="inner"></param>
        public PackFormatException(string message, Type packElement, Exception inner)
            : base(message, inner)
        {
            PackElement = packElement;
            base.Data.Add("PackElement", PackElement);
        }

        /// <inheritdoc/>
        protected PackFormatException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
