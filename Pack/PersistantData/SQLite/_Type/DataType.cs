namespace AchievementLib.Pack.PersistantData.SQLite
{
    /// <summary>
    /// The data types for columns in SQLite.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Integer values are whole numbers (either positive or negative). An integer can have variable sizes such as 1, 2,3, 4, or 8 bytes.
        /// </summary>
        INTEGER,
        /// <summary>
        /// Real values are real numbers with decimal values that use 8-byte floats.
        /// </summary>
        REAL,
        /// <summary>
        /// TEXT is used to store character data. The maximum length of TEXT is unlimited. SQLite supports various character encodings.
        /// </summary>
        TEXT,
        /// <summary>
        /// BLOB stands for a binary large object that can store any kind of data. Theoretically, the maximum size of BLOB is unlimited.
        /// </summary>
        BLOB
    }
}
