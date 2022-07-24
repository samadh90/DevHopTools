using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DevHopTools.Connection
{
    public class MsSqlCon : BaseConnection<DbConnection, DbCommand>, IConnection
    {
        public MsSqlCon(string connectionString)
        {
            if (_connectionString is null)
            {
                throw new ArgumentNullException(nameof(_connectionString));
            }

            if (_connectionString.Length == 0)
            {
                throw new ArgumentException($"{nameof(_connectionString)} is not a valid connection string");
            }

            _connectionString = connectionString;
            _providerFactory = SqlClientFactory.Instance;
        }

        public MsSqlCon(string connectionString, DbProviderFactory providerFactory)
        {
            if (_connectionString is null)
            {
                throw new ArgumentNullException(nameof(_connectionString));
            }

            if (_connectionString.Length == 0)
            {
                throw new ArgumentException($"{nameof(_connectionString)} is not a valid connection string");
            }

            if (_providerFactory is null)
            {
                throw new ArgumentNullException(nameof(_providerFactory));
            }

            _connectionString = connectionString;
            _providerFactory = providerFactory;
        }

        public int ExecuteNonQuery(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
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
            using (DbConnection dbConnection = CreateConnection())
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
                    {
                        FinalizeQuery(command, dbCommand);
                    }
                }
            }
        }

        public object ExecuteScalar(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
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

        public DataTable GetDataTable(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
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

        public DataSet GetDataSet(Command command)
        {
            using (DbConnection dbConnection = CreateConnection())
            {
                using (DbCommand dbCommand = CreateCommand(command, dbConnection))
                {
                    using (DbDataAdapter dbDataAdapter = _providerFactory.CreateDataAdapter())
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

        internal override DbConnection CreateConnection()
        {
            DbConnection dbConnection = _providerFactory.CreateConnection();
            dbConnection.ConnectionString = _connectionString;

            return dbConnection;
        }

        internal override DbCommand CreateCommand(Command command, DbConnection dbConnection)
        {
            DbCommand dbCommand = dbConnection.CreateCommand();
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

        internal override void FinalizeQuery(Command command, DbCommand dbCommand)
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
