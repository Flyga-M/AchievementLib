using System;
using System.Collections.Concurrent;

namespace AchievementLib.Pack
{
    /// <inheritdoc cref="IActionCheckContext"/>
    public class ActionCheckContext : IActionCheckContext
    {
        private readonly ConcurrentDictionary<Type, Func<IAction, bool>> _actionCheckers = new ConcurrentDictionary<Type, Func<IAction, bool>>();

        /// <inheritdoc/>
        public bool CanCheck<TAction>(TAction action) where TAction : IAction
        {
            return _actionCheckers.ContainsKey(typeof(TAction));
        }

        /// <inheritdoc/>
        public bool IsMet<TAction>(TAction action) where TAction : IAction
        {
            if (!CanCheck(action))
            {
                return false;
            }

            return _actionCheckers[typeof(TAction)](action);
        }

        /// <summary>
        /// Adds a <paramref name="actionChecker"/> to the <see cref="ActionCheckContext"/>.
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="actionChecker"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add<TAction>(Func<IAction, bool> actionChecker) where TAction : IAction
        {
            if (actionChecker == null)
            {
                throw new ArgumentNullException(nameof(actionChecker));
            }

            if (_actionCheckers.ContainsKey(typeof(TAction)))
            {
                throw new ArgumentException("Can't add action checker to Action Check Context, whose type is already " +
                    "added.");
            }

            _actionCheckers[typeof(TAction)] = actionChecker;
        }

        /// <summary>
        /// Attempts to add a <paramref name="actionChecker"/> to the <see cref="ActionCheckContext"/>.
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <param name="actionChecker"></param>
        /// <param name="exception"></param>
        /// <returns>True, if the <paramref name="actionChecker"/> was successfully added. Otherwise false.</returns>
        public bool TryAdd<TAction>(Func<IAction, bool> actionChecker, out AchievementLibException exception) where TAction : IAction
        {
            try
            {
                Add<TAction>(actionChecker);
            }
            catch (Exception ex)
            {
                exception = new AchievementLibException("Unable to add action checker to Action Check " +
                    "Context.", ex);
                return false;
            }
            exception = null;
            return true;
        }

        /// <summary>
        /// Removes the action checker for the <typeparamref name="TAction"/> from 
        /// the <see cref="HierarchyResolveContext"/>.
        /// </summary>
        /// <typeparam name="TAction"></typeparam>
        /// <returns>True, if a action checker for the <typeparamref name="TAction"/> is contained in 
        /// the <see cref="HierarchyResolveContext"/> and successfully removed. Otherwise false.</returns>
        public bool TryRemove<TAction>() where TAction : IAction
        {
            return _actionCheckers.TryRemove(typeof(TAction), out Func<IAction, bool> _);
        }
    }
}
