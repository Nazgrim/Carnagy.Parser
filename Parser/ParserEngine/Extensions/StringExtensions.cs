using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserEngine.Extensions
{
    public static class StringExtensions
    {
        public static string FormatFromDictionary(this string formatString, Dictionary<string, object> ValueDict)
        {
            int i = 0;
            var newFormatString = new StringBuilder(formatString);
            var keyToInt = new Dictionary<string, int>();
            foreach (var tuple in ValueDict)
            {
                newFormatString = newFormatString.Replace("{" + tuple.Key + "}", "{" + i.ToString() + "}");
                keyToInt.Add(tuple.Key, i);
                i++;
            }
            return string.Format(newFormatString.ToString(), ValueDict.OrderBy(x => keyToInt[x.Key]).Select(x => x.Value).ToArray());
        }
    }
}
