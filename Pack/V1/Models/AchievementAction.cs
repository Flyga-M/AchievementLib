using Microsoft.Xna.Framework;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether a <see cref="Achievement"/> is completed.
    /// </summary>
    public class AchievementAction : Action, IResolvable
    {
        /// <summary>
        /// The full name (id) of the <see cref="Models.Achievement"/> in the hierarchy tree.
        /// </summary>
        public ResolvableHierarchyReference Achievement { get; set; }

        /// <inheritdoc/>
        public bool IsResolved => Achievement?.IsResolved ?? false;

        /// <inheritdoc/>
        public override bool IsValid()
        {
            return Achievement.IsValid();
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public override void Validate()
        {
            try
            {
                Achievement?.Validate();
            }
            catch (PackFormatException ex)
            {
                throw new PackFormatException($"AchievementAction {this} is invalid.", this.GetType(), ex);
            }

            throw new PackFormatException($"AchievementAction {this} is invalid.", this.GetType());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(AchievementAction)}: {{ " +
                $"\"Achievement\": {Achievement}, " +
                $" }}, Valid?: {IsValid()} }}";
        }

        ///<inheritdoc cref="ResolvableHierarchyReference.Resolve(IResolveContext)"/>
        public void Resolve(IResolveContext context)
        {
            Achievement.Resolve(context);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            return Achievement.TryResolve(context, out exception);
        }
    }
}
