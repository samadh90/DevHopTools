using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DevHopTools.Connection
{
    internal static class MSSQL
    {
        /// <summary>
        /// Create an instance of <see cref="DbConnection"/> with the connection string and database provider factory
        /// </summary>
        /// <param name="connectionString">Complete connection string</param>
        /// <param name="providerFactory">Valable provider factory</param>
        /// <returns>A <see cref="MySqlConnection"/> object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static DbConnection CreateConnection(string connectionString, DbProviderFactory providerFactory)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));

            DbConnection dbConnection = providerFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;

            return dbConnection;
        }

        /// <summary>
        /// Execute a query and returns the number of row affected
        /// </summary>
        /// <param name="connectionString">Complete database connection string</param>
        /// <param name="providerFactory">Valid object of type <see cref="DbProviderFactory"/></param>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <returns>The number of row affected</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static int ExecuteNonQuery(string connectionString, DbProviderFactory providerFactory, Command command)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));
            if (command is null) throw new ArgumentNullException(nameof(command));

            using (DbConnection dbConnection = CreateConnection(connectionString, providerFactory))
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    int rows = dbCommand.ExecuteNonQuery();

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, dbCommand);

                    return rows;
                }
            }
        }

        /// <summary>
        /// Execute <see cref="Command"/>, retrieve selected data from the database and map it through <paramref name="selector"/>
        /// </summary>
        /// <typeparam name="TResult">Return object type</typeparam>
        /// <param name="connectionString">Complete database connection string</param>
        /// <param name="providerFactory">Valid object of type <see cref="DbProviderFactory"/></param>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="selector">Mapping function to turn a <see cref="IDataRecord"/> to <typeparamref name="TResult"/></param>
        /// <returns>A <see cref="IEnumerable{T}"/> with <c>T</c> being of type <typeparamref name="TResult"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static IEnumerable<TResult> ExecuteReader<TResult>(string connectionString, DbProviderFactory providerFactory,
            Command command, Func<IDataRecord, TResult> selector)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            using (DbConnection dbConnection = CreateConnection(connectionString, providerFactory))
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    using (DbDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            yield return selector(dataReader);
                        }
                    }

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, dbCommand);
                }
            }
        }

        /// <summary>
        /// Execute a <see cref="Command"/> to retrieve only one field value from the database
        /// </summary>
        /// <param name="connectionString">Complete database connection string</param>
        /// <param name="providerFactory">Valid object of type <see cref="DbProviderFactory"/></param>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <returns>A castable object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static object ExecuteScalar(string connectionString, DbProviderFactory providerFactory, Command command)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));
            if (command is null) throw new ArgumentNullException(nameof(command));

            using (DbConnection dbConnection = CreateConnection(connectionString, providerFactory))
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    object result = dbCommand.ExecuteScalar();

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, dbCommand);

                    return result is DBNull ? null : result;
                }
            }
        }

        // TODO : Write a complete comment
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">Complete database connection string</param>
        /// <param name="providerFactory">Valid object of type <see cref="DbProviderFactory"/></param>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static DataTable GetDataTable(string connectionString, DbProviderFactory providerFactory, Command command)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));
            if (command is null) throw new ArgumentNullException(nameof(command));

            using (DbConnection dbConnection = CreateConnection(connectionString, providerFactory))
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = providerFactory.CreateDataAdapter())
                    {
                        DataTable dataTable = new DataTable();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataTable);

                        if (command.IsStoredProcedure)
                            FinalizeQuery(command, dbCommand);

                        return dataTable;
                    }
                }
            }
        }

        // TODO : Write a complete comment
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">Complete database connection string</param>
        /// <param name="providerFactory">Valid object of type <see cref="DbProviderFactory"/></param>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static DataSet GetDataSet(string connectionString, DbProviderFactory providerFactory, Command command)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));
            if (command is null) throw new ArgumentNullException(nameof(command));

            using (DbConnection dbConnection = CreateConnection(connectionString, providerFactory))
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = providerFactory.CreateDataAdapter())
                    {
                        DataSet dataSet = new DataSet();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataSet);

                        if (command.IsStoredProcedure)
                            FinalizeQuery(command, dbCommand);

                        return dataSet;
                    }
                }
            }
        }

        /// <summary>
        /// Create a <see cref="DbCommand"/> object from <paramref name="dbConnection"/> object with keys 
        /// and values of <paramref name="command"/>
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="dbConnection">A non null and valid <see cref="DbConnection"/> object</param>
        /// <returns>A new instance of <see cref="DbCommand"/></returns>
        private static DbCommand CreateCommand(Command command, DbConnection dbConnection)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (dbConnection is null) throw new ArgumentNullException(nameof(dbConnection));

            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            foreach (KeyValuePair<string, Parameter> parameter in command.Parameters)
            {
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value.ParameterValue;

                if (parameter.Value.Direction == Direction.Output)
                    dbParameter.Direction = ParameterDirection.Output;

                dbCommand.Parameters.Add(dbParameter);
            }

            return dbCommand;
        }

        /// <summary>
        /// Finalize the <paramref name="dbCommand"/> object's Parameters value if <see cref="Direction"/> is 
        /// <see cref="Direction.Output"/>
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="dbCommand">A valid, not null <see cref="DbCommand"/> object</param>
        private static void FinalizeQuery(Command command, DbCommand dbCommand)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (dbCommand is null) throw new ArgumentNullException(nameof(dbCommand));

            foreach (KeyValuePair<string, Parameter> parameter in command.Parameters)
            {
                if (parameter.Value.Direction == Direction.Output)
                {
                    parameter.Value.ParameterValue = dbCommand.Parameters[parameter.Key].Value;
                }
            }
        }
    }
}
