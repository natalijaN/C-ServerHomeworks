using System.Collections.Generic;
using System.Text;

namespace ServerInterfaces
{
    public class QueryCollection
    {
        private readonly List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();

        public QueryCollection(List<KeyValuePair<string, string>> initialData)
        {
            data = initialData;
        }

        public static QueryCollection Empty
        {
            get
            {
                return new QueryCollection(new List<KeyValuePair<string, string>>());
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in data)
            {
                sb.AppendLine($"{kvp.Key}={kvp.Value}");
            }
            return sb.ToString();
        }
    }
}