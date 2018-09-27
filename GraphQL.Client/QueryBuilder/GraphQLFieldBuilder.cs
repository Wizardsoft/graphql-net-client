using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Client.QueryBuilder
{
    public class GraphQLFieldBuilder
    {
        private readonly string _name;

        private readonly GraphQLSelectBuilder _child = new GraphQLSelectBuilder();

        internal GraphQLFieldBuilder(string name)
        {
            _name = name;
        }

        public void Select(Action<GraphQLSelectBuilder> innerFields) 
            => innerFields(_child);

        internal string Build(int nesting, GraphQLFormatting formatting)
        {
            var spaces = string.Empty;
            var child = string.Empty;
            var newLine = " ";

            if (formatting == GraphQLFormatting.Indented)
            {
                spaces = spaces.PadLeft(nesting * 4);
                newLine = Environment.NewLine;
            }

            if (!_child.Empty)
            {
                child = $" {{{newLine}{_child.Build(nesting + 1, formatting)}{newLine}{spaces}}}";
            }

            return $"{spaces}{_name}{child}";
        }
    }
}