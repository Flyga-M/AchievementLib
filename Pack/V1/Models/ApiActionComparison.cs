using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Compares the value from the given <see cref="Key"/> of the elements depending on 
    /// the <see cref="ComparisonTarget"/> in the api response at the <see cref="ApiAction.ResultLayer"/>, 
    /// after the <see cref="ApiAction.Filter"/> is applied, to the <see cref="Value"/>.
    /// </summary>
    public class ApiActionComparison : ApiAction
    {
        /// <summary>
        /// The key of the value that should be compared.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value, that the api response should be compared to.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The <see cref="Comparison"/> that evaluates the values. [Optional]
        /// </summary>
        public Comparison Comparison { get; set; }

        /// <summary>
        /// Determines how many elements need to be compared against. [Optional]
        /// </summary>
        public ComparisonTarget ComparisonTarget { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Key)
                && !string.IsNullOrWhiteSpace(Value)
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

            inner.Add($"{nameof(Key)}", Key);
            inner.Add($"{nameof(Value)}", Value);
            inner.Add($"{nameof(Comparison)}", Comparison);
            inner.Add($"{nameof(ComparisonTarget)}", ComparisonTarget);

            return inner;
        }
    }
}
