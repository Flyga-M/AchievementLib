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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns><inheritdoc/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <inheritdoc cref="Action.Check(IActionCheckContext)"/>
        public bool Check(IActionCheckContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!Action.Check(context)) // this (internal) condition is not met
            {
                if (OrCondition == null) // no or conditions exist
                {
                    IsFulfilled = false;
                    return IsFulfilled;
                }

                // or conditions exist
                IsFulfilled = OrCondition.Check(context);
                return IsFulfilled;
            }

            // this (internal) condition is met
            if (AndCondition == null) // no and conditions exist
            {
                IsFulfilled = true;
                return IsFulfilled;
            }

            // and conditions exists
            IsFulfilled = AndCondition.Check(context);
            return IsFulfilled;
        }

        /// <inheritdoc/>
        public bool TryCheck(IActionCheckContext context, out bool isFulfilled, out PackSolveException exception)
        {
            try
            {
                Check(context);
            }
            catch (Exception ex)
            {
                exception = new PackSolveException($"Unable to solve condition.", ex);
                isFulfilled = false;
                return false;
            }

            isFulfilled = IsFulfilled;
            exception = null;
            return true;
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
