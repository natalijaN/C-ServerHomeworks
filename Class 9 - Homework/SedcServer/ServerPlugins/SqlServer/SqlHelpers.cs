using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerPlugins.SqlServer
{
    public class SqlHelpers
    {
        public static string GenerateJsonData(List<Dictionary<string, string>> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(string.Join(",", data.Select(item => GenerateJsonData(item))));
            sb.Append("]");

            return sb.ToString();
        }

        public static string GenerateJsonData(Dictionary<string, string> item)
        {
            Dictionary<string, string> formatItem = new Dictionary<string, string>();
            foreach (var value in item)
            {
                if (value.Value != "")
                {
                    var keyValue = TranformFirstLetterToUpper(value.Key);
                    formatItem.Add(keyValue, value.Value);
                }
            }
            var formatedDictionary = JsonConvert.SerializeObject(formatItem, Formatting.Indented);
            return formatedDictionary;
        }
        public static string TranformFirstLetterToUpper(string value)
        {
            char[] a = value.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
