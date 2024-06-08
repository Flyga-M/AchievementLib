using System;

namespace AchievementLib.Pack.PersistantData
{
    /// <summary>
    /// An <see cref="Attribute"/> that is used on a property, if the property should be stored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StoragePropertyAttribute : Attribute
    {
        /// <summary>
        /// The column name where the property is saved to.
        /// </summary>
        public string ColumnName;

        /// <summary>
        /// The storage version.
        /// </summary>
        public int Version = 1;

        /// <summary>
        /// The default value for the column.
        /// </summary>
        public object Default;

        /// <summary>
        /// Determines whether the column is the primary key for the table.
        /// </summary>
        public bool IsPrimaryKey = false;

        /// <summary>
        /// Determines whether entries in the column have to be unique.
        /// </summary>
        public bool IsUnique = false;

        /// <summary>
        /// Determines whether an entry for the column may be <see langword="null"/>.
        /// </summary>
        public bool NotNull = false;
    }
}
