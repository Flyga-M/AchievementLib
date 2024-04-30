using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="ICondition"/>
    /// This is the V1 implementation.
    /// </summary>
    public class Condition : ICondition, IResolvable
    {
        /// <summary>
        /// If not null, an alternative <see cref="Condition"/> that may be satisfied 
        /// instead of this <see cref="Condition"/> to be true. Functions as an 
        /// OR-condition. [Optional]
        /// </summary>
        public Condition OrCondition { get; set; }

        /// <summary>
        /// If not null, an additional <see cref="Condition"/> that must be satisfied 
        /// with this <see cref="Condition"/> to be true. Functions as an 
        /// AND-condition. [Optional]
        /// </summary>
        public Condition AndCondition { get; set; }

        /// <summary>
        /// The <see cref="Action"/> carrying the data associated with the 
        /// <see cref="Condition"/>.
        /// </summary>
        public Action Action { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        IAction ICondition.Action => Action;

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsFulfilled { get; private set; } = false;

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsResolved
        {
            get
            {
                if (Action is IResolvable resolvable)
                {
                    return resolvable.IsResolved;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return Action != null
                && Action.IsValid()
                && (OrCondition == null || OrCondition.IsValid())
                && (AndCondition == null || AndCondition.IsValid());
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                try
                {
                    Action?.Validate();
                    OrCondition?.Validate();
                    AndCondition?.Validate();
                }
                catch (PackFormatException ex)
                {
                    throw new PackFormatException($"Condition {this} is invalid.", this.GetType(), ex);
                }

                throw new PackFormatException($"Condition {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(Condition)}: {{ " +
                $"\"OrCondition\": {OrCondition}, " +
                $"\"AndCondition\": {AndCondition}, " +
                $"\"Action\": {Action}, " +
                $" }}, Valid?: {IsValid()} }}";
        }

        /// <inheritdoc/>
        public void Resolve(IResolveContext context)
        {
            if (Action is IResolvable resolvable)
            {
                resolvable.Resolve(context);
            }
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            if (Action is IResolvable resolvable)
            {
                return resolvable.TryResolve(context, out exception);
            }

            exception = null;
            return true;
        }
    }
}
