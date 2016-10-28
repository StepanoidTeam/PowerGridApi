using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public static class StringExtensions
    {
        /// <summary>
        /// left+right trim and remove all extra spaces from the string (allow only one between words)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveExtraSpaces(this string value)
        {
            var ret = value.Trim();
            for (int i = 0; i < ret.Length-1; i++)
            {
                if (ret.Substring(i, 2) == "  ")
                {
                    ret = ret.Remove(i, 1);
                    i--;
                }
            }
            return ret;
        }

        public static string NormalizeId(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            return str.ToLowerInvariant().Trim().Replace(" ", "");
        }

        public static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
