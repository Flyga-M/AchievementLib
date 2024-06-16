using SharpDX;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace AchievementLib.Pack.PersistantData.SQLite
{
    /// <summary>
    /// A table in a SQLite database.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// The prefix for value parameterization.
        /// </summary>
        public const string VALUE_PLACEHOLDER = "@value";

        private List<Column> _columns = new List<Column>();
        
        /// <summary>
        /// The <see cref="Table"/> name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="Column"/>s of the <see cref="Table"/>.
        /// </summary>
        public Column[] Columns => _columns.ToArray();

        /// <summary>
        /// Determines whether the table has a primary key.
        /// </summary>
        public bool HasPrimaryKey => _columns.Any(column => column.IsPrimaryKey);

        /// <summary>
        /// The indices of the primary keys.
        /// </summary>
        public int[] PrimaryKeyColumns
        {
            get
            {
                if (!HasPrimaryKey)
                {
                    return Array.Empty<int>();
                }

                List<int> columnIndices = new List<int>();

                Column[] columns = Columns;

                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].IsPrimaryKey)
                    {
                        columnIndices.Add(i);
                    }
                }

                return columnIndices.ToArray();
            }
        }

        /// <summary>
        /// The names of the <see cref="Column"/>s with the primary key attribute.
        /// </summary>
        public string[] PrimaryKeyColumnNames
        {
            get
            {
                if (!HasPrimaryKey)
                {
                    return Array.Empty<string>();
                }
                Column[] columns = Columns;

                return columns.Where(column => column.IsPrimaryKey).Select(column => column.Name).ToArray();
            }
        }

        /// <inheritdoc/>
        public Table(string name, IEnumerable<Column> columns = null)
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

            if (columns != null)
            {
                _columns.AddRange(columns);
            }
        }

        /// <summary>
        /// Attempts to add a <paramref name="column"/> to the <see cref="Table"/>.
        /// </summary>
        /// <param name="column"></param>
        /// <returns><see langword="true"/>, if the <paramref name="column"/> is not already contained in the 
        /// <see cref="Table"/>. Otherwise <see langword="false"/>.</returns>
        public bool TryAddColumn(Column column)
        {
            if (_columns.Contains(column))
            {
                return false;
            }

            _columns.Add(column);
            return true;
        }

        /// <summary>
        /// Creates the <see cref="Table"/> with the provided <paramref name="connection"/>.
        /// </summary>
        /// <remarks>
        /// Will NOT dispose the <paramref name="connection"/>.
        /// </remarks>
        /// <param name="connection"></param>
        /// <param name="ifNotExists"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the creation did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Create(SQLiteConnection connection, bool ifNotExists, out Exception exception)
        {
            string commandText = GetCreateString(ifNotExists);

            return ExecuteCommand(connection, commandText, Array.Empty<(string, object)>(), out exception);
        }
        
        /// <summary>
        /// Creates the <see cref="Table"/> with the provided <paramref name="connection"/>, if it does not already exist.
        /// </summary>
        /// <remarks>
        /// Will NOT dispose the <paramref name="connection"/>.
        /// </remarks>
        /// <param name="connection"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the creation did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Create(SQLiteConnection connection, out Exception exception)
        {
            return Create(connection, true, out exception);
        }

        /// <summary>
        /// Creates the <see cref="Table"/> with the <see cref="ConnectionHandler.DefaultConnection"/>, if it does 
        /// not already exist.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the creation did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Create(out Exception exception)
        {
            return Create(null, out exception);
        }

        private string GetCreateString(bool ifNotExists)
        {
            Column[] columns = Columns;
            string[] primaryKeyColumnNames = PrimaryKeyColumnNames;
            bool multiplePrimaryKeys = primaryKeyColumnNames.Length > 1;

            if (columns == null || !columns.Any())
            {
                throw new InvalidOperationException($"Unable to build Create command. {nameof(Columns)} is null or empty.");
            }

            string result = "CREATE TABLE";

            if (ifNotExists)
            {
                result += " IF NOT EXISTS";
            }

            // table and column names can't be parameterized. So they need to directly put into the string.
            result += " " + Name;

            result += " (";

            for(int i = 0; i < columns.Length; i++)
            {
                result += " " + columns[i].GetString(multiplePrimaryKeys);

                if (i != columns.Length - 1 || multiplePrimaryKeys)
                {
                    result += ",";
                }
            }

            if (multiplePrimaryKeys)
            {
                result += " PRIMARY KEY ( " + string.Join(", ", primaryKeyColumnNames)  + " )";
            }

            result += " );";

            return result;
        }

        /// <summary>
        /// Inserts the given <paramref name="values"/> into the <see cref="Table"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="values"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the insertion did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Insert(SQLiteConnection connection, IEnumerable<(string ColumnName, object Value)> values, out Exception exception)
        {
            string commandText = GetInsertString(false, values, out (string Placeholder, object Value)[] parameters);

            return ExecuteCommand(connection, commandText, parameters, out exception);
        }

        /// <summary>
        /// Inserts the given <paramref name="values"/> into the <see cref="Table"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the insertion did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Insert(IEnumerable<(string ColumnName, object Value)> values, out Exception exception)
        {
            return Insert(null, values, out exception);
        }

        /// <summary>
        /// Inserts or replaces the given <paramref name="values"/> into the <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        /// Will delete the row, if it exists, first and then insert the new row. Values are not preserved.
        /// </remarks>
        /// <param name="connection"></param>
        /// <param name="values"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the insertion did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool InsertOrReplace(SQLiteConnection connection, IEnumerable<(string ColumnName, object Value)> values, out Exception exception)
        {
            string commandText = GetInsertString(true, values, out (string Placeholder, object Value)[] parameters);

            return ExecuteCommand(connection, commandText, parameters, out exception);
        }

        /// <summary>
        /// Inserts or replaces the given <paramref name="values"/> into the <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        /// Will delete the row, if it exists, first and then insert the new row. Values are not preserved.
        /// </remarks>
        /// <param name="values"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the insertion did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool InsertOrReplace(IEnumerable<(string ColumnName, object Value)> values, out Exception exception)
        {
            return InsertOrReplace(null, values, out exception);
        }

        /// <summary>
        /// Inserts or updates the given <paramref name="values"/> into the <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        /// If an entry with the 
        /// given <paramref name="values"/> for the <paramref name="primaryKeyColumnNames"/> already exists, 
        /// only the <paramref name="values"/> for the columns in <paramref name="updateColumnNames"/> are updated.
        /// </remarks>
        /// <param name="connection"></param>
        /// <param name="values"></param>
        /// <param name="updateColumnNames"></param>
        /// <param name="primaryKeyColumnNames"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the insertion (or update) did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool InsertOrUpdate(SQLiteConnection connection, IEnumerable<(string ColumnName, object Value)> values, IEnumerable<string> updateColumnNames, IEnumerable<string> primaryKeyColumnNames, out Exception exception)
        {
            string commandText = GetInsertOrUpdateString(values, updateColumnNames, primaryKeyColumnNames, out (string Placeholder, object Value)[] parameters);

            return ExecuteCommand(connection, commandText, parameters, out exception);
        }

        /// <summary>
        /// Inserts or updates the given <paramref name="values"/> into the <see cref="Table"/>.
        /// </summary>
        /// <remarks>
        /// If an entry with the 
        /// given <paramref name="values"/> for the <paramref name="primaryKeyColumnNames"/> already exists, 
        /// only the <paramref name="values"/> for the columns in <paramref name="updateColumnNames"/> are updated.
        /// </remarks>
        /// <param name="values"></param>
        /// <param name="updateColumnNames"></param>
        /// <param name="primaryKeyColumnNames"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the insertion (or update) did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool InsertOrUpdate(IEnumerable<(string ColumnName, object Value)> values, IEnumerable<string> updateColumnNames, IEnumerable<string> primaryKeyColumnNames, out Exception exception)
        {
            return InsertOrUpdate(null, values, updateColumnNames, primaryKeyColumnNames, out exception);
        }

        private bool ExecuteCommand(SQLiteConnection connection, string commandText, (string Placeholder, object Value)[] parameters, out Exception exception)
        {
            exception = null;
            bool disposeConnection = connection == null;

            if (connection == null)
            {
                try
                {
                    connection = ConnectionHandler.DefaultConnection;
                }
                catch (Exception ex)
                {
                    exception = new InvalidOperationException($"Default connection has invalid default values " +
                        $"(dir: {ConnectionHandler.DefaultDirectory}, filename: {ConnectionHandler.DefaultFileName}, ext: " +
                        $"{ConnectionHandler.DefaultFileExtension}). Set " +
                        $"{nameof(ConnectionHandler.DefaultConnection)} properly, or provide a {nameof(SQLiteConnection)}.", ex);
                    return false;
                }
            }

            try
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                {
                    command.Parameters.AddRange(parameters.Select(parameter => new SQLiteParameter(parameter.Placeholder, parameter.Value)).ToArray());

                    int result = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                connection.Close();
                if (disposeConnection)
                {
                    connection.Dispose();
                }
            }

            return exception == null;
        }

        private bool ExecuteCommandWithResult(SQLiteConnection connection, string commandText, IEnumerable<string> columnNames, (string Placeholder, object Value)[] parameters, out Row[] result, out Exception exception)
        {
            exception = null;
            result = Array.Empty<Row>();
            bool disposeConnection = connection == null;

            if (connection == null)
            {
                try
                {
                    connection = ConnectionHandler.DefaultConnection;
                }
                catch (Exception ex)
                {
                    exception = new InvalidOperationException($"Default connection has invalid default values " +
                        $"(dir: {ConnectionHandler.DefaultDirectory}, filename: {ConnectionHandler.DefaultFileName}, ext: " +
                        $"{ConnectionHandler.DefaultFileExtension}). Set " +
                        $"{nameof(ConnectionHandler.DefaultConnection)} properly, or provide a {nameof(SQLiteConnection)}.", ex);
                    return false;
                }
            }

            List<Row> rows = new List<Row>();

            try
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                {
                    command.Parameters.AddRange(parameters.Select(parameter => new SQLiteParameter(parameter.Placeholder, parameter.Value)).ToArray());

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(Row.FromReader(reader, columnNames));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                connection.Close();
                if (disposeConnection)
                {
                    connection.Dispose();
                }
            }

            result = rows.ToArray();
            return exception == null;
        }

        private bool ExecuteScalar(SQLiteConnection connection, string commandText, (string Placeholder, object Value)[] parameters, out object result, out Exception exception)
        {
            exception = null;
            result = null;
            bool disposeConnection = connection == null;

            if (connection == null)
            {
                try
                {
                    connection = ConnectionHandler.DefaultConnection;
                }
                catch (Exception ex)
                {
                    exception = new InvalidOperationException($"Default connection has invalid default values " +
                        $"(dir: {ConnectionHandler.DefaultDirectory}, filename: {ConnectionHandler.DefaultFileName}, ext: " +
                        $"{ConnectionHandler.DefaultFileExtension}). Set " +
                        $"{nameof(ConnectionHandler.DefaultConnection)} properly, or provide a {nameof(SQLiteConnection)}.", ex);
                    return false;
                }
            }

            try
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                {
                    command.Parameters.AddRange(parameters.Select(parameter => new SQLiteParameter(parameter.Placeholder, parameter.Value)).ToArray());

                    result = command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                connection.Close();
                if (disposeConnection)
                {
                    connection.Dispose();
                }
            }

            return exception == null;
        }

        private string GetInsertString(bool withOrReplace, IEnumerable<(string ColumnName, object Value)> values, out (string Placeholder, object Value)[] parameters)
        {
            List<(string Placeholder, object Value)> @params = new List<(string Placeholder, object Value)>();

            Column[] columns = Columns;
            IEnumerable<string> columnNames = columns.Select(column => column.Name);
            
            if (columns == null || !columns.Any())
            {
                throw new InvalidOperationException($"Unable to build Insert command. {nameof(Columns)} is null or empty.");
            }

            string result = "INSERT";

            if (withOrReplace)
            {
                result += " OR REPLACE";
            }

            result += " INTO";

            // table and column names can't be parameterized. So they need to directly put into the string.
            result += " " + Name;

            result += " (";

            string valueString = "VALUES (";

            for (int i = 0; i < values.Count(); i++)
            {
                string valuePlaceholder = GetValuePlaceholder(i);

                (string ColumnName, object Value) value = values.ElementAt(i);

                if (!columnNames.Contains(value.ColumnName))
                {
                    throw new InvalidOperationException($"Unable to build Insert command. {nameof(Columns)} does not contain any " +
                    $"column with the name {value.ColumnName}.");
                }

                // table and column names can't be parameterized. So they need to directly put into the string.
                result += " " + value.ColumnName;
                valueString += " " + valuePlaceholder;

                if (i != columns.Length - 1)
                {
                    result += ",";
                    valueString += ",";
                }

                @params.Add((valuePlaceholder, value.Value));
            }
            result += " )";
            valueString += " )";

            result += " " + valueString + ";";

            parameters = @params.ToArray();
            return result;
        }

        private string GetInsertOrUpdateString(IEnumerable<(string ColumnName, object Value)> values, IEnumerable<string> updateColumnNames, IEnumerable<string> primaryKeyColumnNames, out (string Placeholder, object Value)[] parameters)
        {
            string insertString = GetInsertString(false, values, out parameters);

            // removes ;
            string result = insertString.Remove(insertString.Length - 1);

            result += $" ON CONFLICT ({string.Join(", ", primaryKeyColumnNames)}) DO UPDATE SET " +
                $"{string.Join(", ", updateColumnNames.Select(columnName => $"{columnName}=excluded.{columnName}"))};";

            return result;
        }

        private Column GetColumn(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException($"{nameof(columnName)} can't be empty or whitespace.", nameof(columnName));
            }

            Column result = _columns.Where(column => column.Name == columnName).FirstOrDefault();

            if (result == null)
            {
                throw new ArgumentOutOfRangeException(nameof(columnName), $"No column with the {nameof(columnName)} exists " +
                    $"in the table.");
            }

            return result;
        }

        private string GetValuePlaceholder(int index)
        {
            return VALUE_PLACEHOLDER + "_" + index.ToString();
        }

        /// <summary>
        /// Selects the given <paramref name="columnNames"/> from the <see cref="Table"/> after the <paramref name="filters"/> 
        /// are applied. Limits the <paramref name="result"/>, if <paramref name="limit"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="distinct"></param>
        /// <param name="columnNames"></param>
        /// <param name="filters"></param>
        /// <param name="limit"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the selection did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Select(SQLiteConnection connection, bool distinct, IEnumerable<string> columnNames, IEnumerable<(string ColumnName, object Value)> filters, int? limit, out Row[] result, out Exception exception)
        {
            string commandText = GetSelectString(distinct, columnNames, filters, limit, out (string Placeholder, object Value)[] parameters);

            return ExecuteCommandWithResult(connection, commandText, columnNames, parameters, out result, out exception);
        }

        /// <summary>
        /// Selects the given <paramref name="columnNames"/> from the <see cref="Table"/> after the <paramref name="filters"/> 
        /// are applied. Limits the <paramref name="result"/>, if <paramref name="limit"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="distinct"></param>
        /// <param name="columnNames"></param>
        /// <param name="filters"></param>
        /// <param name="limit"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the selection did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Select(bool distinct, IEnumerable<string> columnNames, IEnumerable<(string ColumnName, object Value)> filters, int? limit, out Row[] result, out Exception exception)
        {
            return Select(null, distinct, columnNames, filters, limit, out result, out exception);
        }

        /// <summary>
        /// Selects the given <paramref name="columnName"/> from the <see cref="Table"/> after the <paramref name="filters"/> 
        /// are applied.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="distinct"></param>
        /// <param name="columnName"></param>
        /// <param name="filters"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the selection did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Select(SQLiteConnection connection, bool distinct, string columnName, IEnumerable<(string ColumnName, object Value)> filters, out object result, out Exception exception)
        {
            string commandText = GetSelectString(distinct, new string[] { columnName }, filters, 1, out (string Placeholder, object Value)[] parameters);

            return ExecuteScalar(connection, commandText, parameters, out result, out exception);
        }

        /// <summary>
        /// Selects the given <paramref name="columnName"/> from the <see cref="Table"/> after the <paramref name="filters"/> 
        /// are applied.
        /// </summary>
        /// <param name="distinct"></param>
        /// <param name="columnName"></param>
        /// <param name="filters"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the selection did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Select(bool distinct, string columnName, IEnumerable<(string ColumnName, object Value)> filters, out object result, out Exception exception)
        {
            return Select(null, distinct, columnName, filters, out result, out exception);
        }

        private string GetSelectString(bool distinct, IEnumerable<string> columnNames, IEnumerable<(string ColumnName, object Value)> filters, int? limit, out (string Placeholder, object Value)[] parameters)
        {
            List<(string Placeholder, object Value)> @params = new List<(string Placeholder, object Value)>();

            Column[] columns = Columns;
            IEnumerable<string> allColumnNames = columns.Select(column => column.Name);

            if (columns == null || !columns.Any())
            {
                throw new InvalidOperationException($"Unable to build Select command. {nameof(Columns)} is null or empty.");
            }

            string result = "SELECT";

            if (distinct)
            {
                result += " DISTINCT";
            }

            for (int i = 0; i < columnNames.Count(); i++)
            {
                string columnName = columnNames.ElementAt(i);

                if (!allColumnNames.Contains(columnName))
                {
                    throw new InvalidOperationException($"Unable to build Select command. {nameof(Columns)} does not contain any " +
                    $"column with the name {columnName}.");
                }

                // table and column names can't be parameterized. So they need to directly put into the string.
                result += " " + columnName;

                if (i != columnNames.Count() - 1)
                {
                    result += ",";
                }
            }

            // table and column names can't be parameterized. So they need to directly put into the string.
            result += " FROM " + Name;

            if (filters.Any())
            {
                result += " WHERE";
            }

            for (int i = 0; i < filters.Count(); i++)
            {
                (string ColumnName, object Value) filter = filters.ElementAt(i);

                result += " " + GetFilterString(GetValuePlaceholder(i), filter, out (string Placeholder, object Value)[] filterParameters);

                if (i != filters.Count() - 1)
                {
                    result += "AND";
                }

                @params.AddRange(filterParameters);
            }

            if (limit.HasValue)
            {
                result += " LIMIT " + limit.Value.ToString();
            }

            result += ";";

            parameters = @params.ToArray();
            return result;
        }

        // currently only supports equals, because that's all we need here
        private string GetFilterString(string valuePlaceholder, (string ColumnName, object Value) filter, out (string Placeholder, object Value)[] parameters)
        {
            parameters = new (string Placeholder, object Value)[]
            {
                (valuePlaceholder, filter.Value)
            };

            // table and column names can't be parameterized. So they need to directly put into the string.
            return filter.ColumnName + " = " + valuePlaceholder;
        }

        /// <summary>
        /// Determines whether a <see cref="Row"/> in the <see cref="Table"/> with the given <paramref name="filters"/> exists.
        /// </summary>
        /// <remarks>
        /// The method will return if the query was successfull - NOT the result of the query. To determine if a <see cref="Row"/> with 
        /// the given <paramref name="filters"/> exists, check the <paramref name="result"/>.
        /// </remarks>
        /// <param name="connection"></param>
        /// <param name="filters"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the exists query did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Exists(SQLiteConnection connection, IEnumerable<(string ColumnName, object Value)> filters, out bool result, out Exception exception)
        {
            string commandText = GetExistsString(filters, out (string Placeholder, object Value)[] parameters);

            bool eval = ExecuteScalar(connection, commandText, parameters, out object scalarResult, out exception);

            if (scalarResult == null)
            {
                result = false;
            }
            else
            {
                result = (bool)Convert.ChangeType(scalarResult, typeof(bool));
            }

            return eval;
        }

        /// <summary>
        /// Determines whether a <see cref="Row"/> in the <see cref="Table"/> with the given <paramref name="filters"/> exists.
        /// </summary>
        /// <remarks>
        /// The method will return if the query was successfull - NOT the result of the query. To determine if a <see cref="Row"/> with 
        /// the given <paramref name="filters"/> exists, check the <paramref name="result"/>.
        /// </remarks>
        /// <param name="filters"></param>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/>, if the exists query did not throw any <see cref="Exception"/>s. 
        /// Otherwise <see langword="false"/>.</returns>
        public bool Exists(IEnumerable<(string ColumnName, object Value)> filters, out bool result, out Exception exception)
        {
            return Exists(null, filters, out result, out exception);
        }

        private string GetExistsString(IEnumerable<(string ColumnName, object Value)> filters, out (string Placeholder, object Value)[] parameters)
        {
            List<(string Placeholder, object Value)> @params = new List<(string Placeholder, object Value)>();
            
            string filterString = string.Empty;

            for (int i = 0; i < filters.Count(); i++)
            {
                (string ColumnName, object Value) filter = filters.ElementAt(i);

                filterString += " " + GetFilterString(GetValuePlaceholder(i), filter, out (string Placeholder, object Value)[] filterParameters);

                if (i != filters.Count() - 1)
                {
                    filterString += "AND";
                }

                @params.AddRange(filterParameters);
            }

            string result = $"SELECT EXISTS( SELECT 1 FROM {Name} WHERE {filterString} );";
            parameters = @params.ToArray();
            return result;
        }
    }
}
