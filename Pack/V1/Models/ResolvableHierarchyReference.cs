using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IResolvableHierarchyReference"/>
    /// This is the V1 implementation.
    /// </summary>
    public class ResolvableHierarchyReference : IResolvableHierarchyReference, IValidateable
    {
        /// <inheritdoc/>
        public event EventHandler Resolved;
        
        /// <inheritdoc/>
        public string ReferenceId {get; set;}

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Reference { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsResolved => Reference != null;

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="PackReferenceException"></exception>
        public void Resolve(IResolveContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.CanResolve(this))
            {
                throw new InvalidOperationException($"This {nameof(ResolvableHierarchyReference)} can not " +
                    $"be resolved by the provided {nameof(context)} ({context.GetType()}).");
            }

            if (string.IsNullOrWhiteSpace(ReferenceId))
            {
                throw new InvalidOperationException("ReferenceId is null or invalid.");
            }

            try
            {
                Reference = context.Resolve(this) as IHierarchyObject;
            }
            catch (Exception ex)
            {
                throw new PackReferenceException($"ReferenceId {ReferenceId} could not be " +
                    $"resolved with the provided context.", ReferenceId, ex);
            }

            if (Reference == null)
            {
                throw new PackReferenceException($"ReferenceId {ReferenceId} could not be " +
                    $"resolved with the provided context.", ReferenceId);
            }

            Resolved?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            exception = null;

            try
            {
                Resolve(context);
            }
            catch (Exception ex)
            {
                exception = new PackReferenceException("Resolving of Hierarchy Reference failed.", ex);
                return false;
            }

            return IsResolved;
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ReferenceId);
        }

        /// <inheritdoc/>
        /// <exception cref="PackFormatException"></exception>
        public void Validate()
        {
            if (!IsValid())
            {
                throw new PackFormatException($"ResolvableHierarchyReference {this} is invalid.", this.GetType());
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{{ {typeof(ResolvableHierarchyReference)}: {{ " +
                $"\"ReferenceId\": {ReferenceId}, " +
                $" }}, Valid?: {IsValid()} }}";
        }
    }
}
