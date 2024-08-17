using AchievementLib.Pack.PersistantData.SQLite;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AchievementLib.Pack.PersistantData
{
    /// <summary>
    /// Handles storing and retrieving of certain properties for Achievement Packs.
    /// </summary>
    public static class Storage
    {
        /// <summary>
        /// The column name that stores the <see cref="StoreAttribute.Version"/>.
        /// </summary>
        public const string VERSION_COLUMN = "_InternalStorageVersion";

        /// <summary>
        /// Fires, when an exception during storing or retrieving occures.
        /// </summary>
        public static event EventHandler<Exception> ExceptionOccured;

        /// <summary>
        /// The directory that is used by the default connection.
        /// </summary>
        public static string DefaultDirectory
        {
            get => ConnectionHandler.DefaultDirectory;
            set => ConnectionHandler.DefaultDirectory = value;
        }

        /// <summary>
        /// The filename that is used by the default connection.
        /// </summary>
        /// <remarks>
        /// Should NOT include the extension. Set <see cref="DefaultFileExtension"/> instead. 
        /// </remarks>
        public static string DefaultFileName
        {
            get => ConnectionHandler.DefaultFileName;
            set => ConnectionHandler.DefaultFileName = value;
        }

        /// <summary>
        /// The file extension that is used by the default connection.
        /// </summary>
        public static string DefaultFileExtension
        {
            get => ConnectionHandler.DefaultFileExtension;
            set => ConnectionHandler.DefaultFileExtension = value;
        }

        /// <summary>
        /// The full default file path (with file name and extension) that is used by the default connection.
        /// </summary>
        public static string DefaultPath
        {
            get
            {
                return Path.Combine(DefaultDirectory, DefaultFileName) + "." + DefaultFileExtension;
            }
            set
            {
                DefaultDirectory = Path.GetDirectoryName(value);
                DefaultFileName = Path.GetFileNameWithoutExtension(value);

                string extensionWithDot = Path.GetExtension(value);

                if (string.IsNullOrWhiteSpace(extensionWithDot))
                {
                    DefaultFileExtension = string.Empty;
                }
                else
                {
                    DefaultFileExtension = extensionWithDot.Substring(1);
                }
            }
        }

        private static void OnException(Exception ex)
        {
            ExceptionOccured?.Invoke(null, ex);
        }

        /// <summary>
        /// <inheritdoc cref="StoreProperty(SQLiteConnection, bool, object, string)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns><see langword="true"/>, if the property was successfully stored. Otherwise <see langword="false"/>.</returns>
        public static bool TryStoreProperty(SQLiteConnection connection, bool keepConnectionOpen, object @object, string propertyName)
        {
            try
            {
                StoreProperty(connection, keepConnectionOpen, @object, propertyName);
            }
            catch (Exception ex)
            {
                OnException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc cref="StoreProperty(object, string)"/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="StoreProperty(object, string)"/>
        /// </remarks>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns><see langword="true"/>, if the property was successfully stored. Otherwise <see langword="false"/>.</returns>
        public static bool TryStoreProperty(object @object, string propertyName)
        {
            return TryStoreProperty(null, false, @object, propertyName);
        }

        /// <summary>
        /// <inheritdoc cref="Retrieve(SQLiteConnection, bool, IRetrievable)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <param name="isStored"></param>
        /// <returns><see langword="true"/>, if the properties of the <paramref name="object"/> were successfully retrieved. 
        /// Otherwise <see langword="false"/>.</returns>
        public static bool TryRetrieve(SQLiteConnection connection, bool keepConnectionOpen, IRetrievable @object, out bool isStored)
        {
            isStored = false;

            try
            {
                isStored = Retrieve(connection, keepConnectionOpen, @object);
            }
            catch (Exception ex)
            {
                OnException(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc cref="Retrieve(IRetrievable)"/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="Retrieve(IRetrievable)"/>
        /// </remarks>
        /// <param name="object"></param>
        /// <param name="isStored"></param>
        /// <returns><see langword="true"/>, if the properties of the <paramref name="object"/> were successfully retrieved. 
        /// Otherwise <see langword="false"/>.</returns>
        public static bool TryRetrieve(IRetrievable @object, out bool isStored)
        {
            return TryRetrieve(null, false, @object, out isStored);
        }

        /// <summary>
        /// <inheritdoc cref="RetrieveProperty{T}(SQLiteConnection, bool, object, string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <param name="result"></param>
        /// <returns><see langword="true"/>, if the property was successfully retrieved. Otherwise <see langword="false"/>.</returns>
        internal static bool TryRetrieveProperty<T>(SQLiteConnection connection, bool keepConnectionOpen, object @object, string propertyName, out T result)
        {
            try
            {
                result = RetrieveProperty<T>(connection, keepConnectionOpen, @object, propertyName);
            }
            catch (Exception ex)
            {
                OnException(ex);
                result = default;
                return false;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc cref="RetrieveProperty{T}(object, string)"/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="RetrieveProperty{T}(object, string)"/>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <param name="result"></param>
        /// <returns><see langword="true"/>, if the property was successfully retrieved. Otherwise <see langword="false"/>.</returns>
        internal static bool TryRetrieveProperty<T>(object @object, string propertyName, out T result)
        {
            return TryRetrieveProperty<T>(null, false, @object, propertyName, out result);
        }

        /// <summary>
        /// <inheritdoc cref="IsStored(SQLiteConnection, bool, object)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <param name="isStored"></param>
        /// <returns><see langword="true"/>, if the exists query was successfull. Otherwise <see langword="false"/>.</returns>
        internal static bool TryIsStored(SQLiteConnection connection, bool keepConnectionOpen, object @object, out bool isStored)
        {
            try
            {
                isStored = IsStored(connection, keepConnectionOpen, @object);
            }
            catch (Exception ex)
            {
                OnException(ex);
                isStored = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc cref="IsStored(object)"/>
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="IsStored(object)"/>
        /// </remarks>
        /// <param name="object"></param>
        /// <param name="isStored"></param>
        /// <returns><see langword="true"/>, if the exists query was successfull. Otherwise <see langword="false"/>.</returns>
        internal static bool TryIsStored(object @object, out bool isStored)
        {
            return TryIsStored(null, false, @object, out isStored);
        }

        /// <summary>
        /// Stores the given <paramref name="object"/> according to its <see cref="StoreAttribute"/> and 
        /// <see cref="StoragePropertyAttribute"/>s.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the insert command fails.</exception>
        internal static void Store(SQLiteConnection connection, bool keepConnectionOpen, object @object)
        {   
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            StoreAttribute storeAttribute = AttributeUtil.GetAttribute<StoreAttribute>(@object);

            if (string.IsNullOrWhiteSpace(storeAttribute.TableName))
            {
                storeAttribute.TableName = @object.GetType().Namespace + "." + @object.GetType().Name;
            }

            storeAttribute.TableName = ConvertTableName(storeAttribute.TableName);

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value)[] propertyAttributes = AttributeUtil.GetPropertyAttributes<StoragePropertyAttribute>(@object);

            foreach (var attribute in propertyAttributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Attribute.ColumnName))
                {
                    attribute.Attribute.ColumnName = attribute.Name;
                }
            }

            SQLite.Table table = GetTable(connection, keepConnectionOpen, storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            List<(string ColumnName, object Value)> values = propertyAttributes.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value)).ToList();
            values.Add((VERSION_COLUMN, storeAttribute.Version));

            if (!table.InsertOrReplace(connection, keepConnectionOpen, values, out Exception insertException))
            {
                throw new InvalidOperationException($"Unable to store retrievable of type {@object.GetType()}. " +
                    $"Value insertion failed.", insertException);
            }
        }

        private static string ConvertTableName(string tableName)
        {
            return tableName.Replace('.', '_').Replace(' ', '_');
        }

        /// <summary>
        /// Stores the given <paramref name="object"/> according to its <see cref="StoreAttribute"/> and 
        /// <see cref="StoragePropertyAttribute"/>s.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="SQLite.ConnectionHandler.DefaultConnection"/>. Make sure to set the appropriate 
        /// parameters (<see cref="DefaultDirectory"/>, <see cref="DefaultFileName"/>, <see cref="DefaultFileExtension"/>) before using.
        /// </remarks>
        /// <param name="object"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the insert command fails.</exception>
        internal static void Store(object @object)
        {
            Store(null, false, @object);
        }

        /// <summary>
        /// Stores the given <paramref name="object"/> according to its <see cref="StoreAttribute"/> and 
        /// <see cref="StoragePropertyAttribute"/>s. Will only update the property with the <paramref name="propertyName"/>, 
        /// if an entry for the <paramref name="object"/> already exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the upsert command fails.</exception>
        internal static void StoreProperty(SQLiteConnection connection, bool keepConnectionOpen, object @object, string propertyName)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            StoreAttribute storeAttribute = AttributeUtil.GetAttribute<StoreAttribute>(@object);

            if (string.IsNullOrWhiteSpace(storeAttribute.TableName))
            {
                storeAttribute.TableName = @object.GetType().Namespace + "." + @object.GetType().Name;
            }

            storeAttribute.TableName = ConvertTableName(storeAttribute.TableName);

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value)[] propertyAttributes = AttributeUtil.GetPropertyAttributes<StoragePropertyAttribute>(@object);

            foreach (var attribute in propertyAttributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Attribute.ColumnName))
                {
                    attribute.Attribute.ColumnName = attribute.Name;
                }
            }

            SQLite.Table table = GetTable(connection, keepConnectionOpen, storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value) propertyColumn = propertyAttributes.FirstOrDefault(attribute => attribute.Name == propertyName);

            if (propertyColumn == default)
            {
                throw new InvalidOperationException($"Unable to insert or update property with name {propertyName} on retrievable " +
                    $"of type {@object.GetType()}. Property with that name has no {nameof(StoragePropertyAttribute)}.");
            }

            List<(string ColumnName, object Value)> values = propertyAttributes.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value)).ToList();
            values.Add((VERSION_COLUMN, storeAttribute.Version));
            IEnumerable<string> primaryKeyColumnNames = table.PrimaryKeyColumnNames;

            if (!table.InsertOrUpdate(connection, keepConnectionOpen, values, new string[] { propertyColumn.Attribute.ColumnName }, primaryKeyColumnNames, out Exception insertOrUpdateException))
            {
                throw new InvalidOperationException($"Unable to insert or update property with name {propertyName} on retrievable " +
                    $"of type {@object.GetType()}. Insert or Update command failed.", insertOrUpdateException);
            }
        }

        /// <summary>
        /// Stores the given <paramref name="object"/> according to its <see cref="StoreAttribute"/> and 
        /// <see cref="StoragePropertyAttribute"/>s. Will only update the property with the <paramref name="propertyName"/>, 
        /// if an entry for the <paramref name="object"/> already exists.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="SQLite.ConnectionHandler.DefaultConnection"/>. Make sure to set the appropriate 
        /// parameters (<see cref="DefaultDirectory"/>, <see cref="DefaultFileName"/>, <see cref="DefaultFileExtension"/>) before using.
        /// </remarks>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the upsert command fails.</exception>
        internal static void StoreProperty(object @object, string propertyName)
        {
            StoreProperty(null, false, @object, propertyName);
        }

        /// <summary>
        /// Retrieves the stored values and applies them to the <paramref name="retrievable"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="retrievable"></param>
        /// <returns><see langword="true"/>, if the <paramref name="retrievable"/> has an entry in the database. 
        /// Otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="retrievable"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the exists or select command fails. Also if the select command does not retrieve a value, or if at least 
        /// one property can't be set to it's saved value.</exception>
        internal static bool Retrieve(SQLiteConnection connection, bool keepConnectionOpen, IRetrievable retrievable)
        {
            if (retrievable == null)
            {
                throw new ArgumentNullException(nameof(retrievable));
            }

            retrievable.IsRetrieving = true;

            StoreAttribute storeAttribute = AttributeUtil.GetAttribute<StoreAttribute>(retrievable);

            if (string.IsNullOrWhiteSpace(storeAttribute.TableName))
            {
                storeAttribute.TableName = retrievable.GetType().Namespace + "." + retrievable.GetType().Name;
            }

            storeAttribute.TableName = ConvertTableName(storeAttribute.TableName);

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value)[] propertyAttributes = AttributeUtil.GetPropertyAttributes<StoragePropertyAttribute>(retrievable);

            foreach (var attribute in propertyAttributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Attribute.ColumnName))
                {
                    attribute.Attribute.ColumnName = attribute.Name;
                }
            }

            SQLite.Table table = GetTable(connection, keepConnectionOpen, storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            if (!TryIsStored(connection, keepConnectionOpen, retrievable, out bool isStored))
            {
                retrievable.IsRetrieving = false;
                throw new InvalidOperationException($"Unable to retrieve retrievable of type {retrievable.GetType()}. " +
                    $"Exists command failed.");
            }

            if (!isStored)
            {
                retrievable.IsRetrieving = false;
                return false;
            }

            IEnumerable<(string Name, StoragePropertyAttribute Attribute, Type Type, object Value)> doRetrieve = propertyAttributes.Where(attribute => !attribute.Attribute.DoNotRetrieve);
            List<string> columnNames = doRetrieve.Select(attribute => attribute.Attribute.ColumnName).ToList();
            // not currently used. Will only be used, if the store version changes in the future.
            columnNames.Add(VERSION_COLUMN);

            IEnumerable<(string Name, StoragePropertyAttribute Attribute, Type Type, object Value)> primaryKeys = propertyAttributes.Where(attribute => attribute.Attribute.IsPrimaryKey);
            IEnumerable<(string ColumnName, object Value)> filters = primaryKeys.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value));

            if (!table.Select(connection, keepConnectionOpen, true, columnNames, filters, 1, out SQLite.Row[] result, out Exception selectException))
            {
                retrievable.IsRetrieving = false;
                throw new InvalidOperationException($"Unable to retrieve retrievable of type {retrievable.GetType()}. " +
                    $"Select command failed.", selectException);
            }

            if (!result.Any())
            {
                retrievable.IsRetrieving = false;
                throw new InvalidOperationException($"Unable to retrieve retrievable of type {retrievable.GetType()}. " +
                    $"Select command did not return any value.", selectException);
            }

            SQLite.Row retrievedValues = result.First();

            try
            {
                foreach ((string Name, StoragePropertyAttribute Attribute, Type Type, object Value) attribute in doRetrieve)
                {
                    AttributeUtil.SetPropertyValue(retrievable, attribute.Name, attribute.Type, retrievedValues[attribute.Attribute.ColumnName]);
                }
            }
            catch (Exception ex)
            {
                retrievable.IsRetrieving = false;
                throw new InvalidOperationException($"Unable to retrieve retrievable of type {retrievable.GetType()}. " +
                    $"Unable to set property values.", ex);
            }

            retrievable.IsRetrieving = false;
            return true;
        }

        /// <summary>
        /// Retrieves the stored values and applies them to the <paramref name="object"/>.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="SQLite.ConnectionHandler.DefaultConnection"/>. Make sure to set the appropriate 
        /// parameters (<see cref="DefaultDirectory"/>, <see cref="DefaultFileName"/>, <see cref="DefaultFileExtension"/>) before using.
        /// </remarks>
        /// <param name="object"></param>
        /// <returns><see langword="true"/>, if the <paramref name="object"/> has an entry in the database. 
        /// Otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the exists or select command fails. Also if the select command does not retrieve a value, or if at least 
        /// one property can't be set to it's saved value.</exception>
        internal static bool Retrieve(IRetrievable @object)
        {
            return Retrieve(null, false, @object);
        }

        /// <summary>
        /// Retrieves the value of the property with the <paramref name="propertyName"/> on the <paramref name="object"/> 
        /// from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns>The value of the property with the <paramref name="propertyName"/> on the <paramref name="object"/> 
        /// from the database.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the select command fails or returns <see langword="null"/>. Also if the property with the <paramref name="propertyName"/> does not have 
        /// the <see cref="StoragePropertyAttribute"/>.</exception>
        internal static T RetrieveProperty<T>(SQLiteConnection connection, bool keepConnectionOpen, object @object, string propertyName)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            StoreAttribute storeAttribute = AttributeUtil.GetAttribute<StoreAttribute>(@object);

            if (string.IsNullOrWhiteSpace(storeAttribute.TableName))
            {
                storeAttribute.TableName = @object.GetType().Namespace + "." + @object.GetType().Name;
            }

            storeAttribute.TableName = ConvertTableName(storeAttribute.TableName);

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value)[] propertyAttributes = AttributeUtil.GetPropertyAttributes<StoragePropertyAttribute>(@object);

            foreach (var attribute in propertyAttributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Attribute.ColumnName))
                {
                    attribute.Attribute.ColumnName = attribute.Name;
                }
            }

            SQLite.Table table = GetTable(connection, keepConnectionOpen, storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value) propertyColumn = propertyAttributes.FirstOrDefault(attribute => attribute.Name == propertyName);

            if (propertyColumn == default)
            {
                throw new InvalidOperationException($"Unable to retrieve property with name {propertyName} from retrievable " +
                    $"of type {@object.GetType()}. Property with that name has no {nameof(StoragePropertyAttribute)}.");
            }

            IEnumerable<(string Name, StoragePropertyAttribute Attribute, Type Type, object Value)> primaryKeys = propertyAttributes.Where(attribute => attribute.Attribute.IsPrimaryKey);
            IEnumerable<(string ColumnName, object Value)> filters = primaryKeys.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value));

            if (!table.Select(connection, keepConnectionOpen, true, propertyColumn.Attribute.ColumnName, filters, out object value, out Exception selectException))
            {
                throw new InvalidOperationException($"Unable to retrieve property with name {propertyName} from retrievable " +
                    $"of type {@object.GetType()}. Select command failed.", selectException);
            }

            if (value == null)
            {
                throw new InvalidOperationException($"Unable to retrieve property with name {propertyName} from retrievable " +
                    $"of type {@object.GetType()}. Select command did not retrieve any value.");
            }

            // TODO: will most certainly not work for non-primitive objects.
            // For the current use case this is fine though.
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Retrieves the value of the property with the <paramref name="propertyName"/> on the <paramref name="object"/> 
        /// from the database.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="SQLite.ConnectionHandler.DefaultConnection"/>. Make sure to set the appropriate 
        /// parameters (<see cref="DefaultDirectory"/>, <see cref="DefaultFileName"/>, <see cref="DefaultFileExtension"/>) before using.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns>The value of the property with the <paramref name="propertyName"/> on the <paramref name="object"/> 
        /// from the database.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the select command fails or returns <see langword="null"/>. Also if the property with the <paramref name="propertyName"/> does not have 
        /// the <see cref="StoragePropertyAttribute"/>.</exception>
        internal static T RetrieveProperty<T>(object @object, string propertyName)
        {
            return RetrieveProperty<T>(null, false, @object, propertyName);
        }

        /// <summary>
        /// Determines whether an entry for the given <paramref name="object"/> is stored.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="keepConnectionOpen"></param>
        /// <param name="object"></param>
        /// <returns><see langword="true"/>, if an entry for the given <paramref name="object"/> exists. 
        /// Otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the exists command fails.</exception>
        internal static bool IsStored(SQLiteConnection connection, bool keepConnectionOpen, object @object)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object));
            }

            StoreAttribute storeAttribute = AttributeUtil.GetAttribute<StoreAttribute>(@object);

            if (string.IsNullOrWhiteSpace(storeAttribute.TableName))
            {
                storeAttribute.TableName = @object.GetType().Namespace + "." + @object.GetType().Name;
            }

            storeAttribute.TableName = ConvertTableName(storeAttribute.TableName);

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value)[] propertyAttributes = AttributeUtil.GetPropertyAttributes<StoragePropertyAttribute>(@object);

            foreach (var attribute in propertyAttributes)
            {
                if (string.IsNullOrWhiteSpace(attribute.Attribute.ColumnName))
                {
                    attribute.Attribute.ColumnName = attribute.Name;
                }
            }

            SQLite.Table table = GetTable(connection, keepConnectionOpen, storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            IEnumerable<(string Name, StoragePropertyAttribute Attribute, Type Type, object Value)> primaryKeys = propertyAttributes.Where(attribute => attribute.Attribute.IsPrimaryKey);
            IEnumerable<(string ColumnName, object Value)> filters = primaryKeys.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value));

            if (!table.Exists(connection, keepConnectionOpen, filters, out bool exists, out Exception existsException))
            {
                throw new InvalidOperationException($"Unable to determine whether retrievable of type {@object.GetType()} with " +
                    $"primary keys {{ {string.Join(", ", primaryKeys.Select(key => $"{key.Name}: {key.Value}"))} }} " +
                    $"exists. Exists command failed.", existsException);
            }

            return exists;
        }

        /// <summary>
        /// Determines whether an entry for the given <paramref name="object"/> is stored.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="SQLite.ConnectionHandler.DefaultConnection"/>. Make sure to set the appropriate 
        /// parameters (<see cref="DefaultDirectory"/>, <see cref="DefaultFileName"/>, <see cref="DefaultFileExtension"/>) before using.
        /// </remarks>
        /// <param name="object"></param>
        /// <returns><see langword="true"/>, if an entry for the given <paramref name="object"/> exists. 
        /// Otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the exists command fails.</exception>
        internal static bool IsStored(object @object)
        {
            return IsStored(null, false, @object);
        }

        /// <exception cref="InvalidOperationException"></exception>
        private static SQLite.Table GetTable(SQLiteConnection connection, bool keepConnectionOpen, StoreAttribute storeAttribute, IEnumerable<(StoragePropertyAttribute Attribute, Type Type)> fieldAttributes)
        {
            SQLite.Table table = new SQLite.Table(storeAttribute.TableName);

            foreach ((StoragePropertyAttribute Attribute, Type Type) field in fieldAttributes)
            {
                if (!table.TryAddColumn(new SQLite.Column(
                                            field.Attribute.ColumnName,
                                            SQLiteUtil.GetType(field.Type),
                                            field.Attribute.Default,
                                            field.Attribute.IsPrimaryKey,
                                            field.Attribute.IsUnique,
                                            field.Attribute.NotNull)))
                {
                    throw new InvalidOperationException($"Unable to add column with name {field.Attribute.ColumnName}. A column " +
                        $"with that name already exists.");
                }
            }

            table.TryAddColumn(new SQLite.Column(
                    VERSION_COLUMN,
                    DataType.INTEGER,
                    null,
                    false,
                    false,
                    true
                ));

            if (!table.Create(connection, keepConnectionOpen, true, out Exception createException))
            {
                throw new InvalidOperationException($"Unable to create table {table.Name} at {connection?.FileName}. " +
                    $"Table creation failed.", createException);
            }

            return table;
        }

        /// <summary>
        /// Clears all subscribers to the static events.
        /// </summary>
        public static void ClearEvents()
        {
            ExceptionOccured = null;
        }
    }
}
