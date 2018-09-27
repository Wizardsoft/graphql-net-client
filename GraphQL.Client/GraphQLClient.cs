using GraphQL.Client.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace GraphQL
{
    /// <summary>
    /// A client to access GraphQL apis.
    /// </summary>
    public class GraphQLClient
    {
        private string _url;

        private Action<WebHeaderCollection> _configureHeaders;

        /// <summary>
        /// Creates a new GraphQLClient instance.
        /// </summary>
        /// <param name="url">The GraphQL API's URL</param>
        public GraphQLClient(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Creates a new GraphQLClient instance with a headers configurator delegate.
        /// </summary>
        /// <param name="url">The GraphQL API's URL</param>
        /// <param name="configureHeaders">A delegate that configures the request headers.</param>
        public GraphQLClient(string url, Action<WebHeaderCollection> configureHeaders)
        {
            _url = url;
            _configureHeaders = configureHeaders;
        }

        /// <summary>
        /// Performs a request to the GraphQL API with the given variables.
        /// </summary>
        /// <param name="query">The query to be performed.</param>
        /// <param name="variables">The variables that will be added to the Query.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="GraphQLRequestException">When there's a communication problem.</exception>
        public GraphQLQueryResult Query(string query, object variables = null)
        {
            var fullQuery = new GraphQLQuery()
            {
                query = query,
                variables = variables ?? new object { },
            };

            var jsonContent = JsonConvert.SerializeObject(fullQuery);
            
            var request = WebRequest.Create(_url);
            request.Method = "POST";

            _configureHeaders?.Invoke(request.Headers);

            var encoding = new UTF8Encoding();
            var byteArray = encoding.GetBytes(jsonContent.Trim());

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var json = reader.ReadToEnd();
                        return new GraphQLQueryResult(json);
                    }
                }
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;
                using (var responseStream = errorResponse.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    var errorText = reader.ReadToEnd();
                    throw new GraphQLRequestException(ex.Message, errorText, ex);
                }
            }
        }
    }
}
