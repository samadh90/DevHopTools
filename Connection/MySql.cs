using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DevHopTools.Connection
{
    internal static class MySql
    {
        /// <summary>
        /// Create an instance of <see cref="MySqlConnection"/> with the connection string and database provider factory
        /// </summary>
        /// <param name="connectionString">Complete connection string</param>
        /// <param name="providerFactory">Valable provider factory</param>
        /// <returns>A <see cref="MySqlConnection"/> object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal static MySqlConnection CreateConnection(string connectionString, DbProviderFactory providerFactory)
        {
            if (connectionString is null) throw new ArgumentNullException(nameof(connectionString));
            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");
            if (providerFactory is null) throw new ArgumentNullException(nameof(providerFactory));

            MySqlConnection mySqlConnection = (MySqlConnection)providerFactory.CreateConnection();
            mySqlConnection.ConnectionString = connectionString;

            return mySqlConnection;
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

            using (MySqlConnection mySqlConnection = CreateConnection(connectionString, providerFactory))
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    mySqlConnection.Open();
                    int rows = mySqlCommand.ExecuteNonQuery();

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, mySqlCommand);

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

            using (MySqlConnection mySqlConnection = CreateConnection(connectionString, providerFactory))
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    mySqlConnection.Open();
                    using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (mySqlDataReader.Read())
                        {
                            yield return selector(mySqlDataReader);
                        }
                    }

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, mySqlCommand);
                }
            }
        }

        /// <summary>
        /// Execute a <see cref="Command"/> to retrieve only one fied value from the database
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

            using (MySqlConnection mySqlConnection = CreateConnection(connectionString, providerFactory))
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    mySqlConnection.Open();
                    object result = mySqlCommand.ExecuteScalar();

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, mySqlCommand);

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

            using (MySqlConnection mySqlConnection = CreateConnection(connectionString, providerFactory))
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    using (MySqlDataAdapter mySqlDataAdapter = (MySqlDataAdapter)providerFactory.CreateDataAdapter())
                    {
                        DataTable dataTable = new DataTable();
                        mySqlDataAdapter.SelectCommand = mySqlCommand;
                        mySqlDataAdapter.Fill(dataTable);

                        if (command.IsStoredProcedure)
                            FinalizeQuery(command, mySqlCommand);

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

            using (MySqlConnection mySqlConnection = CreateConnection(connectionString, providerFactory))
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    using (MySqlDataAdapter mySqlDataAdapter = (MySqlDataAdapter)providerFactory.CreateDataAdapter())
                    {
                        DataSet dataSet = new DataSet();
                        mySqlDataAdapter.SelectCommand = mySqlCommand;
                        mySqlDataAdapter.Fill(dataSet);

                        if (command.IsStoredProcedure)
                            FinalizeQuery(command, mySqlCommand);

                        return dataSet;
                    }
                }
            }
        }

        /// <summary>
        /// Create a <see cref="MySqlCommand"/> object from <paramref name="connection"/> object with keys 
        /// and values of <paramref name="command"/>
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="connection">A non null and valid <see cref="MySqlConnection"/> object</param>
        /// <returns>A new instance of <see cref="MySqlCommand"/></returns>
        private static MySqlCommand CreateCommand(Command command, MySqlConnection connection)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (connection is null) throw new ArgumentNullException(nameof(connection));

            MySqlCommand mySqlCommand = connection.CreateCommand();
            mySqlCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
                mySqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (KeyValuePair<string, Parameter> parameter in command.Parameters)
            {
                MySqlParameter mySqlParameter = mySqlCommand.CreateParameter();
                mySqlParameter.ParameterName = parameter.Key;
                mySqlParameter.Value = parameter.Value.ParameterValue;

                if (parameter.Value.Direction == Direction.Output)
                    mySqlParameter.Direction = ParameterDirection.Output;

                mySqlCommand.Parameters.Add(mySqlParameter);
            }

            return mySqlCommand;
        }

        /// <summary>
        /// Finalize the <paramref name="mySqlCommand"/> object's Parameters value if <see cref="Direction"/> is 
        /// <see cref="Direction.Output"/>
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="mySqlCommand">A valid, not null <see cref="MySqlCommand"/> object</param>
        private static void FinalizeQuery(Command command, MySqlCommand mySqlCommand)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            if (mySqlCommand is null) throw new ArgumentNullException(nameof(mySqlCommand));

            foreach (KeyValuePair<string, Parameter> parameter in command.Parameters)
            {
                if (parameter.Value.Direction == Direction.Output)
                {
                    parameter.Value.ParameterValue = mySqlCommand.Parameters[parameter.Key].Value;
                }
            }
        }
    }
}
