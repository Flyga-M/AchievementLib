using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace AchievementLib.Pack.PersistantData.SQLite
{
    /// <summary>
    /// A column in a SQLite <see cref="Table"/>.
    /// </summary>
    public class Column
    {
        private static Random _random = new Random();
        
        /// <summary>
        /// The suffix that is used for the <see cref="Default"/> value placeholder in <see cref="GetString(bool, out ValueTuple{string, object}[])"/>.
        /// </summary>
        public const string DEFAULT_SUFFIX = "_default";

        /// <summary>
        /// The prefix that is used for the <see cref="Column"/> <see cref="Name"/> placeholder 
        /// in <see cref="GetString(out ValueTuple{string, object}[])"/>.
        /// </summary>
        public const string DEFAULT_PREFIX_RANDOM = "@column_";

        /// <summary>
        /// The <see cref="Column"/> name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of data the <see cref="Column"/> holds.
        /// </summary>
        public DataType Type { get; set; }

        /// <summary>
        /// The default value for that <see cref="Column"/>.
        /// </summary>
        public object Default {  get; set; }

        /// <summary>
        /// Determines whether this <see cref="Column"/> is the primary key for the <see cref="Table"/>.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Determines whether entries in this <see cref="Column"/> must be unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Determines whether an entry in this <see cref="Column"/> may be <see cref="DBNull.Value"/>.
        /// </summary>
        public bool NotNull { get; set; }

        /// <summary>
        /// Determines whether the <see cref="Column"/> has a default value.
        /// </summary>
        public bool HasDefault => Default != null;

        /// <inheritdoc/>
        public Column(string name, DataType type, object @default, bool isPrimaryKey, bool isUnique, bool notNull)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} can't be empty or whitespace.", nameof(name));
            }

            Name = name;
            Type = type;
            Default = @default;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUnique;
            NotNull = notNull;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Column"/> to be used in <see cref="SQLiteCommand"/>s.
        /// </summary>
        /// <param name="omitPrimaryKey"></param>
        /// <param name="parameters"></param>
        /// <returns>A string representation of the <see cref="Column"/> to be used in <see cref="SQLiteCommand"/>s.</returns>
        public string GetString(bool omitPrimaryKey, out (string Placeholder, object Value)[] parameters)
        {
            List<(string Placeholder, object Value)> @params = new List<(string Placeholder, object Value)>();
            
            string result = Name;
            result += " " + Type.ToString();

            if (HasDefault)
            {
                string defaultPlaceholder = GetRandomPlaceholder() + DEFAULT_SUFFIX;

                result += " DEFAULT " + defaultPlaceholder;

                @params.Add((defaultPlaceholder, Default));
            }

            if (IsPrimaryKey && !omitPrimaryKey)
            {
                result += " PRIMARY KEY";
            }

            if (IsUnique)
            {
                result += " UNIQUE";
            }

            if (NotNull)
            {
                result += " NOT NULL";
            }

            parameters = @params.ToArray();
            return result;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Column"/> to be used in <see cref="SQLiteCommand"/>s.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>A string representation of the <see cref="Column"/> to be used in <see cref="SQLiteCommand"/>s.</returns>
        public string GetString(out (string Placeholder, object Value)[] parameters)
        {
            return GetString(false, out parameters);
        }

        private string GetRandomPlaceholder()
        {
            return DEFAULT_PREFIX_RANDOM + _random.Next().ToString();
        }
    }
}
