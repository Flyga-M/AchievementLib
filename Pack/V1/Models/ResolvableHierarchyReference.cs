using Newtonsoft.Json;
using System;

namespace AchievementLib.Pack.V1.Models
{
    /// <summary>
    /// <inheritdoc cref="IResolvableHierarchyReference"/>
    /// This is the V1 implementation.
    /// </summary>
    public class ResolvableHierarchyReference : IResolvableHierarchyReference
    {
        /// <inheritdoc/>
        public string ReferenceId {get; set;}

        /// <inheritdoc/>
        [JsonIgnore]
        public IHierarchyObject Reference { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        [JsonIgnore]
        public bool IsResolved => Reference != null;

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Resolve(IResolveContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!(context is IHierarchyResolveContext hierarchyContext))
            {
                throw new ArgumentException($"Must provide IResolveContext that is {typeof(IHierarchyResolveContext)}. " +
                    $"Provided context: {context.GetType()}", nameof(context));
            }

            Resolve(hierarchyContext);
        }

        /// <inheritdoc cref="IResolvable.Resolve(IResolveContext)"/>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Resolve(IHierarchyResolveContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(ReferenceId))
            {
                throw new InvalidOperationException("ReferenceId is null or invalid.");
            }

            if (!context.TryResolveId(ReferenceId, out IHierarchyObject resolved))
            {
                throw new InvalidOperationException($"ReferenceId {ReferenceId} could not be found in the provided context.");
            }

            if (resolved == null)
            {
                throw new InvalidOperationException($"ReferenceId {ReferenceId} could not be found in the provided context.");
            }

            Reference = resolved;
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolveContext context, out PackReferenceException exception)
        {
            if (!(context is IHierarchyResolveContext hierarchyContext))
            {
                exception = new PackReferenceException($"Provided IResolveContext ({context.GetType()}) is not " +
                    $"of correct type {typeof(IHierarchyResolveContext)}.");
                return false;
            }

            return TryResolve(hierarchyContext, out exception);
        }

        /// <inheritdoc cref="IResolvable.TryResolve(IResolveContext, out PackReferenceException)"/>
        public bool TryResolve(IHierarchyResolveContext context, out PackReferenceException exception)
        {
            try
            {
                Resolve(context);
            }
            catch (Exception ex)
            {
                exception = new PackReferenceException("Resolving of Hierarchy Reference failed.", ex);
                return false;
            }

            exception = null;
            return IsResolved;
        }
    }
}
