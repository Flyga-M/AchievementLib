using System;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack
{
    /// <summary>
    /// A collection of <see cref="IResolveContext"/>s.
    /// </summary>
    public class ResolveContextCollection : IResolveContext
    {
        private readonly List<IResolveContext> _contexts;

        /// <inheritdoc/>
        public ResolveContextCollection()
        {
            _contexts = new List<IResolveContext>();
        }

        /// <inheritdoc/>
        public ResolveContextCollection(IResolveContext context) : this()
        {
            _contexts.Add(context);
        }

        /// <inheritdoc/>
        public ResolveContextCollection(IEnumerable<IResolveContext> contexts) : this()
        {
            _contexts.AddRange(contexts);
        }

        /// <summary>
        /// Attempts to add a <paramref name="context"/> to the 
        /// <see cref="ResolveContextCollection"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns><see langword="false"/>, if <paramref name="context"/> is 
        /// <see langword="null"/>, or has already been added. 
        /// Otherwise <see langword="true"/>.</returns>
        public bool TryAddContext(IResolveContext context)
        {
            if (context == null || _contexts.Contains(context))
            {
                return false;
            }

            _contexts.Add(context);
            return true;
        }

        /// <summary>
        /// Returns the <see cref="IResolveContext"/> of type <typeparamref name="TResolveContext"/>.
        /// </summary>
        /// <typeparam name="TResolveContext"></typeparam>
        /// <returns>The requested <see cref="IResolveContext"/>, or <see langword="null"/>, 
        /// if no such <see cref="IResolveContext"/> was added.</returns>
        public TResolveContext GetContext<TResolveContext>() where TResolveContext : IResolveContext
        {
            return (TResolveContext)GetContext(typeof(TResolveContext));
        }

        /// <summary>
        /// Returns the <see cref="IResolveContext"/> of type <paramref name="typeResolveContext"/>.
        /// </summary>
        /// <param name="typeResolveContext"></param>
        /// <returns>The requested <see cref="IResolveContext"/>, or <see langword="null"/>, 
        /// if no such <see cref="IResolveContext"/> was added.</returns>
        public IResolveContext GetContext(Type typeResolveContext)
        {
            if (typeResolveContext == null)
            {
                return null;
            }

            if (!typeof(IResolveContext).IsAssignableFrom(typeResolveContext))
            {
                return null;
            }

            return _contexts.Where(context => context.GetType() == typeResolveContext).FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool CanResolve(object resolvable)
        {
            return _contexts.Any(context => context.CanResolve(resolvable));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc/>
        /// </remarks>
        /// <param name="resolvable"></param>
        /// <returns><inheritdoc/></returns>
        /// <exception cref="InvalidOperationException">If the <paramref name="resolvable"/> 
        /// can't be resolved by any of the added contexts.</exception>
        public object Resolve(object resolvable)
        {
            foreach(IResolveContext context in _contexts.ToArray())
            {
                if (!context.CanResolve(resolvable))
                {
                    continue;
                }

                return context.Resolve(resolvable);
            }

            throw new InvalidOperationException($"Resolvable of type {resolvable.GetType()} " +
                $"can't be resolved by any context in the {nameof(ResolveContextCollection)}.");
        }

        /// <inheritdoc/>
        public bool TryResolve(object resolvable, out object resolved)
        {
            resolved = null;

            foreach (IResolveContext context in _contexts.ToArray())
            {
                if (!context.CanResolve(resolvable))
                {
                    continue;
                }

                return context.TryResolve(resolvable, out resolved);
            }

            return false;
        }
    }
}
