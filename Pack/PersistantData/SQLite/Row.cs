using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace AchievementLib.Pack.PersistantData.SQLite
{
    /// <summary>
    /// A row of data from a SQLite <see cref="Table"/>.
    /// </summary>
    public class Row : IEnumerable<(string ColumnName, object Value)>
    {
        private readonly Dictionary<string, object> _entries;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entries"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="entries"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="entries"/> is empty.</exception>
        public Row(IEnumerable<(string ColumnName, object Value)> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            if (!entries.Any())
            {
                throw new ArgumentException($"{nameof(entries)} must have at least one entry.", nameof(entries));
            }

            _entries = entries.ToDictionary(entry => entry.ColumnName, entry => entry.Value);
        }

        /// <summary>
        /// Creates a <see cref="Row"/> from the current data in the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnNames"></param>
        /// <returns>The created <see cref="Row"/>.</returns>
        public static Row FromReader(SQLiteDataReader reader, IEnumerable<string> columnNames)
        {
            List<(string ColumnName, object Value)> entries = new List<(string ColumnName, object Value)>();

            foreach(string columnName in columnNames)
            {
                entries.Add((columnName, reader[columnName]));
            }

            return new Row(entries);
        }

        /// <inheritdoc/>
        public IEnumerator<(string ColumnName, object Value)> GetEnumerator()
        {
            foreach(KeyValuePair<string, object> entry in _entries)
            {
                yield return (entry.Key, entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the value for the given <paramref name="columnName"/>, or <see langword="null"/> if no value for such a 
        /// column exists.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>The value for the given <paramref name="columnName"/>, or <see langword="null"/> if no value for such a 
        /// column exists.</returns>
        public object this[string columnName]
        {
            get
            {
                if (!_entries.ContainsKey(columnName))
                {
                    return null;
                }
                
                return _entries[columnName];
            }
        }
    }
}
