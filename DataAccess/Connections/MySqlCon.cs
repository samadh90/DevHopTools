using DevHopTools.DataAccess.Enums;
using DevHopTools.DataAccess.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace DevHopTools.DataAccess.Connections
{
    public class MySqlCon : BaseConnection<MySqlConnection, MySqlCommand>, IDevHopConnection
    {
        /// <summary>
        /// MySqlConnection construction with a connection string to the database.
        /// </summary>
        /// <param name="connectionString">Complete database connection string</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public MySqlCon(string connectionString) : base(connectionString)
        {
            _providerFactory = MySqlConnectorFactory.Instance;
        }

        /// <summary>
        /// Execute a query and returns the number of row affected
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <returns>The number of row affected</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int ExecuteNonQuery(Command command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            using (MySqlConnection mySqlConnection = CreateConnection())
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
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="selector">Mapping function to turn a <see cref="IDataRecord"/> to <typeparamref name="TResult"/></param>
        /// <returns>A <see cref="IEnumerable{T}"/> with <c>T</c> being of type <typeparamref name="TResult"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            using (MySqlConnection mySqlConnection = CreateConnection())
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    mySqlConnection.Open();
                    using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
                    {
                        while (mySqlDataReader.Read())
                            yield return selector(mySqlDataReader);
                    }

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, mySqlCommand);
                }
            }
        }

        /// <summary>
        /// Execute a <see cref="Command"/> to retrieve only one fied value from the database
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <returns>A castable object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public object ExecuteScalar(Command command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            using (MySqlConnection mySqlConnection = CreateConnection())
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

        public DataSet GetDataSet(Command command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            using (MySqlConnection mySqlConnection = CreateConnection())
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    using (MySqlDataAdapter mySqlDataAdapter = (MySqlDataAdapter)_providerFactory.CreateDataAdapter())
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

        // TODO : Write a complete comment
        public DataTable GetDataTable(Command command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            using (MySqlConnection mySqlConnection = CreateConnection())
            {
                using (MySqlCommand mySqlCommand = CreateCommand(command, mySqlConnection))
                {
                    using (MySqlDataAdapter mySqlDataAdapter = (MySqlDataAdapter)_providerFactory.CreateDataAdapter())
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

        /// <summary>
        /// Create a <see cref="MySqlCommand"/> object from <paramref name="dbConnection"/> object with keys and values of <paramref name="command"/>
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="dbConnection">A non null and valid <see cref="MySqlCon"/> object</param>
        /// <returns>A new instance of <see cref="MySqlCommand"/></returns>
        internal override MySqlCommand CreateCommand(Command command, MySqlConnection dbConnection)
        {
            MySqlCommand mySqlCommand = dbConnection.CreateCommand();
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
        /// Create an instance of <see cref="MySqlConnection"/> with the connection string and database provider factory.
        /// </summary>
        /// <returns>A <see cref="MySqlConnection"/> object</returns>
        internal override MySqlConnection CreateConnection()
        {
            MySqlConnection dbConnection = (MySqlConnection)_providerFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;

            return dbConnection;
        }

        /// <summary>
        /// Finalize the <paramref name="dbCommand"/> object's Parameters value if <see cref="Direction"/> is <see cref="Direction.Output"/>.
        /// </summary>
        /// <param name="command">Valid object of type <see cref="Command"/></param>
        /// <param name="dbCommand">A valid, not null <see cref="MySqlCommand"/> object</param>
        internal override void FinalizeQuery(Command command, MySqlCommand dbCommand)
        {
            foreach (KeyValuePair<string, Parameter> parameter in command.Parameters)
            {
                if (parameter.Value.Direction == Direction.Output)
                    parameter.Value.ParameterValue = dbCommand.Parameters[parameter.Key].Value;
            }
        }
    }
}
