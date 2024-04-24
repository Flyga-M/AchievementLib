namespace AchievementLib.Pack
{
    public interface ILocalizable
    {
        /// <summary>
        /// Determines whether the <see cref="ILocalizable"/> has an entry for the 
        /// <paramref name="locale"/>.
        /// </summary>
        /// <param name="locale"></param>
        /// <returns>True, if the <see cref="ILocalizable"/> has an entry for the 
        /// <paramref name="locale"/>. Otherwise false.</returns>
        bool HasLocale(string locale);

        /// <summary>
        /// Returns the localized version of the <see cref="ILocalizable"/>.
        /// </summary>
        /// <param name="locale"></param>
        /// <returns>The localized version in the given <paramref name="locale"/> of the 
        /// <see cref="ILocalizable"/>, or in any other locale if the <see cref="ILocalizable"/> 
        /// has no entry for the <paramref name="locale"/>, or <see cref="string.Empty"/>, 
        /// if the <see cref="ILocalizable"/> does not contain any entries.</returns>
        string GetLocalized(string locale);

        /// <summary>
        /// Returns the localized version of the <see cref="ILocalizable"/> or the 
        /// localized version for the <paramref name="fallbackLocale"/>, if the 
        /// <see cref="ILocalizable"/> has no entry for the <paramref name="locale"/>. 
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="fallbackLocale"></param>
        /// <returns>The localized version of the <see cref="ILocalizable"/> or the 
        /// localized version for the <paramref name="fallbackLocale"/>. If the 
        /// <see cref="ILocalizable"/> does not contain any of the given 
        /// locales, it will return the localized version for any other locale. 
        /// If the <see cref="ILocalizable"/> does not contain an entry for any locale at 
        /// all, it will return <see cref="string.Empty"/>.</returns>
        string GetLocalized(string locale, string fallbackLocale);
    }
}
