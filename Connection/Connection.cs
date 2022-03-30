using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DevHopTools.Connection
{
    public class Connection
    {
        /// <summary>
        /// The connection string to the database
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// TODO : Comment correctly this property
        /// </summary>
        private readonly DbProviderFactory _providerFactory;

        /// <summary>
        /// Specify which type of database is used
        /// </summary>
        private readonly DbType _dbType;

        /// <summary>
        /// Create an instance of the <see cref="Connection"/> taking the <paramref name="connectionString"/>,
        /// <paramref name="providerFactory"/> and <paramref name="dbType"/>
        /// </summary>
        /// <param name="connectionString">The complete connection string with all needed parameters</param>
        /// <param name="providerFactory">The database provider factory</param>
        /// <param name="dbType">The database type used</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Connection(string connectionString, DbProviderFactory providerFactory, DbType dbType)
        {
            if (providerFactory is null)
                throw new ArgumentException("providerFactory is not valid !");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is not valid !");

            _connectionString = connectionString;
            _providerFactory = providerFactory;
            _dbType = dbType;

            try
            {
                switch (_dbType)
                {
                    case DbType.MSSQL:
                        using (DbConnection dbConnection = CreateSqlConnection())
                        {
                            dbConnection.Open();
                        }
                        break;
                    case DbType.MySQL:
                        using (MySqlConnection mySqlConnection = MySql.CreateConnection(_connectionString, _providerFactory))
                        {
                            mySqlConnection.Open();
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"The given database type is not in the list!");
                }


            }
            catch (Exception)
            {
                throw new InvalidOperationException("the connection string is not valid or the server is down...");
            }
        }

        /// <summary>
        /// Create an instance of a configured <see cref="DbConnection"/>
        /// </summary>
        /// <returns>A initialized <see cref="DbConnection"/> object with its connection string</returns>
        private DbConnection CreateSqlConnection()
        {
            DbConnection dbConnection = _providerFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;

            return dbConnection;
        }

        /// <summary>
        /// Create an instance of configured <see cref="MySql"/>
        /// </summary>
        /// <returns>A initialized <see cref="MySql"/> object with its connection string</returns>
        private MySql CreateMySqlConnection()
        {
            MySql mySqlConnection = (MySql)_providerFactory.CreateConnection();
            mySqlConnection.ConnectionString = _connectionString;

            return mySqlConnection;
        }

        /// <summary>
        /// Create an instance of <see cref="DbCommand"/> 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbConnection"></param>
        /// <returns>A new instance of initialized <see cref="DbCommand"/> object</returns>
        private static DbCommand CreateSqlCommand(Command command, DbConnection dbConnection)
        {
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
        /// Execute a query and returns the number of row affected
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The number of row affected</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int ExecuteNonQuery(Command command)
        {
            switch (_dbType)
            {
                case DbType.MSSQL: return ExecuteNonQuery(command);
                case DbType.MySQL: return ExecuteMySqlNonQuery(command);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
            }
        }

        /// <summary>
        /// Execute a query and returns the number of row affected for MSSQL database type
        /// </summary>
        /// <param name="command">An object of type <see cref="Command"/> with the desired query</param>
        /// <returns>The number of rows affected</returns>
        private int ExecuteSqlNonQuery(Command command)
        {
            using (DbConnection dbConnection = CreateSqlConnection())
            {
                using (DbCommand dbCommand = CreateSqlCommand(command, dbConnection))
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
        /// Execute a query and retrieve selected fields values from the database
        /// </summary>
        /// <typeparam name="TResult">The type of the object with want to retrieve</typeparam>
        /// <param name="command">The SQL Command</param>
        /// <param name="selector">Callback function to map the values from database to the desired type</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of type <typeparamref name="TResult"/></returns>
        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            switch (_dbType)
            {
                case DbType.MSSQL: return ExecuteSqlReader(command, selector);
                case DbType.MySQL: return ExecuteMySqlReader(command, selector);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
            }
        }

        /// <summary>
        /// Execute a query and retrieve selected fields values from a Microsoft SQL Database
        /// </summary>
        /// <typeparam name="TResult">The type of the object with want to retrieve</typeparam>
        /// <param name="command">The SQL Command</param>
        /// <param name="selector">Callback function to map the values from database to the desired type</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of type <typeparamref name="TResult"/></returns>
        private IEnumerable<TResult> ExecuteSqlReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            using (DbConnection dbConnection = CreateSqlConnection())
            {
                using (DbCommand dbCommand = CreateSqlCommand(command, dbConnection))
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
        /// Execute query and retrieve only one data
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The desired unique object from database</returns>
        public object ExecuteScalar(Command command)
        {
            using (DbConnection dbConnection = CreateSqlConnection())
            {
                using (DbCommand dbCommand = CreateSqlCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    object result = dbCommand.ExecuteScalar();

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, dbCommand);

                    return result is DBNull ? null : result;
                }
            }
        }

        /// <summary>
        /// The private method of <see cref="ExecuteScalar(Command)"/> for Microsoft SQL databases
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private object ExecuteSqlScalar(Command command)
        {
            using (DbConnection dbConnection = CreateSqlConnection())
            {
                using (DbCommand dbCommand = CreateSqlCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    object result = dbCommand.ExecuteScalar();

                    if (command.IsStoredProcedure)
                        FinalizeQuery(command, dbCommand);

                    return result is DBNull ? null : result;
                }
            }
        }

        public DataTable GetDataTable(Command command)
        {
            using (DbConnection dbConnection = CreateSqlConnection())
            {
                using (DbCommand dbCommand = CreateSqlCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = _providerFactory.CreateDataAdapter())
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

        public DataSet GetDataSet(Command command)
        {
            using (DbConnection dbConnection = CreateSqlConnection())
            {
                using (DbCommand dbCommand = CreateSqlCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = _providerFactory.CreateDataAdapter())
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

        private void FinalizeQuery(Command command, DbCommand dbCommand)
        {
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
