﻿namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Counts the amount of element in the api response at the <see cref="ApiAction.ResultLayer"/>, 
    /// after the <see cref="ApiAction.Filter"/> is applied.
    /// </summary>
    public class ApiActionCount : ApiAction
    {
        /// <inheritdoc/>
        public override bool IsValid()
        {
            return base.IsValid();
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
        public override string ToString()
        {
            return $"{{ {typeof(ApiActionCount)}: {{ " +
                $"\"Endpoint\": {Endpoint}, " +
                $"\"ResultLayer\": {ResultLayer}, " +
                $"\"Filter\": {Filter}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
