using GraphQL.Client.Exceptions;
using Newtonsoft.Json;
using System.Linq;
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
        private string _authToken;

        private Action<WebHeaderCollection> _configureHeaders;

        /// <summary>
        /// Creates a new GraphQLClient instance.
        /// </summary>
        /// <param name="url">The GraphQL API's URL</param>

        public GraphQLClient(string url, string authToken = "")
        {
            this._url = url;
            this._authToken = authToken;
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
        public dynamic Query(string query, object variables)
        {
            var fullQuery = new GraphQLQuery()
            {
                query = query,
                variables = variables,
            };
            string jsonContent = JsonConvert.SerializeObject(fullQuery);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";

            if (!string.IsNullOrEmpty(_authToken))
                request.Headers.Add("Authorization", $"Bearer {_authToken}");

            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent.Trim());

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        var json = reader.ReadToEnd();
                        return new GraphQLQueryResult(json);
                    }
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Console.WriteLine(errorText);
                    return new GraphQLQueryResult(null, ex);
                }
            }
        }
    }
}
