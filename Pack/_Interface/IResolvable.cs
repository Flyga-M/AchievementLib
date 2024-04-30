using Newtonsoft.Json;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class that can be resolved.
    /// </summary>
    public interface IResolvable
    {
        /// <summary>
        /// True, if the <see cref="IResolvable"/> was successfully resolved. 
        /// Otherwise false.
        /// </summary>
        [JsonIgnore]
        bool IsResolved { get; }

        /// <summary>
        /// Resolves the <see cref="IResolvable"/>. Might throw Exceptions.
        /// </summary>
        /// <param name="context"></param>
        void Resolve(IResolveContext context);

        /// <summary>
        /// Attempts to resolve the <see cref="IResolvable"/>.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        /// <returns>True, if the <see cref="IResolvable"/> was successfully resolved. Otherwise false.</returns>
        bool TryResolve(IResolveContext context, out PackReferenceException exception);
    }
}
