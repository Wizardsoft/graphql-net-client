﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace GraphQL
{
    public class GraphQLQueryResult
    {
        private string raw;
        private JObject data;

        public GraphQLQueryResult(string text)
        {
            raw = text;
            data = text != null ? JObject.Parse(text) : null;
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

