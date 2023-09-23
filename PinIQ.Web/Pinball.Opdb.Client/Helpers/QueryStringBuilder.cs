using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinball.OpdbClient.Helpers
{
    public class QueryStringBuilder
    {
        private readonly List<KeyValuePair<string, string>> _list;

        public QueryStringBuilder()
        {
            _list = new List<KeyValuePair<string, string>>();
        }

        public void Add(string name, string value)
        {
            _list.Add(new KeyValuePair<string, string>(name, value));
        }

        public override string ToString()
        {
            return string.Join("&", _list.Select(kvp => string.Concat(Uri.EscapeDataString(kvp.Key), "=", Uri.EscapeDataString(kvp.Value))));
        }
    }
}