using DevHopTools.Connection.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DevHopTools.Connection
{
    public class Query
    {
        public string Scheme { get; private set; }
        public string Table { get; private set; }

        public string Columns { get; set; }

        public StringBuilder QueryBuilder { get; private set; }

        private Query(string table, string scheme)
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException(nameof(table));
            }

            Table = $"[{table}]";
            Scheme = scheme;
            QueryBuilder = new StringBuilder();
        }

        /// <summary>
        /// Create an instance of <see cref="Query"/> with default scheme being <c>dbo</c>
        /// </summary>
        /// <param name="table">
        /// The table from which the data must be returned
        /// </param>
        /// <param name="scheme">
        /// The table's schema in the database
        /// </param>
        /// <returns>
        /// A new instance of <see cref="Query"/> with <paramref name="table"/> and 
        /// <paramref name="scheme"/> parameters
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Query From(string table, string scheme = "dbo")
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException(nameof(table));
            }

            Query query = new Query(table, scheme);

            return query;
        }

        public void SetColumns()
        {

        }

        public void SetColumns(string columns)
        {

        }

        public IEnumerable<TResult> Execute<TConnection, TCommand, TResult>(
            BaseConnection<TConnection, TCommand> connection,
            Func<IDataRecord, TResult> mapper)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            QueryBuilder.AppendJoin(' ',
                 "SELECT",
                        Columns,
                        "FROM",
                        TableScheme());

            Command command = new Command(QueryBuilder.ToString(), false);

            return connection.ExecuteReader(command, mapper);
        }

        private string TableScheme()
        {
            if (string.IsNullOrEmpty(Scheme))
            {
                return $"[{Table}]";
            }
            else
            {
                return string.Join('.', $"[{Scheme}]", $"{Table}");
            }
        }

    }
}
