using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Client.QueryBuilder
{
    public class GraphQLSelectBuilder
    {
        private readonly IList<GraphQLFieldBuilder> _fields = new List<GraphQLFieldBuilder>();

        internal bool Empty => _fields.Count == 0;

        public GraphQLFieldBuilder Field(string name)
        {
            var fieldBuilder = new GraphQLFieldBuilder(name);

            _fields.Add(fieldBuilder);

            return fieldBuilder;
        }

        internal string Build(int nesting = 0, GraphQLFormatting formatting = GraphQLFormatting.None)
        {
            var newLine = " ";

            if (formatting == GraphQLFormatting.Indented)
            {
                newLine = Environment.NewLine;
            }

            return $"{string.Join(newLine, _fields.Select(f => f.Build(nesting, formatting)))}";
        }
    }
}