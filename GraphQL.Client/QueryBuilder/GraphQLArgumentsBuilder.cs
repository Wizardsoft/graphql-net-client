using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Client.QueryBuilder
{
    public class GraphQLArgumentsBuilder
    {
        private readonly IDictionary<string, string> _params = new Dictionary<string, string>();
        private readonly IList<GraphQLVariableBuilder> _variables = new List<GraphQLVariableBuilder>();

        public GraphQLVariableBuilder Var(string name, string defaultValue = "")
        {
            var variable = new GraphQLVariableBuilder(name, defaultValue);

            _variables.Add(variable);

            return variable;
        }

        public string this[string name]{
            set
            {
                _params[name] = value;
            }
        }

        public void Param(string name, string value)
        {
            _params[name] = value;
        }

        internal bool Empty
            => !_variables.Any() && !_params.Any();

        internal string Build()
        {
            var variables = _variables.Select(v => v.Build());
            var parameters = _params.Select(kv => $"{kv.Key}:{kv.Value}");

            return $"{string.Join(", ", variables.Concat(parameters))}";
        }
    }
}