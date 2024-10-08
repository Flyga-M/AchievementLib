﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace AchievementLib.Pack
{
    /// <summary>
    /// Represents a class, that has different content depending on the locale of the user.
    /// </summary>
    public interface ILocalizable : IValidateable
    {
        /// <summary>
        /// Determines whether the <see cref="ILocalizable"/> has at least one entry.
        /// </summary>
        [JsonIgnore]
        bool AnyLocale { get; }

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

        /// <summary>
        /// Returns the localized version of the <see cref="ILocalizable"/> or the localized version of a similar 
        /// <see cref="ILocalizable"/> contained in <paramref name="references"/>, if the <see cref="ILocalizable"/> has 
        /// no entry for the <paramref name="locale"/>.
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="references"></param>
        /// <returns>The localized version of the <see cref="ILocalizable"/> or the 
        /// localized version of a similar <see cref="ILocalizable"/> contained in <paramref name="references"/>. If the 
        /// <see cref="ILocalizable"/> or the <paramref name="references"/> do not contain the given 
        /// <paramref name="locale"/>, it will return the localized version for any other locale. 
        /// If the <see cref="ILocalizable"/> does not contain an entry for any locale at 
        /// all, it will return <see cref="string.Empty"/>.</returns>
        string GetLocalized(string locale, IEnumerable<ILocalizable> references);

        /// <summary>
        /// Returns the localized version of the <see cref="ILocalizable"/> or the localized version of a similar 
        /// <see cref="ILocalizable"/> contained in <paramref name="references"/>, if the <see cref="ILocalizable"/> has 
        /// no entry for the <paramref name="locale"/>, or the 
        /// localized version for the <paramref name="fallbackLocale"/>, if the previous attempts yield no results. 
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="references"></param>
        /// <param name="fallbackLocale"></param>
        /// <returns>The localized version of the <see cref="ILocalizable"/> or the 
        /// localized version of a similar <see cref="ILocalizable"/> contained in <paramref name="references"/>, or the 
        /// localized version for the <paramref name="fallbackLocale"/>. If the 
        /// <see cref="ILocalizable"/> or the <paramref name="references"/> do not contain the given 
        /// <paramref name="locale"/> and the <see cref="ILocalizable"/> does not contain the <paramref name="fallbackLocale"/>, 
        /// it will return the localized version for any other locale. 
        /// If the <see cref="ILocalizable"/> does not contain an entry for any locale at 
        /// all, it will return <see cref="string.Empty"/>.</returns>
        string GetLocalized(string locale, IEnumerable<ILocalizable> references, string fallbackLocale);

        /// <summary>
        /// Returns all entries of the <see cref="ILocalizable"/>.
        /// </summary>
        /// <returns>All entries of the <see cref="ILocalizable"/>.</returns>
        Dictionary<string, string> GetAll();
    }
}
