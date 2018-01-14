using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace GraphQL
{
    public class GraphQLClient
    {
        private string _url;

        private Action<WebHeaderCollection> _configureHeaders;

        public GraphQLClient(string url)
        {
            _url = url;
        }

        public GraphQLClient(string url, Action<WebHeaderCollection> configureHeaders)
        {
            _url = url;
            _configureHeaders = configureHeaders;
        }

        public dynamic Query(string query, object variables)
        {
            var fullQuery = new GraphQLQuery()
            {
                query = query,
                variables = variables,
            };
            string jsonContent = JsonConvert.SerializeObject(fullQuery);

            Console.WriteLine(jsonContent);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";

            _configureHeaders?.Invoke(request.Headers);

            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent.Trim());

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
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
