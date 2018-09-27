using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL.Client.QueryBuilder
{
    /// <summary>
    /// Utility class to build a GraphQL query.
    /// </summary>
    public sealed class GraphQLQueryBuilder
    {
        private readonly GraphQLQueryBuilder _root;
        private readonly string _objectToQuery;
        private readonly IList<GraphQLQueryBuilder> _children = new List<GraphQLQueryBuilder>();

        private readonly GraphQLArgumentsBuilder _arguments = new GraphQLArgumentsBuilder();
        private readonly GraphQLSelectBuilder _select = new GraphQLSelectBuilder();
        private readonly int _nesting = 0;

        private GraphQLQueryBuilder(
            GraphQLQueryBuilder root,
            string objectToQuery,
            Action<GraphQLArgumentsBuilder> arguments,
            Action<GraphQLSelectBuilder> select, 
            int nesting = 0)
        {
            _objectToQuery = objectToQuery;
            arguments?.Invoke(_arguments);
            select?.Invoke(_select);
            _nesting = nesting;
            _root = root;
        }

        /// <summary>
        /// Builds the collected data and generates the query as a string.
        /// </summary>
        /// <param name="formatting">None: One line, Indented: Human-readable.</param>
        /// <returns>The query as a string.</returns>
        public string Build(GraphQLFormatting formatting = GraphQLFormatting.None)
        {
            if(_root == null)
            {
                return ChildBuild(formatting);
            }

            return _root.Build(formatting);
        }

        internal string ChildBuild(GraphQLFormatting formatting)
        {
            var newLine = " ";
            var nesting = string.Empty;

            if (formatting == GraphQLFormatting.Indented)
            {
                newLine = Environment.NewLine;
                nesting = "".PadLeft(_nesting * 4);
            }

            var builtArguments = _arguments.Empty ? string.Empty : $"({_arguments.Build()})";
            var fields = _select.Build(_nesting + 1, formatting);
            var children = string.Join(newLine, _children.Select(q => q.ChildBuild(formatting)));

            return 
$@"{nesting}{_objectToQuery}{builtArguments} {{{newLine}{fields}{string.Join(newLine, _children.Select(q => q.ChildBuild(formatting)))}{newLine}{nesting}}}";
        }

        /// <summary>
        /// Creates a new query builder instance.
        /// </summary>
        /// <param name="objectToQuery">Object to query.</param>
        /// <param name="arguments">A delegate to build the arguments.</param>
        /// <param name="select">A delegate to build the list of fields to be selected.</param>
        /// <returns>The query builder.</returns>
        public static GraphQLQueryBuilder New(
            string objectToQuery,
            Action<GraphQLArgumentsBuilder> arguments,
            Action<GraphQLSelectBuilder> select)
            => new GraphQLQueryBuilder(null, objectToQuery, arguments, select);

        /// <summary>
        /// Creates a new query builder instance without name.
        /// </summary>
        /// <param name="arguments">A delegate to build the arguments.</param>
        /// <param name="select">A delegate to build the list of fields to be selected.</param>
        /// <returns>The query builder.</returns>
        public static GraphQLQueryBuilder New(
            Action<GraphQLArgumentsBuilder> arguments,
            Action<GraphQLSelectBuilder> select)
            => new GraphQLQueryBuilder(null, string.Empty, arguments, select);

        /// <summary>
        /// Creates a new query builder instance without arguments.
        /// </summary>
        /// <param name="arguments">A delegate to build the arguments.</param>
        /// <param name="select">A delegate to build the list of fields to be selected.</param>
        /// <returns>The query builder.</returns>
        public static GraphQLQueryBuilder New(
            string objectToQuery,
            Action<GraphQLSelectBuilder> select)
            => new GraphQLQueryBuilder(null, objectToQuery, null, select);

        /// <summary>
        /// Creates a new query builder instance without names and arguments.
        /// </summary>
        /// <param name="select">A delegate to build the list of fields to be selected.</param>
        /// <returns>The query builder.</returns>
        public static GraphQLQueryBuilder New(
            Action<GraphQLSelectBuilder> select)
            => new GraphQLQueryBuilder(null, string.Empty, null, select);

        /// <summary>
        /// Creates a new query builder instance without fields to select.
        /// </summary>
        /// <param name="objectToQuery">Object to query.</param>
        /// <param name="arguments">A delegate to build the arguments.</param>
        /// <returns>The query builder.</returns>
        public static GraphQLQueryBuilder New(
            string objectToQuery,
            Action<GraphQLArgumentsBuilder> arguments)
           => new GraphQLQueryBuilder(null, objectToQuery, arguments, null);

        /// <summary>
        /// Creates a new query builder instance.
        /// </summary>
        /// <param name="objectToQuery">Object to query.</param>
        /// <returns>The query builder.</returns>
        public static GraphQLQueryBuilder New(
            string objectToQuery = "")
            => new GraphQLQueryBuilder(null, objectToQuery, null, null);

        /// <summary>
        /// Creates a sub-query
        /// </summary>
        /// <param name="objectToQuery">Inner object to query</param>
        /// <param name="arguments">A delegate to build the arguments.</param>
        /// <param name="select">A delegate to build the list of fields to be selected.</param>
        /// <returns>An inner query builder</returns>
        public GraphQLQueryBuilder Query(
            string objectToQuery,
            Action<GraphQLArgumentsBuilder> arguments,
            Action<GraphQLSelectBuilder> select)
        {
            var childBuilder = new GraphQLQueryBuilder(_root ?? this, objectToQuery, arguments, select, _nesting + 1);
            _children.Add(childBuilder);
            return childBuilder;
        }

        /// <summary>
        /// Creates a sub-query
        /// </summary>
        /// <param name="objectToQuery">Inner object to query</param>
        /// <param name="arguments">A delegate to build the arguments.</param>
        /// <returns>An inner query builder</returns>
        public GraphQLQueryBuilder Query(
            string objectToQuery,
            Action<GraphQLArgumentsBuilder> arguments)
            => Query(objectToQuery, arguments, null);

        /// <summary>
        /// Creates a sub-query
        /// </summary>
        /// <param name="objectToQuery">Inner object to query</param>
        /// <param name="select">A delegate to build the list of fields to be selected.</param>
        /// <returns>An inner query builder</returns>
        public GraphQLQueryBuilder Query(
            string objectToQuery,
            Action<GraphQLSelectBuilder> select)
            => Query(objectToQuery, null, select);
    }
}
