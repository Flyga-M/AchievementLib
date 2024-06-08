using System;

namespace AchievementLib.Pack.PersistantData
{
    /// <summary>
    /// An <see cref="Attribute"/> that is used on a class, if the class should be stored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class StoreAttribute : Attribute
    {
        /// <summary>
        /// The table name where the object is saved to.
        /// </summary>
        public string TableName;

        /// <summary>
        /// The storage version. Will be incremented, if a new column is added a later point in time.
        /// </summary>
        public int Version = 1;
    }
}
