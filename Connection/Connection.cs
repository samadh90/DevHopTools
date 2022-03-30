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
                        using (DbConnection dbConnection = MSSQL.CreateConnection(_connectionString, _providerFactory))
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
        /// Execute a query and returns the number of row affected
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The number of row affected</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int ExecuteNonQuery(Command command)
        {
            switch (_dbType)
            {
                case DbType.MSSQL: return MSSQL.ExecuteNonQuery(_connectionString, _providerFactory, command);
                case DbType.MySQL: return MySql.ExecuteNonQuery(_connectionString, _providerFactory, command);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
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
                case DbType.MSSQL: return MSSQL.ExecuteReader(_connectionString, _providerFactory, command, selector);
                case DbType.MySQL: return MySql.ExecuteReader(_connectionString, _providerFactory, command, selector);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
            }
        }

        /// <summary>
        /// Execute query and retrieve only one data
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The desired unique object from database</returns>
        public object ExecuteScalar(Command command)
        {
            switch (_dbType)
            {
                case DbType.MSSQL: return MSSQL.ExecuteScalar(_connectionString, _providerFactory, command);
                case DbType.MySQL: return MySql.ExecuteScalar(_connectionString, _providerFactory, command);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
            }
        }

        // TODO : Write a good complete comment
        public DataTable GetDataTable(Command command)
        {
            switch (_dbType)
            {
                case DbType.MSSQL: return MSSQL.GetDataTable(_connectionString, _providerFactory, command);
                case DbType.MySQL: return MySql.GetDataTable(_connectionString, _providerFactory, command);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
            }
        }

        // TODO : Write a good complete comment
        public DataSet GetDataSet(Command command)
        {
            switch (_dbType)
            {
                case DbType.MSSQL: return MSSQL.GetDataSet(_connectionString, _providerFactory, command);
                case DbType.MySQL: return MySql.GetDataSet(_connectionString, _providerFactory, command);
                default:
                    throw new InvalidOperationException($"Provided database type is not valid!");
            }
        }
    }
}
