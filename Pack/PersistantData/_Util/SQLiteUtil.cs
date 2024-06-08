using System;

namespace AchievementLib.Pack.PersistantData
{
    /// <summary>
    /// Provides utility functions for SQLite.
    /// </summary>
    public static class SQLiteUtil
    {
        /// <summary>
        /// Returns the corresponding <see cref="SQLite.DataType"/> for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The corresponding <see cref="SQLite.DataType"/> for the given <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
        public static SQLite.DataType GetType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(int) || type == typeof(long) || type == typeof(bool))
            {
                return SQLite.DataType.INTEGER;
            }

            if (type == typeof(float) || type == typeof(double))
            {
                return SQLite.DataType.REAL;
            }

            if (type == typeof(string) || type == typeof(char))
            {
                return SQLite.DataType.TEXT;
            }

            return SQLite.DataType.BLOB;
        }
    }
}
