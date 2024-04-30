namespace AchievementLib.Pack
{
    /// <summary>
    /// <inheritdoc cref="IResolveContext"/>
    /// Provides information on other objects inside the hierarchy tree(s).
    /// </summary>
    public interface IHierarchyResolveContext : IResolveContext
    {
        /// <summary>
        /// Attempts to resolve the <paramref name="fullName"/> with the information provided by the 
        /// <see cref="HierarchyResolveContext"/>.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="resolved"></param>
        /// <returns>True, if the <paramref name="fullName"/> was successfully resolved. Otherwise false.</returns>
        bool TryResolveId(string fullName, out IHierarchyObject resolved);
    }
}
