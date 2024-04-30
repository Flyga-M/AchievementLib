using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// The action, that determines how a condition can be resolved.
    /// </summary>
    public abstract class Action : IAction
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsFulfilled { get; private set; } = false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns><inheritdoc/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Check(IActionCheckContext context)
        {
            if (!context.CanCheck(this))
            {
                throw new InvalidOperationException($"Unable to check action of type {this.GetType()}, because " +
                    $"the provided context does not contain an appropriate entry.");
            }

            IsFulfilled = context.IsMet(this);

            return IsFulfilled;
        }

        /// <inheritdoc/>
        public abstract bool IsValid();

        /// <inheritdoc/>
        public bool TryCheck(IActionCheckContext context, out bool isFulfilled, out PackSolveException exception)
        {
            try
            {
                Check(context);
            }
            catch (Exception ex)
            {
                exception = new PackSolveException($"Unable to solve action of type {this.GetType()}.", ex);
                isFulfilled = false;
                return false;
            }

            isFulfilled = IsFulfilled;
            exception = null;
            return true;
        }

        /// <inheritdoc/>
        public abstract void Validate();
    }
}
