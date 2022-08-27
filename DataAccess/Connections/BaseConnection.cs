using System;
using System.Data.Common;

namespace DevHopTools.DataAccess.Connections
{
    public abstract class BaseConnection<TConnection, TCommand>
    {
        internal string _connectionString;
        internal DbProviderFactory _providerFactory;

        public BaseConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException($"{nameof(connectionString)} is not valid !");

            if (connectionString.Length == 0)
                throw new ArgumentException($"{nameof(connectionString)} is not a valid connection string");

            _connectionString = connectionString;
        }

        internal abstract TCommand CreateCommand(Command command, TConnection dbConnection);
        internal abstract TConnection CreateConnection();
        internal abstract void FinalizeQuery(Command command, TCommand dbCommand);
    }
}
