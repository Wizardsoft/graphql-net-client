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
        private string raw;
        private JObject data;
        private Exception Exception;
        public GraphQLQueryResult(string text, Exception ex = null)
        {
            Exception = ex;
            raw = text;
            data = text != null ? JObject.Parse(text) : null;
        }
        public Exception GetException()
        {
            return Exception;
        }
        public string GetRaw()
        {
            return raw;
        }
        public T Get<T>(string key)
        {
            if (data == null) return default(T);
            try
            {
                return JsonConvert.DeserializeObject<T>(this.data["data"][key].ToString());
            }
            catch
            {
                return default(T);
            }
        }
        public dynamic Get(string key)
        {
            if (data == null) return null;
            try
            {
                return JsonConvert.DeserializeObject<dynamic>(this.data["data"][key].ToString());
            }
            catch
            {
                return null;
            }
        }
        public dynamic GetData()
        {
            if (data == null) return null;
            try
            {
                return JsonConvert.DeserializeObject<dynamic>(this.data["data"].ToString());
            }
            catch
            {
                return null;
            }
        }
    }
}

