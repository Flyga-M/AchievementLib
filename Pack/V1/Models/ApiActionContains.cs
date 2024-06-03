using System.Collections.Generic;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks, whether the <see cref="Value"/> of the element determined by 
    /// <see cref="ChooseOption"/> exists in the api response at the <see cref="ApiAction.ResultLayer"/>, 
    /// after the <see cref="ApiAction.Filter"/> is applied.
    /// </summary>
    public class ApiActionContains : ApiAction
    {
        /// <summary>
        /// The value, that the response from the Api should contain.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The element that will be chosen, if the api response contains more than 1 element. 
        /// [Optional]
        /// </summary>
        public ChooseOption ChooseOption { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Value)
                && base.IsValid();
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    base.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"{this.GetType()} {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"{this.GetType()} {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        protected override Dictionary<string, object> InnerToString()
        {
            Dictionary<string, object> inner = base.InnerToString();

            inner.Add($"{nameof(Value)}", Value);
            inner.Add($"{nameof(ChooseOption)}", ChooseOption);

            return inner;
        }
    }
}
