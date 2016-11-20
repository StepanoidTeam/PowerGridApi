using Newtonsoft.Json;
using System;

namespace PowerGridApi
{
    public static class JsonHelper
    {
        public static string ToJson(object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T ToObject<T>(string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object ToObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }

    public static class Extensions
    {

        public static string ToJson(this object @object)
        {
            return JsonHelper.ToJson(@object);
        }


    }

}
