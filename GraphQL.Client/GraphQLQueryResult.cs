using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL
{
    /// <summary>
    /// The result of a call to a GraphQL API. It has two parts, the data, which may be null if there are errors in the Query, and the Error list if the Query has any.
    /// </summary>
    public class GraphQLQueryResult
    {
        /// <summary>
        /// Creates a new GraphQLQueryResult instance.
        /// </summary>
        /// <param name="text">The result payload as JSON text with the `{ "data": {...}, "errors": [...] }` structure.</param>
        public GraphQLQueryResult(string text, IDictionary<string, string> headers)
        {
            Raw = text ?? throw new ArgumentNullException(nameof(text));
            Headers = headers;

            var jObject = JObject.Parse(text);

            var dataAsDictionary = jObject as IDictionary<string, JToken>;

            if (dataAsDictionary.ContainsKey("data"))
            {
                Data = (JObject)jObject["data"];
            }

            if (dataAsDictionary.ContainsKey("errors"))
            {
                Errors = jObject["errors"].ToObject<IEnumerable<GraphQLQueryError>>(new JsonSerializer
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }

        }

        /// <summary>
        /// The result headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// The raw JSON payload with the `{ "data": {...}, "errors": [...] }` structure.
        /// </summary>
        public string Raw { get; }

        /// <summary>
        /// Result's data part as JObject. It may be null if there are errors, <see cref="GraphQLQueryResult.Errors"/> to check if there were any errors in the Query.
        /// </summary>
        public JObject Data { get; }

        /// <summary>
        /// Gets the result's data part as TResult type. It may be <code>default(TResult)</code> if there were query errors, <see cref="GraphQLQueryResult.Errors"/> to check if there were any errors in the Query.
        /// </summary>
        public TResult DataAs<TResult>(JsonSerializerSettings settings = null)
        {
            if (Data == null)
            {
                return default(TResult);
            }

            if (settings != null)
            {
                return Data.ToObject<TResult>(JsonSerializer.Create(settings));
            }

            return Data.ToObject<TResult>();
        }

        /// <summary>
        /// A detailed list of Query errors.
        /// </summary>
        public IEnumerable<GraphQLQueryError> Errors { get; } = Enumerable.Empty<GraphQLQueryError>();

        [Obsolete]
        public string GetRaw() => Raw;

        [Obsolete]
        public T Get<T>(string key)
        {
            if (Data == null)
            {
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(Data["data"][key].ToString());
            }
            catch
            {
                return default(T);
            }
        }

        [Obsolete]
        public dynamic Get(string key)
        {
            if (Data == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<dynamic>(Data["data"][key].ToString());
            }
            catch
            {
                return null;
            }
        }

        [Obsolete]
        public dynamic GetData()
        {
            if (Data == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<dynamic>(Data["data"].ToString());
            }
            catch
            {
                return null;
            }
        }
    }
}

