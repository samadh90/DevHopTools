using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DevHopTools.Connection
{
    /// <summary>
    /// WARNING !
    /// SQLite does not provide the stored procedure concept;
    /// basically, stored procedure means we can prepare a single code, and that code we can reuse again and again as per user requirement. 
    /// When we create the stored procedure, that means once we create the stored procedure that procedure, we can just call in your SQL statement to execute it.
    /// </summary>
    public class SQLiteCo : BaseConnection<SqliteConnection, SqliteCommand>, IConnection
    {
        public SQLiteCo(string connectionString)
        {
            if (_connectionString.Length == 0 || string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException($"{nameof(_connectionString)} is not a valid connection string");
            }

            _connectionString = connectionString;
            _providerFactory = SqliteFactory.Instance;
        }

        public SQLiteCo(string connectionString, DbProviderFactory providerFactory)
        {
            if (_providerFactory is null)
            {
                throw new ArgumentNullException($"{nameof(_providerFactory)}  is not valid !");
            }

            if (_connectionString.Length == 0 || string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException($"{nameof(_connectionString)} is not a valid connection string");
            }

            _connectionString = connectionString;
            _providerFactory = providerFactory;
        }

        public int ExecuteNonQuery(Command command)
        {
            using (SqliteConnection dbConnection = CreateConnection())
            {
                using (SqliteCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    int rows = dbCommand.ExecuteNonQuery();

                    if (command.IsStoredProcedure)
                    {
                        FinalizeQuery(command, dbCommand);
                    }

                    return rows;
                }
            }
        }

        public IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector)
        {
            using (SqliteConnection dbConnection = CreateConnection())
            {
                using (SqliteCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    using (SqliteDataReader dataReader = dbCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            yield return selector(dataReader);
                        }
                    }

                    if (command.IsStoredProcedure)
                    {
                        FinalizeQuery(command, dbCommand);
                    }
                }
            }
        }

        public object ExecuteScalar(Command command)
        {
            using (SqliteConnection dbConnection = CreateConnection())
            {
                using (SqliteCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    dbConnection.Open();
                    object result = dbCommand.ExecuteScalar();

                    if (command.IsStoredProcedure)
                    {
                        FinalizeQuery(command, dbCommand);
                    }

                    return result is DBNull ? null : result;
                }
            }
        }

        public DataSet GetDataSet(Command command)
        {
            using (SqliteConnection dbConnection = CreateConnection())
            {
                using (SqliteCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (var dbDataAdapter = _providerFactory.CreateDataAdapter())
                    {
                        DataSet dataSet = new DataSet();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataSet);

                        if (command.IsStoredProcedure)
                        {
                            FinalizeQuery(command, dbCommand);
                        }

                        return dataSet;
                    }
                }
            }
        }

        public DataTable GetDataTable(Command command)
        {
            using (SqliteConnection dbConnection = CreateConnection())
            {
                using (SqliteCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = _providerFactory.CreateDataAdapter())
                    {
                        DataTable dataTable = new DataTable();
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataTable);

                        if (command.IsStoredProcedure)
                        {
                            FinalizeQuery(command, dbCommand);
                        }

                        return dataTable;
                    }
                }
            }
        }

        internal override SqliteCommand CreateCommand(Command command, SqliteConnection dbConnection)
        {
            SqliteCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = command.Query;

            if (command.IsStoredProcedure)
            {
                dbCommand.CommandType = CommandType.StoredProcedure;
            }

            foreach (KeyValuePair<string, Parameter> parameter in command.Parameters)
            {
                DbParameter dbParameter = dbCommand.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value.ParameterValue;

                if (parameter.Value.Direction == Direction.Output)
                {
                    dbParameter.Direction = ParameterDirection.Output;
                }

                dbCommand.Parameters.Add(dbParameter);
            }

            return dbCommand;
        }

        internal override SqliteConnection CreateConnection()
        {
            SqliteConnection dbConnection = (SqliteConnection)_providerFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;

            return dbConnection;
        }

        internal override void FinalizeQuery(Command command, SqliteCommand dbCommand)
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
