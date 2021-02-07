using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinball.OpdbClient.Helpers
{
    public class QueryStringBuilder
    {
        private readonly List<KeyValuePair<string, object>> _list;

        public QueryStringBuilder()
        {
            _list = new List<KeyValuePair<string, object>>();
        }

        public void Add(string name, object value)
        {
            _list.Add(new KeyValuePair<string, object>(name, value));
        }

        public override string ToString()
        {
            return string.Join("&", _list.Select(kvp => string.Concat(Uri.EscapeDataString(kvp.Key), "=", Uri.EscapeDataString(kvp.Value.ToString()))));
        }
    }
}