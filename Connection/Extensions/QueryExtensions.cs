using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevHopTools.Connection.Extensions
{
    public static class QueryExtensions
    {
        public static Query Select(this Query query)
        {
            query.Columns = "*";
            return query;
        }

        public static Query Select(this Query query, string column)
        {
            query.Columns = column;

            return query;
        }

        public static Query Select(this Query query, params string[] columns)
        {
            query.Columns = string.Join(',', columns.Select(c => $"[{c}]"));

            return query;
        }
    }
}
