using System;
using System.Runtime.Serialization;

namespace AchievementLib
{
    /// <summary>
    /// The <see cref="PackException"/> that is thrown, when an error with a 
    /// reference arises.
    /// </summary>
    [Serializable]
    public class PackReferenceException : PackException
    {
        /// <summary>
        /// The id of the affected reference. Is not always set.
        /// </summary>
        public string ReferenceId { get; }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ReferenceId))
                {
                    return base.Message + $" ReferenceId: {ReferenceId}";
                }
                return base.Message;
            }
        }

        /// <inheritdoc/>
        public PackReferenceException()
        {

        }

        /// <inheritdoc/>
        public PackReferenceException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackReferenceException"/> with the 
        /// given <paramref name="message"/> and <paramref name="referenceId"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="referenceId"></param>
        public PackReferenceException(string message, string referenceId)
            : base(message)
        {
            ReferenceId = referenceId;
            base.Data.Add("ReferenceId", ReferenceId);
        }

        /// <inheritdoc/>
        public PackReferenceException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackReferenceException"/> with the 
        /// given <paramref name="message"/>, <paramref name="referenceId"/> and a reference to 
        /// the inner <see cref="Exception"/>, that triggered this <see cref="PackReferenceException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="referenceId"></param>
        /// <param name="inner"></param>
        public PackReferenceException(string message, string referenceId, Exception inner)
            : base(message, inner)
        {
            ReferenceId = referenceId;
            base.Data.Add("ReferenceId", ReferenceId);
        }

        /// <inheritdoc/>
        protected PackReferenceException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {

        }
    }
}
