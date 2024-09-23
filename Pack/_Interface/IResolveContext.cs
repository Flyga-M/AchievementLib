namespace AchievementLib.Pack
{
    /// <summary>
    /// Provides context for an <see cref="IResolvable"/> to be resolved.
    /// </summary>
    public interface IResolveContext
    {
        /// <summary>
        /// Determines whether the type of <paramref name="resolvable"/> can be 
        /// resolved by the <see cref="IResolveContext"/>.
        /// </summary>
        /// <param name="resolvable"></param>
        /// <returns><see langword="true"/>, if the <paramref name="resolvable"/> can 
        /// be resolved by the <see cref="IResolveContext"/>. 
        /// Otherwise <see langword="false"/>.</returns>
        bool CanResolve(object resolvable);

        /// <summary>
        /// Resolves the <paramref name="resolvable"/>.
        /// </summary>
        /// <remarks>
        /// Might throw exceptions.
        /// </remarks>
        /// <param name="resolvable"></param>
        /// <returns>The resolved <paramref name="resolvable"/>.</returns>
        object Resolve(object resolvable);

        /// <summary>
        /// Attempts to resolve the <paramref name="resolvable"/>.
        /// </summary>
        /// <param name="resolvable"></param>
        /// <param name="resolved"></param>
        /// <returns><see langword="true"/>, if the <paramref name="resolvable"/> was 
        /// successfully resolved. Otherwise <see langword="false"/>.</returns>
        bool TryResolve(object resolvable, out object resolved);
    }

    /// <summary>
    /// <inheritdoc cref="IResolveContext"/>
    /// </summary>
    /// <typeparam name="TResolvable">The type of the resolvable.</typeparam>
    /// <typeparam name="TResolved">The type of the resolved.</typeparam>
    public interface IResolveContext<TResolvable, TResolved> : IResolveContext where TResolvable : IResolvable
    {
        /// <inheritdoc cref="IResolveContext.Resolve(object)"/>
        TResolved Resolve(TResolvable resolvable);

        /// <inheritdoc cref="IResolveContext.TryResolve(object, out object)"/>
        bool TryResolve(TResolvable resolvable, out TResolved resolved);
    }
}
