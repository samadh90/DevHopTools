using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DevHopTools.Connection
{
    public abstract class BaseConnection<TConnection, TCommand>
    {
        /// <summary>
        /// The connection string to the database.
        /// </summary>
        internal string _connectionString;

        // TODO : Comment correctly this property
        internal DbProviderFactory _providerFactory;

        internal abstract TCommand CreateCommand(Command command, TConnection dbConnection);
        internal abstract TConnection CreateConnection();
        internal abstract void FinalizeQuery(Command command, TCommand dbCommand);
        public abstract IEnumerable<TResult> ExecuteReader<TResult>(Command command,
            Func<IDataRecord, TResult> selector);
    }
}
