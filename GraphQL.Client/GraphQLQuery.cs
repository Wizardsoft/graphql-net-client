namespace GraphQL
{
    internal class GraphQLQuery
    {
        // public string OperationName { get; set; }
        public string query { get; set; }
        public object variables { get; set; }
    }
}
