using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// check if string contains spec symbols (not a-z, а-я, 0-9)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckIfNameIsOk(this string str)
        {
            var regexp = new Regex(@"^\w+( {0,1}[\w'])*$", RegexOptions.Compiled);
            return regexp.IsMatch(str ?? "") && !str.ToLowerInvariant().Contains("admin");
        }
    }
}
