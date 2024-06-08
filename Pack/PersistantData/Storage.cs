using AchievementLib.Pack.PersistantData.SQLite;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
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

        private static void OnException(Exception ex)
        {
            ExceptionOccured?.Invoke(null, ex);
        }

        /// <summary>
        /// <inheritdoc cref="StoreProperty(SQLiteConnection, object, string)"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns><see langword="true"/>, if the property was successfully stored. Otherwise <see langword="false"/>.</returns>
        internal static bool TryStoreProperty(SQLiteConnection connection, object @object, string propertyName)
        {
            try
            {
                StoreProperty(connection, @object, propertyName);
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
        internal static bool TryStoreProperty(object @object, string propertyName)
        {
            return TryStoreProperty(null, @object, propertyName);
        }

        /// <summary>
        /// <inheritdoc cref="RetrieveProperty{T}(SQLiteConnection, object, string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <param name="result"></param>
        /// <returns><see langword="true"/>, if the property was successfully retrieved. Otherwise <see langword="false"/>.</returns>
        internal static bool TryRetrieveProperty<T>(SQLiteConnection connection, object @object, string propertyName, out T result)
        {
            try
            {
                result = RetrieveProperty<T>(connection, @object, propertyName);
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
            return TryRetrieveProperty<T>(null, @object, propertyName, out result);
        }

        /// <summary>
        /// Stores the given <paramref name="object"/> according to its <see cref="StoreAttribute"/> and 
        /// <see cref="StoragePropertyAttribute"/>s.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="object"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the insert command fails.</exception>
        internal static void Store(SQLiteConnection connection, object @object)
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

            SQLite.Table table = GetTable(storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            if (!table.Create(connection, true, out Exception createException))
            {
                throw new InvalidOperationException($"Unable to store object of type {@object.GetType()}. " +
                    $"Table creation failed.", createException);
            }

            List<(string ColumnName, object Value)> values = propertyAttributes.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value)).ToList();
            values.Add((VERSION_COLUMN, storeAttribute.Version));

            if (!table.InsertOrReplace(connection, values, out Exception insertException))
            {
                throw new InvalidOperationException($"Unable to store object of type {@object.GetType()}. " +
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
            Store(null, @object);
        }

        /// <summary>
        /// Stores the given <paramref name="object"/> according to its <see cref="StoreAttribute"/> and 
        /// <see cref="StoragePropertyAttribute"/>s. Will only update the property with the <paramref name="propertyName"/>, 
        /// if an entry for the <paramref name="object"/> already exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="object"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the creation of the <see cref="SQLite.Table"/> or 
        /// the upsert command fails.</exception>
        internal static void StoreProperty(SQLiteConnection connection, object @object, string propertyName)
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

            SQLite.Table table = GetTable(storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            if (!table.Create(connection, true, out Exception createException))
            {
                throw new InvalidOperationException($"Unable to insert or update property with name {propertyName} on object " +
                    $"of type {@object.GetType()}. Table creation failed.", createException);
            }

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value) propertyColumn = propertyAttributes.FirstOrDefault(attribute => attribute.Name == propertyName);

            if (propertyColumn == default)
            {
                throw new InvalidOperationException($"Unable to insert or update property with name {propertyName} on object " +
                    $"of type {@object.GetType()}. Property with that name has no {nameof(StoragePropertyAttribute)}.");
            }

            List<(string ColumnName, object Value)> values = propertyAttributes.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value)).ToList();
            values.Add((VERSION_COLUMN, storeAttribute.Version));
            IEnumerable<string> primaryKeyColumnNames = table.PrimaryKeyColumnNames;

            if (!table.InsertOrUpdate(connection, values, new string[] { propertyColumn.Attribute.ColumnName }, primaryKeyColumnNames, out Exception insertOrUpdateException))
            {
                throw new InvalidOperationException($"Unable to insert or update property with name {propertyName} on object " +
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
            StoreProperty(null, @object, propertyName);
        }

        /// <summary>
        /// Retrieves the value of the property with the <paramref name="propertyName"/> on the <paramref name="object"/> 
        /// from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns>The value of the property with the <paramref name="propertyName"/> on the <paramref name="object"/> 
        /// from the database.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static T RetrieveProperty<T>(SQLiteConnection connection, object @object, string propertyName)
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

            SQLite.Table table = GetTable(storeAttribute, propertyAttributes.Select(attribute => (attribute.Attribute, attribute.Type)));

            (string Name, StoragePropertyAttribute Attribute, Type Type, object Value) propertyColumn = propertyAttributes.FirstOrDefault(attribute => attribute.Name == propertyName);

            if (propertyColumn == default)
            {
                throw new InvalidOperationException($"Unable to retrieve property with name {propertyName} from object " +
                    $"of type {@object.GetType()}. Property with that name has no {nameof(StoragePropertyAttribute)}.");
            }

            IEnumerable<(string Name, StoragePropertyAttribute Attribute, Type Type, object Value)> primaryKeys = propertyAttributes.Where(attribute => attribute.Attribute.IsPrimaryKey);
            IEnumerable<(string ColumnName, object Value)> filters = primaryKeys.Select(attribute => (attribute.Attribute.ColumnName, attribute.Value));

            if (!table.Select(connection, true, propertyColumn.Attribute.ColumnName, filters, out object value, out Exception selectException))
            {
                throw new InvalidOperationException($"Unable to retrieve property with name {propertyName} from object " +
                    $"of type {@object.GetType()}. Select command failed.", selectException);
            }

            if (value == null)
            {
                throw new InvalidOperationException($"Unable to retrieve property with name {propertyName} from object " +
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        internal static T RetrieveProperty<T>(object @object, string propertyName)
        {
            return RetrieveProperty<T>(null, @object, propertyName);
        }

        private static SQLite.Table GetTable(StoreAttribute storeAttribute, IEnumerable<(StoragePropertyAttribute Attribute, Type Type)> fieldAttributes)
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
                    DBNull.Value,
                    false,
                    false,
                    true
                ));

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
