namespace AchievementLib.Pack
{
    /// <summary>
    /// Provides context for an <see cref="IAction"/> to be checked.
    /// </summary>
    public interface IActionCheckContext
    {
        /// <summary>
        /// Determines whether the <see cref="IActionCheckContext"/> is able to check 
        /// <see cref="IAction">IActions</see> of the given <typeparamref name="TAction"/>.
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="action"></param>
        /// <returns>True, if the <paramref name="action"/> can be checked by the 
        /// <see cref="IActionCheckContext"/>.</returns>
        bool CanCheck<TAction>(TAction action) where TAction : IAction;

        /// <summary>
        /// Attempts to determine, whether the given <paramref name="action"/> is met.
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="action"></param>
        /// <returns>True, if the <paramref name="action"/> can be checked by this context and is met. 
        /// Otherwise false.</returns>
        bool IsMet<TAction>(TAction action) where TAction : IAction;
    }
}
