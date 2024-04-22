using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    [Serializable]
    /// <summary>
    /// The exception that is thrown when the loaded AchievementPack has incomplete 
    /// information.
    /// </summary>
    public class PackFormatException : PackException
    {
        public Type PackElement { get; }

        public override string Message
        {
            get
            {
                if (PackElement != null)
                {
                    return base.Message + $", PackElement: {PackElement}";
                }
                return base.Message;
            }
        }

        public PackFormatException()
        {

        }

        public PackFormatException(string message)
            : base(message)
        {

        }

        public PackFormatException(string message, Type packElement)
            : base(message)
        {
            PackElement = packElement;
            base.Data.Add("PackElement", PackElement);
        }

        public PackFormatException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public PackFormatException(string message, Type packElement, Exception inner)
            : base(message, inner)
        {
            PackElement = packElement;
            base.Data.Add("PackElement", PackElement);
        }

        protected PackFormatException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
