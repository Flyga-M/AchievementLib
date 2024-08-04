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
        /// Returns the highest <see cref="IHierarchyObject"/> in the hierarchy tree.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <returns>The highest <see cref="IHierarchyObject"/> in the hierarchy tree.</returns>
        public static IHierarchyObject GetRoot(this IHierarchyObject hierarchyObject)
        {
            if (hierarchyObject.Parent == null)
            {
                return hierarchyObject;
            }

            return hierarchyObject.Parent.GetRoot();
        }

        /// <summary>
        /// Checks whether the <see cref="IHierarchyObject"/> is part of the given
        /// <paramref name="namespace"/>.
        /// </summary>
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
        /// Determines whether the <see cref="IHierarchyObject"/> is a parent (according to the namespace) of 
        /// the <see cref="IHierarchyObject"/> with the given <paramref name="fullName"/>.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <param name="fullName"></param>
        /// <returns>True, if the <paramref name="fullName"/> starts with the fullName of the 
        /// <see cref="IHierarchyObject"/>. Otherwise false.</returns>
        public static bool IsNamespaceParentOf(this IHierarchyObject hierarchyObject, string fullName)
        {
            string @namespace = hierarchyObject.GetFullName();

            return fullName.StartsWith(@namespace);
        }

        /// <summary>
        /// Checks whether the <see cref="IHierarchyObject"/> could be a part of the same 
        /// namespace as the given <paramref name="fullName"/>.
        /// </summary>
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
        /// Attempts to resolve all references of all children of the <see cref="IHierarchyObject"/>. 
        /// Might contain information on failed attempts. Will continue through all 
        /// children, even if one child fails.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <param name="context"></param>
        /// <param name="exceptions"></param>
        /// <returns>True, if all childrens references were successfully resolved. False, if at least one 
        /// child failed to resolve a reference.</returns>
        public static bool TryResolveChildren(this IHierarchyObject hierarchyObject, IResolveContext context, out PackReferenceException[] exceptions)
        {
            exceptions = Array.Empty<PackReferenceException>();

            if (hierarchyObject.Children == null || !hierarchyObject.Children.Any())
            {
                return true;
            }

            bool eval = true;

            List<PackReferenceException> allExceptions = new List<PackReferenceException>();

            foreach (IHierarchyObject child in hierarchyObject.Children)
            {
                if (!child.TryResolveChildren(context, out PackReferenceException[] grandChildrenExceptions))
                {
                    eval = false;
                    allExceptions.AddRange(grandChildrenExceptions);
                }

                if (child is IResolvable resolvable)
                {
                    if (!resolvable.TryResolve(context, out PackReferenceException childException))
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
        /// Determines whether the <see cref="IHierarchyObject"/> is combinable with the <paramref name="other"/> one.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <param name="other"></param>
        /// <returns><see langword="true"/>, if the <see cref="IHierarchyObject"/>s are combinable. 
        /// Otherwise <see langword="false"/>.</returns>
        public static bool IsCombinable(this IHierarchyObject hierarchyObject, IHierarchyObject other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.GetType() != hierarchyObject.GetType())
            {
                return false;
            }

            if (hierarchyObject == other)
            {
                return false;
            }

            if (hierarchyObject.GetFullName() != other.GetFullName())
            {
                return false;
            }

            return true;
        }

        // TODO: this will currently not release "top level" resources that need to be disposed
        // we can't just call Dispose() on the other object, because the added children may be disposed as well

        // TODO: this might mess up references that were already resolved

        /// <summary>
        /// Attempts to combine the <see cref="IHierarchyObject"/> with the <paramref name="other"/> one.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <param name="other"></param>
        /// <returns><see langword="true"/>, if the objects are combinable and the children of <paramref name="other"/> 
        /// were successfully added to this <see cref="IHierarchyObject"/> and successfully combined if 
        /// applicable. Otherwise <see langword="false"/>.</returns>
        public static bool TryCombine(this IHierarchyObject hierarchyObject, IHierarchyObject other)
        {
            if (!hierarchyObject.IsCombinable(other))
            {
                return false;
            }

            bool success = true;
            List<IHierarchyObject> successfullyAddedChildren = new List<IHierarchyObject>();

            foreach (IHierarchyObject child in other.Children)
            {
                if (hierarchyObject.TryAddChild(child))
                {
                    successfullyAddedChildren.Add(child);
                }
                else
                {
                    success = false;
                    break;
                }
            }

            // TODO: implement TryRemoveChild method?
            //if (!success)
            //{
            //    foreach(IHierarchyObject child in successfullyAddedChildren)
            //    {
            //        hierarchyObject.TryRemoveChild(child);
            //    }
            //}

            if (!success)
            {
                return false;
            }

            return hierarchyObject.TryCombineChildren();
        }

        /// <summary>
        /// Attempts to combine the children of the <see cref="IHierarchyObject"/> with each other.
        /// </summary>
        /// <param name="hierarchyObject"></param>
        /// <returns><see langword="true"/>, if all similar children were sucessfully combined. Otherwise 
        /// <see langword="false"/>.</returns>
        public static bool TryCombineChildren(this IHierarchyObject hierarchyObject)
        {
            List<IHierarchyObject> remainingChildren = new List<IHierarchyObject>(hierarchyObject.Children);
            
            while (remainingChildren.Any())
            {
                IHierarchyObject child = remainingChildren.First();
                remainingChildren.Remove(child);

                if (!ContainsSimilarChildren(remainingChildren, child, out IHierarchyObject[] similarChildren))
                {
                    continue;
                }

                foreach (IHierarchyObject similarChild in similarChildren)
                {
                    if (!child.TryCombine(similarChild))
                    {
                        return false;
                    }

                    remainingChildren.Remove(similarChild);
                }
            }

            return true;
        }

        private static bool ContainsSimilarChildren(IEnumerable<IHierarchyObject> children, IHierarchyObject child, out IHierarchyObject[] similarChildren)
        {
            similarChildren = Array.Empty<IHierarchyObject>();

            if (children == null || !children.Any() || child == null)
            {
                return false;
            }

            similarChildren = children.Where(currentChild => currentChild.IsCombinable(child)).ToArray();

            return similarChildren.Any();
        }
    }
}
