using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.Client.Exceptions
{
    [Serializable]
    public class GraphQLRequestException : Exception
    {
        public GraphQLRequestException() { }

        public GraphQLRequestException(string message) : base(message) { }

        public GraphQLRequestException(string message, Exception inner) : base(message, inner) { }

        public GraphQLRequestException(string message, string response, IDictionary<string, string> responseHeaders, Exception inner) : base(message, inner) {
            ResponseContents = response;
            ResponseHeaders = responseHeaders;
        }

        public string ResponseContents { get; }

        public IDictionary<string, string> ResponseHeaders { get; } = new Dictionary<string, string>();

        protected GraphQLRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    
}
