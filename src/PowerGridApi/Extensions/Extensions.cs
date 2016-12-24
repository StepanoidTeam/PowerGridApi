using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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

        public static T ToObject<T>(this string json)
        {
            return JsonHelper.ToObject<T>(json);
        }

		public static object ToObject(this string json)
		{
			return JsonHelper.ToObject(json);
		}

		public static ArraySegment<byte> GetByteSegment(this string str)
		{
            return new ArraySegment<byte>(Encoding.UTF8.GetBytes(str));
            //byte[] bytes = new byte[str.Length * sizeof(char)];
			//System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			//return new ArraySegment<byte>(bytes);
		}

        public static Dictionary<string, object> AddItem(this Dictionary<string, object> dict, string key, object value)
        {
            dict.Add(key, value);
            return dict;
        }
    }

}
