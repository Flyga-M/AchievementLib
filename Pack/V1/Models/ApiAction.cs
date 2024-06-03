using System.Collections.Generic;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Represents an <see cref="Action"/>, that uses the GW2 API to evaluates it's status.
    /// </summary>
    public abstract class ApiAction : Action
    {
        /// <summary>
        /// The Api endpoint the <see cref="Condition"/> needs to access information.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The <see cref="Layer"/> on which the action result will be determined. [Optional]
        /// </summary>
        public Layer ResultLayer { get; set; }

        /// <summary>
        /// The <see cref="Restraint"/> that is applied to the api call before the action is executed. 
        /// [Optional]
        /// </summary>
        public Restraint Filter { get; set; }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Endpoint)
                && (ResultLayer == null || ResultLayer.IsValid())
                && (Filter == null || Filter.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    ResultLayer?.Validate();
                    Filter?.Validate();
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

            inner.Add($"{nameof(Endpoint)}", Endpoint);
            inner.Add($"{nameof(ResultLayer)}", ResultLayer);
            inner.Add($"{nameof(Filter)}", Filter);

            return inner;
        }
    }
}
