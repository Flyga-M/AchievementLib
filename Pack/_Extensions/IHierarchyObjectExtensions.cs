using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Provides extension methods for the <see cref="IHierarchyObject"/> interface.
    /// </summary>
    public static class IHierarchyObjectExtensions
    {
        /// <summary>
        /// Returns the full name of the <see cref="IHierarchyObject"/> inside 
        /// it's hierarchy tree.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't do anything.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        /// <returns>The full name of the <see cref="IHierarchyObject"/> inside 
        /// it's hierarchy tree.</returns>
        public static string GetFullName(this IHierarchyObject hierarchyObject)
        {
            if (hierarchyObject.Parent != null)
            {
                return hierarchyObject.Parent.GetFullName() + "." + hierarchyObject.Id;
            }

            return hierarchyObject.Id;
        }

        /// <summary>
        /// Returns the child with the given 
        /// <paramref name="relativeId"/> in the hierarchy tree below the 
        /// <see cref="IHierarchyObject"/>, or null if none was found.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't do anything.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        /// <param name="relativeId"></param>
        /// <returns>The child with the given 
        /// <paramref name="relativeId"/> in the hierarchy tree below the 
        /// <see cref="IHierarchyObject"/>, or null if none was found.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IHierarchyObject GetChild(this IHierarchyObject hierarchyObject, string relativeId)
        {
            if (string.IsNullOrWhiteSpace(relativeId))
            {
                throw new ArgumentException("relativeId can't be null, empty " +
                    "or whitespace.", nameof(relativeId));
            }

            if (hierarchyObject.Children == null)
            {
                return null;
            }

            string[] parts = relativeId.Split(new char[] { '.' }, 2);

            IHierarchyObject firstChild = hierarchyObject.Children.Where(child => child.Id == parts.First()).FirstOrDefault();

            if (firstChild == null)
            {
                return null;
            }

            if (parts.Length == 1)
            {
                return firstChild;
            }

            return firstChild.GetChild(parts[1]);
        }

        /// <summary>
        /// Checks whether the <see cref="IHierarchyObject"/> is part of the given
        /// <paramref name="namespace"/>.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't be able to get the full name of the <see cref="IHierarchyObject"/>.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        /// <param name="namespace"></param>
        /// <returns>True, if the full name of the <see cref="IHierarchyObject"/> 
        /// starts with the <paramref name="namespace"/>. Otherwise false.</returns>
        public static bool IsInNamespace(this IHierarchyObject hierarchyObject, string @namespace)
        {
            string fullName = hierarchyObject.GetFullName();

            return fullName.StartsWith(@namespace);
        }

        /// <summary>
        /// Checks whether the <see cref="IHierarchyObject"/> could be a part of the same 
        /// namespace as the given <paramref name="fullName"/>.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't be able to get the full name of the <see cref="IHierarchyObject"/>.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        /// <param name="fullName"></param>
        /// <returns>True, if the first part of the full name of the 
        /// <see cref="IHierarchyObject"/> equals the first part of the 
        /// <paramref name="fullName"/>. Otherwise false.</returns>
        public static bool CouldBeInSameNamespace(this IHierarchyObject hierarchyObject, string fullName)
        {
            string thisFullName = hierarchyObject.GetFullName();

            string[] thisSplit = thisFullName.Split(new char[] { '.' });
            string[] otherSplit = fullName.Split(new char[] { '.' });

            return thisSplit.FirstOrDefault() == otherSplit.FirstOrDefault();
        }

        /// <summary>
        /// Disposes all the disposable children of the <see cref="IHierarchyObject"/>.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't do anything.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        public static void DisposeChildren(this IHierarchyObject hierarchyObject)
        {
            if (hierarchyObject.Children == null)
            {
                return;
            }
            
            foreach(IHierarchyObject child in hierarchyObject.Children)
            {
                child.DisposeChildren();
                if (child is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Attempts to load all resources of all children of the 
        /// <see cref="IHierarchyObject"/>. Might contain information on failed 
        /// attempts in <paramref name="exceptions"/>. Will continue through all 
        /// children, even if one child fails.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't do anything.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="exceptions"></param>
        /// <returns>True, if all children successfully loaded their 
        /// resources. False, if at least one childs resources were not 
        /// loaded successfully.</returns>
        public static bool TryLoadChildrensResources(this IHierarchyObject hierarchyObject, AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, out PackResourceException[] exceptions)
        {
            exceptions = Array.Empty<PackResourceException>();

            if (hierarchyObject.Children == null)
            {
                return true;
            }

            bool eval = true;

            List<PackResourceException> allExceptions = new List<PackResourceException>();

            foreach (IHierarchyObject child in hierarchyObject.Children)
            {
                if (!child.TryLoadChildrensResources(resourceManager, graphicsDevice, out PackResourceException[] grandChildrenExceptions))
                {
                    eval = false;
                    allExceptions.AddRange(grandChildrenExceptions);
                }

                if (child is ILoadable loadable)
                {
                    if (!loadable.TryLoad(resourceManager, graphicsDevice, out PackResourceException childException))
                    {
                        eval = false;
                        allExceptions.Add(childException);
                    }
                }
            }

            exceptions = allExceptions.ToArray();

            return eval;
        }

        /// <summary>
        /// Attempts to load all resources of all children of the 
        /// <see cref="IHierarchyObject"/> asynchronously. Might contain information on failed 
        /// attempts. Will continue through all children, even if one child fails.
        /// </summary>
        /// <remarks>
        /// Make sure that <see cref="AssertHierarchy(IHierarchyObject, IHierarchyObject)"/> 
        /// was called beforehand, or children and parents have been manually set. Otherwise this 
        /// won't do anything.
        /// </remarks>
        /// <param name="hierarchyObject"></param>
        /// <param name="resourceManager"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True, if all children successfully loaded their 
        /// resources. False, if at least one childs resources were not 
        /// loaded successfully. Might contain information on failed 
        /// attempts.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async static Task<(bool, PackResourceException[])> TryLoadChildrensResourcesAsync(this IHierarchyObject hierarchyObject, AchievementPackResourceManager resourceManager, GraphicsDevice graphicsDevice, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<PackResourceException> allExceptions = new List<PackResourceException>();

            if (hierarchyObject.Children == null || !hierarchyObject.Children.Any())
            {
                return (true, Array.Empty<PackResourceException>());
            }

            bool eval = true;

            foreach (IHierarchyObject child in hierarchyObject.Children)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool grandChildrenSuccess = false;
                PackResourceException[] grandChildrenExceptions = Array.Empty<PackResourceException>();

                try
                {
                    (grandChildrenSuccess, grandChildrenExceptions) = await child.TryLoadChildrensResourcesAsync(resourceManager, graphicsDevice, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                
                if (!grandChildrenSuccess)
                {
                    eval = false;
                    allExceptions.AddRange(grandChildrenExceptions);
                }

                if (child is ILoadable loadable)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    bool childSuccess = false;
                    PackResourceException childException = null;

                    try
                    {
                        (childSuccess, childException) = await loadable.TryLoadAsync(resourceManager, graphicsDevice, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!childSuccess)
                    {
                        eval = false;
                        allExceptions.Add(childException ?? new PackResourceException("An internal exception occured.", new AchievementLibInternalException("PackResourceException is null.")));
                    }
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            return (eval, allExceptions.ToArray());
        }

        /// <summary>
        /// Asserts the hierarchy for the <see cref="IHierarchyObject"/> and all it's children.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <param name="parent"></param>
        public static void AssertHierarchy(this IHierarchyObject hierarchyObject, IHierarchyObject parent)
        {
            hierarchyObject.Parent = parent;
            
            if (hierarchyObject.Children == null || !hierarchyObject.Children.Any())
            {
                return;
            }

            foreach (IHierarchyObject child in hierarchyObject.Children)
            {
                child.AssertHierarchy(hierarchyObject);
            }
        }
    }
}
