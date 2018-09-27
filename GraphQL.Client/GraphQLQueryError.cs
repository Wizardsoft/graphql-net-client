using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL
{
    /// <summary>
    /// Represents an error in the query.
    /// </summary>
    public sealed class GraphQLQueryError
    {
        /// <summary>
        /// Describes the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The places in the Query where the error is located.
        /// </summary>
        public IEnumerable<ErrorLocation> Locations { get; set; }
    }

    /// <summary>
    /// Represents the location where a query error was detected.
    /// </summary>
    public struct ErrorLocation
    {
        /// <summary>
        /// The line where the error was detected.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The column where the error was detected.
        /// </summary>
        public int Column { get; set; }
    }
}
