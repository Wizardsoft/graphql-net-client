namespace GraphQL.Client.QueryBuilder
{
    public class GraphQLVariableBuilder
    {
        private readonly string _name;
        private readonly string _defaultValue;

        private string _type = "String";
        private bool _required = false;

        internal GraphQLVariableBuilder(string name, string defaultValue)
        {
            _name = name;
            _defaultValue = defaultValue;
        }

        public GraphQLVariableBuilder String()
            => Object(nameof(String));

        public GraphQLVariableBuilder Int()
            => Object(nameof(Int));

        public GraphQLVariableBuilder Float()
            => Object(nameof(Float));

        public GraphQLVariableBuilder Boolean()
            => Object(nameof(Boolean));

        public GraphQLVariableBuilder ID()
            => Object(nameof(ID));

        public GraphQLVariableBuilder Object(string ofType = "")
        {
            _type = string.IsNullOrWhiteSpace(ofType) ? nameof(Object) : ofType;
            return this;
        }

        public GraphQLVariableBuilder Required()
        {
            _required = true;
            return this;
        }

        internal string Build() 
            => $"${_name}:{_type}{(_required ? "!" : "")}{(string.IsNullOrWhiteSpace(_defaultValue) ? "" : $" = {_defaultValue}")}";
    }
}