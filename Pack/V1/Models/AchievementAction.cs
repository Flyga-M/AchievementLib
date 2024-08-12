using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// Checks whether a <see cref="Achievement"/> is completed.
    /// </summary>
    public class AchievementAction : Action, IResolvable, IDisposable
    {
        /// <inheritdoc/>
        public event EventHandler Resolved;
        
        /// <summary>
        /// The full name (id) of the <see cref="Models.Achievement"/> in the hierarchy tree.
        /// </summary>
        public ResolvableHierarchyReference Achievement { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsResolved => Achievement?.IsResolved ?? false;

        /// <summary>
        /// Instantiates an <see cref="AchievementAction"/>.
        /// </summary>
        /// <param name="achievement"></param>
        [JsonConstructor]
        public AchievementAction(ResolvableHierarchyReference achievement)
        {
            Achievement = achievement;

            if (Achievement != null)
            {
                if (Achievement.IsResolved)
                {
                    OnAchievementResolved(achievement, null);
                }

                achievement.Resolved += OnAchievementResolved;
            }
        }

        private void OnAchievementResolved(object sender, EventArgs _)
        {
            if (!(sender is IResolvableHierarchyReference resolvable))
            {
                throw new AchievementLibInternalException("OnAchievementResolved sender could not be unboxed to " +
                    $"IResolvableHierarchyReference. Given type: {sender.GetType()}.");
            }

            if (!(resolvable.Reference is IAchievement achievement))
            {
                throw new PackReferenceException("Reference in AchievementAction must be to another IAchievement. " +
                    $"Referenced type: {Achievement.Reference?.GetType()}.");
            }

            OnAchievementFulfilledChanged(achievement, achievement.IsFulfilled);

            achievement.FulfilledChanged += OnAchievementFulfilledChanged;
        }

        private void OnAchievementFulfilledChanged(object _, bool isFulfilled)
        {
            IsFulfilled = isFulfilled;
        }

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
            if (!(Achievement.Reference is IAchievement referencedAchievement))
            {
                throw new PackReferenceException("Reference in AchievementAction must be to another IAchievement. " +
                    $"Referenced type: {Achievement.Reference.GetType()}.");
            }

            Resolved?.Invoke(this, null);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            if (!Achievement.TryResolve(context, out exception))
            {
                return false;
            }

            if (!(Achievement.Reference is IAchievement referencedAchievement))
            {
                exception = new PackReferenceException("Reference in AchievementAction must be to another IAchievement. " +
                    $"Referenced type: {Achievement.Reference.GetType()}.");
                return false;
            }

            Resolved?.Invoke(this, null);

            exception = null;
            return true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Achievement != null)
            {
                Achievement.Resolved -= OnAchievementResolved;

                if (Achievement.Reference is IAchievement achievement)
                {
                    achievement.FulfilledChanged -= OnAchievementFulfilledChanged;
                }
            }

            Resolved = null;
        }
    }
}
