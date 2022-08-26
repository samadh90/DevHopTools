using System;
using System.Collections.Generic;
using DevHopTools.DataAccess.Enums;

namespace DevHopTools.DataAccess
{
    public class Command
    {
        /// <summary>
        /// The SQL Query or the stored procedure's name
        /// </summary>
        internal string Query { get; private set; }

        /// <summary>
        /// Boolean declaration if this <see cref="Command"/> is a Stored Procedure or not
        /// </summary>
        internal bool IsStoredProcedure { get; private set; }

        /// <summary>
        /// Dictionnary with key of type <see cref="string"/> and value of type <see cref="Parameter"/>
        /// </summary>
        internal IDictionary<string, Parameter> Parameters { get; private set; }

        /// <summary>
        /// Retrieve the Parameter based on its name
        /// </summary>
        /// <param name="parameterName">The parameter name from <see cref="Parameters"/></param>
        /// <returns>A <see cref="Parameter"/> object from <see cref="Parameters"/> dictionnary</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object this[string parameterName]
        {
            get
            {
                if (!Parameters.ContainsKey(parameterName))
                    throw new InvalidOperationException($"The parameter \"{parameterName}\" does not exist!");

                //if(Parameters[parameterName].Direction == Direction.Input)
                //    throw new InvalidOperationException("the direction is Input, there no change after call the procedure!");

                return Parameters[parameterName].ParameterValue;
            }
        }

        /// <summary>
        /// Create an instance of <see cref="Command"/> object
        /// </summary>
        /// <param name="query">The SQL Query or stored procedure's name.</param>
        /// <param name="isStoredProcedure">
        /// true If <paramref name="query"/> is a stored procedure. false Otherwise.
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        public Command(string query, bool isStoredProcedure = false)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("query is not valid !");

            Query = query;
            IsStoredProcedure = isStoredProcedure;
            Parameters = new Dictionary<string, Parameter>();
        }

        /// <summary>
        /// Add a parameter with it's key and value in the <see cref="Parameters"/> dictionnary
        /// </summary>
        /// <param name="parameterName">Parameter's name for the query</param>
        /// <param name="value">Parameter's value for the query</param>
        public void AddParameter(string parameterName, object value)
        {
            AddParameter(parameterName, value, Direction.Input);
        }

        /// <summary>
        /// Add a parameter with it's key, its value and the direction
        /// </summary>
        /// <param name="parameterName">Parameter key</param>
        /// <param name="value">Parameter's value</param>
        /// <param name="direction">SQL direction. It query is not stored procedure and direction is output,
        /// an exception is thrown.</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddParameter(string parameterName, object value, Direction direction)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                throw new ArgumentException("parameterName is not valid !");

            if (direction == Direction.Output && !IsStoredProcedure)
                throw new ArgumentException("The direction 'Output' can be use only with Stored Procedure!");

            Parameters.Add(parameterName, new Parameter(value ?? DBNull.Value, direction));
        }
    }
}
