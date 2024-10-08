﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AchievementLib.Pack
{
    /// <summary>
    /// <inheritdoc cref="IHierarchyResolveContext"/>
    /// </summary>
    public class HierarchyResolveContext : IHierarchyResolveContext
    {
        private readonly SafeList<IHierarchyObject> _roots = new SafeList<IHierarchyObject>();

        /// <summary>
        /// Initializes a <see cref="HierarchyResolveContext"/> without any roots.
        /// </summary>
        public HierarchyResolveContext() : this(Array.Empty<IHierarchyObject>()) 
        {
            
        }

        /// <summary>
        /// Initializes a <see cref="HierarchyResolveContext"/> with the given <paramref name="roots"/>.
        /// </summary>
        /// <param name="roots"></param>
        public HierarchyResolveContext(IEnumerable<IHierarchyObject> roots)
        {
            if (roots == null)
            {
                return;
            }
            
            foreach (IHierarchyObject root in roots)
            {
                Add(root);
            }
        }

        /// <summary>
        /// Attempts to add a <paramref name="root"/> to the <see cref="HierarchyResolveContext"/>.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <paramref name="root"/> was successfully added. Otherwise false.</returns>
        public bool TryAdd(IHierarchyObject root, out AchievementLibException exception)
        {
            try
            {
                Add(root);
            }
            catch (Exception ex)
            {
                exception = new AchievementLibException("Unable to add root to Hierarchy Resolve " +
                    "Context.", ex);
                return false;
            }
            exception = null;
            return true;
        }

        /// <summary>
        /// Adds a <paramref name="root"/> to the <see cref="HierarchyResolveContext"/>.
        /// </summary>
        /// <param name="root"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Add(IHierarchyObject root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (root.Parent != null)
            {
                throw new ArgumentException("Can't add object to Hierarchy Resolve Context, that is not a root.",
                    nameof(root));
            }

            if (_roots.Contains(root))
            {
                throw new InvalidOperationException("Can't add root to Hierarchy Resolve Context, that is already added.");
            }

            if(_roots.Any(existingRoot => root.IsInNamespace(existingRoot.GetFullName())))
            {
                throw new InvalidOperationException("Can't add root to Hierarchy Resolve context, because the namespace " +
                    "is already part of the context.");
            }

            _roots.Add(root);
        }

        /// <summary>
        /// Removes a <paramref name="root"/> from the <see cref="HierarchyResolveContext"/>.
        /// </summary>
        /// <param name="root"></param>
        /// <returns>True, if the <paramref name="root"/> is contained in the <see cref="HierarchyResolveContext"/> 
        /// and successfully removed. Otherwise false.</returns>
        public bool TryRemove(IHierarchyObject root)
        {
            return _roots.Remove(root);
        }

        /// <summary>
        /// Removes all roots from the <see cref="HierarchyResolveContext"/>.
        /// </summary>
        public void Clear()
        {
            _roots.Clear();
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">If the <paramref name="resolvable"/> 
        /// can't be resolved by any of the added roots</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="resolvable"/> is 
        /// <see langword="null"/>.</exception>
        public IHierarchyObject Resolve(IResolvableHierarchyReference resolvable)
        {
            if (resolvable == null)
            {
                throw new ArgumentNullException(nameof(resolvable));
            }

            IEnumerable<IHierarchyObject> candidates = _roots.Where(root => root.IsNamespaceParentOf(resolvable.ReferenceId));

            if (!candidates.Any())
            {
                throw new InvalidOperationException($"Resolvable with id " +
                    $"{resolvable.ReferenceId} can't be resolved by any of " +
                    $"the added roots.");
            }

            IHierarchyObject resolved = null;

            foreach (IHierarchyObject root in candidates)
            {
                string @namespace = root.GetFullName();

                if (@namespace == resolvable.ReferenceId)
                {
                    return root;
                }

                string relativeId = resolvable.ReferenceId.Substring(@namespace.Length + 1); // +1 for the dot

                if (string.IsNullOrWhiteSpace(relativeId))
                {
                    continue;
                }

                resolved = root.GetChild(relativeId);

                if (resolved != null)
                {
                    break;
                }
            }

            if (resolved == null)
            {
                throw new InvalidOperationException($"Resolvable with id " +
                    $"{resolvable.ReferenceId} can't be resolved by any of " +
                    $"the added roots.");
            }

            return resolved;
        }

        /// <inheritdoc/>
        public bool TryResolve(IResolvableHierarchyReference resolvable, out IHierarchyObject resolved)
        {
            resolved = null;
            try
            {
                resolved = Resolve(resolvable);
            }
            catch (Exception)
            {
                // NOOP
            }

            return resolved != null;
        }

        /// <inheritdoc/>
        public bool CanResolve(object resolvable)
        {
            if (resolvable == null)
            {
                return false;
            }

            return typeof(IResolvableHierarchyReference).IsAssignableFrom(resolvable.GetType());
        }

        /// <inheritdoc/>
        public object Resolve(object resolvable)
        {
            return Resolve(resolvable as IResolvableHierarchyReference);
        }

        /// <inheritdoc/>
        public bool TryResolve(object resolvable, out object resolved)
        {
            resolved = null;

            if (!CanResolve(resolvable))
            {
                return false;
            }

            return TryResolve(resolvable as IResolvableHierarchyReference, out resolved);
        }
    }
}
