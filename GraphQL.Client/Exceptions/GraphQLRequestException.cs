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

        public GraphQLRequestException(string message, string response, Exception inner) : base(message, inner) {
            ResponseContents = response;
        }

        public string ResponseContents { get; }

        protected GraphQLRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    
}
